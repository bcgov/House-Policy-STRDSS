using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace StrDss.Data.Telemetry;

/// <summary>
/// A <see cref="DispatchProxy"/> that wraps a repository interface implementation and records
/// telemetry (timing, success/failure counts and active operation counts) for every method call
/// routed through the interface.
/// </summary>
/// <remarks>
/// <para>
/// A separate proxy instance is created per target repository instance via
/// <see cref="Create(Type, object)"/>. Each instance captures the wrapped target, the repository
/// (interface) name used for metric labels, and the resolved per-method metadata needed to decide
/// whether metrics should be collected.
/// </para>
/// <para>
/// Metric collection can be suppressed for an entire repository or for individual methods by
/// applying the disable-metrics attribute to the interface, the implementation type, or a specific
/// method. Synchronous, <see cref="Task"/>, <see cref="Task{TResult}"/>, <see cref="ValueTask"/>
/// and <see cref="ValueTask{TResult}"/> return types are all supported, with asynchronous results
/// being awaited so that timing reflects the full operation duration.
/// </para>
/// <para>
/// The static reflection caches are shared across all instances and are therefore thread-safe,
/// while the per-instance state is populated once during <see cref="Create(Type, object)"/> and
/// treated as read-only thereafter.
/// </para>
/// </remarks>
public class RepositoryMetricsDispatchProxy : DispatchProxy
{
    private static readonly MethodInfo AwaitTaskWithResultMethod =
        GetWrapperMethod(nameof(AwaitTaskWithResult));

    private static readonly MethodInfo AwaitValueTaskWithResultMethod =
        GetWrapperMethod(nameof(AwaitValueTaskWithResult));

    private static readonly ConcurrentDictionary<Type, MethodInfo> TaskResultWrappers = new();
    private static readonly ConcurrentDictionary<Type, MethodInfo> ValueTaskResultWrappers = new();

    private object _target = null!;
    private string _repositoryName = null!;
    private bool _repositoryMetricsDisabled;
    private IReadOnlyDictionary<MethodInfo, MethodInfo> _implementationMethods = null!;
    private readonly ConcurrentDictionary<MethodInfo, bool> _metricsDisabledCache = new();

    /// <summary>
    /// Creates a telemetry-collecting proxy for the specified repository interface that forwards
    /// all calls to <paramref name="target"/>.
    /// </summary>
    /// <param name="repositoryInterface">
    /// The repository interface to proxy. Must be an interface type that <paramref name="target"/>
    /// implements.
    /// </param>
    /// <param name="target">
    /// The underlying repository implementation that actually handles the calls.
    /// </param>
    /// <returns>
    /// An object that implements <paramref name="repositoryInterface"/> and records metrics around
    /// each invocation before delegating to <paramref name="target"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="repositoryInterface"/> or <paramref name="target"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="repositoryInterface"/> is not an interface, or <paramref name="target"/>
    /// does not implement it.
    /// </exception>
    public static object Create(Type repositoryInterface, object target)
    {
        ArgumentNullException.ThrowIfNull(repositoryInterface);
        ArgumentNullException.ThrowIfNull(target);

        if (!repositoryInterface.IsInterface)
        {
            throw new ArgumentException(
                $"{repositoryInterface.Name} must be an interface.",
                nameof(repositoryInterface));
        }

        if (!repositoryInterface.IsInstanceOfType(target))
        {
            throw new ArgumentException(
                $"{target.GetType().Name} does not implement {repositoryInterface.Name}.",
                nameof(target));
        }

        var proxy = (RepositoryMetricsDispatchProxy)DispatchProxy.Create(
            repositoryInterface,
            typeof(RepositoryMetricsDispatchProxy));

        proxy._target = target;
        proxy._repositoryName = repositoryInterface.Name;
        proxy._repositoryMetricsDisabled =
            HasDisableMetricsAttribute(repositoryInterface) ||
            HasDisableMetricsAttribute(target.GetType());
        proxy._implementationMethods = GetImplementationMethods(
            repositoryInterface,
            target.GetType());

        return proxy;
    }

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        ArgumentNullException.ThrowIfNull(targetMethod);

        if (MetricsAreDisabled(targetMethod))
        {
            return InvokeTarget(targetMethod, args);
        }

        var operationName = targetMethod.Name;
        var started = Stopwatch.GetTimestamp();

        ChangeActiveOperations(operationName, 1);

        try
        {
            var result = targetMethod.Invoke(_target, args);
            var returnType = targetMethod.ReturnType;

            // Non-generic async method: Task Method(args)
            if (returnType == typeof(Task))
            {
                return AwaitTask((Task)result!, started, operationName);
            }

            // Generic async method: Task<TResult> Method(args)
            if (returnType.IsGenericType &&
                returnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                var resultType = returnType.GenericTypeArguments[0];
                var wrapper = TaskResultWrappers.GetOrAdd(
                    resultType,
                    static type => AwaitTaskWithResultMethod.MakeGenericMethod(type));

                return wrapper.Invoke(this, [result!, started, operationName]);
            }

            // Non-generic value-task async method: ValueTask Method(args)
            if (returnType == typeof(ValueTask))
            {
                return AwaitValueTask((ValueTask)result!, started, operationName);
            }

            // Generic value-task async method: ValueTask<TResult> Method(args)
            if (returnType.IsGenericType &&
                returnType.GetGenericTypeDefinition() == typeof(ValueTask<>))
            {
                var resultType = returnType.GenericTypeArguments[0];
                var wrapper = ValueTaskResultWrappers.GetOrAdd(
                    resultType,
                    static type => AwaitValueTaskWithResultMethod.MakeGenericMethod(type));

                return wrapper.Invoke(this, [result!, started, operationName]);
            }

            // Synchronous method: TResult Method(args) (or void)
            RecordSuccess(started, operationName);
            return result;
        }
        catch (TargetInvocationException exception) when (exception.InnerException is not null)
        {
            RecordFailure(started, operationName, exception.InnerException);
            ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
            throw;
        }
        catch (Exception exception)
        {
            RecordFailure(started, operationName, exception);
            throw;
        }
    }

    private object? InvokeTarget(MethodInfo targetMethod, object?[]? args)
    {
        try
        {
            return targetMethod.Invoke(_target, args);
        }
        catch (TargetInvocationException exception) when (exception.InnerException is not null)
        {
            ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
            throw;
        }
    }

    private bool MetricsAreDisabled(MethodInfo interfaceMethod)
    {
        if (_repositoryMetricsDisabled)
        {
            return true;
        }

        return _metricsDisabledCache.GetOrAdd(interfaceMethod, ResolveMetricsDisabled);
    }

    private bool ResolveMetricsDisabled(MethodInfo interfaceMethod)
    {
        if (HasDisableMetricsAttribute(interfaceMethod))
        {
            return true;
        }

        return _implementationMethods.TryGetValue(interfaceMethod, out var implementationMethod) &&
               HasDisableMetricsAttribute(implementationMethod);
    }

    private async Task AwaitTask(Task task, long started, string operationName)
    {
        try
        {
            await task.ConfigureAwait(false);
            RecordSuccess(started, operationName);
        }
        catch (Exception exception)
        {
            RecordFailure(started, operationName, exception);
            throw;
        }
    }

    private async Task<TResult> AwaitTaskWithResult<TResult>(
        Task<TResult> task,
        long started,
        string operationName)
    {
        try
        {
            var result = await task.ConfigureAwait(false);
            RecordSuccess(started, operationName);
            return result;
        }
        catch (Exception exception)
        {
            RecordFailure(started, operationName, exception);
            throw;
        }
    }

    private async ValueTask AwaitValueTask(
        ValueTask task,
        long started,
        string operationName)
    {
        try
        {
            await task.ConfigureAwait(false);
            RecordSuccess(started, operationName);
        }
        catch (Exception exception)
        {
            RecordFailure(started, operationName, exception);
            throw;
        }
    }

    private async ValueTask<TResult> AwaitValueTaskWithResult<TResult>(
        ValueTask<TResult> task,
        long started,
        string operationName)
    {
        try
        {
            var result = await task.ConfigureAwait(false);
            RecordSuccess(started, operationName);
            return result;
        }
        catch (Exception exception)
        {
            RecordFailure(started, operationName, exception);
            throw;
        }
    }

    private void RecordSuccess(long started, string operationName)
    {
        RecordDuration(started, operationName, "success");
        ChangeActiveOperations(operationName, -1);
    }

    private void RecordFailure(long started, string operationName, Exception exception)
    {
        RecordDuration(started, operationName, "error");

        var tags = new TagList
        {
            { "repository", _repositoryName },
            { "operation", operationName },
            { "error.type", exception.GetType().Name },
        };

        RepositoryMetrics.OperationErrors.Add(1, tags);
        ChangeActiveOperations(operationName, -1);
    }

    private void RecordDuration(long started, string operationName, string outcome)
    {
        var tags = new TagList
        {
            { "repository", _repositoryName },
            { "operation", operationName },
            { "outcome", outcome },
        };

        RepositoryMetrics.OperationDuration.Record(
            Stopwatch.GetElapsedTime(started).TotalSeconds,
            tags);
    }

    private void ChangeActiveOperations(string operationName, long value)
    {
        var tags = new TagList
        {
            { "repository", _repositoryName },
            { "operation", operationName },
        };

        RepositoryMetrics.ActiveOperations.Add(value, tags);
    }

    private static MethodInfo GetWrapperMethod(string name)
    {
        return typeof(RepositoryMetricsDispatchProxy).GetMethod(
                   name,
                   BindingFlags.Instance | BindingFlags.NonPublic)
               ?? throw new InvalidOperationException($"Unable to find proxy wrapper method {name}.");
    }

    private static IReadOnlyDictionary<MethodInfo, MethodInfo> GetImplementationMethods(
        Type repositoryInterface,
        Type implementationType)
    {
        var interfaceMap = implementationType.GetInterfaceMap(repositoryInterface);

        return interfaceMap.InterfaceMethods
            .Select((method, index) => new
            {
                InterfaceMethod = method,
                ImplementationMethod = interfaceMap.TargetMethods[index],
            })
            .ToDictionary(
                pair => pair.InterfaceMethod,
                pair => pair.ImplementationMethod);
    }

    private static bool HasDisableMetricsAttribute(MemberInfo member)
    {
        return member.IsDefined(
            typeof(DisableMetricsAttribute),
            inherit: true);
    }
}
