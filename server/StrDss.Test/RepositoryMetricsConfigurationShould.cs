using OpenTelemetry;
using OpenTelemetry.Metrics;
using StrDss.Data.Telemetry;
using Xunit;

namespace StrDss.Test;

public class RepositoryMetricsConfigurationShould
{
    [Fact]
    public void ConfigureSecondsBasedOperationDurationBuckets()
    {
        using var exporter = new HistogramBoundsExporter();
        using var provider = Sdk.CreateMeterProviderBuilder()
            .AddRepositoryMetrics()
            .AddReader(new BaseExportingMetricReader(exporter))
            .Build();

        RepositoryMetrics.OperationDuration.Record(0.01);

        Assert.True(provider.ForceFlush());
        Assert.Equal(
            [
                0.01, 0.025, 0.05, 0.1, 0.25,
                0.5, 1, 2.5, 5, 10, 15, 30, 60, 120,
                double.PositiveInfinity,
            ],
            exporter.Bounds);
    }

    private sealed class HistogramBoundsExporter : BaseExporter<Metric>
    {
        public IReadOnlyList<double> Bounds { get; private set; } = [];

        public override ExportResult Export(in Batch<Metric> batch)
        {
            foreach (var metric in batch)
            {
                if (metric.Name != RepositoryMetrics.OperationDurationName)
                {
                    continue;
                }

                foreach (ref readonly var metricPoint in metric.GetMetricPoints())
                {
                    var bounds = new List<double>();
                    foreach (var bucket in metricPoint.GetHistogramBuckets())
                    {
                        bounds.Add(bucket.ExplicitBound);
                    }

                    Bounds = bounds;
                }
            }

            return ExportResult.Success;
        }
    }
}
