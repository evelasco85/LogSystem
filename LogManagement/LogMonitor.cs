using System.Collections.Generic;
using System.Linq;
using LogManagement.Managers;

namespace LogManagement
{
    public interface ILogMonitor<TLogEntity>
    {
        void Evaluate(TLogEntity logEntity);
        void EvaluateAll();
        LogMonitor<TLogEntity>.LogEvaluationCompleteDelegate LogEvaluationComplete { get; set; }
        LogMonitor<TLogEntity>.TriggerInvocationCompleteDelegate TriggerInvocationComplete { get; set; }
    }

    public class LogMonitor<TLogEntity> : ILogMonitor<TLogEntity>
    {
        public delegate void TriggerInvocationCompleteDelegate(ILogTriggerInfo triggerInfo, TLogEntity logEntity, ILogRepository<TLogEntity> logRepository, ILogCreator logger);
        public delegate void LogEvaluationCompleteDelegate(IList<ILogTriggerInfo> invokedTriggerList, TLogEntity logEntity, ILogRepository<TLogEntity> logRepository, ILogCreator logger);

        private ILogRepository<TLogEntity> _logRepository;
        private List<ILogTrigger<TLogEntity>> _triggers = new List<ILogTrigger<TLogEntity>>();
        private ILogCreator _logger;

        public LogEvaluationCompleteDelegate LogEvaluationComplete { get; set; }
        public TriggerInvocationCompleteDelegate TriggerInvocationComplete { get; set; }

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
            IList<ILogTriggerInfo> listOfTriggersProcessed = new List<ILogTriggerInfo>();

            for (int index = 0; index < _triggers.Count; index++)
            {
                ILogTrigger<TLogEntity> trigger = _triggers[index];

                if(trigger == null) continue;

                if (trigger.Process(logEntity, _logRepository))
                {
                    listOfTriggersProcessed.Add(trigger);

                    if (TriggerInvocationComplete != null) TriggerInvocationComplete(trigger, logEntity, _logRepository, _logger);
                }
            }

            if (LogEvaluationComplete != null) LogEvaluationComplete(listOfTriggersProcessed, logEntity, _logRepository, _logger);
        }

        //Analyze all log entities in log repository
        public void EvaluateAll()
        {
            Evaluate(default(TLogEntity));
        }
    }
}
