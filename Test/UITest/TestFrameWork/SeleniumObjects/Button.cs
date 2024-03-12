using UITest.TestDriver;

namespace UITest.SeleniumObjects
{
    public class Button : UIElement
    {
        public Button(IDriver Driver, Enums.FINDBY LocatorType, string Locator) : base(Driver)
        {
            base.Locator = Locator;
            base.LocatorType = LocatorType;
        }
    }
}
