using System;

namespace LogManagement
{
    public interface ILogTrigger<TLogEntity>
    {
        string RuleId { get; }

        bool Evaluate(TLogEntity logEntity, ILogRepository<TLogEntity> logRepository);
        void InvokeEvent(TLogEntity logEntity, ILogRepository<TLogEntity> logRepository);
    }

    public class LogTrigger<TLogEntity> : ILogTrigger<TLogEntity>
    {
        public delegate bool EvaluationDelegate(TLogEntity logEntity, ILogRepository<TLogEntity> repository);
        public delegate void EventInvocationDelegate(string ruleId, TLogEntity logEntity, ILogRepository<TLogEntity> repository);

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

        public bool Evaluate(TLogEntity logEntity, ILogRepository<TLogEntity> logRepository)
        {
            if (_evaluationFunc == null) return false;

            return _evaluationFunc(logEntity, logRepository);
        }

        public void InvokeEvent(TLogEntity logEntity, ILogRepository<TLogEntity> logRepository)
        {
            if (_invocationFunc == null) return;

            _invocationFunc(_ruleId, logEntity, logRepository);
        }
    }
}
