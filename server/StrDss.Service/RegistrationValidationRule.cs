using StrDss.Common;

namespace StrDss.Service
{
    public static class RegistrationValidationRule
    {
        public static void LoadRegistrationValidationRules(List<FieldValidationRule> rules)
        {
            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.RptPeriod,
                FieldType = FieldTypes.String,
                Required = false,
                RegexInfo = RegexDefs.GetRegexInfo(RegexDefs.YearMonth)
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.OrgCd,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 25,
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.ListingId,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 25,
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.ListingUrl,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 4000,
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.RentalAddress,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 250
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.BusLicNo,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 320
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.BcRegNo,
                FieldType = FieldTypes.String,
                Required = true,
                MaxLength = 25
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.RentalUnit,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 25
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.RentalStreet,
                FieldType = FieldTypes.String,
                Required = true,
                MaxLength = 25
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.RentalPostal,
                FieldType = FieldTypes.String,
                Required = true,
                MaxLength = 6
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.IsEntireUnit,
                FieldType = FieldTypes.String,
                Required = false,
                RegexInfo = RegexDefs.GetRegexInfo(RegexDefs.YN)
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.BedroomsQty,
                FieldType = FieldTypes.String,
                Required = false,
                RegexInfo = RegexDefs.GetRegexInfo(RegexDefs.Quantity)
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.NightsBookedQty,
                FieldType = FieldTypes.String,
                Required = false,
                RegexInfo = RegexDefs.GetRegexInfo(RegexDefs.Quantity)
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.ReservationsQty,
                FieldType = FieldTypes.String,
                Required = false,
                RegexInfo = RegexDefs.GetRegexInfo(RegexDefs.Quantity)
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.PropertyHostNm,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 320
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.PropertyHostEmail,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 320
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.PropertyHostPhone,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 30
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.PropertyHostFax,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 30
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.PropertyHostAddress,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 250
            });

            //supplier #1

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.SupplierHost1Nm,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 320
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.SupplierHost1Email,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 320
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.SupplierHost1Phone,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 30
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.SupplierHost1Fax,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 30
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.SupplierHost1Address,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 250
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.SupplierHost1Id,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 25
            });

            // supplier #2

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.SupplierHost2Nm,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 320
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.SupplierHost2Email,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 320
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.SupplierHost2Phone,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 30
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.SupplierHost2Fax,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 30
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.SupplierHost2Address,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 250
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.SupplierHost2Id,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 25
            });

            // supplier #3

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.SupplierHost3Nm,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 320
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.SupplierHost3Email,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 320
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.SupplierHost3Phone,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 30
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.SupplierHost3Fax,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 30
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.SupplierHost3Address,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 250
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.SupplierHost3Id,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 25
            });

            //supplier 4

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.SupplierHost4Nm,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 320
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.SupplierHost4Email,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 320
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.SupplierHost4Phone,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 30
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.SupplierHost4Fax,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 30
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.SupplierHost4Address,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 250
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.SupplierHost4Id,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 25
            });

            // supplier 5

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.SupplierHost5Nm,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 320
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.SupplierHost5Email,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 320
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.SupplierHost5Phone,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 30
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.SupplierHost5Fax,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 30
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.SupplierHost5Address,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 250
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.SupplierHost5Id,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 25
            });
        }
    }
}
