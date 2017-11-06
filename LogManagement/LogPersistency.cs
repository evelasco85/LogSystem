using System.Collections.Generic;
using System.Linq;

namespace LogManagement
{
    public interface ILogPersistency<TLogEntity>
    {
        void Insert(TLogEntity logEntity);
        void Insert(IList<TLogEntity> logEntities);
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

        public void Insert(IList<TLogEntity> logEntities)
        {
            if((logEntities == null) || (!logEntities.Any())) return;

            for (int index = 0; index < logEntities.Count; index++)
            {
                TLogEntity logEntity = logEntities[index];

                Insert(logEntity);
            }
        }

        public void Insert(TLogEntity logEntity)
        {
            if (_preInsertOperation != null) _preInsertOperation(logEntity, _repository);

            if (_insertOperation != null) _insertOperation(logEntity, _repository);

            if (_postInsertOperation != null) _postInsertOperation(logEntity, _repository);
        }
    }
}
