namespace StrDss.Model.UserDtos
{
    public class UserUpdateDto
    {
        public long UserIdentityId { get; set; }
        public long RepresentedByOrganizationId { get; set; }
        public List<string> RoleCds { get; set; } = new List<string>();

    }
}
