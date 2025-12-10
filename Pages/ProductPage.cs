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
        private By _proceedToCheckoutButton = By.CssSelector("#blockcart-modal a.btn-primary");
        private By _continueShoppingButton = By.CssSelector("#blockcart-modal .btn.btn-secondary");

        public void SelectSize(string size = "S")
        {
            var dropdown = new SelectElement(FindElement(_sizeDropdown));
            dropdown.SelectByText(size);
        }

        public void AddToCart()
        {
            Wait.Until(ExpectedConditions.ElementToBeClickable(_addToCartButton));
            Click(_addToCartButton);
        }

        public bool IsSuccessModalDisplayed()
        {
            try
            {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
                wait.Until(ExpectedConditions.ElementIsVisible(_successModal));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void ProceedToCheckout()
        {
            Wait.Until(ExpectedConditions.ElementIsVisible(_successModal));
            
            if (IsElementDisplayed(_proceedToCheckoutButton, 3))
            {
                Wait.Until(ExpectedConditions.ElementToBeClickable(_proceedToCheckoutButton));
                Click(_proceedToCheckoutButton);
            }
            
            // Esperar que el modal desaparezca
            Wait.Until(ExpectedConditions.InvisibilityOfElementLocated(_successModal));
        }

        public void ContinueShopping()
        {
            Wait.Until(ExpectedConditions.ElementIsVisible(_successModal));

            if (IsElementDisplayed(_continueShoppingButton, 3))
            {
                Wait.Until(ExpectedConditions.ElementToBeClickable(_continueShoppingButton));
                Click(_continueShoppingButton);
            }
            else
            {
                Driver.SwitchTo().ActiveElement().SendKeys(Keys.Escape);
            }

            Wait.Until(ExpectedConditions.InvisibilityOfElementLocated(_successModal));
        }
    }
}