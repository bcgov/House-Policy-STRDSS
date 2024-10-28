namespace StrDss.Model.OrganizationDtos
{
    public class LocalGovViewDto
    {
        public LocalGovViewDto()
        {
            Jurisdictions = new List<JurisdictionsViewDto>();
        }
        public long OrganizationId { get; set; }
        public string OrganizationNm { get; set; } = "";
        public string OrganizationType { get; set; } = "";
        public string LocalGovernmentType { get; set; } = "";
        public string LocalGovernmentTypeNm { get; set; } = "";
        public string OrganizationCd { get; set; } = "";
        public string BusinessLicenceFormatTxt { get; set; } = "";
        public DateTime UpdDtm { get; set; }
        public virtual ICollection<JurisdictionsViewDto> Jurisdictions { get; set; }
    }
}
