using System;

namespace LogManagement
{
    public interface ILogTriggerInfo
    {
        string TriggerId { get; }
    }

    public interface ILogTrigger<TLogEntity> : ILogTriggerInfo
    {
        bool Process(TLogEntity logEntity, ILogRepository<TLogEntity> logRepository);
    }

    public class LogTrigger<TLogEntity> : ILogTrigger<TLogEntity>
    {
        public delegate bool EvaluateDelegate(string triggerId,
            TLogEntity logEntity,
            ILogRepository<TLogEntity> repository);
        public delegate void InvokeEventDelegate(string triggerId,
            TLogEntity logEntity,
            ILogRepository<TLogEntity> repository);

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

        public bool Process(TLogEntity logEntity, ILogRepository<TLogEntity> logRepository)
        {
            if (_evaluate == null) return false;

            if (_evaluate(_triggerId, logEntity, logRepository) && (_invokeEvent != null))
            {
                _invokeEvent(_triggerId, logEntity, logRepository);

                return true;
            }

            return false;
        }
    }
}
