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
        RegexDefs _regex;
        public List<CommonCodeDto> CommonCodes { get; set; } = new List<CommonCodeDto>();
        public FieldValidatorService(RegexDefs regex)
        {
            _rules = new List<FieldValidationRule>();
            _regex = regex;

            LoadSystemUserEntityRules();
            LoadStrApplicationEntityRules();
        }

        public IEnumerable<FieldValidationRule> GetFieldValidationRules(string entityName)
        {
            return _rules.Where(x => x.EntityName.ToLowerInvariant() == entityName.ToLowerInvariant());
        }

        private void LoadSystemUserEntityRules()
        {
            _rules.Add(new FieldValidationRule
            {
                EntityName = Entities.SystemUser,
                FieldName = Fields.Username,
                FieldType = FieldTypes.String,
                Required = true,
                MinLength = 1,
                MaxLength = 10
            });

            _rules.Add(new FieldValidationRule
            {
                EntityName = Entities.SystemUser,
                FieldName = Fields.Passwrod,
                FieldType = FieldTypes.String,
                Required = true,
                MinLength = 8,
                MaxLength = 255,
                RegexInfo = _regex.GetRegexInfo(RegexDefs.Password)
            });

            _rules.Add(new FieldValidationRule
            {
                EntityName = Entities.SystemUser,
                FieldName = Fields.LastName,
                FieldType = FieldTypes.String,
                Required = true,
                MinLength = 1,
                MaxLength = 30
            });

            _rules.Add(new FieldValidationRule
            {
                EntityName = Entities.SystemUser,
                FieldName = Fields.StreetAddress,
                FieldType = FieldTypes.String,
                Required = true,
                MinLength = 1,
                MaxLength = 255
            });

            _rules.Add(new FieldValidationRule
            {
                EntityName = Entities.SystemUser,
                FieldName = Fields.City,
                FieldType = FieldTypes.String,
                Required = true,
                MinLength = 1,
                MaxLength = 255
            });

            _rules.Add(new FieldValidationRule
            {
                EntityName = Entities.SystemUser,
                FieldName = Fields.Province,
                FieldType = FieldTypes.String,
                Required = true,
                MinLength = 1,
                MaxLength = 255
            });

            _rules.Add(new FieldValidationRule
            {
                EntityName = Entities.SystemUser,
                FieldName = Fields.PostalCode,
                FieldType = FieldTypes.String,
                Required = true,
                MinLength = 6,
                MaxLength = 6
            });

            _rules.Add(new FieldValidationRule
            {
                EntityName = Entities.SystemUser,
                FieldName = Fields.PhoneNumber,
                FieldType = FieldTypes.String,
                Required = true,
                MinLength = 14,
                MaxLength = 14,
                RegexInfo = _regex.GetRegexInfo(RegexDefs.PhoneNumber)
            });
        }

        private void LoadStrApplicationEntityRules()
        {
            _rules.Add(new FieldValidationRule
            {
                EntityName = Entities.StrApplication,
                FieldName = Fields.StreetAddress,
                FieldType = FieldTypes.String,
                Required = true,
                MinLength = 1,
                MaxLength = 255
            });

            _rules.Add(new FieldValidationRule
            {
                EntityName = Entities.StrApplication,
                FieldName = Fields.City,
                FieldType = FieldTypes.String,
                Required = true,
                MinLength = 1,
                MaxLength = 255
            });

            _rules.Add(new FieldValidationRule
            {
                EntityName = Entities.StrApplication,
                FieldName = Fields.Province,
                FieldType = FieldTypes.String,
                Required = true,
                MinLength = 1,
                MaxLength = 255
            });

            _rules.Add(new FieldValidationRule
            {
                EntityName = Entities.StrApplication,
                FieldName = Fields.PostalCode,
                FieldType = FieldTypes.String,
                Required = true,
                MinLength = 6,
                MaxLength = 6
            });

            _rules.Add(new FieldValidationRule
            {
                EntityName = Entities.StrApplication,
                FieldName = Fields.SquareFootage,
                FieldType = FieldTypes.String,
                Required = true
            });

            _rules.Add(new FieldValidationRule
            {
                EntityName = Entities.StrApplication,
                FieldName = Fields.ZoningTypeId,
                FieldType = FieldTypes.String,
                Required = true,
                CodeSet = CodeSet.ZoneType
            });

            _rules.Add(new FieldValidationRule
            {
                EntityName = Entities.StrApplication,
                FieldName = Fields.StrAffiliateId,
                FieldType = FieldTypes.String,
                Required = true,
                CodeSet = CodeSet.StrAffiliate
            });

            _rules.Add(new FieldValidationRule
            {
                EntityName = Entities.StrApplication,
                FieldName = Fields.ComplianceStatusId,
                FieldType = FieldTypes.String,
                Required = true,
                CodeSet = CodeSet.ComplianceStatus
            });
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

            if (!rule.Required && (val is null || val.ToString().IsEmpty()))
                return messages;

            string value = Convert.ToString(val);

            if (rule.Required && value.IsEmpty())
            {
                messages.Add($"{rowNumPrefix}The {field} field is required.");
                return messages;
            }

            if (rule.MinLength != null && rule.MaxLength != null)
            {               
                if (value.Length < rule.MinLength || value.Length > rule.MaxLength)
                {
                    if (rule.MinLength == rule.MaxLength)
                    {
                        messages.Add($"{rowNumPrefix}The length of {field} field must be {rule.MinLength}.");
                    }
                    else
                    {
                        messages.Add($"{rowNumPrefix}The length of {field} field must be between {rule.MinLength} and {rule.MaxLength}.");
                    }
                }
            }

            if (rule.RegexInfo != null)
            {
                if (!Regex.IsMatch(value, rule.RegexInfo.Regex))
                {
                    var message = string.Format(rule.RegexInfo.ErrorMessage, val.ToString());
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

            if (!rule.Required && (val is null || val.ToString().IsEmpty()))
                return messages;

            var (parsed, parsedDate) = DateUtils.ParseDate(val);

            if (!parsed)
            {
                messages.Add($"{rowNumPrefix}Invalid value. [{val.ToString()}] cannot be converted to a date");
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
    }
}