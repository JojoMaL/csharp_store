# 🧪 Selenium C# Automation Framework

Framework de automatización web end-to-end para una tienda online, construido con **Selenium WebDriver**, **C#**, **NUnit** y el patrón **Page Object Model (POM)**. Incluye doble reporte de resultados: **ExtentReports** (HTML autocontenido) y **Allure** (reporte interactivo).

## 🚀 Características

- ✅ Page Object Model (POM)
- ✅ Selenium WebDriver + ChromeDriver gestionado automáticamente
- ✅ NUnit como framework de testing
- ✅ Reportes en HTML con **ExtentReports**
- ✅ Reportes interactivos con **Allure**
- ✅ Screenshots automáticos en fallos (adjuntos a ambos reportes)
- ✅ Configuración centralizada vía `config.json`
- ✅ Logs detallados por paso (`LogInfo`, `LogPass`, `LogFail`, `LogWarning`)

## 📋 Pre-requisitos

- [.NET SDK 8.0](https://dotnet.microsoft.com/download) o superior
- Google Chrome instalado
- (Opcional, solo para el reporte Allure) [Allure Commandline](https://allurereport.org/docs/install/) — se puede instalar con `npm install -g allure-commandline` o `scoop install allure`

## 🏗️ Estructura del proyecto

```
csharp_store/
├── Config/                    # Lectura de configuración
│   └── ConfigReader.cs
├── Drivers/                   # Ciclo de vida del WebDriver
│   └── DriverManager.cs
├── Pages/                     # Page Objects
│   ├── BasePage.cs
│   ├── HomePage.cs
│   ├── LoginPage.cs
│   ├── SearchPage.cs
│   ├── CategoryPage.cs
│   ├── ProductPage.cs
│   └── CartPage.cs
├── Tests/                     # Suites de test
│   ├── BaseTest.cs            # Setup/TearDown, logging y wiring de reportes
│   ├── GlobalTestSetup.cs     # Abre/cierra el reporte HTML una sola vez por corrida
│   ├── LoginTest.cs
│   ├── NavegationTest.cs
│   ├── SearchTest.cs
│   └── CartTest.cs
├── Utils/                     # Utilidades de reporte
│   ├── ExtentManager.cs       # Configura y escribe el reporte HTML
│   └── ExtentTestManager.cs   # Registra pasos/resultados por test
├── config.json                 # baseUrl y browser a usar
├── allureConfig.json           # Carpeta donde Allure escribe sus resultados crudos
└── selenium_tienda_csharp.csproj
```

> `Reports/`, `Screenshots/`, `bin/` y `obj/` no están versionados (ver `.gitignore`): se generan automáticamente al ejecutar los tests, por eso no aparecen en un clon nuevo del repositorio.

## ⚙️ Configuración

Editá `config.json` para apuntar a otro ambiente o navegador:

```json
{
  "baseUrl": "https://www.google.com",
  "browser": "chrome"
}
```

## 🧪 Ejecutar los tests

```bash
# Restaurar dependencias (primera vez)
dotnet restore

# Correr todos los tests
dotnet test

# Correr una categoría específica (ej: solo pruebas de humo)
dotnet test --filter "TestCategory=Smoke"

# Con salida detallada en consola
dotnet test --logger "console;verbosity=detailed"
```

## 📊 Dónde encontrar el reporte de resultados

Este proyecto genera **dos** reportes distintos en cada corrida:

### 1. Reporte HTML (ExtentReports)

Se crea automáticamente en la carpeta `Reports/` **dentro de la carpeta de salida del build** (no en la raíz del proyecto), con un nombre con fecha y hora, por ejemplo `TestReport_20260703_143000.html`.

Al finalizar `dotnet test`, la consola imprime la ruta exacta:
```
📊 Reporte generado exitosamente en: /ruta/completa/TestReport_20260703_143000.html
```

Rutas típicas según el sistema operativo:

```bash
# macOS / Linux
open "$(find bin/Debug/net8.0/Reports -name '*.html' | sort | tail -1)"

# Windows (PowerShell)
Invoke-Item (Get-ChildItem -Path .\bin\Debug\net8.0\Reports -Filter *.html | Sort-Object Name | Select-Object -Last 1).FullName
```

Si la carpeta `Reports/` no aparece, confirmá que corriste `dotnet test` al menos una vez después de clonar el repositorio — al ser una carpeta ignorada por git, no viene incluida en el clon.

### 2. Reporte interactivo (Allure)

Los tests están decorados con `[AllureNUnitAttribute]`, así que cada corrida escribe resultados crudos (JSON) en la carpeta `allure-results/` (definida en `allureConfig.json`). Ese JSON **no es el reporte final**: hace falta generarlo con el Allure Commandline:

```bash
# Generar el reporte HTML a partir de los resultados
allure generate allure-results --clean -o allure-report

# Generarlo y abrirlo directamente en el navegador
allure serve allure-results
```

## 📝 Extender el framework

### Agregar un nuevo Page Object

```csharp
using OpenQA.Selenium;

namespace selenium_tineda_csharp.Pages
{
    public class LoginPage : BasePage
    {
        private By _usernameField = By.Id("username");
        private By _passwordField = By.Id("password");

        public LoginPage(IWebDriver driver) : base(driver)
        {
        }

        public void Login(string username, string password)
        {
            Type(_usernameField, username);
            Type(_passwordField, password);
        }
    }
}
```

### Agregar un nuevo test

```csharp
using NUnit.Framework;

namespace selenium_tineda_csharp.Test
{
    [TestFixture]
    public class LoginTests : BaseTest
    {
        [Test]
        public void Test_Login()
        {
            LogInfo("Iniciando test de login");
            // Tu código aquí
            LogPass("Login exitoso");
        }
    }
}
```

Los métodos `LogInfo`, `LogPass`, `LogFail`, `LogWarning` y `LogStep` de `BaseTest` quedan reflejados automáticamente tanto en el reporte de Allure como en el HTML de ExtentReports, así que no hace falta llamar a ambos reporteros por separado.

## 🩺 Troubleshooting

| Problema | Causa probable | Solución |
|---|---|---|
| No encuentro la carpeta `Reports/` | Es una carpeta generada, ignorada por git | Corré `dotnet test` primero |
| `allure` no se reconoce como comando | Allure Commandline no está instalado | `npm install -g allure-commandline` |
| El reporte HTML queda vacío o a medias | La corrida se interrumpió antes de terminar | `GlobalTestSetup` hace flush del reporte en `OneTimeTearDown`; esperá a que la corrida termine sola |

## ✉️ Contacto

Jose Mantecon Luengas — mantecon95@gmail.com