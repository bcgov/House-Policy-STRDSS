﻿namespace UITest.Models
{
    public class DetailedPlatformContactInformationPageModel
    {
        public static string AddSubsidiaryPlatformButton { get => "body > app-root > app-layout > div.content > app-view-platform > div.header > button"; }
        public static string UpdateParentPlatformInformationButton { get => "[id^='pn_id_'][id$='_content']:nth-child(2) > div.p-panel-footer[class*='ng-tns-c'][class*='ng-star-inserted'] > button"; }

        public static string UpdateSubsidiaryPlatformInformationButton { get => "document.querySelector(\"[id^='pn_id_'][id$='_content']:nth-child(2) > div.p-panel-footer[class*='ng-tns-c'][class*='ng-star-inserted'] > button\").click()"; }
   }
}
