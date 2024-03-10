using Models;
using NUnit.Framework;
using System;


using Microsoft.VisualStudio.TestPlatform.Utilities;
using Models;
using NUnit.Framework;
using UITest.PageObjects;
using UITest.TestDriver;
using UITest.TestEngine;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.Security.Policy;
using System.Collections.Generic;

namespace SpecFlowProjectBDD.StepDefinitions
{
    [Binding]
    public sealed class SendDelistingWarningWithoutADSSListingStepDefinitions
    {
        LoginPage _loginPage;
        SignupPage _signupPage;
        DashboardPage _dashboardPage;
        CreateApplicationPage _createApplicationPage;
        IDriver _driver;

 
        public SendDelistingWarningWithoutADSSListingStepDefinitions(SeleniumDriver Driver)
        {
            _driver = Driver;
            _loginPage = new LoginPage(_driver);
            _signupPage = new SignupPage(_driver);
            _dashboardPage = new DashboardPage(_driver);
            _createApplicationPage = new CreateApplicationPage(_driver);
        }

        //SendDelistingWarningWithoutADSSListing
        [Given("I am an authenticated LG staff member")]
        public void GivenIAmAauthenticatedLGStaffMemberUser()
        {
        }


        [When("I navigate to the delisting warning feature")]
        public void WhenINavigateToTheDelistingWarningFeature()
        {
        }

        [Then("I should be presented with an input form that includes fields for the listing URL, and an optional field for the host email address")]
        public void IshouldBePresentedWithAnInputFormThatIncludesFields()
        {
        }

        [Then("I should be presented with a field to select which platform to send the warning to")]
        public void IShouldBePresentedWithAFieldToSelectWhichPlatformToSendTheWarningTo()
        {
        }
        
        [Then("I should be presented with a dropdown menu to select reason for delisting")]
        public void IShouldBePresentedWithADropdownMenuToSelectReasonForDelisting()
        {
        }

        [Then("I should see an optional field for adding a LG staff user email address to be copied on the email")]
        public void IShouldSeeAnOptionalFieldForAddingALGStaffUserEmailAddressToBeCopiedOnTheEmail()
        {
        }

        //ListingURLField
        [When(@"Entering the listing URL ""(.*)""")]
        public void WhenEnteringTheListingURL(String URL)
        {
        }

        [Then("The system should validate the URL format and ensure it is a valid link to the property listing")]
        public void TheSystemShouldValidateTheURLFormat()
        {
        }

        //PlatformField
        [When("selecting the platform")]
        public void WhenSelectingThePlatform()
        {

        }

        [Then("the system should present a list of available platform options to populate the field")]
        public void TheSystemShouldPresentAListOfAvailablePlatformOption()
        {
        }


        //HostEmailAddressField
        [When("entering the optional host email address")]
        public void WhenENteringTheOptionalHostEmailAddress() 
        { 
        }


        [Then("the system should validate the email format")]
        public void ThenTheSystemShouldValidateTheEmailFormat() 
        { 
        }

        //ReasonForDelisting
        [When("I select a reason for delisting")]
        public void WhenISelectAReasonForDelisting() { }

        [Then("the system should present a list of reasons for requesting delisting: No business licence provided, invalid business licence number, expired business licence, or suspended business license")]
        public void ThenTheSystemShouldPresentAListOfReasonsForRequestingDelisting() { }


        //DelistingWarningMessage
        [When("all required fields are entered")]
        public void WhenALlRequiredFieldsAreEntered() { }

        [Then("I see a template delisting warning message that will be sent to both the platform and host")]
        public void ThenISeeATemplateDelistingWarningMessage() { }

        //SendDelistingRequest
        [When("I submit the form with valid information")]
        public void WhenISubmitTheFormWithValidInformation() { }

        [Then("the system should send the delisting warning message to the provided platform and host email addresses")]
        public void ThenTheSystemShouldSendTheDelistingWarningMessage() { }

        //ConfirmationMessage
        [When("successful submission")]
        public void WhenSuccessfulSubmission() { }

        [Then("I should receive a confirmation message indicating that the delisting warning has been sent")]
        public void ThenIShouldReceiveAConfirmationMessage() { }

        [Then("I should be copied on the email")]
        public void ThenIShouldBeCopiedOnTheEmail() { }

        //NotificationToPlatformAndHost
        [When("the delisting warning is submitted")]
        public void WhenTheDelistingWarningIsSubmitted() { }

        [Then("the platform and host should receive email notifications containing the delisting warning and instructions for compliance")]
        public void ThenThePlatformAndHostShouldReceiveEmailNotifications() { }


        //FrontEndErrorHandling
        [When("there are issues with the submission, such as invalid email addresses or a missing URL")]
        public void WhenThereAreIssuesWithTheSubmission() { }

        [Then("the system should provide clear error messages guiding me on how to correct the issues")]
        public void ThenTheSystemShouldProvideClearErrorMessages() { }
    }
}
