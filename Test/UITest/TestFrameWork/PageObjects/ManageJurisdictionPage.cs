using UITest.Models;
using UITest.SeleniumObjects;
using UITest.TestDriver;
using UITest.TestObjectFramework;

namespace UITest.PageObjects
{
    public class ManageJurisdictionPage
    {
        private IDriver _Driver;

        private string _URL = @"http://127.0.0.1:4200/manage-jurisdictions";

        private Button _EditPlatformButton;
        private Table _LGListingsTable;
        private Button _ExpandJurisdictionsButton;
        private Table _JurisdictionsListingsTable;
        private Button _EditJurisdictionButton;

        public string URL { get => _URL; set => _URL = value; }
        public Button EditPlatformButton { get => _EditPlatformButton; }
        public Table LGListingsTable { get => _LGListingsTable; }
        public IDriver Driver { get => _Driver; }
        public Button ExpandJurisdictionsButton { get => _ExpandJurisdictionsButton; }
        public Table JurisdictionsListingsTable { get => _JurisdictionsListingsTable; }
        public Button EditJurisdictionButton { get => _EditJurisdictionButton; }

        public ManageJurisdictionPage(IDriver Driver)
        {
            _Driver = Driver;

            _EditPlatformButton = new Button(Driver, Enums.FINDBY.CSSSELECTOR, ManageJurisdictionsPageModel.EditLocalGovernmentButton);
            _LGListingsTable = new Table(Driver, Enums.FINDBY.ID, ManageJurisdictionsPageModel.LGListingsTable);
            _ExpandJurisdictionsButton = new Button(Driver, Enums.FINDBY.ID, ManageJurisdictionsPageModel.ExpandJurisdictionsButton);
            _JurisdictionsListingsTable = new Table(Driver, Enums.FINDBY.ID, ManageJurisdictionsPageModel.JurisdictionsListingsTable);
            _EditJurisdictionButton = new Button(Driver, Enums.FINDBY.CSSSELECTOR, ManageJurisdictionsPageModel.EditJurisdictionButton);

        }
    }
}
