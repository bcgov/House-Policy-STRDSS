namespace UITest.Models
{
    public class DelistingWarningModel
    {
        public static string PlatformReceipientDropDown { get => "platformId"; }

        public static string ListingIDNumberTextBox { get => "listingId"; }

        public static string ListingUrlTextBox { get => "listingUrl"; }

        public static string HostEmailAddressTextBox { get => "hostEmail"; }

        public static string AlternativeNoticeSentCheckbox { get => "#hostEmailSent > div > div.p-checkbox-box"; }

        public static string ReasonDropdown { get => "reasonId"; }

        public static string SendCopyCheckbox { get => "sendCopy"; }

        public static string AdditionalCCsTextBox { get => "ccList"; }

        public static string LocalGovEmailTextBox { get => "LgContactEmail"; }

        public static string LocalGovPhoneTextBox { get => "LgContactPhone"; }

        public static string LocalGovUrlTextBox { get => "StrBylawUrl"; }

        public static string ReviewButton { get => "form-preview-btn"; }

        public static string ReturnHomeButton { get => "return-home-btn"; }

    }
}
