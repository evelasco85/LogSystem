using System;
using System.Collections.Generic;

namespace LogManagement
{
    public interface IBaseLogQueryObject
    {
        Type SearchInputType { get; }
        object SearchInputObject { get; set; }
    }

    public interface IBaseLogQueryObject<TEntity> : IBaseLogQueryObject
    {
        IEnumerable<TEntity> Execute();
    }

    public interface IBaseLogQueryObject<TEntity, TSearchInput> : IBaseLogQueryObject<TEntity>
    {
        TSearchInput SearchInput { get; set; }
        IEnumerable<TEntity> PerformSearchOperation(TSearchInput searchInput);
    }

    public abstract class BaseLogQueryObject<TEntity, TSearchInput> :
        IBaseLogQueryObject<TEntity, TSearchInput>
    {
        public TSearchInput SearchInput { get; set; }

        public object SearchInputObject
        {
            get { return SearchInput; }
            set { SearchInput = (TSearchInput)value; }
        }

        public Type SearchInputType
        {
            get { return typeof(TSearchInput); }
        }

        public abstract IEnumerable<TEntity> PerformSearchOperation(TSearchInput searchInput);

        public IEnumerable<TEntity> Execute()
        {
            return PerformSearchOperation(SearchInput);
        }
    }
}
