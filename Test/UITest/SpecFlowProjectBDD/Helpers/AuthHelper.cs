using Configuration;
using OpenQA.Selenium;
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
        private BCIDPage _BCIDPage;

        public AuthHelper(IDriver Driver)
        {
            _Driver = Driver;
            _PathFinderPage = new PathFinderPage(_Driver);
            _IDRLoginPage = new IDirLoginPage(_Driver);
            _BCIDPage = new BCIDPage(_Driver);
            _AppSettings = new AppSettings();
        }
        private LogonTypeEnum? SetLogonType(UserTypeEnum UserType)
        {
            LogonTypeEnum? logonType;
            switch (UserType)
            {
                case UserTypeEnum.BCGOVERNMENTSTAFF:
                case UserTypeEnum.CEUSTAFF:
                case UserTypeEnum.CEUADMIN:
                    {
                        logonType = SFEnums.LogonTypeEnum.IDIR;
                        break;
                    }
                case UserTypeEnum.LOCALGOVERNMENT:
                case UserTypeEnum.SHORTTERMRENTALPLATFORM:
                    {
                        logonType = SFEnums.LogonTypeEnum.BCID;
                        break;
                    }
                default:
                    throw new ArgumentException("Unknown User Type (" + UserType + ")");
            }
            if (null == logonType)
            {
            }
            return (logonType);
        }

        public LogonTypeEnum? Authenticate(string UserName, string Password, UserTypeEnum UserType)
        {
            LogonTypeEnum? logonType;
            _TestUserName = UserName;
            _TestPassword = Password;
            logonType = SetLogonType(UserType);

            _Driver.Url = _AppSettings.GetServer("default");
            _Driver.Navigate();

            bool result = false;
            int i = 0;

            //Sleep for 5 seconds and try twice in case text boxes are rendered, but not yet ready. Selenium WaitFor would be a better option than 
            // Sleep, but it is not reliable for authentication
            while ((result == false) && (i++ < 2))
            {
                switch (logonType)
                {
                    case LogonTypeEnum.IDIR:
                        {
                            try
                            {
                                _PathFinderPage.IDRButton.Click();
                                Thread.Sleep(5000);
                                //_IDRLoginPage.UserNameTextBox.WaitFor(30);
                                _IDRLoginPage.UserNameTextBox.EnterText(_TestUserName);
                                _IDRLoginPage.PasswordTextBox.EnterText(_TestPassword);
                                _IDRLoginPage.ContinueButton.Click();
                                result = true;
                            }
                            catch (Exception ex) when (ex is NoSuchElementException || ex is WebDriverTimeoutException)
                            {
                                if (_Driver.GetCurrentURL().Contains(@"openid-connect/auth"))
                                    continue;
                            }

                            break;
                        }
                    case LogonTypeEnum.BCID:
                        {
                            try
                            {
                                _PathFinderPage.BCIDButton.Click();
                                Thread.Sleep(5000);
                                _BCIDPage.UserNameTextBox.WaitFor(5);
                                _BCIDPage.UserNameTextBox.EnterText(_TestUserName);
                                _BCIDPage.PasswordTextBox.EnterText(_TestPassword);
                                _BCIDPage.ContinueButton.Click();
                                result = true;
                            }
                            catch (Exception ex) when (ex is NoSuchElementException || ex is WebDriverTimeoutException)
                            {
                                if (_Driver.GetCurrentURL().Contains(@"openid-connect/auth"))
                                    continue;
                            }

                            break;
                        }
                }
            }

            if (result == false)
                logonType = null;

            return (logonType);
        }
    }
}
