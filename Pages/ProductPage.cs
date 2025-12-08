using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using OpenQA.Selenium.Support.UI;
using System;

namespace selenium_tineda_csharp.Pages
{
    public class ProductPage : BasePage
    {
        public ProductPage(IWebDriver driver) : base(driver) {}

        private By _sizeDropdown = By.Id("group_1");
        private By _addToCartButton = By.CssSelector("button.add-to-cart");
        private By _successModal = By.CssSelector("#blockcart-modal");
        private By _modalContent = By.CssSelector(".modal-content");
        
        // Selectores alternativos para el botón de checkout
        private By _proceedToCheckoutButton = By.CssSelector("#blockcart-modal a.btn-primary");
        private By _proceedToCheckoutAlt = By.CssSelector("a[href*='controller=cart']");

        public void SelectSize(string size = "S")
        {
            Console.WriteLine($"[ProductPage] Seleccionando talla: {size}");
            var dropdown = new SelectElement(FindElement(_sizeDropdown));
            dropdown.SelectByText(size);
            Console.WriteLine($"[ProductPage] Talla {size} seleccionada");
        }

      public void AddToCart()
{
    try
    {
        Wait.Until(ExpectedConditions.ElementToBeClickable(_addToCartButton));
        Click(_addToCartButton);
        Console.WriteLine("[ProductPage] Clic en 'Add to cart' realizado");

        Wait.Until(ExpectedConditions.ElementIsVisible(_successModal));
        Console.WriteLine("[ProductPage] Modal de éxito visible");

        Wait.Until(driver => 
        {
            var modal = driver.FindElement(_successModal);
            return modal.Displayed && modal.Size.Height > 0 && modal.Size.Width > 0;
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ProductPage] ERROR en AddToCart: {ex.Message}");
        throw;
    }
}


        public bool IsSuccessModalDisplayed()
        {
            try
            {
                Console.WriteLine("[ProductPage] Verificando si el modal está visible...");
                
                // Esperar explícitamente a que el modal aparezca
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
                wait.Until(ExpectedConditions.ElementIsVisible(_successModal));
                
                Console.WriteLine("[ProductPage] Modal de éxito encontrado y visible");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProductPage] Modal NO visible: {ex.Message}");
                return false;
            }
        }

       public void ProceedToCheckout()
{
    try
    {
        Console.WriteLine("[ProductPage] Esperando que el modal esté completamente visible...");
        Wait.Until(ExpectedConditions.ElementIsVisible(_successModal));

        Console.WriteLine("[ProductPage] Buscando botón 'Proceed to checkout'...");
        
        if (IsElementDisplayed(_proceedToCheckoutButton, 3))
        {
            Console.WriteLine("[ProductPage] Botón encontrado, haciendo clic...");
            Wait.Until(ExpectedConditions.ElementToBeClickable(_proceedToCheckoutButton));
            Click(_proceedToCheckoutButton);
            Console.WriteLine("[ProductPage] Clic en 'Proceed to checkout' exitoso");
        }
        else
        {
            Console.WriteLine("[ProductPage] Intentando con selector alternativo...");
            Wait.Until(ExpectedConditions.ElementToBeClickable(_proceedToCheckoutAlt));
            Click(_proceedToCheckoutAlt);
            Console.WriteLine("[ProductPage] Clic con selector alternativo exitoso");
        }

        // Esperar a que el modal desaparezca para evitar clics interceptados
        Wait.Until(ExpectedConditions.InvisibilityOfElementLocated(_successModal));

        // Esperar a que la URL cambie (indicando navegación)
        Wait.Until(driver => !driver.Url.Contains("ProductPage"));
        Console.WriteLine($"[ProductPage] URL después de checkout: {Driver.Url}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ProductPage] ERROR en ProceedToCheckout: {ex.Message}");
        throw;
    }
}

public void ContinueShopping()
{
    try
    {
        By continueShoppingButton = By.CssSelector("#blockcart-modal .continue.btn, .cart-content-btn .continue");

        // Esperar a que el modal sea visible antes de cerrarlo
        Wait.Until(ExpectedConditions.ElementIsVisible(_successModal));

        if (IsElementDisplayed(continueShoppingButton, 3))
        {
            Wait.Until(ExpectedConditions.ElementToBeClickable(continueShoppingButton));
            Click(continueShoppingButton);
            Console.WriteLine("[ProductPage] Modal cerrado con 'Continue Shopping'");
        }
        else
        {
            Console.WriteLine("[ProductPage] Botón 'Continue Shopping' no encontrado, cerrando modal con ESC...");
            Driver.SwitchTo().ActiveElement().SendKeys(Keys.Escape);
        }

        // Esperar a que el modal desaparezca realmente
        Wait.Until(ExpectedConditions.InvisibilityOfElementLocated(_successModal));
        Console.WriteLine("[ProductPage] Modal cerrado correctamente");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ProductPage] ERROR al cerrar modal: {ex.Message}");
        throw;
    }
}

        
        



    }
}



