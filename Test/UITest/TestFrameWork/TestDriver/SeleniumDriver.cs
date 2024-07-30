using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;
using System.Diagnostics;


namespace UITest.TestDriver
{
    public class SeleniumDriver : IDriver, IDisposable
    {
        private IWebElement _Element;
        private DRIVERTYPE _DriverType;

        public IWebDriver Driver { get; set; }

        public enum DRIVERTYPE { CHROME, EDGE, FIREFOX };

        public string Url 
        {
            get
            {
             return Driver.Url;
            }
            set
            {
                Driver.Url = value;
            }
        }
        public string PageSource { get => Driver.PageSource;}

        public string Title { get => Driver.Title; }
        public string CurrentWindowHandle { get => Driver.CurrentWindowHandle; }

        public ReadOnlyCollection<string> WindowHandles { get => Driver.WindowHandles; }

        public SeleniumDriver(DRIVERTYPE DriverType)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var assemblyDirectory = assembly.Location.Replace(assembly.ManifestModule.Name.ToString(), string.Empty);

            switch (DriverType)
            {
                case DRIVERTYPE.CHROME:
                    {
                        var options = new ChromeOptions();
                        options.SetLoggingPreference(LogType.Driver, LogLevel.All);
                        options.AddArgument("--ignore-ssl-errors=yes");
                        options.AddArgument("--ignore-certificate-errors");
                        options.AddArgument("--start-maximized");
                        //options.AddArgument("--headless");

                        Driver = new ChromeDriver(assemblyDirectory, options);
                        break;
                    }
                case DRIVERTYPE.EDGE:
                    {
                        Driver = new EdgeDriver();
                        break;
                    }
                case DRIVERTYPE.FIREFOX:
                    {
                        Driver = new FirefoxDriver(assemblyDirectory);
                        break;
                    }
            }
            _DriverType = DriverType;
        }

        public INavigation Navigate()
        {
            INavigation navigation = Driver.Navigate();
            return (navigation);
        }

        public bool Navigate(string URL)
        {
            Driver.Navigate().GoToUrl(URL);
            return (true);
        }


        public string GetCurrentURL() 
        {
            return (Driver.Url);
        }

        public IWebElement FindElement(By by)
        {
            return (Driver.FindElement(by));
        }

        public IWebElement FindElement(Enums.FINDBY By, string Locator)
        {
            try
            {
                switch (By)
                {
                    case Enums.FINDBY.NAME:
                        {
                            _Element = Driver.FindElement(OpenQA.Selenium.By.Name(Locator));
                            break;
                        }
                    case Enums.FINDBY.ID:
                        {
                            _Element = Driver.FindElement(OpenQA.Selenium.By.Id(Locator));
                            break;
                        }
                    case Enums.FINDBY.CSSSELECTOR:
                        {
                            _Element = Driver.FindElement(OpenQA.Selenium.By.CssSelector(Locator));
                            break;
                        }
                    case Enums.FINDBY.XPATH:
                        {
                            _Element = Driver.FindElement(OpenQA.Selenium.By.XPath(Locator));
                            break;
                        }
                    case Enums.FINDBY.TAG:
                        {
                            _Element = Driver.FindElement(OpenQA.Selenium.By.TagName(Locator));
                            break;
                        }
                }

            }
            catch (NoSuchElementException)
            {
                throw;
            }

            return (_Element);
        }

        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            return (Driver.FindElements(by));
        }

        public ReadOnlyCollection<IWebElement> FindElements(Enums.FINDBY By, string Locator)
        {
            ReadOnlyCollection<IWebElement> elements = null;
            try
            {
                switch (By)
                {
                    case Enums.FINDBY.NAME:
                        {
                            elements = Driver.FindElements(OpenQA.Selenium.By.Name(Locator));
                            break;
                        }
                    case Enums.FINDBY.ID:
                        {
                            elements = Driver.FindElements(OpenQA.Selenium.By.Id(Locator));
                            break;
                        }
                    case Enums.FINDBY.CSSSELECTOR:
                        {
                            elements = Driver.FindElements(OpenQA.Selenium.By.CssSelector(Locator));
                            break;
                        }
                    case Enums.FINDBY.TAG:
                        {
                            elements = Driver.FindElements(OpenQA.Selenium.By.TagName(Locator));
                            break;
                        }
                }

            }
            catch (NoSuchElementException)
            {
                throw;
            }

            return (elements);
        }
        public void WaitFor(Enums.FINDBY By, string Locator, int WaitSeconds = 5)
        {
            WebDriverWait wait = new WebDriverWait(Driver, System.TimeSpan.FromSeconds(WaitSeconds));
            //set default timeout
            wait.Timeout = new TimeSpan(0,0,WaitSeconds);
            try
            {
                switch (By)
                {
                    case Enums.FINDBY.NAME:
                        {
                            IWebElement element = wait.Until(drv => {
                                var elem = drv.FindElement(OpenQA.Selenium.By.Name(Locator));
                                return (elem.Displayed && elem.Enabled) ? elem : null;
                            });
                            break;
                        }
                    case Enums.FINDBY.ID:
                        {
                            IWebElement element = wait.Until(drv => {
                                var elem = drv.FindElement(OpenQA.Selenium.By.Id(Locator));
                                return (elem.Displayed && elem.Enabled) ? elem : null;
                            });

                            break;
                        }
                    case Enums.FINDBY.CSSSELECTOR:
                        {
                        IWebElement element = wait.Until(drv => {
                            var elem = drv.FindElement(OpenQA.Selenium.By.CssSelector(Locator));
                            return (elem.Displayed && elem.Enabled) ? elem : null;
                        });
                        break;
                        }
                    case Enums.FINDBY.XPATH:
                        {
                        IWebElement element = wait.Until(drv => {
                            var elem = drv.FindElement(OpenQA.Selenium.By.XPath(Locator));
                            return (elem.Displayed && elem.Enabled) ? elem : null;
                        });
                        break;
                        }
                }

            }
            catch (NoSuchElementException)
            {
                throw;
            }
        }

        public void SwitchTo(int WindowHandle)
        {
            var windowHandles = Driver.WindowHandles;
            Driver.SwitchTo().Window(windowHandles[WindowHandle]);
        }

        public ITargetLocator SwitchTo()
        {
            return(Driver.SwitchTo());
        }

        public string GetPageSource()
        {
            return (Driver.PageSource);
        }

        public IOptions Manage()
        {
            return (Driver.Manage());
        }

        public void CloseTab(int TabNumber)
        {
            var windowHandles = Driver.WindowHandles;
            Driver.SwitchTo().Window(windowHandles[TabNumber]);
            Driver.Close();
        }

        public void Close()
        {
            Driver.Close();
            Driver.Quit();
            Driver.Dispose();
        }

        public void Quit()
        {
            Driver.Quit();
        }


        private void KillDriverProcess()
        {
            string processName = string.Empty;

            switch (_DriverType)
            {
                case DRIVERTYPE.CHROME:
                    {
                        processName = "Chrome.exe";
                        break;
                    }
                case DRIVERTYPE.EDGE: { break; }
                case DRIVERTYPE.FIREFOX: { break; }
            }
            foreach (Process proc in Process.GetProcessesByName(processName))
            {
                proc.Kill();
            }
        }

        public void Dispose()
        { 
            Driver.Dispose();
        }

    }
}
