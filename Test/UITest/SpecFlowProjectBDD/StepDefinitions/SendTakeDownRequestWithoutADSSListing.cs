﻿using Configuration;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using OpenQA.Selenium;
using SpecFlowProjectBDD.Helpers;
using TestFrameWork.Models;
using UITest.PageObjects;
using UITest.TestDriver;
using static SpecFlowProjectBDD.SFEnums;

namespace SpecFlowProjectBDD.StepDefinitions
{
    [Binding]
    [Scope(Scenario = "SendTakedownRequestWithoutADSSListing")]
    public sealed class SendTakeDownRequestWithoutADSSListing
    {
        private LandingPage _LandingPage;
        private DelistingRequestPage _DelistingRequestPage;
        private TakeDownRequestPage _TakeDownRequestPage;
        private PathFinderPage _PathFinderPage;
        private BCIDPage _BCIDPage;
        private string _TestUserName;
        private string _TestPassword;
        private int _CurrentRow = 0;
        private bool _ExpectedResult = false;
        private IDriver _Driver;
        private AppSettings _AppSettings;
        private SFEnums.LogonTypeEnum? _LogonType;

        public SendTakeDownRequestWithoutADSSListing(SeleniumDriver Driver)
        {
            _Driver = Driver;
            _LandingPage = new LandingPage(_Driver);
            _DelistingRequestPage = new DelistingRequestPage(_Driver);
            _TakeDownRequestPage = new TakeDownRequestPage(_Driver);
            _PathFinderPage = new PathFinderPage(_Driver);
            _BCIDPage = new BCIDPage(_Driver);
            _AppSettings = new AppSettings();
        }

        //User Authentication
        [Given(@"that I am an authenticated LG staff member ""(.*)"" and the expected result is ""(.*)""")]
        public void GivenIAmAauthenticatedLGStaffMemberUser(string UserName, string ExpectedResult)
        {
            _TestUserName = UserName;
            _TestPassword = _AppSettings.GetUser(_TestUserName) ?? string.Empty;
            _ExpectedResult = ExpectedResult.ToUpper() == "PASS" ? true : false;

            _Driver.Url = _AppSettings.GetServer("default");
            _Driver.Navigate();

            AuthHelper authHelper = new AuthHelper(_Driver);

            //Authenticate user using IDir or BCID depending on the user
            _LogonType = authHelper.Authenticate(_TestUserName, _TestPassword, UserTypeEnum.LOCALGOVERNMENT);
            ClassicAssert.IsNotNull(_LogonType, "Logon FAILED");

            TermsAndConditionsHelper termsAndConditionsHelper = new TermsAndConditionsHelper(_Driver);
            termsAndConditionsHelper.HandleTermsAndConditions();
        }


        [When("I navigate to the delisting request feature")]
        public void WhenINavigateToTheDelistingRequestFeature()
        {
            _LandingPage.SendTakedownLetterButton.Click();
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
        [When(@"Entering the listing URL ""(.*)""")]
        public void WhenEnteringTheListingURL(string URL)
        {
            _DelistingRequestPage.ListingUrlTextBox.EnterText(URL);
        }

        [Then("The system should validate the URL format and ensure it is a valid link to the property listing")]
        public void TheSystemShouldValidateTheURLFormat()
        {
        }

        //DSS-305 remove

        ////RequestInitiaitedByField
        //[When("Selecting the LG for Initiated By")]
        //public void SelectingTheLGForInitiatedBy()
        //{
        //    _DelistingRequestPage.RequestInitiatedByDropDown.Click();
        //    _DelistingRequestPage.RequestInitiatedByDropDown.ExecuteJavaScript(@"document.querySelector(""#lgId_0"").click()");        }

        //[Then("The system should present a list of available LG options to populate the field")]
        //public void TheSystemShouldPresentAListOfLGOptions()
        //{
        //}

        //PlatformField
        [When("selecting the platform")]
        public void WhenSelectingThePlatform()
        {
            _DelistingRequestPage.PlatformReceipientDropdown.Click();
            _DelistingRequestPage.PlatformReceipientDropdown.JSExecuteJavaScript(@"document.querySelector(""#platformId_0"").click()");
            ClassicAssert.IsTrue(_DelistingRequestPage.PlatformReceipientDropdown.Text.ToUpper().Contains("TEST AIRBNB"));
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
            if (!_DelistingRequestPage.ReviewButton.IsEnabled())
                if (_ExpectedResult == false)
                    Assert.Pass("Expected result was false. Actual result is false. Review button was disabled");
                else
                    Assert.Fail("Expected result was true, but the review button is disabled");

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
            ClassicAssert.IsTrue(_DelistingRequestPage.Driver.PageSource.Contains("Your Takedown Request was Successfully Submitted!"));
            //DSS-457 - request to remove "return to home" button to stay on takedown request page so that another request can submitted
            ///_DelistingRequestPage.ReturnHomeButton.Click();
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
