namespace StrDss.Model.RentalReportDtos
{
    public class RentalListingExtractDtoSetters
    {
        public static readonly Action<RentalListingExportDto, short?>[] NightsBookedSetters = new Action<RentalListingExportDto, short?>[]
        {
                (l, v) => l.NightsBookedQty00 = v,
                (l, v) => l.NightsBookedQty01 = v,
                (l, v) => l.NightsBookedQty02 = v,
                (l, v) => l.NightsBookedQty03 = v,
                (l, v) => l.NightsBookedQty04 = v,
                (l, v) => l.NightsBookedQty05 = v,
                (l, v) => l.NightsBookedQty06 = v,
                (l, v) => l.NightsBookedQty07 = v,
                (l, v) => l.NightsBookedQty08 = v,
                (l, v) => l.NightsBookedQty09 = v,
                (l, v) => l.NightsBookedQty10 = v,
                (l, v) => l.NightsBookedQty11 = v
        };

        public static readonly Action<RentalListingExportDto, short?>[] SeparateReservationsSetters = new Action<RentalListingExportDto, short?>[]
        {
                (l, v) => l.SeparateReservationsQty00 = v,
                (l, v) => l.SeparateReservationsQty01 = v,
                (l, v) => l.SeparateReservationsQty02 = v,
                (l, v) => l.SeparateReservationsQty03 = v,
                (l, v) => l.SeparateReservationsQty04 = v,
                (l, v) => l.SeparateReservationsQty05 = v,
                (l, v) => l.SeparateReservationsQty06 = v,
                (l, v) => l.SeparateReservationsQty07 = v,
                (l, v) => l.SeparateReservationsQty08 = v,
                (l, v) => l.SeparateReservationsQty09 = v,
                (l, v) => l.SeparateReservationsQty10 = v,
                (l, v) => l.SeparateReservationsQty11 = v
        };
    }
}
