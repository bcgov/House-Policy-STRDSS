using TestFrameWork.Models;
using UITest.PageObjects;
using UITest.TestDriver;
using Configuration;

using Xunit.Abstractions;
using TestFrameWork.PageObjects;
using OpenQA.Selenium;

namespace XUnitTests
{

    public class HousrSTRE2E
    {
        private ITestOutputHelper _Output;
        private LandingPage _HomePage;
        private TermsAndConditionsPage _TermsAndConditionsPage;
        private DelistingRequestPage _DelistingRequestPage;
        private DelistingWarningPage _DelistingWarningPage;
        private NoticeOfTakeDownPage _NoticeOfTakeDownPage;
        private TakeDownRequestPage _TakeDownRequestPage;
        private PathFinderPage _PathFinderPage;
        private IDirPage _IDirPage;
        private RequestAccessPage _RequestAccessPage;
        private string _TestUserName;
        private string _TestPassword;
        private IDriver _Driver;
        private string _FeURL = "http://127.0.0.1:4200";

        public HousrSTRE2E(ITestOutputHelper output)
        {
            this._Output = output;

            _TestUserName = "CEUATST";
            _Driver = new SeleniumDriver(SeleniumDriver.DRIVERTYPE.CHROME);
            _HomePage = new LandingPage(_Driver);
            _TermsAndConditionsPage = new TermsAndConditionsPage(_Driver);
            _DelistingRequestPage = new DelistingRequestPage(_Driver);
            _DelistingWarningPage = new DelistingWarningPage(_Driver);
            _NoticeOfTakeDownPage = new NoticeOfTakeDownPage(_Driver);
            _TakeDownRequestPage = new TakeDownRequestPage(_Driver);
            _PathFinderPage = new PathFinderPage(_Driver);
            _IDirPage = new IDirPage(_Driver);
            AppSettings appSettings = new AppSettings();
            _TestPassword = appSettings.GetUser(_TestUserName) ?? string.Empty;
        }

        [Fact]
        public void SendTakeDownRequestWithoutADSSListing()
        {

            try
            {
                _Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
                _Driver.Url = _FeURL+ "/delisting-request";
                _Driver.Navigate();

                _PathFinderPage.IDRButton.WaitFor();
                _PathFinderPage.IDRButton.Click();


                _IDirPage.UserNameTextBox.WaitFor();
                _IDirPage.UserNameTextBox.EnterText(_TestUserName);

                _IDirPage.PasswordTextBox.WaitFor();
                _IDirPage.PasswordTextBox.EnterText(_TestPassword);

                _IDirPage.ContinueButton.WaitFor();
                _IDirPage.ContinueButton.Click();

                //handle terms and conditions

                try
                {
                    if (_DelistingRequestPage.Driver.PageSource.Contains("Terms and Conditions"))
                    {
                        //Nested Angular controls obscure the TermsAndConditionsCheckbox. Need JS 
                        _TermsAndConditionsPage.TermsAndConditionsCheckBox.ExecuteJavaScript(@"document.querySelector(""body > app-root > app-layout > div.content > app-terms-and-conditions > p-card > div > div.p-card-body > div > div > div.checkbox-container > p-checkbox > div > div.p-checkbox-box"").click()");
                        _TermsAndConditionsPage.ContinueButton.Click();
                    }
                }
                catch (NoSuchElementException ex)
                {
                    //No terms and conditions present. Continue
                }

                //Enter ListingID Number
                _DelistingRequestPage.ListingIDNumberTextBox.EnterText("1");

                //Test with Valid email address
                _DelistingRequestPage.AdditionalCCsTextBox.EnterText("foo@foo.com");

                _DelistingRequestPage.ListingUrlTextBox.EnterText("http://listingUrl.com");

                _DelistingRequestPage.RequestInitiatedByDropDown.Click();
                _DelistingRequestPage.RequestInitiatedByDropDown.ExecuteJavaScript(@"document.querySelector(""#lgId_0"").click()");

                _DelistingRequestPage.PlatformReceipientDropdown.Click();
                _DelistingRequestPage.PlatformReceipientDropdown.ExecuteJavaScript(@"document.querySelector(""#platformId_0"").click()");
                Assert.Contains("TEST PLATFORM", _DelistingRequestPage.PlatformReceipientDropdown.Text.ToUpper());

                _DelistingRequestPage.ReviewButton.Click();

                _TakeDownRequestPage.SubmitButton.Click();

                //Validate successful submission 

                //Wait for page source to load
                System.Threading.Thread.Sleep(3000);
                Assert.True(_DelistingRequestPage.Driver.PageSource.Contains("Your Notice of Takedown was Successfully Submitted!"));
                _DelistingRequestPage.ReturnHomeButton.Click();

                
            }
            finally
            {
                _Driver.Close();
            }
        }

        [Fact]
        public void SendNoticeOfTakedownWithoutADSSlisting()
        {

            try
            {
                _Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
                _Driver.Url = _FeURL + "/compliance-notice";
                _Driver.Navigate();

                _PathFinderPage.IDRButton.WaitFor();
                _PathFinderPage.IDRButton.Click();


                _IDirPage.UserNameTextBox.WaitFor();
                _IDirPage.UserNameTextBox.EnterText(_TestUserName);

                _IDirPage.PasswordTextBox.WaitFor();
                _IDirPage.PasswordTextBox.EnterText(_TestPassword);

                _IDirPage.ContinueButton.WaitFor();
                _IDirPage.ContinueButton.Click();


                //Handle Terms and Conditions
                try
                {
                    if (_DelistingWarningPage.Driver.PageSource.Contains("Terms and Conditions"))
                    {
                        //Nested Angular controls obscure the TermsAndConditionsCheckbox. Need JS 
                        _TermsAndConditionsPage.TermsAndConditionsCheckBox.ExecuteJavaScript(@"document.querySelector(""body > app-root > app-layout > div.content > app-terms-and-conditions > p-card > div > div.p-card-body > div > div > div.checkbox-container > p-checkbox > div > div.p-checkbox-box"").click()");
                        _TermsAndConditionsPage.ContinueButton.Click();
                    }
                }
                catch (NoSuchElementException ex)
                {
                    //No terms and conditions present. Continue
                }

                //Add Platform receipient
                _DelistingWarningPage.PlatformReceipientDropdown.WaitFor();
                _DelistingWarningPage.PlatformReceipientDropdown.Click();
                _DelistingWarningPage.PlatformReceipientDropdown.ExecuteJavaScript(@"document.querySelector(""#platformId_0"").click()");
                Assert.Contains("TEST PLATFORM", _DelistingWarningPage.PlatformReceipientDropdown.Text.ToUpper());

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

                //Wait for page source to load
                System.Threading.Thread.Sleep(3000);
                Assert.True(_DelistingWarningPage.Driver.PageSource.Contains("Your Notice of Takedown was Successfully Submitted!"));

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


                _IDirPage.UserNameTextBox.WaitFor();
                _IDirPage.UserNameTextBox.EnterText(_TestUserName);

                _IDirPage.PasswordTextBox.WaitFor();
                _IDirPage.PasswordTextBox.EnterText(_TestPassword);

                _IDirPage.ContinueButton.WaitFor();
                _IDirPage.ContinueButton.Click();

                _RequestAccessPage.UserRoleDropDown.Click();
                _RequestAccessPage.UserRoleDropDown.WaitFor();
                _RequestAccessPage.UserRoleDropDown.ExecuteJavaScript(@"document.querySelector(""#organizationType_0"").click()");
                _RequestAccessPage.UserOrganizationTextBox.EnterText("Test Org");
                _RequestAccessPage.SubmitButton.Click();
            }
            finally
            {
                _Driver.Close();
            }
        }
    }
}