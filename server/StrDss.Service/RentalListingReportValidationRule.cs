using StrDss.Common;

namespace StrDss.Service
{
    public static class RentalListingReportValidationRule
    {
        public static void LoadReportValidationRules(List<FieldValidationRule> rules)
        {
            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RentalListingRowUntyped,
                FieldName = RentalListingReportFields.RptPeriod,
                FieldType = FieldTypes.String,
                Required = true,
                RegexInfo = RegexDefs.GetRegexInfo(RegexDefs.YearMonth)
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RentalListingRowUntyped,
                FieldName = RentalListingReportFields.OrgCd,
                FieldType = FieldTypes.String,
                Required = true,
                MaxLength = 25,
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RentalListingRowUntyped,
                FieldName = RentalListingReportFields.ListingId,
                FieldType = FieldTypes.String,
                Required = true,
                MaxLength = 25,
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RentalListingRowUntyped,
                FieldName = RentalListingReportFields.ListingUrl,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 4000,
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RentalListingRowUntyped,
                FieldName = RentalListingReportFields.RentalAddress,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 250
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RentalListingRowUntyped,
                FieldName = RentalListingReportFields.BusLicNo,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 320
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RentalListingRowUntyped,
                FieldName = RentalListingReportFields.BcRegNo,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 25
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RentalListingRowUntyped,
                FieldName = RentalListingReportFields.IsEntireUnit,
                FieldType = FieldTypes.String,
                Required = false,
                RegexInfo = RegexDefs.GetRegexInfo(RegexDefs.YN)
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RentalListingRowUntyped,
                FieldName = RentalListingReportFields.BedroomsQty,
                FieldType = FieldTypes.String,
                Required = false,
                RegexInfo = RegexDefs.GetRegexInfo(RegexDefs.Quantity)
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RentalListingRowUntyped,
                FieldName = RentalListingReportFields.NightsBookedQty,
                FieldType = FieldTypes.String,
                Required = false,
                RegexInfo = RegexDefs.GetRegexInfo(RegexDefs.Quantity)
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RentalListingRowUntyped,
                FieldName = RentalListingReportFields.ReservationsQty,
                FieldType = FieldTypes.String,
                Required = false,
                RegexInfo = RegexDefs.GetRegexInfo(RegexDefs.Quantity)
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RentalListingRowUntyped,
                FieldName = RentalListingReportFields.PropertyHostNm,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 320
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RentalListingRowUntyped,
                FieldName = RentalListingReportFields.PropertyHostEmail,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 320
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RentalListingRowUntyped,
                FieldName = RentalListingReportFields.PropertyHostPhone,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 30
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RentalListingRowUntyped,
                FieldName = RentalListingReportFields.PropertyHostFax,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 30
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RentalListingRowUntyped,
                FieldName = RentalListingReportFields.PropertyHostAddress,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 250
            });

            //supplier #1

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RentalListingRowUntyped,
                FieldName = RentalListingReportFields.SupplierHost1Nm,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 320
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RentalListingRowUntyped,
                FieldName = RentalListingReportFields.SupplierHost1Email,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 320
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RentalListingRowUntyped,
                FieldName = RentalListingReportFields.SupplierHost1Phone,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 30
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RentalListingRowUntyped,
                FieldName = RentalListingReportFields.SupplierHost1Fax,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 30
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RentalListingRowUntyped,
                FieldName = RentalListingReportFields.SupplierHost1Address,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 250
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RentalListingRowUntyped,
                FieldName = RentalListingReportFields.SupplierHost1Id,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 25
            });

            // supplier #2

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RentalListingRowUntyped,
                FieldName = RentalListingReportFields.SupplierHost2Nm,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 320
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RentalListingRowUntyped,
                FieldName = RentalListingReportFields.SupplierHost2Email,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 320
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RentalListingRowUntyped,
                FieldName = RentalListingReportFields.SupplierHost2Phone,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 30
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RentalListingRowUntyped,
                FieldName = RentalListingReportFields.SupplierHost2Fax,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 30
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RentalListingRowUntyped,
                FieldName = RentalListingReportFields.SupplierHost2Address,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 250
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RentalListingRowUntyped,
                FieldName = RentalListingReportFields.SupplierHost2Id,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 25
            });

            // supplier #3

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RentalListingRowUntyped,
                FieldName = RentalListingReportFields.SupplierHost3Nm,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 320
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RentalListingRowUntyped,
                FieldName = RentalListingReportFields.SupplierHost3Email,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 320
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RentalListingRowUntyped,
                FieldName = RentalListingReportFields.SupplierHost3Phone,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 30
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RentalListingRowUntyped,
                FieldName = RentalListingReportFields.SupplierHost3Fax,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 30
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RentalListingRowUntyped,
                FieldName = RentalListingReportFields.SupplierHost3Address,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 250
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RentalListingRowUntyped,
                FieldName = RentalListingReportFields.SupplierHost3Id,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 25
            });

            //supplier 4

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RentalListingRowUntyped,
                FieldName = RentalListingReportFields.SupplierHost4Nm,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 320
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RentalListingRowUntyped,
                FieldName = RentalListingReportFields.SupplierHost4Email,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 320
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RentalListingRowUntyped,
                FieldName = RentalListingReportFields.SupplierHost4Phone,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 30
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RentalListingRowUntyped,
                FieldName = RentalListingReportFields.SupplierHost4Fax,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 30
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RentalListingRowUntyped,
                FieldName = RentalListingReportFields.SupplierHost4Address,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 250
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RentalListingRowUntyped,
                FieldName = RentalListingReportFields.SupplierHost4Id,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 25
            });

            // supplier 5

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RentalListingRowUntyped,
                FieldName = RentalListingReportFields.SupplierHost5Nm,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 320
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RentalListingRowUntyped,
                FieldName = RentalListingReportFields.SupplierHost5Email,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 320
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RentalListingRowUntyped,
                FieldName = RentalListingReportFields.SupplierHost5Phone,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 30
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RentalListingRowUntyped,
                FieldName = RentalListingReportFields.SupplierHost5Fax,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 30
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RentalListingRowUntyped,
                FieldName = RentalListingReportFields.SupplierHost5Address,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 250
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RentalListingRowUntyped,
                FieldName = RentalListingReportFields.SupplierHost5Id,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 25
            });
        }
    }
}
