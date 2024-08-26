using CsvHelper.Configuration.Attributes;

namespace StrDss.Model
{
    public class BizLicenceRowUntyped
    {
        [Name("org_cd")]
        public string OrgCd { get; set; }

        [Name("bus_lic_no")]
        public string BusinessLicenceNo { get; set; }

        [Name("bus_lic_exp_dt")]
        public string ExpiryDt { get; set; } 

        [Name("rental_address")]
        public string PhysicalRentalAddressTxt { get; set; }

        [Name("bus_lic_status")]
        public string LicenceStatusType { get; set; }

        [Name("bus_lic_type")]
        public string LicenceTypeTxt { get; set; }

        [Name("restriction_txt")]
        public string RestrictionTxt { get; set; }

        [Name("business_nm")]
        public string BusinessNm { get; set; }

        [Name("mail_street_addr_txt")]
        public string MailingStreetAddressTxt { get; set; }

        [Name("mail_city_nm")]
        public string MailingCityNm { get; set; }

        [Name("mail_province_cd")]
        public string MailingProvinceCd { get; set; }

        [Name("postal_cd")]
        public string MailingPostalCd { get; set; }

        [Name("owner_nm")]
        public string BusinessOwnerNm { get; set; }

        [Name("owner_phone")]
        public string BusinessOwnerPhoneNo { get; set; }

        [Name("owner_email")]
        public string BusinessOwnerEmailAddressDsc { get; set; }

        [Name("operator_nm")]
        public string BusinessOperatorNm { get; set; }

        [Name("operator_phone")]
        public string BusinessOperatorPhoneNo { get; set; }

        [Name("operator_email")]
        public string BusinessOperatorEmailAddressDsc { get; set; }

        [Name("infraction_txt")]
        public string InfractionTxt { get; set; }

        [Name("infraction_dt")]
        public string InfractionDt { get; set; }

        [Name("property_zone_txt")]
        public string PropertyZoneTxt { get; set; }

        [Name("bedrooms_qty")]
        public string AvailableBedroomsQty { get; set; }

        [Name("max_guests_qty")]
        public string MaxGuestsAllowedQty { get; set; }

        [Name("is_principal_res_yn")]
        public string IsPrincipalResidence { get; set; }

        [Name("is_owner_onsite_yn")]
        public string IsOwnerLivingOnsite { get; set; }

        [Name("is_owner_tenant_yn")]
        public string IsOwnerPropertyTenant { get; set; }

        [Name("folio_no")]
        public string PropertyFolioNo { get; set; }

        [Name("pid_no")]
        public string PropertyParcelIdentifierNo { get; set; }

        [Name("legal_desc_txt")]
        public string PropertyLegalDescriptionTxt { get; set; }
    }
}

