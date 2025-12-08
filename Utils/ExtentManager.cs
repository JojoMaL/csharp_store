using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using System;
using System.IO;

namespace SeleniumFramework.Utils
{
    public class ExtentManager
    {
        private static ExtentReports? _extent;
        private static string _reportPath = string.Empty;

        public static ExtentReports GetExtent()
        {
            if (_extent == null)
            {
                // Crear carpeta de reportes si no existe
                var reportDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports");
                Directory.CreateDirectory(reportDirectory);

                // Nombre del reporte con fecha y hora
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                _reportPath = Path.Combine(reportDirectory, $"TestReport_{timestamp}.html");

                // Configurar el reporte HTML
                var htmlReporter = new ExtentHtmlReporter(_reportPath);

                // ===== PERSONALIZACIÓN DEL DISEÑO =====
                
                // Tema: Dark o Standard
                htmlReporter.Config.Theme = AventStack.ExtentReports.Reporter.Configuration.Theme.Dark;
                
                // Información del encabezado
                htmlReporter.Config.DocumentTitle = "🛒 E-Commerce Test Report";
                htmlReporter.Config.ReportName = "Test Store - Automation Results";
                
                // Personalizar encoding
                htmlReporter.Config.Encoding = "UTF-8";
                
                // Mostrar u ocultar elementos
                htmlReporter.Config.EnableTimeline = true; // Línea de tiempo
                
                // CSS personalizado para más estilo
                htmlReporter.Config.CSS = @"
                    .test-name { font-size: 16px; font-weight: bold; }
                    .category-name { background-color: #4CAF50; }
                ";

                // Crear la instancia de ExtentReports
                _extent = new ExtentReports();
                _extent.AttachReporter(htmlReporter);

                // ===== INFORMACIÓN DEL SISTEMA =====
                _extent.AddSystemInfo("👤 Tester", "Jose Mantecon Luengas");
                _extent.AddSystemInfo("🌐 Ambiente", "QA");
                _extent.AddSystemInfo("🖥️ Sistema Operativo", Environment.OSVersion.ToString());
                _extent.AddSystemInfo("🔧 .NET Version", Environment.Version.ToString());
                _extent.AddSystemInfo("🌍 Browser", "Chrome");
                _extent.AddSystemInfo("📅 Fecha de Ejecución", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                _extent.AddSystemInfo("🔗 URL Base", "https://teststore.automationtesting.co.uk");

                Console.WriteLine($"✅ Reporte configurado: {_reportPath}");
            }

            return _extent;
        }

        public static void FlushReport()
        {
            _extent?.Flush();
            Console.WriteLine($"📊 Reporte generado exitosamente en: {_reportPath}");
        }

        public static string GetReportPath()
        {
            return _reportPath;
        }
    }
}