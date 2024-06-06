using StrDss.Common;

namespace StrDss.Service.EmailTemplates
{
    public class BatchTakedownNotice : EmailTemplateBase
    {
        public BatchTakedownNotice(IEmailMessageService emailService) 
            : base(emailService)
        {
            EmailMessageType = EmailMessageTypes.NoticeOfTakedown;
            From = Environment.GetEnvironmentVariable("STR_NOTICE_EMAIL") ?? From;

        }

        public override string GetContent()
        {
            Subject = "Notice of non-compliance with short-term rental business licence requirement";

            return (Preview ? GetPreviewContent() : "") + $@"
Dear Platform,<br/><br/>
The respective local governments in B.C. with a short-term rental business licencing requirement have determined the the short-term rental listings in the attached spreadsheet are not in compliance with an applicable local government business licence requirement.<br/><br/>
Under the provincial <a href='https://www.bclaws.gov.bc.ca/civix/document/id/bills/billsprevious/4th42nd:gov35-1'><i>Short-Term Rental Accommodations Act</i></a> and its regulations, the local government may submit a request to the short-term rental platform to cease providing platform services (e.g., remove this listing from the platform and cancel bookings) within a period of 5-90 days after the date of delivery of this Notice. Short-term rental platforms are required to comply with the local government’s request within 5 days of receiving the request.<br/><br/>
For more information on the <i>Short-term Rental Accommodations Act</i>, please visit: <a href='https://www2.gov.bc.ca/gov/content/housing-tenancy/short-term-rentals'>New rules for short-term rentals - Province of British Columbia (gov.bc.ca)</a>.<br/><br/>
";
        }
    }
}
