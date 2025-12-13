using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;

namespace selenium_tineda_csharp.Pages
{
    public class SearchPage : BasePage
    {
        // Locators - Múltiples opciones para mayor robustez
        private By _searchInput = By.CssSelector("input[name='s']");
        private By _searchButton = By.CssSelector("button[type='submit']");

        private By _searchResultsContainer = By.Id("js-product-list");
        private By _searchResults = By.CssSelector("#js-product-list .product-miniature");

        private By _noResultsMessage = By.CssSelector("#js-product-list p.alert");
        private By _productNames = By.CssSelector("#js-product-list .product-title a");


        public SearchPage(IWebDriver driver) : base(driver) { }

        /// <summary>
        /// Realiza una búsqueda escribiendo en el campo de búsqueda
        /// Prueba múltiples estrategias para mayor compatibilidad
        /// </summary>
        public void Search(string searchTerm)
        {
            try
            {
                // Estrategia 1: Usar el campo de búsqueda y botón
                try
                {
                    Type(_searchInput, searchTerm);
                    Click(_searchButton);
                    return;
                }
                catch
                {
                    // Si falla, intentar otras estrategias
                }

                // Estrategia 2: Enviar Enter directamente en el campo
                try
                {
                    var input = FindElement(_searchInput);
                    input.Clear();
                    input.SendKeys(searchTerm);
                    input.SendKeys(Keys.Enter);
                    return;
                }
                catch
                {
                    // Si falla, intentar siguiente estrategia
                }

                // Estrategia 3: Navegar directamente a la URL de búsqueda
                string currentUrl = Driver.Url;
                string baseUrl = new System.Uri(currentUrl).GetLeftPart(System.UriPartial.Authority);
                string searchUrl = $"{baseUrl}/index.php?controller=search&s={System.Uri.EscapeDataString(searchTerm)}";
                Driver.Navigate().GoToUrl(searchUrl);
            }
            catch (System.Exception ex)
            {
                throw new System.Exception($"No se pudo realizar la búsqueda. Error: {ex.Message}");
            }
        }

        // ========== MÉTODOS DE WAIT ESPECÍFICOS ==========

        /// <summary>
        /// Espera a que aparezcan resultados de búsqueda
        /// </summary>
        public void WaitForSearchResults(int timeoutSeconds = 10)
        {
            // Esperamos que aparezcan productos O el mensaje de sin resultados
            try
            {
                WaitForMinimumElements(_searchResults,1, timeoutSeconds);
            }
            catch
            {
                // Si no hay productos, verificamos si hay mensaje de sin resultados
                WaitForElementVisible(_noResultsMessage, timeoutSeconds);
            }
        }

        /// <summary>
        /// Espera a que la búsqueda se complete (productos o mensaje)
        /// </summary>
        public void WaitForSearchComplete(int timeoutSeconds = 10)
        {
            // Esperamos que la URL cambie a search O que aparezca contenido
            if (WaitForUrlContains("search", 3) || WaitForUrlContains("controller=search", 3))
            {
                WaitForPageLoad();
            }
            
            // Luego esperamos resultados o mensaje
            try
            {
                WaitForMinimumElements(_searchResults, 1, timeoutSeconds);
            }
            catch
            {
                // Intentamos esperar el mensaje de sin resultados
                try
                {
                    WaitForElementVisible(_noResultsMessage, 3);
                }
                catch
                {
                    // Si no hay ni productos ni mensaje, está OK
                }
            }
        }

        // ========== MÉTODOS DE VERIFICACIÓN ==========

        /// <summary>
        /// Verifica si hay resultados de búsqueda
        /// </summary>
        public bool HasSearchResults()
        {
            
                return Driver.FindElements(_searchResults).Count>0;
        }

        /// <summary>
        /// Obtiene la cantidad de resultados encontrados
        /// </summary>
        public int GetSearchResultsCount()
        {
            try
            {
                var results = Driver.FindElements(_searchResults);
                return results.Count;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Verifica si se muestra el mensaje de "sin resultados"
        /// </summary>
        public bool IsNoResultsMessageDisplayed()
        {
            return IsElementDisplayed(_noResultsMessage, 5);
        }

        /// <summary>
        /// Obtiene el texto del mensaje de "sin resultados"
        /// </summary>
        public string GetNoResultsMessageText()
        {
            if (IsNoResultsMessageDisplayed())
            {
                return FindElement(_noResultsMessage).Text;
            }
            return string.Empty;
        }

        /// <summary>
        /// Obtiene los nombres de todos los productos en los resultados
        /// </summary>
        public List<string> GetProductNames()
        {
                var elements = Driver.FindElements(_productNames);
                return elements.Select(e => e.Text.ToLower()).ToList();
        }

        /// <summary>
        /// Verifica si todos los resultados contienen el término de búsqueda
        /// </summary>
        public bool AllResultsContainSearchTerm(string searchTerm)
        {
            var productNames = GetProductNames();
            if (productNames.Count == 0)
                return false;

            var lowerSearchTerm = searchTerm.ToLower();
            return productNames.All(name => name.Contains(lowerSearchTerm));
        }

        /// <summary>
        /// Hace clic en el primer resultado de búsqueda
        /// </summary>
        public void ClickFirstResult()
        {
            WaitForMinimumElements(_searchResults, 1);
            var results = Driver.FindElements(_searchResults);
            if (results.Count > 0)
            {
                // Esperamos que el elemento sea clickeable
                WaitForElementToBeClickable(_searchResults);
                results[0].Click();
            }
        }

        /// <summary>
        /// Verifica si el campo de búsqueda está visible
        /// </summary>
        public bool IsSearchInputVisible()
        {
            return IsElementDisplayed(_searchInput, 3);
        }
    }
}