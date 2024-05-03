using Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using TestFrameWork.Models;
using UITest.PageObjects;
using UITest.TestDriver;
using static SpecFlowProjectBDD.SFEnums;

namespace SpecFlowProjectBDD.Helpers
{
    public class AuthHelper
    {
        private IDriver _Driver;
        private PathFinderPage _PathFinderPage;
        private IDirLoginPage _IDRLoginPage;
        private string _TestUserName;
        private string _TestPassword;
        private AppSettings _AppSettings;
        private UserTypeEnum _UserType;
        private BCIDPage _BCIDPage;

        public AuthHelper(IDriver Driver)
        {
            _Driver = Driver;
            _PathFinderPage = new PathFinderPage(_Driver);
            _IDRLoginPage = new IDirLoginPage(_Driver);
            _BCIDPage = new BCIDPage(_Driver);
            _AppSettings = new AppSettings();
        }

        public UserTypeEnum Authenticate(string UserName, string UserType)
        {
            _TestUserName = UserName;
            _TestPassword = _AppSettings.GetUser(_TestUserName) ?? string.Empty;
            UserHelper userHelper = new UserHelper();
            _UserType = userHelper.SetUserType(UserType);

            _Driver.Url = _AppSettings.GetServer("default");
            _Driver.Navigate();

            switch (_UserType)
            {
                case UserTypeEnum.BCGOVERNMENT:
                    {
                        _PathFinderPage.IDRButton.Click();
                        _IDRLoginPage.UserNameTextBox.WaitFor(5);
                        _IDRLoginPage.UserNameTextBox.EnterText(_TestUserName);
                        _IDRLoginPage.PasswordTextBox.EnterText(_TestPassword);
                        _IDRLoginPage.ContinueButton.Click();
                        break;
                    }
                case UserTypeEnum.PLATFORM:
                    {
                        _PathFinderPage.BCIDButton.Click();
                        _BCIDPage.UserNameTextBox.WaitFor(5);
                        _BCIDPage.UserNameTextBox.EnterText(_TestUserName);
                        _BCIDPage.PasswordTextBox.EnterText(_TestPassword);
                        _BCIDPage.ContinueButton.Click();
                        break;
                    }
            }

            return (_UserType);
        }
    }
}
