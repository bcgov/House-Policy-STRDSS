namespace StrDss.Model.UserDtos
{
    public class UpdateIsEnabledDto
    {
        public long UserIdentityId { get; set; }
        public bool IsEnabled { get; set; } = false;
        public DateTime UpdDtm { get; set; }
    }
}
