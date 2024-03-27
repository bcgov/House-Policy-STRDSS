using NUnit.Framework;
using UITest.PageObjects;
using UITest.TestDriver;
using Configuration;
using TestFrameWork.Models;

namespace SpecFlowProjectBDD.StepDefinitions
{
    [Binding]
    [Scope(Scenario = "SendDelistingRequestWithoutADSSListing")]
    public sealed class SendTakeDownRequestWithoutADSSListing
    {
        HomePage _HomePage;
        DelistingRequestPage _DelistingRequestPage;
        TakeDownRequestPage _TakeDownRequestPage;
        PathFinderPage _PathFinderPage;
        IDRLoginPage _IDRLoginPage;
        string _TestUserName;
        string _TestPassword;

        IDriver _driver;

 
        public SendTakeDownRequestWithoutADSSListing(SeleniumDriver Driver)
        {
            _driver = Driver;
            _HomePage = new HomePage(_driver);
            _DelistingRequestPage = new DelistingRequestPage(_driver);
            _TakeDownRequestPage = new TakeDownRequestPage(_driver);
            _PathFinderPage = new PathFinderPage(_driver);
            _IDRLoginPage = new IDRLoginPage(_driver);
            AppSettings appSettings = new AppSettings();
            _TestUserName = appSettings.GetValue("TestUserName") ?? string.Empty;
            _TestPassword = appSettings.GetValue("TestPassword") ?? string.Empty;
        }

        //User Authentication
        [Given("I am an authenticated LG staff member")]
        public void GivenIAmAauthenticatedLGStaffMemberUser()
        {
            _driver.Url = "http://127.0.0.1:4200/delisting-request";
            _driver.Navigate();
            
            _PathFinderPage.IDRButton.Click();

            _IDRLoginPage.UserNameTextBox.EnterText(_TestUserName);
            _IDRLoginPage.PasswordTextBox.EnterText(_TestPassword);

            _IDRLoginPage.ContinueButton.Click();

        }


        [When("I navigate to the delisting request feature")]
        public void WhenINavigateToTheDelistingRequestFeature()
        {
            //_driver.Url = "http://127.0.0.1:4200/delisting-request";
            //_driver.Navigate();
        }

        //Input Form
        [Then("I should be presented with an input form that includes fields for the listing URL")]
        public void IshouldBePresentedWithAnInputFormThatIncludesFields()
        {

        }

        [Then("I Should be Presented with an Input form that Lists requests Initiated By")]
        public void IShouldBePresentedWithAnInputFormThatListsRequestsInitiatedBy()
        { 

        }

        [Then("I should be presented with a field to select which platform to send the request to")]
        public void IShouldBePresentedWithAFieldToSelectWhichPlatformToSendTheRequestTo()
        {
            _DelistingRequestPage.PlatformReceipientDropdown.Click();
            _DelistingRequestPage.PlatformReceipientDropdown.Click();
        }

        [Then("I should see an optional field for adding a LG staff user email address to be copied on the email")]
        public void IShouldSeeAnOptionalFieldForAddingALGStaffUserEmailAddressToBeCopiedOnTheEmail()
        {
            _DelistingRequestPage.AdditionalCCsTextBox.EnterText("foo@foo.com");
        }

        //ListingID
        [When(@"Entering the listing ID ""(.*)""")]
        public void WhenEnteringTheListingID(String ID)
        {
            _DelistingRequestPage.ListingIDNumberTextBox.EnterText(ID);
        }

        [Then("The system should validate the ID is a number")]
        public void TheSystemShouldValidateTheIDFormat()
        {
        }

        //ListingURLField
        [When(@"Entering the listing URL ""(.*)""")]
        public void WhenEnteringTheListingURL(String URL)
        {
            _DelistingRequestPage.ListingUrlTextBox.EnterText(URL);
        }

        [Then("The system should validate the URL format and ensure it is a valid link to the property listing")]
        public void TheSystemShouldValidateTheURLFormat()
        {
        }

        //RequestInitiaitedByField
        [When("Selecting the LG for Initiated By")]
        public void SelectingTheLGForInitiatedBy()
        {
            _DelistingRequestPage.RequestInitiatedByDropDown.Click();
            _DelistingRequestPage.RequestInitiatedByDropDown.ExecuteJavaScript(@"document.querySelector(""#lgId_0"").click()");
            //Assert.IsTrue(_DelistingRequestPage.PlatformReceipientDropdown.Text.Contains("AIRBNB"));
        }

        [Then("The system should present a list of available LG options to populate the field")]
        public void TheSystemShouldPresentAListOfLGOptions()
        {
        }

        //PlatformField
        [When("selecting the platform")]
        public void WhenSelectingThePlatform()
        {
            _DelistingRequestPage.PlatformReceipientDropdown.Click();
            _DelistingRequestPage.PlatformReceipientDropdown.ExecuteJavaScript(@"document.querySelector(""#platformId_0"").click()");
            Assert.IsTrue( _DelistingRequestPage.PlatformReceipientDropdown.Text.ToUpper().Contains("AIRBNB"));
        }

        [Then("the system should present a list of available platform options to populate the field")]
        public void TheSystemShouldPresentAListOfAvailablePlatformOption()
        {
        }

        //DelistingRequestMessage
        [When("all required fields are entered")]
        public void WhenAllRequiredFieldsAreEntered() 
        { 
        }

        [Then("I click the review button")]
        public void ThenIClickTheReviewButton()
        {
            _DelistingRequestPage.ReviewButton.Click();
        }

        [Then("I see a template delisting request message that will be sent to both the platform")]
        public void ThenISeeATemplateDelistingRequestMessage() 
        { 
        }

        //SendDelistingRequest
        [When("I submit the form with valid information")]
        public void WhenISubmitTheFormWithValidInformation() 
        {
            _TakeDownRequestPage.SubmitButton.Click();
        }

        [Then("the system should send the delisting request message to the platform email addresses associated with the selected platform")]
        public void ThenTheSystemShouldSendTheDelistingRequestMessage() 
        {

        }

        //ConfirmationMessage
        [When("successful submission")]
        public void WhenSuccessfulSubmission() 
        {
        }

        [Then("I should receive a confirmation message indicating that the delisting request has been sent")]
        public void ThenIShouldReceiveAConfirmationMessage()
        {
            System.Threading.Thread.Sleep(3000);
            Assert.IsTrue(_DelistingRequestPage.EmbededDriver.PageSource.Contains("Your Notice of Takedown was Successfully Submitted!"));
            _DelistingRequestPage.ReturnHomeButton.Click();
        }

        [Then("I should be copied on the email")]
        public void ThenIShouldBeCopiedOnTheEmail()
        {
        }

        //NotificationToPlatformAndHost
        [When("the delisting request is submitted")]
        public void WhenTheDelistingRequestIsSubmitted()
        {
        }

        [Then("the platform and host should receive email notifications containing the delisting request and instructions for compliance")]
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
