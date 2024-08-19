using CsvHelper.Configuration.Attributes;

namespace StrDss.Model
{
    public class UploadLine
    {
        [Name("rpt_period")]
        public string RptPeriod { get; set; } = "";
        [Name("rpt_type")]
        public string RptType { get; set; } = "";
        [Name("org_cd")]
        public string OrgCd { get; set; } = "";
        [Name("listing_id")]
        public string ListingId { get; set; } = "";
        [Name("bus_lic_no")]
        public string BusinessLicenceNo { get; set; } = "";
    }
}
