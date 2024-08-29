using UITest.Models;
using UITest.SeleniumObjects;
using UITest.TestDriver;
using UITest.TestObjectFramework;

namespace UITest.PageObjects
{
    public class ListingsPage
    {
        private IDriver _Driver;

        private string _URL = @"localhost:5002/istings";

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

        public string URL { get => _URL; set => _URL = value; }

        public DropDownList ListingsTypeDropDown { get => _ListingsTypeDropDown; set => _ListingsTypeDropDown = value; }

        public MessageBox ErrorMessageBox { get => _ErrorMessageBox; set => _ErrorMessageBox = value; }

        public  TextBox RowsPerPageTextBox { get => _RowsPerPageTextBox; set => _RowsPerPageTextBox = value; }

        public Button FirstPageButton { get => _FirstPageButton; set => _FirstPageButton = value; }

        public Button LastPageButton { get => _LastPageButton; set => _LastPageButton = value; }

        public Button PrevPageButton { get => _PrevPageButton; set => _PrevPageButton = value; }

        public Button NextPageButton { get => _NextPageButton; set => _NextPageButton = value; }

        public Button SendNoticeOfNonComplianceButton { get => _SendNoticeOfNonComplianceButton; set => _SendNoticeOfNonComplianceButton = value; }
        public Button SendTakedownRequestButton { get => _SendTakedownRequestButton; set => _SendTakedownRequestButton = value; }

        public CheckBox SelectAllCheckbox { get => _SelectAllCheckbox; set => _SelectAllCheckbox = value; }

        public WebElement ListingsTable { get => _ListingsTable; }
        public IDriver Driver { get => _Driver; }

        public ListingsPage(IDriver Driver)
        {
            _Driver = Driver;
            _LastPageButton = new Button(Driver, Enums.FINDBY.CSSSELECTOR, ListingsPageModel.LastPageButton);
            _FirstPageButton = new Button(Driver, Enums.FINDBY.CSSSELECTOR, ListingsPageModel.FirstPageButton);
            _NextPageButton = new Button(Driver, Enums.FINDBY.CSSSELECTOR, ListingsPageModel.NextPageButton);
            _PrevPageButton = new Button(Driver, Enums.FINDBY.CSSSELECTOR, ListingsPageModel.PrevPageButton);
            _SendNoticeOfNonComplianceButton = new Button(Driver, Enums.FINDBY.ID, ListingsPageModel.SendNoticeOfNonComplianceButton);
            _SendTakedownRequestButton = new Button(Driver, Enums.FINDBY.ID, ListingsPageModel.SendTakedownRequestButton);
            _RowsPerPageTextBox = new TextBox(Driver, Enums.FINDBY.CSSSELECTOR, ListingsPageModel.RowsPerPageTextBox);
            _ErrorMessageBox = new MessageBox(Driver, Enums.FINDBY.CSSSELECTOR, ListingsPageModel.ErrorMessageBox);
            _ListingsTypeDropDown = new DropDownList(Driver, Enums.FINDBY.CSSSELECTOR, ListingsPageModel.ListingsTypeDropDown);
            _SelectAllCheckbox = new CheckBox(Driver, Enums.FINDBY.CSSSELECTOR, ListingsPageModel.SelectAllCheckbox);
            _ListingsTable = new WebElement(Driver, Enums.FINDBY.ID, ListingsPageModel.ListingsTable);
        }
    }
}
