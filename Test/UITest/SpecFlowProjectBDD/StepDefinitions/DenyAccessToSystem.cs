using Configuration;
using DataBase.Entities;
using DataBase.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using SpecFlowProjectBDD.Helpers;
using TestFrameWork.Models;
using UITest.PageObjects;
using UITest.TestDriver;

namespace SpecFlowProjectBDD.StepDefinitions
{
    [Binding]
    [Scope(Scenario = "DenyAccessToSystem")]
    public sealed class DenyAccessToSystem
    {
        private IDriver _Driver;
        private LayoutPage _LayoutPage;
        private DelistingWarningPage _DelistingWarningPage;
        private PathFinderPage _PathFinderPage;
        private IDirLoginPage _IDirPage;
        private NoticeOfTakeDownPage _NoticeOfTakeDownPage;
        private string _TestUserName;
        private string _TestPassword;
        private string _TestEmail;
        private bool _ExpectedResult = false;
        private SFEnums.UserTypeEnum _UserType;
        private DssDbContext _DssDBContext;
        private DssUserIdentity _UserIdentity;
        private bool _OriginalEnabledValue;
        AppSettings _AppSettings;
        private IUnitOfWork _UnitOfWork;
        private SFEnums.Environment _Environment = SFEnums.Environment.LOCAL;


        public DenyAccessToSystem(SeleniumDriver Driver)
        {
            _Driver = Driver;
            _LayoutPage = new LayoutPage(_Driver);
            _PathFinderPage = new PathFinderPage(_Driver);
            _IDirPage = new IDirLoginPage(_Driver);

            _AppSettings = new AppSettings();

            DbContextOptions<DssDbContext> dbContextOptions = new DbContextOptions<DssDbContext>();

            string dbConnectionString = _AppSettings.GetConnectionString(_Environment.ToString().ToLower()) ?? string.Empty;

            _DssDBContext = new DssDbContext(dbContextOptions, dbConnectionString);
            _UnitOfWork = new UnitOfWork(_DssDBContext);
        }

        //User Authentication
        [Given(@"that I am an authenticated LG, CEU, Provincial Gov or Platform user and the expected result is ""(.*)""")]
        public void GivenIAmAauthenticatedGovernmentUser(string ExpectedResult)
        {
            _ExpectedResult = ExpectedResult.ToUpper() == "PASS" ? true : false;

            _Driver.Url = _AppSettings.GetServer("default");
            _Driver.Navigate();
        }

        //[When(@"I attempt to access the Data Sharing System as ""(.*)"" with email ""(.*)"" and Role ""(.*)""")]
        [When(@"I attempt to access the Data Sharing System as ""(.*)"" with email ""(.*)"" and Role ""(.*)""")]
        public void IAttemptToAccessTheDataSharingSystem(string UserName, string Email, string RoleName)
        {
            if (string.IsNullOrWhiteSpace(UserName))
            {
                throw new ArgumentException("UserName cannot be empty");
            }

            if ((string.IsNullOrWhiteSpace(Email)))
            {
                throw new ArgumentException("Email cannot be empty");
            }

            if ((string.IsNullOrWhiteSpace(RoleName)))
            {
                throw new ArgumentException("Rolename cannot be empty");
            }

            _TestUserName = UserName;
            _TestPassword = _AppSettings.GetUser(_TestUserName) ?? string.Empty;
            _TestEmail = Email;
            //////////////////// DB Setup ////////////////////////////////////////
            // Retrieve the user identity
            _UserIdentity = _DssDBContext.DssUserIdentities.FirstOrDefault(p => p.EmailAddressDsc == _TestEmail);
            _OriginalEnabledValue = _UserIdentity.IsEnabled;

            // Update properties of the identity
            _UserIdentity.IsEnabled = false;

            _DssDBContext.SaveChanges();
            /////////////////////////////////////////////////////////////
            
            UserHelper userHelper = new UserHelper();

            _UserType = userHelper.SetUserType(RoleName);

            AuthHelper authHelper = new AuthHelper(_Driver);

            //Authenticate user using IDir or BCID depending on the user
            authHelper.Authenticate(_TestUserName, _TestPassword, _UserType);

        }

        [Then("I dont have the required access permissions")]
        public void IDontHaveTheRequiredAccessPermissions()
        {
        }


        [Then("I should see a specific message indicating that access is restricted")]
        public void IShouldSeeASpecificMessageIndicatingThatAccessIsRestricted()
        {
            System.Threading.Thread.Sleep(1000);
            ClassicAssert.IsTrue(_LayoutPage.Driver.PageSource.Contains("401 Access Denied"));
        }

        [AfterScenario]
        public void TestTearDown()
        {
            //restore original User value

            _UserIdentity.IsEnabled = _OriginalEnabledValue;

            _DssDBContext.SaveChanges();
        }
    }
}
