using UITest.Models;
using UITest.SeleniumObjects;
using UITest.TestDriver;
using UITest.TestObjectFramework;

namespace UITest.PageObjects
{
    public class DelistingWarningPage
    {
        private DropDownList _PlatformReceipientDropdown;
        private TextBox _ListingIDNumberTextBox;
        private TextBox _ListingUrlTextBox;
        private TextBox _HostEmailAddressTextBox;
        private DropDownList _ReasonDropdown;
        private CheckBox _SendCopyCheckbox;   
        private TextBox _AdditionalCCsTextBox;
        private TextBox _LocalGovEmailTextBox; 
        private TextBox _LocalGovPhoneTextBox;
        private TextBox _LocalGovUrlTextBox;
        private Button _ReviewButton;

        private IDriver _Driver;

        public DropDownList PlatformReceipientDropdown { get => _PlatformReceipientDropdown;  }
        public TextBox ListingIDNumberTextBox { get => _ListingIDNumberTextBox;  }
        public TextBox ListingUrlTextBox { get => _ListingUrlTextBox;  }
        public TextBox HostEmailAddressTextBox { get => _HostEmailAddressTextBox;  }
        public DropDownList ReasonDropdown { get => _ReasonDropdown;  }

        public CheckBox SendCopyCheckbox { get => _SendCopyCheckbox;  }

        public TextBox AdditionalCCsTextBox { get => _AdditionalCCsTextBox; }
        public TextBox LocalGovEmailTextBox { get => _LocalGovEmailTextBox;  }
        public TextBox LocalGovPhoneTextBox { get => _LocalGovPhoneTextBox; }
        public TextBox LocalGovUrlTextBox { get => _LocalGovUrlTextBox; }
        public Button ReviewButton { get => _ReviewButton; }


        public DelistingWarningPage(IDriver Driver)
        {
            _Driver = Driver;

            _PlatformReceipientDropdown = new DropDownList(Driver, Enums.FINDBY.ID, DelistingWarningModel.PlatformReceipientDropDown);
            _ListingIDNumberTextBox = new TextBox(Driver, Enums.FINDBY.ID, DelistingWarningModel.ListingIDNumberTextBox);
            _ListingUrlTextBox = new TextBox(Driver, Enums.FINDBY.ID, DelistingWarningModel.ListingIDNumberTextBox);
            _HostEmailAddressTextBox = new TextBox(Driver, Enums.FINDBY.ID, DelistingWarningModel.HostEmailAddressTextBox);
            _ReasonDropdown = new DropDownList(Driver, Enums.FINDBY.ID, DelistingWarningModel.ReasonDropdown);
            _SendCopyCheckbox = new CheckBox(Driver, Enums.FINDBY.ID, DelistingWarningModel.SendCopyCheckbox);
            _AdditionalCCsTextBox = new TextBox(Driver, Enums.FINDBY.ID, DelistingWarningModel.AdditionalCCsTextBox);
            _LocalGovEmailTextBox = new TextBox(Driver, Enums.FINDBY.ID, DelistingWarningModel.LocalGovEmailTextBox);
            _LocalGovPhoneTextBox = new TextBox(Driver, Enums.FINDBY.ID, DelistingWarningModel.LocalGovPhoneTextBox);
            _LocalGovUrlTextBox = new TextBox(Driver, Enums.FINDBY.ID, DelistingWarningModel.LocalGovUrlTextBox);
            _ReviewButton = new Button(Driver, Enums.FINDBY.ID, DelistingWarningModel.ReviewButton);
        }

    }
}
