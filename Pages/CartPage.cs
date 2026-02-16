using OpenQA.Selenium;
using System;

namespace selenium_tineda_csharp.Pages
{
    /// <summary>
    /// Page Object para la página del carrito de compras
    /// Refactorizado para eliminar logging en consola y simplificar métodos
    /// </summary>
    public class CartPage : BasePage
    {
        // ========== LOCATORS ==========
        private By _cartItems = By.CssSelector(".cart-item");
        private By _deleteButton = By.CssSelector(".material-icons.float-xs-left");
        private By _emptyCartMessage = By.CssSelector(".no-items");
        private By _quantityInput = By.CssSelector(".js-cart-line-product-quantity");
        private By _touchspinUp = By.CssSelector(".bootstrap-touchspin-up");
        private By _touchspinDown = By.CssSelector(".bootstrap-touchspin-down");
        private By _totalPriceElement = By.CssSelector(".cart-total .value");
        private By _productNameElements = By.CssSelector(".product-line-info a");

        public CartPage(IWebDriver driver) : base(driver) { }

        // ========== NAVEGACIÓN ==========

        /// <summary>
        /// Navega directamente a la página del carrito
        /// </summary>
        public void GoToCart()
        {
            NavigateTo("https://teststore.automationtesting.co.uk/index.php?controller=cart&action=show");
            WaitForCartPageLoad();
        }

        // ========== WAIT METHODS ==========

        /// <summary>
        /// Espera a que la página del carrito cargue completamente
        /// </summary>
        private void WaitForCartPageLoad()
        {
            WaitForPageLoad();
            
            // Esperar a que aparezca el mensaje de carrito vacío O productos
            try
            {
                WaitForMinimumElements(_emptyCartMessage, 1, 5);
            }
            catch
            {
                WaitForMinimumElements(_cartItems, 1, 5);
            }
        }

        /// <summary>
        /// Espera a que la cantidad del producto se actualice
        /// </summary>
        private void WaitForQuantityChange(int productIndex, int expectedQuantity, int timeoutSeconds = 10)
        {
            try
            {
                var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutSeconds));
                wait.Until(driver =>
                {
                    try
                    {
                        int currentQuantity = GetProductQuantityInternal(productIndex);
                        return currentQuantity == expectedQuantity;
                    }
                    catch
                    {
                        return false;
                    }
                });
            }
            catch
            {
                // Si falla el wait, el test de validación capturará la diferencia
            }
        }

        /// <summary>
        /// Espera a que un producto sea eliminado del carrito
        /// </summary>
        private void WaitForProductRemoval(int productIndex, int timeoutSeconds = 10)
        {
            try
            {
                var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutSeconds));
                wait.Until(driver =>
                {
                    try
                    {
                        // El carrito está vacío
                        if (IsCartEmptyInternal()) return true;
                        
                        // El índice ya no es válido (producto eliminado)
                        var inputs = Driver.FindElements(_quantityInput);
                        if (productIndex >= inputs.Count) return true;
                        
                        return false;
                    }
                    catch
                    {
                        // Si hay excepción, asumimos que el elemento desapareció
                        return true;
                    }
                });
            }
            catch
            {
                // Timeout - el producto no se eliminó automáticamente
            }
        }

        // ========== VERIFICACIONES ==========

        /// <summary>
        /// Verifica si el carrito tiene productos
        /// </summary>
        public bool HasItems()
        {
            try
            {
                var items = Driver.FindElements(_cartItems);
                return items.Count > 0;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Obtiene la cantidad de productos en el carrito
        /// </summary>
        public int GetItemCount()
        {
            try
            {
                var items = Driver.FindElements(_cartItems);
                return items.Count;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Verifica si el carrito está vacío
        /// </summary>
        public bool IsCartEmpty()
        {
            return IsCartEmptyInternal();
        }

        /// <summary>
        /// Verifica si se muestra el mensaje de carrito vacío
        /// </summary>
        public bool IsEmptyCartMessageDisplayed()
        {
            return IsElementDisplayed(_emptyCartMessage, 5);
        }

        /// <summary>
        /// Método interno para verificar si el carrito está vacío
        /// </summary>
        private bool IsCartEmptyInternal()
        {
            try
            {
                var items = Driver.FindElements(_cartItems);
                if (items.Count == 0) return true;

                return IsElementDisplayed(_emptyCartMessage, 3);
            }
            catch
            {
                return true;
            }
        }

        // ========== ACCIONES DE PRODUCTOS ==========

        /// <summary>
        /// Elimina el primer producto del carrito
        /// </summary>
        public void DeleteFirstItem()
        {
            int countBefore = GetItemCount();
            
            var deleteBtn = WaitForElementToBeClickable(_deleteButton);
            deleteBtn.Click();
            
            // Esperar que la cantidad disminuya o el carrito esté vacío
            var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(Driver, TimeSpan.FromSeconds(10));
            wait.Until(driver => 
            {
                int countAfter = GetItemCount();
                return countAfter < countBefore || IsCartEmpty();
            });
        }

        /// <summary>
        /// Obtiene la cantidad de un producto específico
        /// </summary>
        public int GetProductQuantity(int productIndex = 0)
        {
            ValidateProductIndex(productIndex);
            return GetProductQuantityInternal(productIndex);
        }

        /// <summary>
        /// Actualiza la cantidad de un producto en el carrito
        /// </summary>
        public void UpdateProductQuantity(int productIndex, int newQuantity)
        {
            ValidateProductIndex(productIndex);
            
            int currentQuantity = GetProductQuantityInternal(productIndex);
            
            // Si la cantidad ya es la correcta, no hacer nada
            if (currentQuantity == newQuantity) return;

            // Obtener el input del producto
            var quantityInputs = Driver.FindElements(_quantityInput);
            var input = quantityInputs[productIndex];
            
            // Actualizar cantidad usando JavaScript para mayor confiabilidad
            var js = (IJavaScriptExecutor)Driver;
            js.ExecuteScript("arguments[0].value = arguments[1];", input, newQuantity.ToString());
            js.ExecuteScript("arguments[0].dispatchEvent(new Event('change', { bubbles: true }));", input);
            js.ExecuteScript("arguments[0].dispatchEvent(new Event('input', { bubbles: true }));", input);

            // Dar tiempo para que AJAX procese
            System.Threading.Thread.Sleep(1500);

            // Si la nueva cantidad es 0, esperar eliminación
            if (newQuantity == 0)
            {
                WaitForProductRemoval(productIndex);
            }
            else
            {
                // Para otras cantidades, esperar que el valor se actualice
                WaitForQuantityChange(productIndex, newQuantity);
            }
        }

        /// <summary>
        /// Incrementa la cantidad de un producto usando el botón +
        /// </summary>
        public void IncrementProductQuantity(int productIndex = 0)
        {
            ValidateProductIndex(productIndex);
            
            var upButtons = Driver.FindElements(_touchspinUp);
            ValidateButtonIndex(upButtons.Count, productIndex);
            
            int currentQty = GetProductQuantityInternal(productIndex);
            upButtons[productIndex].Click();
            
            // Esperar incremento
            WaitForQuantityChange(productIndex, currentQty + 1);
            System.Threading.Thread.Sleep(500);
        }

        /// <summary>
        /// Decrementa la cantidad de un producto usando el botón -
        /// </summary>
        public void DecrementProductQuantity(int productIndex = 0)
        {
            ValidateProductIndex(productIndex);
            
            var downButtons = Driver.FindElements(_touchspinDown);
            ValidateButtonIndex(downButtons.Count, productIndex);
            
            int currentQty = GetProductQuantityInternal(productIndex);
            downButtons[productIndex].Click();
            
            // Esperar decremento o eliminación
            if (currentQty == 1)
            {
                // Puede ser que elimine el producto
                WaitForProductRemoval(productIndex);
            }
            else
            {
                WaitForQuantityChange(productIndex, currentQty - 1);
            }
            
            System.Threading.Thread.Sleep(500);
        }

        // ========== INFORMACIÓN DE PRODUCTOS ==========

        /// <summary>
        /// Obtiene el nombre de un producto en el carrito
        /// </summary>
        public string GetProductName(int productIndex = 0)
        {
            try
            {
                var productNames = Driver.FindElements(_productNameElements);
                
                if (productIndex >= productNames.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(productIndex), 
                        $"Índice fuera de rango. Índice: {productIndex}, Total: {productNames.Count}");
                }

                return productNames[productIndex].Text;
            }
            catch (ArgumentOutOfRangeException)
            {
                throw;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Obtiene el precio total del carrito
        /// </summary>
        public string GetTotalPrice()
        {
            try
            {
                var totalElement = WaitForElementVisible(_totalPriceElement, 5);
                return totalElement.Text;
            }
            catch
            {
                return "N/A";
            }
        }

        // ========== MÉTODOS PRIVADOS DE VALIDACIÓN ==========

        /// <summary>
        /// Valida que el índice del producto sea válido
        /// </summary>
        private void ValidateProductIndex(int productIndex)
        {
            var quantityInputs = Driver.FindElements(_quantityInput);
            
            if (quantityInputs.Count == 0)
            {
                throw new InvalidOperationException("No hay productos en el carrito");
            }
            
            if (productIndex >= quantityInputs.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(productIndex), 
                    $"Índice fuera de rango. Índice: {productIndex}, Total productos: {quantityInputs.Count}");
            }
        }

        /// <summary>
        /// Valida que el índice del botón sea válido
        /// </summary>
        private void ValidateButtonIndex(int totalButtons, int index)
        {
            if (index >= totalButtons)
            {
                throw new ArgumentOutOfRangeException(nameof(index), 
                    $"Índice de botón fuera de rango. Índice: {index}, Total: {totalButtons}");
            }
        }

        /// <summary>
        /// Obtiene la cantidad interna sin validaciones externas
        /// </summary>
        private int GetProductQuantityInternal(int productIndex)
        {
            try
            {
                var quantityInputs = Driver.FindElements(_quantityInput);
                var input = quantityInputs[productIndex];
                
                // Intentar con GetDomProperty primero (Selenium 4+)
                string valueAttr = input.GetDomProperty("value") ?? input.GetDomAttribute("value") ?? "0";
                
                if (string.IsNullOrEmpty(valueAttr)) return 0;
                
                return int.Parse(valueAttr);
            }
            catch (FormatException)
            {
                return 0;
            }
            catch
            {
                return 0;
            }
        }
    }
}