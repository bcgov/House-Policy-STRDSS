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

namespace SpecFlowProjectBDD.StepDefinitions
{
    [Binding]
    [Scope(Scenario = "DenyAccessToSystem")]
    public sealed class DenyAccessToSystem
    {
        private IDriver _Driver;
        private LayoutPage _LayoutPage;
        private DelistingWarningPage _DelistingWarningPage;
        private PathFinderPage _PathFinderPage;
        private IDirLoginPage _IDirPage;
        private NoticeOfTakeDownPage _NoticeOfTakeDownPage;
        private string _TestUserName;
        private string _TestPassword;
        private bool _ExpectedResult = false;
        AppSettings _AppSettings;

        public DenyAccessToSystem(SeleniumDriver Driver)
        {
            _Driver = Driver;
            _LayoutPage = new LayoutPage(_Driver);
            _PathFinderPage = new PathFinderPage(_Driver);
            _IDirPage = new IDirLoginPage(_Driver);

            _AppSettings = new AppSettings();
        }

        //User Authentication
        [Given(@"that I am an authenticated LG, CEU, Provincial Gov or Platform user and the expected result is ""(.*)""")]
        public void GivenIAmAauthenticatedGovernmentUseer(string ExpectedResult)
        {
            _ExpectedResult = ExpectedResult.ToUpper() == "PASS" ? true : false;

            _Driver.Url = _AppSettings.GetServer("default");
            _Driver.Navigate();

            _PathFinderPage.IDRButton.Click();
        }


        [When(@"I attempt to access the Data Sharing System as ""(.*)""")]
        public void IAttemptToAccessTheDataSharingSystem(string UserName)
        {
            _TestUserName = UserName;
            _TestPassword = _AppSettings.GetUser(_TestUserName) ?? string.Empty;

            _IDirPage.UserNameTextBox.WaitFor(5);

            _IDirPage.UserNameTextBox.EnterText(_TestUserName);

            _IDirPage.PasswordTextBox.EnterText(_TestPassword);

            _IDirPage.ContinueButton.Click();
        }

        [Then("I dont have the required access permissions")]
        public void IDontHaveTheRequiredAccessPermissions()
        {
        }


        [Then("I should see a specific message indicating that access is restricted")]
        public void IShouldSeeASpecificMessageIndicatingThatAccessIsRestricted()
        {
            System.Threading.Thread.Sleep(1000);
            ClassicAssert.IsTrue(_LayoutPage.Driver.PageSource.Contains("401 Access Denied"));
        }
    }
}
