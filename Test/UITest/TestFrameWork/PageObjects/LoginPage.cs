using System;
using UITest.Models;
using UITest.SeleniumObjects;
using UITest.TestDriver;

namespace UITest.PageObjects
{
    public class LoginPage
    {
        private IDriver _Driver;
        private TextBox _UserName;
        private TextBox _Password;
        private Button  _SubmitButton;
        private Button _SignupButton;
        private string _URL = @"localhost:5002/login";


        public TextBox UserName { get => _UserName; }
        public TextBox Password { get => _Password; }
        public Button SigninButton { get => _SubmitButton; }
        public Button SignupButton { get => _SignupButton; }
        public string URL { get => _URL; set => _URL = value; }

        public LoginPage(IDriver Driver)
        {
            _Driver = Driver;
            _UserName = new TextBox(Driver, Enums.FINDBY.CSSSELECTOR, LoginPageModel.UserName);
            _Password = new TextBox(Driver, Enums.FINDBY.CSSSELECTOR, LoginPageModel.Password);
            _SignupButton = new Button(Driver, Enums.FINDBY.CSSSELECTOR, LoginPageModel.SignupButton);
            _SubmitButton = new Button(Driver, Enums.FINDBY.ID, LoginPageModel.SubmitButton);
        }

        public bool Navigate()
        {
            _Driver.Navigate(URL);
            return (true);
        }

    }
}
