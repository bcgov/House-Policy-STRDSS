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
        private Button _Upload_ListingsButton;
        private Button _ViewPolicyGuidelinesButton;

        private IDriver _Driver;

        public string URL { get => _URL; set => _URL = value; }
        public Button ViewPolicyGuidenceButton { get => _ViewPolicyGuidenceButton; }
        public Button SendNoticeButton { get => _SendNoticeButton;  }
        public Button SendTakedownLetterButton { get => _SendTakedownLetterButton; }
        public Button EscalateTakedownToCEUButto { get => _EscalateTakedownToCEUButton; }
        public Button ManageAccessRequestsButton { get => _ManageAccessRequestsButton; }
        public IDriver Driver { get => _Driver; }
        public Button Upload_ListingsButton { get => _Upload_ListingsButton; }
        public Button ViewPolicyGuidelinesButton { get => _ViewPolicyGuidelinesButton;}

        public LandingPage(IDriver Driver)
        {
            _Driver = Driver;
            _ViewPolicyGuidenceButton = new Button(Driver, Enums.FINDBY.ID, LandingPageModel.ViewPolicyGuidenceButton);
            _SendNoticeButton = new Button(Driver, Enums.FINDBY.ID, LandingPageModel.SendNoticeButton);
            _SendTakedownLetterButton = new Button(Driver, Enums.FINDBY.ID, LandingPageModel.SendTakeDownLetterButton);
            _ManageAccessRequestsButton = new Button(Driver, Enums.FINDBY.ID, LandingPageModel.ManageAccessRequestsButton);
            _EscalateTakedownToCEUButton = new Button(Driver, Enums.FINDBY.ID, LandingPageModel.EscalateTakedownToCEUButton);
            _Upload_ListingsButton = new Button(Driver, Enums.FINDBY.ID, LandingPageModel.UploadListingsButton);
            _ViewPolicyGuidelinesButton = new Button(Driver, Enums.FINDBY.ID, LandingPageModel.ViewPolicyGuidenceButton);

        }

        public bool Navigate()
        {
            _Driver.Navigate(URL);
            return (true);
        }
    }
}
