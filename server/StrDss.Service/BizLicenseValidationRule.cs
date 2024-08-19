using StrDss.Common;

namespace StrDss.Service
{
    public static class BizLicenseValidationRule
    {
        public static void LoadBizLicenseValidationRules(List<FieldValidationRule> rules)
        {
            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenseRowUntyped,
                FieldName = BizLicenseRowFields.OrgCd,
                FieldType = FieldTypes.String,
                Required = true,
                MaxLength = 25,
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenseRowUntyped,
                FieldName = BizLicenseRowFields.BusinessLicenceNo,
                FieldType = FieldTypes.String,
                Required = true,
                MaxLength = 50,
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenseRowUntyped,
                FieldName = BizLicenseRowFields.ExpiryDt,
                FieldType = FieldTypes.String,
                Required = true,
                RegexInfo = RegexDefs.GetRegexInfo(RegexDefs.YearMonthDay) 
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenseRowUntyped,
                FieldName = BizLicenseRowFields.PhysicalRentalAddressTxt,
                FieldType = FieldTypes.String,
                Required = true,
                MaxLength = 250,
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenseRowUntyped,
                FieldName = BizLicenseRowFields.LicenceStatusType,
                FieldType = FieldTypes.String,
                Required = false,
                //AllowedValues = new List<string> { "Pending", "Issued", "Suspended", "Revoked", "Cancelled", "Expired" }
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenseRowUntyped,
                FieldName = BizLicenseRowFields.LicenceTypeTxt,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 320,
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenseRowUntyped,
                FieldName = BizLicenseRowFields.RestrictionTxt,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 320,
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenseRowUntyped,
                FieldName = BizLicenseRowFields.BusinessNm,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 320,
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenseRowUntyped,
                FieldName = BizLicenseRowFields.MailingStreetAddressTxt,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 100,
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenseRowUntyped,
                FieldName = BizLicenseRowFields.MailingCityNm,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 100,
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenseRowUntyped,
                FieldName = BizLicenseRowFields.MailingProvinceCd,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 2,
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenseRowUntyped,
                FieldName = BizLicenseRowFields.MailingPostalCd,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 10,
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenseRowUntyped,
                FieldName = BizLicenseRowFields.BusinessOwnerNm,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 320,
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenseRowUntyped,
                FieldName = BizLicenseRowFields.BusinessOwnerPhoneNo,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 30,
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenseRowUntyped,
                FieldName = BizLicenseRowFields.BusinessOwnerEmailAddressDsc,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 320,
                RegexInfo = RegexDefs.GetRegexInfo(RegexDefs.Email) 
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenseRowUntyped,
                FieldName = BizLicenseRowFields.InfractionTxt,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 320,
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenseRowUntyped,
                FieldName = BizLicenseRowFields.InfractionDt,
                FieldType = FieldTypes.String,
                Required = false,
                RegexInfo = RegexDefs.GetRegexInfo(RegexDefs.YearMonthDay) 
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenseRowUntyped,
                FieldName = BizLicenseRowFields.AvailableBedroomsQty,
                FieldType = FieldTypes.String,
                RegexInfo = RegexDefs.GetRegexInfo(RegexDefs.Quantity)
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenseRowUntyped,
                FieldName = BizLicenseRowFields.MaxGuestsAllowedQty,
                FieldType = FieldTypes.String,
                Required = false,
                RegexInfo = RegexDefs.GetRegexInfo(RegexDefs.Quantity)
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenseRowUntyped,
                FieldName = BizLicenseRowFields.IsPrincipalResidence,
                FieldType = FieldTypes.String,
                Required = false,
                RegexInfo = RegexDefs.GetRegexInfo(RegexDefs.YN)
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenseRowUntyped,
                FieldName = BizLicenseRowFields.IsOwnerLivingOnsite,
                FieldType = FieldTypes.String,
                Required = false,
                RegexInfo = RegexDefs.GetRegexInfo(RegexDefs.YN)
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenseRowUntyped,
                FieldName = BizLicenseRowFields.IsOwnerPropertyTenant,
                FieldType = FieldTypes.String,
                Required = false,
                RegexInfo = RegexDefs.GetRegexInfo(RegexDefs.YN)
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenseRowUntyped,
                FieldName = BizLicenseRowFields.PropertyFolioNo,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 30,
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenseRowUntyped,
                FieldName = BizLicenseRowFields.PropertyParcelIdentifierNo,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 30,
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenseRowUntyped,
                FieldName = BizLicenseRowFields.PropertyLegalDescriptionTxt,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 320,
            });
        }
    }
}
