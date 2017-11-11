using System;
using LogManagement.Managers;
using System.Linq.Expressions;

namespace LogManagement
{
    public interface ILogTriggerInfo
    {
        string RuleId { get; }
    }

    public interface ILogTrigger<TLogEntity> : ILogTriggerInfo
    {
        bool Evaluate(TLogEntity logEntity, ILogRepository<TLogEntity> logRepository, ILogCreator logger);
        void InvokeEvent(TLogEntity logEntity, ILogRepository<TLogEntity> logRepository, ILogCreator logger);
    }

    public class LogTrigger<TLogEntity> : ILogTrigger<TLogEntity>
    {
        public delegate bool EvaluateDelegate(string ruleId, TLogEntity logEntity, ILogRepository<TLogEntity> repository, ILogCreator logger);
        public delegate void InvokeEventDelegate(string ruleId, TLogEntity logEntity, ILogRepository<TLogEntity> repository, ILogCreator logger);

        private string _ruleId;
        private EvaluateDelegate _evaluate;
        private InvokeEventDelegate _invokeEvent;

        public string RuleId { get { return _ruleId; } }

        public LogTrigger(string ruleId,
            EvaluateDelegate evaluationFunction,
            InvokeEventDelegate invokeEvent)
        {
            if(string.IsNullOrEmpty(ruleId)) throw new ArgumentNullException("'ruleId' parameter is required");

            _ruleId = ruleId;
            _evaluate = evaluationFunction;
            _invokeEvent = invokeEvent;
        }

        public bool Evaluate(TLogEntity logEntity, ILogRepository<TLogEntity> logRepository, ILogCreator logger)
        {
            if (_evaluate == null) return false;

            return _evaluate(_ruleId, logEntity, logRepository, logger);
        }

        public void InvokeEvent(TLogEntity logEntity, ILogRepository<TLogEntity> logRepository, ILogCreator logger)
        {
            if (_invokeEvent == null) return;

            _invokeEvent(_ruleId, logEntity, logRepository, logger);
        }
    }
}
