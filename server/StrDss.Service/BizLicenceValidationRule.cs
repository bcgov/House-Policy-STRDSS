using StrDss.Common;

namespace StrDss.Service
{
    public static class BizLicenceValidationRule
    {
        public static void LoadBizLicenceValidationRules(List<FieldValidationRule> rules)
        {
            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenceRowUntyped,
                FieldName = BizLicenceRowFields.OrgCd,
                FieldType = FieldTypes.String,
                Required = true,
                MaxLength = 25,
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenceRowUntyped,
                FieldName = BizLicenceRowFields.BusinessLicenceNo,
                FieldType = FieldTypes.String,
                Required = true,
                MaxLength = 50,
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenceRowUntyped,
                FieldName = BizLicenceRowFields.ExpiryDt,
                FieldType = FieldTypes.String,
                Required = true,
                RegexInfo = RegexDefs.GetRegexInfo(RegexDefs.YearMonthDay) 
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenceRowUntyped,
                FieldName = BizLicenceRowFields.PhysicalRentalAddressTxt,
                FieldType = FieldTypes.String,
                Required = true,
                MaxLength = 250,
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenceRowUntyped,
                FieldName = BizLicenceRowFields.LicenceStatusType,
                FieldType = FieldTypes.String,
                Required = false,
                CodeSet = CodeSet.LicenceStatus,
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenceRowUntyped,
                FieldName = BizLicenceRowFields.LicenceTypeTxt,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 320,
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenceRowUntyped,
                FieldName = BizLicenceRowFields.RestrictionTxt,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 320,
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenceRowUntyped,
                FieldName = BizLicenceRowFields.BusinessNm,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 320,
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenceRowUntyped,
                FieldName = BizLicenceRowFields.MailingStreetAddressTxt,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 100,
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenceRowUntyped,
                FieldName = BizLicenceRowFields.MailingCityNm,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 100,
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenceRowUntyped,
                FieldName = BizLicenceRowFields.MailingProvinceCd,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 2,
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenceRowUntyped,
                FieldName = BizLicenceRowFields.MailingPostalCd,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 10,
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenceRowUntyped,
                FieldName = BizLicenceRowFields.BusinessOwnerNm,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 320,
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenceRowUntyped,
                FieldName = BizLicenceRowFields.BusinessOwnerPhoneNo,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 30,
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenceRowUntyped,
                FieldName = BizLicenceRowFields.BusinessOwnerEmailAddressDsc,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 320,
                RegexInfo = RegexDefs.GetRegexInfo(RegexDefs.Email) 
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenceRowUntyped,
                FieldName = BizLicenceRowFields.InfractionTxt,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 320,
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenceRowUntyped,
                FieldName = BizLicenceRowFields.InfractionDt,
                FieldType = FieldTypes.String,
                Required = false,
                RegexInfo = RegexDefs.GetRegexInfo(RegexDefs.YearMonthDay) 
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenceRowUntyped,
                FieldName = BizLicenceRowFields.AvailableBedroomsQty,
                FieldType = FieldTypes.String,
                Required = false,
                RegexInfo = RegexDefs.GetRegexInfo(RegexDefs.Quantity)
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenceRowUntyped,
                FieldName = BizLicenceRowFields.MaxGuestsAllowedQty,
                FieldType = FieldTypes.String,
                Required = false,
                RegexInfo = RegexDefs.GetRegexInfo(RegexDefs.Quantity)
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenceRowUntyped,
                FieldName = BizLicenceRowFields.IsPrincipalResidence,
                FieldType = FieldTypes.String,
                Required = false,
                RegexInfo = RegexDefs.GetRegexInfo(RegexDefs.YN)
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenceRowUntyped,
                FieldName = BizLicenceRowFields.IsOwnerLivingOnsite,
                FieldType = FieldTypes.String,
                Required = false,
                RegexInfo = RegexDefs.GetRegexInfo(RegexDefs.YN)
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenceRowUntyped,
                FieldName = BizLicenceRowFields.IsOwnerPropertyTenant,
                FieldType = FieldTypes.String,
                Required = false,
                RegexInfo = RegexDefs.GetRegexInfo(RegexDefs.YN)
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenceRowUntyped,
                FieldName = BizLicenceRowFields.PropertyFolioNo,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 30,
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenceRowUntyped,
                FieldName = BizLicenceRowFields.PropertyParcelIdentifierNo,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 30,
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.BizLicenceRowUntyped,
                FieldName = BizLicenceRowFields.PropertyLegalDescriptionTxt,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 320,
            });
        }
    }
}
