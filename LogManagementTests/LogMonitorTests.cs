﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LogManagement;
using LogManagement.Managers;
using LogManagement.Models;
using LogManagement.ProducerConsumerLogQueue;
using LogManagementTests.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace LogManagementTests
{
    [TestClass]
    public class LogMonitorTests
    {
        private List<ILogEntry> _inMemoryLogEntries = new List<ILogEntry>();
        private ILogRepository<ILogEntry> _logRepository;
        private ILogInserter<ILogEntry> _logInserter;
        private ILogMonitor<ILogEntry> _logMonitor;
        private ILogManager _manager = LogManager.GetInstance();
        private string _lastTriggerIdInvoked = string.Empty;
        private IProducerConsumerLogQueue<ILogEntry> _logProcessorQueue;

        private string _businessTransactionId = "123";
        private string _sessionId = "abc";
        private string _user = "sa";

        public LogMonitorTests()
        {
            SetIdRetriever();
            SetLogRepository();
            SetLogInserter();
            SetLogMonitor();
            SetLogProcessorQueue();
        }

        ~LogMonitorTests()
        {
            DestroyLogProcessorQueue();
        }

        [TestInitialize]
        public void Initialize()
        {
            _manager.LogEmit = log =>
            {
                if (log == null)
                    return;

                _logProcessorQueue.EnqueueLog(log);
            };
        }

        #region Setup
        void SetIdRetriever()
        {
            _manager.RetrieveBusinessTransactionId = () =>
            {
                return _businessTransactionId;
            };

            _manager.RetrieveSessionId = () =>
            {
                return _sessionId;
            };

            _manager.RetrieveUser = () =>
            {
                return _user;
            };
        }

        void SetLogRepository()
        {
            _logRepository = new LogRepository<ILogEntry>(new List<IBaseLogQueryObject<ILogEntry>>
            {
                {new GetFailedLoginAttemptsQuery(_inMemoryLogEntries)},
            });
        }

        void SetLogInserter()
        {
            _logInserter = new LogInserter<ILogEntry>(_logRepository,
                (logEntityToAdd, preInsertRepository) =>
                {
                    /*Pre-insert*/
                },
                (logEntityToAdd, currentRepository) =>
                {
                    /*Insert*/
                    _inMemoryLogEntries.Add(logEntityToAdd);

                    Debug.WriteLine(JsonConvert.SerializeObject(logEntityToAdd));
                    Console.WriteLine(JsonConvert.SerializeObject(logEntityToAdd));
                },
                (addedLogEntity, postInsertRepository) =>
                {
                    /*Post-insert*/
                }
            );
        }

        void SetLogMonitor()
        {
            List<ILogTrigger<ILogEntry>> triggers = new List<ILogTrigger<ILogEntry>>
            {
                GetFailedLoginTrigger(),
                GetAccountLockoutTrigger(),
            };

            _logMonitor = new LogMonitor<ILogEntry>(_manager, _logRepository, triggers);

            _logMonitor.LogEvaluationComplete = (list, entity, repository, logger) =>
            {
                //Completed per-log evaluation

                Debug.WriteLine("Trigger(s) invoked against log entry");
                Console.WriteLine("Trigger(s) invoked against log entry");

                for (int index = 0; index < list.Count; index++)
                {
                    ILogTriggerInfo triggerInvoked = list[index];
                    string description = string.Format("Trigger Id Invoked: {0}", triggerInvoked.TriggerId);

                    Debug.WriteLine(description);
                    Console.WriteLine(description);
                }
            };

            _logMonitor.TriggerInvocationComplete = (info, entity, repository, logger) =>
            {
                //Completed per-trigger invoked in log evaluation

                _lastTriggerIdInvoked = info.TriggerId;
            };
        }
#endregion

        #region Triggers
        ILogTrigger<ILogEntry> GetFailedLoginTrigger()
        {
            return new LogTrigger<ILogEntry>("0001",
                (triggerId, logEntity, repository) =>
                { 
                    /*Trigger Evaluation*/
                    return logEntity.Event == "Invalid Login";
                },
                (triggerId, entity, repository) =>
                {
                    /*Trigger Invocation*/

                    Debug.WriteLine(JsonConvert.SerializeObject(entity));
                    Console.WriteLine(JsonConvert.SerializeObject(entity));
                });
        }

        ILogTrigger<ILogEntry> GetAccountLockoutTrigger()
        {
            return new LogTrigger<ILogEntry>("0002",
                (triggerId, logEntity, repository) =>
                {
                    /*Trigger Evaluation*/
                    IEnumerable<ILogEntry> result = repository
                        .Matching(new GetFailedLoginAttemptsQuery.Criteria());

                    bool isLockout = result.Count() >= 3;

                    return isLockout;
                },
                (triggerId, logEntity, repository) =>
                {
                    /*Trigger Invocation*/

                    Debug.WriteLine(JsonConvert.SerializeObject(logEntity));
                    Console.WriteLine(JsonConvert.SerializeObject(logEntity));
                });
        }
#endregion
       
        #region Log Processor Queue Setup and Destruction
        void SetLogProcessorQueue()
        {
            if (_logProcessorQueue == null)
                _logProcessorQueue = new LogProcessorQueue<ILogEntry>(_logInserter, _logMonitor);
        }

        void DestroyLogProcessorQueue()
        {
            if (_logProcessorQueue != null)
            {
                _logProcessorQueue.Dispose();

                _logProcessorQueue = null;

                string message = "Log processor queue disposed";

                Debug.WriteLine(message);
                Console.WriteLine(message);
            }
        }
        #endregion 

        [TestMethod]
        public void TestMultipleLoginAttempt()
        {
            IStaticLogEntryWrapper staticLogCreator = new StaticLogEntryWrapper(_manager)
            {
                System = "Security System",
                Application = "Security Tester",
                Component = "Authentication Component",
            };

            staticLogCreator
                .SetEvent("Invalid Login")
                .EmitLog(Priority.Warning, Status.Failure);     //First attempt
            staticLogCreator
                .SetEvent("Invalid Login")
                .EmitLog(Priority.Warning, Status.Failure);     //Second attempt

            while (!_logProcessorQueue.IsEmpty) ;     //Wait for queue to complete(optional, but required during this test)

            Assert.AreEqual("0001", _lastTriggerIdInvoked);
        }

        [TestMethod]
        public void TestLockout()
        {
            IStaticLogEntryWrapper staticLogCreator = new StaticLogEntryWrapper(_manager)
            {
                System = "Security System",
                Application = "Security Tester",
                Component = "Authentication Component",
            };

            staticLogCreator
                .SetEvent("Invalid Login")
                .EmitLog(Priority.Warning, Status.Failure);     //First attempt
            staticLogCreator
                .SetEvent("Invalid Login")
                .EmitLog(Priority.Warning, Status.Failure);     //Second attempt
            staticLogCreator
                .SetEvent("Invalid Login")
                .EmitLog(Priority.Warning, Status.Failure);     //Third attempt.... LOCKOUT INVOCATION!!!

            while (!_logProcessorQueue.IsEmpty) ;     //Wait for queue to complete(optional, but required during this test)

            Assert.AreEqual("0002", _lastTriggerIdInvoked);
        }
    }
}
