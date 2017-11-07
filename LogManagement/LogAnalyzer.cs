﻿using System.Collections.Generic;
using System.Linq;
using LogManagement.Managers;

namespace LogManagement
{
    public interface ILogAnalyzer<TLogEntity>
    {
        void Analyze(TLogEntity logEntity);
        void Analyze();
    }

    public class LogAnalyzer<TLogEntity> : ILogAnalyzer<TLogEntity>
    {
        private ILogRepository<TLogEntity> _logRepository;
        private List<ILogTrigger<TLogEntity>> _triggers = new List<ILogTrigger<TLogEntity>>();
        private ILogCreator _logger;

        public LogAnalyzer(ILogCreator logger,
            ILogRepository<TLogEntity> logRepository,
            List<ILogTrigger<TLogEntity>> triggers
            )
        {
            _logRepository = logRepository;
            _logger = logger;

            if ((triggers != null) && (triggers.Any())) _triggers.AddRange(triggers);
        }

        //Analyze specific log entities
        public void Analyze(TLogEntity logEntity)
        {
            for (int index = 0; index < _triggers.Count; index++)
            {
                ILogTrigger<TLogEntity> trigger = _triggers[index];

                if(trigger == null) continue;

                bool mustInvoke = trigger.Evaluate(logEntity, _logRepository, _logger);

                if(mustInvoke)
                    trigger.InvokeEvent(logEntity, _logRepository, _logger);
            }
        }

        //Analyze all log entities in log repository
        public void Analyze()
        {
            Analyze(default(TLogEntity));
        }
    }
}