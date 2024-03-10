using Models;
using NUnit.Framework;
using UITest.PageObjects;
using UITest.TestDriver;

namespace SpecFlowProjectBDD.StepDefinitions
{
    [Binding]
    public sealed class SignupStepDefinitions
    {
        LoginPage _loginPage;
        SignupPage _signupPage;
        DashboardPage _dashboardPage;
        CreateApplicationPage _createApplicationPage;
        IDriver _driver;

        public SignupStepDefinitions(SeleniumDriver Driver)
        {
            _driver = Driver;
            //Create PageObjectsAndControle
            _loginPage = new LoginPage(_driver);
            _signupPage = new SignupPage(_driver);
            _dashboardPage = new DashboardPage(_driver);
            _createApplicationPage = new CreateApplicationPage(_driver);
        }


        [Given("I am a user")]
        public void GivenIAmAUser()
        {
        }


        [When(@"I navigate to the STR Homepage \(""(.*)""\)")]
        public void WhenINavigateToTheSTRHomepage(string HomePage)
        {
            _driver.Url = HomePage;
            _loginPage.Navigate();
        }

        [When("I click the Signup Button")]
        public void WhenIClickTheSignupButton()
        {

            _loginPage.SignupButton.Click();

        }

        [Then("I am presented with a signup dialog")]
        public void ThenIAmPresentedWithASignupDialog()
        {

            Assert.IsTrue(_signupPage.UserName.FindElement(Enums.FINDBY.CSSSELECTOR, SignupPageModel.UserName));
        }
    }
}
