using BoDi;
using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions;
using System.Diagnostics;
using UITest.TestDriver;

namespace SpecFlowProjectBDD.Hooks
{
    /// <summary>
    /// Hooks for scenario and test setup/teardown. Currently only uses Chrome and will need to be modified for other browser types
    /// </summary>
    [Binding]
    public class SeleniumSpecFlowHooks
    {
        private int driverProcessPID;
        private Process[] driverProcessess;
        protected IObjectContainer _Container;

        public SeleniumSpecFlowHooks(IObjectContainer container)
        {
            _Container = container;
        }

        [BeforeScenario(Order = 3)]
        public void SetupDrivers()
        {
            CleanupDrivers();
            SeleniumDriver webDriver = new SeleniumDriver(SeleniumDriver.DRIVERTYPE.CHROME, Headless: false);
            webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

            _Container.RegisterInstanceAs<SeleniumDriver>(webDriver);
        }

        [AfterScenario(Order = 3)]
        public void DestroyDrivers()
        {
            SeleniumDriver? webDriver = null;
            try
            {
               webDriver = _Container.Resolve<SeleniumDriver>();
            }
            catch (BoDi.ObjectContainerException ex)
            {
                CleanupDrivers();
            }

            if (webDriver == null) 
                return;

            webDriver.Quit();
            webDriver.Dispose();
        }

        private void CleanupDrivers()
        {
            //check for any Chromedrivers running as a disconnected process rather than under VS. Having running drivers will cause the current test to fail
            driverProcessess = Process.GetProcessesByName("chromedriver");

            if (driverProcessess.Length != 0)
            {
                //cleanup old drivers
                foreach (var driverProcess in driverProcessess)
                {
                    driverProcess.Kill();
                }
            }
        }


    }
}
