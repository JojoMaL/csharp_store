using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace selenium_tineda_csharp.Pages
{
    public class CartPage : BasePage
    {
        public CartPage(IWebDriver driver) : base(driver) {}

        // Selectores existentes
        private By _cartItems = By.CssSelector(".cart-item");
        private By _deleteButton = By.CssSelector(".material-icons.float-xs-left");
        private By _emptyCartMessage = By.CssSelector(".no-items");
        
        // Nuevos selectores para actualizar cantidad
        private By _quantityInput = By.CssSelector(".js-cart-line-product-quantity");
        private By _touchspinUp = By.CssSelector(".bootstrap-touchspin-up");
        private By _touchspinDown = By.CssSelector(".bootstrap-touchspin-down");
        private By _cartUrl = By.CssSelector("a[href*='controller=cart']");

        // ========== MÉTODOS EXISTENTES ==========

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

        public void DeleteFirstItem()
        {
            int countBefore = GetItemCount();
            
            var deleteBtn = FindElement(_deleteButton);
            deleteBtn.Click();
            
            // Esperar que la cantidad disminuya o el carrito esté vacío
            Wait.Until(driver => 
            {
                int countAfter = GetItemCount();
                return countAfter < countBefore || IsCartEmpty();
            });
        }

        public bool IsCartEmpty()
        {
            try
            {
                var items = Driver.FindElements(_cartItems);
                if (items.Count == 0)
                    return true;

                return IsElementDisplayed(_emptyCartMessage, 3);
            }
            catch
            {
                return true;
            }
        }

        // ========== MÉTODOS NUEVOS ==========

        /// <summary>
        /// Navega directamente a la página del carrito
        /// </summary>
        public void GoToCart()
        {
            NavigateTo("https://teststore.automationtesting.co.uk/index.php?controller=cart&action=show");
            
            // Esperar a que la página cargue
            Wait.Until(driver => 
                driver.Url.Contains("controller=cart") || 
                driver.FindElements(_emptyCartMessage).Count > 0 ||
                driver.FindElements(_cartItems).Count > 0
            );
        }

        /// <summary>
        /// Verifica si se muestra el mensaje de carrito vacío
        /// </summary>
        public bool IsEmptyCartMessageDisplayed()
        {
            return IsElementDisplayed(_emptyCartMessage, 5);
        }

        /// <summary>
        /// Obtiene la cantidad actual de un producto específico en el carrito
        /// </summary>
        /// <param name="productIndex">Índice del producto (0 para el primero)</param>
        /// <returns>Cantidad del producto</returns>
        public int GetProductQuantity(int productIndex = 0)
        {
            try
            {
                var quantityInputs = Driver.FindElements(_quantityInput);
                
                if (quantityInputs.Count == 0)
                {
                    Console.WriteLine("[GetProductQuantity] No se encontraron inputs de cantidad");
                    return 0;
                }
                
                if (productIndex >= quantityInputs.Count)
                {
                    Console.WriteLine($"[GetProductQuantity] Índice {productIndex} fuera de rango. Total: {quantityInputs.Count}");
                    throw new ArgumentOutOfRangeException(nameof(productIndex), 
                        $"El índice del producto está fuera de rango. Índice: {productIndex}, Total: {quantityInputs.Count}");
                }

                var input = quantityInputs[productIndex];
                
                // Usar GetDomProperty primero (Selenium 4+), fallback a GetDomAttribute
                string valueAttr = input.GetDomProperty("value") ?? input.GetDomAttribute("value") ?? "0";
                
                Console.WriteLine($"[GetProductQuantity] Producto {productIndex}, Valor leído: '{valueAttr}'");
                
                if (string.IsNullOrEmpty(valueAttr))
                {
                    Console.WriteLine("[GetProductQuantity] Valor vacío, retornando 0");
                    return 0;
                }
                
                int quantity = int.Parse(valueAttr);
                Console.WriteLine($"[GetProductQuantity] Cantidad parseada: {quantity}");
                return quantity;
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"[GetProductQuantity] Error de formato: {ex.Message}");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetProductQuantity] Error al obtener cantidad del producto: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Actualiza la cantidad de un producto específico en el carrito
        /// </summary>
        /// <param name="productIndex">Índice del producto (0 para el primero)</param>
        /// <param name="newQuantity">Nueva cantidad deseada</param>
        public void UpdateProductQuantity(int productIndex, int newQuantity)
        {
            try
            {
                int currentQuantity = GetProductQuantity(productIndex);
                Console.WriteLine($"[UpdateQuantity] Cantidad actual: {currentQuantity}, Nueva cantidad: {newQuantity}");
                
                if (currentQuantity == newQuantity)
                {
                    Console.WriteLine($"La cantidad ya es {newQuantity}, no se requiere actualización");
                    return;
                }

                // Obtener el input de cantidad específico
                var quantityInputs = Driver.FindElements(_quantityInput);
                if (productIndex >= quantityInputs.Count)
                    throw new ArgumentOutOfRangeException(nameof(productIndex));

                var input = quantityInputs[productIndex];
                
                // ESTRATEGIA MEJORADA: Usar JavaScript para más confiabilidad
                IJavaScriptExecutor js = (IJavaScriptExecutor)Driver;
                
                // Paso 1: Establecer el valor con JavaScript
                js.ExecuteScript("arguments[0].value = arguments[1];", input, newQuantity.ToString());
                
                // Paso 2: Disparar evento 'change' para que el sitio detecte el cambio
                js.ExecuteScript(@"
                    var evt = new Event('change', { bubbles: true });
                    arguments[0].dispatchEvent(evt);
                ", input);
                
                // Paso 3: También disparar 'input' event por si el sitio lo usa
                js.ExecuteScript(@"
                    var evt = new Event('input', { bubbles: true });
                    arguments[0].dispatchEvent(evt);
                ", input);
                
                Console.WriteLine($"[UpdateQuantity] Eventos disparados, esperando actualización...");
                
                // Paso 4: Esperar a que el carrito se actualice
                // Si es 0, esperamos que el carrito se vacíe O que el producto desaparezca
                if (newQuantity == 0)
                {
                    Console.WriteLine("[UpdateQuantity] Cantidad = 0, verificando comportamiento...");
                    
                    try
                    {
                        // Esperar hasta 10 segundos (algunos sitios tardan más)
                        var shortWait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
                        
                        bool productoEliminado = shortWait.Until(driver => 
                        {
                            try
                            {
                                System.Threading.Thread.Sleep(500);
                                
                                // Opción 1: El carrito se vació completamente
                                bool carritoVacio = IsCartEmpty();
                                if (carritoVacio)
                                {
                                    Console.WriteLine("[UpdateQuantity] Carrito vacío detectado");
                                    return true;
                                }
                                
                                // Opción 2: El producto desapareció pero hay otros productos
                                var itemsActuales = Driver.FindElements(_cartItems);
                                bool menosItems = itemsActuales.Count < Driver.FindElements(_cartItems).Count;
                                if (menosItems)
                                {
                                    Console.WriteLine("[UpdateQuantity] Producto eliminado del carrito");
                                    return true;
                                }
                                
                                // Opción 3: Verificar que el input ya no existe en ese índice
                                var inputsActuales = Driver.FindElements(_quantityInput);
                                bool inputDesaparecio = productIndex >= inputsActuales.Count;
                                if (inputDesaparecio)
                                {
                                    Console.WriteLine("[UpdateQuantity] Input de cantidad desapareció");
                                    return true;
                                }
                                
                                Console.WriteLine($"[UpdateQuantity] Esperando... Items: {itemsActuales.Count}, Inputs: {inputsActuales.Count}");
                                return false;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"[UpdateQuantity] Excepción durante espera: {ex.Message}");
                                // Si hay excepción, asumimos que el elemento desapareció
                                return true;
                            }
                        });
                        
                        Console.WriteLine($"[UpdateQuantity] Producto eliminado exitosamente");
                    }
                    catch (WebDriverTimeoutException)
                    {
                        // Si timeout, el sitio puede no eliminar automáticamente con cantidad 0
                        Console.WriteLine("[UpdateQuantity] ADVERTENCIA: Timeout esperando eliminación. Algunos sitios no eliminan con cantidad 0.");
                        Console.WriteLine("[UpdateQuantity] Verificando si el valor al menos se estableció en 0...");
                        
                        try
                        {
                            var inputsActuales = Driver.FindElements(_quantityInput);
                            if (productIndex < inputsActuales.Count)
                            {
                                string valorActual = inputsActuales[productIndex].GetDomProperty("value") ?? "-1";
                                Console.WriteLine($"[UpdateQuantity] Valor actual del input: {valorActual}");
                                
                                if (valorActual == "0")
                                {
                                    Console.WriteLine("[UpdateQuantity] El valor es 0 pero el producto no se eliminó automáticamente.");
                                    Console.WriteLine("[UpdateQuantity] Esto es comportamiento normal en algunos sitios.");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[UpdateQuantity] Error verificando valor: {ex.Message}");
                        }
                        
                        // No lanzar excepción, dejar que el test decida qué hacer
                    }
                }
                else
                {
                    // Para otras cantidades, esperamos cambios en la página o en el subtotal
                    System.Threading.Thread.Sleep(2000); // Dar tiempo para que AJAX actualice
                    
                    // Verificar que el valor cambió en el input
                    int verificarCantidad = GetProductQuantity(productIndex);
                    Console.WriteLine($"[UpdateQuantity] Cantidad verificada después de actualización: {verificarCantidad}");
                    
                    if (verificarCantidad != newQuantity)
                    {
                        Console.WriteLine($"[UpdateQuantity] ADVERTENCIA: La cantidad no se actualizó correctamente. Esperada: {newQuantity}, Actual: {verificarCantidad}");
                    }
                }
                
                Console.WriteLine($"[UpdateQuantity] Actualización completada");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UpdateQuantity] Error: {ex.Message}");
                Console.WriteLine($"[UpdateQuantity] StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        /// <summary>
        /// Incrementa la cantidad de un producto usando el botón +
        /// </summary>
        /// <param name="productIndex">Índice del producto</param>
        public void IncrementProductQuantity(int productIndex = 0)
        {
            try
            {
                var upButtons = Driver.FindElements(_touchspinUp);
                
                if (productIndex >= upButtons.Count)
                    throw new ArgumentOutOfRangeException(nameof(productIndex));

                int currentQty = GetProductQuantity(productIndex);
                upButtons[productIndex].Click();
                
                // Esperar que la cantidad incremente
                Wait.Until(driver => GetProductQuantity(productIndex) > currentQty);
                
                System.Threading.Thread.Sleep(500);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al incrementar cantidad: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Decrementa la cantidad de un producto usando el botón -
        /// </summary>
        /// <param name="productIndex">Índice del producto</param>
        public void DecrementProductQuantity(int productIndex = 0)
        {
            try
            {
                var downButtons = Driver.FindElements(_touchspinDown);
                
                if (productIndex >= downButtons.Count)
                    throw new ArgumentOutOfRangeException(nameof(productIndex));

                int currentQty = GetProductQuantity(productIndex);
                downButtons[productIndex].Click();
                
                // Esperar que la cantidad disminuya o el carrito se vacíe
                Wait.Until(driver => 
                {
                    try
                    {
                        int newQty = GetProductQuantity(productIndex);
                        return newQty < currentQty || IsCartEmpty();
                    }
                    catch
                    {
                        return IsCartEmpty();
                    }
                });
                
                System.Threading.Thread.Sleep(500);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al decrementar cantidad: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Obtiene el precio total del carrito
        /// </summary>
        public string GetTotalPrice()
        {
            try
            {
                var totalElement = FindElement(By.CssSelector(".cart-total .value"));
                return totalElement.Text;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener precio total: {ex.Message}");
                return "N/A";
            }
        }

        /// <summary>
        /// Obtiene el nombre de un producto en el carrito
        /// </summary>
        /// <param name="productIndex">Índice del producto</param>
        public string GetProductName(int productIndex = 0)
        {
            try
            {
                var productNames = Driver.FindElements(By.CssSelector(".product-line-info a"));
                
                if (productIndex >= productNames.Count)
                    throw new ArgumentOutOfRangeException(nameof(productIndex));

                return productNames[productIndex].Text;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener nombre del producto: {ex.Message}");
                return string.Empty;
            }
        }
    }
}