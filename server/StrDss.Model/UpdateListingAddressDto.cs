namespace StrDss.Model
{
    public class UpdateListingAddressDto
    {
        public long RentalListingId { get; set; }
        public string AddressString { get; set; } = string.Empty;
    }
}
