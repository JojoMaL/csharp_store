using NUnit.Framework;
using SeleniumFramework.Utils;

namespace selenium_tineda_csharp.Tests
{
    /// <summary>
    /// Se ejecuta una única vez por cada corrida de tests (no por cada test individual).
    /// Se encarga de escribir en disco el reporte HTML de ExtentReports al finalizar
    /// todos los tests, y de dejar constancia si la corrida se interrumpió sin cerrar
    /// el reporte correctamente.
    /// </summary>
    [SetUpFixture]
    public class GlobalTestSetup
    {
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            // Fuerza la creación temprana del reporte para que la ruta quede
            // impresa en consola desde el inicio de la corrida.
            ExtentManager.GetExtent();
        }

        [OneTimeTearDown]
        public void RunAfterAllTests()
        {
            ExtentManager.FlushReport();
        }
    }
}