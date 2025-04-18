﻿using OpenQA.Selenium;

namespace UITest.Models
{
    public class EditSubPlatformPageModel
    {
        public static string PlatformNameTextBox { get => "organizationNm"; }
        public static string PlatformCodeTextBox { get => "organizationCd"; }
        public static string EmailForNonComplianceNoticesTextBox { get => "primaryNoticeOfTakedownContactEmail"; }
        public static string EmailForTakedownRequestLettersTextBox { get => "primaryTakedownRequestContactEmail"; }
        public static string SecondaryEmailForNonComplianceNoticesTextBox {get => "secondaryNoticeOfTakedownContactEmail"; }
        public static string SecondaryEmailForTakedownRequest { get => "secondaryTakedownRequestContactEmail"; }
        public static string SaveButton { get => "body > app-root > app-layout > div.content > app-edit-platform > div.actions.ng-star-inserted > button:nth-child(1)"; }
        public static string CancelButton { get => "body > app-root > app-layout > div.content > app-edit-platform > div.actions.ng-star-inserted > button.p-element.p-button-transparent.p-button.p-component"; }
        public static string DisablePlatformButton {get => "isActive-false"; }
        public static string EnablePlatformButton { get => "isActive-true"; }
    }
}
