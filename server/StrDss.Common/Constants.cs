namespace StrDss.Common
{
    public class Constants
    {
        public static DateTime MaxDate = new DateTime(9999, 12, 31);
        public static DateTime MinDate = new DateTime(1900, 1, 1);
        public const string VancouverTimeZone = "America/Vancouver";
        public const string PacificTimeZone = "Pacific Standard Time";
    }
    public static class Entities
    {
        public const string SystemUser = "SystemUser";
        public const string StrApplication = "StrApplication";
        public const string Audit = "Audit";
    }
    public static class Fields
    {
        public const string StreetAddress = "StreetAddress";
        public const string City = "City";
        public const string Province = "Province";
        public const string PostalCode = "PostalCode";
        public const string ZoningTypeId = "ZoningTypeId";
        public const string SquareFootage = "SquareFootage";
        public const string StrAffiliateId = "StrAffiliateId";
        public const string IsOwnerPrimaryResidence = "IsOwnerPrimaryResidence";
        public const string ComplianceStatusId = "ComplianceStatusId";

        public const string Username = "Username";
        public const string Passwrod = "Passwrod";
        public const string LastName = "LastName";
        public const string PhoneNumber = "PhoneNumber";
        public const string RoleId = "RoleId";

    }
    public static class FieldTypes
    {
        public const string String = "S";
        public const string Decimal = "N";
        public const string Date = "D";
    }

    public static class CodeSet
    {
        public const string Role = "ROLE";
        public const string ZoneType = "ZONE_TYPE";
        public const string StrAffiliate = "STR_AFFILIATE";
        public const string ComplianceStatus = "COMPLIANCE_STATUS";
    }

    public static class StrDssUserTypes
    {
        public const string IDIR = "idir";
        public const string BceidBusiness = "bceidbusiness";
        public const string External = "external";
        public const string Aps = "aps";

        public const string StrDss = "strdss";
        public static string GetBceidUserType(string userType)
        {
            switch (userType.ToLowerInvariant())
            {
                case StrDssUserTypes.IDIR:
                    return BceidUserTypes.Internal;
                case StrDssUserTypes.BceidBusiness:
                    return BceidUserTypes.Business;
                default:
                    return "Unknown";
            }
        }
    }

    public static class BceidUserTypes
    {
        public const string Internal = "internal";
        public const string Business = "business";
    }

    public static class StrDssClaimTypes
    {
        public const string Permission = "str_dss_permission";
        public const string IdirUserGuid = "idir_user_guid";
        public const string BceidUserGuid = "bceid_user_guid";
        public const string StrDssUserGuid = "str_dss_user_guid";
        public const string IdirUsername = "idir_username";
        public const string BceidUsername = "bceid_username";
        public const string StrDssUsername = "str_dss_username";
        public const string IdentityProvider = "identity_provider";
        public const string BceidBusinessName = "bceid_business_name";
        public const string BceidBusinessGuid = "bceid_business_guid";
        public const string EmailVerified = "email_verified";
        public const string FullName = "full_name";
        public const string DisplayName = "display_name";
        public const string Title = "title";
        public const string SecurityIdentifierId = "security_identifier_id";
        public const string Expired = "expired";
        public const string StrDssCred = "str_dss_cred";
        public const string ClientId = "clientId";

        public static string GetSimpleName(string fullName)
        {
            return fullName.Contains("/") ? fullName.Substring(fullName.LastIndexOf("/") + 1) : fullName;
        }
    }
}
