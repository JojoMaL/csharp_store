    using AventStack.ExtentReports.Model;
using NUnit.Framework;
using OpenQA.Selenium;
using selenium_tineda_csharp.Pages;
using selenium_tineda_csharp.Tests;

namespace selenium_tineda_csharp.Test
{
    
    public class Test_Cart : BaseTest
    {
 
        [Test]
        [Category("Smoke")]
        [Description("Agregar un producto al carrito")]
        public void Test_AgregarProducto_AlCarrito()
        {   
            var home = new HomePage(Driver);
            var product = new ProductPage(Driver);
            var cart = new CartPage(Driver);

            LogInfo("Navegando a Home");
            home.GoToHomePage("https://teststore.automationtesting.co.uk/index.php");

           // --- Primer producto ---
            LogInfo("Seleccionando primer producto");
            home.ClickProductByIndex(0);
            Assert.That(Driver.Url, Does.Contain("id_product"), "No se abrió la página del primer producto");

            LogInfo("Seleccionando talla y agregando al carrito");
            product.SelectSize("S");
            product.AddToCart();

            LogInfo("Verificando modal de éxito del primer producto");
            Assert.That(product.IsSuccessModalDisplayed(), Is.True, "El modal de éxito no apareció");

            LogInfo("Yendo al carrito");
            product.ProceedToCheckout();

            LogInfo("Verificando items en carrito");
            Assert.That(cart.HasItems(), Is.True);
    
            LogPass("Test completado exitosamente");
        }


        [Test]
[Category("Functional")]
[Description("Agregar varios productos al carrito")]
public void Test_AgregarVariosProductos_AlCarrito()
{
    var home = new HomePage(Driver);
    var product = new ProductPage(Driver);
    var cart = new CartPage(Driver);

    // Ir a la página principal
    LogInfo("Navegando a Home");
    home.GoToHomePage();

    // --- Primer producto ---
    LogInfo("Seleccionando primer producto");
    home.ClickProductByIndex(0);
    Assert.That(Driver.Url, Does.Contain("id_product"), "No se abrió la página del primer producto");

    LogInfo("Seleccionando talla y agregando al carrito");
    product.SelectSize("S");
    product.AddToCart();

    LogInfo("Verificando modal de éxito del primer producto");
    Assert.That(product.IsSuccessModalDisplayed(), Is.True, "El modal de éxito no apareció");
    product.ContinueShopping(); // ✅ cerrar modal

    // --- Segundo producto ---
    LogInfo("Volviendo al home para seleccionar segundo producto");
    home.GoToHome();

    LogInfo("Seleccionando segundo producto");
    home.ClickProductByIndex(1);
    Assert.That(Driver.Url, Does.Contain("id_product"), "No se abrió la página del segundo producto");

    LogInfo("Seleccionando talla y agregando al carrito");
    product.SelectSize("M");
    product.AddToCart();

    LogInfo("Verificando modal de éxito del segundo producto");
    Assert.That(product.IsSuccessModalDisplayed(), Is.True, "El modal de éxito no apareció");

    // Ir al carrito (no cerramos modal aquí, vamos directo a checkout)
    LogInfo("Yendo al carrito");
    product.ProceedToCheckout();

    // Verificar items en carrito
    LogInfo("Verificando items en carrito");
    Assert.That(cart.HasItems(), Is.True, "El carrito está vacío");

    LogPass("Test completado exitosamente");
}


        [Test]
        [Category("Functional")]
        [Description("Eliminar un producto del carrito")]
        public void Test_EliminarProducto_DelCarrito()
        {
        var home = new HomePage(Driver);
        var product = new ProductPage(Driver);
        var cart = new CartPage(Driver);

        LogInfo("Navegando a la tienda");
        home.GoToHomePage("https://teststore.automationtesting.co.uk/index.php");

        LogInfo("Seleccionando primer producto");
        home.ClickFirstProduct();
        Assert.That(Driver.Url, Does.Contain("id_product"));

        LogInfo("Agregando producto al carrito");
        product.SelectSize("S");
        product.AddToCart();
    
        Assert.That(product.IsSuccessModalDisplayed(), Is.True);
    
        LogInfo("Yendo al carrito");
        product.ProceedToCheckout();

        LogInfo("Verificando que el carrito tiene items");
        Assert.That(cart.HasItems(), Is.True);
        int itemsAntes = cart.GetItemCount();

        LogInfo("Eliminando producto del carrito");
        cart.DeleteFirstItem();

        LogInfo("Verificando que el producto fue eliminado");
        int itemsDespues = cart.GetItemCount();
    
        Assert.That(itemsDespues, Is.LessThan(itemsAntes), 
            "La cantidad de items debe haber disminuido");

        if (itemsAntes == 1)
        {
            Assert.That(cart.IsCartEmpty(), Is.True, 
                "El carrito debe estar vacío");
        }

        LogPass("Producto eliminado exitosamente del carrito");
}









    }
}