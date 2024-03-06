using UITest.Models;
using UITest.SeleniumObjects;
using UITest.TestDriver;

namespace UITest.PageObjects
{
    public class DashboardPage
    {
        private TextBox _AddApplicationButton;
        private string _URL = @"localhost:5002/login";
        private Button _LogoutButton;
        private IDriver _Driver;

        public TextBox AddApplicationButton { get => _AddApplicationButton; }
        public string URL { get => _URL; set => _URL = value; }
        public Button LogoutButton { get => _LogoutButton;  }

        public DashboardPage(IDriver Driver)
        {
            _Driver = Driver;
            _AddApplicationButton = new TextBox(Driver, Enums.FINDBY.CSSSELECTOR, DashboardPageModel.AddApplicationButton);
            _LogoutButton = new Button(Driver, Enums.FINDBY.CSSSELECTOR, DashboardPageModel.LogoutButton);
        }

        public bool Navigate()
        {
            _Driver.Navigate(URL);
            return (true);
        }
    }
}
