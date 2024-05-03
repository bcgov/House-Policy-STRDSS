namespace StrDss.Api.Models
{
    public class BatchTakedownNoticeCreateDto
    {
        public long PlatformId { get; set; }
        public string LgName { get; set; }
        public IFormFile File { get; set; } = null!;
    }
}
