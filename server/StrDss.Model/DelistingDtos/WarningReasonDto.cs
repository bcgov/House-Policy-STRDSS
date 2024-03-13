namespace StrDss.Model.DelistingDtos
{
    public class WarningReasonDto
    {
        public long RequestReasonId { get; set; }
        public string Reason { get; set; } = "";

        public static readonly WarningReasonDto[] RequestReasons = new WarningReasonDto[]
        {
            new WarningReasonDto { RequestReasonId = 1, Reason = "No business license provided" },
            new WarningReasonDto { RequestReasonId = 2, Reason = "Invalid business licence number" },
            new WarningReasonDto { RequestReasonId = 3, Reason = "Expired business licence"},
            new WarningReasonDto { RequestReasonId = 4, Reason = "Suspended business license"}
        };
    }
}
