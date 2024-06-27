namespace StrDss.Model
{
    public class RentalListingContactDto
    {
        public long RentalListingContactId { get; set; }
        public bool IsPropertyOwner { get; set; }
        public short? ListingContactNbr { get; set; }
        public string? SupplierHostNo { get; set; }
        public string? FullNm { get; set; }
        public string? PhoneNo { get; set; }
        public string? FaxNo { get; set; }
        public string? FullAddressTxt { get; set; }
        public string? EmailAddressDsc { get; set; }
        public long ContactedThroughRentalListingId { get; set; }
        public bool HasValidEmail { get; set; }
        public DateTime UpdDtm { get; set; }
        public Guid? UpdUserGuid { get; set; }
    }
}
