using AutoFixture.Xunit2;
using Moq;
using StrDss.Common;
using StrDss.Data;
using StrDss.Data.Entities;
using StrDss.Data.Repositories;
using StrDss.Model;
using StrDss.Model.OrganizationDtos;
using StrDss.Service;
using Xunit;

namespace StrDss.Test
{
    public class TakedownConfirmationServiceShould
    {
        private const string TakedownHeader = "rpt_period,rpt_type,org_cd,listing_id,reason";
        private const string OrgCd = "ORG1";
        private const string ListingId = "LIST1";
        private const long OrgId = 10;
        private const long RentalListingId = 100;

        private static DssUploadDelivery CreateUpload()
        {
            return new DssUploadDelivery
            {
                UploadDeliveryId = 1,
                ProvidingOrganizationId = OrgId,
                ReportPeriodYm = new DateOnly(2024, 4, 1),
                SourceHeaderTxt = TakedownHeader,
                ProvidingOrganization = new DssOrganization
                {
                    OrganizationId = OrgId,
                    OrganizationNm = "Test Platform"
                }
            };
        }

        private static DssUploadLine CreateUploadLine(string reason = "LG Request")
        {
            return new DssUploadLine
            {
                UploadLineId = 1,
                IncludingUploadDeliveryId = 1,
                SourceLineTxt = $"202404,takedown,{OrgCd},{ListingId},{reason}",
                IsProcessed = false
            };
        }

        private static DssRentalListing CreateListing(bool isTakenDown)
        {
            return new DssRentalListing
            {
                RentalListingId = RentalListingId,
                PlatformListingNo = ListingId,
                IsTakenDown = isTakenDown,
                TakeDownReason = isTakenDown ? TakeDownReasonStatus.LGRequest : null
            };
        }

        [Theory]
        [AutoDomainData]
        public async Task ProcessTakedownConfirmationUploadAsync_WhenListingAlreadyTakenDown_RecordsActionWithoutEmail(
            [Frozen] Mock<IUploadDeliveryRepository> uploadRepoMock,
            [Frozen] Mock<IRentalListingReportRepository> listingRepoMock,
            [Frozen] Mock<IEmailMessageRepository> emailRepoMock,
            [Frozen] Mock<IOrganizationRepository> orgRepoMock,
            [Frozen] Mock<IListingActionService> listingActionServiceMock,
            [Frozen] Mock<IUnitOfWork> unitOfWorkMock,
            TakedownConfirmationService sut)
        {
            var upload = CreateUpload();
            var line = CreateUploadLine();

            uploadRepoMock
                .Setup(x => x.GetUploadLineEntitiesToProcessAsync(upload.UploadDeliveryId))
                .ReturnsAsync(new List<DssUploadLine> { line });

            orgRepoMock
                .Setup(x => x.GetOrganizationByOrgCdAsync(OrgCd))
                .ReturnsAsync(new OrganizationDto { OrganizationId = OrgId });

            listingRepoMock
                .Setup(x => x.GetMasterListingAsync(OrgId, ListingId))
                .ReturnsAsync(CreateListing(isTakenDown: true));

            await sut.ProcessTakedownConfirmationUploadAsync(upload);

            emailRepoMock.Verify(x => x.AddEmailMessage(It.IsAny<DssEmailMessage>()), Times.Never);
            listingActionServiceMock.Verify(
                x => x.RecordActionAsync(It.Is<RecordListingActionDto>(dto =>
                    dto.RentalListingId == RentalListingId
                    && dto.ListingActionType == ListingActionTypes.PlatformTakedown
                    && dto.TakedownReason == TakeDownReasonStatus.LGRequest
                    && dto.SourceEmailMessageId == null)),
                Times.Once);
            Assert.True(line.IsProcessed);
            unitOfWorkMock.Verify(x => x.Commit(), Times.AtLeast(2));
        }

        [Theory]
        [AutoDomainData]
        public async Task ProcessTakedownConfirmationUploadAsync_WhenListingNotTakenDown_SendsEmailAndRecordsAction(
            [Frozen] Mock<IUploadDeliveryRepository> uploadRepoMock,
            [Frozen] Mock<IRentalListingReportRepository> listingRepoMock,
            [Frozen] Mock<IEmailMessageRepository> emailRepoMock,
            [Frozen] Mock<IOrganizationRepository> orgRepoMock,
            [Frozen] Mock<IListingActionService> listingActionServiceMock,
            TakedownConfirmationService sut)
        {
            var upload = CreateUpload();
            var line = CreateUploadLine(TakeDownReasonStatus.InvalidRegistration);
            var listing = CreateListing(isTakenDown: false);

            uploadRepoMock
                .Setup(x => x.GetUploadLineEntitiesToProcessAsync(upload.UploadDeliveryId))
                .ReturnsAsync(new List<DssUploadLine> { line });

            orgRepoMock
                .Setup(x => x.GetOrganizationByOrgCdAsync(OrgCd))
                .ReturnsAsync(new OrganizationDto { OrganizationId = OrgId });

            listingRepoMock
                .Setup(x => x.GetMasterListingAsync(OrgId, ListingId))
                .ReturnsAsync(listing);

            emailRepoMock
                .Setup(x => x.AddEmailMessage(It.IsAny<DssEmailMessage>()))
                .Callback<DssEmailMessage>(email => email.EmailMessageId = 555)
                .Returns(Task.CompletedTask);

            await sut.ProcessTakedownConfirmationUploadAsync(upload);

            emailRepoMock.Verify(
                x => x.AddEmailMessage(It.Is<DssEmailMessage>(email =>
                    email.EmailMessageType == EmailMessageTypes.CompletedTakedown
                    && email.ConcernedWithRentalListingId == RentalListingId)),
                Times.Once);
            Assert.True(listing.IsTakenDown);
            Assert.Equal(TakeDownReasonStatus.InvalidRegistration, listing.TakeDownReason);
            listingActionServiceMock.Verify(
                x => x.RecordActionAsync(It.Is<RecordListingActionDto>(dto =>
                    dto.RentalListingId == RentalListingId
                    && dto.ListingActionType == ListingActionTypes.PlatformTakedown
                    && dto.TakedownReason == TakeDownReasonStatus.InvalidRegistration
                    && dto.SourceEmailMessageId == 555)),
                Times.Once);
            Assert.True(line.IsProcessed);
        }
    }
}
