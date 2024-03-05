using System.Text.Json.Serialization;

namespace StrDss.Model.CommonCode
{
    public class CommonCodeDto
    {
        [JsonPropertyName("value")]
        public int Id { get; set; }
        public string CodeSet { get; set; }
        [JsonPropertyName("label")]
        public string CodeName { get; set; }
        public string CodeValue { get; set; }
    }
}
