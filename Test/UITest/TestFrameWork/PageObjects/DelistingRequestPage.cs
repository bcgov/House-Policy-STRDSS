using UITest.Models;
using UITest.SeleniumObjects;
using UITest.TestDriver;
using UITest.TestObjectFramework;

namespace UITest.PageObjects
{
    public class DelistingRequestPage
    {
        //private DropDownList _RequestInitiatedByDropDown;
        private DropDownList _PlatformReceipientDropdown;
        private DropDownList _RequestInitiatedByDropDown;
        private TextBox _ListingIDNumberTextBox;
        private CheckBox _SendCopyCheckbox;
        private TextBox _ListingUrlTextBox;
        private TextBox _AdditionalCCsTextBox;
        private Button _ReviewButton;
        private Button _ReturnHomeButton;

        private IDriver _Driver;

        public DropDownList PlatformReceipientDropdown { get => _PlatformReceipientDropdown; }
        public DropDownList RequestInitiatedByDropDown { get => _RequestInitiatedByDropDown; }
        public TextBox ListingIDNumberTextBox { get => _ListingIDNumberTextBox; }
        public TextBox ListingUrlTextBox { get => _ListingUrlTextBox; }
        public TextBox AdditionalCCsTextBox { get => _AdditionalCCsTextBox; }
        //public DropDownList RequestInitiatedByDropDown { get => _RequestInitiatedByDropDown; }
        public CheckBox SendCopyCheckbox { get => _SendCopyCheckbox; }
        public Button ReviewButton { get => _ReviewButton; }

        public Button ReturnHomeButton { get => _ReturnHomeButton; }

        public DelistingRequestPage(IDriver Driver)
        {
            _Driver = Driver;
            //_RequestInitiatedByDropDown = new DropDownList(Driver, Enums.FINDBY.ID, DelistingRequestModel.RequestInitiatedByDropDown);
            _PlatformReceipientDropdown = new DropDownList(Driver, Enums.FINDBY.ID, DelistingRequestModel.PlaformRecepientDropDown);
            _RequestInitiatedByDropDown = new DropDownList(Driver, Enums.FINDBY.ID, DelistingRequestModel.RequestInitiatedByDropDown);
            _ListingIDNumberTextBox = new TextBox(Driver, Enums.FINDBY.ID, DelistingRequestModel.ListingIDNumberTextBox);
            _SendCopyCheckbox = new CheckBox(Driver, Enums.FINDBY.ID, DelistingRequestModel.SendCopyCheckbox);
            _ListingUrlTextBox = new TextBox(Driver, Enums.FINDBY.ID, DelistingRequestModel.ListingUrlTextBox);
            _AdditionalCCsTextBox = new TextBox(Driver, Enums.FINDBY.ID, DelistingRequestModel.AdditionalCCsTextBox);
            _ReviewButton = new Button(Driver, Enums.FINDBY.ID, DelistingRequestModel.ReviewButton);
            _ReturnHomeButton = new Button(Driver, Enums.FINDBY.ID, DelistingRequestModel.ReturnHomeButton);
        }

    }
}
