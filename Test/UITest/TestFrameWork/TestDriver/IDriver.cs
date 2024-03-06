using System.Collections.ObjectModel;
using OpenQA.Selenium;

namespace UITest.TestDriver
{
    public interface IDriver: IWebDriver
    {

        IWebElement FindElement(Enums.FINDBY By, string Locator);
        ReadOnlyCollection<IWebElement> FindElements(Enums.FINDBY By, string Locator);
        bool Navigate(string URL);

        void WaitFor(Enums.FINDBY By, string Locator, int Seconds);


        void CloseTab(int TabNumber);

        string GetCurrentURL();

        IWebDriver Driver { get; set; }
    }
}
