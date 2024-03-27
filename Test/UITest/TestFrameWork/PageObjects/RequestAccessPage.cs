using TestFrameWork.Models;
using UITest.SeleniumObjects;
using UITest.TestDriver;
using UITest.TestObjectFramework;

namespace TestFrameWork.PageObjects
{
    public  class RequestAccessPage
    {
        private IDriver _Driver;
        private DropDownList _UserRoleDropDown;
        private TextBox _UserOrganizationTextBox;
        private Button _SubmitButton;

        public DropDownList UserRoleDropDown { get => _UserRoleDropDown; }
        public TextBox UserOrganizationTextBox { get => _UserOrganizationTextBox; }
        public Button SubmitButton { get => _SubmitButton;}

        public RequestAccessPage(IDriver Driver)
        {
            _Driver = Driver;
            _UserRoleDropDown = new DropDownList(Driver, Enums.FINDBY.ID, RequestAccessModel.UserRoleDropDown);
            _UserOrganizationTextBox = new TextBox(Driver, Enums.FINDBY.ID, RequestAccessModel.UserOrganizationTextBox);
            _SubmitButton = new Button(Driver, Enums.FINDBY.ID, RequestAccessModel.SubmitButton);
        }
    }
}
