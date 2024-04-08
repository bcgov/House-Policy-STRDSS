using AutoMapper;
using StrDss.Data.Entities;
using StrDss.Model.RentalReportDtos;
using StrDss.Model.UserDtos;

namespace StrDss.Data.Mappings
{
    public class ModelToEntityProfile : Profile
    {
        public ModelToEntityProfile()
        {
            CreateMap<UserCreateDto, DssUserIdentity>();
            CreateMap<UserDto, DssUserIdentity>();
            CreateMap<AccessRequestDenyDto, DssUserIdentity>();
            CreateMap<AccessRequestApproveDto, DssUserIdentity>();
            CreateMap<UpdateIsEnabledDto, DssUserIdentity>();

            CreateMap<RentalListingRowUntyped, DssRentalListing>()
                .ForMember(dest => dest.PlatformListingNo, opt => opt.MapFrom(src => src.ListingId))
                .ForMember(dest => dest.PlatformListingUrl, opt => opt.MapFrom(src => src.ListingUrl))
                .ForMember(dest => dest.BusinessLicenceNo, opt => opt.MapFrom(src => src.BusLicNo))
                .ForMember(dest => dest.BcRegistryNo, opt => opt.MapFrom(src => src.BcRegNo))
                .ForMember(dest => dest.IsEntireUnit, opt => opt.MapFrom(src => src.IsEntireUnit))
                .ForMember(dest => dest.AvailableBedroomsQty, opt => opt.MapFrom(src => Convert.ToInt16(src.BedroomsQty)))
                .ForMember(dest => dest.NightsBookedQty, opt => opt.MapFrom(src => Convert.ToInt16(src.NightsBookedQty)))
                .ForMember(dest => dest.SeparateReservationsQty, opt => opt.MapFrom(src => Convert.ToInt16(src.ReservationsQty)));
        }
    }
}
