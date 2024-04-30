using NUnit.Framework;
using UITest.PageObjects;
using UITest.TestDriver;
using TestFrameWork.Models;
using Configuration;
using NUnit.Framework.Legacy;
using System.Linq.Expressions;
using UITest.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using System.Reflection.Metadata;
using OpenQA.Selenium.Support.UI;

namespace SpecFlowProjectBDD.StepDefinitions
{
    [Binding]
    [Scope(Scenario = "SendNoticeOfTakedownWithoutADSSlisting")]
    public sealed class SendNoticeOfTakedownWithoutADSSlisting
    {
        private IDriver _Driver;
        private LandingPage _LandingPage;
        private DelistingWarningPage _DelistingWarningPage;
        private TermsAndConditionsPage _TermsAndConditionsPage;
        private PathFinderPage _PathFinderPage;
        private IDRLoginPage _IDRLoginPage;
        private NoticeOfTakeDownPage _NoticeOfTakeDownPage;
        private string _TestUserName;
        private string _TestPassword;
        private bool _ExpectedResult = false;
        private AppSettings  _AppSettings;

        public SendNoticeOfTakedownWithoutADSSlisting(SeleniumDriver Driver)
        {
            _Driver = Driver;
            _LandingPage = new LandingPage(_Driver);
            _DelistingWarningPage = new DelistingWarningPage(_Driver);
            _TermsAndConditionsPage = new TermsAndConditionsPage(Driver);
            _NoticeOfTakeDownPage = new NoticeOfTakeDownPage(_Driver);
            _PathFinderPage = new PathFinderPage(_Driver);
            _IDRLoginPage = new IDRLoginPage(_Driver);
            _AppSettings = new AppSettings();
        }

        //User Authentication
        [Given(@"that I am an authenticated LG staff member ""(.*)"" and the expected result is ""(.*)""")]
        public void GivenIAmAauthenticatedLGStaffMemberUser(string UserName, string ExpectedResult)
        {
            _TestUserName = UserName;
            _TestPassword = _AppSettings.GetUserValue(_TestUserName) ?? string.Empty;
            _ExpectedResult = ExpectedResult.ToUpper() == "PASS" ? true : false;

            _Driver.Url = "http://127.0.0.1:4200";
            _Driver.Navigate();

            _PathFinderPage.IDRButton.Click();

            _IDRLoginPage.UserNameTextBox.WaitFor(5);

            _IDRLoginPage.UserNameTextBox.EnterText(_TestUserName);

            _IDRLoginPage.PasswordTextBox.EnterText(_TestPassword);

            _IDRLoginPage.ContinueButton.Click();

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

            //[Then("I should be presented with a dropdown menu to select reason for delisting")]
            //public void IShouldBePresentedWithADropdownMenuToSelectReasonForDelisting()
            //{
            //    _DelistingWarningPage.ReasonDropdown.Click();
            //    _DelistingWarningPage.ReasonDropdown.Click();
            //}

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
                _DelistingWarningPage.PlatformReceipientDropdown.ExecuteJavaScript(@"document.querySelector(""#platformId_0"").click()");
                ClassicAssert.IsTrue(_DelistingWarningPage.PlatformReceipientDropdown.Text.ToUpper().Contains("TEST PLATFORM"));
            }

            [Then("the system should validate the email format")]
            public void ThenTheSystemShouldValidateTheEmailFormat()
            {
            }

            ////ReasonForDelisting
            //[When("I select a reason for delisting")]
            //public void WhenISelectAReasonForDelisting()
            //{
            //    _DelistingWarningPage.ReasonDropdown.Click();
            //    _DelistingWarningPage.ReasonDropdown.ExecuteJavaScript(@"document.querySelector(""#reasonId_0"").click()");
            //}

            //[Then("the system should present a list of reasons for requesting delisting: No business licence provided, invalid business licence number, expired business licence, or suspended business license")]
            //public void ThenTheSystemShouldPresentAListOfReasonsForRequestingDelisting()
            //{

            //}

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
                ClassicAssert.IsTrue(_DelistingWarningPage.Driver.PageSource.Contains("Your Notice of Takedown was Successfully Submitted!"));
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
