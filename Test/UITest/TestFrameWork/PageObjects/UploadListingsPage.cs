using TestFrameWork.Models;
using UITest.Models;
using UITest.SeleniumObjects;
using UITest.TestDriver;
using UITest.TestObjectFramework;

namespace UITest.PageObjects
{
    public class UploadListingsPage
    {
        private DropDownList _ReportingMonthDropDown;
        private Button _SelectFileButton;
        private Button _UploadButton;
        private IDriver _Driver;

        public DropDownList ReportingMonthDropDown { get => _ReportingMonthDropDown; }
        public Button SelectFileButton { get => _SelectFileButton;  }
        public Button UploadButton { get => _UploadButton; }
        public IDriver Driver { get => _Driver; }

        public UploadListingsPage(IDriver Driver)
        {
            _Driver = Driver;
            _ReportingMonthDropDown = new DropDownList(Driver, Enums.FINDBY.CSSSELECTOR, UploadListingsModel.ReportingMonthDropDownList);
            _SelectFileButton = new Button(Driver, Enums.FINDBY.ID, UploadListingsModel.SelectFileButton);
            _UploadButton = new Button(Driver, Enums.FINDBY.ID, UploadListingsModel.UploadButton);
        }
    }
}
