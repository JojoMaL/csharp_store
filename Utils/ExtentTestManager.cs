using AventStack.ExtentReports;
using System.Collections.Generic;

namespace SeleniumFramework.Utils
{
    public class ExtentTestManager
    {
        private static Dictionary<int, ExtentTest> _testMap = new Dictionary<int, ExtentTest>();
        private static object _lock = new object();

        public static ExtentTest CreateTest(string testName, string description = "")
        {
            lock (_lock)
            {
                var extent = ExtentManager.GetExtent();
                var test = extent.CreateTest(testName, description);
                
                // Guardar referencia del test
                var threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
                if (_testMap.ContainsKey(threadId))
                {
                    _testMap[threadId] = test;
                }
                else
                {
                    _testMap.Add(threadId, test);
                }

                return test;
            }
        }

        public static ExtentTest? GetTest()
        {
            lock (_lock)
            {
                var threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
                return _testMap.ContainsKey(threadId) ? _testMap[threadId] : null;
            }
        }

        public static void RemoveTest()
        {
            lock (_lock)
            {
                var threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
                if (_testMap.ContainsKey(threadId))
                {
                    _testMap.Remove(threadId);
                }
            }
        }

        // Métodos helper para agregar información con emojis
        public static void LogStep(string stepDescription)
        {
            GetTest()?.Info($"▶️ {stepDescription}");
        }

        public static void LogSuccess(string message)
        {
            GetTest()?.Pass($"✅ {message}");
        }

        public static void LogWarning(string message)
        {
            GetTest()?.Warning($"⚠️ {message}");
        }

        public static void LogError(string message)
        {
            GetTest()?.Fail($"❌ {message}");
        }
    }
}