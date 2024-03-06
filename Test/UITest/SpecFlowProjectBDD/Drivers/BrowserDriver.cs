using OpenQA.Selenium.Chrome;
//using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UITest.TestDriver;

namespace SpecFlowProjectBDD.Drivers
{
    public class BrowserDriver : IDisposable
    {
        private readonly Lazy<IDriver> _currentWebDriverLazy;
        private bool _isDisposed;

        public BrowserDriver()
        {
            _currentWebDriverLazy = new Lazy<IDriver>(CreateWebDriver);
        }

        /// <summary>
        /// The Selenium IWebDriver instance
        /// </summary>
        public IDriver Current => _currentWebDriverLazy.Value;

        /// <summary>
        /// Creates the Selenium web driver (opens a browser)
        /// </summary>
        /// <returns></returns>
        private IDriver CreateWebDriver()
        {
            //We use the Chrome browser
            //var chromeDriverService = ChromeDriverService.CreateDefaultService();

            //var chromeOptions = new ChromeOptions();

            var IDriver = new SeleniumDriver(SeleniumDriver.DRIVERTYPE.CHROME);

            return IDriver;
        }

        /// <summary>
        /// Disposes the Selenium web driver (closing the browser) after the Scenario completed
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            if (_currentWebDriverLazy.IsValueCreated)
            {
                Current.Quit();
            }

            _isDisposed = true;
        }
    }
}
