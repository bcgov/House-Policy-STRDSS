namespace StrDss.Api.Models
{
    public class BatchTakedownNoticeCreateDto
    {
        public long PlatformId { get; set; }
        public IFormFile File { get; set; } = null!;
    }
}
