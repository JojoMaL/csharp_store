using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

namespace SeleniumFramework.Drivers
{
    public class DriverManager
    {
        private static IWebDriver? _driver;

        public static IWebDriver GetDriver()
        {
            if (_driver == null)
            {
                var chromeOptions = new ChromeOptions();
                chromeOptions.AddArgument("--start-maximized");
                chromeOptions.AddArgument("--disable-search-engine-choice-screen");
                
                // Especificar la ruta del ChromeDriver
                var service = ChromeDriverService.CreateDefaultService("/usr/local/bin");
                
                _driver = new ChromeDriver(service, chromeOptions);
                _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            }

            return _driver;
        }

        public static void QuitDriver()
        {
            if (_driver != null)
            {
                _driver.Quit();
                _driver.Dispose();
                _driver = null;
            }
        }
    }
}