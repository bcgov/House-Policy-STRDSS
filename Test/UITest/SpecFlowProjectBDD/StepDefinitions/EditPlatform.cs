using Configuration;
using NUnit.Framework.Legacy;
using SpecFlowProjectBDD.Helpers;
using SpecFlowProjectBDD.Utilities;
using System;
using TechTalk.SpecFlow;
using TestFrameWork.Models;
using UITest.PageObjects;
using UITest.TestDriver;
using static SpecFlowProjectBDD.SFEnums;

namespace SpecFlowProjectBDD.StepDefinitions
{
    [Binding]
    [Scope(Scenario = "EditPlatform")]
    public class EditPlatform
    {
        private IDriver _Driver;
        private LandingPage _LandingPage;
        private ManagePlatformsPage _ManagePlatformsPage;
        private DetailedPlatformContactInformationPage _DetailedPlatformContactInformationPage;
        private AddSubPlatformPage _AddSubPlatformPage;
        private PathFinderPage _PathFinderPage;
        private IDirLoginPage _IDRLoginPage;
        private EditPlatformPage _EditPlatformPage;

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

        public EditPlatform(SeleniumDriver Driver)
        {
            _Driver = Driver;
            _LandingPage = new LandingPage(_Driver);
            _ManagePlatformsPage = new ManagePlatformsPage(_Driver);
            _DetailedPlatformContactInformationPage = new DetailedPlatformContactInformationPage(_Driver);
            _AddSubPlatformPage = new AddSubPlatformPage(_Driver);
            _PathFinderPage = new PathFinderPage(_Driver);
            _IDRLoginPage = new IDirLoginPage(_Driver);
            _BCIDPage = new BCIDPage(_Driver);
            _EditPlatformPage = new EditPlatformPage(Driver);

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

        [When(@"I click on the Manage Platforms button")]
        public void WhenIClickOnTheManagePlatformsButton()
        {
            Thread.Sleep(3000);
            _LandingPage.ManagePlatformsButton.Click();
        }

        [Then(@"I should be presented with a list of platforms and sub-platforms")]
        public void ThenIShouldBePresentedWithAListOfPlatformsAndSub_Platforms()
        {
            ClassicAssert.IsTrue(_Driver.GetCurrentURL().Contains("platform-management"));
        }

        [When(@"I select an existing platform")]
        public void WhenISelectAnExistingPlatform()
        {
            _ManagePlatformsPage.EditPlatformButton.Click();
        }

        [Then(@"I should be be able to edit platform information,")]
        public void ThenIShouldBeBeAbleToEditPlatformInformation()
        {
            _DetailedPlatformContactInformationPage.UpdateParentPlatformInformationButton.Click();
        }

        [Then(@"I should see a call to action to disable the platform")]
        public void ThenIShouldSeeACallToActionToDisableThePlatform()
        {

            //ClassicAssert.IsTrue(_EditPlatformPage.DisablePlatformButton.IsEnabled());
        }

        [When(@"I edit platform name information")]
        public void WhenIEditPlatformNameInformation()
        {
            _EditPlatformPage.PlatformNameTextBox.EnterText(_PlatformName);
        }

        [Then(@"platform information should update across the platform \(e\.g\., listing view, detailed view, and drop down platform select menus, etc\.\)")]
        public void ThenPlatformInformationShouldUpdateAcrossThePlatformE_G_ListingViewDetailedViewAndDropDownPlatformSelectMenusEtc_()
        {

        }

        [When(@"I edit platform email addresses")]
        public void WhenIEditPlatformEmailAddresses()
        {
            _EditPlatformPage.EmailForNonComplianceNoticesTextBox.ClearText();
            _EditPlatformPage.EmailForNonComplianceNoticesTextBox.EnterText("Foo@foo.com");

            _EditPlatformPage.EmailForTakedownRequestLettersTextBox.ClearText();
            _EditPlatformPage.EmailForTakedownRequestLettersTextBox.EnterText("Foo@foo.com");

            _EditPlatformPage.SecondaryEmailForNonComplianceNoticesTextBox.ClearText();
            _EditPlatformPage.SecondaryEmailForNonComplianceNoticesTextBox.EnterText("Foo@foo.com");

            _EditPlatformPage.SecondaryEmailForTakedownRequest.ClearText();
            _EditPlatformPage.SecondaryEmailForTakedownRequest.EnterText("Foo@foo.com");
            _EditPlatformPage.PlatformTypeDropDown.JSExecuteJavaScript(@"document.querySelector(""#platformType > div"").click()");
            _EditPlatformPage.PlatformTypeDropDown.JSExecuteJavaScript(@"document.querySelector(""#platformType_0"").click()");
        }

        [Then(@"emails should go to the updated platform contacts for each type of email \(Notice, takedown,\)")]
        public void ThenEmailsShouldGoToTheUpdatedPlatformContactsForEachTypeOfEmailNoticeTakedown()
        {

        }

        [When(@"I update parent or subsidiary information or platform code")]
        public void WhenIUpdateParentOrSubsidiaryInformationOrPlatformCode()
        {
            //Not Enabled for sub-platform
            //_EditPlatformPage.PlatformCodeTextBox.EnterText(_PlatformCode);
        }

        [Then(@"the platform should be able to upload monthly data reports or takedown reports for all platforms associated with it \(ie\. parent or subsidiary platforms\)")]
        public void ThenThePlatformShouldBeAbleToUploadMonthlyDataReportsOrTakedownReportsForAllPlatformsAssociatedWithItIe_ParentOrSubsidiaryPlatforms()
        {

        }

        [Then(@"the platform uploads should validate against the updated platform code")]
        public void ThenThePlatformUploadsShouldValidateAgainstTheUpdatedPlatformCode()
        {

        }

        [When(@"submitting platform details")]
        public void WhenSubmittingPlatformDetails()
        {

        }

        [Then(@"the system should perform validation checks and provide clear error messages for any input errors")]
        public void ThenTheSystemShouldPerformValidationChecksAndProvideClearErrorMessagesForAnyInputErrors()
        {

        }

        [When(@"I click the Save button")]
        public void WhenIClickTheSaveButton()
        {
            _EditPlatformPage.SaveButton.Click();
        }

        [Then(@"the sub platform should be created")]
        public void ThenTheSubPlatformShouldBeCreated()
        {

        }

        [Then(@"the sub platform should be a child of the parent platform")]
        public void ThenTheSubPlatformShouldBeAChildOfTheParentPlatform()
        {

        }
    }
}
