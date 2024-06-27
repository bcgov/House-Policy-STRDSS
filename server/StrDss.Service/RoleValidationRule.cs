using StrDss.Common;

namespace StrDss.Service
{
    public class RoleValidationRule
    {
        public static void LoadReportValidationRules(List<FieldValidationRule> rules)
        {
            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.Role,
                FieldName = RoleFields.UserRoleCd,
                FieldType = FieldTypes.String,
                Required = true,
                MaxLength = 25,
                MinLength = 1,
            });

            rules.Add(new FieldValidationRule
            {
                EntityName = Entities.Role,
                FieldName = RoleFields.UserRoleNm,
                FieldType = FieldTypes.String,
                Required = true,
                MaxLength = 250,
                MinLength = 1,
            });
        }
    }
}
