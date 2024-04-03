using OpenQA.Selenium;
using System.Collections.ObjectModel;
using UITest.TestDriver;

namespace UITest.SeleniumObjects
{
    public class UIElement
    {
        private string _Locator;
        private IDriver _Driver;
        protected Enums.FINDBY LocatorType;
        protected IWebElement Element { get; set; }
        public Validations Validations { get; set; }

        public string Text
        {
            get
            {
                return (Element.Text);
            }
        }

        public string Locator
        {
            get
            {
                return _Locator;
            }

            set
            {
                _Locator = value;
            }
        }

        public UIElement(IDriver Driver)
        {
            _Driver = Driver;
            Validations = new Validations(Driver);
        }

        virtual public bool Click()
        {
            try
            {
                FindElement(LocatorType, Locator);
                Element.Click();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("stale element reference"))
                {
                    //element locator changed between previous find and click. Try again before bailing
                    FindElement(LocatorType, Locator);
                    Element.Click();
                }
                else
                {
                    throw;
                }
            }

            return (true);
        }

        public bool FindElement(Enums.FINDBY By, string Locator)
        {
            Element = _Driver.FindElement(By, Locator);
            return (true);
        }

        public bool ExecuteJavaScript(string JavaScript)
        {
            if (null == _Driver)
                throw new ArgumentNullException("No driver defined");
            else
            {
                IJavaScriptExecutor js = _Driver.Driver as IJavaScriptExecutor;
                js.ExecuteScript(JavaScript);
            }

            return (true);
        }

        /// <summary>
        /// Finds a Web Element of the current locator type (Use when a locator can change)
        /// </summary>
        /// <param name="Locator"></param>
        /// <returns></returns>
        public IWebElement FindElement(string Locator)
        {
            Element = _Driver.FindElement(LocatorType, Locator);
            return (Element);
        }

        public ReadOnlyCollection<IWebElement> FindElements(Enums.FINDBY By, string Locator)
        {
            ReadOnlyCollection<IWebElement> elements = _Driver.FindElements(By, Locator);
            return (elements);
        }

        public bool Navigate(string URL)
        {
            _Driver.Navigate(URL); ;
            return (true);
        }

        public void SendKeys(string Text)
        {
            Element.SendKeys(Text);
        }

        public bool WaitFor(int Seconds = 5)
        {
            _Driver.WaitFor(LocatorType, Locator, Seconds);

            return (true);
        }

        public bool IsEnabled()
        {
            FindElement(LocatorType, Locator);
            return (Element.Enabled);
        }

        /// <summary>
        /// Causes the currently executing test to break when debugging. 
        /// Remove when not debugging as this will cause the test to hang.
        /// </summary>
        /// <returns></returns>
        public bool Break()
        {
            System.Diagnostics.Debugger.Break();
            return (true);
        }
    }
}

