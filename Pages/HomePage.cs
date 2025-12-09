using OpenQA.Selenium;
using System.Linq;
using System.Collections.Generic;

namespace selenium_tineda_csharp.Pages
{
    public class HomePage : BasePage
    {
        public HomePage(IWebDriver driver) : base(driver) {}

        // Selector de todos los productos en la página
        private By _allProducts = By.CssSelector(".products.row .product-miniature");
        private By _homeLink = By.CssSelector("#_desktop_logo a");

        public void GoToHomePage(string url = "https://teststore.automationtesting.co.uk/index.php")
        {
            NavigateTo(url);
        }

        // Método genérico para hacer clic en un producto por índice
        public void ClickProductByIndex(int index)
        {
            IReadOnlyCollection<IWebElement> products = Driver.FindElements(_allProducts);
            var productList = products.ToList();

            if (index < 0 || index >= productList.Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Índice de producto fuera de rango");

            productList[index].Click();
        }

        public void GoToHome()
        {
            Click(_homeLink);
        }

        public void ClickFirstProduct()
        {
            ClickProductByIndex(0);
        }


    }
}
