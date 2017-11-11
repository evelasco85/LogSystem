using System;
using LogManagement.Managers;
using System.Linq.Expressions;

namespace LogManagement
{
    public interface ILogTriggerInfo
    {
        string TriggerId { get; }
    }

    public interface ILogTrigger<TLogEntity> : ILogTriggerInfo
    {
        bool Evaluate(TLogEntity logEntity, ILogRepository<TLogEntity> logRepository, ILogCreator logger);
        void InvokeEvent(TLogEntity logEntity, ILogRepository<TLogEntity> logRepository, ILogCreator logger);
    }

    public class LogTrigger<TLogEntity> : ILogTrigger<TLogEntity>
    {
        public delegate bool EvaluateDelegate(string triggerId,
            Func<Expression<Func<TLogEntity, dynamic>>, dynamic> getLogValue,
            ILogRepository<TLogEntity> repository, ILogCreator logger);
        public delegate void InvokeEventDelegate(string ruleId,
            Func<Expression<Func<TLogEntity, dynamic>>, dynamic> getLogValue,
            ILogRepository<TLogEntity> repository, ILogCreator logger);

        private string _triggerId;
        private EvaluateDelegate _evaluate;
        private InvokeEventDelegate _invokeEvent;

        public string TriggerId { get { return _triggerId; } }

        public LogTrigger(string triggerId,
            EvaluateDelegate evaluationFunction,
            InvokeEventDelegate invokeEvent)
        {
            if(string.IsNullOrEmpty(triggerId)) throw new ArgumentNullException("'ruleId' parameter is required");

            this._triggerId = triggerId;
            _evaluate = evaluationFunction;
            _invokeEvent = invokeEvent;
        }

        public bool Evaluate(TLogEntity logEntity, ILogRepository<TLogEntity> logRepository, ILogCreator logger)
        {
            if (_evaluate == null) return false;

            Func<Expression<Func<TLogEntity, dynamic>>, dynamic> getLogValue = expr =>
            {
                return expr.Compile().Invoke(logEntity);
            };

            return _evaluate(_triggerId, getLogValue, logRepository, logger);
        }

        public void InvokeEvent(TLogEntity logEntity, ILogRepository<TLogEntity> logRepository, ILogCreator logger)
        {
            if (_invokeEvent == null) return;

            Func<Expression<Func<TLogEntity, dynamic>>, dynamic> getLogValue = expr =>
            {
                return expr.Compile().Invoke(logEntity);
            };

            _invokeEvent(_triggerId, getLogValue, logRepository, logger);
        }
    }
}
