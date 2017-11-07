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
        private List<ILogEntry> _inMemoryLogEntries = new List<ILogEntry>();
        private ILogRepository<ILogEntry> _logRepository;
        private ILogPersistency<ILogEntry> _logPersistency;
        private ILogAnalyzer<ILogEntry> _logAnalyzer;
        private ILogManager _manager = LogManager.GetInstance();
        private string _invokedRuleId = string.Empty;

        [TestInitialize]
        public void Initialize()
        {
            _logRepository = new LogRepository<ILogEntry>(new List<IBaseLogQueryObject<ILogEntry>>
            {
                {new GetFailedInvocationLogsQuery(_inMemoryLogEntries)},
                {new GetFailedValidationDescriptionLogsQuery(_inMemoryLogEntries)},
            });
            _logPersistency = new LogPersistency<ILogEntry>(_logRepository,
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
            _logAnalyzer = new LogAnalyzer<ILogEntry>(_logRepository,
                new List<ILogTrigger<ILogEntry>>
                {
                    {
                        new LogTrigger<ILogEntry>("0001",
                            (entity, repository) =>
                            {
                                /*Trigger Evaluation*/
                                IEnumerable<ILogEntry> result = repository
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
                        new LogTrigger<ILogEntry>("0002",
                            (entity, repository) =>
                            {
                                /*Trigger Evaluation*/
                                IEnumerable<ILogEntry> result = repository
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

                _logPersistency.Insert(log);

                //-->> Normalize log persistency table here (if necessary) prior to analysis of log entries
                _logAnalyzer.Analyze(log);
                //-->> Clear log persistency table here (if necessary)
            };
        }

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

            staticLogCreator.EmitLog(Priority.Info, Status.Success, "Validation has been invoked but was failed");
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

            staticLogCreator.EmitLog(Priority.Info, Status.Failure, string.Empty);
            staticLogCreator.EmitLog(Priority.Info, Status.Failure, string.Empty);
            Assert.AreEqual("0002", _invokedRuleId);
        }
    }
}
