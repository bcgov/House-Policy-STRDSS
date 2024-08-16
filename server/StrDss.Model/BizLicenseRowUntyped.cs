using CsvHelper.Configuration.Attributes;

namespace StrDss.Model
{
    public class BizLicenseRowUntyped
    {
        [Name("org_cd")]
        public string LocalGovernmentCode { get; set; }

        [Name("bus_lic_no")]
        public string LicenceNumber { get; set; }

        [Name("bus_lic_exp_dt")]
        public string LicenceExpiryDate { get; set; }

        [Name("rental_address")]
        public string STRAddress { get; set; }

        [Name("bus_lic_status")]
        public string LicenceStatus { get; set; }

        [Name("bus_lic_type")]
        public string LicenceType { get; set; }

        [Name("restriction_txt")]
        public string RestrictionNote { get; set; }

        [Name("business_nm")]
        public string BusinessName { get; set; }

        [Name("mail_street_addr_txt")]
        public string MailingAddress { get; set; }
        [Name("mail_city_nm")]
        public string MailingCity { get; set; }

        [Name("mail_province_cd")]
        public string MailingProvince { get; set; }

        [Name("postal_cd")]
        public string MailingPostalCode { get; set; }

        [Name("owner_nm")]
        public string OwnerName { get; set; }

        [Name("owner_phone")]
        public string OwnerPhone { get; set; }

        [Name("owner_email")]
        public string OwnerEmail { get; set; }

        [Name("operator_nm")]
        public string OperatorName { get; set; }

        [Name("operator_phone")]
        public string OperatorPhone { get; set; }

        [Name("operator_email")]
        public string OperatorEmail { get; set; }

        [Name("infraction_txt")]
        public string Infraction { get; set; }

        [Name("infraction_dt")]
        public string InfractionDate { get; set; }

        [Name("property_zone_txt")]
        public string PropertyZoning { get; set; }

        [Name("bedrooms_qty")]
        public string BedroomsAvailable { get; set; }

        [Name("max_guests_qty")]
        public string GuestsAllowed { get; set; }

        [Name("is_principal_res_yn")]
        public string IsPrincipalResidence { get; set; }

        [Name("is_owner_onsite_yn")]
        public string IsOwnerOnsite { get; set; }

        [Name("is_owner_tenant_yn")]
        public string IsOwnerTenant { get; set; }

        [Name("folio_no")]
        public string FolioNumber { get; set; }

        [Name("pid_no")]
        public string ParcelIdentifier { get; set; }

        [Name("legal_desc_txt")]
        public string LegalDescription { get; set; }
    }
}
