using UITest.Models;
using UITest.SeleniumObjects;
using UITest.TestDriver;
using UITest.TestObjectFramework;

namespace UITest.PageObjects
{
    public class BulkComplianceNoticePage
    {
        private IDriver _Driver;

        private string _URL = @"localhost:5002/listings";

        private DropDownList _ListingsTypeDropDown;
        private MessageBox _ErrorMessageBox;
        private TextBox _RowsPerPageTextBox;
        private Button _FirstPageButton;
        private Button _LastPageButton;
        private Button _PrevPageButton;
        private Button _NextPageButton;
        private Button _SendNoticeOfNonComplianceButton;
        private Button _SendTakedownRequestButton;
        private CheckBox _SelectAllCheckbox;
        private WebElement _ListingsTable;
        private TextBox _ContactEmailTextBox;
        private Button _SubmitButton;
        private Button _CancelButton;
        private TextBox _AddBCCsTextBox;
        private TextBox _OptionCommentsTextBox;
        private Button _SubmitPreviewButton;
        private Button _CancelPreviewButton;

        public string URL { get => _URL; set => _URL = value; }


        public IDriver Driver { get => _Driver; }
        public TextBox ContactEmailTextBox { get => _ContactEmailTextBox; }
        public Button SubmitButton { get => _SubmitButton;  }
        public Button CancelButton { get => _CancelButton;  }
        public TextBox AddBCCsTextBox { get => _AddBCCsTextBox; }
        public TextBox OptionCommentsTextBox { get => _OptionCommentsTextBox;  }
        public Button SubmitPreviewButton { get => _SubmitPreviewButton; }
        public Button CancelPreviewButton { get => _CancelPreviewButton; }

        public BulkComplianceNoticePage(IDriver Driver)
        {
            _Driver = Driver;
            _ContactEmailTextBox = new TextBox(Driver, Enums.FINDBY.ID, BulkComplianceNoticeModel.ContactEmailTextBox);
            _SubmitButton = new Button(Driver, Enums.FINDBY.ID, BulkComplianceNoticeModel.SubmitButton);
            _CancelButton = new Button(Driver, Enums.FINDBY.ID, BulkComplianceNoticeModel.CancelButton);
            _AddBCCsTextBox = new TextBox(Driver, Enums.FINDBY.CSSSELECTOR, BulkComplianceNoticeModel.AddBCCsTextBox);
            _OptionCommentsTextBox = new TextBox(Driver, Enums.FINDBY.CSSSELECTOR, BulkComplianceNoticeModel.OptionCommentsTextBox);
            _SubmitPreviewButton = new Button(Driver, Enums.FINDBY.ID, BulkComplianceNoticeModel.SubmitPreviewButton);
            _CancelPreviewButton = new Button(Driver, Enums.FINDBY.ID, BulkComplianceNoticeModel.CancelPreviewButton);
        }
    }
}
