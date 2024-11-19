using UITest.Models;
using UITest.SeleniumObjects;
using UITest.TestDriver;
using UITest.TestObjectFramework;

namespace UITest.PageObjects
{
    public class UpdateJurisdictionInformationPage
    {
        private IDriver _Driver;

        //No Valid URL since the URL must contain a unique and new platform number
        //private string _URL = @"http://127.0.0.1:4200/update-jurisdiction-information";

        private TextBox _JurisdictionNameTextBox;
        private TextBox _ShapeFileIDTextBox;
        private DropDownList _LocalGovernmentNameDropDown;
        private Button _PrincipleResidenceRequirementTrueButton;
        private Button _PrincipleResidenceRequirementFalseButton;
        private Button _ShortTermRentalProhibitedTrueButton;
        private Button _ShortTermRentalProhibitedFalseButton;
        private Button _BusinessLiscenseRequirementTrueButton;
        private Button _BusinessLiscenseRequirementFalseButton;
        private DropDownList _EconomicRegionDropDown;
        private Button _CancelButton;
        private Button _SaveButton;


        public IDriver Driver { get => _Driver; }
        public TextBox JurisdictionNameTextBox { get => _JurisdictionNameTextBox; }
        public TextBox ShapeFileIDTextBox { get => _ShapeFileIDTextBox; }
        public DropDownList LocalGovernmentNameDropDown { get => _LocalGovernmentNameDropDown; }
        public Button PrincipleResidenceRequirementTrueButton { get => _PrincipleResidenceRequirementTrueButton; }
        public Button PrincipleResidenceRequirementFalseButton { get => _PrincipleResidenceRequirementFalseButton; }
        public Button ShortTermRentalProhibitedTrueButton { get => _ShortTermRentalProhibitedTrueButton; }
        public Button ShortTermRentalProhibitedFalseButton { get => _ShortTermRentalProhibitedFalseButton; }
        public Button BusinessLiscenseRequirementTrueButton { get => _BusinessLiscenseRequirementTrueButton; }
        public Button BusinessLiscenseRequirementFalseButton { get => _BusinessLiscenseRequirementFalseButton; }
        public DropDownList EconomicRegionDropDown { get => _EconomicRegionDropDown; }
        public Button SaveButton { get => _SaveButton; }
        public Button CancelButton { get => _CancelButton; }


        public UpdateJurisdictionInformationPage(IDriver Driver)
        {
            _Driver = Driver;

            _JurisdictionNameTextBox = new TextBox(Driver, Enums.FINDBY.ID, UpdateJurisdictionInformationPageModel.JurisdictionNameTextBox);
            _ShapeFileIDTextBox = new TextBox(Driver, Enums.FINDBY.ID, UpdateJurisdictionInformationPageModel.ShapeFileIDTextBox);
            _LocalGovernmentNameDropDown = new DropDownList(Driver, Enums.FINDBY.ID, UpdateJurisdictionInformationPageModel.LocalGovernmentNameDropDown);
            _PrincipleResidenceRequirementTrueButton = new Button(Driver, Enums.FINDBY.JAVASCRIPT, UpdateJurisdictionInformationPageModel.PrincipleResidenceRequirementTrueButton);
            _PrincipleResidenceRequirementFalseButton = new Button(Driver, Enums.FINDBY.JAVASCRIPT, UpdateJurisdictionInformationPageModel.PrincipleResidenceRequirementFalseButton);
            _ShortTermRentalProhibitedTrueButton = new Button(Driver, Enums.FINDBY.JAVASCRIPT, UpdateJurisdictionInformationPageModel.ShortTermRentalProhibitedTrueButton);
            _ShortTermRentalProhibitedFalseButton = new Button(Driver, Enums.FINDBY.JAVASCRIPT, UpdateJurisdictionInformationPageModel.ShortTermRentalProhibitedFalseButton);
            _BusinessLiscenseRequirementTrueButton = new Button(Driver, Enums.FINDBY.JAVASCRIPT, UpdateJurisdictionInformationPageModel.BusinessLiscenseRequirementTrueButton);
            _BusinessLiscenseRequirementFalseButton = new Button(Driver, Enums.FINDBY.JAVASCRIPT, UpdateJurisdictionInformationPageModel.BusinessLiscenseRequirementFalseButton);
            _EconomicRegionDropDown = new DropDownList(Driver, Enums.FINDBY.ID, UpdateJurisdictionInformationPageModel.EconomicRegionDropDown);
            _SaveButton = new Button(Driver, Enums.FINDBY.CSSSELECTOR, UpdateJurisdictionInformationPageModel.SaveButton);
            _CancelButton = new Button(Driver, Enums.FINDBY.CSSSELECTOR, UpdateJurisdictionInformationPageModel.CancelButton);
        }

        /// <summary>
        /// SelectEconomicRegionListItem is a custom page method for the PrimeNG customer dropdown list on this page
        /// </summary>
        /// <param name="index"></param>
        public void SelectEconomicRegionListItem(int index)
        {
            if (index >= 0)
                _EconomicRegionDropDown.JSExecuteJavaScript($"document.querySelector(\"#economicRegionDsc_{index}\").click()");
        }
    }
}
