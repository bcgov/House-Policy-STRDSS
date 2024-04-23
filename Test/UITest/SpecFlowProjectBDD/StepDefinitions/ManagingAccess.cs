using NUnit.Framework;
using UITest.PageObjects;
using UITest.TestDriver;
using TestFrameWork.Models;
using Configuration;
using NUnit.Framework.Legacy;
using TechTalk.SpecFlow.CommonModels;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Security.Policy;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;

namespace SpecFlowProjectBDD.StepDefinitions
{
    [Binding]
    [Scope(Scenario = "ManagingAccess")]
    public sealed class ManagingAccess
    {
        private IDriver _Driver;
        private LandingPage _LandingPage;
        private TermsAndConditionsPage _TermsAndConditionsPage;
        private ManagingAccessPage _ManagingAccessPage;
        private PathFinderPage _PathFinderPage;
        private IDirPage _IDirPage;
        private NoticeOfTakeDownPage _NoticeOfTakeDownPage;
        private string _TestUserName;
        private string _TestPassword;
        private bool _ExpectedResult = false;
        AppSettings _AppSettings;

        public ManagingAccess(SeleniumDriver Driver)
        {
            _Driver = Driver;
            _LandingPage = new LandingPage(_Driver);
            _TermsAndConditionsPage = new TermsAndConditionsPage(Driver);
            _ManagingAccessPage = new ManagingAccessPage(_Driver);
            _NoticeOfTakeDownPage = new NoticeOfTakeDownPage(_Driver);
            _PathFinderPage = new PathFinderPage(_Driver);
            _IDirPage = new IDirPage(_Driver);
            _AppSettings = new AppSettings();
        }

        //User Authentication
        //[Given(@"I am an authenticated LG staff member and the expected result is ""(.*)""")]
        [Given(@"that I am an authenticated government user ""(.*)"" and the expected result is ""(.*)""")]
        public void GivenIAmAauthenticatedGovernmentUseer(string UserName, string ExpectedResult)
        {
            _TestUserName = UserName;
            _TestPassword = _AppSettings.GetUser(_TestUserName) ?? string.Empty;
            _ExpectedResult = ExpectedResult.ToUpper() == "PASS" ? true : false;

            _Driver.Url = _AppSettings.GetServer("local");
            _Driver.Navigate();

            _PathFinderPage.IDRButton.Click();

            _IDirPage.UserNameTextBox.WaitFor(5);

            _IDirPage.UserNameTextBox.EnterText(_TestUserName);

            _IDirPage.PasswordTextBox.EnterText(_TestPassword);

            _IDirPage.ContinueButton.Click();


            IWebElement TOC = null;

            try
            {
                TOC = _LandingPage.Driver.FindElement(Enums.FINDBY.CSSSELECTOR, TermsAndConditionsModel.TermsAndCondititionsCheckBox);
            }
            catch (NoSuchElementException ex)
            {
                //no Terms and Conditions. Continue
            }


            if ((null != TOC) && (TOC.Displayed))
            {
                //Nested Angular controls obscure the TermsAndConditionsCheckbox. Need JS 
                _TermsAndConditionsPage.TermsAndConditionsCheckBox.ExecuteJavaScript(@"document.querySelector(""body > app-root > app-layout > div.content > app-terms-and-conditions > p-card > div > div.p-card-body > div > div > div.checkbox-container > p-checkbox > div > div.p-checkbox-box"").click()");
                _TermsAndConditionsPage.ContinueButton.Click();
            }
        }


        [When("I access the administrative interface of the system")]
        public void IAccessTheAdministrativeInterfaceOfTheSystem()
        {
            _LandingPage.ManageAccessRequestsButton.Click();
        }

        [Then("There should be a dedicated section for managing user access requests")]
        public void ThereShouldBeADedicatedSectionForManagingUserAccessRequests()
        {
           // _ManagingAccessPage.Driver.ExecuteJavaScript();
        }

        //#User Access Request List


        [When("I navigate to the user access request section")]
        public void INavigateToTheUserAccessRequestSection()
        {
        }

        [Then("I should see a list displaying all user access requests, including relevant details such as the user's name, role request, and date of submission")]
        public void IShouldSeeAListDisplayingAllUserAccessRequestss()
        {
        }


        //Request Details
        [When("Reviewing a specific access request")]
        public void ReviewingASpecificAccessRequest()
        {
        }

        [Then("I should be able to view detailed information provided by the user, including their role request and any justifications or additional comments")]
        public void ShouldBeAbleToViewDetailedInformationProvidedByTheUser()
        {
        }

        //Grant Access Button
        [When("Reviewing an access request")]
        public void ReviewingAnAccessRequest()
        {
        }

        [Then("There should be a Grant Access button allowing me to approve the user's request")]
        public void ThereShouldBeAGrantAccessButton()
        {
        }

        //Role Assignment
        [When("Clicking the Grant Access button")]
        public void ClickingTheGrantAccessButton()
        {
        }

        [Then("I should be prompted to assign the appropriate roles to the user based on their request and the system's role hierarchy")]
        public void IShouldBePromptedToAssignTheAppropriateRolesToTheUser()
        {
        }


        //Deny Access Option
        [When("Reviewing an access request for denial")]
        public void ReviewingAnAccessRequestForFenial()
        {
        }

        [Then("There should be a Deny Access option allowing me to reject the user's request if it is deemed inappropriate or unnecessary")]
        public void ThereShouldBeADenyAccessOption()
        {
        }

        //Remove Access Option
        [When("Reviewing an access request that has been granted")]
        public void ReviewingAnAccessRequestThatHasBeenGranted()
        {
        }

        [Then("There should be a Remove Access option allowing me to remove the user's access if it is deemed inappropriate or unnecessary")]
        public void ThereShouldBeARemoveAccessOption()
        {
        }

        //Confirmation Message
        [When("Granting or denying access")]
        public void GrantingOrDenyingAccess()
        {
        }

        [Then("I should receive a confirmation message indicating the success of the action taken, and the user should be notified accordingly")]
        public void IShouldReceiveAConfirmationMessageIndicatingTheSuccessOfTheActionTaken()
        {
        }

        //User Access Status Updates
        [When("Managing user access requests")]
        public void ManagingUserAccessRequests()
        {
        }

        [Then("The access request list should dynamically update to reflect the current status approved or denied of each request")]
        public void TheAccessRequestListShouldDynamicallyUpdate()
        {
        }

    }
}
