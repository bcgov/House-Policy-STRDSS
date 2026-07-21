using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data;
using StrDss.Data.Entities;
using StrDss.Data.Repositories;
using StrDss.Model;

namespace StrDss.Service
{
    public interface IListingActionService
    {
        Task RecordActionAsync(RecordListingActionDto dto);
        (string ShortNm, string LongNm) ResolveDisplayLabels(string actionType, string? takedownReason);
    }

    public class ListingActionService : ServiceBase, IListingActionService
    {
        private readonly IListingActionRepository _listingActionRepo;

        public ListingActionService(
            ICurrentUser currentUser,
            IFieldValidatorService validator,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<StrDssLogger> logger,
            IListingActionRepository listingActionRepo)
            : base(currentUser, validator, unitOfWork, mapper, httpContextAccessor, logger)
        {
            _listingActionRepo = listingActionRepo;
        }

        public async Task RecordActionAsync(RecordListingActionDto dto)
        {
            if (dto.SourceEmailMessageId != null
                && await _listingActionRepo.ActionExistsForEmailAsync(dto.SourceEmailMessageId.Value))
            {
                return;
            }

            var (shortNm, longNm) = ResolveDisplayLabels(dto.ListingActionType, dto.TakedownReason);

            var action = new DssRentalListingAction
            {
                RentalListingId = dto.RentalListingId,
                ListingActionType = dto.ListingActionType,
                ActionDtm = dto.ActionDtm ?? DateTime.UtcNow,
                ActionShortNm = shortNm,
                ActionLongNm = longNm,
                TakedownReason = dto.ListingActionType == ListingActionTypes.PlatformTakedown ? dto.TakedownReason : null,
                InitiatingUserIdentityId = dto.InitiatingUserIdentityId,
                SourceEmailMessageId = dto.SourceEmailMessageId
            };

            await _listingActionRepo.AddActionAsync(action);
        }

        public (string ShortNm, string LongNm) ResolveDisplayLabels(string actionType, string? takedownReason)
        {
            return actionType switch
            {
                ListingActionTypes.NonComplianceNotice => ("notice of non-compliance", "Notice of Non-compliance"),
                ListingActionTypes.TakedownRequest => ("takedown request", "Takedown Request"),
                ListingActionTypes.ComplianceOrder => ("compliance order", "Compliance Order"),
                ListingActionTypes.PlatformTakedown when takedownReason == TakeDownReasonStatus.LGRequest
                    => ("takedown reported", "Takedown Reported: LG Request"),
                ListingActionTypes.PlatformTakedown when takedownReason == TakeDownReasonStatus.InvalidRegistration
                    => ("Reg Check Failed", "Takedown Reported: Registration Check Failed"),
                ListingActionTypes.PlatformTakedown => ("takedown reported", "Takedown Reported"),
                ListingActionTypes.LgTransfer => ("transferred to local government", "Transferred to Local Government"),
                ListingActionTypes.NoticeOfTakedown => ("notice of takedown", "Notice of Takedown"),
                ListingActionTypes.DelistingWarning => ("delisting warning", "Delisting Warning"),
                ListingActionTypes.DelistingRequest => ("delisting request", "Delisting Request"),
                _ => (actionType.ToLowerInvariant(), actionType)
            };
        }
    }
}
