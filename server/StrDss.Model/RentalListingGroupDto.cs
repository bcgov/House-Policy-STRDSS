using StrDss.Model.RentalReportDtos;

namespace StrDss.Model
{
    public class RentalListingGroupDto
    {
        public string? EffectiveBusinessLicenceNo { get; set; }
        public string? EffectiveHostNm { get; set; }
        public string? MatchAddressTxt { get; set; }
        public string? PrimaryHostNm { get; set; }
        public int NightsBookedYtdQty { get; set; }
        public string? BusinessLicenceNo { get; set; }
        public string? LastActionNm { get; set; }
        public DateTime? LastActionDtm { get; set; }
        public List<RentalListingViewDto> Listings { get; set; } = new List<RentalListingViewDto>();
    }
}
