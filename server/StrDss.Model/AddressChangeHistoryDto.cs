namespace StrDss.Model
{
    public class AddressChangeHistoryDto
    {
        public string Type { get; set; } = string.Empty;
        public string PlatformAddress { get; set; } = string.Empty;
        public string BestMatchAddress { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string User { get; set; } = string.Empty;
    }
}
