namespace StrDss.Service.EmailTemplates
{
    public class AccessRequestApproval : EmailTemplateBase
    {
        public AccessRequestApproval(IEmailService emailService) : base(emailService)
        {
        }

        public string Link { get; set; } = "";
        public string AdminEmail { get; set; } = "";
        public override string GetContent()
        {
            Subject = "STR Data Portal - Access Granted";

            return
$@"You have been granted access to the Short Term Rental Data Portal. Please access the portal here: {Link}. If you have any issues accessing this link, please contact {AdminEmail}.";
        }

    }
}
