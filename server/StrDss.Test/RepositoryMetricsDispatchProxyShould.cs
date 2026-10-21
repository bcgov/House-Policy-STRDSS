using System.Collections.Concurrent;
using System.Diagnostics.Metrics;
using StrDss.Data.Telemetry;
using Xunit;

namespace StrDss.Test;

public class RepositoryMetricsDispatchProxyShould
{
    [Fact]
    public async Task RecordDurationForSuccessfulTaskOperations()
    {
        using var measurements = new RepositoryMeasurementListener();
        var repository = CreateProxy(new TestRepository());

        var result = await repository.GetValueAsync();

        Assert.Equal(42, result);

        var duration = Assert.Single(measurements.DoubleMeasurements.Where(
            measurement => measurement.InstrumentName ==
                           "strdss.repository.operation.duration"));

        Assert.True(duration.Value >= 0);
        Assert.Equal(nameof(ITestRepository), duration.Tags["repository"]);
        Assert.Equal(nameof(ITestRepository.GetValueAsync), duration.Tags["operation"]);
        Assert.Equal("success", duration.Tags["outcome"]);
        Assert.Equal(new long[] { 1L, -1L }, measurements.ActiveOperationValues);
    }

    [Fact]
    public async Task RecordErrorsAndPreserveTheOriginalException()
    {
        using var measurements = new RepositoryMeasurementListener();
        var repository = CreateProxy(new TestRepository());

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            repository.ThrowAsync);

        Assert.Equal("Repository failed.", exception.Message);

        var duration = Assert.Single(measurements.DoubleMeasurements.Where(
            measurement => measurement.InstrumentName ==
                           "strdss.repository.operation.duration"));
        var error = Assert.Single(measurements.LongMeasurements.Where(
            measurement => measurement.InstrumentName ==
                           "strdss.repository.operation.errors"));

        Assert.Equal("error", duration.Tags["outcome"]);
        Assert.Equal(1, error.Value);
        Assert.Equal(nameof(InvalidOperationException), error.Tags["error.type"]);
        Assert.Equal(new long[] { 1L, -1L }, measurements.ActiveOperationValues);
    }

    [Fact]
    public async Task RecordValueTaskOperations()
    {
        using var measurements = new RepositoryMeasurementListener();
        var repository = CreateProxy(new TestRepository());

        var result = await repository.GetValueTaskAsync();

        Assert.Equal(24, result);
        Assert.Contains(
            measurements.DoubleMeasurements,
            measurement =>
                measurement.Tags["operation"].ToString() ==
                nameof(ITestRepository.GetValueTaskAsync) &&
                measurement.Tags["outcome"].ToString() == "success");
    }

    [Fact]
    public async Task NotRecordMetricsForDisabledMethods()
    {
        using var measurements = new RepositoryMeasurementListener();
        var repository = CreateProxy(new TestRepository());

        var result = await repository.GetUnmeasuredValueAsync();

        Assert.Equal(12, result);
        Assert.Empty(measurements.DoubleMeasurements);
        Assert.Empty(measurements.LongMeasurements);
    }

    [Fact]
    public async Task NotRecordMetricsForDisabledRepositoryClasses()
    {
        using var measurements = new RepositoryMeasurementListener();
        var repository = CreateProxy(new UnmeasuredTestRepository());

        var result = await repository.GetValueAsync();

        Assert.Equal(42, result);
        Assert.Empty(measurements.DoubleMeasurements);
        Assert.Empty(measurements.LongMeasurements);
    }

    private static ITestRepository CreateProxy(ITestRepository target)
    {
        return (ITestRepository)RepositoryMetricsDispatchProxy.Create(
            typeof(ITestRepository),
            target);
    }

    public interface ITestRepository
    {
        Task<int> GetValueAsync();
        Task ThrowAsync();
        ValueTask<int> GetValueTaskAsync();
        Task<int> GetUnmeasuredValueAsync();
    }

    private sealed class TestRepository : ITestRepository
    {
        public Task<int> GetValueAsync() => Task.FromResult(42);

        public Task ThrowAsync() =>
            Task.FromException(new InvalidOperationException("Repository failed."));

        public ValueTask<int> GetValueTaskAsync() => ValueTask.FromResult(24);

        [DisableMetrics]
        public Task<int> GetUnmeasuredValueAsync() => Task.FromResult(12);
    }

    [DisableMetrics]
    private sealed class UnmeasuredTestRepository : ITestRepository
    {
        public Task<int> GetValueAsync() => Task.FromResult(42);

        public Task ThrowAsync() => Task.CompletedTask;

        public ValueTask<int> GetValueTaskAsync() => ValueTask.FromResult(24);

        public Task<int> GetUnmeasuredValueAsync() => Task.FromResult(12);
    }

    private sealed class RepositoryMeasurementListener : IDisposable
    {
        private readonly MeterListener _listener = new();

        public ConcurrentQueue<Measurement<double>> DoubleMeasurements { get; } = new();
        public ConcurrentQueue<Measurement<long>> LongMeasurements { get; } = new();

        public long[] ActiveOperationValues => LongMeasurements
            .Where(measurement => measurement.InstrumentName ==
                                  "strdss.repository.operation.active")
            .Select(measurement => measurement.Value)
            .ToArray();

        public RepositoryMeasurementListener()
        {
            _listener.InstrumentPublished = (instrument, listener) =>
            {
                if (instrument.Meter.Name == RepositoryMetrics.MeterName)
                {
                    listener.EnableMeasurementEvents(instrument);
                }
            };

            _listener.SetMeasurementEventCallback<double>(
                (instrument, value, tags, _) => DoubleMeasurements.Enqueue(
                    new Measurement<double>(instrument.Name, value, tags)));

            _listener.SetMeasurementEventCallback<long>(
                (instrument, value, tags, _) => LongMeasurements.Enqueue(
                    new Measurement<long>(instrument.Name, value, tags)));

            _listener.Start();
        }

        public void Dispose()
        {
            _listener.Dispose();
        }
    }

    public sealed record Measurement<T>(
        string InstrumentName,
        T Value,
        IReadOnlyDictionary<string, object?> Tags)
    {
        public Measurement(
            string instrumentName,
            T value,
            ReadOnlySpan<KeyValuePair<string, object?>> tags)
            : this(
                instrumentName,
                value,
                tags.ToArray().ToDictionary(tag => tag.Key, tag => tag.Value))
        {
        }
    }
}
