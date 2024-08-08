using BoDi;
using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions;
using System.Diagnostics;
using UITest.TestDriver;

namespace SpecFlowProjectBDD.Hooks
{
    [Binding]
    public class SeleniumSpecFlowHooks
    {
        private int driverProcessPID;
        private Process[] driverProcessess1;
        private Process[] driverProcessess2;
        protected IObjectContainer _Container;

        public SeleniumSpecFlowHooks(IObjectContainer container)
        {
            _Container = container;
        }

        [BeforeScenario(Order = 3)]
        public void SetupDrivers()
        {
            //check for any Chromedrivers running as a disconnected process rather than under VS. Having running drivers will cause the current test to fail
            driverProcessess1 = Process.GetProcessesByName("chromedriver.exe");

            if (driverProcessess1.Length != 0)
            {
                //cleanup old drivers
                foreach (var driverProcess in driverProcessess1)
                {
                    driverProcess.Kill();
                }
            }

            SeleniumDriver webDriver = new SeleniumDriver(SeleniumDriver.DRIVERTYPE.CHROME);
            webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

            _Container.RegisterInstanceAs<SeleniumDriver>(webDriver);
        }

        [AfterScenario(Order = 3)]
        public void DestroyDrivers()
        {
            var webDriver = _Container.Resolve<SeleniumDriver>();

            if (webDriver == null) 
                return;

            webDriver.Quit();
            webDriver.Dispose();
        }

    }
}
