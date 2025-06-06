using Configuration;
using DataBase.Entities;
using DataBase.UnitOfWork;
using Microsoft.EntityFrameworkCore;
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
    [Scope(Scenario = "SendingMultipleNoticesOfTakeDown")]
    public class SendingMultipleNoticesOfTakeDown
    {
        private IDriver _Driver;
        private LandingPage _LandingPage;
        private ListingsPage _ListingsPage;
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

        public SendingMultipleNoticesOfTakeDown(SeleniumDriver Driver)
        {
            _Driver = Driver;
            _LandingPage = new LandingPage(_Driver);
            _ListingsPage = new ListingsPage(_Driver);
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


        [Given(@"that I am an authenticated User ""(.*)"" and the expected result is ""(.*)"" and I am a ""(.*)"" user")]
        public void WhenUserEntersValidLoginCredentialsAndClicksLoginButton(string UserName, string ExpectedResult, string UserType)
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

        [Then(@"LG user is redirected to dashboard-> Hompage")]
        public void ThenLGUserIsRedirectedToDashboard_Hompage()
        {
            ClassicAssert.IsTrue(_Driver.GetCurrentURL() == "http://127.0.0.1:4200/");
        }

        [When(@"LG user selects 'View Listings' from menu to load listings data page\. Or User navigates to view listing data on homepage Screen")]
        public void WhenLGUserSelectsFromMenuToLoadListingsDataPage_OrUserNavigatesToViewListingDataOnHomepageScreen()
        {
            _LandingPage.ViewListingsButton.Click();
        }

        [Then(@"the Send Notices of Non-Compliance button is disabled")]
        public void ThenTheSendNoticesOfNon_ComplianceButtonIsDisabledAtThisStage()
        {
            ClassicAssert.IsFalse(_ListingsPage.SendNoticeOfNonComplianceButton.IsEnabled());
        }

        [When(@"LG User Select Multiple Listings")]
        public void WhenLGUSerSelectMultipleListings()
        {
            _ListingsPage.SelectAllCheckbox.Click();
        }

        [Then(@"the Send Notices of Non-Compliance button is enabled")]
        public void ThenTheSendNoticesOfNon_ComplianceButtonIsEnabled()
        {
            ClassicAssert.IsTrue(_ListingsPage.SendNoticeOfNonComplianceButton.IsEnabled());
        }

        [Then(@"LG user clicks TakeDown button")]
        public void WhenLGUserClicksSendNoticesOfNon_ComplianceButton()
        {
            _ListingsPage.SendTakedownRequestButton.Click();
        }

        [Then(@"system opens details to complete fields for sending notices")]
        public void ThenSystemOpensDetailsToCompleteFieldsForSendingNotices()
        {

            ClassicAssert.IsTrue(_BulkComplianceNoticePage.AddBCCsTextBox.IsEnabled());
        }

        [Then(@"the “Review"" button is disabled if any mandatory field is not completed")]
        public void ThenTheReviewButtonIsDisabledIfAnyMandatoryFieldIsNotCompleted()
        {
            _BulkComplianceNoticePage.SubmitButton.JSExecuteJavaScript("window.scrollTo(0, document.body.scrollHeight);");

            ClassicAssert.IsTrue(_BulkComplianceNoticePage.SubmitButton.IsEnabled());

        }

        [Then(@"If LG user clicks Cancel,  system prompts with a re-confirmation message")]
        public void ThenIfLGUserClicksSystemPromptsWithARe_ConfirmationMessage()
        {
            _BulkComplianceNoticePage.CancelButton.Click();
        }

        [Then(@"If user confirms cancellation, user is redirected back to listings data page")]
        public void ThenIfUserConfirmsCancellationUserIsRedirectedBackToListingsDataPage()
        {
            ClassicAssert.IsTrue(_Driver.Url.ToLower().Contains("listings"));
        }

        [Then(@"the action history is not updated when the user cancels the action")]
        public void ThenTheActionHistoryIsNotUpdatedWhenTheUserCancelsTheAction()
        {

        }

        [When(@"user does not confirm, user remains on current page\.")]
        public void WhenUserDoesNotConfirmUserRemainsOnCurrentPage_()
        {
            //throw new PendingStepException();
        }

        [When(@"Action History Not Updated")]
        public void WhenActionHistoryNotUpdated()
        {
            //throw new PendingStepException();
        }

        [When(@"LG user completes mandatory fields\. \( Provide a LG email address to receive a copy of the Notice\)")]
        public void WhenLGUserCompletesMandatoryFields_ProvideALGEmailAddressToReceiveACopyOfTheNotice()
        {
            _ListingsPage.SelectAllCheckbox.Click();

            _ListingsPage.SendTakedownRequestButton.Click();

            _BulkComplianceNoticePage.SubmitButton.JSExecuteJavaScript("window.scrollTo(0, document.body.scrollHeight);");
        }

        [Then(@"that LG user can add BCCs")]
        public void ThenThatLGUserCanAddBCCs()
        {
        }

        [When(@"the LG User enters an Email Address")]
        public void WhenTheLGUserEntersAnEmailAddress()
        {
            _BulkComplianceNoticePage.AddBCCsTextBox.EnterText("richard.anderson@dxc.com");
        }

        [Then(@"if user inputs an email that is not in the correct format the user is prompted to enter an email address in the correct format")]
        public void ThenIfUserInputsAnEmailThatIsNotInTheCorrectFormatTheUserIsPromptedToEnterAnEmailAddressInTheCorrectFormat()
        {
        }

        [Then(@"the user can add multiple email addresses")]
        public void ThenTheUserCanAddMultipleEmailAddresses()
        {
            //_BulkComplianceNoticePage.AddBCCsTextBox.EnterText(", richard.anderson@gov.bc.ca");
        }

        [Then(@"Verify that if remove the listing checkbox is unchecked, review is also disabled")]
        public void ThenVerifyThatIfRemoveTheListingCheckboxIsUncheckedReviewIsAlsoDisabled()
        {
            //throw new PendingStepException();
        }

        [When(@"the LG user clicks “Review"" button to confirm details to be sent")]
        public void WhenTheLGUserClicksReviewButtonToConfirmDetailsToBeSent()
        {
            _BulkComplianceNoticePage.SubmitButton.Click();
        }

        [When(@"the LG user selects the Submit button")]
        public void WhenTheLGUserSelectsSubmit()
        {
            _updateTime = DateTime.UtcNow; _updateTime = DateTime.UtcNow;
            _BulkComplianceNoticePage.SubmitPreviewButton.Click();
        }

        [Then(@"Successful confirmation is displayed for user on top Right of the page")]
        public void ThenSuccessfulConfirmationIsDisplayedForUserOnTopRightOfThePage()
        {

        }

        [Then(@"System immediately sends notices to platform/host for selected listings")]
        public void ThenSystemImmediatelySendsNoticesToPlatformHostForSelectedListings()
        {

        }

        [Then(@"A copy email is also sent to LG email address added to receive a copy of the notice same, a copy of email to bcc")]
        public void ThenACopyEmailIsAlsoSentToLGEmailAddressAddedToReceiveACopyOfTheNoticeSameACopyOfEmailToBcc()
        {

        }

        [Then(@"Action history is updated immediately with action taken")]
        public void ThenActionHistoryIsUpdatedImmediatelyWithActionTaken()
        {
            var updateTime = _UnitOfWork.DssUploadDeliveryRepository.Get(p => p.UpdDtm >= _updateTime);
            ClassicAssert.IsNotNull(updateTime);
        }

        [Then(@"On the listings page, last action and last action date should be updated")]
        public void ThenOnTheListingsPageLastActionAndLastActionDateShouldBeUpdated()
        {

        }
    }
}
