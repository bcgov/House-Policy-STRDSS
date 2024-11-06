using StrDss.Common;

namespace StrDss.Service.EmailTemplates
{
    public class ComplianceOrderFromListing : EmailTemplateBase
    {
        public ComplianceOrderFromListing(IEmailMessageService emailService)
            : base(emailService)
        {
            EmailMessageType = EmailMessageTypes.ComplianceOrder;
            From = Environment.GetEnvironmentVariable("STR_CEU_EMAIL") ?? From;
            Subject = "New mail from the Short-term Rental Compliance and Enforcement Unit";
        }

        public long RentalListingId { get; set; }
        public string OrgCd { get; set; } = "";
        public string Url { get; set; } = "";
        public string? ListingId { get; set; }
        public string Comment { get; set; } = "";

        public override string GetContent()
        {
            return (Preview ? GetPreviewHeader() : "") + $@"
Dear Host,<br/><br/>
This message has been sent to you by B.C.'s Short-Term Rental Compliance Unit regarding your short-term rental<br/>
listing: <b>{Url}</b><br/><br/>
{Comment}<br/>
";
        }

        public string GetHtmlPreview()
        {
            return GetContent();
        }
    }
}
