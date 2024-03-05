using System.Text.Json;
using StrDss.Common;

namespace StrDss.Model
{
    public class ErrorDetail
    {
        private static JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { WriteIndented = false, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public List<FieldMessage> FieldMessages { get; set; }

        public ErrorDetail(Dictionary<string, List<string>> errors)
        {
            FieldMessages = new List<FieldMessage>();

            foreach (var error in errors)
            {
                FieldMessages.Add(new FieldMessage
                {
                    Field = error.Key.WordToWords(),
                    Messages = error.Value
                });
            }
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, _jsonOptions);
        }
    }
}
