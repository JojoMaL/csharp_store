using OpenQA.Selenium;

namespace selenium_tineda_csharp.Pages
{
    public class CartPage : BasePage
    {
        public CartPage(IWebDriver driver) : base(driver) {}

        private By _cartItems = By.CssSelector(".cart-item");

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
    }
}