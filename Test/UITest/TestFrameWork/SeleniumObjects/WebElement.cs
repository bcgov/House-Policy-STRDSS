using UITest.TestDriver;

namespace UITest.SeleniumObjects
{
    public class WebElement : UIElement
    {
        public WebElement(IDriver Driver, Enums.FINDBY LocatorType, string Locator) : base(Driver)
        {
            base.LocatorType = LocatorType;
            base.Locator = Locator;
        }
    }
}

