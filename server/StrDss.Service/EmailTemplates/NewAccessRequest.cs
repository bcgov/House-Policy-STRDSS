using StrDss.Common;

namespace StrDss.Service.EmailTemplates
{
    public class NewAccessRequest : EmailTemplateBase
    {
        public NewAccessRequest(IEmailMessageService emailService) 
            : base(emailService)
        {
            EmailMessageType = EmailMessageTypes.AccessRequested;
        }
        public string Link { get; set; } = "";
        public override string GetContent()
        {
            Subject = "STR Data Portal - New Access Request";

            return
$@"New access request has been raised and requires review. {Link}";
        }
    }
}
