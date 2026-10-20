namespace StrDss.Data.Telemetry;

/// <summary>
/// Suppresses telemetry metrics collection for the decorated type or method.
/// </summary>
/// <remarks>
/// When applied to a class, metrics are disabled for all of its methods;
/// when applied to a single method, only that method is excluded.
/// Apply this attribute to the concrete interface implementation type
/// (the class that implements the interface), not to the interface itself,
/// because attribute usage is not inherited from interface declarations.
/// </remarks>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Method,
    Inherited = true,
    AllowMultiple = false)]
internal sealed class DisableMetricsAttribute : Attribute
{
}
