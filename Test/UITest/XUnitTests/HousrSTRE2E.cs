using System.Diagnostics;
using TestFrameWork.Models;
using UITest.PageObjects;
using UITest.TestDriver;
using Configuration;

using Xunit.Abstractions;
using OpenQA.Selenium.DevTools.V118.Page;
using UITest.Models;

namespace XUnitTests
{

    public class HousrSTRE2E
    {
        private ITestOutputHelper _Output;
        private HomePage _HomePage;
        private DelistingRequestPage _DelistingRequestPage;
        private DelistingWarningPage _DelistingWarningPage;
        private NoticeOfTakeDownPage _NoticeOfTakeDownPage;
        private PathFinderPage _PathFinderPage;
        private IDRLoginPage _IDRLoginPage;
        private string _TestUserName;
        private string _TestPassword;
        private IDriver _Driver;

        public HousrSTRE2E(ITestOutputHelper output)
        {
            this._Output = output;
            _Driver = new SeleniumDriver(SeleniumDriver.DRIVERTYPE.CHROME);
            _HomePage = new HomePage(_Driver);
            _DelistingRequestPage = new DelistingRequestPage(_Driver);
            _DelistingWarningPage = new DelistingWarningPage(_Driver);
            _NoticeOfTakeDownPage = new NoticeOfTakeDownPage(_Driver);
            _PathFinderPage = new PathFinderPage(_Driver);
            _IDRLoginPage = new IDRLoginPage(_Driver);
            AppSettings appSettings = new AppSettings();
            _TestUserName = appSettings.GetValue("TestUserName") ?? string.Empty;
            _TestPassword = appSettings.GetValue("TestPassword") ?? string.Empty;
        }

        [Fact]
        public void TestLoginAndRequestDelisting()
        {

            try
            {
                _Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
                _Driver.Url = "http://127.0.0.1:4200/delisting-request";
                _Driver.Navigate();

                _PathFinderPage.IDRButton.WaitFor();
                _PathFinderPage.IDRButton.Click();


                _IDRLoginPage.UserNameTextBox.WaitFor();
                _IDRLoginPage.UserNameTextBox.EnterText(_TestUserName);

                _IDRLoginPage.PasswordTextBox.WaitFor();
                _IDRLoginPage.PasswordTextBox.EnterText(_TestPassword);

                _IDRLoginPage.ContinueButton.WaitFor();
                _IDRLoginPage.ContinueButton.Click();

                //Click to populate dropdown values
                _DelistingRequestPage.PlatformReceipientDropdown.Click();

                Assert.Contains("SELECT A PLATFORM", _DelistingRequestPage.PlatformReceipientDropdown.Text.ToUpper());

                //Click to deselect
                _DelistingRequestPage.PlatformReceipientDropdown.Click();

                //Test with Valid email address
                _DelistingRequestPage.AdditionalCCsTextBox.EnterText("foo@foo.com");



                //Test with Invalid email address
                //_DelistingRequestPage.AdditionalCCsTextBox.EnterText("foo@@joe");

                _DelistingRequestPage.ListingUrlTextBox.EnterText("http://listingUrl.com");

                _DelistingRequestPage.PlatformReceipientDropdown.Click();
                _DelistingRequestPage.PlatformReceipientDropdown.ExecuteJavaScript(@"document.querySelector(""#platformId_0"").click()");
                Assert.Contains("AIRBNB", _DelistingRequestPage.PlatformReceipientDropdown.Text.ToUpper());

                _DelistingRequestPage.ReviewButton.Click();
            }
            finally
            {
                _Driver.Close();
            }
        }

        [Fact]
        public void TestLoginAndSendWarningDelisting()
        {

            try
            {
                _Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
                _Driver.Url = "http://127.0.0.1:4200/compliance-notice";
                _Driver.Navigate();

                _PathFinderPage.IDRButton.WaitFor();
                _PathFinderPage.IDRButton.Click();


                _IDRLoginPage.UserNameTextBox.WaitFor();
                _IDRLoginPage.UserNameTextBox.EnterText(_TestUserName);

                _IDRLoginPage.PasswordTextBox.WaitFor();
                _IDRLoginPage.PasswordTextBox.EnterText(_TestPassword);

                _IDRLoginPage.ContinueButton.WaitFor();
                _IDRLoginPage.ContinueButton.Click();


                //_Driver.Url = "http://127.0.0.1:4200/delisting-request";
                //_Driver.Navigate();

                //Add Platform receipient
                _DelistingWarningPage.PlatformReceipientDropdown.Click();
                _DelistingWarningPage.PlatformReceipientDropdown.ExecuteJavaScript(@"document.querySelector(""#platformId_0"").click()");
                Assert.Contains("AIRBNB", _DelistingWarningPage.PlatformReceipientDropdown.Text.ToUpper());

                //Click to deselect
                _DelistingWarningPage.PlatformReceipientDropdown.Click();

                //add listing ID
                _DelistingWarningPage.ListingIDNumberTextBox.EnterText("0");

                //add listing URL

                _DelistingWarningPage.ListingUrlTextBox.EnterText("http://listingUrl.com");

                //Add host email address
                _DelistingWarningPage.HostEmailAddressTextBox.EnterText("hostemail@host.com");

                //check alternative notification sent
                _DelistingWarningPage.AlternativeNoticeSentCheckbox.Click();

                //select a reason for request
                _DelistingWarningPage.ReasonDropdown.Click();
                _DelistingWarningPage.ReasonDropdown.ExecuteJavaScript(@"document.querySelector(""#reasonId_0"").click()");

                //Additional CCs
                _DelistingWarningPage.AdditionalCCsTextBox.EnterText("foo@foo.com");
                

                //_DelistingWarningPage.PlatformReceipientDropdown.Click();
                //_DelistingWarningPage.PlatformReceipientDropdown.ExecuteJavaScript(@"document.querySelector(""#platformId_0"").click()");
                //Assert.Contains("AIRBNB", _DelistingWarningPage.PlatformReceipientDropdown.Text.ToUpper());

                //local gov email
                _DelistingWarningPage.LocalGovEmailTextBox.EnterText("Local@gov.com");

                //local gov phone
                _DelistingWarningPage.LocalGovPhoneTextBox.EnterText("999-999-9999");

                //local gov STR by-law info
                _DelistingWarningPage.LocalGovUrlTextBox.EnterText("http://STRBylaw.local.gov");

                _DelistingWarningPage.ReviewButton.Click();

                _NoticeOfTakeDownPage.CommentsTextBox.EnterText("get a business license");
                _NoticeOfTakeDownPage.SubmitButton.Click();


            }
            finally
            {
                _Driver.Close();
            }
        }
    }
}