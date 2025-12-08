using NUnit.Framework;
using OpenQA.Selenium;
using SeleniumFramework.Drivers;
using SeleniumFramework.Utils;
using AventStack.ExtentReports;
using System;
using System.IO;
using System.Diagnostics;

namespace selenium_tineda_csharp.Tests
{
    public class BaseTest
    {
        protected IWebDriver Driver;
        protected ExtentTest Test;
        private Stopwatch _stopwatch;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentManager.GetExtent();
        }

        [SetUp]
        public void Setup()
        {
            // Iniciar cronómetro
            _stopwatch = Stopwatch.StartNew();

            var testName = TestContext.CurrentContext.Test.Name;
            var description = TestContext.CurrentContext.Test.Properties.Get("Description")?.ToString() ?? "";
            var category = TestContext.CurrentContext.Test.Properties.Get("Category")?.ToString() ?? "General";
            
            Test = ExtentTestManager.CreateTest(testName, description);
            
            // Agregar categoría con emoji
            if (category == "Smoke")
                Test.AssignCategory("🔥 Smoke");
            else if (category == "Regression")
                Test.AssignCategory("🔄 Regression");
            else
                Test.AssignCategory("📋 " + category);
            
            Test.Log(Status.Info, "🚀 Iniciando el test");
            
            Driver = DriverManager.GetDriver();
            Test.Log(Status.Info, "🌐 Navegador Chrome iniciado correctamente");
        }

        [TearDown]
        public void TearDown()
        {
            // Detener cronómetro
            _stopwatch.Stop();
            var duration = _stopwatch.Elapsed.TotalSeconds;

            var testStatus = TestContext.CurrentContext.Result.Outcome.Status;
            var errorMessage = TestContext.CurrentContext.Result.Message;

            // Registrar duración
            Test.Log(Status.Info, $"⏱️ Duración del test: {duration:0.00} segundos");

            if (testStatus == NUnit.Framework.Interfaces.TestStatus.Failed)
            {
                Test.Log(Status.Fail, "❌ Test FALLIDO");
                Test.Log(Status.Fail, $"💬 Error: {errorMessage}");
                
                TakeScreenshot();
            }
            else if (testStatus == NUnit.Framework.Interfaces.TestStatus.Passed)
            {
                Test.Log(Status.Pass, "✅ Test EXITOSO");
            }
            else if (testStatus == NUnit.Framework.Interfaces.TestStatus.Skipped)
            {
                Test.Log(Status.Skip, "⏭️ Test OMITIDO");
            }

            Test.Log(Status.Info, "🔒 Cerrando navegador");
            DriverManager.QuitDriver();
            
            ExtentTestManager.RemoveTest();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            ExtentManager.FlushReport();
            
            // Mostrar ubicación del reporte
            var reportPath = ExtentManager.GetReportPath();
            Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║          📊 REPORTE GENERADO EXITOSAMENTE               ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
            Console.WriteLine($"📁 Ubicación: {reportPath}");
            Console.WriteLine($"🌐 Para ver: Doble clic en el archivo HTML");
        }

        protected void TakeScreenshot()
        {
            try
            {
                var screenshot = ((ITakesScreenshot)Driver).GetScreenshot();
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var testName = TestContext.CurrentContext.Test.Name;
                var filename = $"{testName}_{timestamp}.png";
                
                var screenshotDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Screenshots");
                Directory.CreateDirectory(screenshotDir);
                
                var path = Path.Combine(screenshotDir, filename);
                screenshot.SaveAsFile(path);
                
                // Adjuntar al reporte
                Test.AddScreenCaptureFromPath(path, "Screenshot del error");
                Test.Log(Status.Info, $"📸 Screenshot capturado: {filename}");
            }
            catch (Exception ex)
            {
                Test.Log(Status.Warning, $"⚠️ No se pudo tomar screenshot: {ex.Message}");
            }
        }

        // Métodos de logging mejorados con emojis
        protected void LogInfo(string message)
        {
            Test.Log(Status.Info, $"ℹ️ {message}");
        }

        protected void LogPass(string message)
        {
            Test.Log(Status.Pass, $"✅ {message}");
        }

        protected void LogFail(string message)
        {
            Test.Log(Status.Fail, $"❌ {message}");
        }

        protected void LogWarning(string message)
        {
            Test.Log(Status.Warning, $"⚠️ {message}");
        }

        protected void LogStep(string stepName)
        {
            Test.Log(Status.Info, $"▶️ {stepName}");
        }
    }
}