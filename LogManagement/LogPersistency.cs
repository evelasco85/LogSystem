using System.Collections.Generic;
using System.Linq;

namespace LogManagement
{
    public interface ILogPersistency<TLogEntity>
    {
        void Insert(IList<TLogEntity> logEntities);
    }

    public class LogPersistency<TLogEntity> : ILogPersistency<TLogEntity>
    {
        public delegate void PreInsertOperationDelegate<TLog>(IList<TLog> logEntitiesToAdd,
            ILogRepository<TLogEntity> preInsertRepository);

        public delegate void InsertOperationDelegate<TLog>(IList<TLog> logEntitiesToAdd,
            ILogRepository<TLogEntity> currentRepository);

        public delegate void PostInsertOperationDelegate<TLog>(IList<TLog> addedLogEntities,
            ILogRepository<TLogEntity> postInsertRepository);

        private ILogRepository<TLogEntity> _repository;
        private PreInsertOperationDelegate<TLogEntity> _preInsertOperation;
        private InsertOperationDelegate<TLogEntity> _insertOperation;
        private PostInsertOperationDelegate<TLogEntity> _postInsertOperation;

        public LogPersistency(ILogRepository<TLogEntity> repository,
            PreInsertOperationDelegate<TLogEntity> preInsertOperation,
            InsertOperationDelegate<TLogEntity> insertOperation,
            PostInsertOperationDelegate<TLogEntity> postInsertOperation)
        {
            _repository = repository;
            _preInsertOperation = preInsertOperation;
            _insertOperation = insertOperation;
            _postInsertOperation = postInsertOperation;
        }

        public void Insert(IList<TLogEntity> logEntities)
        {
            if (_preInsertOperation != null) _preInsertOperation(logEntities, _repository);

            if (_insertOperation != null) _insertOperation(logEntities, _repository);

            if (_postInsertOperation != null) _postInsertOperation(logEntities, _repository);
        }
    }
}
