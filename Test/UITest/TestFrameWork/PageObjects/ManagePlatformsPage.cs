using UITest.Models;
using UITest.SeleniumObjects;
using UITest.TestDriver;
using UITest.TestObjectFramework;

namespace UITest.PageObjects
{
    public class ManagePlatformsPage
    {
        private IDriver _Driver;

        private string _URL = @"http://127.0.0.1:4200/platform-management";

        private Button _EditPlatformButton;

        private Table _ListingsTable;

        public string URL { get => _URL; set => _URL = value; }


        public Button EditPlatformButton { get => _EditPlatformButton; set => _EditPlatformButton = value; }

        public Table ListingsTable { get => _ListingsTable; }
        public IDriver Driver { get => _Driver; }

        public ManagePlatformsPage(IDriver Driver)
        {
            _Driver = Driver;

            _EditPlatformButton = new Button(Driver, Enums.FINDBY.CSSSELECTOR, ManagePlatformsPageModel.EditPlatformButton);

            _ListingsTable = new Table(Driver, Enums.FINDBY.ID, ManagePlatformsPageModel.ListingsTable);
        }
    }
}
