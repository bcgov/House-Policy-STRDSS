using AutoMapper;
using StrDss.Data.Entities;

namespace StrDss.Data.Mappings
{
    public class EntityToEntityProfile : Profile
    {
        public EntityToEntityProfile()
        {
            CreateMap<DssRentalListing, DssRentalListing>();
        }
    }
}
