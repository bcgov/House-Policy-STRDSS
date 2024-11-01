﻿using Configuration;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using OpenQA.Selenium;
using SpecFlowProjectBDD.Helpers;
using System.Diagnostics;
using TestFrameWork.Models;
using UITest.PageObjects;
using UITest.TestDriver;
using static SpecFlowProjectBDD.SFEnums;

namespace SpecFlowProjectBDD.StepDefinitions
{
    [Binding]
    [Scope(Scenario = "SendNoticeOfTakedownWithoutADSSlisting")]
    public sealed class SendNoticeOfTakedownWithoutADSSlisting
    {
        private IDriver _Driver;
        private LandingPage _LandingPage;
        private DelistingWarningPage _DelistingWarningPage;
        private PathFinderPage _PathFinderPage;
        private BCIDPage _BCIDPage;
        private NoticeOfTakeDownPage _NoticeOfTakeDownPage;
        private string _TestUserName;
        private string _TestPassword;
        private bool _ExpectedResult = false;
        private AppSettings _AppSettings;
        private SFEnums.LogonTypeEnum? _LogonType;

        public SendNoticeOfTakedownWithoutADSSlisting(SeleniumDriver Driver)
        {
            _Driver = Driver;
            _LandingPage = new LandingPage(_Driver);
            _DelistingWarningPage = new DelistingWarningPage(_Driver);
            _NoticeOfTakeDownPage = new NoticeOfTakeDownPage(_Driver);
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
            //Authenticate user using IDir or BCID depending on the user
            ClassicAssert.IsNotNull(_LogonType, "Logon FAILED");

            TermsAndConditionsHelper termsAndConditionsHelper = new TermsAndConditionsHelper(_Driver);
            termsAndConditionsHelper.HandleTermsAndConditions();
        }

        [When("I navigate to the delisting warning feature")]
        public void WhenINavigateToTheDelistingWarningFeature()
        {
            //_LandingPage.SendNoticeButton.ExecuteJavaScript(@"document.querySelector(""#navigate-to-compliance-notice-btn"").click()");
            _LandingPage.SendNoticeButton.Click();
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

        [Then(@"I should see an optional field for Listing ID ""(.*)""")]
        public void IShouldSeeAnOptionalFieldForListingID(string ListingID)
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
            _DelistingWarningPage.PlatformReceipientDropdown.JSExecuteJavaScript(@"document.querySelector(""#platformId_0"").click()");
            ClassicAssert.IsTrue(_DelistingWarningPage.PlatformReceipientDropdown.Text.ToUpper().Contains("TEST AIRBNB"));
        }

        [Then("the system should validate the email format")]
        public void ThenTheSystemShouldValidateTheEmailFormat()
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
            //_DelistingWarningPage.SendCopyCheckbox.Click();

            _DelistingWarningPage.AdditionalCCsTextBox.EnterText(CCEmailAddress);
        }

        [Then("there should be fields to input email and phone number for the local government contact, with the latter being optional")]
        public void LocalGovernmentContactInformationEmailAndPhone()
        {

            _DelistingWarningPage.LocalGovEmailTextBox.EnterText("local@gov.bc");
            //_DelistingWarningPage.LocalGovPhoneTextBox.EnterText("999-123-1234");

        }

        //DelistingWarningMessage
        [When("all required fields are entered")]
        public void WhenALlRequiredFieldsAreEnteredandIClickTheReviewButton()
        {
            if (!_DelistingWarningPage.ReviewButton.IsEnabled())
                if (_ExpectedResult == false)
                    Assert.Pass("Expected result was false. Actual result is false. Review button was disabled");
                else
                    Assert.Fail("Expected result was true, but the review button is disabled");

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
            ClassicAssert.IsTrue(_DelistingWarningPage.Driver.PageSource.Contains("Your Notice of Non-Compliance was Successfully Submitted!"));
            //DSS-457 - request to remove "return to home" button to stay on takedown request page so that another request can submitted
            //_DelistingWarningPage.ReturnHomeButton.Click();
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
