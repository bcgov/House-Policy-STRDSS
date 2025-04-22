namespace StrDss.Model.OrganizationDtos
{
    public class JurisdictionUpdateDto
    {
        public long OrganizationId { get; set; }
        public long? ManagingOrganizationId { get; set; }
        public bool? IsPrincipalResidenceRequired { get; set; }
        public bool? IsStrProhibited { get; set; }
        public bool? IsBusinessLicenceRequired { get; set; }
        public string? EconomicRegionDsc { get; set; }
        public bool? IsStraaExempt { get; set; }
        public bool? IsActive { get; set; }
        public DateTime UpdDtm { get; set; }
    }
}
