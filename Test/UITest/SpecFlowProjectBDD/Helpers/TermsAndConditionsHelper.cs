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
    public class TermsAndConditionsHelper
    {
        private IDriver _Driver;
        private TermsAndConditionsPage _TermsAndConditionsPage;
        private IDirLoginPage _IDRLoginPage;
        private string _TestUserName;
        private string _TestPassword;
        private AppSettings _AppSettings;
        private BCIDPage _BCIDPage;

        public TermsAndConditionsHelper(IDriver Driver)
        {
            _Driver = Driver;
            _TermsAndConditionsPage = new TermsAndConditionsPage(Driver);
            _AppSettings = new AppSettings();
        }

        public bool HandleTermsAndConditions()
        {
            try
            {
                Thread.Sleep(2000);
                _TermsAndConditionsPage.TermsAndConditionsCheckBox.Click();
                _TermsAndConditionsPage.ContinueButton.Click();
            }
            catch (NoSuchElementException ex)
            {
                //no Terms and Conditions. Continue
            }
            catch (ElementClickInterceptedException ex)
            {
                Thread.Sleep(5000);
                _TermsAndConditionsPage.TermsAndConditionsCheckBox.Click();
                _TermsAndConditionsPage.ContinueButton.Click();
            }

            return (true);
        }
    }
}
