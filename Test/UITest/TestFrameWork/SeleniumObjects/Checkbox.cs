using UITest.TestDriver;

namespace UITest.SeleniumObjects
{
    public class CheckBox: UIElement
    {
        public CheckBox(IDriver Driver, Enums.FINDBY LocatorType, string Locator) : base(Driver)
        {
            base.Locator = Locator;
            base.LocatorType = LocatorType;
        }
    }
}
