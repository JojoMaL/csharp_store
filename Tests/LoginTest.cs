using AventStack.ExtentReports.Model;
using NUnit.Framework;
using OpenQA.Selenium;
using selenium_tineda_csharp.Pages;
using selenium_tineda_csharp.Tests;

namespace selenium_tineda_csharp.Test
{
    public class Test_Login : BaseTest  
    {
        [Test]
        [Category("Smoke")]
        [Description("Verificar si abre la pagina")]
        public void Test_AbrirTiendaOnline()
        {
            LogInfo("Navegando a Tienda");
            Driver.Navigate().GoToUrl("https://teststore.automationtesting.co.uk/index.php");

            LogInfo("Verificando título de la página");
            Console.WriteLine("Titulo real: " + Driver.Title); // con este comando imprimi el titulo real de la pagina solo para verificar que sea la correcta.
            Assert.That(Driver.Title, Does.Contain("Test Store"));
            LogPass("Título verificado correctamente");

            LogInfo("Test completado exitosamente");
        }


        [Test]
        [Category("Smoke")]
        [Description("Realiza login")]
        public void Test_login()
        {
        var loginPage = new LoginPage(Driver);

        LogInfo("Navegando a la página de login");
        loginPage.GoToLoginPage("https://teststore.automationtesting.co.uk/index.php?controller=authentication&back=https%3A%2F%2Fteststore.automationtesting.co.uk%2Findex.php");
        LogPass("Página de login cargada correctamente");

        LogInfo("Ingresando credenciales");
        loginPage.Login("test@test.com", "test123");
        LogPass("Credenciales ingresadas correctamente");

         Assert.IsTrue(loginPage.IsLoggedIn(), "No se encontró el botón Sign out. Login falló.");
        }



        [Test]
        [Category("Smoke")]
        [Description("Realiza login fallido")]
        public void Test_login_fallido()
        {
        var loginPage = new LoginPage(Driver);

        LogInfo("Navegando a la página de login");
        loginPage.GoToLoginPage("https://teststore.automationtesting.co.uk/index.php?controller=authentication&back=https%3A%2F%2Fteststore.automationtesting.co.uk%2Findex.php");

        LogInfo("Ingresando credenciales incorrectas");
        loginPage.Login("mantecon_95@hotmail.com", "elvato123");

        LogInfo("Validando que NO se inició sesión");
        Assert.IsFalse(loginPage.IsLoggedIn(), "El usuario NO debería estar logueado.");

        LogInfo("Validando mensaje de error de login");
        Assert.IsTrue(loginPage.IsLoginErrorDisplayed(), "El mensaje de error no apareció.");

        LogPass("Login fallido validado correctamente.");
        }


        [Test]
        [Category("Smoke")]
        [Description("Verifica que el login con campos vacíos falle correctamente")]
    public void Test_LoginConCamposVacios_Fallido()
    {
        var loginPage = new LoginPage(Driver);

        LogInfo("Navegando a la página de login");
        loginPage.GoToLoginPage("https://teststore.automationtesting.co.uk/index.php?controller=authentication");

        LogInfo("Intentando login con campos vacíos");
        loginPage.Login("", "");

        LogInfo("Validando que el usuario NO se ha logueado");
        Assert.IsFalse(loginPage.IsLoggedIn(), 
            "Error: El usuario no debería estar logueado con campos vacíos.");

        LogInfo("Validando que permanece en la página de login");
        Assert.That(Driver.Url, Does.Contain("authentication"), 
            "El usuario debe permanecer en la página de login.");

        LogPass("Login con campos vacíos bloqueado correctamente.");
    }

 }   

}
