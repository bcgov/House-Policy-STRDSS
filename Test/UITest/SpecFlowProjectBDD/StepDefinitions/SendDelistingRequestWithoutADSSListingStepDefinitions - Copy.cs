﻿using Models;
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
    [Scope(Scenario = "SendDelistingRequestWithoutADSSListing")]
    public sealed class SendDelistingRequestWithoutADSSListingStepDefinitions
    {
        LoginPage _loginPage;
        SignupPage _signupPage;
        DashboardPage _dashboardPage;
        CreateApplicationPage _createApplicationPage;
        IDriver _driver;

 
        public SendDelistingRequestWithoutADSSListingStepDefinitions(SeleniumDriver Driver)
        {
            _driver = Driver;
            _loginPage = new LoginPage(_driver);
            _signupPage = new SignupPage(_driver);
            _dashboardPage = new DashboardPage(_driver);
            _createApplicationPage = new CreateApplicationPage(_driver);
        }

        //User Authentication
        [Given("I am an authenticated LG staff member")]
        public void GivenIAmAauthenticatedLGStaffMemberUser()
        {
        }


        [When("I navigate to the delisting request feature")]
        public void WhenINavigateToTheDelistingRequestFeature()
        {
        }

        //Input Form
        [Then("I should be presented with an input form that includes fields for the listing URL")]
        public void IshouldBePresentedWithAnInputFormThatIncludesFields()
        {
        }

        [Then("I should be presented with a field to select which platform to send the request to")]
        public void IShouldBePresentedWithAFieldToSelectWhichPlatformToSendTheRequestTo()
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

        //DelistingRequestMessage
        [When("all required fields are entered")]
        public void WhenALlRequiredFieldsAreEntered() 
        { 
        }

        [Then("I see a template delisting request message that will be sent to both the platform")]
        public void ThenISeeATemplateDelistingRequestMessage() 
        { 
        }

        //SendDelistingRequest
        [When("I submit the form with valid information")]
        public void WhenISubmitTheFormWithValidInformation() 
        { 
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
