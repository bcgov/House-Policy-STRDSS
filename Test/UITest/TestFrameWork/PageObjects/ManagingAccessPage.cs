﻿using UITest.Models;
using UITest.SeleniumObjects;
using UITest.TestDriver;
using UITest.TestObjectFramework;

namespace UITest.PageObjects
{
    public class ManagingAccessPage
    {
        private IDriver _Driver;

        private string _URL = @"localhost:5002/user-management";

        private DropDownList _StatusDropDown;
        private DropDownList _OrganizationDropDown;
        private TextBox _SearchTextBox;
        private Button _BackButton;
        private Button _ForwardButton;
        private Table _UserTable;


        public string URL { get => _URL; set => _URL = value; }
        public DropDownList StatusDropDown { get => _StatusDropDown; set => _StatusDropDown = value; }
        public DropDownList OrganizationDropDown { get => _OrganizationDropDown; set => _OrganizationDropDown = value; }
        public TextBox SearchTextBox { get => _SearchTextBox; set => _SearchTextBox = value; }
        public Button BackButton { get => _BackButton; }
        public Button ForwardButton { get => _ForwardButton; }
        public Table UserTable { get => _UserTable; }
        public IDriver Driver { get => _Driver; }

        public ManagingAccessPage(IDriver Driver)
        {
            _Driver = Driver;
            _StatusDropDown = new DropDownList(Driver, Enums.FINDBY.ID, ManagingAccessModel.StatusDropDown);
            _OrganizationDropDown = new DropDownList(Driver, Enums.FINDBY.ID, ManagingAccessModel.OrganizationDropDown);
            _SearchTextBox = new TextBox(Driver, Enums.FINDBY.ID, ManagingAccessModel.SearchTextBox);
            _BackButton = new Button(Driver, Enums.FINDBY.CSSSELECTOR, ManagingAccessModel.BackButton);
            _ForwardButton = new Button(Driver, Enums.FINDBY.CSSSELECTOR, ManagingAccessModel.ForwardButton);
            _UserTable = new Table(Driver, Enums.FINDBY.ID, ManagingAccessModel.UserTable);
        }
    }
}
