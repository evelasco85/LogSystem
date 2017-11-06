using System;
using System.Collections.Generic;
using System.Linq;
using LogManagement;
using LogManagement.Dynamic.Managers;
using LogManagement.Models;
using LogManagementTests.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogManagementTests
{
    [TestClass]
    public class LogAnalyzerTests
    {
        private IList<LogEntryKVP> _inMemoryLogEntries = new List<LogEntryKVP>();
        private ILogRepository<LogEntryKVP> _logRepository;
        private ILogPersistency<LogEntryKVP> _logPersistency;
        private ILogAnalyzer<LogEntryKVP> _logAnalyzer;
        private ILogManager _manager = LogManager.GetInstance();
        private string _invokedRuleId = string.Empty;

        [TestInitialize]
        public void Initialize()
        {
            _logRepository = new LogRepository<LogEntryKVP>(new List<IBaseLogQueryObject<LogEntryKVP>>
            {
                {new GetFailedInvocationLogsQuery(_inMemoryLogEntries)},
                {new GetFailedValidationDescriptionLogsQuery(_inMemoryLogEntries)},
            });
            _logPersistency = new LogPersistency<LogEntryKVP>(_logRepository,
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
            _logAnalyzer = new LogAnalyzer<LogEntryKVP>(_logRepository,
                new List<ILogTrigger<LogEntryKVP>>
                {
                    {
                        new LogTrigger<LogEntryKVP>("0001",
                            (entity, repository) =>
                            {
                                /*Trigger Evaluation*/
                                IEnumerable<LogEntryKVP> result = repository
                                    .Matching(new GetFailedValidationDescriptionLogsQuery.Criteria());

                                return result.Any();
                            },
                            (id, entity, repository) =>
                            {
                                /*Trigger Invocation*/
                                _invokedRuleId = id;
                            })
                    },
                    {
                        new LogTrigger<LogEntryKVP>("0002",
                            (entity, repository) =>
                            {
                                /*Trigger Evaluation*/
                                IEnumerable<LogEntryKVP> result = repository
                                    .Matching(new GetFailedInvocationLogsQuery.Criteria());

                                return result.Any();
                            },
                            (id, entity, repository) =>
                            {
                                /*Trigger Invocation*/
                                _invokedRuleId = id;
                            })
                    },
                }
            );

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

            _manager.LogEmit = log =>
            {
                if (log == null)
                    return;

                string transactionId = log.TransactionId;
                Priority priority = log.Priority;
                string logId = log.Id;
                string logCreatorId = log.LogCreatorId;

                IList<LogEntryKVP> logEntities = new List<LogEntryKVP>
                {
                    new LogEntryKVP {LogId = logId, LogCreatorId = logCreatorId, TransactionId = transactionId, Priority = priority, Key = "System", Value = log.System},
                    new LogEntryKVP {LogId = logId, LogCreatorId = logCreatorId, TransactionId = transactionId, Priority = priority, Key = "Application", Value = log.Application},
                    new LogEntryKVP {LogId = logId, LogCreatorId = logCreatorId, TransactionId = transactionId, Priority = priority, Key = "Component", Value = log.Component},
                    new LogEntryKVP {LogId = logId, LogCreatorId = logCreatorId, TransactionId = transactionId, Priority = priority, Key = "Event", Value = log.Event},
                    new LogEntryKVP {LogId = logId, LogCreatorId = logCreatorId, TransactionId = transactionId, Priority = priority, Key = "Description", Value = log.Description},
                    new LogEntryKVP {LogId = logId, LogCreatorId = logCreatorId, TransactionId = transactionId, Priority = priority, Key = "Status", Value = log.Status.ToString()},
                };

                if ((log.Parameters != null) && (log.Parameters.Any()))
                {
                    foreach (Tuple<string, object> parameter in log.Parameters)
                    {
                        logEntities.Add(new LogEntryKVP { LogId = logId, LogCreatorId = logCreatorId, TransactionId = transactionId, Priority = priority, Key = parameter.Item1, Value = Convert.ToString(parameter.Item2) });
                    }
                }

                _logPersistency.Insert(logEntities);

                IList<LogEntryKVP> allLogEntries = _inMemoryLogEntries;

                //Isolate analysis to newly inserted logs
                _inMemoryLogEntries = _inMemoryLogEntries
                    .Where(currentLog => currentLog.LogId == logId)
                    .ToList();

                //-->> Normalize log persistency table here (if necessary) prior to analysis of log entries
                _logAnalyzer.Analyze();
                //-->> Clear log persistency table here (if necessary)

                _inMemoryLogEntries = allLogEntries;        //Restore all log entries
            };
        }

        [TestMethod]
        public void TestMethod1()
        {
            IStaticLogEntryWrapper staticLogCreator = new StaticLogEntryWrapper("Test Log Creator", _manager)
            {
                System = "Security System",
                Application = "Security Tester",
                Component = "Authentication Component",
                Event = "Validation"
            };

            staticLogCreator.EmitLog(Priority.Info, Status.Success, "Validation has been invoked but was failed");
            Assert.AreEqual("0001", _invokedRuleId);
        }

        [TestMethod]
        public void TestMethod2()
        {
            IStaticLogEntryWrapper staticLogCreator = new StaticLogEntryWrapper("Test Log Creator", _manager)
            {
                System = "Security System",
                Application = "Security Tester",
                Component = "Authentication Component",
                Event = "Validation"
            };

            staticLogCreator.EmitLog(Priority.Info, Status.Failure, string.Empty);
            staticLogCreator.EmitLog(Priority.Info, Status.Failure, string.Empty);
            Assert.AreEqual("0002", _invokedRuleId);
        }
    }
}
