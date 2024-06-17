using Configuration;
using NUnit.Framework.Legacy;
using OpenQA.Selenium;
using SpecFlowProjectBDD.Helpers;
using System;
using TechTalk.SpecFlow;
using TestFrameWork.Models;
using UITest.PageObjects;
using UITest.TestDriver;
using TestFrameWork.WindowsAutomation.Controls;
using static SpecFlowProjectBDD.SFEnums;

namespace SpecFlowProjectBDD.StepDefinitions
{
    [Binding]
    public class UploadListingDataPlatformUser
    {
        private IDriver _Driver;
        private LandingPage _LandingPage;
        private TermsAndConditionsPage _TermsAndConditionsPage;
        private PathFinderPage _PathFinderPage;
        private IDirLoginPage _IDRLoginPage;
        private UploadListingsPage _UploadListingsPage;
        private string _TestUserName;
        private string _TestPassword;
        private string _listingFile;
        private bool _ExpectedResult = false;
        private AppSettings _AppSettings;
        private SFEnums.UserTypeEnum _UserType;
        private SFEnums.LogonTypeEnum _LogonType;
        private BCIDPage _BCIDPage;

        public UploadListingDataPlatformUser(SeleniumDriver Driver)
        {
            _Driver = Driver;
            _LandingPage = new LandingPage(_Driver);
            _TermsAndConditionsPage = new TermsAndConditionsPage(Driver);
            _PathFinderPage = new PathFinderPage(_Driver);
            _IDRLoginPage = new IDirLoginPage(_Driver);
            _BCIDPage = new BCIDPage(_Driver);
            _UploadListingsPage = new UploadListingsPage(_Driver);
            _AppSettings = new AppSettings();
        }


        [Given(@"I am an authenticated platform representative ""([^""]*)"" with the necessary permissions and the expected result is ""([^""]*)"" and I am a ""([^""]*)"" user")]
        public void GivenIAmAnAuthenticatedPlatformRepresentativeWithTheNecessaryPermissionsAndTheExpectedResultIsAndIAmAUser(string UserName, string ExpectedResult, string UserType)
        {
            _TestUserName = UserName;
            _TestPassword = _AppSettings.GetUser(_TestUserName) ?? string.Empty;
            _ExpectedResult = ExpectedResult.ToUpper() == "PASS" ? true : false;

            _listingFile = _AppSettings.GetListingFile("File1");

            _Driver.Url = _AppSettings.GetServer("default");
            _Driver.Navigate();

            AuthHelper authHelper = new AuthHelper(_Driver);
            UserHelper userHelper = new UserHelper();

            _UserType = userHelper.SetUserType(UserType);
            //Authenticate user using IDir or BCID depending on the user
            _LogonType = authHelper.Authenticate(UserName, _UserType);
        }

        [When(@"I access the Data Sharing System")]
        public void WhenIAccessTheDataSharingSystem()
        {
            IWebElement TOC = null;

            try
            {
                TOC = _LandingPage.Driver.FindElement(Enums.FINDBY.CSSSELECTOR, TermsAndConditionsModel.TermsAndCondititionsCheckBox);
            }
            catch (NoSuchElementException ex)
            {
                //no Terms and Conditions. Continue
            }


            if ((null != TOC) && (TOC.Displayed))
            {
                //Nested Angular controls obscure the TermsAndConditionsCheckbox. Need JS 
                _TermsAndConditionsPage.TermsAndConditionsCheckBox.ExecuteJavaScript(@"document.querySelector(""body > app-root > app-layout > div.content > app-terms-and-conditions > p-card > div > div.p-card-body > div > div > div.checkbox-container > p-checkbox > div > div.p-checkbox-box"").click()");
                _TermsAndConditionsPage.ContinueButton.Click();
            }
        }

        [Then("I should have the option to upload short-term listing data")]
        public void IShouldHaveTheOptionToUploadShorttermlistingData()
        {
            if (_UserType == UserTypeEnum.SHORTTERMRENTALPLATFORM)
            {
                ClassicAssert.True(_LandingPage.Upload_ListingsButton.IsEnabled());
            }
        }

        [When(@"I opt to upload short-term listing data")]
        public void WhenIOptToUploadShort_TermListingData()
        {
            ////throw new PendingStepException();
        }

        [Then(@"the upload data interface should load")]
        public void ThenTheUploadDataInterfaceShouldLoad()
        {
            _LandingPage.Upload_ListingsButton.Click();
        }


        [Given(@"I am on the upload data interface")]
        public void GivenIAmOnTheUploadDataInterface()
        {
            //throw new PendingStepException();
        }

        [When(@"I select a CSV file containing short-term listing data ""([^""]*)""")]
        public void WhenISelectACSVFileContainingShort_TermListingData(string UploadFile)
        {
            _UploadListingsPage.SelectFileButton.Click();
            FileDialog fileDialog = new FileDialog();
            fileDialog.FindAndSet(UploadFile, "Short-Term Rental Data Portal - Google Chrome", "Chrome_WidgetWin_1");

        }

        [When(@"I select which month the STR listing data is for")]
        public void WhenISelectWhichMonthTheSTRListingDataIsFor()
        {
            _UploadListingsPage.ReportingMonthDropDown.Click();
            _UploadListingsPage.ReportingMonthDropDown.ExecuteJavaScript(@"document.querySelector(""#month_0"").click()");

        }

        [When(@"I initiate the upload")]
        public void WhenInitiateTheUpload()
        {
            _UploadListingsPage.UploadButton.Click();
        }

        [Then(@"the Data Sharing System should import the STR listing data")]
        public void ThenTheDataSharingSystemShouldImportTheSTRListingData()
        {
            //throw new PendingStepException();
        }

        [When(@"the data import is successful")]
        public void WhenTheDataImportIsSuccessful()
        {
            //throw new PendingStepException();
        }

        [Then(@"I should see a success message")]
        public void ThenIShouldSeeASuccessMessage()
        {
            //throw new PendingStepException();
        }

        [Then(@"a new entry on an upload log with a timestamp, username, and the number of records created\.")]
        public void ThenANewEntryOnAnUploadLogWithATimestampUsernameAndTheNumberOfRecordsCreated_()
        {
            //throw new PendingStepException();
        }

        [When(@"the data import is not successful")]
        public void WhenTheDataImportIsNotSuccessful()
        {
            //throw new PendingStepException();
        }

        [Then(@"I should see a confirmation message indicating the issue\.")]
        public void ThenIShouldSeeAConfirmationMessageIndicatingTheIssue_()
        {
            //throw new PendingStepException();
        }

        [Then(@"a new entry on an import log with a timestamp, username, and information about the unsuccessful import, such as error details\.")]
        public void ThenANewEntryOnAnImportLogWithATimestampUsernameAndInformationAboutTheUnsuccessfulImportSuchAsErrorDetails_()
        {
            //throw new PendingStepException();
        }

        [When(@"the data import is complete")]
        public void WhenTheDataImportIsComplete()
        {
            //throw new PendingStepException();
        }

        [Then(@"i should receive an email confirming the status of my upload: Template: Platform Upload Error Notification")]
        public void ThenIShouldReceiveAnEmailConfirmingTheStatusOfMyUploadTemplatePlatformUploadErrorNotification()
        {
            //throw new PendingStepException();
        }

        [Then(@"a report of any error codes that need to be addressed")]
        public void ThenAReportOfAnyErrorCodesThatNeedToBeAddressed()
        {
            //throw new PendingStepException();
        }

    }
}
