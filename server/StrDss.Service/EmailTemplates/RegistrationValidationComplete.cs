using StrDss.Common;

namespace StrDss.Service.EmailTemplates
{
    public class RegistrationValidationComplete : EmailTemplateBase
    {
        public RegistrationValidationComplete(IEmailMessageService emailService) : base(emailService)
        {
            EmailMessageType = EmailMessageTypes.RegistrationValidation;
            From = Environment.GetEnvironmentVariable("ADMIN_EMAIL") ?? From;
        }
        public string UserName { get; set; } = "";
        public string Link { get; set; } = "";
        public string DownloadLink { get; set; } = "";
        public override string GetContent()
        {
            Subject = "B.C. STR Data Portal: Validation report ready ";

            return
$@"Dear {UserName},<br/><br/>
Thank you for submitting your listings to British Columbia’s Short-term Rental Data Portal to validate that all listings include a valid host registration, as required under s. 17(1)(b)(ii) of the <i>Short-term Rental Accommodations Act</i>.<br/> 
<br/>
Your upload has been processed, and the validation report is now available for <a href='{DownloadLink}'>download</a>. The validation results are listed in the first column of the report. For any listings that failed validation, corresponding error messages are provided.<br/> 
<br/>
<b>Next Steps</b><br/>
Platforms are required to take specific actions when a registration is determined to be invalid. See the <a href='{Link}'>Guide to STR Registration validation for Minor Platforms</a> for more details.<br/>
<br/>
Please contact <a href='mailto:{From}'>{From}</a> for technical support if needed.<br/>
";
        }
    }
}
