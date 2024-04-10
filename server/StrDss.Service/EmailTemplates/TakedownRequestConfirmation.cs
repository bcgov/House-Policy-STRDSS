namespace StrDss.Service.EmailTemplates
{
    public class TakedownRequestConfirmation : EmailTemplateBase
    {
        public TakedownRequestConfirmation(IEmailMessageService emailService) : base(emailService)
        {
        }

        public string Url { get; set; } = "";
        public string? ListingId { get; set; }
        public override string GetContent()
        {
            Subject = "Confirmation of Takedown Request";

            return (Preview ? GetPreviewContent() : "") + $@"
<b>A takedown request for the following short-term rental listing was submitted to the STR Data Portal and will be delivered to the respective short-term rental platform at 11:50pm PST tonight:</b><br/><br/>
<b>{Url}</b><br/><br/>
Listing ID Number: <b>{ListingId}</b><br/><br/>
The platform is required to remove the listing within 8 days of the date it was delivered. If the platform fails to remove the listing, local governments can escalate the matter to the Director of the Provincial STR Compliance and Enforcement Unit at: CEUescalations@gov.bc.ca.<br/><br/>
This email has been automatically generated. Please do not reply to this email.
";
        }
    }
}
