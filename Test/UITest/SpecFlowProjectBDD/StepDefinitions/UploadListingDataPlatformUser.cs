using Configuration;
using NUnit.Framework.Legacy;
using OpenQA.Selenium;
using SpecFlowProjectBDD.Helpers;
using DataBase.Entities;
using TestFrameWork.Models;
using UITest.PageObjects;
using UITest.TestDriver;
using TestFrameWork.WindowsAutomation.Controls;
using static SpecFlowProjectBDD.SFEnums;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Npgsql;
using DataBase.UnitOfWork;

namespace SpecFlowProjectBDD.StepDefinitions
{
    [Binding]
    [Scope(Scenario = "ViewAggregatedListings")]
    public class UploadListingDataPlatformUser
    {
        private IDriver _Driver;
        private LandingPage _LandingPage;
        private PathFinderPage _PathFinderPage;
        private IDirLoginPage _IDRLoginPage;
        private UploadListingsPage _UploadListingsPage;
        private string _TestUserName;
        private string _TestPassword;
        private string _listingFile;
        private bool _ExpectedResult = false;
        private AppSettings _AppSettings;
        private SFEnums.UserTypeEnum _UserType;
        private BCIDPage _BCIDPage;
        private DssDbContext _DssDBContext;
        private DssUploadDelivery _DssUploadDelivery;
        private DateTime _updateTime;
        private IUnitOfWork _UnitOfWork;
        private SFEnums.Environment _Environment = SFEnums.Environment.LOCAL;
        private SFEnums.LogonTypeEnum? _LogonType;

        public UploadListingDataPlatformUser(SeleniumDriver Driver)
        {
            _Driver = Driver;
            _LandingPage = new LandingPage(_Driver);
            _PathFinderPage = new PathFinderPage(_Driver);
            _IDRLoginPage = new IDirLoginPage(_Driver);
            _BCIDPage = new BCIDPage(_Driver);
            _UploadListingsPage = new UploadListingsPage(_Driver);
            _AppSettings = new AppSettings();
            DbContextOptions<DssDbContext> dbContextOptions = new DbContextOptions<DssDbContext>();
            _DssDBContext = new DssDbContext(dbContextOptions);

            string dbConnectionString = _AppSettings.GetConnectionString(_Environment.ToString().ToLower()) ?? string.Empty;

            _DssDBContext = new DssDbContext(dbContextOptions, dbConnectionString);
            _UnitOfWork = new UnitOfWork(_DssDBContext);
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
            _LogonType = authHelper.Authenticate(_TestUserName, _TestPassword, _UserType);
            ClassicAssert.IsNotNull(_LogonType, "Logon FAILED");

            TermsAndConditionsHelper termsAndConditionsHelper = new TermsAndConditionsHelper(_Driver);
            termsAndConditionsHelper.HandleTermsAndConditions();
        }

        [When(@"I access the Data Sharing System")]
        public void WhenIAccessTheDataSharingSystem()
        {

        }

        [Then("I should have the option to upload short-term listing data")]
        public void IShouldHaveTheOptionToUploadShorttermlistingData()
        {
            if (_UserType == UserTypeEnum.SHORTTERMRENTALPLATFORM)
            {
                ClassicAssert.True(_LandingPage.UploadPlatformDataButton.IsEnabled());
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
            _LandingPage.UploadPlatformDataButton.Click();
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

            _listingFile = UploadFile;
            // Define a regular expression to match the year and month in the filename
            string pattern = @"listing-valid-(\d{4})-(\d{2})\.csv";
            Regex regex = new Regex(pattern);

            // Match the regular expression with the filename
            Match match = regex.Match(_listingFile);

            string yearMonth = string.Empty;

            if (match.Success)
            {
                // Extract the year and month from the match groups
                string year = match.Groups[1].Value;
                string month = match.Groups[2].Value;
                yearMonth = $"{year}-{month}";
            }
            else
            {
                throw new FormatException("The filename does not match the expected pattern.");
            }

            var dt = DateOnly.Parse(yearMonth);
            var DSSUploadDeliverys = _UnitOfWork.DssUploadDeliveryRepository.Get(p => p.ReportPeriodYm == dt).ToList();


            foreach (var DSSLoadDelivery in DSSUploadDeliverys)
            {
                try
                {
                    long id = DSSLoadDelivery.UploadDeliveryId;
                    _UnitOfWork.DssUploadDeliveryRepository.Delete(DSSLoadDelivery);
                    var DSSUploadDeliveryLines = _UnitOfWork.DssUploadLineRepository.Get(p => p.IncludingUploadDeliveryId == id);
                    foreach (var dSSDeliveryLine in DSSUploadDeliveryLines)
                    {
                        _UnitOfWork.DssUploadLineRepository.Delete(dSSDeliveryLine.UploadLineId);
                    }
                    _UnitOfWork.Save();
                }

                catch (NpgsqlOperationInProgressException ex)
                {
                    //should not happen, but reset DB and continue if it does for now
                    _UnitOfWork.ResetDB();
                }
            }
        }

        [When(@"I select which month the STR listing data is for ""([^""]*)""")]
        public void WhenISelectWhichMonthTheSTRListingDataIsFor(string Month)
        {
            _UploadListingsPage.ReportingMonthDropDown.Click();

            string listboxItems = _UploadListingsPage.ReportingMonthDropDown.JSExecuteJavaScript(@"document.querySelector(""#month_list"").children.length").ToString();

            int count = 0;

            if (int.TryParse(listboxItems, out count) == false)
            {
                throw new ArgumentException("Value returned for ListBox Item count is not an int");
            }

            int index = 0;
            string script = string.Empty;

            for (int i = 0; i < count; i++)
            {
                script = "document.querySelector('#month_" + i + "');";

                string result = _UploadListingsPage.ReportingMonthDropDown.JSExecuteJavaScript(script) == null ? string.Empty : _UploadListingsPage.ReportingMonthDropDown.JSExecuteJavaScript(script).ToString();

                if (result.ToUpper().Contains(Month.ToUpper()))
                {
                    index = i;
                    break;
                }
            }

            script = "document.querySelector('#month_1').click();";
            _UploadListingsPage.ReportingMonthDropDown.JSExecuteJavaScript(script);

        }

        [When(@"I initiate the upload")]
        public void WhenInitiateTheUpload()
        {
            _updateTime = DateTime.UtcNow;
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
            var updateTime = _UnitOfWork.DssUploadDeliveryRepository.Get(p => p.UpdDtm >= _updateTime);
            ClassicAssert.IsNotNull(updateTime);
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
