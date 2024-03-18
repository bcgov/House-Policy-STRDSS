using AutoMapper;
using StrDss.Data.Entities;
using StrDss.Model.UserDtos;

namespace StrDss.Data.Mappings
{
    public class EntityToModelProfile : Profile
    {
        public EntityToModelProfile()
        {
            CreateMap<DssUserIdentity, AccessRequestDto>()
                .ForMember(o => o.OrganizationType, opt => opt.MapFrom(i => i.RepresentedByOrganization == null ? "" : i.RepresentedByOrganization.OrganizationType))
                .ForMember(o => o.OrganizationCd, opt => opt.MapFrom(i => i.RepresentedByOrganization == null ? "" : i.RepresentedByOrganization.OrganizationCd))
                .ForMember(o => o.OrganizationNm, opt => opt.MapFrom(i => i.RepresentedByOrganization == null ? "" : i.RepresentedByOrganization.OrganizationNm))
                ;
        }
    }
}
