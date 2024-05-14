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
        public const string RentalListingRowUntyped = "RentalListingRowUntyped";
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

    public static class RentalListingReportFields
    {
        public const string RptPeriod = "RptPeriod";
        public const string OrgCd = "OrgCd";
        public const string ListingId = "ListingId";
        public const string ListingUrl = "ListingUrl";
        public const string RentalAddress = "RentalAddress";
        public const string BusLicNo = "BusLicNo";
        public const string BcRegNo = "BcRegNo";
        public const string IsEntireUnit = "IsEntireUnit";
        public const string BedroomsQty = "BedroomsQty";
        public const string NightsBookedQty = "NightsBookedQty";
        public const string ReservationsQty = "ReservationsQty";
        public const string PropertyHostNm = "PropertyHostNm";
        public const string PropertyHostEmail = "PropertyHostEmail";
        public const string PropertyHostPhone = "PropertyHostPhone";
        public const string PropertyHostFax = "PropertyHostFax";
        public const string PropertyHostAddress = "PropertyHostAddress";
        public const string SupplierHost1Nm = "SupplierHost1Nm";
        public const string SupplierHost1Email = "SupplierHost1Email";
        public const string SupplierHost1Phone = "SupplierHost1Phone";
        public const string SupplierHost1Fax = "SupplierHost1Fax";
        public const string SupplierHost1Address = "SupplierHost1Address";
        public const string SupplierHost1Id = "SupplierHost1Id";
        public const string SupplierHost2Nm = "SupplierHost2Nm";
        public const string SupplierHost2Email = "SupplierHost2Email";
        public const string SupplierHost2Phone = "SupplierHost2Phone";
        public const string SupplierHost2Fax = "SupplierHost2Fax";
        public const string SupplierHost2Address = "SupplierHost2Address";
        public const string SupplierHost2Id = "SupplierHost2Id";
        public const string SupplierHost3Nm = "SupplierHost3Nm";
        public const string SupplierHost3Email = "SupplierHost3Email";
        public const string SupplierHost3Phone = "SupplierHost3Phone";
        public const string SupplierHost3Fax = "SupplierHost3Fax";
        public const string SupplierHost3Address = "SupplierHost3Address";
        public const string SupplierHost3Id = "SupplierHost3Id";
        public const string SupplierHost4Nm = "SupplierHost4Nm";
        public const string SupplierHost4Email = "SupplierHost4Email";
        public const string SupplierHost4Phone = "SupplierHost4Phone";
        public const string SupplierHost4Fax = "SupplierHost4Fax";
        public const string SupplierHost4Address = "SupplierHost4Address";
        public const string SupplierHost4Id = "SupplierHost4Id";
        public const string SupplierHost5Nm = "SupplierHost5Nm";
        public const string SupplierHost5Email = "SupplierHost5Email";
        public const string SupplierHost5Phone = "SupplierHost5Phone";
        public const string SupplierHost5Fax = "SupplierHost5Fax";
        public const string SupplierHost5Address = "SupplierHost5Address";
        public const string SupplierHost5Id = "SupplierHost5Id";
    }


    public static class AccessRequestStatuses
    {
        public const string Requested = "Requested";
        public const string Approved = "Approved";
        public const string Denied = "Denied";
        public const string None = "None";
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

    public static class StrDssIdProviders
    {
        public const string Idir = "idir";
        public const string BceidBusiness = "bceidbusiness";
        public const string External = "external";
        public const string Aps = "aps";

        public const string StrDss = "strdss";
        public static string GetBceidUserType(string userType)
        {
            switch (userType.ToLowerInvariant())
            {
                case StrDssIdProviders.Idir:
                    return BceidUserTypes.Internal;
                case StrDssIdProviders.BceidBusiness:
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
    public static class OrganizationTypes
    {
        public const string BCGov = "BCGov";
        public const string Platform = "Platform";
        public const string LG = "LG";
    }

    public static class Roles
    {
        public const string CeuAdmin = "ceu_admin";
        public const string CeuStaff = "ceu_staff";
        public const string LgStaff = "lg_staff";
        public const string PlatformStaff = "platform_staff";
    }

    public static class NoReply
    {
        public const string Default = "no_reply@gov.bc.ca";
    }

    public static class EmailMessageTypes
    {
        public const string ComplianceOrder = "Compliance Order";
        public const string AccessGranted = "Access Granted";
        public const string EscalationRequest = "Escalation Request";
        public const string TakedownRequest = "Takedown Request";
        public const string AccessDenied = "Access Denied";
        public const string NoticeOfTakedown = "Notice of Takedown";
        public const string AccessRequested = "Access Requested";
        public const string BatchTakedownRequest = "Batch Takedown Request";
        public const string ListingUploadError = "Listing Upload Error";
    }

    public static class Permissions
    {
        public const string CeuAction = "ceu_action";
        public const string AuditRead = "audit_read";
        public const string ListingRead = "listing_read";
        public const string TakedownAction = "takedown_action";
        public const string UserRead = "user_read";
        public const string LicenceFileUpload = "licence_file_upload";
        public const string ListingFileUpload = "listing_file_upload";
        public const string UserWrite = "user_write";
    }

    public static class UploadDeliveryTypes
    {
        public const string RentalReport = "rental_report";
    }

}
