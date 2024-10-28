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
        public const string RentalListingRowUntyped = "RentalListingRowUntyped";
        public const string Role = "Role";
        public const string BizLicenceRowUntyped = "BizLicenceRowUntyped";
        public const string Platform = "Platform";
        public const string LocalGov = "LocalGov";
    }
    public static class Fields
    {
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

    public static class BizLicenceRowFields
    {
        public const string OrgCd = "OrgCd";
        public const string BusinessLicenceNo = "BusinessLicenceNo";
        public const string ExpiryDt = "ExpiryDt";
        public const string PhysicalRentalAddressTxt = "PhysicalRentalAddressTxt";
        public const string LicenceStatusType = "LicenceStatusType";
        public const string LicenceTypeTxt = "LicenceTypeTxt";
        public const string RestrictionTxt = "RestrictionTxt";
        public const string BusinessNm = "BusinessNm";
        public const string MailingStreetAddressTxt = "MailingStreetAddressTxt";
        public const string MailingCityNm = "MailingCityNm";
        public const string MailingProvinceCd = "MailingProvinceCd";
        public const string MailingPostalCd = "MailingPostalCd";
        public const string BusinessOwnerNm = "BusinessOwnerNm";
        public const string BusinessOwnerPhoneNo = "BusinessOwnerPhoneNo";
        public const string BusinessOwnerEmailAddressDsc = "BusinessOwnerEmailAddressDsc";
        public const string BusinessOperatorNm = "BusinessOperatorNm";
        public const string BusinessOperatorPhoneNo = "BusinessOperatorPhoneNo";
        public const string BusinessOperatorEmailAddressDsc = "BusinessOperatorEmailAddressDsc";
        public const string InfractionTxt = "InfractionTxt";
        public const string InfractionDt = "InfractionDt";
        public const string PropertyZoneTxt = "PropertyZoneTxt";
        public const string AvailableBedroomsQty = "AvailableBedroomsQty";
        public const string MaxGuestsAllowedQty = "MaxGuestsAllowedQty";
        public const string IsPrincipalResidence = "IsPrincipalResidence";
        public const string IsOwnerLivingOnsite = "IsOwnerLivingOnsite";
        public const string IsOwnerPropertyTenant = "IsOwnerPropertyTenant";
        public const string PropertyFolioNo = "PropertyFolioNo";
        public const string PropertyParcelIdentifierNo = "PropertyParcelIdentifierNo";
        public const string PropertyLegalDescriptionTxt = "PropertyLegalDescriptionTxt";
    }

    public static class PlatformFields
    {
        public const string OrganizationId = "OrganizationId";
        public const string OrganizationCd = "OrganizationCd";
        public const string OrganizationNm = "OrganizationNm";
        public const string PrimaryNoticeOfTakedownContactEmail = "PrimaryNoticeOfTakedownContactEmail";
        public const string PrimaryTakedownRequestContactEmail = "PrimaryTakedownRequestContactEmail";
        public const string SecondaryNoticeOfTakedownContactEmail = "SecondaryNoticeOfTakedownContactEmail";
        public const string SecondaryTakedownRequestContactEmail = "SecondaryTakedownRequestContactEmail";
        public const string PlatformType = "PlatformType";
    }

    public static class LocalGovFields
    {
        public const string OrganizationId = "OrganizationId";
        public const string OrganizationCd = "OrganizationCd";
        public const string OrganizationNm = "OrganizationNm";
        public const string LocalGovernmentType = "LocalGovernmentType";
        public const string BusinessLicenceFormatTxt = "BusinessLicenceFormatTxt";
    }

    public static class CsvCols
    {
        public const string MostRecentPlatformReportMonth = "Most Recent Platform Report Month";
        public const string Status = "Status";
        public const string JurisdictionAssignedTo = "Jurisdiction assigned to";
        public const string EconomicRegionName = "economic_region_name";
        public const string PrRequirement = "pr_requirement";
        public const string BlRequirement = "BL_requirement";
        public const string PlatformCode = "Platform Code";
        public const string ListingId = "Listing ID";
        public const string UrlAddress = "URL Address";
        public const string PlatformListingAddress = "Platform listing address";
        public const string GeocoderBestMatchAddressComplete = "Geocoder Best match address (current month) - complete address";
        public const string GeocoderBestMatchAddressCity = "Geocoder Best match address (current month) -city only";
        public const string LocalGovernmentBusinessLicenceNumber = "Local Government Business Licence Number";
        public const string EntireUnit = "Entire Unit";
        public const string NumberOfBedroomsAvailableForStr = "Number of Bedrooms available for STR";
        public const string CurrentMonth = "Current Month";
        public const string NumberOfNightsBookedCurrentMonth = "Number of nights booked (Current month)";
        public const string NumberOfNightsBookedPreviousMonth1 = "Number of nights booked (Current month - 1)";
        public const string NumberOfNightsBookedPreviousMonth2 = "Number of nights booked (Current month - 2)";
        public const string NumberOfNightsBookedPreviousMonth3 = "Number of nights booked (Current month - 3)";
        public const string NumberOfNightsBookedPreviousMonth4 = "Number of nights booked (Current month - 4)";
        public const string NumberOfNightsBookedPreviousMonth5 = "Number of nights booked (Current month - 5)";
        public const string NumberOfNightsBookedPreviousMonth6 = "Number of nights booked (Current month - 6)";
        public const string NumberOfNightsBookedPreviousMonth7 = "Number of nights booked (Current month - 7)";
        public const string NumberOfNightsBookedPreviousMonth8 = "Number of nights booked (Current month - 8)";
        public const string NumberOfNightsBookedPreviousMonth9 = "Number of nights booked (Current month - 9)";
        public const string NumberOfNightsBookedPreviousMonth10 = "Number of nights booked (Current month - 10)";
        public const string NumberOfNightsBookedPreviousMonth11 = "Number of nights booked (Current month - 11)";
        public const string NumberOfSeparateReservationsCurrentMonth = "Number of separate reservations (Current month)";
        public const string NumberOfSeparateReservationsPreviousMonth1 = "Number of separate reservations (Current month - 1)";
        public const string NumberOfSeparateReservationsPreviousMonth2 = "Number of separate reservations (Current month - 2)";
        public const string NumberOfSeparateReservationsPreviousMonth3 = "Number of separate reservations (Current month - 3)";
        public const string NumberOfSeparateReservationsPreviousMonth4 = "Number of separate reservations (Current month - 4)";
        public const string NumberOfSeparateReservationsPreviousMonth5 = "Number of separate reservations (Current month - 5)";
        public const string NumberOfSeparateReservationsPreviousMonth6 = "Number of separate reservations (Current month - 6)";
        public const string NumberOfSeparateReservationsPreviousMonth7 = "Number of separate reservations (Current month - 7)";
        public const string NumberOfSeparateReservationsPreviousMonth8 = "Number of separate reservations (Current month - 8)";
        public const string NumberOfSeparateReservationsPreviousMonth9 = "Number of separate reservations (Current month - 9)";
        public const string NumberOfSeparateReservationsPreviousMonth10 = "Number of separate reservations (Current month - 10)";
        public const string NumberOfSeparateReservationsPreviousMonth11 = "Number of separate reservations (Current month - 11)";
        public const string PropertyHostName = "Property Host name";
        public const string PropertyHostEmailAddress = "Property Host email address";
        public const string PropertyHostPhoneNumber = "Property Host phone number";
        public const string PropertyHostFaxNumber = "Property Host fax number";
        public const string PropertyHostMailingAddress = "Property Host Mailing Address";
        public const string SupplierHost1Name = "Supplier Host 1 name";
        public const string SupplierHost1EmailAddress = "Supplier Host 1 email address";
        public const string SupplierHost1PhoneNumber = "Supplier Host 1 phone number";
        public const string SupplierHost1FaxNumber = "Supplier Host 1 fax number";
        public const string SupplierHost1MailingAddress = "Supplier Host 1 Mailing Address";
        public const string HostIdOfSupplierHost1 = "Host ID of Supplier Host 1";
        public const string SupplierHost2Name = "Supplier Host 2 name";
        public const string SupplierHost2EmailAddress = "Supplier Host 2 email address";
        public const string SupplierHost2PhoneNumber = "Supplier Host 2 phone number";
        public const string SupplierHost2FaxNumber = "Supplier Host 2 fax number";
        public const string SupplierHost2MailingAddress = "Supplier Host 2 Mailing Address";
        public const string HostIdOfSupplierHost2 = "Host ID of Supplier Host 2";
        public const string SupplierHost3Name = "Supplier Host 3 name";
        public const string SupplierHost3EmailAddress = "Supplier Host 3 email address";
        public const string SupplierHost3PhoneNumber = "Supplier Host 3 phone number";
        public const string SupplierHost3FaxNumber = "Supplier Host 3 fax number";
        public const string SupplierHost3MailingAddress = "Supplier Host 3 Mailing Address";
        public const string HostIdOfSupplierHost3 = "Host ID of Supplier Host 3";
        public const string SupplierHost4Name = "Supplier Host 4 name";
        public const string SupplierHost4EmailAddress = "Supplier Host 4 email address";
        public const string SupplierHost4PhoneNumber = "Supplier Host 4 phone number";
        public const string SupplierHost4FaxNumber = "Supplier Host 4 fax number";
        public const string SupplierHost4MailingAddress = "Supplier Host 4 Mailing Address";
        public const string HostIdOfSupplierHost4 = "Host ID of Supplier Host 4";
        public const string SupplierHost5Name = "Supplier Host 5 name";
        public const string SupplierHost5EmailAddress = "Supplier Host 5 email address";
        public const string SupplierHost5PhoneNumber = "Supplier Host 5 phone number";
        public const string SupplierHost5FaxNumber = "Supplier Host 5 fax number";
        public const string SupplierHost5MailingAddress = "Supplier Host 5 Mailing Address";
        public const string HostIdOfSupplierHost5 = "Host ID of Supplier Host 5";
        public const string LastActionTaken = "Last Action Taken";
        public const string DateOfLastActionTaken = "Date of Last Action Taken";
        public const string PreviousActionTaken1 = "Previous Action Taken 1";
        public const string DateOfPreviousActionTaken1 = "Date of Previous Action Taken 1";
        public const string PreviousActionTaken2 = "Previous Action Taken 2";
        public const string DateOfPreviousActionTaken2 = "Date of Previous Action Taken 2";
    }

    public static class RentalListingExport
    {
        public static readonly string[] Headers = new string[]
        {
            CsvCols.MostRecentPlatformReportMonth,
            CsvCols.Status,
            CsvCols.JurisdictionAssignedTo,
            CsvCols.EconomicRegionName,
            CsvCols.PrRequirement,
            CsvCols.BlRequirement,
            CsvCols.PlatformCode,
            CsvCols.ListingId,
            CsvCols.UrlAddress,
            CsvCols.PlatformListingAddress,
            CsvCols.GeocoderBestMatchAddressComplete,
            CsvCols.GeocoderBestMatchAddressCity,
            CsvCols.LocalGovernmentBusinessLicenceNumber,
            CsvCols.EntireUnit,
            CsvCols.NumberOfBedroomsAvailableForStr,
            CsvCols.CurrentMonth,
            CsvCols.NumberOfNightsBookedCurrentMonth,
            CsvCols.NumberOfNightsBookedPreviousMonth1,
            CsvCols.NumberOfNightsBookedPreviousMonth2,
            CsvCols.NumberOfNightsBookedPreviousMonth3,
            CsvCols.NumberOfNightsBookedPreviousMonth4,
            CsvCols.NumberOfNightsBookedPreviousMonth5,
            CsvCols.NumberOfNightsBookedPreviousMonth6,
            CsvCols.NumberOfNightsBookedPreviousMonth7,
            CsvCols.NumberOfNightsBookedPreviousMonth8,
            CsvCols.NumberOfNightsBookedPreviousMonth9,
            CsvCols.NumberOfNightsBookedPreviousMonth10,
            CsvCols.NumberOfNightsBookedPreviousMonth11,
            CsvCols.NumberOfSeparateReservationsCurrentMonth,
            CsvCols.NumberOfSeparateReservationsPreviousMonth1,
            CsvCols.NumberOfSeparateReservationsPreviousMonth2,
            CsvCols.NumberOfSeparateReservationsPreviousMonth3,
            CsvCols.NumberOfSeparateReservationsPreviousMonth4,
            CsvCols.NumberOfSeparateReservationsPreviousMonth5,
            CsvCols.NumberOfSeparateReservationsPreviousMonth6,
            CsvCols.NumberOfSeparateReservationsPreviousMonth7,
            CsvCols.NumberOfSeparateReservationsPreviousMonth8,
            CsvCols.NumberOfSeparateReservationsPreviousMonth9,
            CsvCols.NumberOfSeparateReservationsPreviousMonth10,
            CsvCols.NumberOfSeparateReservationsPreviousMonth11,
            CsvCols.PropertyHostName,
            CsvCols.PropertyHostEmailAddress,
            CsvCols.PropertyHostPhoneNumber,
            CsvCols.PropertyHostFaxNumber,
            CsvCols.PropertyHostMailingAddress,
            CsvCols.SupplierHost1Name,
            CsvCols.SupplierHost1EmailAddress,
            CsvCols.SupplierHost1PhoneNumber,
            CsvCols.SupplierHost1FaxNumber,
            CsvCols.SupplierHost1MailingAddress,
            CsvCols.HostIdOfSupplierHost1,
            CsvCols.SupplierHost2Name,
            CsvCols.SupplierHost2EmailAddress,
            CsvCols.SupplierHost2PhoneNumber,
            CsvCols.SupplierHost2FaxNumber,
            CsvCols.SupplierHost2MailingAddress,
            CsvCols.HostIdOfSupplierHost2,
            CsvCols.SupplierHost3Name,
            CsvCols.SupplierHost3EmailAddress,
            CsvCols.SupplierHost3PhoneNumber,
            CsvCols.SupplierHost3FaxNumber,
            CsvCols.SupplierHost3MailingAddress,
            CsvCols.HostIdOfSupplierHost3,
            CsvCols.SupplierHost4Name,
            CsvCols.SupplierHost4EmailAddress,
            CsvCols.SupplierHost4PhoneNumber,
            CsvCols.SupplierHost4FaxNumber,
            CsvCols.SupplierHost4MailingAddress,
            CsvCols.HostIdOfSupplierHost4,
            CsvCols.SupplierHost5Name,
            CsvCols.SupplierHost5EmailAddress,
            CsvCols.SupplierHost5PhoneNumber,
            CsvCols.SupplierHost5FaxNumber,
            CsvCols.SupplierHost5MailingAddress,
            CsvCols.HostIdOfSupplierHost5,
            CsvCols.LastActionTaken,
            CsvCols.DateOfLastActionTaken,
            CsvCols.PreviousActionTaken1,
            CsvCols.DateOfPreviousActionTaken1,
            CsvCols.PreviousActionTaken2,
            CsvCols.DateOfPreviousActionTaken2,
        };

        public static readonly string[] FinHeaders = new string[]
        {
            CsvCols.MostRecentPlatformReportMonth,
            CsvCols.Status,
            CsvCols.JurisdictionAssignedTo,
            CsvCols.EconomicRegionName,
            CsvCols.PrRequirement,
            CsvCols.BlRequirement,
            CsvCols.PlatformCode,
            CsvCols.ListingId,
            CsvCols.UrlAddress,
            CsvCols.PlatformListingAddress,
            CsvCols.GeocoderBestMatchAddressComplete,
            CsvCols.GeocoderBestMatchAddressCity,
            CsvCols.LocalGovernmentBusinessLicenceNumber,
            CsvCols.EntireUnit,
            CsvCols.NumberOfBedroomsAvailableForStr,
            CsvCols.CurrentMonth,
            CsvCols.NumberOfNightsBookedCurrentMonth,
            CsvCols.NumberOfNightsBookedPreviousMonth1,
            CsvCols.NumberOfNightsBookedPreviousMonth2,
            CsvCols.NumberOfNightsBookedPreviousMonth3,
            CsvCols.NumberOfNightsBookedPreviousMonth4,
            CsvCols.NumberOfNightsBookedPreviousMonth5,
            CsvCols.NumberOfNightsBookedPreviousMonth6,
            CsvCols.NumberOfNightsBookedPreviousMonth7,
            CsvCols.NumberOfNightsBookedPreviousMonth8,
            CsvCols.NumberOfNightsBookedPreviousMonth9,
            CsvCols.NumberOfNightsBookedPreviousMonth10,
            CsvCols.NumberOfNightsBookedPreviousMonth11,
            CsvCols.NumberOfSeparateReservationsCurrentMonth,
            CsvCols.NumberOfSeparateReservationsPreviousMonth1,
            CsvCols.NumberOfSeparateReservationsPreviousMonth2,
            CsvCols.NumberOfSeparateReservationsPreviousMonth3,
            CsvCols.NumberOfSeparateReservationsPreviousMonth4,
            CsvCols.NumberOfSeparateReservationsPreviousMonth5,
            CsvCols.NumberOfSeparateReservationsPreviousMonth6,
            CsvCols.NumberOfSeparateReservationsPreviousMonth7,
            CsvCols.NumberOfSeparateReservationsPreviousMonth8,
            CsvCols.NumberOfSeparateReservationsPreviousMonth9,
            CsvCols.NumberOfSeparateReservationsPreviousMonth10,
            CsvCols.NumberOfSeparateReservationsPreviousMonth11,
            CsvCols.PropertyHostName,
            CsvCols.PropertyHostEmailAddress,
            CsvCols.PropertyHostPhoneNumber,
            CsvCols.PropertyHostFaxNumber,
            CsvCols.PropertyHostMailingAddress,
            CsvCols.SupplierHost1Name,
            CsvCols.SupplierHost1EmailAddress,
            CsvCols.SupplierHost1PhoneNumber,
            CsvCols.SupplierHost1FaxNumber,
            CsvCols.SupplierHost1MailingAddress,
            CsvCols.HostIdOfSupplierHost1,
            CsvCols.SupplierHost2Name,
            CsvCols.SupplierHost2EmailAddress,
            CsvCols.SupplierHost2PhoneNumber,
            CsvCols.SupplierHost2FaxNumber,
            CsvCols.SupplierHost2MailingAddress,
            CsvCols.HostIdOfSupplierHost2,
            CsvCols.SupplierHost3Name,
            CsvCols.SupplierHost3EmailAddress,
            CsvCols.SupplierHost3PhoneNumber,
            CsvCols.SupplierHost3FaxNumber,
            CsvCols.SupplierHost3MailingAddress,
            CsvCols.HostIdOfSupplierHost3,
            CsvCols.SupplierHost4Name,
            CsvCols.SupplierHost4EmailAddress,
            CsvCols.SupplierHost4PhoneNumber,
            CsvCols.SupplierHost4FaxNumber,
            CsvCols.SupplierHost4MailingAddress,
            CsvCols.HostIdOfSupplierHost4,
            CsvCols.SupplierHost5Name,
            CsvCols.SupplierHost5EmailAddress,
            CsvCols.SupplierHost5PhoneNumber,
            CsvCols.SupplierHost5FaxNumber,
            CsvCols.SupplierHost5MailingAddress,
            CsvCols.HostIdOfSupplierHost5,
        };

        public static string GetHeadersAsCsv()
        {
            return string.Join(",", Headers);
        }

        public static string GetFinHeadersAsCsv()
        {
            return string.Join(",", FinHeaders);
        }
    }

    public static class RoleFields
    {
        public const string UserRoleCd = "UserRoleCd";
        public const string UserRoleNm = "UserRoleNm";
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
        public const string LicenceStatus = "Licence Status";
        public const string PlatformTypes = "Platform Types";
        public const string LocalGovTypes = "Local Gov Types";
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
        public const string ClientId = "client_id";
        public const string Sub = "sub";

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
        public const string LGSub = "LGSub";
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
        public const string CompletedTakedown = "Completed Takedown";
    }

    public static class Permissions
    {
        public const string UserRead = "user_read";
        public const string UserWrite = "user_write";
        public const string RoleRead = "role_read";
        public const string RoleWrite = "role_write";
        public const string ListingRead = "listing_read";
        public const string AddressWrite = "address_write";
        public const string LicenceFileUpload = "licence_file_upload";
        public const string ListingFileUpload = "listing_file_upload";
        public const string TakdownFileUpload = "takedown_file_upload";
        public const string UploadHistoryRead = "upload_history_read";
        public const string AuditRead = "audit_read";
        public const string TakedownAction = "takedown_action";
        public const string ProvinceAction = "province_action";
        public const string CeuAction = "ceu_action";
        public const string PlatformRead = "platform_read";
        public const string PlatformWrite = "platform_write";
        public const string RegistryView = "registry_view";
        public const string BlLinkWrite = "bl_link_write";
        public const string JurisdictionRead = "jurisdiction_read";
        public const string JurisdictionWrite = "jurisdiction_write";
    }

    public static class UploadDeliveryTypes
    {
        public const string ListingData = "Listing Data";
        public const string TakedownData = "Takedown Data";
        public const string LicenceData = "Licence Data";
    }

    public static class ListingExportFileNames
    {
        public const string All = "BC";
        public const string AllPr = "BC_PR";
        public const string Fin = "BC_FIN";
    }

    public static class ApiTags
    {
        public const string Default = "stadata";
        public const string Aps = "aps";
        public static readonly string[] ApsTagList = { "aps" };
    }
}
