using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
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
            int countBefore = GetItemCount();
            
            var deleteBtn = FindElement(_deleteButton);
            deleteBtn.Click();
            
            // ✅ Esperar que la cantidad disminuya o el carrito esté vacío
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
    }
}