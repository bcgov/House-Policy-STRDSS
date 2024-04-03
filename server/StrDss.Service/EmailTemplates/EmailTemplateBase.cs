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
        public string Info { get; set; }
        public bool preview { get; set; } = false;

        public string GetPreviewContent()
        {
            return $@"To: {string.Join(";", To)}<br/>cc: {string.Join(";", Cc)}<br/><br/>";
        }

        public virtual string GetContent()
        {
            return "";
        }

        public async Task<bool> SendEmail()
        {
            var emailContent = new EmailContent
            {
                Body = GetContent(),
                From = NoReply.Default,
                Subject = Subject,
                To = To,
                Cc = Cc,
                Bcc = Bcc,
                Info = Info
            };

            return await _emailService.SendEmailAsync(emailContent);
        }
    }
}
