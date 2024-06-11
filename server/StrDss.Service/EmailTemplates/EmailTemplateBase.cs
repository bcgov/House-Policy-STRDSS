using StrDss.Common;
using StrDss.Model;

namespace StrDss.Service.EmailTemplates
{
    public class EmailTemplateBase
    {
        public IEmailMessageService _emailService { get; }
        public EmailTemplateBase(IEmailMessageService emailService)
        {
            _emailService = emailService;
        }

        public string Subject { get; set;  } = "";
        public string From { get; set; } = NoReply.Default;
        public IEnumerable<string> To { get; set; } = new List<string>();
        public IEnumerable<string> Cc { get; set; } = new List<string>();   
        public IEnumerable<string> Bcc { get; set; } = new List<string>();
        public string Info { get; set; } = "";
        public bool Preview { get; set; } = false;
        public string EmailMessageType { get; set; } = "";
        public IEnumerable<EmailAttachment> Attachments { get; set; } = Enumerable.Empty<EmailAttachment>();
        public IEnumerable<string> EmailsToHide { get; set; } = new List<string>();

        public virtual string GetPreviewHeader()
        {
            return $@"To: {GetRecipentsForPreview(To)}<br/><br/>" 
                + (Bcc.Count() > 0 ? $"<br/>Bcc: {GetRecipentsForPreview(Bcc)}<br/><br/>" : "")
                + (Cc.Count() > 0 ? $"<br/>Cc: {GetRecipentsForPreview(Cc)}<br/><br/>" : "")
                ;
        }

        public string GetRecipentsForPreview(IEnumerable<string> list)
        {
            var filteredList = list.Except(EmailsToHide);
            return string.Join(", ", filteredList);
        }

        public virtual string GetContent()
        {
            return "";
        }

        public async Task<string> SendEmail()
        {
            var emailContent = new EmailContent
            {
                Body = GetContent(),
                From = From,
                Subject = Subject,
                To = To,
                Cc = Cc,
                Bcc = Bcc,
                Info = Info,
                Attachments = Attachments,
            };

            return await _emailService.SendEmailAsync(emailContent);
        }
    }
}
