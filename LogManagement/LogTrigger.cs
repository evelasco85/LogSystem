using System;
using LogManagement.Managers;
using System.Linq.Expressions;

namespace LogManagement
{
    public interface ILogTriggerInfo
    {
        string TriggerId { get; }
    }

    public interface ILogTriggerValueRetriever<TLogEntity>
    {
        TLogEntity TemporaryLogDataHolder { set; }
        TResult GetValue<TResult>(Expression<Func<TLogEntity, TResult>> queryExpression);
    }

    public interface ILogTrigger<TLogEntity> : ILogTriggerInfo, ILogTriggerValueRetriever<TLogEntity>
    {
        bool Evaluate(TLogEntity logEntity, ILogRepository<TLogEntity> logRepository, ILogCreator logger);
        void InvokeEvent(TLogEntity logEntity, ILogRepository<TLogEntity> logRepository, ILogCreator logger);
    }

    public class LogTrigger<TLogEntity> : ILogTrigger<TLogEntity>
    {
        public delegate bool EvaluateDelegate(string triggerId,
            ILogTriggerValueRetriever<TLogEntity> logRetriever,
            ILogRepository<TLogEntity> repository, ILogCreator logger);
        public delegate void InvokeEventDelegate(string ruleId,
            ILogTriggerValueRetriever<TLogEntity> logRetriever,
            ILogRepository<TLogEntity> repository, ILogCreator logger);

        private string _triggerId;
        private EvaluateDelegate _evaluate;
        private InvokeEventDelegate _invokeEvent;

        public string TriggerId { get { return _triggerId; } }
        public TLogEntity TemporaryLogDataHolder { private get; set; }

        public LogTrigger(string triggerId,
            EvaluateDelegate evaluationFunction,
            InvokeEventDelegate invokeEvent)
        {
            if(string.IsNullOrEmpty(triggerId)) throw new ArgumentNullException("'ruleId' parameter is required");

            this._triggerId = triggerId;
            _evaluate = evaluationFunction;
            _invokeEvent = invokeEvent;
        }

        public TResult GetValue<TResult>(Expression<Func<TLogEntity, TResult>> queryExpression)
        {
            if (queryExpression == null) return default(TResult);

            return queryExpression.Compile().Invoke(TemporaryLogDataHolder);
        }

        public bool Evaluate(TLogEntity logEntity, ILogRepository<TLogEntity> logRepository, ILogCreator logger)
        {
            if (_evaluate == null) return false;

            ILogTriggerValueRetriever<TLogEntity> retriever = this;

            retriever.TemporaryLogDataHolder = logEntity;

            bool matched = _evaluate(_triggerId, retriever, logRepository, logger);

            retriever.TemporaryLogDataHolder = default(TLogEntity);

            return matched;
        }

        public void InvokeEvent(TLogEntity logEntity, ILogRepository<TLogEntity> logRepository, ILogCreator logger)
        {
            if (_invokeEvent == null) return;

            ILogTriggerValueRetriever<TLogEntity> retriever = this;

            retriever.TemporaryLogDataHolder = logEntity;

            _invokeEvent(_triggerId, retriever, logRepository, logger);

            retriever.TemporaryLogDataHolder = default(TLogEntity);
        }
    }
}
