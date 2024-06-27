using StrDss.Common;
using StrDss.Model.CommonCode;
using System.Text.RegularExpressions;

namespace StrDss.Service
{
    public interface IFieldValidatorService
    {
        Dictionary<string, List<string>> Validate<T>(string entityName, string fieldName, T value, Dictionary<string, List<string>> errors, int rowNum = 0);
        Dictionary<string, List<string>> Validate<T>(string entityName, T entity, Dictionary<string, List<string>> errors, int rowNum = 0, params string[] fieldsToSkip);
        List<CommonCodeDto> CommonCodes { get; set; }
    }
    public class FieldValidatorService : IFieldValidatorService
    {
        List<FieldValidationRule> _rules;
        public List<CommonCodeDto> CommonCodes { get; set; } = new List<CommonCodeDto>();
        public FieldValidatorService()
        {
            _rules = new List<FieldValidationRule>();

            RentalListingReportValidationRule.LoadReportValidationRules(_rules);
            RoleValidationRule.LoadReportValidationRules(_rules);
        }

        public IEnumerable<FieldValidationRule> GetFieldValidationRules(string entityName)
        {
            return _rules.Where(x => x.EntityName.ToLowerInvariant() == entityName.ToLowerInvariant());
        }

        public Dictionary<string, List<string>> Validate<T>(string entityName, T entity, Dictionary<string, List<string>> errors, int rowNum = 0, params string[] fieldsToSkip)
        {
            var fields = typeof(T).GetProperties();

            foreach (var field in fields)
            {
                if (fieldsToSkip.Any(x => x == field.Name))
                    continue;

                Validate(entityName, field.Name, field.GetValue(entity), errors, rowNum);
            }

            return errors;
        }

        public Dictionary<string, List<string>> Validate<T>(string entityName, string fieldName, T val, Dictionary<string, List<string>> errors, int rowNum = 0)
        {
            var rule = _rules.FirstOrDefault(r => r.EntityName == entityName && r.FieldName == fieldName);

            if (rule == null)
                return errors;

            var messages = new List<string>();

            switch (rule.FieldType)
            {
                case FieldTypes.String:
                    messages.AddRange(ValidateStringField(rule, val, rowNum));
                    break;
                case FieldTypes.Date:
                    messages.AddRange(ValidateDateField(rule, val));
                    break;
                case FieldTypes.Decimal:
                    messages.AddRange(ValidateNumberField(rule, val));
                    break;
                default:
                    throw new NotImplementedException($"Validation for {rule.FieldType} is not implemented.");
            }

            if (messages.Count > 0)
            {
                foreach (var message in messages)
                {
                    errors.AddItem(rule.FieldName, message);
                }
            }

            return errors;
        }

        private List<string> ValidateStringField<T>(FieldValidationRule rule, T val, int rowNum = 0)
        {
            var messages = new List<string>();

            var rowNumPrefix = rowNum == 0 ? "" : $"Row # {rowNum}: ";

            var field = rule.FieldName.WordToWords();

            if (rule.Required && val is null)
            {
                messages.Add($"{rowNumPrefix}The {field} field is required.");
                return messages;
            }

            if (!rule.Required && (val is null || val!.ToString()!.IsEmpty()))
                return messages;

            string value = Convert.ToString(val)!;

            if (rule.Required && value!.IsEmpty())
            {
                messages.Add($"{rowNumPrefix}The {field} field is required.");
                return messages;
            }

            if (rule.MinLength != null && value.Length < rule.MinLength)
            {
                messages.Add($"{rowNumPrefix}The length of {field} field must be equal to or greater than {rule.MinLength}.");
            }

            if (rule.MaxLength != null && value.Length > rule.MaxLength)
            {
                messages.Add($"{rowNumPrefix}The length of {field} field must be equal to or smaller than {rule.MaxLength}.");
            }

            if (rule.RegexInfo != null)
            {
                if (!Regex.IsMatch(value, rule.RegexInfo.Regex))
                {
                    var message = string.Format(rule.RegexInfo.ErrorMessage, val!.ToString());
                    messages.Add($"{rowNumPrefix}{message}.");
                }
            }

            if (rule.CodeSet != null)
            {
                if (decimal.TryParse(value, out decimal numValue))
                {
                    var exists = CommonCodes.Any(x => x.CodeSet == rule.CodeSet && x.Id == numValue);

                    if (!exists)
                    {
                        messages.Add($"{rowNumPrefix}Invalid value. [{value}] doesn't exist in the code set {rule.CodeSet}.");
                    }
                }
                else
                {
                    messages.Add($"{rowNumPrefix}Invalid value. [{value}] doesn't exist in the code set {rule.CodeSet}.");
                }
            }

            return messages;
        }

        private List<string> ValidateDateField<T>(FieldValidationRule rule, T val, int rowNum = 0)
        {
            var messages = new List<string>();

            var rowNumPrefix = rowNum == 0 ? "" : $"Row # {rowNum}: ";

            var field = rule.FieldName.WordToWords();

            if (rule.Required && val is null)
            {
                messages.Add($"{rowNumPrefix}{field} field is required.");
                return messages;
            }

            if (!rule.Required && (val is null || val!.ToString()!.IsEmpty()))
                return messages;

            var (parsed, parsedDate) = DateUtils.ParseDate(val!);

            if (!parsed)
            {
                messages.Add($"{rowNumPrefix}Invalid value. [{val!}] cannot be converted to a date");
                return messages;
            }

            var value = parsedDate;

            if (rule.MinDate != null && rule.MaxDate != null)
            {
                if (value < rule.MinDate || value > rule.MaxDate)
                {
                    messages.Add($"{rowNumPrefix}The length of {field} must be between {rule.MinDate} and {rule.MaxDate}.");
                }
            }

            return messages;
        }

        private List<string> ValidateNumberField<T>(FieldValidationRule rule, T val, int rowNum = 0)
        {
            var messages = new List<string>();

            var rowNumPrefix = rowNum == 0 ? "" : $"Row # {rowNum}: ";

            var field = rule.FieldName.WordToWords();

            if (rule.Required && val is null)
            {
                messages.Add($"{rowNumPrefix}{field} field is required.");
                return messages;
            }

            if (!rule.Required && (val is null || val!.ToString()!.IsEmpty()))
                return messages;

            string value = Convert.ToString(val) ?? ""; 

            if (!value.IsNumeric())
            {
                messages.Add($"{{rowNumPrefix}}{{field}} field must be numeric.");
                return messages;
            }

            decimal numVal = Convert.ToDecimal(value);

            if (numVal < rule.MinValue)
            {
                messages.Add($"{{rowNumPrefix}}{{field}} field must be greater than or equal to {rule.MinValue}.");
            }

            if (numVal > rule.MaxValue)
            {
                messages.Add($"{{rowNumPrefix}}{{field}} field must be less than or equal to {rule.MinValue}.");
            }

            return messages;
        }
    }
}