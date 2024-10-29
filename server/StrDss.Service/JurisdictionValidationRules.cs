using StrDss.Common;

namespace StrDss.Service
{
    public class JurisdictionValidationRules
    {
        public static void LoadJurisdictionValidationRules(List<FieldValidationRule> rules)
        {
            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.Jurisdiction,
                FieldName = JurisdictionFields.IsPrincipalResidenceRequired,
                FieldType = FieldTypes.Bool,
                Required = true
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.Jurisdiction,
                FieldName = JurisdictionFields.IsStrProhibited,
                FieldType = FieldTypes.Bool,
                Required = true
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.Jurisdiction,
                FieldName = JurisdictionFields.IsBusinessLicenceRequired,
                FieldType = FieldTypes.Bool,
                Required = true
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.Jurisdiction,
                FieldName = JurisdictionFields.EconomicRegionDsc,
                FieldType = FieldTypes.String,
                Required = false,
                CodeSet = CodeSet.EconomicRegions,
            });
        }
    }
}
