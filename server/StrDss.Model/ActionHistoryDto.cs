namespace StrDss.Model
{
    public class ActionHistoryDto
    {
        public string Action { get; set; } = "";
        public DateTime? Date { get; set; }
        public string? User { get; set; };
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
