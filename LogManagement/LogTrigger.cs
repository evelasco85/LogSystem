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
        List<string> _allowablePropertyAccess = new List<string>();
        IList<string> _registeredPropertyAccess = new List<string>();

        public LogTrigger(string triggerId,
            RegisterLogPropertyAccessDelegate registerLogPropertyAccess,
            EvaluateDelegate evaluation,
            InvokeEventDelegate invokeEvent)
        {
            if(string.IsNullOrEmpty(triggerId)) throw new ArgumentNullException("'ruleId' parameter is required");

            _triggerId = triggerId;
            _allowablePropertyAccess.AddRange(GetAllowablePropertyAccess());

            if (registerLogPropertyAccess != null)
                registerLogPropertyAccess(RegisterPropertyAccess);

            _evaluate = evaluation;
            _invokeEvent = invokeEvent;
        }

        public static IList<string> GetAllowablePropertyAccess()
        {
            return new List<string>();
        }

        void RegisterPropertyAccess(Expression<Func<TLogEntity, object>> registerExpression)
        {
            //if (registerExpression == null) return;

            //string propertyName = string.Empty;

            //if (!_allowablePropertyAccess.Contains(propertyName))
            //    throw new Exception(string.Format("Property '{0}' is not valid for registration", propertyName));

            //if (_registeredPropertyAccess.Contains(propertyName)) return;

            //_registeredPropertyAccess.Add(propertyName);
        }

        public TResult GetValue<TResult>(Expression<Func<TLogEntity, TResult>> queryExpression)
        {
            if (queryExpression == null) return default(TResult);

            string propertyName = string.Empty;

            //if (!AllowPropertyAccess(string.Empty))
            //    throw new FieldAccessException(string.Format("Access to '{0}' property is not registered", propertyName));

            return queryExpression.Compile().Invoke(_temporaryLogDataHolder);
        }

        bool AllowPropertyAccess(string propertyName)
        {
            return true;
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
