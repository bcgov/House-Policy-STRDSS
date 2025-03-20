namespace StrDss.Api.Models
{
    public class ValidateRegistrationUploadDto
    {
        public long OrganizationId { get; set; }
        public IFormFile File { get; set; } = null!;
    }
}
