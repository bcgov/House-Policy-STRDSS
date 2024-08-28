using StrDss.Model.RentalReportDtos;

namespace StrDss.Model
{
    public class RentalListingGroupDto
    {
        public string? EffectiveBusinessLicenceNo { get; set; }
        public string? EffectiveHostNm { get; set; }
        public string? MatchAddressTxt { get; set; }
        public List<RentalListingViewDto> Listings { get; set; } = new List<RentalListingViewDto>();
    }
}
