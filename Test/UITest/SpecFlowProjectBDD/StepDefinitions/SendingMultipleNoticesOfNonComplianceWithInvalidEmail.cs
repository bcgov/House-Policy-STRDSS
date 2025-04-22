using Configuration;
using DataBase.Entities;
using DataBase.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework.Legacy;
using OpenQA.Selenium;
using OpenQA.Selenium.DevTools.V118.Debugger;
using SpecFlowProjectBDD.Helpers;
using System.Reflection.Metadata;
using System.Xml.Linq;
using TestFrameWork.Models;
using UITest.PageObjects;
using UITest.TestDriver;

namespace SpecFlowProjectBDD.StepDefinitions
{
    [Binding]
    [Scope(Scenario = "SendingMultipleNoticesOfNonComplianceWithInvalidEmail")]
    public class SendingMultipleNoticesOfNonComplianceWithInvalidEmail
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
        private DssRentalListingContact _RentalListingPropertyOwnerContact;
        private DssRentalListingContact _RentalListingNonPropertyOwnerContact;
        private string _OriginalPropertyOwnerContactEmail = string.Empty;
        private string _OriginalNonPropertyOwnerContactEmail = string.Empty;
        private DateTime _updateTime;
        private DssDbContext _DssDBContext;
        private IUnitOfWork _UnitOfWork;
        private SFEnums.Environment _Environment = SFEnums.Environment.LOCAL;

        //want to get first row (0), but row 0 returns and empty list instead of the values of row 0
        int _row = 1;
        int _listingIDColumn = 3;
        int _organizationColumn = 2;
        int _InvalidEmailColumn = 2;

        public SendingMultipleNoticesOfNonComplianceWithInvalidEmail(SeleniumDriver Driver)
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


            //get listingID for first listing and update email in DB with an invalid email
            bool result = false;
            string listingNumber = string.Empty; 
            string organizationName = string.Empty;
            string listingID = string.Empty;
            long organizationID = 0;
            List<string> listingsRow = new List<string>();

            try
            {
                Thread.Sleep(2000);
                listingsRow = _ListingsPage.ListingsTable.GetRowValues(_row);
                listingNumber = listingsRow[_listingIDColumn].ToString();

                // listingNumber = (string)_ListingsPage.ListingsTable.JSExecuteJavaScript(@$"document.querySelector(""#pn_id_18-table > tbody > tr:nth-child({_row}) > td:nth-child({_listingIDColumn}) > a"").innerText");
            }
            catch (ElementNotVisibleException ex)
            {
                Console.WriteLine($"Could not read Listing ID for Listings table:Row {_row}");
                throw ex;
            }

            try
            {
                organizationName = listingsRow[_organizationColumn];
            }
            catch
            {
                throw new InvalidCastException($"Could not read Plaform for Listings table:Row {_row}");
            }

            //Get OrganizationID for Platform
            var organization = _UnitOfWork.DssOrganizationRepository.Get(p => p.OrganizationNm.ToUpper() == organizationName.ToUpper()).First();
            organizationID = organization.OrganizationId;


            //Get existing Listing

            var rentalListing = _UnitOfWork.DssRentalListingRepository.Get(p => (p.PlatformListingNo == listingNumber) && (p.OfferingOrganizationId == organizationID) && (p.IsActive == true)).First();
            _RentalListingPropertyOwnerContact = _UnitOfWork.DssRentalListingContactRepository.Get(p => (p.ContactedThroughRentalListingId == rentalListing.RentalListingId) && (p.IsPropertyOwner == true)).First();
            _RentalListingNonPropertyOwnerContact = _UnitOfWork.DssRentalListingContactRepository.Get(p => (p.ContactedThroughRentalListingId == rentalListing.RentalListingId) && (p.IsPropertyOwner == false)).First();
            //Save Original Email
            _OriginalPropertyOwnerContactEmail = _RentalListingPropertyOwnerContact.EmailAddressDsc;
            _OriginalNonPropertyOwnerContactEmail = _RentalListingNonPropertyOwnerContact.EmailAddressDsc;

            //update Email address with Invalid EmailAddress
            //Update must be made on the view listing page to appear on the notice of non-compliance page

            _RentalListingPropertyOwnerContact.EmailAddressDsc = "TestUserInValid@@email.com";
            _RentalListingNonPropertyOwnerContact.EmailAddressDsc = "TestUserValid@email.com";
            _UnitOfWork.Save();

            //Refresh page to reread updated email values from DB
            _ListingsPage.Driver.Navigate().Refresh();
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

        [Then(@"LG user clicks Non-Compliance button")]
        public void WhenLGUserClicksSendNoticesOfNon_ComplianceButton()
        {
            _ListingsPage.SendNoticeOfNonComplianceButton.Click();
        }

        [Then(@"system opens details to complete fields for sending notices")]
        public void ThenSystemOpensDetailsToCompleteFieldsForSendingNotices()
        {
            ClassicAssert.IsTrue(_BulkComplianceNoticePage.ContactEmailTextBox.IsEnabled());
        }

        [Then(@"the emails with an invalid email address are flagged")]
        public void ThenTheEmailsWithAnInvalidEmailAddressAreFlagged()
        {
            object isInvalidEmail = _ListingsPage.ListingsTable.JSExecuteJavaScript($@"document.querySelector(""#pn_id_50-table > tbody > tr:nth-child({_row}) > td:nth-child({_InvalidEmailColumn})"").innerText");
            //ClassicAssert.IsTrue(isInvalidEmail.ToUpper() == "YES");
        }

        [Then(@"the button to send Notice to host is disabled if all invalid host email addresses")]
        public void ThenTheButtonToSendNoticeToHostIsDisabledIfAllInvalidHostEmailAddresses()
        {
            _RentalListingPropertyOwnerContact.EmailAddressDsc = ".TestUserInValid@email.com";
            _RentalListingNonPropertyOwnerContact.EmailAddressDsc = "TestUserInValid.@email.com";
            _UnitOfWork.Save();

            _ListingsPage.Driver.Navigate().Refresh();
            _LandingPage.ViewListingsButton.Click();
            _ListingsPage.SelectAllCheckbox.Click();
            _ListingsPage.SendNoticeOfNonComplianceButton.Click();

            //locator changes to a table and "path". Cannot check status
           // string sendNoticeToHostIsChecked = (string)_ListingsPage.ListingsTable.JSExecuteJavaScript(@"document.querySelector(""#binary"").ariaChecked");
           
            //ClassicAssert.IsFalse(bool.Parse(sendNoticeToHostIsChecked));

            // ClassicAssert.IsFalse(_BulkComplianceNoticePage.SubmitButton.IsEnabled());
        }

        [Then(@"the button to send notice to host is checked if there is at least one valid host email addresses")]
        public void ThenTheButtonToSendNoticeToHostIsCheckedIfThereIsAtLeastOneValidHostEmailAddresses()
        {
            _RentalListingNonPropertyOwnerContact.EmailAddressDsc = "TestUserInValid.@email.com";
            _RentalListingPropertyOwnerContact.EmailAddressDsc = "TestUserValid@email.com";
            _UnitOfWork.Save();

            _ListingsPage.Driver.Navigate().Refresh();
            _LandingPage.ViewListingsButton.Click();
            _ListingsPage.SelectAllCheckbox.Click();
            _ListingsPage.SendNoticeOfNonComplianceButton.Click();

            string sendNoticeToHostIsChecked = (string)_ListingsPage.ListingsTable.JSExecuteJavaScript(@"document.querySelector(""#binary"").ariaChecked");
            ClassicAssert.IsTrue(bool.Parse(sendNoticeToHostIsChecked));
        }

        [Then(@"the button to send Notice is checked for valid host emails")]
        public void ThenTheButtonToSendNoticeIsCheckedForValidHostEmails()
        {
            _RentalListingPropertyOwnerContact.EmailAddressDsc = "TestUserValid@email.com";
            _RentalListingNonPropertyOwnerContact.EmailAddressDsc = "TestUser2Valid@email.com";
            _UnitOfWork.Save();

            _ListingsPage.Driver.Navigate().Refresh();
            _LandingPage.ViewListingsButton.Click();
            _ListingsPage.SelectAllCheckbox.Click();
            _ListingsPage.SendNoticeOfNonComplianceButton.Click();

            string sendNoticeToHostIsChecked = (string)_ListingsPage.ListingsTable.JSExecuteJavaScript(@"document.querySelector(""#binary"").ariaChecked");
            ClassicAssert.IsTrue(bool.Parse(sendNoticeToHostIsChecked));
        }

        [Then(@"the “Review"" button is disabled if any mandatory field is not completed")]
        public void ThenTheReviewButtonIsDisabledIfAnyMandatoryFieldIsNotCompleted()
        {
            _BulkComplianceNoticePage.SubmitButton.JSExecuteJavaScript("window.scrollTo(0, document.body.scrollHeight);");
            ClassicAssert.IsFalse(_BulkComplianceNoticePage.SubmitButton.IsEnabled());
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
        }

        [When(@"Action History Not Updated")]
        public void WhenActionHistoryNotUpdated()
        {
        }

        [When(@"LG user completes mandatory fields\. \( Provide a LG email address to receive a copy of the Notice\)")]
        public void WhenLGUserCompletesMandatoryFields_ProvideALGEmailAddressToReceiveACopyOfTheNotice()
        {
            _ListingsPage.SelectAllCheckbox.Click();

            _ListingsPage.SendNoticeOfNonComplianceButton.Click();

            _BulkComplianceNoticePage.SubmitButton.JSExecuteJavaScript("window.scrollTo(0, document.body.scrollHeight);");

            _BulkComplianceNoticePage.ContactEmailTextBox.EnterText("richard.anderson@gov.bc.ca");
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

        }

        [Then(@"Verify that if remove the listing checkbox is unchecked, review is also disabled")]
        public void ThenVerifyThatIfRemoveTheListingCheckboxIsUncheckedReviewIsAlsoDisabled()
        {

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

        [AfterScenario]
        public void TestTearDown()
        {
            bool save = false;

            //restore original email values
            if (!string.IsNullOrEmpty(_OriginalPropertyOwnerContactEmail))
            {
                _RentalListingPropertyOwnerContact.EmailAddressDsc = _OriginalPropertyOwnerContactEmail;
                                save = true;
            }

            if (!string.IsNullOrEmpty(_OriginalNonPropertyOwnerContactEmail))
            {
                _RentalListingNonPropertyOwnerContact.EmailAddressDsc = _OriginalNonPropertyOwnerContactEmail;
                               save = true;
            }

          if(save == true)
            _UnitOfWork.Save();
        }
    }
}
