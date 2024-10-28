namespace StrDss.Model.OrganizationDtos
{
    public class LocalGovUpdateDto
    {
        public long OrganizationId { get; set; }
        public string OrganizationNm { get; set; } = "";
        public string LocalGovernmentType { get; set; } = "";
        public string BusinessLicenceFormatTxt { get; set; } = "";
        public DateTime UpdDtm { get; set; }
    }
}
