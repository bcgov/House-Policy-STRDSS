using StrDss.Common;

namespace StrDss.Service.EmailTemplates
{
    public class BatchTakedownRequest : EmailTemplateBase
    {
        public BatchTakedownRequest(IEmailMessageService emailService)
            : base(emailService)
        {
            EmailMessageType = EmailMessageTypes.BatchTakedownRequest;
            From = Environment.GetEnvironmentVariable("STR_DATA_EMAIL") ?? From;
        }

        public override string GetContent()
        {
            Subject = "Takedown Request";

            return (Preview ? GetPreviewHeader() : "") + $@"
The short-term rental listings in the attached file are the subject of a request by a local government described under s. 18(3) of the <i>Short-Term Rental Accommodations Act</i> and s. 16 of the <i>Short-Term Rental Accommodations Regulation</i>. <br/><br/>
A “takedown request” (a request for a platform to cease providing platform services, ie., remove the listing and cancel bookings associated with the listing) has been submitted for each of these listings by the respective local government via the Province of British Columbia’s Short-term Rental (STR) Data Portal.<br/><br/>
In accordance with s. 18(3) of the <i>Short-Term Rental Accommodations Act</i> and s. 16 (3) of the <i>Short-Term Rental Accommodations Regulation</i>, please cease providing platform services in respect of the attached platform offers within 5 days from the date of receipt of this request.<br/><br/>
Failure to comply with this request could result in enforcement actions or penalties under the <i>Short-Term Rental Accommodations Act</i>.<br/><br/>
For more information on these requests, or local government short-term rental business licences, please contact the local government.<br/><br/>
For more information on the <i>Short-term Rental Accommodations Act</i>, please visit: <a href='https://www2.gov.bc.ca/gov/content/housing-tenancy/short-term-rentals'>New rules for short-term rentals - Province of British Columbia (gov.bc.ca)</a><br/><br/>
This email has been automatically generated. Please do not reply to this email.
";
        }
    }
}
