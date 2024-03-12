using UITest.TestDriver;

namespace UITest.SeleniumObjects
{
    public class JScript : UIElement
    {
        public JScript(IDriver Driver, Enums.FINDBY LocatorType, string Locator) : base(Driver)
        {
            base.Locator = Locator;
            base.LocatorType = LocatorType;
        }
        public bool ExecuteJScript()
        {
            base.ExecuteJavaScript(base.Locator);
            return (true);
        }
    }
}
