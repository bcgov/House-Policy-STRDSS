using UITest.Models;
using UITest.SeleniumObjects;
using UITest.TestDriver;
using UITest.TestObjectFramework;

namespace UITest.PageObjects
{
    public class EditPlatformPage
    {
        private IDriver _Driver;

        //No Valid URL since the URL must contain a unique and new platform number
        //private string _URL = @"http://127.0.0.1:4200/add-sub-platform/590";

        private TextBox _PlatformNameTextBox;

        private TextBox _PlatformCodeTextBox;

        private TextBox _EmailForNonComplianceNoticesTextBox;
        private TextBox _EmailForTakedownRequestLettersTextBox;
        private TextBox _SecondaryEmailForNonComplianceNoticesTextBox;
        private TextBox _SecondaryEmailForTakedownRequest;
        private Button _CancelButton;
        private Button _SaveButton;
        private Button _DisablePlatformButton;
        private Button _EnablePlatformButton;
        private DropDownList _PlatformTypeDropDown;

        public TextBox PlatformNameTextBox { get => _PlatformNameTextBox; }
        public TextBox PlatformCodeTextBox { get => _PlatformCodeTextBox; }
        public TextBox EmailForNonComplianceNoticesTextBox { get => _EmailForNonComplianceNoticesTextBox; }
        public TextBox EmailForTakedownRequestLettersTextBox { get => _EmailForTakedownRequestLettersTextBox; }
        public TextBox SecondaryEmailForNonComplianceNoticesTextBox { get => _SecondaryEmailForNonComplianceNoticesTextBox; }
        public TextBox SecondaryEmailForTakedownRequest { get => _SecondaryEmailForTakedownRequest; }
        public Button SaveButton { get => _SaveButton; }
        public Button CancelButton { get => _CancelButton; }
        public Button DisablePlatformButton { get => _DisablePlatformButton; }
        public Button EnablePlatformButton { get => _EnablePlatformButton; }
        public DropDownList PlatformTypeDropDown { get => _PlatformTypeDropDown; set => _PlatformTypeDropDown = value; }

        public IDriver Driver { get => _Driver; }


        public EditPlatformPage(IDriver Driver)
        {
            _Driver = Driver;

            _PlatformNameTextBox = new TextBox(Driver, Enums.FINDBY.ID, EditPlatformPageModel.PlatformNameTextBox);
            _PlatformCodeTextBox = new TextBox(Driver, Enums.FINDBY.ID, EditPlatformPageModel.PlatformCodeTextBox);
            _EmailForNonComplianceNoticesTextBox = new TextBox(Driver, Enums.FINDBY.ID, EditPlatformPageModel.EmailForNonComplianceNoticesTextBox);
            _EmailForTakedownRequestLettersTextBox = new TextBox(Driver, Enums.FINDBY.ID, EditPlatformPageModel.EmailForTakedownRequestLettersTextBox);
            _SecondaryEmailForNonComplianceNoticesTextBox = new TextBox(Driver, Enums.FINDBY.ID, EditPlatformPageModel.SecondaryEmailForNonComplianceNoticesTextBox);
            _SecondaryEmailForTakedownRequest = new TextBox(Driver, Enums.FINDBY.ID, EditPlatformPageModel.SecondaryEmailForTakedownRequest);
            _SaveButton = new Button(Driver, Enums.FINDBY.CSSSELECTOR, EditPlatformPageModel.SaveButton);
            _CancelButton = new Button(Driver, Enums.FINDBY.CSSSELECTOR, EditPlatformPageModel.CancelButton);
            _DisablePlatformButton = new Button(Driver, Enums.FINDBY.CSSSELECTOR, EditPlatformPageModel.DisablePlatformButton);
            _EnablePlatformButton = new Button(Driver, Enums.FINDBY.CSSSELECTOR, EditPlatformPageModel.EnablePlatformButton);
            _PlatformTypeDropDown = new DropDownList(Driver, Enums.FINDBY.CSSSELECTOR, AddPlatformPageModel.PlatformNameTextBox);
        }
    }
}
