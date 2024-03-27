using TestFrameWork.Models;
using UITest.PageObjects;
using UITest.TestDriver;
using Configuration;

using Xunit.Abstractions;
using TestFrameWork.PageObjects;

namespace XUnitTests
{

    public class HousrSTRE2E
    {
        private ITestOutputHelper _Output;
        private HomePage _HomePage;
        private DelistingRequestPage _DelistingRequestPage;
        private DelistingWarningPage _DelistingWarningPage;
        private NoticeOfTakeDownPage _NoticeOfTakeDownPage;
        private TakeDownRequestPage _TakeDownRequestPage;
        private PathFinderPage _PathFinderPage;
        private IDRLoginPage _IDRLoginPage;
        private RequestAccessPage _RequestAccessPage;
        private string _TestUserName;
        private string _TestPassword;
        private IDriver _Driver;
        private string _FeURL = "http://127.0.0.1:4200";

        public HousrSTRE2E(ITestOutputHelper output)
        {
            this._Output = output;
            _Driver = new SeleniumDriver(SeleniumDriver.DRIVERTYPE.CHROME);
            _HomePage = new HomePage(_Driver);
            _DelistingRequestPage = new DelistingRequestPage(_Driver);
            _DelistingWarningPage = new DelistingWarningPage(_Driver);
            _NoticeOfTakeDownPage = new NoticeOfTakeDownPage(_Driver);
            _TakeDownRequestPage = new TakeDownRequestPage(_Driver);
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
                _Driver.Url = _FeURL+ "/delisting-request";
                _Driver.Navigate();

                _PathFinderPage.IDRButton.WaitFor();
                _PathFinderPage.IDRButton.Click();


                _IDRLoginPage.UserNameTextBox.WaitFor();
                _IDRLoginPage.UserNameTextBox.EnterText(_TestUserName);

                _IDRLoginPage.PasswordTextBox.WaitFor();
                _IDRLoginPage.PasswordTextBox.EnterText(_TestPassword);

                _IDRLoginPage.ContinueButton.WaitFor();
                _IDRLoginPage.ContinueButton.Click();

                //Enter ListingID Number
                _DelistingRequestPage.ListingIDNumberTextBox.EnterText("1");

                //Test with Valid email address
                _DelistingRequestPage.AdditionalCCsTextBox.EnterText("foo@foo.com");

                _DelistingRequestPage.ListingUrlTextBox.EnterText("http://listingUrl.com");

                _DelistingRequestPage.RequestInitiatedByDropDown.Click();
                _DelistingRequestPage.RequestInitiatedByDropDown.ExecuteJavaScript(@"document.querySelector(""#lgId_0"").click()");

                _DelistingRequestPage.PlatformReceipientDropdown.Click();
                _DelistingRequestPage.PlatformReceipientDropdown.ExecuteJavaScript(@"document.querySelector(""#platformId_0"").click()");
                Assert.Contains("AIRBNB", _DelistingRequestPage.PlatformReceipientDropdown.Text.ToUpper());

                _DelistingRequestPage.ReviewButton.Click();

                _TakeDownRequestPage.SubmitButton.Click();

                //Validate successful submission 
                Assert.True(_DelistingRequestPage.EmbededDriver.PageSource.Contains("Your Notice of Takedown was Successfully Submitted!"));
                _DelistingRequestPage.ReturnHomeButton.Click();

                
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
                _Driver.Url = _FeURL + "/compliance-notice";
                _Driver.Navigate();

                _PathFinderPage.IDRButton.WaitFor();
                _PathFinderPage.IDRButton.Click();


                _IDRLoginPage.UserNameTextBox.WaitFor();
                _IDRLoginPage.UserNameTextBox.EnterText(_TestUserName);

                _IDRLoginPage.PasswordTextBox.WaitFor();
                _IDRLoginPage.PasswordTextBox.EnterText(_TestPassword);

                _IDRLoginPage.ContinueButton.WaitFor();
                _IDRLoginPage.ContinueButton.Click();

                //Add Platform receipient
                _DelistingWarningPage.PlatformReceipientDropdown.WaitFor();
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
                
                //local gov email
                _DelistingWarningPage.LocalGovEmailTextBox.EnterText("Local@gov.com");

                //local gov phone
                _DelistingWarningPage.LocalGovPhoneTextBox.EnterText("999-999-9999");

                //local gov STR by-law info
                _DelistingWarningPage.LocalGovUrlTextBox.EnterText("http://STRBylaw.local.gov");


                //Click Review Button
                _DelistingWarningPage.ReviewButton.Click();

                //Add comment and submit the request
                _NoticeOfTakeDownPage.CommentsTextBox.EnterText("get a business license");
                _NoticeOfTakeDownPage.SubmitButton.Click();


                //Validate successful submission 
                Assert.True(_DelistingWarningPage.EmbededDriver.PageSource.Contains("Your Notice of Takedown was Successfully Submitted!"));

                //If submit is sucessful, then return  to home page
                _DelistingWarningPage.ReturnHomeButton.Click();
            }
            finally
            {
                _Driver.Close();
            }
        }

        [Fact]
        public void TestRequestAccess()
        {
            _RequestAccessPage = new RequestAccessPage(_Driver);
            try
            {
                _Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
                _Driver.Url = _FeURL + "/access-request";
                _Driver.Navigate();

                _PathFinderPage.IDRButton.WaitFor();
                _PathFinderPage.IDRButton.Click();


                _IDRLoginPage.UserNameTextBox.WaitFor();
                _IDRLoginPage.UserNameTextBox.EnterText(_TestUserName);

                _IDRLoginPage.PasswordTextBox.WaitFor();
                _IDRLoginPage.PasswordTextBox.EnterText(_TestPassword);

                _IDRLoginPage.ContinueButton.WaitFor();
                _IDRLoginPage.ContinueButton.Click();

                _RequestAccessPage.UserRoleDropDown.Click();
                _RequestAccessPage.UserRoleDropDown.ExecuteJavaScript("@\"document.querySelector(\"\"##organizationType_0\"\").click()\"");
                _RequestAccessPage.UserOrganizationTextBox.EnterText("Test Org");

            }
            finally
            {
                _Driver.Close();
            }
        }
    }
}