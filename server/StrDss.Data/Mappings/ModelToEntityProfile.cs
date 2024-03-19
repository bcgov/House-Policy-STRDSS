using AutoMapper;
using StrDss.Data.Entities;
using StrDss.Model.UserDtos;

namespace StrDss.Data.Mappings
{
    public class ModelToEntityProfile : Profile
    {
        public ModelToEntityProfile()
        {
            CreateMap<UserCreateDto, DssUserIdentity>();
            CreateMap<UserDto, DssUserIdentity>()
                .ForMember(o => o.UpdDtm, opt => opt.MapFrom(i => i.ConcurrencyToken))
                ; 
        }
    }
}
