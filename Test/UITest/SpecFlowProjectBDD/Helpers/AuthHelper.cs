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
        private LogonTypeEnum? _LogonType;
        private BCIDPage _BCIDPage;

        public AuthHelper(IDriver Driver)
        {
            _Driver = Driver;
            _PathFinderPage = new PathFinderPage(_Driver);
            _IDRLoginPage = new IDirLoginPage(_Driver);
            _BCIDPage = new BCIDPage(_Driver);
            _AppSettings = new AppSettings();
        }
        public LogonTypeEnum SetLogonType(UserTypeEnum UserType)
        {
            LogonTypeEnum _LogonType;
            switch (UserType)
            {
                case UserTypeEnum.BCGOVERNMENTSTAFF:
                case UserTypeEnum.CEUSTAFF:
                case UserTypeEnum.CEUADMIN:
                    {
                        _LogonType = SFEnums.LogonTypeEnum.IDIR;
                        break;
                    }
                case UserTypeEnum.LOCALGOVERNMENT:
                case UserTypeEnum.SHORTTERMRENTALPLATFORM:
                    {
                        _LogonType = SFEnums.LogonTypeEnum.BCID;
                        break;
                    }
                default:
                    throw new ArgumentException("Unknown User Type (" + UserType + ")");
            }
            return (_LogonType);
        }

        public LogonTypeEnum? Authenticate(string UserName, string Password, UserTypeEnum UserType)
        {
            _TestUserName = UserName;
            _TestPassword = Password;
            _LogonType = SetLogonType(UserType);

            _Driver.Url = _AppSettings.GetServer("default");
            _Driver.Navigate();

            switch (_LogonType)
            {
                case LogonTypeEnum.IDIR:
                    {
                        _PathFinderPage.IDRButton.Click();
                        _IDRLoginPage.UserNameTextBox.WaitFor(5);
                        _IDRLoginPage.UserNameTextBox.EnterText(_TestUserName);
                        _IDRLoginPage.PasswordTextBox.EnterText(_TestPassword);
                        _IDRLoginPage.ContinueButton.Click();
                        break;
                    }
                case LogonTypeEnum.BCID:
                    {
                        _PathFinderPage.BCIDButton.Click();
                        _BCIDPage.UserNameTextBox.WaitFor(5);
                        _BCIDPage.UserNameTextBox.EnterText(_TestUserName);
                        _BCIDPage.PasswordTextBox.EnterText(_TestPassword);
                        _BCIDPage.ContinueButton.Click();
                        break;
                    }
            }

            if (_Driver.Url.ToLower().Contains("logon.cgi"))
            {
                _LogonType = null;
            }

            return (_LogonType);
        }
    }
}
