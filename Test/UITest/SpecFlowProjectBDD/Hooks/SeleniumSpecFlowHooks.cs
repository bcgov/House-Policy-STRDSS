using BoDi;
using UITest.TestDriver;

namespace SpecFlowProjectBDD.Hooks
{
    [Binding]
    public class SeleniumSpecFlowHooks
    {
        protected IObjectContainer _Container;

        public SeleniumSpecFlowHooks(IObjectContainer container)
        {
            _Container = container;
        }

        [BeforeScenario(Order = 3)]
        public void SetupDrivers()
        {
            SeleniumDriver webDriver = new SeleniumDriver(SeleniumDriver.DRIVERTYPE.CHROME);
            webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

            _Container.RegisterInstanceAs<SeleniumDriver>(webDriver);
        }

        [AfterScenario()]
        public void DestroyDrivers()
        {
            var webDriver = _Container.Resolve<SeleniumDriver>();

            if (webDriver == null) return;

            webDriver.Close();
            webDriver.Dispose();
        }

    }
}
