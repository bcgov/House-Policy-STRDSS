using Microsoft.VisualStudio.TestPlatform.Utilities;
using Models;
using NUnit.Framework;
using UITest.PageObjects;
using UITest.TestDriver;
using UITest.TestEngine;

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

        [BeforeScenario()]
        public void BeforeScenario()
        {
            _driver = new SeleniumDriver(SeleniumDriver.DRIVERTYPE.CHROME);
            _loginPage = new LoginPage(_driver);
            _signupPage = new SignupPage(_driver);
            _dashboardPage = new DashboardPage(_driver);
            _createApplicationPage = new CreateApplicationPage(_driver);
        }

        [AfterScenario()]
        public void AfterScenario()
        {
            _driver.Close();
        }

        // For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef

        [Given("I am a user")]
        public void GivenIAmAUser()
        {
            //TODO: implement arrange (precondition) logic
            // For storing and retrieving scenario-specific data see https://go.specflow.org/doc-sharingdata
            // To use the multiline text or the table argument of the scenario,
            // additional string/Table parameters can be defined on the step definition
            // method. 

           // throw new PendingStepException();
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
