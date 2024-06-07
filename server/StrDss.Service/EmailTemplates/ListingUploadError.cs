using StrDss.Common;

namespace StrDss.Service.EmailTemplates
{
    public class ListingUploadError : EmailTemplateBase
    {
        public ListingUploadError(IEmailMessageService emailService)
            : base(emailService)
        {
            EmailMessageType = EmailMessageTypes.ListingUploadError;
            From = Environment.GetEnvironmentVariable("ADMIN_EMAIL") ?? From;
        }
        public string UserName { get; set; } = "";
        public long NumErrors { get; set; }
        public string Link { get; set; } = "";
        public override string GetContent()
        {
            Subject = "B.C. STR Data Portal: Upload error report";

            return
$@"Dear {UserName},<br/><br/>
Thank you for uploading a monthly STR report to British Columbia’s Short-term Rental Data Portal. Your upload has been processed and {NumErrors} error(s) have been found in your uploaded file.<br/><br/>
Here’s how to fix errors: <br/><br/>
<ol>
<li>Download the file containing the error rows <a href='{Link}'>here</a>.</li>
<li>Fix the highlighted errors in the columns.</li>
<li>Upload the corrected file to the STR Data Portal like you would normally upload your monthly report.</li>
</ol>
<br/>
Please contact DSSadmin@gov.bc.ca if you encounter any problems.<br/>
";
        }
    }
}
