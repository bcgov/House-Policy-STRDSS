using StrDss.Model.RentalReportDtos;

namespace StrDss.Model
{
    public class AggregatedListingResponseDto
    {
        public List<RentalListingGroupDto> Data { get; set; } = new List<RentalListingGroupDto>();
    }
}
