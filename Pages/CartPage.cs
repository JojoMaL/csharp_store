using OpenQA.Selenium;
using System;

namespace selenium_tineda_csharp.Pages
{
    public class CartPage : BasePage
    {
        public CartPage(IWebDriver driver) : base(driver) {}

        private By _cartItems = By.CssSelector(".cart-item");
        private By _deleteButton = By.CssSelector(".material-icons.float-xs-left");
        private By _emptyCartMessage = By.CssSelector(".no-items");

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
            try
            {
                var deleteBtn = FindElement(_deleteButton);
                deleteBtn.Click();
                System.Threading.Thread.Sleep(2000); // Esperar que se elimine
            }
            catch (Exception ex)
            {
                throw new Exception($"No se pudo eliminar el producto: {ex.Message}");
            }
        }

        public bool IsCartEmpty()
        {
            try
            {
                // Verificar si no hay items
                var items = Driver.FindElements(_cartItems);
                if (items.Count == 0)
                    return true;

                // O si aparece mensaje de carrito vacío
                return IsElementDisplayed(_emptyCartMessage, 3);
            }
            catch
            {
                return true; // Si hay error, asumimos que está vacío
            }
        }
    }
}