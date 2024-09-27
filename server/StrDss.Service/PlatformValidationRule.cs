using StrDss.Common;

namespace StrDss.Service
{
    public static class PlatformValidationRule
    {
        public static void LoadPlatformUpdateValidationRules(List<FieldValidationRule> rules)
        {
            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.Platform,
                FieldName = PlatformFields.OrganizationCd,
                FieldType = FieldTypes.String,
                Required = true,
                MaxLength = 25,
                MinLength = 1,
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.Platform,
                FieldName = PlatformFields.OrganizationNm,
                FieldType = FieldTypes.String,
                Required = true,
                MaxLength = 250,
                MinLength = 1,
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.Platform,
                FieldName = PlatformFields.PrimaryNoticeOfTakedownContactEmail,
                FieldType = FieldTypes.String,
                Required = true,
                MaxLength = 320,
                RegexInfo = RegexDefs.GetRegexInfo(RegexDefs.Email)
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.Platform,
                FieldName = PlatformFields.PrimaryTakedownRequestContactEmail,
                FieldType = FieldTypes.String,
                Required = true,
                MaxLength = 320,
                RegexInfo = RegexDefs.GetRegexInfo(RegexDefs.Email)
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.Platform,
                FieldName = PlatformFields.SecondaryNoticeOfTakedownContactEmail,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 320,
                RegexInfo = RegexDefs.GetRegexInfo(RegexDefs.Email)
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.Platform,
                FieldName = PlatformFields.SecondaryTakedownRequestContactEmail,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 320,
                RegexInfo = RegexDefs.GetRegexInfo(RegexDefs.Email)
            });
        }
    }

}
