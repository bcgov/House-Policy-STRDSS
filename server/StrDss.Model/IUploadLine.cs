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
        [Name("reason")]
        public string TakeDownReason { get; set; } = "";
        [Name("reg_no")]
        public string RegNo { get; set; } = "";
        [Name("bc_reg_no")]
        public string BcRegNo { get; set; } = "";
        [Name("rental_street")]
        public string RentalStreet { get; set; } = "";
        [Name("rental_postal")]
        public string RentalPostal { get; set; } = "";
        [Name("rental_address")]
        public string RentalAddress { get; set; } = "";
    }
}
