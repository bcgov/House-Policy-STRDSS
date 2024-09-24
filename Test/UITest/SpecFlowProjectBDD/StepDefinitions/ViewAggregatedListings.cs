using Configuration;
using DataBase.Entities;
using DataBase.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework.Legacy;
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
            var rowData = _AggregatedListingsPage.AggregatedListingsTable.GetHeaderRow();
            ClassicAssert.AreEqual("Primary Host Name", rowData[1]);
            ClassicAssert.AreEqual("Address (Best Match)", rowData[2]);
            ClassicAssert.AreEqual("Nights Stayed (YTD)", rowData[3]);
            ClassicAssert.AreEqual("Listing’s Business Licence", rowData[4]);
            ClassicAssert.AreEqual("Last Action", rowData[5]);
            ClassicAssert.AreEqual("Last Action Date", rowData[6]);
        }

        [Then(@"the ability to view drop down child rows under each parent row  so that I can review all listings that are associated with a property grouping")]
        public void ThenTheAbilityToViewDropDownChildRowsUnderEachParentRowSoThatICanReviewAllListingsThatAreAssociatedWithAPropertyGrouping()
        {
            var rowData = _AggregatedListingsPage.AggregatedListingsTable.GetRow(1);

            _AggregatedListingsPage.AggregatedListingsTable.JSExecuteJavaScript("document.querySelector(\"#expand-listing-row-0\").click()");

            //throw new PendingStepException();
        }

        [Given(@"I am viewing a listing on the aggregated listing page")]
        public void GivenIAmViewingAListingOnTheAggregatedListingPage()
        {
            //throw new PendingStepException();
        }

        [Then(@"I should see information that is “common” to all listings associated with a property, including:")]
        public void ThenIShouldSeeInformationThatIsCommonToAllListingsAssociatedWithAPropertyIncluding()
        {

            ClassicAssert.AreEqual(_AggregatedListingsPage.AggregatedListingsTable.JSExecuteJavaScript("document.querySelector(\"#pn_id_24-table > thead > tr > th:nth-child(2)\").innerText") , "Status");
            ClassicAssert.AreEqual(_AggregatedListingsPage.AggregatedListingsTable.JSExecuteJavaScript("document.querySelector(\"#pn_id_24-table > thead > tr > th:nth-child(3)\").innerText") , "Platform");
            ClassicAssert.AreEqual(_AggregatedListingsPage.AggregatedListingsTable.JSExecuteJavaScript("document.querySelector(\"#pn_id_24-table > thead > tr > th:nth-child(4)\").innerText") , "Listing ID");
            ClassicAssert.AreEqual(_AggregatedListingsPage.AggregatedListingsTable.JSExecuteJavaScript("document.querySelector(\"#pn_id_24-table > thead > tr > th:nth-child(5)\").innerText") , "Listing Details");
            ClassicAssert.AreEqual(_AggregatedListingsPage.AggregatedListingsTable.JSExecuteJavaScript("document.querySelector(\"#pn_id_24-table > thead > tr > th:nth-child(6)\").innerText") , "Address (Best Match)");
            ClassicAssert.AreEqual(_AggregatedListingsPage.AggregatedListingsTable.JSExecuteJavaScript("document.querySelector(\"#pn_id_24-table > thead > tr > th:nth-child(7)\").innerText") , "Nights Stayed (YTD)");
            ClassicAssert.AreEqual(_AggregatedListingsPage.AggregatedListingsTable.JSExecuteJavaScript("document.querySelector(\"#pn_id_24-table > thead > tr > th:nth-child(8)\").innerText") , "Business Licence on Listing");
            ClassicAssert.AreEqual(_AggregatedListingsPage.AggregatedListingsTable.JSExecuteJavaScript("document.querySelector(\"#pn_id_24-table > thead > tr > th:nth-child(9)\").innerText") , "Matched Business Licence");
            ClassicAssert.AreEqual(_AggregatedListingsPage.AggregatedListingsTable.JSExecuteJavaScript("document.querySelector(\"#pn_id_24-table > thead > tr > th:nth-child(10)\").innerText") , "Last Action");
            ClassicAssert.AreEqual(_AggregatedListingsPage.AggregatedListingsTable.JSExecuteJavaScript("document.querySelector(\"#pn_id_24-table > thead > tr > th:nth-child(11)\").innerText") , "Last Action Date");
            //document.querySelector("#pn_id_26-table > thead > tr > th:nth-child(2)")
            //throw new PendingStepException();
        }

        [Then(@"I should have the option to expand a dropdown to view all child rows with listings associated with the parent row")]
        public void ThenIShouldHaveTheOptionToExpandADropdownToViewAllChildRowsWithListingsAssociatedWithTheParentRow()
        {
            //throw new PendingStepException();
        }

        [When(@"I expand the dropdown to view all child rows,")]
        public void WhenIExpandTheDropdownToViewAllChildRows()
        {
            //throw new PendingStepException();
        }

        [Then(@"I should see key information for each listing, including:")]
        public void ThenIShouldSeeKeyInformationForEachListingIncluding()
        {
            //throw new PendingStepException();
        }
    }
}
