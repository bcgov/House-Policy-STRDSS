using System.Collections;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;
using System.Reflection;
using Hangfire;
using Moq;
using StrDss.Common;
using StrDss.Data.Repositories;
using StrDss.Data.Telemetry;
using StrDss.Service;
using StrDss.Service.Hangfire;
using Xunit;

namespace StrDss.Test
{
    public class LatestReportRefreshShould
    {
        [Fact]
        public void MaintenanceStatements_RefreshThenVacuumAsSeparateCommands()
        {
            Assert.Equal(
                [
                    "REFRESH MATERIALIZED VIEW CONCURRENTLY dss_rental_listing_latest_report_mv",
                    "VACUUM ANALYZE dss_rental_listing_latest_report_mv"
                ],
                RentalListingLatestReportRepository.MaintenanceStatements);

            Assert.DoesNotContain(";", RentalListingLatestReportRepository.RefreshConcurrentlySql);
            Assert.DoesNotContain(";", RentalListingLatestReportRepository.VacuumAnalyzeSql);
            Assert.DoesNotContain("public.", RentalListingLatestReportRepository.RefreshConcurrentlySql, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void MaintenanceStatements_HaveDistinctMetricNames()
        {
            Assert.Equal("s", RepositoryMetrics.MaintenanceStatementDuration.Unit);
            Assert.Equal(
                RepositoryMetrics.MeterName,
                RepositoryMetrics.MaintenanceStatementDuration.Meter.Name);
            Assert.Equal(
                [
                    RentalListingLatestReportRepository.RefreshStatementName,
                    RentalListingLatestReportRepository.VacuumAnalyzeStatementName
                ],
                RentalListingLatestReportRepository.MaintenanceStatements
                    .Select(RentalListingLatestReportRepository.MaintenanceStatementName));
        }

        [Fact]
        public async Task MaintenanceStatements_RecordEachStatementDuration()
        {
            using var measurements = new StatementMeasurementListener();
            var connection = new ScriptedDbConnection(async (sql, cancellationToken) =>
            {
                var delay = sql == RentalListingLatestReportRepository.VacuumAnalyzeSql
                    ? TimeSpan.FromMilliseconds(80)
                    : TimeSpan.FromMilliseconds(10);
                await Task.Delay(delay, cancellationToken);
            });

            await RentalListingLatestReportRepository.ExecuteMaintenanceStatementsAsync(connection);

            Assert.Equal(
                RentalListingLatestReportRepository.MaintenanceStatements,
                connection.Executed);

            var durations = measurements.Durations;
            Assert.Equal(2, durations.Count);
            AssertStatement(durations[0], RentalListingLatestReportRepository.RefreshStatementName, "success");
            AssertStatement(durations[1], RentalListingLatestReportRepository.VacuumAnalyzeStatementName, "success");
            Assert.True(durations[0].Value >= 0);
            Assert.True(durations[1].Value >= 0.04);
        }

        [Fact]
        public async Task MaintenanceStatements_RecordTheFailingStatementAndStop()
        {
            using var measurements = new StatementMeasurementListener();
            var connection = new ScriptedDbConnection((sql, _) =>
            {
                if (sql == RentalListingLatestReportRepository.VacuumAnalyzeSql)
                {
                    throw new InvalidOperationException("vacuum failed");
                }

                return Task.CompletedTask;
            });

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => RentalListingLatestReportRepository.ExecuteMaintenanceStatementsAsync(connection));

            Assert.Equal("vacuum failed", exception.Message);
            Assert.Equal(
                RentalListingLatestReportRepository.MaintenanceStatements,
                connection.Executed);

            var durations = measurements.Durations;
            Assert.Equal(2, durations.Count);
            AssertStatement(durations[0], RentalListingLatestReportRepository.RefreshStatementName, "success");
            AssertStatement(durations[1], RentalListingLatestReportRepository.VacuumAnalyzeStatementName, "error");
            Assert.True(durations[1].Value >= 0);
        }

        [Fact]
        public async Task MaintenanceStatements_DoNotRunVacuumWhenRefreshFails()
        {
            using var measurements = new StatementMeasurementListener();
            var connection = new ScriptedDbConnection((sql, _) =>
                throw new InvalidOperationException($"failed: {sql}"));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => RentalListingLatestReportRepository.ExecuteMaintenanceStatementsAsync(connection));

            Assert.Contains(
                RentalListingLatestReportRepository.RefreshConcurrentlySql,
                exception.Message,
                StringComparison.Ordinal);
            Assert.Equal(
                [RentalListingLatestReportRepository.RefreshConcurrentlySql],
                connection.Executed);

            var duration = Assert.Single(measurements.Durations);
            AssertStatement(duration, RentalListingLatestReportRepository.RefreshStatementName, "error");
        }

        [Fact]
        public void Repository_IsMeasuredThroughItsInterface()
        {
            Assert.EndsWith("Repository", typeof(IRentalListingLatestReportRepository).Name, StringComparison.Ordinal);
            Assert.Null(typeof(IRentalListingLatestReportRepository).GetCustomAttribute<DisableMetricsAttribute>());
            Assert.Null(typeof(RentalListingLatestReportRepository).GetCustomAttribute<DisableMetricsAttribute>());
            Assert.Null(typeof(IRentalListingLatestReportRepository)
                .GetMethod(nameof(IRentalListingLatestReportRepository.RefreshLatestReportPeriodAsync))!
                .GetCustomAttribute<DisableMetricsAttribute>());
        }

        [Fact]
        public async Task HangfireJob_CallsRepositoryOnceAndSkipsADuplicateEnqueue()
        {
            var repository = new Mock<IRentalListingLatestReportRepository>();
            repository.Setup(x => x.RefreshLatestReportPeriodAsync()).Returns(Task.CompletedTask);

            var jobs = new HangfireJobs(
                Mock.Of<IRentalListingReportService>(),
                Mock.Of<IRentalListingService>(),
                Mock.Of<IDelistingService>(),
                Mock.Of<ITakedownConfirmationService>(),
                Mock.Of<IBizLicenceService>(),
                Mock.Of<IUploadDeliveryService>(),
                Mock.Of<IRegistrationService>(),
                repository.Object);

            var method = typeof(HangfireJobs).GetMethod(nameof(HangfireJobs.RefreshLatestRentalListingReportAsync));
            Assert.NotNull(method);
            Assert.NotNull(method!.GetCustomAttribute<SkipSameJobAttribute>());
            Assert.Equal("default", method.GetCustomAttribute<QueueAttribute>()?.Queue);
            Assert.Equal(0, method.GetCustomAttribute<AutomaticRetryAttribute>()?.Attempts);

            await jobs.RefreshLatestRentalListingReportAsync();

            repository.Verify(x => x.RefreshLatestReportPeriodAsync(), Times.Once);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(UploadStatus.Pending)]
        [InlineData(UploadStatus.Failed)]
        public void IncompleteUpload_DoesNotEnqueueRefresh(string? uploadStatus)
        {
            var service = CreateReportService();

            service.EnqueueLatestReportRefreshWhenProcessed(uploadStatus);
        }

        [Fact]
        public void ProcessedListingUpload_EnqueuesRefreshJob()
        {
            var service = CreateReportService();

            var exception = Assert.Throws<InvalidOperationException>(
                () => service.EnqueueLatestReportRefreshWhenProcessed(UploadStatus.Processed));

            Assert.Contains("JobStorage", exception.Message, StringComparison.Ordinal);
        }

        private static void AssertStatement(
            StatementMeasurement measurement,
            string statement,
            string outcome)
        {
            Assert.Equal(
                RepositoryMetrics.MaintenanceStatementDurationName,
                measurement.InstrumentName);
            Assert.Equal(nameof(IRentalListingLatestReportRepository), measurement.Tags["repository"]);
            Assert.Equal(statement, measurement.Tags["statement"]);
            Assert.Equal(outcome, measurement.Tags["outcome"]);
        }

        private static RentalListingReportService CreateReportService()
        {
            return new RentalListingReportService(
                Mock.Of<StrDss.Model.ICurrentUser>(),
                Mock.Of<IFieldValidatorService>(),
                Mock.Of<StrDss.Data.IUnitOfWork>(),
                Mock.Of<AutoMapper.IMapper>(),
                Mock.Of<Microsoft.AspNetCore.Http.IHttpContextAccessor>(),
                Mock.Of<IOrganizationRepository>(),
                Mock.Of<IUploadDeliveryRepository>(),
                Mock.Of<IRentalListingReportRepository>(),
                Mock.Of<IPhysicalAddressRepository>(),
                Mock.Of<StrDss.Service.HttpClients.IGeocoderApi>(),
                Mock.Of<IUserRepository>(),
                Mock.Of<IEmailMessageService>(),
                Mock.Of<IEmailMessageRepository>(),
                Mock.Of<IBizLicenceRepository>(),
                Mock.Of<IPermitValidationService>(),
                Mock.Of<Microsoft.Extensions.Configuration.IConfiguration>(),
                Mock.Of<IListingActionService>(),
                Mock.Of<Microsoft.Extensions.Logging.ILogger<StrDssLogger>>());
        }

        private sealed class StatementMeasurementListener : IDisposable
        {
            private readonly MeterListener _listener = new();

            public List<StatementMeasurement> Durations { get; } = [];

            public StatementMeasurementListener()
            {
                _listener.InstrumentPublished = (instrument, listener) =>
                {
                    if (instrument.Meter.Name == RepositoryMetrics.MeterName &&
                        instrument.Name == RepositoryMetrics.MaintenanceStatementDurationName)
                    {
                        listener.EnableMeasurementEvents(instrument);
                    }
                };

                _listener.SetMeasurementEventCallback<double>(
                    (instrument, value, tags, _) => Durations.Add(
                        new StatementMeasurement(instrument.Name, value, tags)));

                _listener.Start();
            }

            public void Dispose()
            {
                _listener.Dispose();
            }
        }

        private sealed record StatementMeasurement(
            string InstrumentName,
            double Value,
            IReadOnlyDictionary<string, object?> Tags)
        {
            public StatementMeasurement(
                string instrumentName,
                double value,
                ReadOnlySpan<KeyValuePair<string, object?>> tags)
                : this(
                    instrumentName,
                    value,
                    tags.ToArray().ToDictionary(tag => tag.Key, tag => tag.Value))
            {
            }
        }

        private sealed class ScriptedDbConnection : DbConnection
        {
            private readonly Func<string, CancellationToken, Task> _execute;
            private ConnectionState _state = ConnectionState.Closed;

            public ScriptedDbConnection(Func<string, CancellationToken, Task> execute)
            {
                _execute = execute;
            }

            public List<string> Executed { get; } = [];

            [AllowNull]
            public override string ConnectionString { get; set; } = "";

            public override string Database => "strdss";

            public override string DataSource => "fake";

            public override string ServerVersion => "1";

            public override ConnectionState State => _state;

            public override void ChangeDatabase(string databaseName)
            {
            }

            public override void Close() => _state = ConnectionState.Closed;

            public override void Open() => _state = ConnectionState.Open;

            protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) =>
                throw new NotSupportedException();

            protected override DbCommand CreateDbCommand() => new ScriptedDbCommand(this);

            private sealed class ScriptedDbCommand : DbCommand
            {
                private readonly ScriptedDbConnection _connection;

                public ScriptedDbCommand(ScriptedDbConnection connection)
                {
                    _connection = connection;
                    DbParameterCollection = new EmptyParameterCollection();
                }

                [AllowNull]
                public override string CommandText { get; set; } = "";

                public override int CommandTimeout { get; set; }

                public override CommandType CommandType { get; set; }

                public override bool DesignTimeVisible { get; set; }

                public override UpdateRowSource UpdatedRowSource { get; set; }

                protected override DbConnection? DbConnection { get; set; }

                protected override DbParameterCollection DbParameterCollection { get; }

                protected override DbTransaction? DbTransaction { get; set; }

                public override void Cancel()
                {
                }

                public override int ExecuteNonQuery() => 0;

                public override async Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
                {
                    _connection.Executed.Add(CommandText);
                    await _connection._execute(CommandText, cancellationToken);
                    return 0;
                }

                public override object? ExecuteScalar() => null;

                public override void Prepare()
                {
                }

                protected override DbParameter CreateDbParameter() =>
                    throw new NotSupportedException();

                protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) =>
                    throw new NotSupportedException();
            }
        }

        private sealed class EmptyParameterCollection : DbParameterCollection
        {
            public override int Count => 0;

            public override object SyncRoot => this;

            public override int Add(object value) => 0;

            public override void AddRange(Array values)
            {
            }

            public override void Clear()
            {
            }

            public override bool Contains(object value) => false;

            public override bool Contains(string value) => false;

            public override void CopyTo(Array array, int index)
            {
            }

            public override IEnumerator GetEnumerator() => Array.Empty<object>().GetEnumerator();

            public override int IndexOf(object value) => -1;

            public override int IndexOf(string parameterName) => -1;

            public override void Insert(int index, object value)
            {
            }

            public override void Remove(object value)
            {
            }

            public override void RemoveAt(int index)
            {
            }

            public override void RemoveAt(string parameterName)
            {
            }

            protected override DbParameter GetParameter(int index) =>
                throw new NotSupportedException();

            protected override DbParameter GetParameter(string parameterName) =>
                throw new NotSupportedException();

            protected override void SetParameter(int index, DbParameter value)
            {
            }

            protected override void SetParameter(string parameterName, DbParameter value)
            {
            }
        }
    }
}
