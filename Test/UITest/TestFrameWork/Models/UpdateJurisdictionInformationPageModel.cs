using OpenQA.Selenium;

namespace UITest.Models
{
    public class UpdateJurisdictionInformationPageModel
    {
        public static string JurisdictionNameTextBox { get => "organizationNm"; }
        public static string ShapeFileIDTextBox { get => "shapeFileId"; }
        public static string LocalGovernmentNameDropDown { get => "community-filter-dropdown"; }
        public static string PrincipleResidenceRequirementTrueButton { get => "document.querySelector(\"#isPrincipalResidenceRequired-true > div > div.p-radiobutton-box\").click()"; }
        public static string PrincipleResidenceRequirementFalseButton { get => "document.querySelector(\"#isPrincipalResidenceRequired-false > div > div.p-radiobutton-box\").click()"; }
        public static string ShortTermRentalProhibitedTrueButton {get => "document.querySelector(\"#isStrProhibited-true > div > div.p-radiobutton-box\").click()"; }
        public static string ShortTermRentalProhibitedFalseButton { get => "document.querySelector(\"#isStrProhibited-false > div > div.p-radiobutton-box\").click()"; }
        public static string BusinessLiscenseRequirementTrueButton { get => "document.querySelector(\"#isBusinessLicenceRequired-true > div > div.p-radiobutton-box\").click()"; }
        public static string BusinessLiscenseRequirementFalseButton { get => "document.querySelector(\"#isBusinessLicenceRequired-false > div > div.p-radiobutton-box\").click()"; }
        public static string EconomicRegionDropDown { get => "economicRegionDsc"; }
        public static string SaveButton { get => "body > app-root > app-layout > div.content > app-update-jurisdiction-information > div.actions.ng-star-inserted > button:nth-child(1)"; }
        public static string CancelButton { get => "body > app-root > app-layout > div.content > app-update-jurisdiction-information > div.actions.ng-star-inserted > button.p-element.p-button-transparent.p-button.p-component"; }
    }
}
