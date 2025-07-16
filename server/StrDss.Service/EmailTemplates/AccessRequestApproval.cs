using StrDss.Common;

namespace StrDss.Service.EmailTemplates
{
    public class AccessRequestApproval : EmailTemplateBase
    {
        public AccessRequestApproval(IEmailMessageService emailService) : base(emailService)
        {
            EmailMessageType = EmailMessageTypes.AccessGranted;
        }

        public string Link { get; set; } = "";
        public string AdminEmail { get; set; } = "";
        public override string GetContent()
        {
            Subject = "STR Data Portal - Access Granted";

            return
$@"You have been granted access to the Short Term Rental Data Portal. Please access the portal here: <a href='{Link}'>{Link}</a>. If you have any issues accessing this link, please contact <a href='mailto:{AdminEmail}'>{AdminEmail}</a>.";
        }

    }
}
