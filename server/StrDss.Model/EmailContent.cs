﻿using System.Text.Json;
using System.Text.Json.Serialization;

namespace StrDss.Model
{
    public class EmailContent
    {
        private static JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { WriteIndented = false, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public IEnumerable<string> Bcc { get; set; } = new List<string>();

        public string BodyType { get; set; } = "html";

        public string Body { get; set; } = "";

        public IEnumerable<string> Cc { get; set; } = new List<string>();

        public int DelayTS { get; set; } = 0;

        public string Encoding { get; set; } = "utf-8";

        public string From { get; set; } = "";

        public string Priority { get; set; } = "normal";

        public string Subject { get; set; } = "";

        public IEnumerable<string> To { get; set; } = new List<string>();
        public IEnumerable<EmailAttachment> Attachments { get; set; } = Array.Empty<EmailAttachment>();

        [JsonIgnore]
        public string Info { get; set; } = "";

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, _jsonOptions);
        }
    }
}
