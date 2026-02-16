using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;

namespace selenium_tineda_csharp.Pages
{
    /// <summary>
    /// Page Object para páginas de categorías
    /// Versión refactorizada sin logging en consola
    /// </summary>
    public class CategoryPage : BasePage
    {
        // ========== LOCATORS ==========
        private By _categoriesMenu = By.CssSelector("#top-menu .category");
        private By _categoryLinks = By.CssSelector("#top-menu .category a");
        private By _categoryTitle = By.CssSelector("h1.h1");
        private By _productsInCategory = By.CssSelector(".products .product-miniature");
        private By _productCount = By.CssSelector(".total-products");
        private By _breadcrumbs = By.CssSelector(".breadcrumb");
        private By _breadcrumbLinks = By.CssSelector(".breadcrumb li a");
        private By _breadcrumbActive = By.CssSelector(".breadcrumb li[aria-current='page']");

        public CategoryPage(IWebDriver driver) : base(driver) { }

        // ========== WAIT METHODS ==========

        /// <summary>
        /// Espera a que la página de categoría cargue completamente
        /// </summary>
        public void WaitForCategoryPageLoad()
        {
            WaitForPageLoad();
            
            try
            {
                WaitForElementVisible(_categoryTitle, 10);
            }
            catch
            {
                // Si no hay título, verificar URL de categoría
                if (!Driver.Url.Contains("id_category"))
                {
                    throw new InvalidOperationException("No se pudo cargar la página de categoría");
                }
            }
            
            WaitForAjaxComplete();
        }

        // ========== NAVEGACIÓN ==========

        /// <summary>
        /// Navega a una categoría específica por nombre
        /// </summary>
        public void GoToCategory(string categoryName)
        {
            WaitForMinimumElements(_categoryLinks, 1, 10);
            
            var links = Driver.FindElements(_categoryLinks);
            var categoryLink = links.FirstOrDefault(l => 
                l.Displayed && 
                l.Text.Trim().Equals(categoryName.Trim(), StringComparison.OrdinalIgnoreCase));
            
            if (categoryLink == null)
            {
                throw new InvalidOperationException($"Categoría no encontrada: {categoryName}");
            }

            ScrollToElement(By.LinkText(categoryName));
            System.Threading.Thread.Sleep(500);
            
            // Intentar clic normal, si falla usar JavaScript
            try
            {
                WaitForElementToBeClickable(By.LinkText(categoryName));
                categoryLink.Click();
            }
            catch
            {
                ClickWithJavaScript(By.LinkText(categoryName));
            }
            
            WaitForCategoryPageLoad();
        }

        /// <summary>
        /// Obtiene lista de categorías disponibles
        /// </summary>
        public List<string> GetAvailableCategories()
        {
            try
            {
                WaitForMinimumElements(_categoryLinks, 1, 5);
                
                var links = Driver.FindElements(_categoryLinks);

                return links
                    .Where(l => l.Displayed && !string.IsNullOrWhiteSpace(l.Text))
                    .Select(l => l.Text.Trim())
                    .Distinct()
                    .ToList();
            }
            catch
            {
                return new List<string>();
            }
        }

        // ========== INFORMACIÓN DE CATEGORÍA ==========

        /// <summary>
        /// Obtiene el título de la categoría actual
        /// </summary>
        public string GetCategoryTitle()
        {
            try
            {
                WaitForElementVisible(_categoryTitle, 5);
                return FindElement(_categoryTitle).Text;
            }
            catch
            {
                // Fallback: obtener de breadcrumb activo
                try
                {
                    var active = Driver.FindElement(_breadcrumbActive);
                    return active.Text;
                }
                catch
                {
                    return "Sin título";
                }
            }
        }

        /// <summary>
        /// Verifica si está en una página de categoría
        /// </summary>
        public bool IsOnCategoryPage()
        {
            return Driver.Url.Contains("id_category") || 
                   IsElementDisplayed(_categoryTitle, 5);
        }

        /// <summary>
        /// Obtiene la cantidad de productos en la categoría
        /// </summary>
        public int GetProductCount()
        {
            try
            {
                var products = Driver.FindElements(_productsInCategory);
                return products.Count;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Verifica si la categoría tiene productos
        /// </summary>
        public bool HasProducts()
        {
            return GetProductCount() > 0;
        }

        /// <summary>
        /// Hace clic en un producto por índice
        /// </summary>
        public void ClickProductByIndex(int index)
        {
            WaitForMinimumElements(_productsInCategory, index + 1, 10);
            var products = Driver.FindElements(_productsInCategory).ToList();
            
            if (index < 0 || index >= products.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), 
                    $"Índice fuera de rango. Índice: {index}, Total: {products.Count}");
            }

            var productElement = products[index];
            
            ScrollToElement(_productsInCategory);
            System.Threading.Thread.Sleep(500);
            
            try
            {
                WaitForElementToBeClickable(_productsInCategory);
                productElement.Click();
            }
            catch
            {
                ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", productElement);
            }
            
            WaitForPageLoad();
        }

        // ========== BREADCRUMBS ==========

        /// <summary>
        /// Verifica si los breadcrumbs están visibles
        /// </summary>
        public bool AreBreadcrumbsVisible()
        {
            return IsElementDisplayed(_breadcrumbs, 5);
        }

        /// <summary>
        /// Obtiene la lista de enlaces en breadcrumbs
        /// </summary>
        public List<string> GetBreadcrumbLinks()
        {
            var list = new List<string>();

            try
            {
                // Links clickeables
                var links = Driver.FindElements(_breadcrumbLinks);
                list.AddRange(links
                    .Select(l => l.Text)
                    .Where(t => !string.IsNullOrWhiteSpace(t)));

                // Breadcrumb actual (no es link)
                var current = Driver.FindElements(_breadcrumbActive);
                if (current.Count > 0)
                {
                    string currentText = current[0].Text;
                    if (!string.IsNullOrWhiteSpace(currentText) && !list.Contains(currentText))
                    {
                        list.Add(currentText);
                    }
                }
            }
            catch
            {
                // Retornar lista vacía en caso de error
            }

            return list;
        }

        /// <summary>
        /// Obtiene el breadcrumb activo (actual)
        /// </summary>
        public string GetActiveBreadcrumb()
        {
            try
            {
                WaitForElementVisible(_breadcrumbActive);
                return FindElement(_breadcrumbActive).Text;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Hace clic en un breadcrumb específico
        /// </summary>
        public void ClickBreadcrumb(string breadcrumbText)
        {
            WaitForMinimumElements(_breadcrumbLinks, 1);
            var links = Driver.FindElements(_breadcrumbLinks);
            var link = links.FirstOrDefault(l => 
                l.Text.Equals(breadcrumbText, StringComparison.OrdinalIgnoreCase));
            
            if (link == null)
            {
                throw new InvalidOperationException($"Breadcrumb no encontrado: {breadcrumbText}");
            }

            ScrollToElement(By.LinkText(breadcrumbText));
            System.Threading.Thread.Sleep(300);
            
            try
            {
                WaitForElementToBeClickable(By.LinkText(breadcrumbText));
                link.Click();
            }
            catch
            {
                ClickWithJavaScript(By.LinkText(breadcrumbText));
            }
            
            WaitForPageLoad();
        }

        /// <summary>
        /// Navega al home usando breadcrumbs
        /// </summary>
        public void GoToHomeViaBreadcrumb()
        {
            try
            {
                ClickBreadcrumb("Home");
            }
            catch
            {
                // Estrategia alternativa: usar el logo
                try
                {
                    var logo = Driver.FindElement(By.CssSelector("#_desktop_logo a, .logo a"));
                    logo.Click();
                    WaitForPageLoad();
                }
                catch
                {
                    // Última opción: navegación directa
                    Driver.Navigate().GoToUrl("https://teststore.automationtesting.co.uk/index.php");
                    WaitForPageLoad();
                }
            }
        }

        /// <summary>
        /// Verifica si los breadcrumbs contienen un texto específico
        /// </summary>
        public bool BreadcrumbContains(string categoryName)
        {
            var breadcrumbs = GetBreadcrumbLinks();
            return breadcrumbs.Any(b => 
                b.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Obtiene la cantidad de elementos en breadcrumbs
        /// </summary>
        public int GetBreadcrumbCount()
        {
            try
            {
                var items = Driver.FindElements(By.CssSelector(".breadcrumb li"));
                return items.Count;
            }
            catch
            {
                return 0;
            }
        }
    }
}