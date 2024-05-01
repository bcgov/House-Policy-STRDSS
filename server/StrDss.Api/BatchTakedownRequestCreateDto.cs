namespace StrDss.Model.DelistingDtos
{
    public class BatchTakedownRequestCreateDto
    {
        public long PlatformId { get; set; }
        public IFormFile File { get; set; } = null!;
    }
}
