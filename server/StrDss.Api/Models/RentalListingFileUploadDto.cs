namespace StrDss.Api.Models
{
    public class RentalListingFileUploadDto
    {
        public string ReportPeriod { get; set; } = "";
        public long OrganizationId { get; set; }
        public IFormFile File { get; set; } = null!;
    }
}
