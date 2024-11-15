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
                if (LocatorType == Enums.FINDBY.JAVASCRIPT)
                {
                    JSExecuteJavaScript(Locator);
                }
                else
                {
                    FindElement(LocatorType, Locator);
                    Element.Click();
                }
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
            if (By == Enums.FINDBY.JAVASCRIPT)
            {
                //do not try to locate
            }
            else
            {
                Element = _Driver.FindElement(By, Locator);
            }

            return (true);
        }


        public bool JSCheckVisability(string Selector)
        {
            IJavaScriptExecutor js = _Driver.Driver as IJavaScriptExecutor;
            var script = @"
                try {
                    var elem = document.querySelector('" + Selector + @"');
                    console.log('Element:', elem);
                    if (elem) {
                        console.log('Element found');
                        if (typeof elem.checkVisibility === 'function') {
                            var visibilityResult = elem.checkVisibility();
                            console.log('checkVisibility result:', visibilityResult);
                            return visibilityResult;
                        } else {
                            console.error('checkVisibility is not a function');
                            return false;
                        }
                    } else {
                        console.error('Element not found');
                        return false;
                    }
                } catch (e) {
                    console.error('Error:', e);
                    return false;
                }
                ";
            bool result = (bool)js.ExecuteScript(script);
            return (result);
        }

        public object JSExecuteJavaScript(string Script)
        {
            IJavaScriptExecutor js = _Driver.Driver as IJavaScriptExecutor;
            var script = @"
                try {
                    var result = " + Script + @";
                    console.log('Result:', result);
                    return(result);
                } catch (e) {
                    console.error('Error:', e);
                    return false;
                }
                ";
            object result = js.ExecuteScript(script);
            return (result);
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

        public bool Exists()
        {
            IJavaScriptExecutor js = _Driver.Driver as IJavaScriptExecutor;
            string script = string.Empty; ;
            switch (LocatorType)
            {
                case Enums.FINDBY.ID:
                    {
                        script = @$"if (document.getElementById('{Locator}') != null) return true; return false;";
                        break;
                    }
                case Enums.FINDBY.CSSSELECTOR:
                    {
                        script = @$"if (document.querySelector('{Locator}') != null) return true; return false;";
                        break;
                    }
            }
            object result = js.ExecuteScript(script);

            if ((bool)result)
            {
                return (true);
            }
            else
            {
                return(false);
            }

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

