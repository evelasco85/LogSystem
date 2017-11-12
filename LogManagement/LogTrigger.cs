using System;
using LogManagement.Managers;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace LogManagement
{
    public interface ILogTriggerInfo
    {
        string TriggerId { get; }
    }

    public interface ILogTriggerValueRetriever<TLogEntity>
    {
        TResult GetValue<TResult>(Expression<Func<TLogEntity, TResult>> queryExpression);
    }

    public interface ILogTrigger<TLogEntity> : ILogTriggerInfo, ILogTriggerValueRetriever<TLogEntity>
    {
        bool Process(TLogEntity logEntity, ILogRepository<TLogEntity> logRepository, ILogCreator logger);
    }

    public class LogTrigger<TLogEntity> : ILogTrigger<TLogEntity>
    {
        public delegate void RegisterLogPropertyAccessDelegate(Action<Expression<Func<TLogEntity, object>>> registerLogPropertyToAccess);
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
        TLogEntity _temporaryLogDataHolder;

        public LogTrigger(string triggerId,
            RegisterLogPropertyAccessDelegate registerLogPropertyAccess,
            EvaluateDelegate evaluation,
            InvokeEventDelegate invokeEvent)
        {
            if(string.IsNullOrEmpty(triggerId)) throw new ArgumentNullException("'ruleId' parameter is required");

            _triggerId = triggerId;

            if (registerLogPropertyAccess != null)
                registerLogPropertyAccess(RegisterPropertyAccess);

            _evaluate = evaluation;
            _invokeEvent = invokeEvent;
        }

        public static IList<string> GetAllowablePropertyAccessAccess()
        {
            throw new NotImplementedException();
        }

        void RegisterPropertyAccess(Expression<Func<TLogEntity, object>> registerExpression)
        {
            //Register here
        }

        public TResult GetValue<TResult>(Expression<Func<TLogEntity, TResult>> queryExpression)
        {
            if (queryExpression == null) return default(TResult);

            return queryExpression.Compile().Invoke(_temporaryLogDataHolder);
        }

        public bool Process(TLogEntity logEntity, ILogRepository<TLogEntity> logRepository, ILogCreator logger)
        {
            _temporaryLogDataHolder = logEntity;

            if (Evaluate(logRepository, logger))
            {
                InvokeEvent(logRepository, logger);

                return true;
            }

            return false;
        }

        bool Evaluate(ILogRepository<TLogEntity> logRepository, ILogCreator logger)
        {
            if (_evaluate == null) return false;
            else return _evaluate(_triggerId, this, logRepository, logger);
        }

        void InvokeEvent(ILogRepository<TLogEntity> logRepository, ILogCreator logger)
        {
            if (_invokeEvent == null) return;

            _invokeEvent(_triggerId, this, logRepository, logger);
        }
    }
}
