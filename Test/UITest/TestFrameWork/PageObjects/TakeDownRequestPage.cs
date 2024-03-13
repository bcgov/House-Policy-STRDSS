using TestFrameWork.Models;
using UITest.Models;
using UITest.SeleniumObjects;
using UITest.TestDriver;

namespace UITest.PageObjects
{
    public class TakeDownRequest
    {
        private TextBox _CommentsTextBox;
        private Button _SubmitButton;
        private Button _CancelButton;


        private IDriver _Driver;
        public TextBox CommentsTextBox { get => _CommentsTextBox; }
        public Button SubmitButton { get => _SubmitButton; }
        public Button CancelButton { get => _CancelButton; }

        public TakeDownRequest(IDriver Driver)
        {
            _Driver = Driver;
            _CommentsTextBox = new TextBox(Driver, Enums.FINDBY.ID, TakeDownRequestModel.CommentsTextBox);
            _SubmitButton = new Button(Driver, Enums.FINDBY.ID, TakeDownRequestModel.SubmitButton);
            _CancelButton = new Button(Driver, Enums.FINDBY.ID, TakeDownRequestModel.CancelButton);
        }

    }
}
