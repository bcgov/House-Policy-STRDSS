namespace StrDss.Api.Models
{
    public class BatchTakedownRequestCreateDto
    {
        public long PlatformId { get; set; }
        public IFormFile File { get; set; } = null!;
    }
}
