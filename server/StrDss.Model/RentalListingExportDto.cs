namespace StrDss.Model
{
    public class RentalListingExportDto
    {
        public partial class DssRentalListingVw
        {
            public long? RentalListingId { get; set; }

            public string? ListingStatusType { get; set; }

            public int? ListingStatusSortNo { get; set; }

            public DateOnly? LatestReportPeriodYm { get; set; }

            public bool? IsActive { get; set; }

            public bool? IsNew { get; set; }

            public bool? IsTakenDown { get; set; }

            public long? OfferingOrganizationId { get; set; }

            public string? OfferingOrganizationCd { get; set; }

            public string? OfferingOrganizationNm { get; set; }

            public string? PlatformListingNo { get; set; }

            public string? PlatformListingUrl { get; set; }

            public string? OriginalAddressTxt { get; set; }

            public short? MatchScoreAmt { get; set; }

            public string? MatchAddressTxt { get; set; }

            public string? AddressSort1ProvinceCd { get; set; }

            public string? AddressSort2LocalityNm { get; set; }

            public string? AddressSort3LocalityTypeDsc { get; set; }

            public string? AddressSort4StreetNm { get; set; }

            public string? AddressSort5StreetTypeDsc { get; set; }

            public string? AddressSort6StreetDirectionDsc { get; set; }

            public string? AddressSort7CivicNo { get; set; }

            public string? AddressSort8UnitNo { get; set; }

            public string? ListingContactNamesTxt { get; set; }

            public long? ManagingOrganizationId { get; set; }

            public string? ManagingOrganizationNm { get; set; }

            public bool? IsPrincipalResidenceRequired { get; set; }

            public bool? IsBusinessLicenceRequired { get; set; }

            public bool? IsEntireUnit { get; set; }

            public short? AvailableBedroomsQty { get; set; }

            public long? NightsBookedYtdQty { get; set; }

            public long? SeparateReservationsYtdQty { get; set; }

            public string? BusinessLicenceNo { get; set; }

            public string? BcRegistryNo { get; set; }

            public string? LastActionNm { get; set; }

            public DateTime? LastActionDtm { get; set; }

            #region Extra Columns
            public long? NightsBookedQty00 { get; set; }
            public long? NightsBookedQty01 { get; set; }
            public long? NightsBookedQty02 { get; set; }
            public long? NightsBookedQty03 { get; set; }
            public long? NightsBookedQty04 { get; set; }
            public long? NightsBookedQty05 { get; set; }
            public long? NightsBookedQty06 { get; set; }
            public long? NightsBookedQty07 { get; set; }
            public long? NightsBookedQty08 { get; set; }
            public long? NightsBookedQty09 { get; set; }
            public long? NightsBookedQty10 { get; set; }
            public long? NightsBookedQty11 { get; set; }

            public long? SeparateReservationsQty00 { get; set; }
            public long? SeparateReservationsQty01 { get; set; }
            public long? SeparateReservationsQty02 { get; set; }
            public long? SeparateReservationsQty03 { get; set; }
            public long? SeparateReservationsQty04 { get; set; }
            public long? SeparateReservationsQty05 { get; set; }
            public long? SeparateReservationsQty06 { get; set; }
            public long? SeparateReservationsQty07 { get; set; }
            public long? SeparateReservationsQty08 { get; set; }
            public long? SeparateReservationsQty09 { get; set; }
            public long? SeparateReservationsQty10 { get; set; }
            public long? SeparateReservationsQty11 { get; set; }

            public string? PropertyHostName { get; set; }
            public string? PropertyHostEmail { get; set; }
            public string? PropertyHostPhoneNumber { get; set;}
            public string? PropertyHostFaxNumber { get;}
            public string? PropertyHostMailingAddress { get; set;}

            public string? SupplierHost1Name { get; set; }
            public string? SupplierHost1Email { get; set; }
            public string? SupplierHost1PhoneNumber { get; set; }
            public string? SupplierHost1FaxNumber { get; }
            public string? SupplierHost1MailingAddress { get; set; }
            public string? SupplierHost1Id { get; }

            public string? SupplierHost2Name { get; set; }
            public string? SupplierHost2Email { get; set; }
            public string? SupplierHost2PhoneNumber { get; set; }
            public string? SupplierHost2FaxNumber { get; }
            public string? SupplierHost2MailingAddress { get; set; }
            public string? SupplierHost2Id { get; }

            public string? SupplierHost3Name { get; set; }
            public string? SupplierHost3Email { get; set; }
            public string? SupplierHost3PhoneNumber { get; set; }
            public string? SupplierHost3FaxNumber { get; }
            public string? SupplierHost3MailingAddress { get; set; }
            public string? SupplierHost3Id { get; }

            public string? SupplierHost4Name { get; set; }
            public string? SupplierHost4Email { get; set; }
            public string? SupplierHost4PhoneNumber { get; set; }
            public string? SupplierHost4FaxNumber { get; }
            public string? SupplierHost4MailingAddress { get; set; }
            public string? SupplierHost4Id { get; }

            public string? SupplierHost5Name { get; set; }
            public string? SupplierHost5Email { get; set; }
            public string? SupplierHost5PhoneNumber { get; set; }
            public string? SupplierHost5FaxNumber { get; }
            public string? SupplierHost5MailingAddress { get; set; }
            public string? SupplierHost5Id { get; }

            public string? LastActionNm1 { get; set; }

            public DateTime? LastActionDtm1 { get; set; }

            public string? LastActionNm2 { get; set; }

            public DateTime? LastActionDtm2 { get; set; }

            #endregion
        }
    }
}
