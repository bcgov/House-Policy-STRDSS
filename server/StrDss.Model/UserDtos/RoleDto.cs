namespace StrDss.Model.UserDtos
{
    public class RoleDto
    {
        public string UserRoleCd { get; set; } = null!;
        public string UserRoleNm { get; set; } = null!;
        public virtual ICollection<PermissionDto> UserPrivilegeCds { get; set; } = new List<PermissionDto>();
    }
}
