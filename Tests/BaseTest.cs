using NUnit.Framework;
using OpenQA.Selenium;
using SeleniumFramework.Drivers;
using Allure.Net.Commons;
using Allure.NUnit;
using System;
using System.IO;



namespace selenium_tineda_csharp.Tests
{
    /// <summary>
    /// Clase base para todos los tests con integración de Allure Reports
    /// </summary>
    [AllureNUnitAttribute]
    public class BaseTest
    {
        protected IWebDriver Driver;
        private DateTime _testStartTime;

        [SetUp]
        public void Setup()
        {
            _testStartTime = DateTime.Now;
            
            // Obtener información del test actual
            var testName = TestContext.CurrentContext.Test.Name;
            var category = TestContext.CurrentContext.Test.Properties.Get("Category")?.ToString() ?? "General";
            
            AllureLifecycle.Instance.UpdateTestCase(testResult =>
            {
                testResult.labels.Add(new Label { name = "suite", value = GetType().Name });
                testResult.labels.Add(new Label { name = "tag", value = category });
            });
            
            // Iniciar el navegador
            Driver = DriverManager.GetDriver();
            
            AddStep("Test iniciado correctamente");
        }

        [TearDown]
        public void TearDown()
        {
            var testStatus = TestContext.CurrentContext.Result.Outcome.Status;
            var errorMessage = TestContext.CurrentContext.Result.Message;
            var duration = DateTime.Now - _testStartTime;

            // Agregar información de duración
            AddStep($"Duración del test: {duration.TotalSeconds:0.00} segundos");

            // Capturar screenshot si el test falló
            if (testStatus == NUnit.Framework.Interfaces.TestStatus.Failed)
            {
                AddStep($"Test FALLIDO: {errorMessage}");
                TakeScreenshot("error_screenshot");
            }

            // Cerrar navegador
            try
            {
                DriverManager.QuitDriver();
                AddStep("Navegador cerrado correctamente");
            }
            catch (Exception ex)
            {
                AddStep($"Error al cerrar navegador: {ex.Message}");
            }
        }

        // ========== MÉTODOS HELPER PARA ALLURE ==========

        /// <summary>
        /// Agrega un paso al reporte de Allure
        /// </summary>
        protected void AddStep(string stepName)
        {
            AllureLifecycle.Instance.UpdateTestCase(x => 
            {
                // El paso se registra en el contexto actual
            });
            
            // También escribir en TestContext para que aparezca en la consola de NUnit
            TestContext.WriteLine($"[STEP] {stepName}");
        }

        /// <summary>
        /// Captura un screenshot y lo adjunta al reporte
        /// </summary>
protected void TakeScreenshot(string screenshotName = "screenshot")
{
    try
    {
        var screenshot = ((ITakesScreenshot)Driver).GetScreenshot();
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var filename = $"{screenshotName}_{timestamp}.png";
        
        var screenshotDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Screenshots");
        Directory.CreateDirectory(screenshotDir);
        
        var path = Path.Combine(screenshotDir, filename);
        screenshot.SaveAsFile(path);
        
        // Adjuntar a Allure
        var screenshotBytes = File.ReadAllBytes(path);
        AllureApi.AddAttachment("Screenshot", "image/png", screenshotBytes);
        
        TestContext.WriteLine($"Screenshot capturado: {filename}");
    }
    catch (Exception ex)
    {
        TestContext.WriteLine($"No se pudo tomar screenshot: {ex.Message}");
    }
}

        /// <summary>
        /// Adjunta texto al reporte de Allure
        /// </summary>
        protected void AttachText(string name, string content)
        {
           AllureApi.AddAttachment(name,"text/plain",System.Text.Encoding.UTF8.GetBytes(content));

;

        }

        /// <summary>
        /// Adjunta JSON al reporte de Allure
        /// </summary>
        protected void AttachJson(string name, string jsonContent)
        {
            AllureApi.AddAttachment(name,"application/json",System.Text.Encoding.UTF8.GetBytes(jsonContent));


        }

        // ========== MÉTODOS DE LOGGING SIMPLIFICADOS ==========

        /// <summary>
        /// Log informativo
        /// </summary>
        protected void LogInfo(string message)
        {
            AddStep($"ℹ️ {message}");
        }

        /// <summary>
        /// Log de éxito
        /// </summary>
        protected void LogPass(string message)
        {
            AddStep($"✅ {message}");
        }

        /// <summary>
        /// Log de error/fallo
        /// </summary>
        protected void LogFail(string message)
        {
            AddStep($"❌ {message}");
        }

        /// <summary>
        /// Log de advertencia
        /// </summary>
        protected void LogWarning(string message)
        {
            AddStep($"⚠️ {message}");
        }

        /// <summary>
        /// Log de paso de test
        /// </summary>
        protected void LogStep(string stepName)
        {
            AddStep($"▶️ {stepName}");
        }
    }
}