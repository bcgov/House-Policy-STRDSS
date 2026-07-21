using AutoFixture.Xunit2;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using StrDss.Common;
using StrDss.Data;
using StrDss.Data.Entities;
using StrDss.Data.Repositories;
using StrDss.Model;
using StrDss.Service;
using Xunit;

namespace StrDss.Test
{
    public class ListingActionServiceShould
    {
        private static ListingActionService CreateService()
        {
            return new ListingActionService(
                Mock.Of<ICurrentUser>(),
                Mock.Of<IFieldValidatorService>(),
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IMapper>(),
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<ILogger<StrDssLogger>>(),
                Mock.Of<IListingActionRepository>());
        }

        [Theory]
        [InlineData(ListingActionTypes.NonComplianceNotice, null, "notice of non-compliance", "Notice of Non-compliance")]
        [InlineData(ListingActionTypes.TakedownRequest, null, "takedown request", "Takedown Request")]
        [InlineData(ListingActionTypes.ComplianceOrder, null, "compliance order", "Compliance Order")]
        [InlineData(ListingActionTypes.LgTransfer, null, "transferred to local government", "Transferred to Local Government")]
        [InlineData(ListingActionTypes.NoticeOfTakedown, null, "notice of takedown", "Notice of Takedown")]
        [InlineData(ListingActionTypes.DelistingWarning, null, "delisting warning", "Delisting Warning")]
        [InlineData(ListingActionTypes.DelistingRequest, null, "delisting request", "Delisting Request")]
        [InlineData(ListingActionTypes.PlatformTakedown, TakeDownReasonStatus.LGRequest, "takedown reported", "Takedown Reported: LG Request")]
        [InlineData(ListingActionTypes.PlatformTakedown, TakeDownReasonStatus.InvalidRegistration, "Reg Check Failed", "Takedown Reported: Registration Check Failed")]
        [InlineData(ListingActionTypes.PlatformTakedown, null, "takedown reported", "Takedown Reported")]
        public void ResolveDisplayLabels_ReturnsExpectedLabels(
            string actionType,
            string? takedownReason,
            string expectedShortNm,
            string expectedLongNm)
        {
            var sut = CreateService();
            var (shortNm, longNm) = sut.ResolveDisplayLabels(actionType, takedownReason);

            Assert.Equal(expectedShortNm, shortNm);
            Assert.Equal(expectedLongNm, longNm);
        }

        [Theory]
        [AutoDomainData]
        public async Task RecordActionAsync_WhenActionExistsForEmail_DoesNotAddAction(
            long emailMessageId,
            [Frozen] Mock<IListingActionRepository> listingActionRepoMock,
            ListingActionService sut)
        {
            listingActionRepoMock
                .Setup(x => x.ActionExistsForEmailAsync(emailMessageId))
                .ReturnsAsync(true);

            await sut.RecordActionAsync(new RecordListingActionDto
            {
                RentalListingId = 1,
                ListingActionType = ListingActionTypes.NonComplianceNotice,
                SourceEmailMessageId = emailMessageId
            });

            listingActionRepoMock.Verify(x => x.AddActionAsync(It.IsAny<DssRentalListingAction>()), Times.Never);
        }

        [Theory]
        [AutoDomainData]
        public async Task RecordActionAsync_WhenActionDoesNotExist_AddsActionWithResolvedLabels(
            [Frozen] Mock<IListingActionRepository> listingActionRepoMock,
            ListingActionService sut)
        {
            listingActionRepoMock
                .Setup(x => x.ActionExistsForEmailAsync(It.IsAny<long>()))
                .ReturnsAsync(false);

            DssRentalListingAction? capturedAction = null;
            listingActionRepoMock
                .Setup(x => x.AddActionAsync(It.IsAny<DssRentalListingAction>()))
                .Callback<DssRentalListingAction>(action => capturedAction = action)
                .Returns(Task.CompletedTask);

            var actionDtm = new DateTime(2024, 4, 15, 12, 0, 0, DateTimeKind.Utc);

            await sut.RecordActionAsync(new RecordListingActionDto
            {
                RentalListingId = 42,
                ListingActionType = ListingActionTypes.ComplianceOrder,
                ActionDtm = actionDtm,
                SourceEmailMessageId = 99
            });

            listingActionRepoMock.Verify(x => x.AddActionAsync(It.IsAny<DssRentalListingAction>()), Times.Once);
            Assert.NotNull(capturedAction);
            Assert.Equal(42, capturedAction!.RentalListingId);
            Assert.Equal(ListingActionTypes.ComplianceOrder, capturedAction.ListingActionType);
            Assert.Equal("compliance order", capturedAction.ActionShortNm);
            Assert.Equal("Compliance Order", capturedAction.ActionLongNm);
            Assert.Equal(actionDtm, capturedAction.ActionDtm);
            Assert.Null(capturedAction.TakedownReason);
            Assert.Equal(99, capturedAction.SourceEmailMessageId);
        }

        [Theory]
        [AutoDomainData]
        public async Task RecordActionAsync_ForPlatformTakedown_PersistsTakedownReason(
            [Frozen] Mock<IListingActionRepository> listingActionRepoMock,
            ListingActionService sut)
        {
            listingActionRepoMock
                .Setup(x => x.ActionExistsForEmailAsync(It.IsAny<long>()))
                .ReturnsAsync(false);

            DssRentalListingAction? capturedAction = null;
            listingActionRepoMock
                .Setup(x => x.AddActionAsync(It.IsAny<DssRentalListingAction>()))
                .Callback<DssRentalListingAction>(action => capturedAction = action)
                .Returns(Task.CompletedTask);

            await sut.RecordActionAsync(new RecordListingActionDto
            {
                RentalListingId = 7,
                ListingActionType = ListingActionTypes.PlatformTakedown,
                TakedownReason = TakeDownReasonStatus.InvalidRegistration
            });

            Assert.NotNull(capturedAction);
            Assert.Equal(TakeDownReasonStatus.InvalidRegistration, capturedAction!.TakedownReason);
            Assert.Equal("Reg Check Failed", capturedAction.ActionShortNm);
            Assert.Equal("Takedown Reported: Registration Check Failed", capturedAction.ActionLongNm);
        }
    }
}
