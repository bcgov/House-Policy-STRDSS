namespace StrDss.Model
{
    public class BizLicenceSearchDto
    {
        public long BusinessLicenceId { get; set; }
        public string BusinessLicenceNo { get; set; } = null!;
        public string? PhysicalRentalAddressTxt { get; set; }
        public long ProvidingOrganizationId { get; set; }
    }
}
