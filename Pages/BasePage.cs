using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace selenium_tineda_csharp.Pages
{
    public class BasePage
    {
        protected IWebDriver Driver;
        protected WebDriverWait Wait;
        protected WebDriverWait ShortWait;
        protected WebDriverWait LongWait;

        public BasePage(IWebDriver driver)
        {
            Driver = driver;
            Wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            ShortWait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            LongWait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
        }

        // ========== MÉTODOS BÁSICOS ==========

        protected IWebElement FindElement(By locator)
        {
            return Wait.Until(ExpectedConditions.ElementIsVisible(locator));
        }

        protected void Click(By locator)
        {
            WaitForElementToBeClickable(locator).Click();
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

        // ========== MÉTODOS DE WAIT MEJORADOS ==========

        /// <summary>
        /// Espera a que un elemento sea visible
        /// </summary>
        protected IWebElement WaitForElementVisible(By locator, int timeoutSeconds = 10)
        {
            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutSeconds));
            return wait.Until(ExpectedConditions.ElementIsVisible(locator));
        }

        /// <summary>
        /// Espera a que un elemento sea clickeable
        /// </summary>
        protected IWebElement WaitForElementToBeClickable(By locator, int timeoutSeconds = 10)
        {
            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutSeconds));
            return wait.Until(ExpectedConditions.ElementToBeClickable(locator));
        }

        /// <summary>
        /// Espera a que un elemento exista en el DOM (no necesariamente visible)
        /// </summary>
        protected IWebElement WaitForElementExists(By locator, int timeoutSeconds = 10)
        {
            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutSeconds));
            return wait.Until(ExpectedConditions.ElementExists(locator));
        }

        /// <summary>
        /// Espera a que un elemento desaparezca
        /// </summary>
        protected bool WaitForElementToDisappear(By locator, int timeoutSeconds = 10)
        {
            try
            {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutSeconds));
                return wait.Until(ExpectedConditions.InvisibilityOfElementLocated(locator));
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Espera a que la URL contenga un texto específico
        /// </summary>
        protected bool WaitForUrlContains(string urlPart, int timeoutSeconds = 10)
        {
            try
            {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutSeconds));
                return wait.Until(ExpectedConditions.UrlContains(urlPart));
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Espera a que el texto de un elemento cambie
        /// </summary>
        protected bool WaitForTextToChange(By locator, string oldText, int timeoutSeconds = 10)
        {
            try
            {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutSeconds));
                return wait.Until(driver =>
                {
                    try
                    {
                        var element = driver.FindElement(locator);
                        return !element.Text.Equals(oldText);
                    }
                    catch
                    {
                        return false;
                    }
                });
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Espera a que un atributo DOM de elemento tenga un valor específico
        /// </summary>
        protected bool WaitForAttributeValue(By locator, string attribute, string value, int timeoutSeconds = 10)
        {
            try
            {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutSeconds));
                return wait.Until(driver =>
                {
                    try
                    {
                        var element = driver.FindElement(locator);
                        return element.GetDomAttribute(attribute)?.Equals(value) ?? false;
                    }
                    catch
                    {
                        return false;
                    }
                });
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Espera a que una propiedad DOM de elemento tenga un valor específico
        /// </summary>
        protected bool WaitForPropertyValue(By locator, string property, string value, int timeoutSeconds = 10)
        {
            try
            {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutSeconds));
                return wait.Until(driver =>
                {
                    try
                    {
                        var element = driver.FindElement(locator);
                        return element.GetDomProperty(property)?.Equals(value) ?? false;
                    }
                    catch
                    {
                        return false;
                    }
                });
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Espera a que una colección de elementos tenga un tamaño específico
        /// </summary>
        protected bool WaitForElementCount(By locator, int expectedCount, int timeoutSeconds = 10)
        {
            try
            {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutSeconds));
                return wait.Until(driver =>
                {
                    try
                    {
                        var elements = driver.FindElements(locator);
                        return elements.Count == expectedCount;
                    }
                    catch
                    {
                        return false;
                    }
                });
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Espera a que haya al menos N elementos
        /// </summary>
        protected bool WaitForMinimumElements(By locator, int minimumCount, int timeoutSeconds = 10)
        {
            try
            {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutSeconds));
                return wait.Until(driver =>
                {
                    try
                    {
                        var elements = driver.FindElements(locator);
                        return elements.Count >= minimumCount;
                    }
                    catch
                    {
                        return false;
                    }
                });
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Espera a que la página termine de cargar completamente
        /// </summary>
        protected bool WaitForPageLoad(int timeoutSeconds = 30)
        {
            try
            {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutSeconds));
                return wait.Until(driver =>
                {
                    return ((IJavaScriptExecutor)driver)
                        .ExecuteScript("return document.readyState")
                        .Equals("complete");
                });
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Espera a que no haya peticiones AJAX activas (para sitios con jQuery)
        /// </summary>
        protected bool WaitForAjaxComplete(int timeoutSeconds = 10)
        {
            try
            {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutSeconds));
                return wait.Until(driver =>
                {
                    try
                    {
                        var ajaxComplete = (bool)((IJavaScriptExecutor)driver)
                            .ExecuteScript("return jQuery.active == 0");
                        return ajaxComplete;
                    }
                    catch
                    {
                        // Si jQuery no está disponible, asumimos que no hay AJAX
                        return true;
                    }
                });
            }
            catch
            {
                return true; // Si falla, continuamos
            }
        }

        /// <summary>
        /// Espera a que un elemento sea estable (no cambie de posición)
        /// Útil para elementos que se mueven durante animaciones
        /// </summary>
        protected bool WaitForElementToBeStable(By locator, int timeoutSeconds = 10)
        {
            try
            {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutSeconds));
                System.Drawing.Point? previousLocation = null;
                
                return wait.Until(driver =>
                {
                    try
                    {
                        var element = driver.FindElement(locator);
                        var currentLocation = element.Location;
                        
                        if (previousLocation == null)
                        {
                            previousLocation = currentLocation;
                            System.Threading.Thread.Sleep(100); // Pequeña pausa para comparar
                            return false;
                        }
                        
                        bool isStable = previousLocation.Value.Equals(currentLocation);
                        previousLocation = currentLocation;
                        return isStable;
                    }
                    catch
                    {
                        return false;
                    }
                });
            }
            catch
            {
                return false;
            }
        }

        // ========== MÉTODO LEGACY (mantener compatibilidad) ==========

        protected bool IsElementDisplayed(By locator, int timeoutSeconds = 5)
        {
            try
            {
                WaitForElementVisible(locator, timeoutSeconds);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // ========== MÉTODOS DE UTILIDAD ==========

        /// <summary>
        /// Obtiene todos los elementos que coinciden con el locator
        /// </summary>
        protected IReadOnlyCollection<IWebElement> FindElements(By locator)
        {
            WaitForMinimumElements(locator, 1, 5);
            return Driver.FindElements(locator);
        }

        /// <summary>
        /// Hace scroll a un elemento para que sea visible
        /// </summary>
        protected void ScrollToElement(By locator)
        {
            var element = FindElement(locator);
            ((IJavaScriptExecutor)Driver).ExecuteScript(
                "arguments[0].scrollIntoView({behavior: 'smooth', block: 'center'});", 
                element
            );
            // Pequeña pausa para que termine la animación del scroll
            System.Threading.Thread.Sleep(300);
        }

        /// <summary>
        /// Hace clic usando JavaScript (útil cuando el click normal falla)
        /// </summary>
        protected void ClickWithJavaScript(By locator)
        {
            var element = FindElement(locator);
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", element);
        }
    }
}