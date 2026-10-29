using OpenTelemetry.Metrics;

namespace StrDss.Data.Telemetry;

public static class RepositoryMetricsExtensions
{
    public static MeterProviderBuilder AddRepositoryMetrics(this MeterProviderBuilder builder)
    {
        return builder
            .AddMeter(RepositoryMetrics.MeterName)
            // The SDK's default first positive boundary is 5 seconds, which groups most repository
            // calls into one bucket and makes latency percentiles unsuitable for finding slow queries.
            .AddView(
                RepositoryMetrics.OperationDurationName,
                new ExplicitBucketHistogramConfiguration
                {
                    Boundaries =
                    [
                        0.01, 0.025, 0.05, 0.1, 0.25,
                        0.5, 1, 2.5, 5, 10, 15, 30, 60, 120,
                    ],
                });
    }
}
