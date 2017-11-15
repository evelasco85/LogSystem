using System;
using LogManagement.Managers;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LogManagement
{
    public interface ILogTriggerInfo
    {
        string TriggerId { get; }
    }

    public interface ILogTrigger<TLogEntity> : ILogTriggerInfo
    {
        bool Process(TLogEntity logEntity, ILogRepository<TLogEntity> logRepository, ILogCreator logger);
    }

    public class LogTrigger<TLogEntity> : ILogTrigger<TLogEntity>
    {
        public delegate bool EvaluateDelegate(string triggerId,
            TLogEntity logEntity,
            ILogRepository<TLogEntity> repository, ILogCreator logger);
        public delegate void InvokeEventDelegate(string ruleId,
            TLogEntity logEntity,
            ILogRepository<TLogEntity> repository, ILogCreator logger);

        private string _triggerId;
        private EvaluateDelegate _evaluate;
        private InvokeEventDelegate _invokeEvent;

        public string TriggerId { get { return _triggerId; } }

        public LogTrigger(string triggerId,
            EvaluateDelegate evaluation,
            InvokeEventDelegate invokeEvent)
        {
            if(string.IsNullOrEmpty(triggerId)) throw new ArgumentNullException("'ruleId' parameter is required");

            _triggerId = triggerId;
            _evaluate = evaluation;
            _invokeEvent = invokeEvent;
        }

        public bool Process(TLogEntity logEntity, ILogRepository<TLogEntity> logRepository, ILogCreator logger)
        {
            if (_evaluate == null) return false;

            if (_evaluate(_triggerId, logEntity, logRepository, logger) && (_invokeEvent != null))
            {
                _invokeEvent(_triggerId, logEntity, logRepository, logger);

                return true;
            }

            return false;
        }
    }
}
