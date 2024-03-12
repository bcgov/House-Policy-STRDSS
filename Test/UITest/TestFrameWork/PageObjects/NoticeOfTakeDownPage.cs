using TestFrameWork.Models;
using UITest.Models;
using UITest.SeleniumObjects;
using UITest.TestDriver;

namespace UITest.PageObjects
{
    public class NoticeOfTakeDownPage
    {
        private TextBox _CommentsTextBox;
        private Button _SubmitButton;
        private Button _CancelButton;

        private IDriver _Driver;
        public TextBox CommentsTextBox { get => _CommentsTextBox; }
        public Button SubmitButton { get => _SubmitButton; }
        public Button CancelButton { get => _CancelButton; }



        public NoticeOfTakeDownPage(IDriver Driver)
        {
            _Driver = Driver;
            _CommentsTextBox = new TextBox(Driver, Enums.FINDBY.CSSSELECTOR, NoticeOfTakeDownModel.CommentsTextBox);
            _SubmitButton = new Button(Driver, Enums.FINDBY.CSSSELECTOR, NoticeOfTakeDownModel.SubmitButton);
            _CancelButton = new Button(Driver, Enums.FINDBY.CSSSELECTOR, NoticeOfTakeDownModel.CancelButton);
        }
    }
}
