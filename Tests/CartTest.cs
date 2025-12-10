using NUnit.Framework;
using OpenQA.Selenium;
using selenium_tineda_csharp.Pages;
using selenium_tineda_csharp.Tests;

namespace selenium_tineda_csharp.Test
{
    public class Test_Cart : BaseTest
    {
        // ========== MÉTODO HELPER PRIVADO ==========
        /// <summary>
        /// Agrega un producto al carrito (elimina duplicación)
        /// </summary>
        private void AgregarProductoAlCarrito(int productIndex = 0, string size = "S")
        {
            var home = new HomePage(Driver);
            var product = new ProductPage(Driver);

            LogInfo($"Agregando producto {productIndex} talla {size} al carrito");
            
            home.GoToHomePage();
            home.ClickProductByIndex(productIndex);
            
            Assert.That(Driver.Url, Does.Contain("id_product"), 
                "No se abrió la página del producto");
            
            product.SelectSize(size);
            product.AddToCart();
            
            Assert.That(product.IsSuccessModalDisplayed(), Is.True, 
                "El modal de confirmación no apareció");
            
            product.ProceedToCheckout();
        }

        // ========== TESTS ==========

        [Test]
        [Category("Smoke")]
        [Description("Agregar un producto al carrito")]
        public void Test_AgregarProducto_AlCarrito()
        {
            var cart = new CartPage(Driver);

            LogInfo("Test: Agregar un producto al carrito");
            AgregarProductoAlCarrito();

            LogInfo("Verificando que el producto está en el carrito");
            Assert.That(cart.HasItems(), Is.True, 
                "El carrito debe contener el producto agregado");
            
            Assert.That(cart.GetItemCount(), Is.EqualTo(1), 
                "Debe haber exactamente 1 producto en el carrito");

            LogPass("Producto agregado exitosamente");
        }

        [Test]
        [Category("Functional")]
        [Description("Agregar varios productos al carrito")]
        public void Test_AgregarVariosProductos_AlCarrito()
        {
            var home = new HomePage(Driver);
            var product = new ProductPage(Driver);
            var cart = new CartPage(Driver);

            LogInfo("Navegando a la tienda");
            home.GoToHomePage();

            // Primer producto
            LogInfo("Agregando primer producto (índice 0, talla S)");
            home.ClickProductByIndex(0);
            Assert.That(Driver.Url, Does.Contain("id_product"));
            
            product.SelectSize("S");
            product.AddToCart();
            Assert.That(product.IsSuccessModalDisplayed(), Is.True);
            product.ContinueShopping();

            // Segundo producto
            LogInfo("Agregando segundo producto (índice 1, talla M)");
            home.GoToHome();
            home.ClickProductByIndex(1);
            Assert.That(Driver.Url, Does.Contain("id_product"));
            
            product.SelectSize("M");
            product.AddToCart();
            Assert.That(product.IsSuccessModalDisplayed(), Is.True);
            product.ProceedToCheckout();

            // Verificaciones
            LogInfo("Verificando cantidad de productos en carrito");
            Assert.That(cart.HasItems(), Is.True, 
                "El carrito debe tener productos");
            
            Assert.That(cart.GetItemCount(), Is.EqualTo(2), 
                "Debe haber exactamente 2 productos en el carrito");

            LogPass("Múltiples productos agregados exitosamente");
        }

        [Test]
        [Category("Functional")]
        [Description("Eliminar un producto del carrito")]
        public void Test_EliminarProducto_DelCarrito()
        {
            var cart = new CartPage(Driver);

            LogInfo("Preparando test: agregando producto al carrito");
            AgregarProductoAlCarrito();

            LogInfo("Verificando estado inicial del carrito");
            Assert.That(cart.HasItems(), Is.True, 
                "El carrito debe tener items antes de eliminar");
            
            int itemsAntes = cart.GetItemCount();
            LogInfo($"Items en carrito antes de eliminar: {itemsAntes}");

            LogInfo("Eliminando primer producto del carrito");
            cart.DeleteFirstItem();

            LogInfo("Verificando que el producto fue eliminado");
            int itemsDespues = cart.GetItemCount();
            LogInfo($"Items después de eliminar: {itemsDespues}");

            Assert.That(itemsDespues, Is.LessThan(itemsAntes), 
                "La cantidad de items debe haber disminuido");

            if (itemsAntes == 1)
            {
                Assert.That(cart.IsCartEmpty(), Is.True, 
                    "El carrito debe estar vacío después de eliminar el único item");
            }

            LogPass("Producto eliminado exitosamente del carrito");
        }
    }
}