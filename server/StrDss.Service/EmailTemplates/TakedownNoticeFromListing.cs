using StrDss.Common;

namespace StrDss.Service.EmailTemplates
{
    public class TakedownNoticeFromListing : TakedownNotice
    {
        public TakedownNoticeFromListing(IEmailMessageService emailService)
            : base(emailService)
        {
        }

        public string OrgCd { get; set; } = ""; 
        public long RentalListingId { get; set; }

        public override string GetPreviewHeader()
        {
            return $@"To: [host] (if selected), [local government contact info]<br/>"
                + $"Bcc: [sender], [optional cc’s], [platform]<br/><br/>";
        }

        public string GetHtmlPreview()
        {
            Subject = "Notice of non-compliance with short-term rental business licence requirement";

            return (Preview ? GetPreviewHeader() : "") + $@"<br/><br/>
Dear Host,<br/><br/>
Short-term rental accommodations in your community are regulated by your local government. The <strong>[local government]</strong> has determined that the following short-term rental listing is not in compliance with an applicable local government business licence requirement:<br/><br/>
<strong>[URL of the listing]</strong><br/><br/>
Listing ID Number: <strong>[Listing ID]</strong><br/><br/>
Under the provincial <a href='https://www.bclaws.gov.bc.ca/civix/document/id/bills/billsprevious/4th42nd:gov35-1'><i>Short-Term Rental Accommodations Act</i></a> and its regulations, the local government may submit a request to the short-term rental platform to cease providing platform services (e.g., remove this listing from the platform and cancel bookings) within a period of 5-90 days after the date of delivery of this Notice. Short-term rental platforms are required to comply with the local government’s request within 5 days of receiving the request.<br/><br/>
This Notice has been issued by <strong>[local government]</strong>.<br/><br/>
<strong>[Optinal remarks]</strong><br/><br/>
For more information on this Notice, or local government short-term rental business licences, please contact your local government.<br/><br/>
For more information on the <i>Short-term Rental Accommodations Act</i>, please visit: <a href='https://www2.gov.bc.ca/gov/content/housing-tenancy/short-term-rentals'>New rules for short-term rentals - Province of British Columbia (gov.bc.ca)</a>.<br/><br/>

This email has been automatically generated. Please do not reply to this email. A copy of this Notice has been sent to the short-term rental platform.<br/><br/>
";
        }
    }
}
