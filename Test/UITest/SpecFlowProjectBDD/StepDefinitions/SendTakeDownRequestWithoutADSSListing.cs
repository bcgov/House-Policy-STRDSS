using NUnit.Framework;
using UITest.PageObjects;
using UITest.TestDriver;
using Configuration;
using TestFrameWork.Models;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using Gherkin.CucumberMessages.Types;
using NUnit.Framework.Legacy;

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
        int _CurrentRow = 0;
        bool _ExpectedResult = false;
        bool _ActualResult = false;
        //IEnumerable<ScenarioParameters> _ScenarioParameters;
        //private readonly ScenarioContext _scenarioContext;

        IDriver _driver;

        [AfterScenario]
        public void AfterScenario()
        {


        }



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
        [Given(@"I am an authenticated LG staff member and the expected result is ""(.*)""")]
        public void GivenIAmAauthenticatedLGStaffMemberUser(string ExpectedResult)
        {
            _ActualResult = true;
            //var inputsTable = ScenarioContext.Current["table"] as Table;
            //_ScenarioParameters = inputsTable.CreateSet<ScenarioParameters>().ToList<ScenarioParameters>();
            _ExpectedResult = ExpectedResult.ToUpper() == "PASS" ? true : false;

            _driver.Url = "http://127.0.0.1:4200/delisting-request";
            _driver.Navigate();

            _PathFinderPage.IDRButton.Click();

            _IDRLoginPage.UserNameTextBox.WaitFor();
            _IDRLoginPage.UserNameTextBox.EnterText(_TestUserName);
            _IDRLoginPage.PasswordTextBox.EnterText(_TestPassword);

            _IDRLoginPage.ContinueButton.Click();
            _ActualResult = true;

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
        public void WhenEnteringTheListingID(string ListingID)
        {
            _DelistingRequestPage.ListingIDNumberTextBox.EnterText(ListingID);
        }

        [Then("The system should validate the ID is a number")]
        public void TheSystemShouldValidateTheIDFormat()
        {
        }

        //ListingURLField
        //[When(@"Entering the listing URL")]
        [When(@"Entering the listing URL ""(.*)""")]
        public void WhenEnteringTheListingURL(string URL)
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
            //ClassicAssert.IsTrue(_DelistingRequestPage.PlatformReceipientDropdown.Text.Contains("AIRBNB"));
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
            ClassicAssert.IsTrue(_DelistingRequestPage.PlatformReceipientDropdown.Text.ToUpper().Contains("AIRBNB"));
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
            try
            {
                _DelistingRequestPage.ReviewButton.Click();
                _ActualResult = true;
            }
            catch (Exception ex)
            {
                if (_ExpectedResult == true)
                    Assert.Fail();
            }

        }

        [Then("I see a template delisting request message that will be sent to both the platform")]
        public void ThenISeeATemplateDelistingRequestMessage()
        {
        }

        //SendDelistingRequest
        [When("I submit the form with valid information")]
        public void WhenISubmitTheFormWithValidInformation()
        {
            _ActualResult = false;
            _TakeDownRequestPage.SubmitButton.Click();
            _ActualResult = true;
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
            _ActualResult = false;
            System.Threading.Thread.Sleep(3000);
            ClassicAssert.IsTrue(_DelistingRequestPage.EmbededDriver.PageSource.Contains("Your Notice of Takedown was Successfully Submitted!"));
            _DelistingRequestPage.ReturnHomeButton.Click();
            _ActualResult = true;
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

    public class ScenarioParameters
    {
        public string ListingID { get; set; }
        public string Comment { get; set; }
        public string LIDExpectedResult { get; set; }
        public string ListingURL { get; set; }
        public string LstExpectedResult { get; set; }

    }
}
