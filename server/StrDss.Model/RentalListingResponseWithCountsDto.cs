using StrDss.Model.RentalReportDtos;

namespace StrDss.Model
{
    public class RentalListingResponseWithCountsDto : PagedDto<RentalListingViewDto>
    {
        public int RecentCount { get; set; }
        public int AllCount { get; set; }
    }

    public class AggregatedListingResponseWithCountsDto
    {
        public List<RentalListingGroupDto> Data { get; set; } = new List<RentalListingGroupDto>();
        public int RecentCount { get; set; }
        public int AllCount { get; set; }
    }
}

