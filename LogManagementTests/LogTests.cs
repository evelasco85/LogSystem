﻿using System;
using LogManagement.Managers;
using LogManagement.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace LogManagementTests
{
    [TestClass]
    public class LogTests
    {
        private ILogManager _manager = LogManager.GetInstance();

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
            string logString = string.Empty;

            _manager.LogEmit = log =>
            {
                if (log == null)
                    return;

                logString = JsonConvert.SerializeObject(log, Formatting.Indented);
            };

            ILogEntry entry = _manager.CreateLogEntry(Priority.Alert);

            entry.System = "Security System";
            entry.Application = "Security Tester";
            entry.Component = "Authentication Component";
            entry.Event = "Validation";
            entry.Status = Status.Failure;

            entry.Parameters.Add("Param 1", "value 1");
            entry.Parameters.Add("Param 2", "value 2");
            entry.Parameters.Add("Description", "Validation has been invoked but was failed");

            _manager.EmitLog(entry);

            Assert.IsTrue(!string.IsNullOrEmpty(logString));
        }
    }
}
