using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace selenium_tineda_csharp.Pages
{
    /// <summary>
    /// Page Object para la página de detalle de producto
    /// Versión refactorizada sin logging en consola
    /// </summary>
    public class ProductPage : BasePage
    {
        // ========== LOCATORS ==========
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

        // ========== WAIT METHODS ==========

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

        // ========== ACCIONES PRINCIPALES ==========

        /// <summary>
        /// Selecciona una talla del dropdown
        /// </summary>
        public void SelectSize(string size)
        {
            WaitForElementVisible(_sizeDropdown);
            var dropdown = new SelectElement(FindElement(_sizeDropdown));
            dropdown.SelectByText(size);
            WaitForAjaxComplete();
        }

        /// <summary>
        /// Agrega el producto al carrito
        /// </summary>
        public void AddToCart()
        {
            WaitForElementToBeClickable(_addToCartButton);
            Click(_addToCartButton);
        }

        /// <summary>
        /// Procede al checkout desde el modal de éxito
        /// </summary>
        public void ProceedToCheckout()
        {
            if (IsSuccessModalDisplayed())
            {
                WaitForElementToBeClickable(_proceedToCheckoutButton);
                Click(_proceedToCheckoutButton);
            }
        }

        /// <summary>
        /// Continúa comprando desde el modal de éxito
        /// </summary>
        public void ContinueShopping()
        {
            if (IsSuccessModalDisplayed())
            {
                WaitForElementToBeClickable(_continueShoppingButton);
                Click(_continueShoppingButton);
                WaitForModalToDisappear();
            }
        }

        // ========== VERIFICACIONES ==========

        /// <summary>
        /// Verifica si el modal de éxito está visible
        /// </summary>
        public bool IsSuccessModalDisplayed()
        {
            return IsElementDisplayed(_successModal, 10);
        }

        /// <summary>
        /// Verifica si la descripción del producto está visible
        /// </summary>
        public bool IsProductDescriptionVisible()
        {
            return IsElementDisplayed(_productDescription, 5);
        }

        /// <summary>
        /// Verifica si la imagen del producto está visible
        /// </summary>
        public bool IsProductImageVisible()
        {
            return IsElementDisplayed(_productImage, 5);
        }

        // ========== INFORMACIÓN DEL PRODUCTO ==========

        /// <summary>
        /// Obtiene el nombre del producto
        /// </summary>
        public string GetProductName()
        {
            WaitForElementVisible(_productName);
            return FindElement(_productName).Text;
        }

        /// <summary>
        /// Obtiene el precio del producto
        /// </summary>
        public string GetProductPrice()
        {
            WaitForElementVisible(_productPrice);
            return FindElement(_productPrice).Text;
        }

        /// <summary>
        /// Obtiene la descripción del producto
        /// </summary>
        public string GetProductDescription()
        {
            if (IsProductDescriptionVisible())
            {
                return FindElement(_productDescription).Text;
            }
            return string.Empty;
        }

        // ========== TALLAS ==========

        /// <summary>
        /// Obtiene lista de tallas disponibles
        /// </summary>
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

        /// <summary>
        /// Obtiene la talla seleccionada actualmente
        /// </summary>
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

        /// <summary>
        /// Verifica si el producto tiene opciones de color
        /// </summary>
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

        /// <summary>
        /// Selecciona un color por índice
        /// </summary>
        public void SelectColorByIndex(int index)
        {
            WaitForMinimumElements(_colorLabels, index + 1);
            var colorLabels = Driver.FindElements(_colorLabels);
            
            if (index < 0 || index >= colorLabels.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), 
                    $"Índice fuera de rango. Índice: {index}, Total: {colorLabels.Count}");
            }

            colorLabels[index].Click();
            WaitForAjaxComplete();
        }

        // ========== CANTIDAD ==========

        /// <summary>
        /// Obtiene la cantidad actual
        /// </summary>
        public int GetQuantity()
        {
            WaitForElementVisible(_quantityInput);
            var quantityText = FindElement(_quantityInput).GetDomProperty("value") ?? "1";
            return int.Parse(quantityText);
        }

        /// <summary>
        /// Incrementa la cantidad en 1
        /// </summary>
        public void IncrementQuantity()
        {
            int currentQty = GetQuantity();
            WaitForElementToBeClickable(_incrementButton);
            Click(_incrementButton);
            WaitForPropertyValue(_quantityInput, "value", (currentQty + 1).ToString(), 5);
        }

        /// <summary>
        /// Decrementa la cantidad en 1
        /// </summary>
        public void DecrementQuantity()
        {
            int currentQty = GetQuantity();
            if (currentQty <= 1) return; // No decrementar si ya está en 1
            
            WaitForElementToBeClickable(_decrementButton);
            Click(_decrementButton);
            WaitForPropertyValue(_quantityInput, "value", (currentQty - 1).ToString(), 5);
        }

        /// <summary>
        /// Establece una cantidad específica
        /// </summary>
        public void SetQuantity(int quantity)
        {
            WaitForElementVisible(_quantityInput);
            var input = FindElement(_quantityInput);
            input.Clear();
            input.SendKeys(quantity.ToString());
            WaitForPropertyValue(_quantityInput, "value", quantity.ToString(), 5);
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
                    .Select(link => link.Text)
                    .Where(t => !string.IsNullOrWhiteSpace(t)));

                // Breadcrumb actual
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
            catch
            {
                // Retornar lista vacía en caso de error
            }

            return list;
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
            System.Threading.Thread.Sleep(500);
            
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
        public void ClickBreadcrumbHome()
        {
            try
            {
                ClickBreadcrumb("Home");
            }
            catch
            {
                // Estrategia alternativa: buscar el logo
                try
                {
                    var homeLink = Driver.FindElement(By.CssSelector("#_desktop_logo a, .logo a, a[href*='index.php']"));
                    homeLink.Click();
                    WaitForPageLoad();
                }
                catch
                {
                    // Última opción: navegación directa
                    Driver.Navigate().GoToUrl("https://teststore.automationtesting.co.uk/index.php");
                }
            }
        }
    }
}