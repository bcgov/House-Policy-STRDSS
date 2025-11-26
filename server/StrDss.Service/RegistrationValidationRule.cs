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
                FieldName = RegistrationValidationFields.RentalAddress,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 250
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.RegNo,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 25
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.BcRegNo,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 25
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.RentalUnit,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 100
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.RentalStreet,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 25
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.RegistrationDataRowUntyped,
                FieldName = RegistrationValidationFields.RentalPostal,
                FieldType = FieldTypes.String,
                Required = false,
                MaxLength = 7
            });
        }
    }
}
