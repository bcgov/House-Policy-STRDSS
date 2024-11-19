using UITest.Models;
using UITest.SeleniumObjects;
using UITest.TestDriver;
using UITest.TestObjectFramework;

namespace UITest.PageObjects
{
    public class EditSubPlatformPage
    {
        private IDriver _Driver;

        //No Valid URL since the URL must contain a unique and new platform number
        //private string _URL = @"http://127.0.0.1:4200/add-sub-platform";

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

        public IDriver Driver { get => _Driver; }


        public EditSubPlatformPage(IDriver Driver)
        {
            _Driver = Driver;

            _PlatformNameTextBox = new TextBox(Driver, Enums.FINDBY.ID, EditSubPlatformPageModel.PlatformNameTextBox);
            _PlatformCodeTextBox = new TextBox(Driver, Enums.FINDBY.ID, EditSubPlatformPageModel.PlatformCodeTextBox);
            _EmailForNonComplianceNoticesTextBox = new TextBox(Driver, Enums.FINDBY.ID, EditSubPlatformPageModel.EmailForNonComplianceNoticesTextBox);
            _EmailForTakedownRequestLettersTextBox = new TextBox(Driver, Enums.FINDBY.ID, EditSubPlatformPageModel.EmailForTakedownRequestLettersTextBox);
            _SecondaryEmailForNonComplianceNoticesTextBox = new TextBox(Driver, Enums.FINDBY.ID, EditSubPlatformPageModel.SecondaryEmailForNonComplianceNoticesTextBox);
            _SecondaryEmailForTakedownRequest = new TextBox(Driver, Enums.FINDBY.ID, EditSubPlatformPageModel.SecondaryEmailForTakedownRequest);
            _SaveButton = new Button(Driver, Enums.FINDBY.CSSSELECTOR, EditSubPlatformPageModel.SaveButton);
            _CancelButton = new Button(Driver, Enums.FINDBY.CSSSELECTOR, EditSubPlatformPageModel.CancelButton);
            _DisablePlatformButton = new Button(Driver, Enums.FINDBY.CSSSELECTOR, EditSubPlatformPageModel.DisablePlatformButton);
            _EnablePlatformButton = new Button(Driver, Enums.FINDBY.CSSSELECTOR, EditSubPlatformPageModel.EnablePlatformButton);
        }
    }
}
