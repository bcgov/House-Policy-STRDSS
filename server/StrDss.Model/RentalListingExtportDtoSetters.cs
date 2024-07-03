namespace StrDss.Model
{
    public class RentalListingExtportDtoSetters
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

        public static readonly Action<RentalListingExportDto, string?>[] PropertyHostNameSetters = new Action<RentalListingExportDto, string?>[]
        {
            (l, v) => l.PropertyHostName = v,
            (l, v) => l.SupplierHost1Name = v,
            (l, v) => l.SupplierHost2Name = v,
            (l, v) => l.SupplierHost3Name = v,
            (l, v) => l.SupplierHost4Name = v,
            (l, v) => l.SupplierHost5Name = v
        };

        public static readonly Action<RentalListingExportDto, string?>[] PropertyHostEmailSetters = new Action<RentalListingExportDto, string?>[]
        {
            (l, v) => l.PropertyHostEmail = v,
            (l, v) => l.SupplierHost1Email = v,
            (l, v) => l.SupplierHost2Email = v,
            (l, v) => l.SupplierHost3Email = v,
            (l, v) => l.SupplierHost4Email = v,
            (l, v) => l.SupplierHost5Email = v
        };

        public static readonly Action<RentalListingExportDto, string?>[] PropertyHostPhoneNumberSetters = new Action<RentalListingExportDto, string?>[]
        {
            (l, v) => l.PropertyHostPhoneNumber = v,
            (l, v) => l.SupplierHost1PhoneNumber = v,
            (l, v) => l.SupplierHost2PhoneNumber = v,
            (l, v) => l.SupplierHost3PhoneNumber = v,
            (l, v) => l.SupplierHost4PhoneNumber = v,
            (l, v) => l.SupplierHost5PhoneNumber = v
        };

        public static readonly Action<RentalListingExportDto, string?>[] PropertyHostMailingAddressSetters = new Action<RentalListingExportDto, string?>[]
        {
            (l, v) => l.PropertyHostMailingAddress = v,
            (l, v) => l.SupplierHost1MailingAddress = v,
            (l, v) => l.SupplierHost2MailingAddress = v,
            (l, v) => l.SupplierHost3MailingAddress = v,
            (l, v) => l.SupplierHost4MailingAddress = v,
            (l, v) => l.SupplierHost5MailingAddress = v
        };
    }
}
