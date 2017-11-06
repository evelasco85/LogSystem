using System.Collections.Generic;
using System.Linq;

namespace LogManagement
{
    public interface ILogRepository<TLogEntity>
    {
        IEnumerable<TLogEntity> Matching<TSearchInput>(TSearchInput criteria);
    }

    public class LogRepository<TLogEntity> : ILogRepository<TLogEntity>
    {
        private readonly IDictionary<string, IBaseLogQueryObject<TLogEntity>> _queryDictionary;

        public LogRepository(IList<IBaseLogQueryObject<TLogEntity>> queryList)
        {
            _queryDictionary = ConvertQueryListToDictionary(queryList);
        }

        IDictionary<string, IBaseLogQueryObject<TLogEntity>> ConvertQueryListToDictionary(IList<IBaseLogQueryObject<TLogEntity>> queryList)
        {
            IDictionary<string, IBaseLogQueryObject<TLogEntity>> queryDictionary = new Dictionary<string, IBaseLogQueryObject<TLogEntity>>();

            if ((queryList == null) || (!queryList.Any()))
                return queryDictionary;

            for (int index = 0; index < queryList.Count; index++)
            {
                IBaseLogQueryObject<TLogEntity> query = queryList[index];

                if ((query == null) || (query.SearchInputType == null) || (string.IsNullOrEmpty(query.SearchInputType.FullName)))
                    continue;

                queryDictionary.Add(query.SearchInputType.FullName, query);
            }

            return queryDictionary;
        }

        IList<TLogEntity> Matching(IBaseLogQueryObject<TLogEntity> query)
        {
            return query.Execute();
        }

        public IEnumerable<TLogEntity> Matching<TSearchInput>(TSearchInput criteria)
        {
            IBaseLogQueryObject<TLogEntity> query = _queryDictionary[typeof(TSearchInput).FullName];

            query.SearchInputObject = criteria;

            return Matching(query);
        }
    }
}
