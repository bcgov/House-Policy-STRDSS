using Configuration;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Utilities;
using NUnit.Framework.Legacy;
using SpecFlowProjectBDD.Helpers;
using System;
using TechTalk.SpecFlow;
using TestFrameWork.Models;
using UITest.PageObjects;
using UITest.TestDriver;
using static SpecFlowProjectBDD.SFEnums;
using SpecFlowProjectBDD.Utilities;

namespace SpecFlowProjectBDD.StepDefinitions
{
    [Binding]
    [Scope(Scenario = "AddPlatform")]
    public class AddPlatform
    {
        private IDriver _Driver;
        private LandingPage _LandingPage;
        private ManagePlatformsPage _ManagePlatformsPage;
        private DetailedPlatformContactInformationPage _DetailedPlatformContactInformationPage;
        private AddPlatformPage _AddPlatformPage;
        private PathFinderPage _PathFinderPage;
        private IDirLoginPage _IDRLoginPage;

        private string _TestUserName;
        private string _TestPassword;
        private bool _ExpectedResult = false;
        private string _PlatformName = string.Empty;
        private string _PlatformCode = string.Empty;
        private string _EmailForNonComplianceNotices = string.Empty;
        private string _EmailForTakedownRequestLetters = string.Empty;
        private string _SecondaryEmailForNonComplianceNotices = string.Empty;
        private string _SecondaryEmailForTakedownRequest = string.Empty;

        private StrUtilities _strUtilities;

        private AppSettings _AppSettings;
        private SFEnums.UserTypeEnum _UserType;
        private BCIDPage _BCIDPage;
        private SFEnums.LogonTypeEnum? _LogonType;

        public AddPlatform(SeleniumDriver Driver)
        {
            _Driver = Driver;
            _LandingPage = new LandingPage(_Driver);
            _ManagePlatformsPage = new ManagePlatformsPage(_Driver);
            _DetailedPlatformContactInformationPage = new DetailedPlatformContactInformationPage(_Driver);
            _AddPlatformPage = new AddPlatformPage(_Driver);
            _PathFinderPage = new PathFinderPage(_Driver);
            _IDRLoginPage = new IDirLoginPage(_Driver);
            _BCIDPage = new BCIDPage(_Driver);
            _AppSettings = new AppSettings();
            _strUtilities = new StrUtilities();
            _PlatformName = "Sub-PlatForm" + _strUtilities.GenerateRandomString(50);
            _PlatformCode = "PC" + _strUtilities.GenerateRandomString(13);
            _EmailForNonComplianceNotices = "Foo@foo.com";
            _EmailForTakedownRequestLetters = "Foo@foo.com";
            _SecondaryEmailForNonComplianceNotices = "Foo@foo.com";
            _SecondaryEmailForTakedownRequest = "Foo@foo.com";
        }

        [Given(@"that I am an authenticated User ""([^""]*)"" and the expected result is ""([^""]*)"" and I am a ""([^""]*)"" user")]
        public void GivenThatIAmAnAuthenticatedUserAndTheExpectedResultIsAndIAmAUser(string UserName, string ExpectedResult, string UserType)
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

        [Then(@"I am directed to the Landing Page")]
        public void ThenIAmDirectedToTheLandingPage()
        {
            if (_UserType == UserTypeEnum.LOCALGOVERNMENT)
            {
                ClassicAssert.True(_LandingPage.ManagePlatformsButton.IsEnabled());
            }
        }

        [When(@"I login and navigate to the Manage Platforms feature")]
        public void WhenILoginAndNavigateToTheManagePlatformsFeature()
        {
            Thread.Sleep(3000);
            _LandingPage.ManagePlatformsButton.Click();
        }

        [Then(@"I should be presented with a list of platforms and sub-platforms")]
        public void ThenIShouldBePresentedWithAListOfPlatformsAndSub_Platforms()
        {
            ClassicAssert.IsTrue(_Driver.GetCurrentURL().Contains("platform-management"));
        }

        [When(@"I click the add new platform button for a platform")]
        public void WhenIClickTheAddNewPlatformButtonForAPlatform()
        {
            _ManagePlatformsPage.AddNewParentPlatformButton.Click();
        }

        [Then(@"I should be presented with the Add Platform page")]
        public void ThenIShouldBePresentedWithTheAddPlatformPage()
        {

        }

        [Then(@"I should see a form with the required input fields for creating a sub-platform")]
        public void ThenIShouldSeeAFormWithTheRequiredInputFieldsForCreatingASub_Platform()
        {

        }

        [When(@"I fill in valid values for the input fields")]
        public void WhenIFillInValidValuesForTheInputFields()
        {
            _AddPlatformPage.PlatformNameTextBox.EnterText(_PlatformName);
            _AddPlatformPage.PlatformCodeTextBox.EnterText(_PlatformCode);
            _AddPlatformPage.EmailForNonComplianceNoticesTextBox.EnterText(_EmailForNonComplianceNotices);
            _AddPlatformPage.EmailForTakedownRequestLettersTextBox.EnterText(_EmailForTakedownRequestLetters);
            _AddPlatformPage.SecondaryEmailForNonComplianceNoticesTextBox.EnterText(_SecondaryEmailForNonComplianceNotices);
            _AddPlatformPage.SecondaryEmailForTakedownRequest.EnterText(_SecondaryEmailForTakedownRequest);
            _AddPlatformPage.PlatformTypeDropDown.JSExecuteJavaScript(@"document.querySelector(""#platformType > div"").click()");
            _AddPlatformPage.PlatformTypeDropDown.JSExecuteJavaScript(@"document.querySelector(""#platformType_0"").click()");
            //_AddPlatformPage.PlatformTypeDropdown.SelectFirstListItem();
        }

        [Then(@"the Save button should be enabled")]
        public void ThenTheSaveButtonShouldBeEnabled()
        {
            //ClassicAssert.IsTrue(_AddPlatformPage.SaveButton.IsEnabled());
        }

        [When(@"I click the Save button")]
        public void WhenIClickTheSaveButton()
        {
            _AddPlatformPage.PlatformTypeDropDown.JSExecuteJavaScript(@"document.querySelector(""body > app-root > app-layout > div.content > app-add-new-platform > div.actions.ng-star-inserted > button:nth-child(1)"").click()");
            //_AddPlatformPage.SaveButton.Click();
        }

        [Then(@"the sub platform should be created")]
        public void ThenThePlatformShouldBeCreated()
        {
            Thread.Sleep(2000);
            ClassicAssert.IsTrue(_Driver.GetCurrentURL().Contains("/platform"));
        }

        [Then(@"the sub platform should be a child of the parent platform")]
        public void ThenThePlatformShouldBeAChildOfTheParentPlatform()
        {
        }
    }
}
