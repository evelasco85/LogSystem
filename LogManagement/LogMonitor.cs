using System.Collections.Generic;
using System.Linq;
using LogManagement.Managers;

namespace LogManagement
{
    public interface ILogMonitor<TLogEntity>
    {
        void Evaluate(TLogEntity logEntity);
        void EvaluateAll();
        LogMonitor<TLogEntity>.TriggerEvaluationCompleteDelegate EvaluationCompleteAction { get; set; }
        LogMonitor<TLogEntity>.TriggerInvokedCompletionDelegate TriggerInvokedCompletionCompletionAction { get; set; }
    }

    public class LogMonitor<TLogEntity> : ILogMonitor<TLogEntity>
    {
        public delegate void TriggerInvokedCompletionDelegate(ILogTriggerInfo triggerInfo, ILogCreator logger, TLogEntity logEntity);
        public delegate void TriggerEvaluationCompleteDelegate();

        private ILogRepository<TLogEntity> _logRepository;
        private List<ILogTrigger<TLogEntity>> _triggers = new List<ILogTrigger<TLogEntity>>();
        private ILogCreator _logger;

        public TriggerEvaluationCompleteDelegate EvaluationCompleteAction { get; set; }
        public TriggerInvokedCompletionDelegate TriggerInvokedCompletionCompletionAction { get; set; }

        public LogMonitor(ILogCreator logger,
            ILogRepository<TLogEntity> logRepository,
            List<ILogTrigger<TLogEntity>> triggers
            )
        {
            _logRepository = logRepository;
            _logger = logger;

            if ((triggers != null) && (triggers.Any())) _triggers.AddRange(triggers);
        }

        //Analyze specific log entities
        public void Evaluate(TLogEntity logEntity)
        {
            for (int index = 0; index < _triggers.Count; index++)
            {
                ILogTrigger<TLogEntity> trigger = _triggers[index];

                if(trigger == null) continue;

                bool mustInvoke = trigger.Evaluate(logEntity, _logRepository, _logger);

                if (mustInvoke)
                {
                    trigger.InvokeEvent(logEntity, _logRepository, _logger);

                    if (TriggerInvokedCompletionCompletionAction != null) TriggerInvokedCompletionCompletionAction(trigger, _logger, logEntity);
                }
            }

            if (EvaluationCompleteAction != null) EvaluationCompleteAction();
        }

        //Analyze all log entities in log repository
        public void EvaluateAll()
        {
            Evaluate(default(TLogEntity));
        }
    }
}
