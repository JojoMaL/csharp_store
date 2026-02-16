using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;
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
                // Automatically download and setup the correct ChromeDriver version
                new WebDriverManager.DriverManager().SetUpDriver(
                    new ChromeConfig(), 
                    VersionResolveStrategy.MatchingBrowser
                );
                
                var chromeOptions = new ChromeOptions();
                chromeOptions.AddArgument("--start-maximized");
                chromeOptions.AddArgument("--disable-search-engine-choice-screen");
                
                _driver = new ChromeDriver(chromeOptions);
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
