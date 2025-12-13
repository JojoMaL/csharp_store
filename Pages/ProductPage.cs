using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace selenium_tineda_csharp.Pages
{
    public class ProductPage : BasePage
    {
        // Locators
        private By _sizeDropdown = By.Id("group_1");
        private By _addToCartButton = By.CssSelector(".add-to-cart");
        private By _successModal = By.Id("blockcart-modal");
        private By _proceedToCheckoutButton = By.CssSelector(".cart-content-btn .btn-primary");
        private By _continueShoppingButton = By.CssSelector(".modal-footer button.btn-secondary");
        private By _productName = By.CssSelector("h1[itemprop='name']");
        private By _productPrice = By.CssSelector(".current-price span[itemprop='price']");
        private By _productDescription = By.CssSelector("#description .product-description");
        private By _productImage = By.CssSelector(".js-qv-product-cover img");
        private By _quantityInput = By.Id("quantity_wanted");
        private By _incrementButton = By.CssSelector(".bootstrap-touchspin-up");
        private By _decrementButton = By.CssSelector(".bootstrap-touchspin-down");
        private By _colorOptions = By.CssSelector(".product-variants input[type='radio']");
        private By _colorLabels = By.CssSelector(".product-variants label");
        private By _breadcrumbs = By.CssSelector(".breadcrumb");
        private By _breadcrumbLinks = By.CssSelector(".breadcrumb li a");

        public ProductPage(IWebDriver driver) : base(driver) { }

        // ========== MÉTODOS DE WAIT ESPECÍFICOS ==========

        /// <summary>
        /// Espera a que la página del producto cargue completamente
        /// </summary>
        public void WaitForProductPageLoad()
        {
            WaitForUrlContains("id_product");
            WaitForElementVisible(_productName);
            WaitForPageLoad();
        }

        /// <summary>
        /// Espera a que el modal de éxito aparezca
        /// </summary>
        public void WaitForSuccessModal()
        {
            WaitForElementVisible(_successModal, 10);
        }

        /// <summary>
        /// Espera a que el modal desaparezca
        /// </summary>
        public void WaitForModalToDisappear()
        {
            WaitForElementToDisappear(_successModal, 10);
        }

        /// <summary>
        /// Espera a que la cantidad se actualice
        /// </summary>
        public void WaitForQuantityUpdate(int expectedValue)
        {
            WaitForPropertyValue(_quantityInput, "value", expectedValue.ToString(), 5);
        }

        // ========== MÉTODOS PRINCIPALES ==========

        public void SelectSize(string size)
        {
            WaitForElementVisible(_sizeDropdown);
            var dropdown = new SelectElement(FindElement(_sizeDropdown));
            dropdown.SelectByText(size);
            // Pequeña espera para que el sitio procese el cambio
            WaitForAjaxComplete();
        }

        public void AddToCart()
        {
            WaitForElementToBeClickable(_addToCartButton);
            Click(_addToCartButton);
        }

        public bool IsSuccessModalDisplayed()
        {
            return IsElementDisplayed(_successModal, 10);
        }

        public void ProceedToCheckout()
        {
            if (IsSuccessModalDisplayed())
            {
                WaitForElementToBeClickable(_proceedToCheckoutButton);
                Click(_proceedToCheckoutButton);
            }
        }

        public void ContinueShopping()
        {
            if (IsSuccessModalDisplayed())
            {
                WaitForElementToBeClickable(_continueShoppingButton);
                Click(_continueShoppingButton);
                WaitForModalToDisappear();
            }
        }

        // ========== INFORMACIÓN DEL PRODUCTO ==========

        public string GetProductName()
        {
            WaitForElementVisible(_productName);
            return FindElement(_productName).Text;
        }

        public string GetProductPrice()
        {
            WaitForElementVisible(_productPrice);
            return FindElement(_productPrice).Text;
        }

        public bool IsProductDescriptionVisible()
        {
            return IsElementDisplayed(_productDescription, 5);
        }

        public string GetProductDescription()
        {
            if (IsProductDescriptionVisible())
            {
                return FindElement(_productDescription).Text;
            }
            return string.Empty;
        }

        public bool IsProductImageVisible()
        {
            return IsElementDisplayed(_productImage, 5);
        }

        // ========== TALLAS ==========

        public List<string> GetAvailableSizes()
        {
            try
            {
                WaitForElementVisible(_sizeDropdown);
                var dropdown = new SelectElement(FindElement(_sizeDropdown));
                return dropdown.Options
                    .Where(o => !string.IsNullOrWhiteSpace(o.Text))
                    .Select(o => o.Text)
                    .ToList();
            }
            catch
            {
                return new List<string>();
            }
        }

        public string GetSelectedSize()
        {
            try
            {
                var dropdown = new SelectElement(FindElement(_sizeDropdown));
                return dropdown.SelectedOption.Text;
            }
            catch
            {
                return string.Empty;
            }
        }

        // ========== COLORES ==========

        public bool HasColorOptions()
        {
            try
            {
                var colors = Driver.FindElements(_colorOptions);
                return colors.Count > 0;
            }
            catch
            {
                return false;
            }
        }

        public void SelectColorByIndex(int index)
        {
            WaitForMinimumElements(_colorLabels, index + 1);
            var colorLabels = Driver.FindElements(_colorLabels);
            if (index >= 0 && index < colorLabels.Count)
            {
                colorLabels[index].Click();
                WaitForAjaxComplete();
            }
        }

        // ========== CANTIDAD ==========

        public int GetQuantity()
        {
            WaitForElementVisible(_quantityInput);
            var quantityText = FindElement(_quantityInput).GetDomProperty("value");
            return int.Parse(quantityText);
        }

        public void IncrementQuantity()
        {
            int currentQty = GetQuantity();
            WaitForElementToBeClickable(_incrementButton);
            Click(_incrementButton);
            // Esperamos que la cantidad cambie
            WaitForQuantityUpdate(currentQty + 1);
        }

        public void DecrementQuantity()
        {
            int currentQty = GetQuantity();
            WaitForElementToBeClickable(_decrementButton);
            Click(_decrementButton);
            // Esperamos que la cantidad cambie
            WaitForQuantityUpdate(currentQty - 1);
        }

        public void SetQuantity(int quantity)
        {
            WaitForElementVisible(_quantityInput);
            var input = FindElement(_quantityInput);
            input.Clear();
            input.SendKeys(quantity.ToString());
            // Verificamos que el valor se estableció correctamente
            WaitForQuantityUpdate(quantity);
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
                list.AddRange(links.Select(link => link.Text).Where(t => !string.IsNullOrWhiteSpace(t)));

                // Breadcrumb actual (no es link, puede estar en <span> o <li> sin <a>)
                try
                {
                    var activeItems = Driver.FindElements(By.CssSelector(".breadcrumb li[aria-current='page']"));
                    if (activeItems.Count > 0)
                    {
                        string activeText = activeItems[0].Text;
                        if (!string.IsNullOrWhiteSpace(activeText) && !list.Contains(activeText))
                        {
                            list.Add(activeText);
                        }
                    }
                }
                catch { }
            }
            catch (System.Exception ex)
            {
                LogInfo($"⚠️ Error al obtener breadcrumbs: {ex.Message}");
            }

            return list;
        }

        // Método helper para logging
        private void LogInfo(string message)
        {
            System.Console.WriteLine(message);
        }

        public void ClickBreadcrumb(string breadcrumbText)
        {
            try
            {
                WaitForMinimumElements(_breadcrumbLinks, 1);
                var links = Driver.FindElements(_breadcrumbLinks);
                var link = links.FirstOrDefault(l => l.Text.Equals(breadcrumbText, StringComparison.OrdinalIgnoreCase));
                
                if (link != null)
                {
                    // Hacer scroll al elemento primero
                    ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView(true);", link);
                    System.Threading.Thread.Sleep(500);
                    
                    // Intentar clic normal
                    try
                    {
                        WaitForElementToBeClickable(By.LinkText(breadcrumbText));
                        link.Click();
                    }
                    catch
                    {
                        // Si falla, usar JavaScript
                        ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", link);
                    }
                    
                    WaitForPageLoad();
                }
                else
                {
                    throw new System.Exception($"No se encontró el breadcrumb: {breadcrumbText}");
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"⚠️ Error al hacer clic en breadcrumb '{breadcrumbText}': {ex.Message}");
                throw;
            }
        }

        public void ClickBreadcrumbHome()
        {
            try
            {
                ClickBreadcrumb("Home");
            }
            catch
            {
                // Estrategia alternativa: buscar el logo/home
                try
                {
                    var homeLink = Driver.FindElement(By.CssSelector("#_desktop_logo a, .logo a, a[href*='index.php']"));
                    homeLink.Click();
                    WaitForPageLoad();
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine($"⚠️ No se pudo navegar al home via breadcrumb: {ex.Message}");
                    // Como último recurso, navegar directamente
                    Driver.Navigate().GoToUrl("https://teststore.automationtesting.co.uk/index.php");
                }
            }
        }
    }
}