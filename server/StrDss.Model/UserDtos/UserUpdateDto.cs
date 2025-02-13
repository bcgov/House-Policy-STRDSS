﻿namespace StrDss.Model.UserDtos
{
    public class UserUpdateDto : IOrgRoles
    {
        public long UserIdentityId { get; set; }
        public long RepresentedByOrganizationId { get; set; }
        public List<string> RoleCds { get; set; } = new List<string>();
        public bool IsEnabled { get; set; }
        public DateTime? UpdDtm { get; set; }
    }
}
