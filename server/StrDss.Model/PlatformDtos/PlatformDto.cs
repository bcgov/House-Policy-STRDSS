namespace StrDss.Model.PlatformDtos
{
    public class PlatformDto
    {
        public long PlatformId { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public static readonly PlatformDto[] Platforms = new PlatformDto[]
        {
            new PlatformDto {PlatformId = 1, Name = "Airbnb", Email = "young-jin.chung@gov.bc.ca"},
            new PlatformDto {PlatformId = 2, Name = "Vrbo", Email = "young-jin.chung@gov.bc.ca" }
        };
    }
}
