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
    }
}
