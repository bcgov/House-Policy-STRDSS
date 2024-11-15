using OpenQA.Selenium;

namespace UITest.Models
{
    public class UpdateJurisdictionInformationPageModel
    {
        public static string JurisdictionNameTextBox { get => "organizationNm"; }
        public static string ShapeFileIDTextBox { get => "shapeFileId"; }
        public static string LocalGovernmentNameDropDown { get => "community-filter-dropdown"; }
        public static string PrincipleResidenceRequirementTrueButton { get => "isPrincipalResidenceRequired-true"; }
        public static string PrincipleResidenceRequirementFalseButton { get => "isPrincipalResidenceRequired-false"; }
        public static string ShortTermRentalProhibitedTrueButton {get => "isStrProhibited-true"; }
        public static string ShortTermRentalProhibitedFalseButton { get => "isStrProhibited-false"; }
        public static string BusinessLiscenseRequirementTrueButton { get => "isBusinessLicenceRequired-true"; }
        public static string BusinessLiscenseRequirementFalseButton { get => "isBusinessLicenceRequired-false"; }
        public static string EconomicRegionDropDown { get => "economicRegionDsc"; }
        public static string SaveButton { get => "body > app-root > app-layout > div.content > app-update-jurisdiction-information > div.actions.ng-star-inserted > button:nth-child(1)"; }
        public static string CancelButton { get => "body > app-root > app-layout > div.content > app-update-jurisdiction-information > div.actions.ng-star-inserted > button.p-element.p-button-transparent.p-button.p-component"; }
    }
}
