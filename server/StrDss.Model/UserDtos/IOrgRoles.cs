namespace StrDss.Model.UserDtos
{
    public interface IOrgRoles
    {
        public long RepresentedByOrganizationId { get; set; }
        public List<string> RoleCds { get; set; }
    }
}
