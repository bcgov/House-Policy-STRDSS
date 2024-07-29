namespace UITest.Models
{
    public class ManagingAccessModel
    {
        public static string UserTable
        {
            get
            {
                return "user-table";
            }
        }

        public static string ActionButton
        {
            get { return @"/html/body/app-root/app-layout/div[2]/app-user-management/div[2]/div[2]/p-table/div/div/table/tbody/tr[1]/td[8]/span"; }
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
