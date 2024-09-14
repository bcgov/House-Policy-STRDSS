namespace StrDss.Model
{
    public class StrRequirementsDto
    {
        public string OrganizationNm { get; set; }
        public bool? IsPrincipalResidenceRequired { get; set; }
        public bool? IsBusinessLicenceRequired { get; set; }
        public bool? IsStrProhibited { get; set; }
    }
}
