namespace StrDss.Service.Bceid
{
    public class BceidAccount
    {
        public string Username { get; set; } = null!;
        public Guid UserGuid { get; set; }
        public Guid? BusinessGuid { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string BusinessLegalName { get; set; } = null!;
        public decimal BusinessNumber { get; set; } 
        public string DoingBusinessAs { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string UserType { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
    }
}
