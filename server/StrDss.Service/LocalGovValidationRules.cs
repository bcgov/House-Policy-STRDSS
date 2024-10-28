using StrDss.Common;

namespace StrDss.Service
{
    public class LocalGovValidationRules
    {
        public static void LoadLocalGovValidationRules(List<FieldValidationRule> rules)
        {
            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.LocalGov,
                FieldName = LocalGovFields.OrganizationCd,
                FieldType = FieldTypes.String,
                Required = true,
                MaxLength = 25,
                MinLength = 1,
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.LocalGov,
                FieldName = LocalGovFields.OrganizationNm,
                FieldType = FieldTypes.String,
                Required = true,
                MaxLength = 250,
                MinLength = 1,
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.LocalGov,
                FieldName = LocalGovFields.LocalGovernmentType,
                FieldType = FieldTypes.String,
                Required = true,
                CodeSet = CodeSet.LocalGovTypes,
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.LocalGov,
                FieldName = LocalGovFields.BusinessLicenceFormatTxt,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 50,
                MinLength = 1,
            });
        }
    }
}
