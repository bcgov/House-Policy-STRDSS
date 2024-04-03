using NUnit.Framework;
using UITest.PageObjects;
using UITest.TestDriver;
using TestFrameWork.Models;
using Configuration;
using NUnit.Framework.Legacy;
using FluentAssertions.Equivalency;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.Cryptography.X509Certificates;

namespace SpecFlowProjectBDD.StepDefinitions
{
    [Binding]
    [Scope(Scenario = "SendNoticeOfTakedownWithoutADSSlisting")]
    public sealed class SendNoticeOfTakedownWithoutADSSlisting
    {
        HomePage _HomePage;

        IDriver _Driver;

        DelistingWarningPage _DelistingWarningPage;
        PathFinderPage _PathFinderPage;
        IDRLoginPage _IDRLoginPage;
        NoticeOfTakeDownPage _NoticeOfTakeDownPage;

        string _TestUserName;
        string _TestPassword;


        public SendNoticeOfTakedownWithoutADSSlisting(SeleniumDriver Driver)
        {
            _Driver = Driver;
            _HomePage = new HomePage(_Driver);
            _DelistingWarningPage = new DelistingWarningPage(_Driver);
            _NoticeOfTakeDownPage = new NoticeOfTakeDownPage(_Driver);
            _PathFinderPage = new PathFinderPage(_Driver);
            _IDRLoginPage = new IDRLoginPage(_Driver);

            AppSettings appSettings = new AppSettings();
            _TestUserName = appSettings.GetValue("TestUserName") ?? string.Empty;
            _TestPassword = appSettings.GetValue("TestPassword") ?? string.Empty;
        }

        //User Authentication
        [Given("I am an authenticated LG staff member")]
        public void GivenIAmAauthenticatedLGStaffMemberUser()
        {
            _Driver.Url = "http://127.0.0.1:4200/compliance-notice";
            _Driver.Navigate();

            _PathFinderPage.IDRButton.Click();

            _IDRLoginPage.UserNameTextBox.EnterText(_TestUserName);
            _IDRLoginPage.PasswordTextBox.EnterText(_TestPassword);

            _IDRLoginPage.ContinueButton.Click();
        }


        [When("I navigate to the delisting warning feature")]
        public void WhenINavigateToTheDelistingWarningFeature()
        {
            //_Driver.Url = "http://127.0.0.1:4200/compliance-notice";
            //_Driver.Navigate();
        }

        //Input Form
        [Then("I should be presented with an input form that includes fields for the listing URL, and an optional field for the host email address")]
        public void IshouldBePresentedWithAnInputFormThatIncludesFields()
        {
        }

        [Then("I should be presented with a field to select which platform to send the warning to")]
        public void IShouldBePresentedWithAFieldToSelectWhichPlatformToSendTheWarningTo()
        {
            _DelistingWarningPage.PlatformReceipientDropdown.Click();
            _DelistingWarningPage.PlatformReceipientDropdown.Click();
        }

        [Then("I should be presented with a dropdown menu to select reason for delisting")]
        public void IShouldBePresentedWithADropdownMenuToSelectReasonForDelisting()
        {
            _DelistingWarningPage.ReasonDropdown.Click();
            _DelistingWarningPage.ReasonDropdown.Click();
        }

        [Then(@"I should see an optional field for Listing ID ""(.*)""")]
        public void IIhouldDeeAnOptionalFieldForListingID(string ListingID)
        {
            //add listing ID
            _DelistingWarningPage.ListingIDNumberTextBox.EnterText(ListingID);
        }

        [Then("I should see an optional field for adding a LG staff user email address to be copied on the email")]
        public void IShouldSeeAnOptionalFieldForAddingALGStaffUserEmailAddressToBeCopiedOnTheEmail()
        {
        }

        //ListingURLField
        [When(@"Entering the listing URL ""(.*)""")]
        public void WhenEnteringTheListingURL(string URL)
        {
            _DelistingWarningPage.ListingUrlTextBox.EnterText(URL);
        }

        [Then("The system should validate the URL format and ensure it is a valid link to the property listing")]
        public void TheSystemShouldValidateTheURLFormat()
        {
        }

        //PlatformField
        [When("selecting the platform")]
        public void WhenSelectingThePlatform()
        {
            _DelistingWarningPage.PlatformReceipientDropdown.Click();

        }

        //HostEmailAddressField
        [When("entering the optional host email address")]
        public void WhenEnteringTheOptionalHostEmailAddress()
        {
            _DelistingWarningPage.HostEmailAddressTextBox.EnterText("host@foo.com");
        }

        [Then("the system should present a list of available platform options to populate the field")]
        public void TheSystemShouldPresentAListOfAvailablePlatformOption()
        {
            _DelistingWarningPage.PlatformReceipientDropdown.WaitFor();
            _DelistingWarningPage.PlatformReceipientDropdown.ExecuteJavaScript(@"document.querySelector(""#platformId_0"").click()");
            ClassicAssert.IsTrue(_DelistingWarningPage.PlatformReceipientDropdown.Text.ToUpper().Contains("AIRBNB"));
        }

        [Then("the system should validate the email format")]
        public void ThenTheSystemShouldValidateTheEmailFormat()
        {
        }

        //ReasonForDelisting
        [When("I select a reason for delisting")]
        public void WhenISelectAReasonForDelisting()
        {
            _DelistingWarningPage.ReasonDropdown.Click();
            _DelistingWarningPage.ReasonDropdown.ExecuteJavaScript(@"document.querySelector(""#reasonId_0"").click()");
        }

        [Then("the system should present a list of reasons for requesting delisting: No business licence provided, invalid business licence number, expired business licence, or suspended business license")]
        public void ThenTheSystemShouldPresentAListOfReasonsForRequestingDelisting()
        {

        }

        // CC and Send Copy Options:
        [When("submitting a notice")]
        public void WhenSubmittingANotice()
        {
        }
        //Local Government Contact Information:

        [Then(@"there should be checkboxes or fields to enable sending a copy of the request to myself or adding additional CCs ""(.*)""")]
        public void LocalGovernmentContactInformationCC(string CCEmailAddress)
        {
            _DelistingWarningPage.SendCopyCheckbox.Click();

            _DelistingWarningPage.AdditionalCCsTextBox.EnterText(CCEmailAddress);
        }

        [Then("there should be fields to input email and phone number for the local government contact, with the latter being optional")]
        public void LocalGovernmentContactInformationEmailAndPhone()
        {

            _DelistingWarningPage.LocalGovEmailTextBox.EnterText("local@gov.bc");
            _DelistingWarningPage.LocalGovPhoneTextBox.EnterText("999-123-1234");

        }

        //DelistingWarningMessage
        [When("all required fields are entered")]
        public void WhenALlRequiredFieldsAreEnteredandIClickTheReviewButton()
        {
            _DelistingWarningPage.ReviewButton.WaitFor();
            _DelistingWarningPage.ReviewButton.Click();
        }

        [Then("I see a template delisting warning message that will be sent to both the platform and host")]
        public void ThenISeeATemplateDelistingWarningMessage()
        {
        }

        //SendDelistingRequest
        [When("I submit the form with valid information")]
        public void WhenISubmitTheFormWithValidInformation()
        {

            _NoticeOfTakeDownPage.CommentsTextBox.EnterText("get a business license");
            _NoticeOfTakeDownPage.SubmitButton.Click();
        }

        [Then("the system should send the delisting warning message to the provided platform and host email addresses")]
        public void ThenTheSystemShouldSendTheDelistingWarningMessage()
        {
        }

        //ConfirmationMessage
        [When("successful submission")]
        public void WhenSuccessfulSubmission()
        {
        }

        [Then("I should receive a confirmation message indicating that the delisting warning has been sent")]
        public void ThenIShouldReceiveAConfirmationMessage()
        {
            //Validate message and return to home page
            System.Threading.Thread.Sleep(3000);
            ClassicAssert.IsTrue(_DelistingWarningPage.EmbededDriver.PageSource.Contains("Your Notice of Takedown was Successfully Submitted!"));
            _DelistingWarningPage.ReturnHomeButton.Click();
        }

        [Then("I should be copied on the email")]
        public void ThenIShouldBeCopiedOnTheEmail()
        {
        }

        //NotificationToPlatformAndHost
        [When("the delisting warning is submitted")]
        public void WhenTheDelistingWarningIsSubmitted()
        {
        }

        [Then("the platform and host should receive email notifications containing the delisting warning and instructions for compliance")]
        public void ThenThePlatformAndHostShouldReceiveEmailNotifications()
        {
        }


        //FrontEndErrorHandling
        [When("there are issues with the submission, such as invalid email addresses or a missing URL")]
        public void WhenThereAreIssuesWithTheSubmission()
        {
        }

        [Then("the system should provide clear error messages guiding me on how to correct the issues")]
        public void ThenTheSystemShouldProvideClearErrorMessages()
        {
        }
    }
}
