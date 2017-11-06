using System;
using System.Collections.Generic;

namespace LogManagement
{
    public interface ILogTrigger<TLogEntity>
    {
        string RuleId { get; }

        bool Evaluate(IList<TLogEntity> logEntities, ILogRepository<TLogEntity> logRepository);
        void InvokeEvent(IList<TLogEntity> logEntities, ILogRepository<TLogEntity> logRepository);
    }

    public class LogTrigger<TLogEntity> : ILogTrigger<TLogEntity>
    {
        public delegate bool EvaluationDelegate(IList<TLogEntity> logEntities, ILogRepository<TLogEntity> repository);
        public delegate void EventInvocationDelegate(string ruleId, IList<TLogEntity> logEntities, ILogRepository<TLogEntity> repository);

        private string _ruleId;
        private EvaluationDelegate _evaluationFunc;
        private EventInvocationDelegate _invocationFunc;

        public string RuleId { get { return _ruleId; } }

        public LogTrigger(string ruleId,
            EvaluationDelegate evaluationFunction,
            EventInvocationDelegate invocationFunc)
        {
            if(string.IsNullOrEmpty(ruleId)) throw new ArgumentNullException("'ruleId' parameter is required");

            _ruleId = ruleId;
            _evaluationFunc = evaluationFunction;
            _invocationFunc = invocationFunc;
        }

        public bool Evaluate(IList<TLogEntity> logEntities, ILogRepository<TLogEntity> logRepository)
        {
            if (_evaluationFunc == null) return false;

            return _evaluationFunc(logEntities, logRepository);
        }

        public void InvokeEvent(IList<TLogEntity> logEntities, ILogRepository<TLogEntity> logRepository)
        {
            if (_invocationFunc == null) return;

            _invocationFunc(_ruleId, logEntities, logRepository);
        }
    }
}
