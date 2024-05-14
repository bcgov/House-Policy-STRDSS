namespace StrDss.Model.RentalReportDtos
{
    public class RentalUploadHistoryViewDto
    {
        public RentalUploadHistoryViewDto()
        {
            ReportPeriodYM = "";
            OrganizationNm = "";
            GivenNm = "";
            FamilyNm = "";
        }

        public long UploadDeliveryId { get; set; }
        public string ReportPeriodYM { get; set; }
        public long ProvidingOrganizationId { get; set; }
        public string OrganizationNm { get; set; }
        public DateTime UpdDtm { get; set; }
        public string GivenNm { get; set; }
        public string FamilyNm { get; set; }
        public int Total { get; set; }
        public int Processed { get; set; }
        public int Errors { get; set; }
        public int Success => Processed - Errors;
    }
}
