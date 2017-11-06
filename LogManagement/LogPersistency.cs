namespace LogManagement
{
    public interface ILogPersistency<TLogEntity>
    {
        void Insert(TLogEntity logEntity);
    }

    public abstract class LogPersistency<TLogEntity> : ILogPersistency<TLogEntity>
    {
        private ILogRepository<TLogEntity> _repository;

        public LogPersistency(ILogRepository<TLogEntity> repository)
        {
            _repository = repository;
        }

        public void Insert(TLogEntity logEntity)
        {
            PreInsertOperation(logEntity, _repository);
            InsertOperation(logEntity, _repository);
            PostInsertOperation(logEntity, _repository);
        }

        public abstract void PreInsertOperation(TLogEntity logEntityToAdd, ILogRepository<TLogEntity> preInsertRepository);
        public abstract void InsertOperation(TLogEntity logEntityToAdd, ILogRepository<TLogEntity> currentRepository);
        public abstract void PostInsertOperation(TLogEntity addedLogEntity, ILogRepository<TLogEntity> postInsertRepository);
    }
}
