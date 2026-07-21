namespace StrDss.Model
{
    public class RecordListingActionDto
    {
        public long RentalListingId { get; set; }
        public string ListingActionType { get; set; } = "";
        public DateTime? ActionDtm { get; set; }
        public string? TakedownReason { get; set; }
        public long? InitiatingUserIdentityId { get; set; }
        public long? SourceEmailMessageId { get; set; }
    }
}
