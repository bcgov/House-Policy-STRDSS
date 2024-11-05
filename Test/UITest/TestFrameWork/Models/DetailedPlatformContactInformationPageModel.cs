namespace UITest.Models
{
    public class DetailedPlatformContactInformationPageModel
    {
        public static string AddSubsidiaryPlatformButton { get => "body > app-root > app-layout > div.content > app-view-platform > div.header > button"; }

        public static string UpdateParentPlatformInformationButton { get => "document.querySelector(\"#pn_id_47_content > div.p-panel-footer.ng-tns-c3953704192-20.ng-star-inserted > button\")"; }

        //document.querySelector("#pn_id_31_content > div.p-panel-footer.ng-tns-c3953704192-4.ng-star-inserted > button")
        public static string UpdateSubsidiaryPlatformInformationButton { get => "document.querySelector(\"#pn_id_31_content > div.p-panel-footer.ng-tns-c3953704192-4.ng-star-inserted > button\").click()"; }
        //public static string UpdateSubsidiaryPlatformInformationButton { get => "[id^='pn_id_'][id$='_content']:nth-child(2) > div.p-panel-footer[class*='ng-tns-c'][class*='ng-star-inserted'] > button"; }
        //[id^="pn_id_"][id$="_content"] > div.p-panel-footer[class*="ng-tns-c"][class*="ng-star-inserted"] > button

        //"[id^='pn_id_'][id$='_content'] > thead > tr > th:nth-child(1) > p-tableheadercheckbox > div > div.p-checkbox-box"
        //"[id^='pn_id_'][id$='-table']"
    }
}
