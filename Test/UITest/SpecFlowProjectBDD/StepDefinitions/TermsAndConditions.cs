using UITest.PageObjects;
using UITest.TestDriver;
using TestFrameWork.Models;
using Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.DevTools.V118.Debugger;
using SpecFlowProjectBDD.Helpers;

namespace SpecFlowProjectBDD.StepDefinitions
{
    [Binding]
    [Scope(Scenario = "TermsAndConditions")]
    public sealed class TermsAndConditions
    {
        private IDriver _Driver;
        private LandingPage _LandingPage;
        private TermsAndConditionsPage _TermsAndConditionsPage;
        private ManagingAccessPage _ManagingAccessPage;
        private PathFinderPage _PathFinderPage;
        private IDirLoginPage _IDirPage;
        private NoticeOfTakeDownPage _NoticeOfTakeDownPage;
        private string _TestUserName;
        private string _TestPassword;
        private string _TestUserType;
        private SFEnums.UserTypeEnum _UserType;
        private bool _ExpectedResult = false;
        AppSettings _AppSettings;

        public TermsAndConditions(SeleniumDriver Driver)
        {
            _Driver = Driver;
            _LandingPage = new LandingPage(_Driver);
            _TermsAndConditionsPage = new TermsAndConditionsPage(Driver);
            _PathFinderPage = new PathFinderPage(_Driver);
            _IDirPage = new IDirLoginPage(_Driver);
            _AppSettings = new AppSettings();
        }

        //User Authentication
        //[Given(@"that I am an authenticated user ""(.*)"" and the expected result is ""(.*)""")]
        [Given(@"that I am an authenticated User ""(.*)"" and the expected result is ""(.*)"" and I am a ""(.*)"" user")]
        public void GivenIAmAauthenticatedGovernmentUseer(string UserName, string ExpectedResult, string UserType)
        {
            _TestUserName = UserName;
            _TestPassword = _AppSettings.GetUser(_TestUserName) ?? string.Empty;
            _ExpectedResult = ExpectedResult.ToUpper() == "PASS" ? true : false;
            _TestUserType = UserType;
        }

        [When("I log in or access the system")]
        public void ILogInOrAccessTheSystem()
        {
            _Driver.Url = _AppSettings.GetServer("default");
            _Driver.Navigate();

            AuthHelper authHelper = new AuthHelper(_Driver);

            //Authenticate user using IDir or BCID depending on the user
            _UserType = authHelper.Authenticate(_TestUserName, _TestUserType);

            //TODO: Validate that the login was sucessfull
        }

        [Then("I have not previously accepted the terms and conditions after access approval")]
        public void IHaveNotPreviouslyAcceptedTheTermsAndConditions()
        {
        }

        [Then("I should be prompted to accept the terms and conditions")]
        public void ThenIShouldBePromptedToAcceptTheTermsAndConditions()
        {
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
    }
}
