using UITest.SeleniumObjects;
using UITest.TestDriver;

namespace UITest.TestObjectFramework
{
    public class DropDownList : UIElement
    {
        public DropDownList(IDriver Driver, Enums.FINDBY LocatorType, string Locator) : base(Driver)
        {
            base.Locator = Locator;
            base.LocatorType = LocatorType;
        }

        public bool SelectFirstListItem()
        {
            FindElement(LocatorType, Locator);
            Element.Click();
            Element.SendKeys(OpenQA.Selenium.Keys.ArrowDown);
            return (true);
        }
    }
}

