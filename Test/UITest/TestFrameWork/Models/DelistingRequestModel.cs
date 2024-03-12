using OpenQA.Selenium.DevTools.V118.DOM;

namespace UITest.Models
{
    public class DelistingRequestModel
    {
        public static string RequestInitiatedByDropDown { get => ""; }
        public static string PlaformRecepientDropDown { get => "platformId"; }
        public static string ListingIDNumberTextBox { get => ""; }
        public static string ListingUrlTextBox { get => "listingUrl"; }
        public static string SendCopyCheckbox { get => "sendCopy"; }
        public static string AdditionalCCsTextBox { get => "ccList"; }
        public static string ReviewButton { get => "form-preview-btn"; }
    }
}
