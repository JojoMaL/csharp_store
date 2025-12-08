using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;

namespace selenium_tineda_csharp.Pages
{
    public class BasePage
    {
        protected IWebDriver Driver;
        protected WebDriverWait Wait;

        public BasePage(IWebDriver driver)
        {
            Driver = driver;
            Wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        }

        protected IWebElement FindElement(By locator)
        {
            return Wait.Until(ExpectedConditions.ElementIsVisible(locator));
        }

        protected void Click(By locator)
        {
            FindElement(locator).Click();
        }

        protected void Type(By locator, string text)
        {
            var element = FindElement(locator);
            element.Clear();
            element.SendKeys(text);
        }

        protected void NavigateTo(string url)
        {
            Driver.Navigate().GoToUrl(url);
        }

        public string GetPageTitle()
        {
            return Driver.Title;
        }

        protected bool IsElementDisplayed(By locator, int timeoutSeconds = 5)
        {
            try
            {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutSeconds));
                wait.Until(ExpectedConditions.ElementIsVisible(locator));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
