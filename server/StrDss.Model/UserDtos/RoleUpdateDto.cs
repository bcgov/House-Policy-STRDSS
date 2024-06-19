namespace StrDss.Model.UserDtos
{
    public class RoleUpdateDto
    {
        public string UserRoleCd { get; set; } = null!;
        public string UserRoleNm { get; set; } = null!;
        public virtual ICollection<string> Permissions { get; set; } = new List<string>();
    }
}
