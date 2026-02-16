using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

namespace SeleniumFramework.Drivers
{
    /// <summary>
    /// Administrador de WebDriver mejorado
    /// - Sin ubicación hardcodeada del driver (Selenium 4+ lo maneja automáticamente)
    /// - Soporte para diferentes navegadores
    /// - Configuración centralizada
    /// </summary>
    public static class DriverManager
    {
        [ThreadStatic]
        private static IWebDriver? _driver;

        /// <summary>
        /// Obtiene una instancia del WebDriver
        /// Selenium 4+ automáticamente descarga y gestiona el driver
        /// </summary>
        public static IWebDriver GetDriver(BrowserType browserType = BrowserType.Chrome)
        {
            if (_driver == null)
            {
                _driver = CreateDriver(browserType);
            }
            return _driver;
        }

        /// <summary>
        /// Crea una nueva instancia del WebDriver según el navegador especificado
        /// </summary>
        private static IWebDriver CreateDriver(BrowserType browserType)
        {
            IWebDriver driver;

            switch (browserType)
            {
                case BrowserType.Chrome:
                    driver = CreateChromeDriver();
                    break;
                
                case BrowserType.ChromeHeadless:
                    driver = CreateChromeHeadlessDriver();
                    break;
                
                // Puedes agregar más navegadores aquí
                // case BrowserType.Firefox:
                //     driver = CreateFirefoxDriver();
                //     break;
                
                default:
                    throw new ArgumentException($"Navegador no soportado: {browserType}");
            }

            ConfigureDriver(driver);
            return driver;
        }

        /// <summary>
        /// Crea un Chrome Driver con opciones básicas
        /// </summary>
        private static IWebDriver CreateChromeDriver()
        {
            var options = new ChromeOptions();
            
            // Opciones recomendadas para mayor estabilidad
            options.AddArgument("--start-maximized");
            options.AddArgument("--disable-blink-features=AutomationControlled");
            options.AddExcludedArgument("enable-automation");
            options.AddUserProfilePreference("credentials_enable_service", false);
            options.AddUserProfilePreference("profile.password_manager_enabled", false);
            
            // Selenium 4+ automáticamente gestiona el ChromeDriver
            return new ChromeDriver(options);
        }

        /// <summary>
        /// Crea un Chrome Driver en modo headless (sin interfaz gráfica)
        /// </summary>
        private static IWebDriver CreateChromeHeadlessDriver()
        {
            var options = new ChromeOptions();
            
            options.AddArgument("--headless=new"); // Nuevo modo headless mejorado
            options.AddArgument("--window-size=1920,1080");
            options.AddArgument("--disable-blink-features=AutomationControlled");
            options.AddExcludedArgument("enable-automation");
            
            return new ChromeDriver(options);
        }

        /// <summary>
        /// Configuración común para todos los drivers
        /// </summary>
        private static void ConfigureDriver(IWebDriver driver)
        {
            // Timeouts implícitos
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(30);
            
            // Maximizar ventana si no es headless
            try
            {
                driver.Manage().Window.Maximize();
            }
            catch
            {
                // En modo headless puede fallar, pero no es crítico
            }
        }

        /// <summary>
        /// Cierra y limpia el WebDriver
        /// </summary>
        public static void QuitDriver()
        {
            if (_driver != null)
            {
                try
                {
                    _driver.Quit();
                }
                catch (Exception)
                {
                    // Ignorar errores al cerrar
                }
                finally
                {
                    _driver = null;
                }
            }
        }

        /// <summary>
        /// Obtiene el driver actual sin crear uno nuevo
        /// </summary>
        public static IWebDriver? GetCurrentDriver()
        {
            return _driver;
        }
    }

    /// <summary>
    /// Tipos de navegadores soportados
    /// </summary>
    public enum BrowserType
    {
        Chrome,
        ChromeHeadless,
        Firefox,
        Edge
    }
}