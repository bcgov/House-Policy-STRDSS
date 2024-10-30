using UITest.Models;
using UITest.SeleniumObjects;
using UITest.TestDriver;
using UITest.TestObjectFramework;

namespace UITest.PageObjects
{
    public class DetailedPlatformContactInformationPage
    {
        private IDriver _Driver;

        private string _URL = @"http://127.0.0.1:4200/platform/590";

        private Button _AddSubsidiaryPlatformButton;


        public string URL { get => _URL; set => _URL = value; }

        public Button AddSubsidiaryPlatformButton { get => _AddSubsidiaryPlatformButton; set => _AddSubsidiaryPlatformButton = value; }


        public IDriver Driver { get => _Driver; }

        public DetailedPlatformContactInformationPage(IDriver Driver)
        {
            _Driver = Driver;

            _AddSubsidiaryPlatformButton = new Button(Driver, Enums.FINDBY.CSSSELECTOR, DetailedPlatformContactInformationPageModel.AddSubsidiaryPlatformButton);

        }
    }
}
