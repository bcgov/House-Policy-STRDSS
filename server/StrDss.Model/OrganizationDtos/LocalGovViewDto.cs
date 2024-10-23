namespace StrDss.Model.OrganizationDtos
{
    public class LocalGovViewDto
    {
        public LocalGovViewDto()
        {
            Jurisdictions = new List<JurisdictionsViewDto>();
        }
        public long OrganizationId { get; set; }
        public string OrganizationNm { get; set; } = null!;
        public string? LocalGovernmentType { get; set; }
        public string OrganizationCd { get; set; } = null!;
        public virtual ICollection<JurisdictionsViewDto> Jurisdictions { get; set; }
    }
}
