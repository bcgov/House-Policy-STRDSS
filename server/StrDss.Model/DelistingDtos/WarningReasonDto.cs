namespace StrDss.Model.DelistingDtos
{
    public class WarningReasonDto
    {
        public long WarningReasonId { get; set; }
        public string Reason { get; set; } = "";

        public static readonly WarningReasonDto[] WarningReasons = new WarningReasonDto[]
        {
            new WarningReasonDto { WarningReasonId = 1, Reason = "No business license provided" },
            new WarningReasonDto { WarningReasonId = 2, Reason = "Invalid business licence number" },
            new WarningReasonDto { WarningReasonId = 3, Reason = "Expired business licence"},
            new WarningReasonDto { WarningReasonId = 4, Reason = "Suspended business license"}
        };
    }
}
