using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LogManagement.Models;
using LogManagement.Managers;
using Newtonsoft.Json;

namespace LogManagementTests
{
    [TestClass]
    public class StaticLogEntryWrapperTests
    {
        ILogManager _manager = LogManager.GetInstance();

        [TestInitialize]
        public void Initialize()
        {
            _manager.RetrieveBusinessTransactionId = () =>
            {
                return "123";
            };

            _manager.RetrieveSessionId = () =>
            {
                return "abc";
            };

            _manager.RetrieveUser = () =>
            {
                return "sa";
            };

            
        }

        [TestMethod]
        public void TestLogEmission()
        {
            IStaticLogEntryWrapper staticLogCreator = new StaticLogEntryWrapper(_manager)
            {
                System = "Security System",
                Application = "Security Tester",
                Component = "Authentication Component",
                Event = "Validation"
            };

            string logString = string.Empty;

            _manager.LogEmit = log =>
            {
                if (log == null)
                    return;

                logString = JsonConvert.SerializeObject(log, Formatting.Indented);
            };

            ILogEntry entry = staticLogCreator.CreateLogEntry(Priority.Alert);

            _manager.EmitLog(entry);

            Assert.IsTrue(!string.IsNullOrEmpty(logString));
        }        

        [TestMethod]
        public void TestLogEmission2()
        {
            IStaticLogEntryWrapper staticLogCreator = new StaticLogEntryWrapper(
                _manager, "Security System")
            {
                Application = "Security Tester",
                Component = "Authentication Component",
                Event = "Validation"
            };

            string logString = string.Empty;

            _manager.LogEmit = log =>
            {
                if (log == null)
                    return;

                logString = JsonConvert.SerializeObject(log, Formatting.Indented);
            };

            ILogEntry entry = staticLogCreator.CreateLogEntry(Priority.Alert);

            _manager.EmitLog(entry);

            Assert.IsTrue(!string.IsNullOrEmpty(logString));
        }

        [TestMethod]
        public void TestLogEmission3()
        {
            IStaticLogEntryWrapper staticLogCreator = new StaticLogEntryWrapper(
                _manager,
                "Security System",
                "Security Tester")
            {
                Component = "Authentication Component",
                Event = "Validation"
            };

            string logString = string.Empty;

            _manager.LogEmit = log =>
            {
                if (log == null)
                    return;

                logString = JsonConvert.SerializeObject(log, Formatting.Indented);
            };

            ILogEntry entry = staticLogCreator.CreateLogEntry(Priority.Alert);

            _manager.EmitLog(entry);

            Assert.IsTrue(!string.IsNullOrEmpty(logString));
        }

        [TestMethod]
        public void TestLogEmission4()
        {
            IStaticLogEntryWrapper staticLogCreator = new StaticLogEntryWrapper(
                _manager,
                "Security System",
                "Security Tester",
                "Authentication Component")
            {
                Event = "Validation"
            };

            string logString = string.Empty;

            _manager.LogEmit = log =>
            {
                if (log == null)
                    return;

                logString = JsonConvert.SerializeObject(log, Formatting.Indented);
            };

            ILogEntry entry = staticLogCreator.CreateLogEntry(Priority.Alert);

            _manager.EmitLog(entry);

            Assert.IsTrue(!string.IsNullOrEmpty(logString));
        }

        [TestMethod]
        public void TestLogEmission5()
        {
            IStaticLogEntryWrapper staticLogCreator = new StaticLogEntryWrapper(
                _manager,
                "Security System",
                "Security Tester",
                "Authentication Component",
                "Validation")
            {
            };

            string logString = string.Empty;

            _manager.LogEmit = log =>
            {
                if (log == null)
                    return;

                logString = JsonConvert.SerializeObject(log, Formatting.Indented);
            };

            ILogEntry entry = staticLogCreator.CreateLogEntry(Priority.Alert);

            _manager.EmitLog(entry);

            Assert.IsTrue(!string.IsNullOrEmpty(logString));
        }

        [TestMethod]
        public void Test_LogBuilder()
        {
            IStaticLogEntryWrapper staticLogCreator = new StaticLogEntryWrapper(_manager);

            string logString = string.Empty;

            _manager.LogEmit = log =>
            {
                if (log == null)
                    return;

                logString = JsonConvert.SerializeObject(log, Formatting.Indented);
            };

            staticLogCreator
                .SetSystem("Security System")
                .SetApplication("Security Tester")
                .SetComponent("Authentication Component")
                .SetEvent("Validation")
                .EmitLog(Priority.Info, Status.Success, string.Empty);

            Assert.IsTrue(!string.IsNullOrEmpty(logString));
        }
    }
}
