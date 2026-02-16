# 🧪 Selenium C# Automation Framework

Framework de automatización web con Selenium, C# y NUnit con Page Object Model.

## 🚀 Características

- ✅ Page Object Model (POM)
- ✅ Selenium WebDriver
- ✅ NUnit Testing Framework
- ✅ ExtentReports para reportes HTML
- ✅ Screenshots automáticos en fallos
- ✅ Configuración centralizada
- ✅ Logs detallados

## 📋 Pre-requisitos

- .NET SDK 6.0 o superior
- Visual Studio Code
- Google Chrome

```

## 🏗️ Estructura del Proyecto
```
selenium-csharp-framework/
├── Config/                 # Configuración
│   └── ConfigReader.cs
├── Drivers/               # Gestión de WebDriver
│   └── DriverManager.cs
├── Pages/                 # Page Objects
│   └── BasePage.cs
├── Tests/                 # Tests
│   └── BaseTest.cs
├── Utils/                 # Utilidades
│   ├── ExtentManager.cs
│   └── ExtentTestManager.cs
├── Reports/               # Reportes HTML
├── Screenshots/           # Capturas de pantalla
└── config.json           # Configuración
```

## 📝 Uso

### Crear un nuevo Page Object
```csharp
using OpenQA.Selenium;

namespace YourProject.Pages
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

### Crear un nuevo Test
```csharp
using NUnit.Framework;

namespace YourProject.Tests
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

## 🧪 Ejecutar Tests
```bash
# Todos los tests
dotnet test

# Tests específicos por categoría
dotnet test --filter "TestCategory=Smoke"

# Con logs detallados
dotnet test --logger "console;verbosity=detailed"
```

## 📊 Ver Reportes

Los reportes HTML se generan automáticamente en la carpeta `Reports/`.
```bash
# Abrir último reporte (Mac)
open "$(find bin/Debug/net8.0/Reports -name '*.html' | sort | tail -1)"
```
## ✉️ Contacto

Jose Mantecon Luengas  - mantecon95@gmail.com
