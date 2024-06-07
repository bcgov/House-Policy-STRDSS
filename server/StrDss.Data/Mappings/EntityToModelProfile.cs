using AutoMapper;
using StrDss.Common;
using StrDss.Data.Entities;
using StrDss.Model;
using StrDss.Model.OrganizationDtos;
using StrDss.Model.RentalReportDtos;
using StrDss.Model.UserDtos;

namespace StrDss.Data.Mappings
{
    public class EntityToModelProfile : Profile
    {
        public EntityToModelProfile()
        {
            CreateMap<DssUserIdentity, UserListtDto>()
                .ForMember(o => o.OrganizationType, opt => opt.MapFrom(i => i.RepresentedByOrganization == null ? "" : i.RepresentedByOrganization.OrganizationType))
                .ForMember(o => o.OrganizationCd, opt => opt.MapFrom(i => i.RepresentedByOrganization == null ? "" : i.RepresentedByOrganization.OrganizationCd))
                .ForMember(o => o.OrganizationNm, opt => opt.MapFrom(i => i.RepresentedByOrganization == null ? "" : i.RepresentedByOrganization.OrganizationNm))
                ;

            CreateMap<DssUserIdentity, UserDto>();
            CreateMap<DssOrganization, OrganizationDto>()
                .ForMember(o => o.ContactPeople, opt => opt.MapFrom(i => i.DssOrganizationContactPeople))
                ;
            CreateMap<DssOrganizationType, OrganizationTypeDto>();
            CreateMap<DssOrganizationContactPerson, ContactPersonDto>();
            CreateMap<DssAccessRequestStatus, AccessRequestStatusDto>();
            CreateMap<DssUserRole, RoleDto>();
            CreateMap<DssUserPrivilege, PermissionDto>();
            CreateMap<DssUserIdentityView, UserListtDto>();

            CreateMap<DssRentalUploadHistoryView, RentalUploadHistoryViewDto>()
                .ForMember(o => o.ReportPeriodYM, opt => opt.MapFrom(i => ((DateOnly)i.ReportPeriodYm!).ToString("yyyy-MM")))
                .ForMember(o => o.UpdDtm, opt => opt.MapFrom(i => DateUtils.ConvertUtcToPacificTime((DateTime)i.UpdDtm!)))
                ;

            CreateMap<DssRentalListingVw, RentalListingViewDto>();
            CreateMap<DssRentalListingContact, RentalListingContactDto>();
        }
    }
}
