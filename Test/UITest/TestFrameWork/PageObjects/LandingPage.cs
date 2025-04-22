using UITest.Models;
using UITest.SeleniumObjects;
using UITest.TestDriver;

namespace UITest.PageObjects
{
    public class LandingPage
    {
        private string _URL = @"localhost:4200";
        private Button _ViewPolicyGuidenceButton;
        private Button _SendNoticeButton;
        private Button _SendTakedownLetterButton;
        private Button _ManageAccessRequestsButton;
        private Button _EscalateTakedownToCEUButton;
        private Button _UploadPlatformDataButton;
        private Button _ViewPolicyGuidelinesButton;
        private Button _ViewListingsButton;
        private Button _AggregatedListingsButton;
        private Button _ManagePlatformsButton;
        private Button _ManageJurisdictionsButton;

        private IDriver _Driver;

        public IDriver Driver { get => _Driver; }
        public string URL { get => _URL; set => _URL = value; }
        public Button ViewPolicyGuidenceButton { get => _ViewPolicyGuidenceButton; }
        public Button SendNoticeButton { get => _SendNoticeButton;  }
        public Button SendTakedownLetterButton { get => _SendTakedownLetterButton; }
        public Button EscalateTakedownToCEUButto { get => _EscalateTakedownToCEUButton; }
        public Button ManageAccessRequestsButton { get => _ManageAccessRequestsButton; }
        public Button UploadPlatformDataButton { get => _UploadPlatformDataButton; }
        public Button ViewPolicyGuidelinesButton { get => _ViewPolicyGuidelinesButton;}
        public Button ViewListingsButton { get => _ViewListingsButton; }
        public Button AggregatedListingsButton { get => _AggregatedListingsButton; }
        public Button ManagePlatformsButton {get => _ManagePlatformsButton; }
        public Button ManageJurisdictionsButton { get => _ManageJurisdictionsButton;}

        public LandingPage(IDriver Driver)
        {
            _Driver = Driver;
            _ViewPolicyGuidenceButton = new Button(Driver, Enums.FINDBY.ID, LandingPageModel.ViewPolicyGuidenceButton);
            _SendNoticeButton = new Button(Driver, Enums.FINDBY.ID, LandingPageModel.SendNoticeButton);
            _SendTakedownLetterButton = new Button(Driver, Enums.FINDBY.ID, LandingPageModel.SendTakeDownLetterButton);
            _ManageAccessRequestsButton = new Button(Driver, Enums.FINDBY.ID, LandingPageModel.ManageAccessRequestsButton);
            _EscalateTakedownToCEUButton = new Button(Driver, Enums.FINDBY.ID, LandingPageModel.EscalateTakedownToCEUButton);
            _UploadPlatformDataButton = new Button(Driver, Enums.FINDBY.ID, LandingPageModel.UploadPlatformDataButton);
            _ViewPolicyGuidelinesButton = new Button(Driver, Enums.FINDBY.ID, LandingPageModel.ViewPolicyGuidenceButton);
            _ViewListingsButton = new Button(Driver, Enums.FINDBY.ID, LandingPageModel.ListingsButton);
            _AggregatedListingsButton = new Button(Driver, Enums.FINDBY.ID, LandingPageModel.AggregatedListingsButton);
            _ManagePlatformsButton = new Button(Driver, Enums.FINDBY.ID,LandingPageModel.ManagePlatformsButton);
            _ManageJurisdictionsButton = new Button(Driver, Enums.FINDBY.ID, LandingPageModel.ManageJurisdictionsButton);
        }

        public bool Navigate()
        {
            _Driver.Navigate(URL);
            return (true);
        }
    }
}
