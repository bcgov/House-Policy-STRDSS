namespace StrDss.Service.EmailTemplates
{
    public class TakedownRequest : EmailTemplateBase
    {
        public TakedownRequest(IEmailMessageService emailService) : base(emailService)
        {
        }

        public string Url { get; set; } = "";
        public long? ListingId { get; set; }
        public string LgContactInfo { get; set; } = "";
        public string LgName { get; set; } = "";
        public override string GetContent()
        {
            Subject = "Takedown Request";

            return (Preview ? GetPreviewContent() : "") + $@"
<b>Request to platform service provider for takedown of non-compliant platform offering.</b><br/><br/>
The following short-term rental listing is not in compliance with an applicable local government business licence requirement:<br/><br/>
<b>{Url}</b><br/><br/>
Listing ID Number: <b>{ListingId}</b><br/><br/>
In accordance, with 17(2) of the Short-term Rental Accommodations Act, please cease providing platform services in respect of the above platform offer within 3 days.<br/><br/>
<b>{LgName}</b><br/><br/>
<b>{LgContactInfo}</b>
";
        }
    }
}
