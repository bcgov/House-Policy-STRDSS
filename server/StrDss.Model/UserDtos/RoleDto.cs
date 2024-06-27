namespace StrDss.Model.UserDtos
{
    public class RoleDto
    {
        public string UserRoleCd { get; set; } = null!;
        public string UserRoleNm { get; set; } = null!;
        public string? UserRoleDsc { get; set; } = null!;
        public virtual ICollection<PermissionDto> Permissions { get; set; } = new List<PermissionDto>();
        public bool IsReferenced { get; set; }
        public DateTime? UpdDtm { get; set; }
    }
}
