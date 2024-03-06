using Models;
using UITest.SeleniumObjects;
using UITest.TestDriver;

namespace UITest.PageObjects
{
    public class SignupPage
    {
        private TextBox _LastName;

        private TextBox _UserName;

        private TextBox _Password;

        private TextBox _Phone;

        private TextBox _StreetAddress;

        private TextBox _City;

        private TextBox _Province;

        private TextBox _PostalCode;

        private JScript _SubmitButton;

        private string _URL = @"http://localhost:5002/register";
        private IDriver _Driver;

        public TextBox LastName { get => _LastName; }
        public TextBox UserName { get => _UserName; }
        public TextBox Password { get => _Password; }
        public TextBox Phone { get => _Phone; }
        public TextBox StreetAddress { get => _StreetAddress; }
        public JScript SubmitButton { get => _SubmitButton; }
        public TextBox City { get => _City; }
        public TextBox Province { get => _Province;  }
        public TextBox PostalCode { get => _PostalCode;  }
        public string URL { get => _URL; set => _URL = value; }
       

        public SignupPage(IDriver Driver)
        {
            _Driver = Driver;
            _UserName = new TextBox(Driver, Enums.FINDBY.CSSSELECTOR, SignupPageModel.UserName);
            _LastName = new TextBox(Driver, Enums.FINDBY.CSSSELECTOR, SignupPageModel.LastName);
            _Password = new TextBox(Driver, Enums.FINDBY.CSSSELECTOR, SignupPageModel.Password);
            _Phone = new TextBox(Driver, Enums.FINDBY.CSSSELECTOR, SignupPageModel.PhoneNumber);
            _StreetAddress = new TextBox(Driver, Enums.FINDBY.CSSSELECTOR, SignupPageModel.StreetAddress);
            _City = new TextBox(Driver, Enums.FINDBY.CSSSELECTOR, SignupPageModel.City);
            _Province = new TextBox(Driver, Enums.FINDBY.CSSSELECTOR, SignupPageModel.Province);
            _PostalCode = new TextBox(Driver, Enums.FINDBY.CSSSELECTOR, SignupPageModel.PostalCode);
             _SubmitButton = new JScript(Driver, Enums.FINDBY.JAVASCRIPT, SignupPageModel.SubmitButton);
        }

        public bool Navigate()
        {
            _Driver.Navigate(URL);
            return (true);
        }
    }
}

