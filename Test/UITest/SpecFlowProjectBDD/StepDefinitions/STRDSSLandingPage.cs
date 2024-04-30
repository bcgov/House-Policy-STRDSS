using UITest.PageObjects;
using UITest.TestDriver;
using TestFrameWork.Models;
using Configuration;
using NUnit.Framework.Legacy;
using OpenQA.Selenium;
using static SpecFlowProjectBDD.SFEnums;
using System.Reflection;

namespace SpecFlowProjectBDD.StepDefinitions
{
    [Binding]
    [Scope(Scenario = "STRDSSLandingPage")]
    public sealed class STRDSSLandingPage
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
        private AppSettings _AppSettings;
        private SFEnums.UserTypeEnum _UserType;

        public STRDSSLandingPage(SeleniumDriver Driver)
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
        [Given(@"that I am an authenticated User ""(.*)"" and the expected result is ""(.*)""")]
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

        [When("I navigate to the Landing Page")]
        public void INavigateToTheLandingPage()
        {
        }


        //Landing Page for Government Users
        [When(@"I am an authenticated government user ""(.*)"" and I access the Data Sharing System landing page")]
        public void IAmAnAuthenticatedGovernmentUser(string UserType)
        {
            _UserType = SetUserType(UserType);

        }

        [Then("I should find where I can submit delisting warnings and requests to short-term rental platforms")]
        public void IShouldFindWhereICanSubmitDelistingWarningsAndRequests()
        {
            if (_UserType == UserTypeEnum.GOVERNMENT)
            {
                ClassicAssert.True(_LandingPage.SendNoticeButton.IsEnabled());
                ClassicAssert.True(_LandingPage.SendTakedownLetterButton.IsEnabled());
            }
        }

        //Landing Page for Platform Users:
        [When(@"I am an authenticated platform user ""(.*)"" and I access the Data Sharing System landing page")]
        public void WhenINavigateToTheDelistingWarningFeature(string UserType)
        {
            _UserType = SetUserType(UserType);
        }


        [Then("I should find where I can upload a CSV file")]
        public void IShouldFindWhereICanUploadACSVDile()
        {
            if (_UserType == UserTypeEnum.GOVERNMENT)
            {
                ClassicAssert.True(_LandingPage.SendNoticeButton.IsEnabled());
                ClassicAssert.True(_LandingPage.SendTakedownLetterButton.IsEnabled());
            }
        }


        [Then("I should see some information about my obligations as a platform")]
        public void IShouldSeeSomeInformationAboutMyObligationsAsAPlatform()
        {

        }

        //Clear Navigation
        [When("I explore the landing page")]
        public void IExploreTheLandingPage()
        {
        }

        [Then("there should be a clear and intuitive navigation menu that guides me to other relevant sections of the application")]
        public void ThereShouldBeAClearAndIntuitiveNavigationMenu()
        {
        }

        //Brand Guidelines:
        [When("viewing the landing page")]
        public void ViewingThelandingPage()
        {
        }

        [Then("it should have visual elements consistent with branding guidelines")]
        public void ItShouldHaveVisualElementsConsistentWithBrandingGuidelines()
        {

        }

        /****************** Helper Methods **************************/

        private UserTypeEnum SetUserType(string UserType)
        {
            switch (UserType.ToUpper())
            {
                case "CODEENFOREMENTSTAFF":
                case "CODEENFORCEMENTADMIN":
                case "LOCALGOVERNMENTUSER":
                    {
                        _UserType = SFEnums.UserTypeEnum.GOVERNMENT;
                        break;
                    }
                case "PLATFORMUSER":
                    {
                        _UserType = SFEnums.UserTypeEnum.GOVERNMENT;
                        break;
                    }
                default:
                    throw new ArgumentException("Unknown User Type (" + UserType + ")");
            }
            return (_UserType);
        }
    }
}
