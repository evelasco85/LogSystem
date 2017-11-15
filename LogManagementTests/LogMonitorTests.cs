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
        private string _invokedRuleId = string.Empty;
        private IProducerConsumerLogQueue<ILogEntry> _logProcessorQueue;

        private string _businessTransactionId = "123";
        private string _sessionId = "abc";
        private string _user = String.Empty;

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
                {new GetFailedInvocationLogsQuery(_inMemoryLogEntries)},
                {new GetSuccessValidationDescriptionLogsQuery(_inMemoryLogEntries)},
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
                GetSuccessfulValidationTrigger(),
                GetFailedInvocationTrigger(),
            };

            _logMonitor = new LogMonitor<ILogEntry>(_manager, _logRepository, triggers);
        }
#endregion

        #region Triggers
        ILogTrigger<ILogEntry> GetSuccessfulValidationTrigger()
        {
            return new LogTrigger<ILogEntry>("0001",
                (triggerId, logEntity, repository, logger) =>
                { 
                    /*Trigger Evaluation*/
                    IEnumerable<ILogEntry> result = repository
                        .Matching(new GetSuccessValidationDescriptionLogsQuery.Criteria());

                    bool isMatched = result.Any() && (logEntity.Parameters["Description"].ToString() ==
                                                      GetSuccessValidationDescriptionLogsQuery.DESCRIPTION);

                    return isMatched;
                },
                (triggerId, logRetriever, repository, logger) =>
                {
                    /*Trigger Invocation*/
                    _invokedRuleId = triggerId;
                });
        }

        ILogTrigger<ILogEntry> GetFailedInvocationTrigger()
        {
            return new LogTrigger<ILogEntry>("0002",
                (triggerId, logEntity, repository, logger) =>
                {
                    /*Trigger Evaluation*/
                    IEnumerable<ILogEntry> result = repository
                        .Matching(new GetFailedInvocationLogsQuery.Criteria());

                    bool isMatched = result.Any() && (logEntity.Status == GetFailedInvocationLogsQuery.FAILED_STATUS);

                    return isMatched;
                },
                (triggerId, logRetriever, repository, logger) =>
                {
                    /*Trigger Invocation*/
                    _invokedRuleId = triggerId;
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
        public void TestMethod1()
        {
            IStaticLogEntryWrapper staticLogCreator = new StaticLogEntryWrapper(_manager)
            {
                System = "Security System",
                Application = "Security Tester",
                Component = "Authentication Component",
                Event = "Validation"
            };

            _user = "sa";

            staticLogCreator
                .AddParameters("Description", "Validation has been invoked successfully")
                .EmitLog(Priority.Info, Status.Success);

            while (!_logProcessorQueue.IsEmpty) ;     //Wait for queue to complete

            Assert.AreEqual("0001", _invokedRuleId);
        }

        [TestMethod]
        public void TestMethod2()
        {
            IStaticLogEntryWrapper staticLogCreator = new StaticLogEntryWrapper(_manager)
            {
                System = "Security System",
                Application = "Security Tester",
                Component = "Authentication Component",
                Event = "Validation"
            };

            _user = "sa";

            staticLogCreator.EmitLog(Priority.Info, Status.Failure);
            staticLogCreator.EmitLog(Priority.Info, Status.Failure);

            while (!_logProcessorQueue.IsEmpty) ;     //Wait for queue to complete

            Assert.AreEqual("0002", _invokedRuleId);
        }
    }
}
