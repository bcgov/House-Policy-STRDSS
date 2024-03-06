using UITest.TestDriver;

namespace UITest.SeleniumObjects
{
    public class HyperLink : UIElement
    {
        public HyperLink(IDriver Driver, Enums.FINDBY locatorType, string Locator) : base(Driver)
        {
            base.Locator = Locator;
            base.LocatorType = LocatorType;
        }

        public bool Navigate()
        {
            return (base.Navigate(base.Locator));
        }
    }
}

