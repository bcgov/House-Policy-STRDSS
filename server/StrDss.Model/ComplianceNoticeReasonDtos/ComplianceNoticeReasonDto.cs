namespace StrDss.Model.ComplianceNoticeReasonDtos
{
    public class ComplianceNoticeReasonDto
    {
        public long ComplianceNoticeReasonId { get; set; }
        public string Reason { get; set; } = "";

        public static readonly ComplianceNoticeReasonDto[] ComplianceNoticeReasons = new ComplianceNoticeReasonDto[]
        {
            new ComplianceNoticeReasonDto { ComplianceNoticeReasonId = 1, Reason = "No business license provided" },
            new ComplianceNoticeReasonDto { ComplianceNoticeReasonId = 2, Reason = "Invalid business licence number" },
            new ComplianceNoticeReasonDto { ComplianceNoticeReasonId = 3, Reason = "Expired business licence"},
            new ComplianceNoticeReasonDto { ComplianceNoticeReasonId = 4, Reason = "Suspended business license"}
        };
    }
}
