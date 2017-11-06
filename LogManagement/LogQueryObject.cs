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
        IList<TEntity> Execute();
    }

    public interface IBaseLogQueryObject<TEntity, TSearchInput> : IBaseLogQueryObject<TEntity>
    {
        TSearchInput SearchInput { get; set; }
        IList<TEntity> PerformSearchOperation(TSearchInput searchInput);
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

        public abstract IList<TEntity> PerformSearchOperation(TSearchInput searchInput);

        public IList<TEntity> Execute()
        {
            IList<TEntity> ultimateResult = PerformSearchOperation(SearchInput);

            return ultimateResult;
        }
    }
}
