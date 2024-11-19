using OpenQA.Selenium;

namespace UITest.Models
{
    public class ManageJurisdictionsPageModel
    {
        public static string AddNewParentPlatformButton { get => "add-new-platform-btn"; }
        public static string EditLocalGovernmentButton { get => "#lg-edit-0-icon"; }
        public static string LGListingsTable { get => "pn_id_18-table"; }
        public static string JurisdictionsListingsTable { get => "pn_id_31-table"; }
        public static string ExpandJurisdictionsButton { get => "expand-jurisdiction-row-0"; }
        public static string EditJurisdictionButton { get => "[id^=\"jurisdiction-edit-\"][id$=\"-icon\"] > span"; }

    }
}
