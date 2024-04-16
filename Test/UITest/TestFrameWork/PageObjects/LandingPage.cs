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

        private IDriver _Driver;

        public string URL { get => _URL; set => _URL = value; }
        public Button ViewPolicyGuidenceButton { get => _ViewPolicyGuidenceButton; }
        public Button SendNoticeButton { get => _SendNoticeButton;  }
        public Button SendTakedownLetterButton { get => _SendTakedownLetterButton; }
        public Button EscalateTakedownToCEUButto { get => _EscalateTakedownToCEUButton; }
        public Button ManageAccessRequestsButton { get => _ManageAccessRequestsButton; }

        public LandingPage(IDriver Driver)
        {
            _Driver = Driver;
            _ViewPolicyGuidenceButton = new Button(Driver, Enums.FINDBY.ID, LandingPageModel.ViewPolicyGuidenceButton);
            _SendNoticeButton = new Button(Driver, Enums.FINDBY.ID, LandingPageModel.SendNoticeButton);
            _SendTakedownLetterButton = new Button(Driver, Enums.FINDBY.ID, LandingPageModel.SendTakeDownLetterButton);
            _SendTakedownLetterButton = new Button(Driver, Enums.FINDBY.ID, LandingPageModel.SendTakeDownLetterButton);
            _ManageAccessRequestsButton = new Button(Driver, Enums.FINDBY.ID, LandingPageModel.ManageAccessRequestsButton);
            _EscalateTakedownToCEUButton = new Button(Driver, Enums.FINDBY.ID, LandingPageModel.EscalateTakedownToCEUButton);
        }

        public bool Navigate()
        {
            _Driver.Navigate(URL);
            return (true);
        }
    }
}
