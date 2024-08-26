namespace StrDss.Model
{
    public class BizLicenceDto
    {
        public long BusinessLicenceId { get; set; }
        public string BusinessLicenceNo { get; set; } = null!;
        public DateOnly ExpiryDt { get; set; }
        public string? PhysicalRentalAddressTxt { get; set; }
        public string? LicenceTypeTxt { get; set; }
        public string? RestrictionTxt { get; set; }
        public string? BusinessNm { get; set; }
        public string? MailingStreetAddressTxt { get; set; }
        public string? MailingCityNm { get; set; }
        public string? MailingProvinceCd { get; set; }
        public string? MailingPostalCd { get; set; }
        public string? BusinessOwnerNm { get; set; }
        public string? BusinessOwnerPhoneNo { get; set; }
        public string? BusinessOwnerEmailAddressDsc { get; set; }
        public string? BusinessOperatorNm { get; set; }
        public string? BusinessOperatorPhoneNo { get; set; }
        public string? BusinessOperatorEmailAddressDsc { get; set; }
        public string? InfractionTxt { get; set; }
        public DateOnly? InfractionDt { get; set; }
        public string? PropertyZoneTxt { get; set; }
        public short? AvailableBedroomsQty { get; set; }
        public short? MaxGuestsAllowedQty { get; set; }
        public bool? IsPrincipalResidence { get; set; }
        public bool? IsOwnerLivingOnsite { get; set; }
        public bool? IsOwnerPropertyTenant { get; set; }
        public string? PropertyFolioNo { get; set; }
        public string? PropertyParcelIdentifierNo { get; set; }
        public string? PropertyLegalDescriptionTxt { get; set; }
        public string LicenceStatusType { get; set; } = null!;
        public long ProvidingOrganizationId { get; set; }
        public long? AffectedByPhysicalAddressId { get; set; }
        public DateTime UpdDtm { get; set; }
        public Guid? UpdUserGuid { get; set; }
        public virtual LicenceStatus LicenceStatus { get; set; } = null!;
    }
}
