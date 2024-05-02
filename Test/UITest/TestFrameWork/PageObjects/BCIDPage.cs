using UITest.Models;
using UITest.SeleniumObjects;
using UITest.TestDriver;

namespace TestFrameWork.Models
{
    public class BCIDPage
    {
        private IDriver _Driver;
        private TextBox _UserNameTextBox;
        private TextBox _PasswordTextBox;
        private Button _ContinueButton;

        public TextBox UserNameTextBox { get => _UserNameTextBox; set => _UserNameTextBox = value; }  
        public TextBox PasswordTextBox { get => _PasswordTextBox; set => _PasswordTextBox = value; }
        public Button ContinueButton { get => _ContinueButton; set => _ContinueButton = value; }

        public BCIDPage(IDriver Driver)
        {
            _Driver = Driver;
            _UserNameTextBox = new TextBox(Driver, Enums.FINDBY.ID, BCIDModel.UserName);
            _PasswordTextBox = new TextBox(Driver, Enums.FINDBY.ID, BCIDModel.Password);
            _ContinueButton = new(Driver, Enums.FINDBY.CSSSELECTOR, BCIDModel.ContinueButton);
        }


    }
}
