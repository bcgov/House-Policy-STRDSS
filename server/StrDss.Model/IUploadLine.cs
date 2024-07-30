using CsvHelper.Configuration.Attributes;

namespace StrDss.Model
{
    public class UploadLine
    {
        [Name("rpt_period")]
        public string RptPeriod { get; set; } = "";

        [Name("org_cd")]
        public string OrgCd { get; set; } = "";

        [Name("listing_id")]
        public string ListingId { get; set; } = "";
    }
}
