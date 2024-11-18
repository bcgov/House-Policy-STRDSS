using StrDss.Common;

namespace StrDss.Service.EmailTemplates
{
    public class ComplianceOrderFromListing : EmailTemplateBase
    {
        public ComplianceOrderFromListing(IEmailMessageService emailService)
            : base(emailService)
        {
            EmailMessageType = EmailMessageTypes.ComplianceOrder;
            Subject = "New mail regarding your short-term rental listing";
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
<b>This message has been sent to you by B.C.'s Short-term Rental Compliance Unit regarding your short-term rental listing:</b><br/><b>{Url}</b><br/><br/>
<b>{Sanitize(Comment)}</b><br/>
";
        }

        public string GetHtmlPreview()
        {
            return GetContent();
        }
    }
}
