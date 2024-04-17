using StrDss.Common;

namespace StrDss.Service.EmailTemplates
{
    public class TakedownNotice : EmailTemplateBase
    {
        public TakedownNotice(IEmailMessageService emailService) 
            : base(emailService)
        {
            EmailMessageType = EmailMessageTypes.NoticeOfTakedown;
        }

        public string Url { get; set; } = "";
        public string? ListingId { get; set; }
        public string Comment { get; set; } = "";
        public string LgName { get; set; } = "";

        public override string GetContent()
        {
            Subject = "Notice of non-compliance with short-term rental business licence requirement";

            return (Preview ? GetPreviewContent() : "") + $@"
Dear Host,<br/><br/>
Short-term rental accommodations in your community are regulated by your local government. The {LgName} has determined that the following short-term rental listing is not in compliance with an applicable local government business licence requirement:<br/><br/>
<b>{Url}</b><br/><br/>
Listing ID Number: <b>{ListingId}</b><br/><br/>
Under the provincial <a href='https://www.bclaws.gov.bc.ca/civix/document/id/bills/billsprevious/4th42nd:gov35-1'>Short-Term Rental Accommodations Act</a> and its regulations, the local government may submit a request to the short-term rental platform to cease providing platform services (e.g., remove this listing from the platform and cancel all bookings) within a period of 5-90 days after the date of delivery of this Notice. Short-term rental platforms are required to comply with the local government’s request within 5 days of receiving the request.<br/><br/>
This Notice has been issued by {LgName}.<br/><br/>
{Comment}<br/><br/>
For more information on this Notice, or local government short-term rental business licences, please contact your local government.<br/><br/>
For more information on the Short-term Rental Accommodations Act, please visit: <a href='https://www2.gov.bc.ca/gov/content/housing-tenancy/short-term-rentals'>New rules for short-term rentals - Province of British Columbia (gov.bc.ca)</a>.<br/><br/>

This email has been automatically generated. Please do not reply to this email. A copy of this Notice has been sent to the short-term rental platform.<br/><br/>
";
        }
    }
}
