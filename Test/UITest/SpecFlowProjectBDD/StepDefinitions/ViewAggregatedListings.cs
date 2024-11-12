using Configuration;
using DataBase.Entities;
using DataBase.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework.Legacy;
using OpenQA.Selenium;
using SpecFlowProjectBDD.Helpers;
using System;
using System.Reflection.Metadata;
using System.Xml.Linq;
using TechTalk.SpecFlow;
using TestFrameWork.Models;
using UITest.PageObjects;
using UITest.SeleniumObjects;
using UITest.TestDriver;

namespace SpecFlowProjectBDD.StepDefinitions
{
    [Binding]
    public class ViewAggregatedListings
    {
        private IDriver _Driver;
        private LandingPage _LandingPage;
        private AggregatedListingsPage _AggregatedListingsPage;
        private BulkComplianceNoticePage _BulkComplianceNoticePage;
        private DelistingWarningPage _DelistingWarningPage;
        private PathFinderPage _PathFinderPage;
        private BCIDPage _BCIDPage;
        private NoticeOfTakeDownPage _NoticeOfTakeDownPage;
        private string _TestUserName;
        private string _TestPassword;
        private bool _ExpectedResult = false;
        private SFEnums.UserTypeEnum _UserType;
        private SFEnums.LogonTypeEnum? _LogonType;
        private AppSettings _AppSettings;
        private DateTime _updateTime;
        private DssDbContext _DssDBContext;
        private IUnitOfWork _UnitOfWork;
        private SFEnums.Environment _Environment = SFEnums.Environment.LOCAL;

        public ViewAggregatedListings(SeleniumDriver Driver)
        {
            _Driver = Driver;
            _LandingPage = new LandingPage(_Driver);
            _AggregatedListingsPage = new AggregatedListingsPage(_Driver);
            _BulkComplianceNoticePage = new BulkComplianceNoticePage(_Driver);
            _DelistingWarningPage = new DelistingWarningPage(_Driver);
            _NoticeOfTakeDownPage = new NoticeOfTakeDownPage(_Driver);
            _PathFinderPage = new PathFinderPage(_Driver);
            _BCIDPage = new BCIDPage(_Driver);
            _AppSettings = new AppSettings();
            DbContextOptions<DssDbContext> dbContextOptions = new DbContextOptions<DssDbContext>();
            _DssDBContext = new DssDbContext(dbContextOptions);

            string dbConnectionString = _AppSettings.GetConnectionString(_Environment.ToString().ToLower()) ?? string.Empty;

            _DssDBContext = new DssDbContext(dbContextOptions, dbConnectionString);
            _UnitOfWork = new UnitOfWork(_DssDBContext);

        }

        [Given(@"that I am an authenticated user ""(.*)"" with the necessary permissions to view listings and the expected result is ""(.*)"" and I am a ""(.*)"" user")]
        public void GivenIAmAauthenticatedLGStaffMemberUser(string UserName, string ExpectedResult = "pass", string UserType = "")
        {
            _TestUserName = UserName;
            _TestPassword = _AppSettings.GetUser(_TestUserName) ?? string.Empty;
            _ExpectedResult = ExpectedResult.ToUpper() == "PASS" ? true : false;

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

        [When(@"I access the Data Portal")]
        public void WhenIAccessTheDataPortal()
        {

        }

        [Given(@"that I am on the aggregated listing page")]
        public void GivenThatIAmOnTheAggregatedListingPage()
        {

            _LandingPage.AggregatedListingsButton.Click();
        }

        [Then(@"I should see a parent row for each STR “property grouping “with information that is common to all listings associated with the group")]
        public void ThenIShouldSeeAParentRowForEachSTRPropertyGroupingWithInformationThatIsCommonToAllListingsAssociatedWithTheGroup()
        {

        }

        [Then(@"the ability to view drop down child rows under each parent row  so that I can review all listings that are associated with a property grouping")]
        public void ThenTheAbilityToViewDropDownChildRowsUnderEachParentRowSoThatICanReviewAllListingsThatAreAssociatedWithAPropertyGrouping()
        {

        }

        [Given(@"I am viewing a listing on the aggregated listing page")]
        public void GivenIAmViewingAListingOnTheAggregatedListingPage()
        {
        }

        [Then(@"I should see information that is “common” to all listings associated with a property, including:")]
        public void ThenIShouldSeeInformationThatIsCommonToAllListingsAssociatedWithAPropertyIncluding()
        {
            var rowData = _AggregatedListingsPage.AggregatedListingsTable.GetHeaderRow();
            ClassicAssert.AreEqual("Primary Host Name", rowData[1]);
            ClassicAssert.AreEqual("Address (Best Match)", rowData[2]);
            ClassicAssert.AreEqual("Nights stayed (12M)".ToUpper(), rowData[3].ToUpper());
            ClassicAssert.AreEqual("Listing’s Business Licence", rowData[4]);
            ClassicAssert.AreEqual("Last Action", rowData[5]);
            ClassicAssert.AreEqual("Last Action Date", rowData[6]);
        }

        [Then(@"I should have the option to expand a dropdown to view all child rows with listings associated with the parent row")]
        public void ThenIShouldHaveTheOptionToExpandADropdownToViewAllChildRowsWithListingsAssociatedWithTheParentRow()
        {
        }

        [When(@"I expand the dropdown to view all child rows,")]
        public void WhenIExpandTheDropdownToViewAllChildRows()
        {
            var rowData = _AggregatedListingsPage.AggregatedListingsTable.GetRow(1);

            _AggregatedListingsPage.AggregatedListingsTable.JSExecuteJavaScript("document.querySelector(\"#expand-listing-row-0\").click()");
        }

        [Then(@"I should see key information for each listing, including:")]
        public void ThenIShouldSeeKeyInformationForEachListingIncluding()
        {
            ///TODO:the code below is a short term fix. Need to create a solution which follows the POM
            var tables = _Driver.FindElements(By.CssSelector("table[id^='pn_id_']"));

            // Check if there is a second table and select the desired header cell within it
            if (tables.Count >= 2)
            {
                // Select the second table (index 1)
                var secondTable = tables[1];

                ClassicAssert.AreEqual(secondTable.FindElement(By.CssSelector("thead > tr > th:nth-child(2)")).Text, "Status");
                ClassicAssert.AreEqual(secondTable.FindElement(By.CssSelector("thead > tr > th:nth-child(3)")).Text, "Platform");
                ClassicAssert.AreEqual(secondTable.FindElement(By.CssSelector("thead > tr > th:nth-child(4)")).Text, "Listing ID");
                ClassicAssert.AreEqual(secondTable.FindElement(By.CssSelector("thead > tr > th:nth-child(5)")).Text, "Listing Details");
                ClassicAssert.AreEqual(secondTable.FindElement(By.CssSelector("thead > tr > th:nth-child(6)")).Text, "Address (Best Match)");
                ClassicAssert.AreEqual(secondTable.FindElement(By.CssSelector("thead > tr > th:nth-child(7)")).Text, "Nights stayed (12M)");
                ClassicAssert.AreEqual(secondTable.FindElement(By.CssSelector("thead > tr > th:nth-child(8)")).Text, "Business Licence on Listing");
                ClassicAssert.AreEqual(secondTable.FindElement(By.CssSelector("thead > tr > th:nth-child(9)")).Text, "Matched Business Licence");
                ClassicAssert.AreEqual(secondTable.FindElement(By.CssSelector("thead > tr > th:nth-child(10)")).Text, "Last Action");
                ClassicAssert.AreEqual(secondTable.FindElement(By.CssSelector("thead > tr > th:nth-child(11)")).Text, "Last Action Date");

            }
        }
    }
}
