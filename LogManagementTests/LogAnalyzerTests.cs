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
        private IList<ILogEntry> _inMemoryLogEntries = new List<ILogEntry>();
        private ILogRepository<ILogEntry> _repository;
        private ILogPersistency<ILogEntry> _persistency;
        private ILogAnalyzer<ILogEntry> _analyzer;
        private ILogManager _manager = LogManager.GetInstance();
        private string _invokedRuleId = string.Empty;

        [TestInitialize]
        public void Initialize()
        {
            _repository = new LogRepository<ILogEntry>(new List<IBaseLogQueryObject<ILogEntry>>
            {
                {new GetFailedInvocationLogsQuery(_inMemoryLogEntries)},
            });
            _persistency = new LogPersistency<ILogEntry>(_repository,
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
            _analyzer = new LogAnalyzer<ILogEntry>(_repository,
                new List<ILogTrigger<ILogEntry>>
                {
                    {
                        new LogTrigger<ILogEntry>("0001",
                            (entity, repository) =>
                            {
                                /*Evaluation*/
                                if (entity == null) return false;

                                return entity.Description == "Validation has been invoked but was failed";
                            },
                            (id, entity, repository) =>
                            {
                                /*Invocation*/
                                _invokedRuleId = id;
                            })
                    },
                    {
                        new LogTrigger<ILogEntry>("0002",
                            (entity, repository) =>
                            {
                                /*Evaluation*/
                                if (entity == null) return false;

                                IEnumerable<ILogEntry> result = repository
                                    .Matching(new GetFailedInvocationLogsQuery.Criteria());

                                return result.Any();
                            },
                            (id, entity, repository) =>
                            {
                                /*Invocation*/
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

                _persistency.Insert(log);
                _analyzer.Analyze(log);
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

            staticLogCreator.EmitLog(Priority.Info, Status.Failure, "Validation has been invoked but was failed");
            Assert.AreEqual("0002", _invokedRuleId);
        }
    }
}
