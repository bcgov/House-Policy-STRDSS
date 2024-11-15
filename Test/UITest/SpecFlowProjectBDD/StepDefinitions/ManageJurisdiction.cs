using Configuration;
using NUnit.Framework.Legacy;
using SpecFlowProjectBDD.Helpers;
using SpecFlowProjectBDD.Utilities;
using System;
using System.Xml.Linq;
using TechTalk.SpecFlow;
using TestFrameWork.Models;
using UITest.PageObjects;
using UITest.TestDriver;

namespace SpecFlowProjectBDD.StepDefinitions
{
    [Binding]
    [Scope(Scenario = "ManageJurisdiction")]
    public class ManageJurisdiction
    {
        private IDriver _Driver;
        private LandingPage _LandingPage;
        private ManageJurisdictionPage _ManageJurisdictionPage;
        private UpdateJurisdictionInformationPage _UpdateJurisdictionInformationPage;
        private PathFinderPage _PathFinderPage;
        private IDirLoginPage _IDRLoginPage;
        private BCIDPage _BCIDPage;

        private string _TestUserName;
        private string _TestPassword;
        private bool _ExpectedResult = false;

        private StrUtilities _strUtilities;

        private AppSettings _AppSettings;
        private SFEnums.UserTypeEnum _UserType;

        private SFEnums.LogonTypeEnum? _LogonType;

        public ManageJurisdiction(SeleniumDriver Driver)
        {
            _Driver = Driver;
            _LandingPage = new LandingPage(_Driver);
            _ManageJurisdictionPage = new ManageJurisdictionPage(_Driver);
            _UpdateJurisdictionInformationPage = new UpdateJurisdictionInformationPage(_Driver);
            _PathFinderPage = new PathFinderPage(_Driver);
            _IDRLoginPage = new IDirLoginPage(_Driver);
            _BCIDPage = new BCIDPage(_Driver);

            _AppSettings = new AppSettings();
            _strUtilities = new StrUtilities();
        }
        [Given(@"I am an authenticated CEU staff member ""([^""]*)""  with the appropriate permissions \(ADD\) and the expected result is ""([^""]*)"" and I am a ""([^""]*)"" user")]
        public void GivenIAmAnAuthenticatedCEUStaffMemberWithTheAppropriatePermissionsADDAndTheExpectedResultIsAndIAmAUser(string UserName, string ExpectedResult, string UserType)
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

        [When(@"I log in and navigate to the Manage Jurisdictions feature")]
        public void WhenILogInAndNavigateToTheManageJurisdictionsFeature()
        {
            _LandingPage.ManageJurisdictionsButton.Click();
        }

        [Then(@"I should be presented with a list of platforms with a list of local government jurisdictions")]
        public void ThenIShouldBePresentedWithAListOfPlatformsWithAListOfLocalGovernmentJurisdictions()
        {
            ClassicAssert.IsTrue(_ManageJurisdictionPage.LGListingsTable.Exists());
            List<string> headerRow = _ManageJurisdictionPage.LGListingsTable.GetHeaderRow();
            ClassicAssert.IsTrue(headerRow[1] == "Local Government Name");
            ClassicAssert.IsTrue(headerRow[2] == "Local Government Type");
            ClassicAssert.IsTrue(headerRow[3] == "Local Government Code");
            ClassicAssert.IsTrue(headerRow[4] == "BL Format");
            ClassicAssert.IsTrue(headerRow[5] == "Update Local Government Information");
        }

        [When(@"I view the list of jurisdictions")]
        public void WhenIViewTheListOfJurisdictions()
        {
            _ManageJurisdictionPage.ExpandJurisdictionsButton.Click();
        }

        [Then(@"I should see key information about each one")]
        public void ThenIShouldSeeKeyInformationAboutEachOne()
        {
            List<string> headerRow = _ManageJurisdictionPage.JurisdictionsListingsTable.GetHeaderRow();
            ClassicAssert.IsTrue(headerRow[0] == "Jurisdiction Name");
            ClassicAssert.IsTrue(headerRow[1] == "Shape File ID");
            ClassicAssert.IsTrue(headerRow[2] == "Principle Residence Requirement?");
            ClassicAssert.IsTrue(headerRow[3] == "STR Prohibited?");
            ClassicAssert.IsTrue(headerRow[4] == "BL Requirement?");
            ClassicAssert.IsTrue(headerRow[5] == "Update Jurisdiction Info");

        }

        [Then(@"I should have the ability to edit key information about each one \(as above\)")]
        public void ThenIShouldHaveTheAbilityToEditKeyInformationAboutEachOneAsAbove()
        {
            _ManageJurisdictionPage.EditJurisdictionButton.Click();
            //_UpdateJurisdictionInformationPage.JurisdictionNameTextBox.EnterText("Jurisdiction" + _strUtilities.GenerateRandomString(15));
            _UpdateJurisdictionInformationPage.PrincipleResidenceRequirementTrueButton.Click();
            _UpdateJurisdictionInformationPage.PrincipleResidenceRequirementFalseButton.Click();

        }
    }
}
