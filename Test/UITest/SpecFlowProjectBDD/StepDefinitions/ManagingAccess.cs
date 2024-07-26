using Configuration;
using DataBase.Entities;
using DataBase.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using OpenQA.Selenium;
using OpenQA.Selenium.DevTools.V118.Debugger;
using SpecFlowProjectBDD.Helpers;
using System.Reflection.Metadata;
using TechTalk.SpecFlow.CommonModels;
using TestFrameWork.Models;
using UITest.PageObjects;
using UITest.TestDriver;
using static SpecFlowProjectBDD.SFEnums;

namespace SpecFlowProjectBDD.StepDefinitions
{
    [Binding]
    [Scope(Scenario = "ManagingAccess")]
    public sealed class ManagingAccess
    {
        private IDriver _Driver;
        private LandingPage _LandingPage;
        private TermsAndConditionsPage _TermsAndConditionsPage;
        private ManagingAccessPage _ManagingAccessPage;
        private PathFinderPage _PathFinderPage;
        private IDirLoginPage _IDirPage;
        private NoticeOfTakeDownPage _NoticeOfTakeDownPage;
        private string _TestUserName;
        private string _TestPassword;
        private bool _ExpectedResult = false;
        AppSettings _AppSettings;
        private DssUserIdentity _RequestingUserIdentity;
        private bool _OriginalEnabledValue;
        private string _OriginalAccessRequestStatusCd = string.Empty;
        private DssDbContext _DssDBContext;
        private IUnitOfWork _UnitOfWork;
        private SFEnums.Environment _Environment = SFEnums.Environment.LOCAL;

        public ManagingAccess(SeleniumDriver Driver)
        {
            _Driver = Driver;
            _LandingPage = new LandingPage(_Driver);
            _TermsAndConditionsPage = new TermsAndConditionsPage(Driver);
            _ManagingAccessPage = new ManagingAccessPage(_Driver);
            _NoticeOfTakeDownPage = new NoticeOfTakeDownPage(_Driver);
            _PathFinderPage = new PathFinderPage(_Driver);
            _IDirPage = new IDirLoginPage(_Driver);
            _AppSettings = new AppSettings();

            DbContextOptions<DssDbContext> dbContextOptions = new DbContextOptions<DssDbContext>();

            string dbConnectionString = _AppSettings.GetConnectionString(_Environment.ToString().ToLower()) ?? string.Empty;

            _DssDBContext = new DssDbContext(dbContextOptions, dbConnectionString);
            _UnitOfWork = new UnitOfWork(_DssDBContext);
        }

        [SetUp]
        public void Setup()
        {
        }

        [AfterScenario("ManagingAccess")]
        public void TearDown()
        {
            if (null != _RequestingUserIdentity)
            {
                _RequestingUserIdentity.AccessRequestStatusCd = _OriginalAccessRequestStatusCd;
                _DssDBContext.SaveChanges();
            }
        }


        //User Authentication
        //[Given(@"I am an authenticated LG staff member and the expected result is ""(.*)""")]
        [Given(@"that I am an authenticated government user ""(.*)"" and the expected result is ""(.*)""")]
        public void GivenIAmAauthenticatedGovernmentUser(string UserName, string ExpectedResult)
        {
            _TestUserName = UserName;
            _TestPassword = _AppSettings.GetUser(_TestUserName) ?? string.Empty;

            _ExpectedResult = ExpectedResult.ToUpper() == "PASS" ? true : false;

            _Driver.Url = _AppSettings.GetServer("default");
            _Driver.Navigate();

            _PathFinderPage.IDRButton.Click();
            AuthHelper authHelper = new AuthHelper(_Driver);

            //Authenticate user using IDir or BCID depending on the user
            authHelper.Authenticate(_TestUserName, _TestPassword, UserTypeEnum.BCGOVERNMENTSTAFF);


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
                _TermsAndConditionsPage.TermsAndConditionsCheckBox.JSExecuteJavaScript(@"document.querySelector(""body > app-root > app-layout > div.content > app-terms-and-conditions > p-card > div > div.p-card-body > div > div > div.checkbox-container > p-checkbox > div > div.p-checkbox-box"").click()");
                _TermsAndConditionsPage.ContinueButton.Click();
            }
        }


        [When("I access the administrative interface of the system")]
        public void IAccessTheAdministrativeInterfaceOfTheSystem()
        {
            _LandingPage.ManageAccessRequestsButton.Click();
        }

        [Then("There should be a dedicated section for managing user access requests")]
        public void ThereShouldBeADedicatedSectionForManagingUserAccessRequests()
        {
            string selector = "body > app-root > app-layout > div.content > app-user-management > div.table-card-container";

            //bool result = (bool)_ManagingAccessPage.UserTable.JSCheckVisability(selector);
            bool result = (bool)_ManagingAccessPage.UserTable.IsEnabled();

            ClassicAssert.IsTrue(result);
        }


        //#User Access Request List


        [When("I navigate to the user access request section")]
        public void INavigateToTheUserAccessRequestSection()
        {
        }

        [Then("I should see a list displaying all user access requests, including relevant details such as the user's name, role request, and date of submission")]
        public void IShouldSeeAListDisplayingAllUserAccessRequestss()
        {
            bool result = false;

            result = (bool)_ManagingAccessPage.UserTable.JSExecuteJavaScript(@"document.querySelector(""#user-table"").checkVisibility()");
            ClassicAssert.IsTrue(result);

            result = (bool)_ManagingAccessPage.UserTable.JSExecuteJavaScript(@"document.querySelector(""#table-header"").checkVisibility()");
            ClassicAssert.IsTrue(result);

            result = (bool)_ManagingAccessPage.UserTable.JSExecuteJavaScript(@"document.querySelector(""#givenNm_th"").textContent.toLowerCase().trim() === ""first name""");

            ClassicAssert.IsTrue(result);

            result = (bool)_ManagingAccessPage.UserTable.JSExecuteJavaScript(@"document.querySelector(""#familyNm_th"").textContent.toLowerCase().trim() === 'last name'");
            ClassicAssert.IsTrue(result);

            result = (bool)_ManagingAccessPage.UserTable.JSExecuteJavaScript(@"document.querySelector(""#orgName_th"").textContent.toLowerCase().trim() === ""organization""");
            ClassicAssert.IsTrue(result);

            result = (bool)_ManagingAccessPage.UserTable.JSExecuteJavaScript(@"document.querySelector(""#emailAddressDsc_th"").textContent.toLowerCase().trim() === ""email address""");
            ClassicAssert.IsTrue(result);
        }


        //Request Details
        [When(@"Reviewing a specific access request")]
        public void ReviewingASpecificAccessRequest()
        {
        }

        [Then("I should be able to view detailed information provided by the user, including their role request and any justifications or additional comments")]
        public void ShouldBeAbleToViewDetailedInformationProvidedByTheUser()
        {
            bool result = (bool)_ManagingAccessPage.UserTable.JSExecuteJavaScript(@"document.querySelector(""#row-0"").checkVisibility()");
            ClassicAssert.IsTrue(result);
        }

        //Grant Access Button
        [When("Reviewing an access request")]
        public void ReviewingAnAccessRequest()
        {
        }

        [Then(@"There should be a Grant Access button allowing me to approve the user's request ""(.*)""")]
        public void ThereShouldBeAGrantAccessButton(string RequestingAccessUserEmail)
        {

            //Get email for first user in list
            string email = (string)_ManagingAccessPage.UserTable.JSExecuteJavaScript(@"document.querySelector(""#row-0 > td:nth-child(6)"").innerText");

            //////////////////// DB Setup ////////////////////////////////////////
            // Retrieve the user identity
            _RequestingUserIdentity = _DssDBContext.DssUserIdentities.FirstOrDefault(p => p.EmailAddressDsc == email);
            if (null == _RequestingUserIdentity)
            {
                throw new NotFoundException($"{email} not found in Identities table");
            }
            _OriginalAccessRequestStatusCd = _RequestingUserIdentity.AccessRequestStatusCd;
            _RequestingUserIdentity.AccessRequestStatusCd = "Requested";

            _DssDBContext.SaveChanges();

            /////////////////////////////////////////////////////////////

            _ManagingAccessPage.Driver.Navigate().Refresh();

            bool result = false;

            //Wait for control to become visable
            for (int i = 0; i <= 3; i++)
            {
                if ((bool)_ManagingAccessPage.UserTable.JSExecuteJavaScript(@"document.querySelector(""#row-0 > td:nth-child(8)"").checkVisibility()"))
                {
                    result = true;
                    break;
                }
                System.Threading.Thread.Sleep(1000);
            }
            result = (bool)_ManagingAccessPage.UserTable.JSExecuteJavaScript(@"document.querySelector(""#row-0 > td:nth-child(8)"").checkVisibility()");

            ClassicAssert.IsTrue(result);
        }

        //Role Assignment - NYI
        [When("Clicking the Grant Access button")]
        public void ClickingTheGrantAccessButton()
        {
            _ManagingAccessPage.UserTable.JSExecuteJavaScript(@"document.querySelector(""#form-approve-0-btn"").click()");
        }

        [Then("I should be prompted to assign the appropriate roles to the user based on their request and the system's role hierarchy")]
        public void IShouldBePromptedToAssignTheAppropriateRolesToTheUser()
        {
            _ManagingAccessPage.UserTable.JSExecuteJavaScript(@"document.querySelector(""#cancel-dialog-btn"").click()");
        }


        //Deny Access Option
        [When("Reviewing an access request for denial")]
        public void ReviewingAnAccessRequestForDenial()
        {
        }

        [Then("There should be a Deny Access option allowing me to reject the user's request if it is deemed inappropriate or unnecessary")]
        public void ThereShouldBeADenyAccessOption()
        {
        }

        //Remove Access Option
        [When("Reviewing an access request that has been granted")]
        public void ReviewingAnAccessRequestThatHasBeenGranted()
        {
        }

        [Then("There should be a Remove Access option allowing me to remove the user's access if it is deemed inappropriate or unnecessary")]
        public void ThereShouldBeARemoveAccessOption()
        {
            bool result = (bool)_ManagingAccessPage.UserTable.JSExecuteJavaScript(@"document.querySelector(""#access-status-0-insw"").checkVisibility()");
            ClassicAssert.IsTrue(result);
        }

        //Confirmation Message
        [When("Granting or denying access")]
        public void GrantingOrDenyingAccess()
        {
        }

        [Then("I should receive a confirmation message indicating the success of the action taken, and the user should be notified accordingly")]
        public void IShouldReceiveAConfirmationMessageIndicatingTheSuccessOfTheActionTaken()
        {
        }

        //User Access Status Updates
        [When("Managing user access requests")]
        public void ManagingUserAccessRequests()
        {
        }

        [Then("The access request list should dynamically update to reflect the current status approved or denied of each request")]
        public void TheAccessRequestListShouldDynamicallyUpdate()
        {
        }

    }
}
