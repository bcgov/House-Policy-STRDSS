using StrDss.Common;

namespace StrDss.Service.EmailTemplates
{
    public class BatchTakedownRequest : EmailTemplateBase
    {
        public BatchTakedownRequest(IEmailMessageService emailService) : base(emailService)
        {
            EmailMessageType = EmailMessageTypes.BatchTakedownRequest;
        }

        public override string GetContent()
        {
            Subject = "Takedown Request";

            return (Preview ? GetPreviewContent() : "") + $@"
<b>The platform is required to remove the listing within 8 days of the date it was delivered. If the platform fails to remove the listing, local governments can escalate the matter to the Director of the Provincial STR Compliance and Enforcement Unit at: CEUescalations@gov.bc.ca.<br/><br/>
This email has been automatically generated. Please do not reply to this email.
";
        }
    }
}
