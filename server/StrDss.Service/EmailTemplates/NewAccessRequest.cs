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
$@"Hello,<br/><br/>
A new access request is waiting for you to approve.<br/><br/>
To approve or reject, please visit: {Link}<br/><br/>
Thank you.
";
        }
    }
}
