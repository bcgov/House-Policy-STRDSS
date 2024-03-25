namespace StrDss.Service.EmailTemplates
{
    public class AccessRequestDenial : EmailTemplateBase
    {
        public AccessRequestDenial(IEmailMessageService emailService) : base(emailService)
        {
        }

        public string AdminEmail { get; set; } = "";

        public override string GetContent()
        {
            Subject = "STR Data Portal - Access Denied";

            return
$@"Access to the STR Data Portal is restricted to authorized provincial and local government staff and short-term rental platforms. Please contact {AdminEmail} for more information.";
        }
    }
}
