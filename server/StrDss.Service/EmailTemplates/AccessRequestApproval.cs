namespace StrDss.Service.EmailTemplates
{
    public class AccessRequestApproval
    {
        public string Subject { get; } = "STR Data Portal - Access Granted";
        public string Link { get; set; } = "";
        public string AdminEmail { get; set; } = "";
        public string Content
        {
            get {
                return
$@"You have been granted access to the Short Term Rental Data Portal. Please access the portal here: {Link}. If you have any issues accessing this link, please contact {AdminEmail}.";
            }
        }

    }
}
