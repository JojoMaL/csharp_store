using OpenQA.Selenium;

namespace selenium_tineda_csharp.Pages
{
    public class LoginPage : BasePage
    {
        private By _usernameField = By.Id("field-email");
        private By _passwordField = By.Id("field-password");
        private By _loginButton = By.Id("submit-login"); 
        private By _logoutLink = By.XPath("//a[contains(text(),'Sign out')]");
        private By _loginError = By.XPath("//li[contains(text(),'Authentication') or contains(text(),'error')]");

        public LoginPage(IWebDriver driver) : base(driver)
        {
        }

        public void GoToLoginPage(string url)
        {
            NavigateTo(url);
        }

        public void Login(string username, string password)
        {
            Type(_usernameField, username);
            Type(_passwordField, password);
            Click(_loginButton);
        }

        public bool IsLoggedIn()
        {
            return IsElementDisplayed(_logoutLink, 5);
        }

        public bool IsLoginErrorDisplayed()
        {
            return IsElementDisplayed(_loginError, 5);
        }

        public string GetLoginErrorText()
        {
            if (IsLoginErrorDisplayed())
                return Driver.FindElement(_loginError).Text;
            return string.Empty;
        }

        public bool IsEmailFieldRequired()
        {
            try
            {
                var emailField = Driver.FindElement(By.Id("field-email"));
                // ✅ Actualizado a GetDomAttribute (required es un atributo HTML)
                var isRequired = emailField.GetDomAttribute("required") != null;
                return isRequired;
            }
            catch
            {
                return false;
            }
        }
    }
}