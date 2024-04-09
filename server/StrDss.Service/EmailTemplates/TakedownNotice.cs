namespace StrDss.Service.EmailTemplates
{
    public class TakedownNotice : EmailTemplateBase
    {
        public TakedownNotice(IEmailMessageService emailService) 
            : base(emailService)
        {
        }

        public string Reason { get; set; } = "";
        public string Url { get; set; } = "";
        public string? ListingId { get; set; }
        public string LgContactInfo { get; set; } = "";
        public string LgStrBylawLink { get; set; } = "";
        public string Comment { get; set; }
        public override string GetContent()
        {
            Subject = "Notice of Takedown of Short Term Rental Platform Offer";

            return (Preview ? GetPreviewContent() : "") + $@"
Dear Short-term Rental Host,<br/><br/>
Short-term rental accommodations in your community must obtain a short-term rental (STR) business licence from the local government in order to operate.<br/><br/>
Short-term rental accommodations are also regulated by the Province of B.C. Under the Short-term Rental Accommodations Act, short-term rental hosts in communities with a short-term rental business licence requirement must include a valid business licence number on any short-term rental listings advertised on an online platform. Short-term rental platforms are required to remove listings that do not meet this requirement if requested by the local government.<br/><br/>
The short-term rental listing below is not in compliance with an applicable local government business licence requirement for the following reason: <b>{Reason}</b><br/><br/>
<b>{Url}</b><br/><br/>
Listing ID Number: <b>{ListingId}</b><br/><br/>
Unless you are able to demonstrate compliance with the business licence requirement, this listing may be removed from the short-term rental platform after 5 days. The local government has 90 days to submit a request to takedown the listing to the platform.<br/><br/>
For more information, contact:<br/><br/>
<b>{LgContactInfo}<br/><br/></b>
<b>{LgStrBylawLink}<br/><br/></b>
{Comment}<br/>
";
        }
    }
}
