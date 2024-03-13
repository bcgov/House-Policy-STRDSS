namespace StrDss.Model.LocalGovernmentDtos
{
    public class LocalGovernmentDto
    {
        public long LocalGovernmentId { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public static readonly LocalGovernmentDto[] localGovernments = new LocalGovernmentDto[]
        {
            new LocalGovernmentDto {LocalGovernmentId = 1, Name = "Victoria", Email = "young-jin.chung@gov.bc.ca"},
            new LocalGovernmentDto {LocalGovernmentId = 2, Name = "Vancouver", Email = "young-jin.chung@gov.bc.ca" }
        };
    }
}
