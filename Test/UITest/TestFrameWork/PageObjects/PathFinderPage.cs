using UITest.Models;
using UITest.SeleniumObjects;
using UITest.TestDriver;

namespace TestFrameWork.Models
{
    public class PathFinderPage
    {
        private IDriver _Driver;
        private Button _IDRButton;
        private Button _BCIDButton;

        public Button IDRButton { get => _IDRButton; }  
        public Button BCIDButton { get => _BCIDButton; }

        public PathFinderPage(IDriver Driver)
        {
            _Driver = Driver;
            _IDRButton = new Button(Driver, Enums.FINDBY.ID, PathFinderModel.IDRButton);
            _BCIDButton = new Button(Driver, Enums.FINDBY.ID, PathFinderModel.BCIDButton);
        }
    }
}
