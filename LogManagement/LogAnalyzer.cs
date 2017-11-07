using System.Collections.Generic;
using System.Linq;

namespace LogManagement
{
    public interface ILogAnalyzer<TLogEntity>
    {
        void Analyze(TLogEntity logEntity);
        void Analyze();
    }

    public class LogAnalyzer<TLogEntity> : ILogAnalyzer<TLogEntity>
    {
        ILogRepository<TLogEntity> _logRepository;
        private List<ILogTrigger<TLogEntity>> _triggers = new List<ILogTrigger<TLogEntity>>();

        public LogAnalyzer(ILogRepository<TLogEntity> logRepository,
            List<ILogTrigger<TLogEntity>> triggers
            )
        {
            _logRepository = logRepository;

            if ((triggers != null) && (triggers.Any())) _triggers.AddRange(triggers);
        }

        //Analyze specific log entities
        public void Analyze(TLogEntity logEntity)
        {
            for (int index = 0; index < _triggers.Count; index++)
            {
                ILogTrigger<TLogEntity> trigger = _triggers[index];

                if(trigger == null) continue;

                bool mustInvoke = trigger.Evaluate(logEntity, _logRepository);

                if(mustInvoke)
                    trigger.InvokeEvent(logEntity, _logRepository);
            }
        }

        //Analyze all log entities in log repository
        public void Analyze()
        {
            Analyze(default(TLogEntity));
        }
    }
}
