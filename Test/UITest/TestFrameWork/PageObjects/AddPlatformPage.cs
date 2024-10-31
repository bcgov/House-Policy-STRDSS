using UITest.Models;
using UITest.SeleniumObjects;
using UITest.TestDriver;
using UITest.TestObjectFramework;

namespace UITest.PageObjects
{
    public class AddPlatformPage
    {
        private IDriver _Driver;

        //No Valid URL since the URL must contain a unique and new platform number
        //private string _URL = @"http://127.0.0.1:4200/add-sub-platform/590";

        private TextBox     _PlatformNameTextBox;

        private TextBox     _PlatformCodeTextBox;

        private TextBox     _EmailForNonComplianceNoticesTextBox;
        private TextBox     _EmailForTakedownRequestLettersTextBox;
        private TextBox     _SecondaryEmailForNonComplianceNoticesTextBox;
        private TextBox     _SecondaryEmailForTakedownRequest;
        private DropDownList _PlatformTypeDropdown;
        private Button      _CancelButton;
        private Button      _SaveButton;

        public TextBox PlatformNameTextBox { get => _PlatformNameTextBox; }
        public TextBox PlatformCodeTextBox { get => _PlatformCodeTextBox; }
        public TextBox EmailForNonComplianceNoticesTextBox { get => _EmailForNonComplianceNoticesTextBox; }
        public TextBox EmailForTakedownRequestLettersTextBox { get => _EmailForTakedownRequestLettersTextBox; }
        public TextBox SecondaryEmailForNonComplianceNoticesTextBox { get => _SecondaryEmailForNonComplianceNoticesTextBox; }
        public TextBox SecondaryEmailForTakedownRequest { get => _SecondaryEmailForTakedownRequest; }
        public DropDownList PlatformTypeDropdown { get => _PlatformTypeDropdown; }
        public Button SaveButton { get => _SaveButton; }
        public Button CancelButton { get => _CancelButton; }
        //public string URL { get => _URL; set => _URL = value; }
        public IDriver Driver { get => _Driver; }


        public AddPlatformPage(IDriver Driver)
        {
            _Driver = Driver;

            _PlatformNameTextBox = new TextBox(Driver, Enums.FINDBY.ID, AddPlatformPageModel.PlatformNameTextBox);
            _PlatformCodeTextBox = new TextBox(Driver, Enums.FINDBY.ID, AddPlatformPageModel.PlatformCodeTextBox);
            _EmailForNonComplianceNoticesTextBox = new TextBox(Driver, Enums.FINDBY.ID, AddPlatformPageModel.EmailForNonComplianceNoticesTextBox);
            _EmailForTakedownRequestLettersTextBox = new TextBox(Driver, Enums.FINDBY.ID, AddPlatformPageModel.EmailForTakedownRequestLettersTextBox);
            _SecondaryEmailForNonComplianceNoticesTextBox = new TextBox(Driver, Enums.FINDBY.ID, AddPlatformPageModel.SecondaryEmailForNonComplianceNoticesTextBox);
            _SecondaryEmailForTakedownRequest = new TextBox(Driver, Enums.FINDBY.ID, AddPlatformPageModel.SecondaryEmailForTakedownRequest);
            _PlatformTypeDropdown = new DropDownList(Driver, Enums.FINDBY.ID, AddPlatformPageModel.PlatformTypeDropDown);
            _SaveButton = new Button(Driver, Enums.FINDBY.CSSSELECTOR, AddPlatformPageModel.SaveButton);
            _CancelButton = new Button(Driver, Enums.FINDBY.CSSSELECTOR, AddPlatformPageModel.CancelButton);
        }
    }
}
