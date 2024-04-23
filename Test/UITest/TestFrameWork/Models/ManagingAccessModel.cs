namespace UITest.Models
{
    public class ManagingAccessModel
    {
        public static string ManageAccessSection
        {
            get
            {
                return @"/html/body/app-root/app-layout/div[2]/app-user-management/div[2]";
            }
        }
        public static string StatusDropDown
        {
            get
            {
                return "statusSearchId";
            }
        }

        public static string OrganizationDropDown
        {
            get
            {
                return "organizationSearchId";
            }
        }

        public static string SearchTextBox
        {
            get
            {
                return "searchTerm";
            }
        }

        public static string RequestList
        {
            get
            {
                return "";
            }
        }

        public static string BackButton
        {
            get
            {
                return "body > app-root > app-layout > div.content > app-user-management > div.table-card-container > div.table-container > p-paginator > div > button.p-ripple.p-element.p-paginator-prev.p-paginator-element.p-link";
            }
        }

        public static string ForwardButton
        {
            get
            {
                return "body > app-root > app-layout > div.content > app-user-management > div.table-card-container > div.table-container > p-paginator > div > button.p-ripple.p-element.p-paginator-next.p-paginator-element.p-link";
            }
        }
    }
}
