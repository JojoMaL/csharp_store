using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;

namespace selenium_tineda_csharp.Pages
{
    public class CategoryPage : BasePage
    {
        // Locators
        private By _categoriesMenu = By.CssSelector("#top-menu .category");
        private By _categoryLinks = By.CssSelector("#top-menu .category a");
        private By _categoryTitle = By.CssSelector("h1.h1");
        private By _productsInCategory = By.CssSelector(".products .product-miniature");
        private By _productCount = By.CssSelector(".total-products");
        private By _breadcrumbs = By.CssSelector(".breadcrumb");
        private By _breadcrumbLinks = By.CssSelector(".breadcrumb li a");
        private By _breadcrumbActive = By.CssSelector(".breadcrumb li[aria-current='page']");

        public CategoryPage(IWebDriver driver) : base(driver) { }

        // ========== MÉTODOS DE WAIT ==========

        public void WaitForCategoryPageLoad()
        {
            WaitForPageLoad();
            
            try
            {
                WaitForElementVisible(_categoryTitle, 10);
            }
            catch
            {
                // Si no hay título, verificar que al menos hay URL de categoría
                if (!Driver.Url.Contains("id_category"))
                {
                    throw new Exception("No se pudo cargar la página de categoría");
                }
            }
            
            WaitForAjaxComplete();
        }

        // ========== NAVEGACIÓN ==========

        public void GoToCategory(string categoryName)
        {
            try
            {
                // Primero verificar que el menú esté visible
                WaitForMinimumElements(_categoryLinks, 1, 10);
                
                var links = Driver.FindElements(_categoryLinks);
                var categoryLink = links.FirstOrDefault(l => 
                    l.Displayed && 
                    l.Text.Trim().Equals(categoryName.Trim(), StringComparison.OrdinalIgnoreCase));
                
                if (categoryLink != null)
                {
                    // Hacer scroll al elemento
                    ((IJavaScriptExecutor)Driver).ExecuteScript(
                        "arguments[0].scrollIntoView({behavior: 'smooth', block: 'center'});", 
                        categoryLink
                    );
                    
                    System.Threading.Thread.Sleep(500);
                    
                    // Intentar clic normal primero
                    try
                    {
                        WaitForElementToBeClickable(By.LinkText(categoryName));
                        categoryLink.Click();
                    }
                    catch
                    {
                        // Si falla, usar JavaScript
                        ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", categoryLink);
                    }
                    
                    WaitForCategoryPageLoad();
                }
                else
                {
                    throw new Exception($"No se encontró la categoría visible: {categoryName}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error al navegar a categoría '{categoryName}': {ex.Message}");
                throw;
            }
        }

        public List<string> GetAvailableCategories()
        {
            try
            {
                // Esperar que al menos haya un link
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

        public string GetCategoryTitle()
        {
            try
            {
                WaitForElementVisible(_categoryTitle, 5);
                return FindElement(_categoryTitle).Text;
            }
            catch
            {
                // Si no hay título H1, intentar obtener de breadcrumb activo
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

        public bool IsOnCategoryPage()
        {
            return Driver.Url.Contains("id_category") || 
                   IsElementDisplayed(_categoryTitle, 5);
        }

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

        public bool HasProducts()
        {
            return GetProductCount() > 0;
        }

        public void ClickProductByIndex(int index)
        {
            try
            {
                WaitForMinimumElements(_productsInCategory, index + 1, 10);
                var products = Driver.FindElements(_productsInCategory).ToList();
                
                if (index >= 0 && index < products.Count)
                {
                    var productElement = products[index];
                    
                    // Scroll al producto
                    ((IJavaScriptExecutor)Driver).ExecuteScript(
                        "arguments[0].scrollIntoView({behavior: 'smooth', block: 'center'});", 
                        productElement
                    );
                    
                    System.Threading.Thread.Sleep(500);
                    
                    // Intentar clic
                    try
                    {
                        WaitForElementToBeClickable(_productsInCategory);
                        productElement.Click();
                    }
                    catch
                    {
                        // Usar JavaScript si falla
                        ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", productElement);
                    }
                    
                    WaitForPageLoad();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error al hacer clic en producto {index}: {ex.Message}");
                throw;
            }
        }

        // ========== BREADCRUMBS ==========

        public bool AreBreadcrumbsVisible()
        {
            return IsElementDisplayed(_breadcrumbs, 5);
        }

        public List<string> GetBreadcrumbLinks()
        {
            List<string> list = new List<string>();

            try
            {
                // Links clickeables
                var links = Driver.FindElements(_breadcrumbLinks);
                list.AddRange(links.Select(l => l.Text).Where(t => !string.IsNullOrWhiteSpace(t)));

                // Breadcrumb actual (no es link)
                try
                {
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
                catch { }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error al obtener breadcrumbs: {ex.Message}");
            }

            return list;
        }

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

        public void ClickBreadcrumb(string breadcrumbText)
        {
            try
            {
                WaitForMinimumElements(_breadcrumbLinks, 1);
                var links = Driver.FindElements(_breadcrumbLinks);
                var link = links.FirstOrDefault(l => 
                    l.Text.Equals(breadcrumbText, StringComparison.OrdinalIgnoreCase));
                
                if (link != null)
                {
                    // Scroll al breadcrumb
                    ((IJavaScriptExecutor)Driver).ExecuteScript(
                        "arguments[0].scrollIntoView(true);", 
                        link
                    );
                    
                    System.Threading.Thread.Sleep(300);
                    
                    // Intentar clic
                    try
                    {
                        WaitForElementToBeClickable(By.LinkText(breadcrumbText));
                        link.Click();
                    }
                    catch
                    {
                        ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", link);
                    }
                    
                    WaitForPageLoad();
                }
                else
                {
                    throw new Exception($"No se encontró el breadcrumb: {breadcrumbText}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error al hacer clic en breadcrumb: {ex.Message}");
                throw;
            }
        }

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

        public bool BreadcrumbContains(string categoryName)
        {
            var breadcrumbs = GetBreadcrumbLinks();
            return breadcrumbs.Any(b => 
                b.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
        }

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