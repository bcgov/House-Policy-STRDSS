using StrDss.Common;

namespace StrDss.Service.EmailTemplates
{
    public class TakedownRequestFromListing : TakedownRequest
    {
        public TakedownRequestFromListing(IEmailMessageService emailService)
            : base(emailService)
        {
        }

        public string OrgCd { get; set; } = "";
        public long RentalListingId { get; set; }

        public string GetHtmlPreview()
        {
            Subject = "Confirmation of Takedown Request";

            return (Preview ? GetPreviewHeader() : "") + $@"
A takedown request for the following short-term rental listing was submitted to the Province of B.C.’s Short-term Rental Data Portal and will be delivered to the platform at 11:50pm PST tonight:<br/><br/>
<strong>[URL of the listing]</strong><br/><br/>
Listing ID Number: <strong>[Listing ID]</strong><br/><br/>
Under the <a href='https://www.bclaws.gov.bc.ca/civix/document/id/bills/billsprevious/4th42nd:gov35-1'>Short-Term Rental Accommodations Act</a> and its regulations, the platform is required to comply with the request within 5 days from the date of receipt of the request. If the platform fails to comply with the request (e.g., remove the listing), local governments can escalate the matter to the Director of the Provincial STR Compliance and Enforcement Unit at: <a href='mailto: CEUescalations@gov.bc.ca'>CEUescalations@gov.bc.ca</a>.<br/><br/>
This email has been automatically generated. Please do not reply to this email.
";
        }
    }
}
