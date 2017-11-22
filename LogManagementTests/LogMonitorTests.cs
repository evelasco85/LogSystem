using System;
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
using NLog;

namespace LogManagementTests
{
    [TestClass]
    public class LogMonitorTests
    {
        private List<ILogEntry> _inMemoryLogEntries = new List<ILogEntry>();

        private Logger _nLogger = NLog.LogManager.GetLogger("LogMonitorTests");

        private ILogRepository<ILogEntry> _logRepository;
        private ILogInserter<ILogEntry> _logInserter;
        private ILogMonitor<ILogEntry> _logMonitor;
        private ILogManager _manager = LogManagement.Managers.LogManager.GetInstance();
        private IProducerConsumerLogQueue<ILogEntry> _logProcessorQueue;

        private bool _isLockout = false;
        private int _failedLoginCount = 0;

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
            _isLockout = false;
            _failedLoginCount = 0;

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

                    string description = string.Format("Log: {0}", JsonConvert.SerializeObject(logEntityToAdd));

                    switch(logEntityToAdd.Priority)
                    {
                        case Priority.Debug:
                            _nLogger.Log(LogLevel.Debug, description);
                            break;
                        case Priority.Info:
                            _nLogger.Log(LogLevel.Info, description);
                            break;
                        case Priority.Notice:
                        case Priority.Warning:
                            _nLogger.Log(LogLevel.Warn, description);
                            break;
                        case Priority.Error:
                            _nLogger.Log(LogLevel.Error, description);
                            break;
                        case Priority.Critical:
                        case Priority.Alert:
                        case Priority.Emergency:
                            _nLogger.Log(LogLevel.Fatal, description);
                            break;
                    }

                    Debug.WriteLine(string.Empty);
                    Debug.WriteLine(description);
                    Console.WriteLine(string.Empty);
                    Console.WriteLine(description);
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

                string descriptionHeader = ">>[Trigger(s) invoked against log entry]";

                Debug.WriteLine(descriptionHeader);
                Console.WriteLine(descriptionHeader);

                for (int index = 0; index < list.Count; index++)
                {
                    ILogTriggerInfo triggerInvoked = list[index];
                    string description = string.Format(">>***Trigger Id Invoked: {0}", triggerInvoked.TriggerId);

                    Debug.WriteLine(description);
                    Console.WriteLine(description);
                }
            };

            _logMonitor.TriggerInvocationComplete = (info, entity, repository, logger) =>
            {
                //Completed per-trigger invoked in log evaluation

                string description = string.Format(">>Trigger Id: {1} | Failed login count: {0:00}",
                    _failedLoginCount,
                    info.TriggerId);

                Debug.WriteLine(description);
                Console.WriteLine(description);

                if (_isLockout)
                {
                    string lockoutMessage = ">>>!!!Lockout was performed!!!<<<";

                    Debug.WriteLine(lockoutMessage);
                    Console.WriteLine(lockoutMessage);
                }

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

                    _failedLoginCount += 1;
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

                    _isLockout = true;
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

                string message = "[Log processor queue disposed]";

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

            Assert.AreEqual(2, _failedLoginCount);
            Assert.IsFalse(_isLockout);
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

            Assert.AreEqual(3, _failedLoginCount);
            Assert.IsTrue(_isLockout);
        }
    }
}
