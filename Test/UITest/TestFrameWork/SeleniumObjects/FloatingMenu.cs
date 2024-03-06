using OpenQA.Selenium;
using UITest.TestDriver;

namespace UITest.SeleniumObjects
{
    public class FloatingMenu : UIElement
    {
        private Dictionary<string, UIElement> _MenuItemList;

        public Dictionary<string, UIElement> MenuItemList
        {
            get
            {
                return _MenuItemList;
            }
        }

        public FloatingMenu(IDriver Driver, Enums.FINDBY LocatorType, string Locator) : base(Driver)
        {
            base.Locator = Locator;
            base.LocatorType = LocatorType;
            _MenuItemList = new Dictionary<string, UIElement>();
        }

        public void Select(int Index)
        {
            FindElement(LocatorType, Locator);

            for (int i = 1; i <= Index; i++)
            {
                SendKeys(OpenQA.Selenium.Keys.ArrowDown);
            }
            SendKeys(Keys.Enter);
        }

        public override bool Click()
        {
            FindElement(base.LocatorType, base.Locator);
            return (base.Click());
        }
    }
}
