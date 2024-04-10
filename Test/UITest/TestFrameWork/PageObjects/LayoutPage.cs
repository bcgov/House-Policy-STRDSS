using UITest.Models;
using UITest.SeleniumObjects;
using UITest.TestDriver;

namespace UITest.PageObjects
{
    public class LayoutPage
    {
        private TextBox _AddApplicationButton;
        private string _URL = @"localhost:5002/login";
        private Button _LogoutButton;
        private IDriver _Driver;

        public TextBox AddApplicationButton { get => _AddApplicationButton; }
        public string URL { get => _URL; set => _URL = value; }
        public Button LogoutButton { get => _LogoutButton;  }
        public IDriver Driver { get => _Driver; }

        public LayoutPage(IDriver Driver)
        {
            _Driver = Driver;
            _AddApplicationButton = new TextBox(Driver, Enums.FINDBY.CSSSELECTOR, LayoutPageModel.AddApplicationButton);
            _LogoutButton = new Button(Driver, Enums.FINDBY.CSSSELECTOR, LayoutPageModel.LogoutButton);
        }

        public bool Navigate()
        {
            _Driver.Navigate(URL);
            return (true);
        }
    }
}
