using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LogManagement;
using LogManagement.Managers;
using LogManagement.Models;
using LogManagementTests.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogManagementTests
{
    [TestClass]
    public class LogAnalyzerTests
    {
        private List<ILogEntry> _inMemoryLogEntries = new List<ILogEntry>();
        private ILogRepository<ILogEntry> _logRepository;
        private ILogInserter<ILogEntry> _logInserter;
        private ILogMonitor<ILogEntry> _logMonitor;
        private ILogManager _manager = LogManager.GetInstance();
        private string _invokedRuleId = string.Empty;
        private ProducerConsumerLogQueue<ILogEntry> _logMonitorQueue;

        [TestInitialize]
        public void Initialize()
        {
            SetIdRetriever();
            SetLogRepository();
            SetLogInserter();
            SetLogMonitor();
            SetLogMonitorQueue();

            _manager.LogEmit = log =>
            {
                if (log == null)
                    return;

                _logInserter.Insert(log);

                //-->> Normalize log persistency table here (if necessary) prior to analysis of log entries
                _logMonitor.Evaluate(log);
                //-->> Clear log persistency table here (if necessary)
            };
        }

        [TestCleanup]
        public void MockedDestructor()
        {
            DestroyLogMonitorQueue();
        }

        void SetIdRetriever()
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
                },
                (addedLogEntity, postInsertRepository) =>
                {
                    /*Post-insert*/
                }
            );
        }

        void SetLogMonitor()
        {
            _logMonitor = new LogMonitor<ILogEntry>(_manager,
               _logRepository,
               new List<ILogTrigger<ILogEntry>>
                {
                    {
                        new LogTrigger<ILogEntry>("0001",
                            (id, entity, repository, logger) =>
                            {
                                /*Trigger Evaluation*/
                                IEnumerable<ILogEntry> result = repository
                                    .Matching(new GetSuccessValidationDescriptionLogsQuery.Criteria());

                                bool isMatched = result.Any() && (entity.Parameters["Description"].ToString() == GetSuccessValidationDescriptionLogsQuery.DESCRIPTION);

                                return isMatched;
                            },
                            (id, entity, repository, logger) =>
                            {
                                /*Trigger Invocation*/
                                _invokedRuleId = id;
                            })
                    },
                    {
                        new LogTrigger<ILogEntry>("0002",
                            (id, entity, repository, logger) =>
                            {
                                /*Trigger Evaluation*/
                                IEnumerable<ILogEntry> result = repository
                                    .Matching(new GetFailedInvocationLogsQuery.Criteria());

                                bool isMatched = result.Any() && (entity.Status == GetFailedInvocationLogsQuery.FAILED_STATUS);

                                return isMatched;
                            },
                            (id, entity, repository, logger) =>
                            {
                                /*Trigger Invocation*/
                                _invokedRuleId = id;
                            })
                    },
                }
           );
        }

        #region Monitor Queue Intricacies
        void SetLogMonitorQueue()
        {
            if (_logMonitorQueue == null) _logMonitorQueue = new ProducerConsumerLogQueue<ILogEntry>(_logMonitor);
        }

        void DestroyLogMonitorQueue()
        {
            if (_logMonitorQueue != null)
            {
                _logMonitorQueue.Dispose();

                _logMonitorQueue = null;

                string message = "Log monitor queue disposed";

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

            staticLogCreator
                .AddParameters("Description", "Validation has been invoked successfully")
                .EmitLog(Priority.Info, Status.Success);

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

            staticLogCreator.EmitLog(Priority.Info, Status.Failure);
            staticLogCreator.EmitLog(Priority.Info, Status.Failure);
            Assert.AreEqual("0002", _invokedRuleId);
        }
    }
}
