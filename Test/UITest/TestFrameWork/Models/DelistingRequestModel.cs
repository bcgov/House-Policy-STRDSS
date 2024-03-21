namespace UITest.Models
{
    public class DelistingRequestModel
    {
        public static string PlaformRecepientDropDown { get => "platformId"; }

        public static string RequestInitiatedByDropDown { get => "lgId"; }
        public static string ListingIDNumberTextBox { get => "listingId"; }
        public static string ListingUrlTextBox { get => "listingUrl"; }
        public static string SendCopyCheckbox { get => "sendCopy"; }
        public static string AdditionalCCsTextBox { get => "ccList"; }
        public static string ReviewButton { get => "form-preview-btn"; }

        public static string ReturnHomeButton { get => "return-home-btn"; }
    }
}
