namespace StrDss.Model.RentalReportDtos
{
    public class ListingHistoryDto
    {
        public ListingHistoryDto()
        {
            ReportPeriodYM = "";
        }

        public string ReportPeriodYM { get; set; }
        public long? NightsBookedQty { get; set; }
        public long? SeparateReservationsQty { get; set; }
    }
}
