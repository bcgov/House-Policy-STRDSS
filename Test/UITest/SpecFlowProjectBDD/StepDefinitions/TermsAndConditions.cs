using Configuration;
using DataBase.Entities;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using OpenQA.Selenium;
using OpenQA.Selenium.DevTools.V118.Debugger;
using SpecFlowProjectBDD.Helpers;
using TestFrameWork.Models;
using UITest.PageObjects;
using UITest.TestDriver;

namespace SpecFlowProjectBDD.StepDefinitions
{
    [Binding]
    [Scope(Scenario = "TermsAndConditions")]
    public sealed class TermsAndConditions
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
        private string _TestEmail;
        private string _TestUserType;
        private SFEnums.UserTypeEnum _UserType;
        private SFEnums.LogonTypeEnum? _LogonType;
        private bool _ExpectedResult = false;
        private AppSettings _AppSettings;
        private DssDbContext _DssDBContext;
        private DssUserIdentity _UserIdentity;
        private DssUserIdentity _OriginalUserIdentity;
        private SFEnums.Environment _Environment = SFEnums.Environment.LOCAL;

        public TermsAndConditions(SeleniumDriver Driver)
        {
            _Driver = Driver;
            _LandingPage = new LandingPage(_Driver);
            _TermsAndConditionsPage = new TermsAndConditionsPage(Driver);
            _PathFinderPage = new PathFinderPage(_Driver);
            _IDirPage = new IDirLoginPage(_Driver);
            _AppSettings = new AppSettings();
        }

        [BeforeScenario]
        public void TestSetUp()
        {
            string dbConnectionString = _AppSettings.GetConnectionString(_Environment.ToString().ToLower()) ?? string.Empty;
            DbContextOptions <DssDbContext> dbContextOptions = new DbContextOptions<DssDbContext>();

            _DssDBContext = new DssDbContext(dbContextOptions, dbConnectionString);
        }

        [Given(@"User ""(.*)"" is enabled, approved, has the correct roles ""(.*)"", but has not accepted TOC")]
        public void UserIsEnabledApprovedAndHasCorrectRoles(string UserEmail, string UserType)
        {
            _TestEmail = UserEmail;
            _TestUserType = UserType;

            // Retrieve the role
            DssUserRole userRole = _DssDBContext.DssUserRoles.FirstOrDefault(p => p.UserRoleCd == _TestUserType);

            // Retrieve the user identity
            _UserIdentity = _DssDBContext.DssUserIdentities.FirstOrDefault(p => p.EmailAddressDsc == _TestEmail);

            // Update properties of the identity
            _UserIdentity.AccessRequestStatusCd = "Approved";
            _UserIdentity.IsEnabled = true;
            _UserIdentity.TermsAcceptanceDtm = null;
            _UserIdentity.RepresentedByOrganizationId = 1;

            _DssDBContext.SaveChanges();

            userRole.UserIdentities.Add(_UserIdentity);

            // Add the identity to the CEU Admin role
            try
            {
                // Save changes to persist the relationship
                _DssDBContext.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                //Row already exists in User_role_assignment. Ignore
            }
        }


        //User Authentication
        //[Given(@"that I am an authenticated user ""(.*)"" and the expected result is ""(.*)""")]
        [Given(@"that I am an authenticated User ""(.*)"" and the expected result is ""(.*)"" and I am a ""(.*)"" user")]
        public void GivenIAmAauthenticatedGovernmentUser(string UserName, string ExpectedResult, string RoleName)
        {
            _TestUserName = UserName;
            _TestPassword = _AppSettings.GetUser(_TestUserName) ?? string.Empty;
            _ExpectedResult = ExpectedResult.ToUpper() == "PASS" ? true : false;

            UserHelper userHelper = new UserHelper();

            _UserType = userHelper.SetUserType(RoleName);

        }

        [When("I log in or access the system")]
        public void ILogInOrAccessTheSystem()
        {
            _Driver.Url = _AppSettings.GetServer("default");
            _Driver.Navigate();

            AuthHelper authHelper = new AuthHelper(_Driver);

            //Authenticate user using IDir or BCID depending on the user
            _LogonType = authHelper.Authenticate(_TestUserName, _UserType);
            ClassicAssert.IsNotNull(_LogonType, "Logon FAILED");

            //TODO: Validate that the login was sucessfull
        }

        [Then("I have not previously accepted the terms and conditions after access approval")]
        public void IHaveNotPreviouslyAcceptedTheTermsAndConditions()
        {
        }

        [Then("I should be prompted to accept the terms and conditions")]
        public void ThenIShouldBePromptedToAcceptTheTermsAndConditions()
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

        //Terms and Conditions Page
        [When("prompted to accept the terms and conditions")]
        public void PromptedToAcceptTheTermsAndConditions()
        {
        }

        [Then("I should be directed to a dedicated page on the HOUS website in a new tab displaying the complete and updated terms and conditions of system usage")]
        public void IShouldBeDirectedToADedicatedPage()
        {
        }


        //Acceptance Confirmation
        [When("I return to the system after reviewing the terms and conditions")]
        public void IReturnToTheSystemAfterReviewingTheTermsAndConditions()
        {
        }

        [Then("there should be a clear option, such as a checkbox or button, allowing me to confirm my acceptance")]
        public void ThereShouldBeAClearOptionSuchAsAAheckboxOrButtonAllowingMeToConfirmMyAcceptance()
        {
        }

        //Date of Acceptance
        [When("accepting the terms and conditions")]
        public void AcceptingTheTermsAndConditions()
        {
        }

        [Then("the system should record and display the date and time of my acceptance")]
        public void TheSystemShouldRecordAndDisplayTheDateAndTimeofMyAcceptance()
        {
            var identity = _DssDBContext.DssUserIdentities.FirstOrDefault(p => p.EmailAddressDsc == _TestEmail);
            _DssDBContext.Entry<DssUserIdentity>(identity).Reload();

            // DateTime stamp should have been updated
            ClassicAssert.True(identity.TermsAcceptanceDtm != null);
        }

        //Access Restriction for Non-Acceptance
        [When("I attempt to access system features")]
        public void IAttemptToAccessSystemFeatures()
        {
        }

        [When("I haven't accepted the terms and conditions")]
        public void IHaventAcceptedTheTermsAndConditions()
        {
        }

        [Then("I should be restricted from performing certain actions until I complete the acceptance process")]
        public void IShouldBeRestrictedFromPerformingCertainActions()
        {
        }

        //Accessible Language
        [When("presenting the terms and conditions")]
        public void PresentingTheTermsAndConditions()
        {
        }

        [Then("the language should be clear, concise, and accessible to ensure user understanding")]
        public void TheLanguageShouldBeClear()
        {
        }

        //Confirmation Message:

        [When("I successfully accept the terms and conditions")]
        public void ISuccessfullyAcceptTheTermsAndConditions()
        {
        }

        [Then("I should receive a confirmation message indicating that my acceptance has been recorded")]
        public void IShouldReceiveAConfirmationMessage()
        {
        }


        [Then("I should be directed to the landing page for my role")]
        public void IShouldBeDirectedToTheLandingPageForMyRole()
        {

        }

        //Bypass Terms and Conditions for CEU users

        [Given("I am an IDIR authenticated user")]
        public void IAmAnIDIRAuthenticatedUser()
        {
        }

        [Then("TOC flag will be set to true")]
        public void TOCFlagWillBeSetToTrue()
        {
        }

        [Then("I will not have to accept the TOC in order to access the system")]
        public void IWillNotHaveToAcceptTheTOC()
        {
        }

        [TearDown]
        public void TestTearDown()
        {
            //restore original User Identity

            _UserIdentity = _OriginalUserIdentity;

            _DssDBContext.SaveChanges();
        }
    }
}
