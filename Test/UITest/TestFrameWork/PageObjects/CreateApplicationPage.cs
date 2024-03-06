using UITest.Models;
using UITest.SeleniumObjects;
using UITest.TestDriver;
using UITest.TestObjectFramework;

namespace UITest.PageObjects
{
    public class CreateApplicationPage
    {
        public enum ZoneType  { RESIDENTIAL, COMMERCIAL, MIXEDUSE };
        //Residential", "Commercial" and "Mixed Use

        public enum ComplianceType { PENDING, COMPLIANT, NONCOMPLIANT };
        //"Pending", "Compliant" and "Non-compliant."

        private TextBox _PropertyOwnerLastName;

        private TextBox _PropertyStreetAddress;
        private TextBox _City;
        private TextBox _Province;
        private TextBox _PostalCode;
        private DropDownList _ZoningTypeDropDown;
        private TextBox _SquareFootage;
        private DropDownList _STRAffiliateDropDown;
        private CheckBox _IsPropertyOwnerPrimaryResidenceCheckBox;
        private Button _CancelButton;
        private Button _SubmitButton;

        public TextBox PropertyOwnerName { get => _PropertyOwnerLastName; }
        public TextBox PropertyStreetAddress { get => _PropertyStreetAddress; }
        public DropDownList ZoningTypeDropDown { get => _ZoningTypeDropDown; }
        public TextBox SquareFootage { get => _SquareFootage; }
        public DropDownList STRAffiliateDropDown { get => _STRAffiliateDropDown; }
        public CheckBox IsPropertyOwnerPrimaryResidence { get => _IsPropertyOwnerPrimaryResidenceCheckBox; }
        public TextBox City { get => _City; }
        public TextBox Province { get => _Province; }
        public TextBox PostalCode { get => _PostalCode; }
        public Button CancelButton { get => _CancelButton; }
        public Button SubmitButton { get => _SubmitButton;}

        public CreateApplicationPage(IDriver Driver)
        {
            _PropertyOwnerLastName = new TextBox(Driver, Enums.FINDBY.CSSSELECTOR, CreateApplicationPageModel.PropertyOwnerLastName);
            _PropertyStreetAddress = new TextBox(Driver, Enums.FINDBY.CSSSELECTOR, CreateApplicationPageModel.PropertyStreetAddress);
            _City = new TextBox(Driver, Enums.FINDBY.CSSSELECTOR, CreateApplicationPageModel.City);
            _Province = new TextBox(Driver, Enums.FINDBY.CSSSELECTOR, CreateApplicationPageModel.Province);
            _PostalCode = new TextBox(Driver, Enums.FINDBY.CSSSELECTOR, CreateApplicationPageModel.PostalCode);
            _ZoningTypeDropDown = new DropDownList(Driver, Enums.FINDBY.ID, CreateApplicationPageModel.ZoningTypeDropDown);
            _SquareFootage = new TextBox(Driver, Enums.FINDBY.CSSSELECTOR, CreateApplicationPageModel.SquareFootage);
            _STRAffiliateDropDown = new DropDownList(Driver, Enums.FINDBY.CSSSELECTOR, CreateApplicationPageModel.STRAffiliateDropDown);
            _IsPropertyOwnerPrimaryResidenceCheckBox = new CheckBox(Driver, Enums.FINDBY.CSSSELECTOR, CreateApplicationPageModel.IsPropertyOwnerPrimaryResidenceCheckBox);
            _CancelButton = new Button(Driver, Enums.FINDBY.CSSSELECTOR, CreateApplicationPageModel.CancelButton);
            _SubmitButton = new Button(Driver, Enums.FINDBY.CSSSELECTOR, CreateApplicationPageModel.SubmitButton);
        }
    }
}
