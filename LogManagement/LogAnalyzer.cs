using System.Collections.Generic;
using System.Linq;

namespace LogManagement
{
    public interface ILogAnalyzer<TLogEntity>
    {
        void RegisterTrigger(ILogTrigger<TLogEntity> trigger);
        void RegisterTriggers(List<ILogTrigger<TLogEntity>> triggers);
        void Analyze(TLogEntity log);
    }

    public class LogAnalyzer<TLogEntity> : ILogAnalyzer<TLogEntity>
    {
        ILogRepository<TLogEntity> _logRepository;
        private List<ILogTrigger<TLogEntity>> _triggers = new List<ILogTrigger<TLogEntity>>();

        public LogAnalyzer(ILogRepository<TLogEntity> logRepository)
        {
            _logRepository = logRepository;
        }

        public void RegisterTrigger(ILogTrigger<TLogEntity> trigger)
        {
            if(trigger == null) return;

            _triggers.Add(trigger);
        }

        public void RegisterTriggers(List<ILogTrigger<TLogEntity>> triggers)
        {
            if((triggers == null) || (!triggers.Any())) return;

            _triggers.AddRange(triggers);
        }

        public void Analyze(TLogEntity log)
        {
            for (int index = 0; index < _triggers.Count; index++)
            {
                ILogTrigger<TLogEntity> trigger = _triggers[index];

                if(trigger == null) continue;

                bool mustInvoke = trigger.Evaluate(log, _logRepository);

                if(mustInvoke)
                    trigger.InvokeEvent(log, _logRepository);
            }
        }
    }
}
