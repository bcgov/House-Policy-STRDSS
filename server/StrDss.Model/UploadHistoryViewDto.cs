﻿namespace StrDss.Model
{
    public class UploadHistoryViewDto
    {
        public UploadHistoryViewDto()
        {
            ReportPeriodYM = "";
            OrganizationNm = "";
            GivenNm = "";
            FamilyNm = "";
            Status = "";
            UploadDeliveryType = "";
        }

        public long UploadDeliveryId { get; set; }
        public string UploadDeliveryType { get; set; }
        public string ReportPeriodYM { get; set; }
        public long ProvidingOrganizationId { get; set; }
        public string OrganizationNm { get; set; }
        public DateTime UpdDtm { get; set; }
        public string GivenNm { get; set; }
        public string FamilyNm { get; set; }
        public int Total { get; set; }
        public int Processed { get; set; }
        public int Errors { get; set; }
        public int Success { get; set; }
        public string Status { get; set; }
    }
}
