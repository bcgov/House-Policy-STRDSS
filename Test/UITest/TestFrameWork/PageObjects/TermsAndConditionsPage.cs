using TestFrameWork.Models;
using UITest.Models;
using UITest.SeleniumObjects;
using UITest.TestDriver;

namespace UITest.PageObjects
{
    public class TermsAndConditionsPage
    {
        private CheckBox _TermsAndConditionsCheckBox;
        private Button _ContinueButton;
        private IDriver _Driver;

        public CheckBox TermsAndConditionsCheckBox { get => _TermsAndConditionsCheckBox; }
        public Button ContinueButton { get => _ContinueButton;  }
        public IDriver Driver { get => _Driver; }

        public TermsAndConditionsPage(IDriver Driver)
        {
            _Driver = Driver;
            _TermsAndConditionsCheckBox = new CheckBox(Driver, Enums.FINDBY.CSSSELECTOR, TermsAndConditionsModel.TermsAndCondititionsCheckBox);
            _ContinueButton = new Button(Driver, Enums.FINDBY.ID, TermsAndConditionsModel.ContinueButton);
        }
    }
}
