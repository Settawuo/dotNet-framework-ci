using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WBBRepositoryService
{
    public interface IEntityRepositoryService<T> where T : class
    {
        IQueryable<T> Get();

        IQueryable<T> Get(Func<T, bool> where);

        IQueryable<T> SqlQuery(string query, object parameters = null);

        T GetByKey(decimal key);

        //T Get(Func<T, bool> where);

        void Create(T item);

        void Update(T item);

        void Delete(decimal id);

        void Delete(T item);

        void Refresh(T item);

        void Persist();

        int ExecuteStoredProc(string storedProcedure, object parameters = null);

        int ExecuteStoredProc(string storedProcedure, out object[] paramOut, object parameters = null);

        IQueryable<T> ExecuteReadStoredProc(string storedProcedure, object parameters = null);

        IQueryable ExecuteReadStoredProc(Type elementType, string storedProcedure, object parameters = null);
    }
}