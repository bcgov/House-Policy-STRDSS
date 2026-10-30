using System.Diagnostics.Metrics;

namespace StrDss.Data.Telemetry;

public static class RepositoryMetrics
{
    public const string MeterName = "StrDss.Repositories";
    public const string OperationDurationName = "strdss.repository.operation.duration";

    public const string MaintenanceStatementDurationName =
        "strdss.repository.maintenance.statement.duration";

    private static readonly Meter Meter = new(MeterName);

    internal static readonly Histogram<double> OperationDuration = Meter.CreateHistogram<double>(
        OperationDurationName,
        unit: "s",
        description: "Duration of repository operations.");

    internal static readonly Counter<long> OperationErrors = Meter.CreateCounter<long>(
        "strdss.repository.operation.errors",
        unit: "{error}",
        description: "Number of repository operation errors.");

    internal static readonly UpDownCounter<long> ActiveOperations = Meter.CreateUpDownCounter<long>(
        "strdss.repository.operation.active",
        unit: "{operation}",
        description: "Number of repository operations currently executing.");

    internal static readonly Histogram<double> MaintenanceStatementDuration = Meter.CreateHistogram<double>(
        MaintenanceStatementDurationName,
        unit: "s",
        description: "Duration of individual repository maintenance SQL statements.");
}
