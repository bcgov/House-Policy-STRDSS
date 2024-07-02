namespace StrDss.Model.RentalReportDtos
{
    public class RentalListingExtractWithFileDto
    {
        public long RentalListingExtractId { get; set; }
        public string RentalListingExtractNm { get; set; } = null!;
        public bool IsPrRequirementFiltered { get; set; }
        public long? FilteringOrganizationId { get; set; }
        public byte[]? SourceBin { get; set; }
        public DateTime UpdDtm { get; set; }
    }
}
