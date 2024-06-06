namespace StrDss.Model.DelistingDtos
{
    public class RentalListingForTakedownDto
    {
        public long RentalListingId { get; set; }
        public string OrganizationCd { get; set; } = "";
        public string PlatformListingNo { get; set; } = "";
        public string? PlatformListingUrl { get; set; }
        public long OfferingPlatformId { get; set; }
        public long ProvidingPlatformId { get; set; }
        public List<string> HostEmails { get; set; } = new List<string>();
        public List<string> PlatformEmails { get; set; } = new List<string>();

    }
}
