namespace UITest.Models
{
    public class DashboardPageModel
    {
        public static string AddApplicationButton { get => "#add_new_application_button > button"; }
        public static string LogoutButton { get => "body > app-root > app-layout > header > app-user-tools > app-user-info > p-button:nth-child(6) > button"; }
    }
}
