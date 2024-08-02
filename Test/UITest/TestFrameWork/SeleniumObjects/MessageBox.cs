using UITest.TestDriver;

namespace UITest.SeleniumObjects
{
    public class MessageBox : UIElement
    {
        public MessageBox(IDriver Driver, Enums.FINDBY LocatorType, string Locator) : base(Driver)
        {
            base.Locator = Locator;
            base.LocatorType = LocatorType;
        }

        public string GetText()
        {
            return (Text);
        }
    }
}
