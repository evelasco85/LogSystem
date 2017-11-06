namespace LogManagement
{
    public interface ILogPersistency<TLogEntity>
    {
        void Insert(TLogEntity logEntity);
    }

    public class LogPersistency<TLogEntity> : ILogPersistency<TLogEntity>
    {
        public delegate void PreInsertOperationDelegate(TLogEntity logEntityToAdd,
            ILogRepository<TLogEntity> preInsertRepository);

        public delegate void InsertOperationDelegate(TLogEntity logEntityToAdd,
            ILogRepository<TLogEntity> currentRepository);

        public delegate void PostInsertOperationDelegate(TLogEntity addedLogEntity,
            ILogRepository<TLogEntity> postInsertRepository);

        private ILogRepository<TLogEntity> _repository;
        private PreInsertOperationDelegate _preInsertOperation;
        private InsertOperationDelegate _insertOperation;
        private PostInsertOperationDelegate _postInsertOperation;

        public LogPersistency(ILogRepository<TLogEntity> repository,
            PreInsertOperationDelegate preInsertOperation,
            InsertOperationDelegate insertOperation,
            PostInsertOperationDelegate postInsertOperation)
        {
            _repository = repository;
            _preInsertOperation = preInsertOperation;
            _insertOperation = insertOperation;
            _postInsertOperation = postInsertOperation;
        }

        public void Insert(TLogEntity logEntity)
        {
            if (_preInsertOperation != null) _preInsertOperation(logEntity, _repository);

            if (_insertOperation != null) _insertOperation(logEntity, _repository);

            if (_postInsertOperation != null) _postInsertOperation(logEntity, _repository);
        }
    }
}
