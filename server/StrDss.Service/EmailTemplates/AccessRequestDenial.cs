namespace StrDss.Service.EmailTemplates
{
    public class AccessRequestDenial
    {
        public string Subject { get;  } = "STR Data Portal - Access Denied";
        public string AdminEmail { get; set; } = "";

        public string Content
        {
            get
            {
                return
$@"Access to the STR Data Portal is restricted to authorized provincial and local government staff and short-term rental platforms. Please contact {AdminEmail} for more information.";
            }
        }
    }
}
