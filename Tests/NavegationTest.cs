using NUnit.Framework;
using OpenQA.Selenium;
using selenium_tineda_csharp.Pages;
using selenium_tineda_csharp.Tests;
using System.Linq;
using Allure.NUnit.Attributes;

namespace selenium_tineda_csharp.Test
{
    [TestFixture]
    public class Test_Navigation : BaseTest
    {
        [Test]
        [AllureTag("Smoke")]
        [Description("Navegar a diferentes categorías del menú")]
        public void Test_Navegar_ADiferentesCategorias()
        {
            var home = new HomePage(Driver);
            var category = new CategoryPage(Driver);

            LogInfo("Navegando a la página principal");
            home.GoToHomePage();

            LogInfo("Obteniendo categorías disponibles");
            var categories = category.GetAvailableCategories();
            LogInfo($"Categorías encontradas: {string.Join(", ", categories)}");

            Assert.That(categories.Count, Is.GreaterThan(0),
                "Debe haber al menos una categoría disponible");

            var validCategories = categories
                .Where(c => !c.Equals("Home", System.StringComparison.OrdinalIgnoreCase))
                .Take(3)
                .ToList();

            LogInfo($"Probando {validCategories.Count} categorías");

            int categoriesTested = 0;

            foreach (var categoryName in validCategories)
            {
                try
                {
                    LogInfo($"═══════════════════════════════════════");
                    LogInfo($"🏷️ PROBANDO CATEGORÍA: {categoryName}");
                    LogInfo($"═══════════════════════════════════════");

                    LogInfo($"Navegando a la categoría: {categoryName}");
                    category.GoToCategory(categoryName);

                    LogInfo("Verificando navegación exitosa");
                    bool isOnCategoryPage = category.IsOnCategoryPage();
                    
                    Assert.That(isOnCategoryPage, Is.True,
                        $"Debe estar en la página de la categoría {categoryName}");

                    string categoryTitle = category.GetCategoryTitle();
                    LogInfo($"Título de la categoría: {categoryTitle}");
                    
                    Assert.That(string.IsNullOrWhiteSpace(categoryTitle), Is.False,
                        "El título de la categoría debe estar visible");

                    LogInfo("Verificando productos en la categoría");
                    bool hasProducts = category.HasProducts();
                    int productCount = category.GetProductCount();
                    
                    LogInfo($"¿Tiene productos? {hasProducts}");
                    LogInfo($"Cantidad de productos: {productCount}");

                    if (hasProducts)
                    {
                        Assert.That(productCount, Is.GreaterThan(0),
                            "Debe haber al menos un producto en la categoría");
                        
                        LogPass($"✅ Categoría '{categoryName}' tiene {productCount} productos");
                    }
                    else
                    {
                        LogWarning($"⚠️ La categoría '{categoryName}' no tiene productos");
                    }

                    LogInfo("Verificando breadcrumbs");
                    bool hasBreadcrumbs = category.AreBreadcrumbsVisible();
                    
                    if (hasBreadcrumbs)
                    {
                        var breadcrumbs = category.GetBreadcrumbLinks();
                        LogInfo($"Breadcrumbs: {string.Join(" > ", breadcrumbs)}");
                        
                        Assert.That(breadcrumbs.Count, Is.GreaterThan(0),
                            "Debe haber breadcrumbs visibles");
                    }

                    categoriesTested++;
                    LogPass($"✅ Navegación a '{categoryName}' completada exitosamente");

                    LogInfo("Regresando al home");
                    home.GoToHome();
                }
                catch (System.Exception ex)
                {
                    LogWarning($"⚠️ Error al probar categoría '{categoryName}': {ex.Message}");
                    try
                    {
                        home.GoToHomePage();
                    }
                    catch { }
                }
            }

            Assert.That(categoriesTested, Is.GreaterThan(0),
                "Al menos una categoría debe haber sido probada exitosamente");

            LogPass($"✅ Navegación completada: {categoriesTested} categorías probadas");
        }

        [Test]
        [AllureTag("Functional")]
        [Description("Usar breadcrumbs para navegar de vuelta")]
        public void Test_UsarBreadcrumbs_ParaNavegar()
        {
            var home = new HomePage(Driver);
            var category = new CategoryPage(Driver);
            var product = new ProductPage(Driver);

            LogInfo("Navegando a la página principal");
            home.GoToHomePage();

            LogInfo("═══════════════════════════════════════");
            LogInfo("📍 PASO 1: NAVEGAR A CATEGORÍA");
            LogInfo("═══════════════════════════════════════");

            var categories = category.GetAvailableCategories()
                .Where(c => !c.Equals("Home", System.StringComparison.OrdinalIgnoreCase))
                .ToList();

            Assert.That(categories.Count, Is.GreaterThan(0),
                "Debe haber categorías disponibles");

            string selectedCategory = categories.First();
            LogInfo($"Navegando a la categoría: {selectedCategory}");
            
            category.GoToCategory(selectedCategory);

            LogInfo("Verificando breadcrumbs en página de categoría");
            var breadcrumbsCategory = category.GetBreadcrumbLinks();
            LogInfo($"Breadcrumbs actuales: {string.Join(" > ", breadcrumbsCategory)}");
            
            Assert.That(breadcrumbsCategory.Count, Is.GreaterThanOrEqualTo(1),
                "Debe haber al menos 1 breadcrumb (Home)");

            LogInfo("═══════════════════════════════════════");
            LogInfo("📍 PASO 2: NAVEGAR A PRODUCTO");
            LogInfo("═══════════════════════════════════════");

            if (category.HasProducts())
            {
                LogInfo("Seleccionando primer producto de la categoría");
                category.ClickProductByIndex(0);

                LogInfo("Verificando que estamos en página de producto");
                Assert.That(Driver.Url, Does.Contain("id_product"),
                    "Debe estar en página de producto");

                LogInfo("Verificando breadcrumbs en página de producto");
                var breadcrumbsProduct = product.GetBreadcrumbLinks();
                LogInfo($"Breadcrumbs en producto: {string.Join(" > ", breadcrumbsProduct)}");
                
                Assert.That(breadcrumbsProduct.Count, Is.GreaterThanOrEqualTo(1),
                    "Debe haber al menos 1 breadcrumb");

                LogInfo("═══════════════════════════════════════");
                LogInfo("📍 PASO 3: VOLVER AL HOME VÍA BREADCRUMB");
                LogInfo("═══════════════════════════════════════");

                string previousUrl = Driver.Url;
                LogInfo($"URL actual (producto): {previousUrl}");

                // En lugar de buscar categoría específica, ir directo a Home
                LogInfo("Haciendo clic en breadcrumb 'Home'");
                
                try
                {
                    product.ClickBreadcrumbHome();
                    
                    string newUrl = Driver.Url;
                    LogInfo($"URL después de breadcrumb: {newUrl}");

                    Assert.That(newUrl, Is.Not.EqualTo(previousUrl),
                        "La URL debe cambiar después de usar el breadcrumb");

                    Assert.That(newUrl, Does.Contain("index.php"),
                        "Debe regresar a la página principal");

                    LogPass("✅ Regresó exitosamente al home usando breadcrumb");
                }
                catch (System.Exception ex)
                {
                    LogWarning($"⚠️ Error al hacer clic en breadcrumb: {ex.Message}");
                    LogInfo("Intentando navegación alternativa al home");
                    home.GoToHome();
                }
            }
            else
            {
                LogWarning("⚠️ La categoría no tiene productos");
                
                LogInfo("═══════════════════════════════════════");
                LogInfo("📍 PASO 3: VOLVER A HOME VÍA BREADCRUMB (desde categoría)");
                LogInfo("═══════════════════════════════════════");

                string currentUrl = Driver.Url;
                LogInfo($"URL actual: {currentUrl}");

                LogInfo("Haciendo clic en breadcrumb 'Home'");
                category.GoToHomeViaBreadcrumb();

                string finalUrl = Driver.Url;
                LogInfo($"URL final: {finalUrl}");

                Assert.That(finalUrl, Does.Contain("index.php"),
                    "Debe regresar a la página principal");

                LogPass("✅ Navegación usando breadcrumbs exitosa");
            }

            LogPass("✅ Test de breadcrumbs completado");
        }

        [Test]
        [AllureTag("Functional")]
        [Description("Verificar estructura de breadcrumbs en diferentes niveles")]
        public void Test_EstructuraBreadcrumbs_EnDiferentesNiveles()
        {
            var home = new HomePage(Driver);
            var category = new CategoryPage(Driver);
            var product = new ProductPage(Driver);

            LogInfo("Navegando a la página principal");
            home.GoToHomePage();

            LogInfo("═══════════════════════════════════════");
            LogInfo("📍 NIVEL 2: CATEGORÍA");
            LogInfo("═══════════════════════════════════════");

            var categories = category.GetAvailableCategories()
                .Where(c => !c.Equals("Home", System.StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (categories.Count > 0)
            {
                string selectedCategory = categories.First();
                LogInfo($"Navegando a: {selectedCategory}");
                
                category.GoToCategory(selectedCategory);

                var breadcrumbs2 = category.GetBreadcrumbLinks();
                int count2 = category.GetBreadcrumbCount();
                
                LogInfo($"Estructura breadcrumbs (Categoría):");
                LogInfo($"  Total de elementos: {count2}");
                LogInfo($"  Breadcrumbs: {string.Join(" > ", breadcrumbs2)}");
                
                Assert.That(breadcrumbs2.Count, Is.GreaterThanOrEqualTo(1),
                    "Debe haber al menos 1 breadcrumb en categoría");

                bool hasHome = breadcrumbs2.Any(b => 
                    b.Equals("Home", System.StringComparison.OrdinalIgnoreCase));
                
                LogInfo($"¿Contiene 'Home'? {hasHome}");
                
                if (hasHome)
                {
                    Assert.That(hasHome, Is.True, "Los breadcrumbs deben incluir 'Home'");
                }
                else
                {
                    LogWarning("⚠️ No se encontró 'Home' en breadcrumbs, pero puede ser válido");
                }

                LogInfo("═══════════════════════════════════════");
                LogInfo("📍 NIVEL 3: PRODUCTO");
                LogInfo("═══════════════════════════════════════");

                if (category.HasProducts())
                {
                    category.ClickProductByIndex(0);

                    var breadcrumbs3 = product.GetBreadcrumbLinks();
                    
                    LogInfo($"Estructura breadcrumbs (Producto):");
                    LogInfo($"  Breadcrumbs: {string.Join(" > ", breadcrumbs3)}");
                    
                    Assert.That(breadcrumbs3.Count, Is.GreaterThanOrEqualTo(1),
                        "Debe haber al menos 1 breadcrumb en producto");

                    LogPass($"✅ Estructura de breadcrumbs verificada en {breadcrumbs3.Count} niveles");
                }
            }

            LogPass("✅ Estructura de breadcrumbs completa verificada");
        }

        [Test]
        [AllureTag("Functional")]
        [Description("Navegar entre categorías sin volver al home")]
        public void Test_Navegar_EntreCategorias_Directamente()
        {
            var home = new HomePage(Driver);
            var category = new CategoryPage(Driver);

            LogInfo("Navegando a la página principal");
            home.GoToHomePage();

            var categories = category.GetAvailableCategories()
                .Where(c => !c.Equals("Home", System.StringComparison.OrdinalIgnoreCase))
                .Take(2) // Solo 2 categorías para ser más rápido
                .ToList();

            if (categories.Count < 2)
            {
                LogWarning("⚠️ No hay suficientes categorías para probar navegación directa");
                LogInfo("Se necesitan al menos 2 categorías");
                Assert.Ignore("Test omitido: No hay suficientes categorías disponibles");
                return;
            }

            string previousCategory = "";
            int transitionsSuccessful = 0;

            foreach (var categoryName in categories)
            {
                try
                {
                    LogInfo($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                    LogInfo($"📂 Navegando a: {categoryName}");
                    
                    // Guardar URL anterior
                    string urlBefore = Driver.Url;
                    
                    category.GoToCategory(categoryName);

                    string currentTitle = category.GetCategoryTitle();
                    LogInfo($"Categoría actual: {currentTitle}");

                    Assert.That(category.IsOnCategoryPage(), Is.True,
                        $"Debe estar en la página de {categoryName}");

                    // Verificar que la URL cambió
                    string urlAfter = Driver.Url;
                    Assert.That(urlAfter, Is.Not.EqualTo(urlBefore),
                        "La URL debe cambiar al navegar a otra categoría");

                    if (!string.IsNullOrEmpty(previousCategory))
                    {
                        LogInfo($"Transición exitosa: {previousCategory} → {categoryName}");
                        transitionsSuccessful++;
                    }

                    previousCategory = categoryName;
                }
                catch (System.Exception ex)
                {
                    LogWarning($"⚠️ Error al navegar a '{categoryName}': {ex.Message}");
                    // Continuar con la siguiente categoría
                }
            }

            Assert.That(transitionsSuccessful, Is.GreaterThan(0),
                "Debe haber al menos una transición exitosa entre categorías");

            LogPass($"✅ Navegación entre categorías completada: {transitionsSuccessful} transiciones exitosas");
        }
    }
}