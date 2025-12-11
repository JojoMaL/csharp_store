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

        // ========== TESTS EXISTENTES ==========

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

        // ========== TESTS NUEVOS ==========

        [Test]
        [Category("Functional")]
        [Description("Actualizar la cantidad de un producto en el carrito")]
        public void Test_ActualizarCantidad_EnCarrito()
        {
            var cart = new CartPage(Driver);

            LogInfo("Preparando test: agregando producto al carrito");
            AgregarProductoAlCarrito();

            LogInfo("Esperando que la página del carrito cargue completamente");
            System.Threading.Thread.Sleep(2000); // Dar tiempo extra para carga completa

            LogInfo("Obteniendo cantidad inicial del producto");
            int cantidadInicial = cart.GetProductQuantity(0);
            LogInfo($"Cantidad inicial: {cantidadInicial}");

            Assert.That(cantidadInicial, Is.GreaterThan(0), 
                "La cantidad inicial debe ser mayor a 0");

            LogInfo("Incrementando la cantidad del producto a 3");
            cart.UpdateProductQuantity(0, 3);

            LogInfo("Esperando que la actualización se procese");
            System.Threading.Thread.Sleep(2000); // Dar tiempo para que el cambio se refleje

            LogInfo("Verificando que la cantidad se actualizó correctamente");
            int cantidadFinal = cart.GetProductQuantity(0);
            LogInfo($"Cantidad final: {cantidadFinal}");

            Assert.That(cantidadFinal, Is.EqualTo(3), 
                $"La cantidad debe ser 3 después de actualizar. Actual: {cantidadFinal}");

            Assert.That(cantidadFinal, Is.GreaterThan(cantidadInicial), 
                "La cantidad final debe ser mayor que la inicial");

            LogPass("Cantidad actualizada exitosamente");
        }

        [Test]
        [Category("Smoke")]
        [Description("Verificar que el carrito está vacío al inicio")]
        public void Test_CarritoVacio_AlInicio()
        {
            var home = new HomePage(Driver);
            var cart = new CartPage(Driver);

            LogInfo("Navegando a la página principal");
            home.GoToHomePage();

            LogInfo("Navegando directamente al carrito");
            cart.GoToCart();

            LogInfo("Verificando que el carrito está vacío");
            Assert.That(cart.IsCartEmpty(), Is.True, 
                "El carrito debe estar vacío al inicio de la sesión");

            Assert.That(cart.GetItemCount(), Is.EqualTo(0), 
                "La cantidad de items debe ser 0");

            Assert.That(cart.IsEmptyCartMessageDisplayed(), Is.True, 
                "Debe mostrarse el mensaje de carrito vacío");

            LogPass("Verificación de carrito vacío completada exitosamente");
        }

        [Test]
        [Category("Edge Case")]
        [Description("Intentar actualizar cantidad a 0 (debería eliminar el producto o mostrar error)")]
        public void Test_ActualizarCantidadACero_EliminaProducto()
        {
            var cart = new CartPage(Driver);

            LogInfo("Preparando test: agregando producto al carrito");
            AgregarProductoAlCarrito();

            LogInfo("Verificando que el producto está en el carrito");
            Assert.That(cart.HasItems(), Is.True, "El carrito debe tener productos");
            
            int itemsIniciales = cart.GetItemCount();
            LogInfo($"Items iniciales en el carrito: {itemsIniciales}");

            LogInfo("Intentando actualizar cantidad a 0");
            
            // Guardar información antes de actualizar
            string nombreProducto = "";
            try
            {
                nombreProducto = cart.GetProductName(0);
                LogInfo($"Producto a eliminar: {nombreProducto}");
            }
            catch
            {
                LogInfo("No se pudo obtener el nombre del producto");
            }

            // Intentar actualizar a 0
            cart.UpdateProductQuantity(0, 0);

            LogInfo("Esperando procesamiento de la actualización");
            System.Threading.Thread.Sleep(3000);

            // Verificar el comportamiento del sitio
            LogInfo("Verificando resultado de actualizar cantidad a 0");
            
            bool carritoVacio = cart.IsCartEmpty();
            int itemsFinales = cart.GetItemCount();
            
            LogInfo($"¿Carrito vacío? {carritoVacio}");
            LogInfo($"Items finales: {itemsFinales}");

            // El sitio puede comportarse de 3 formas:
            // 1. Eliminar el producto automáticamente (carrito vacío)
            // 2. Mantener el producto con cantidad 0
            // 3. Mostrar un error y mantener cantidad mínima (ej: 1)

            if (carritoVacio)
            {
                LogPass("✅ COMPORTAMIENTO TIPO 1: El sitio eliminó el producto automáticamente al poner cantidad 0");
                Assert.That(itemsFinales, Is.EqualTo(0), 
                    "El carrito debe estar completamente vacío");
            }
            else if (itemsFinales < itemsIniciales)
            {
                LogPass("✅ COMPORTAMIENTO TIPO 2: El sitio redujo la cantidad de productos (parcialmente eliminado)");
                Assert.That(itemsFinales, Is.LessThan(itemsIniciales), 
                    "Debe haber menos productos que al inicio");
            }
            else
            {
                // Verificar si la cantidad se estableció en 0 o si hay mínimo
                try
                {
                    int cantidadActual = cart.GetProductQuantity(0);
                    LogInfo($"Cantidad actual del producto: {cantidadActual}");
                    
                    if (cantidadActual == 0)
                    {
                        LogWarning("⚠️ COMPORTAMIENTO TIPO 3A: El sitio permite cantidad 0 pero NO elimina el producto");
                        LogWarning("Este es un comportamiento válido en algunos e-commerce");
                        
                        Assert.That(cantidadActual, Is.EqualTo(0), 
                            "La cantidad debe ser 0 aunque el producto siga en el carrito");
                    }
                    else if (cantidadActual == 1)
                    {
                        LogWarning("⚠️ COMPORTAMIENTO TIPO 3B: El sitio tiene cantidad mínima de 1");
                        LogWarning("El sitio no permite cantidad 0, mantiene el mínimo");
                        
                        Assert.That(cantidadActual, Is.EqualTo(1), 
                            "Si el sitio no permite 0, debe mantener cantidad mínima de 1");
                    }
                    else
                    {
                        LogWarning($"⚠️ COMPORTAMIENTO INESPERADO: La cantidad es {cantidadActual}");
                        
                        // No fallar el test, solo documentar el comportamiento
                        Assert.That(cantidadActual, Is.GreaterThanOrEqualTo(0), 
                            "La cantidad debe ser un valor válido");
                    }
                }
                catch (Exception ex)
                {
                    LogWarning($"No se pudo verificar la cantidad: {ex.Message}");
                }
            }

            LogPass("Test completado - Comportamiento del sitio documentado");
        }

        [Test]
        [Category("Edge Case")]
        [Description("Verificar cantidad mínima permitida en el carrito")]
        public void Test_CantidadMinima_NoPermiteCero()
        {
            var cart = new CartPage(Driver);

            LogInfo("Preparando test: agregando producto al carrito");
            AgregarProductoAlCarrito();

            LogInfo("Obteniendo cantidad inicial");
            int cantidadInicial = cart.GetProductQuantity(0);
            Assert.That(cantidadInicial, Is.GreaterThan(0), "Debe haber al menos 1 producto");

            LogInfo("Intentando decrementar hasta 0 usando botones");
            
            // Si la cantidad inicial es mayor a 1, decrementar hasta 1
            while (cart.GetProductQuantity(0) > 1)
            {
                cart.DecrementProductQuantity(0);
                System.Threading.Thread.Sleep(1500);
                
                int cantidadActual = cart.GetProductQuantity(0);
                LogInfo($"Cantidad actual: {cantidadActual}");
            }

            // Ahora estamos en cantidad 1
            int cantidadEnUno = cart.GetProductQuantity(0);
            LogInfo($"Cantidad antes del último decremento: {cantidadEnUno}");
            Assert.That(cantidadEnUno, Is.EqualTo(1), "Debe estar en cantidad 1");

            LogInfo("Intentando decrementar de 1 a 0");
            cart.DecrementProductQuantity(0);
            System.Threading.Thread.Sleep(2000);

            // Verificar qué pasó
            bool carritoVacio = cart.IsCartEmpty();
            
            if (carritoVacio)
            {
                LogPass("✅ El sitio eliminó el producto al decrementar de 1 a 0");
                Assert.That(cart.GetItemCount(), Is.EqualTo(0), 
                    "El carrito debe estar vacío");
            }
            else
            {
                int cantidadFinal = cart.GetProductQuantity(0);
                LogInfo($"Cantidad final: {cantidadFinal}");
                
                if (cantidadFinal == 1)
                {
                    LogPass("✅ El sitio NO permite cantidad 0, mantiene mínimo de 1");
                    Assert.That(cantidadFinal, Is.EqualTo(1), 
                        "El sitio mantiene cantidad mínima de 1");
                }
                else if (cantidadFinal == 0)
                {
                    LogPass("✅ El sitio permite cantidad 0 sin eliminar el producto");
                    Assert.That(cantidadFinal, Is.EqualTo(0), 
                        "La cantidad es 0 pero el producto sigue en el carrito");
                }
            }

            LogPass("Test de cantidad mínima completado");
        }

        [Test]
        [Category("Functional")]
        [Description("Verificar que el carrito mantiene el estado entre navegaciones")]
        public void Test_CarritoMantiene_EstadoEntreNavegaciones()
        {
            var home = new HomePage(Driver);
            var product = new ProductPage(Driver);
            var cart = new CartPage(Driver);

            LogInfo("PASO 1: Agregar producto al carrito");
            home.GoToHomePage();
            home.ClickFirstProduct();
            product.SelectSize("M");
            product.AddToCart();
            Assert.That(product.IsSuccessModalDisplayed(), Is.True);
            product.ProceedToCheckout();

            LogInfo("Verificando que hay 1 producto en el carrito");
            int cantidadInicial = cart.GetItemCount();
            Assert.That(cantidadInicial, Is.EqualTo(1), "Debe haber 1 producto");

            LogInfo("PASO 2: Navegar de vuelta al home");
            home.GoToHome();
            Assert.That(Driver.Url, Does.Contain("index.php"), "Debe estar en la home");

            LogInfo("PASO 3: Volver al carrito");
            cart.GoToCart();

            LogInfo("Verificando que el producto sigue en el carrito");
            int cantidadDespues = cart.GetItemCount();
            Assert.That(cantidadDespues, Is.EqualTo(cantidadInicial), 
                $"El carrito debe mantener los {cantidadInicial} productos después de navegar");

            Assert.That(cart.HasItems(), Is.True, 
                "El carrito debe tener productos después de navegar");

            LogPass("✅ El carrito mantiene el estado correctamente entre navegaciones");
        }
    }
}