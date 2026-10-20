using Microsoft.Extensions.DependencyInjection;

namespace StrDss.Data.Telemetry;

/// <summary>
/// Provides extension methods for registering repository metrics instrumentation
/// in an <see cref="IServiceCollection"/>.
/// </summary>
public static class RepositoryMetricsServiceCollectionExtensions
{
    /// <summary>
    /// Decorates all registered repository services (interfaces whose name ends with
    /// "Repository" and that have a matching implementation type) with a metrics-collecting
    /// dispatch proxy, preserving each registration's original lifetime.
    /// </summary>
    /// <param name="services">The service collection to scan and decorate.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance so that calls can be chained.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is <see langword="null"/>.</exception>
    public static IServiceCollection DecorateRepositoriesWithMetrics(
        this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        var repositoryDescriptors = services
            .Where(IsRepositoryRegistration)
            .ToArray();

        foreach (var descriptor in repositoryDescriptors)
        {
            services.Remove(descriptor);
            services.Add(ServiceDescriptor.Describe(
                descriptor.ServiceType,
                serviceProvider => RepositoryMetricsDispatchProxy.Create(
                    descriptor.ServiceType,
                    CreateTarget(descriptor, serviceProvider)),
                descriptor.Lifetime));
        }

        return services;
    }

    private static bool IsRepositoryRegistration(ServiceDescriptor descriptor)
    {
        return descriptor.ServiceType.IsInterface &&
               descriptor.ServiceType.Name.EndsWith("Repository", StringComparison.Ordinal) &&
               descriptor.ImplementationType?.Name.EndsWith("Repository", StringComparison.Ordinal) == true &&
               !HasMetricsDisabled(descriptor.ImplementationType);
    }

    private static bool HasMetricsDisabled(Type implementationType)
    {
        // Allow a repository implementation to opt out of metrics instrumentation by
        // decorating the class with [DisableMetrics].
        return implementationType.IsDefined(
            typeof(DisableMetricsAttribute),
            inherit: true);
    }

    /// <summary>
    /// Builds the underlying repository instance that the metrics proxy will wrap.
    /// A ServiceDescriptor can specify its implementation in one of three mutually
    /// exclusive ways, so each is handled in order of preference.
    /// </summary>
    /// <param name="descriptor"></param>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private static object CreateTarget(
        ServiceDescriptor descriptor,
        IServiceProvider serviceProvider)
    {
        // Most common case: the registration specifies a concrete type. Use
        // ActivatorUtilities so constructor dependencies are resolved from the provider.
        if (descriptor.ImplementationType is not null)
        {
            return ActivatorUtilities.CreateInstance(serviceProvider, descriptor.ImplementationType);
        }

        // Factory-based registration: invoke the factory to produce the instance.
        if (descriptor.ImplementationFactory is not null)
        {
            return descriptor.ImplementationFactory(serviceProvider);
        }

        // Singleton instance registration: use the pre-built instance directly.
        // If none of the three forms are present, the descriptor cannot be constructed.
        return descriptor.ImplementationInstance
               ?? throw new InvalidOperationException($"Unable to construct {descriptor.ServiceType.Name}.");
    }
}
