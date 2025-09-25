using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace WBBData.DbIteration
{
    public interface IEntityRepositoryBase<T> where T : class
    {
        //void Add(T entity);

        //void Update(T entity);

        //void Delete(T entity);

        //void Delete(Func<T, bool> predicate);

        //T GetByKey(decimal id);

        //T Get(Func<T, bool> where);

        //IQueryable<T> GetAll();

        //IQueryable<T> GetMany(Func<T, bool> where);

        //IQueryable<T> SqlQuery(string query, object parameters = null);

        //void Refresh(T entity);

        T GetByKey(decimal key);

        IQueryable<T> Get();

        IQueryable<T> Get(Expression<Func<T, bool>> where);

        IQueryable<T> SqlQuery(string query, object parameters = null);

        //T Get(Func<T, bool> where);

        void Create(T item);

        void Update(T item);

        void Update(T entity, T oldEntity);

        void Delete(T item);

        void Delete(Func<T, bool> where);

        void Refresh(T item);

        // extension
        int ExecuteStoredProc(string storedProcedure, object parameters = null);

        int ExecuteStoredProc(string storedProcedure, out object[] paramOut, object parameters = null);

        IQueryable<T> ExecuteReadStoredProc(string storedProcedure, object parameters = null);

        IQueryable ExecuteReadStoredProc(Type elementType, string storedProcedure, object parameters = null);

        #region Output Multiple Cursor
        List<object> ExecuteStoredProcMultipleCursor(string storedProcedure, object[] parameters);
        IEnumerable<T> Translate(OracleDataReader dataReader, string entitySetName);
        //IEnumerable<T> TranslateNpgsql(NpgsqlDataReader dataReader, string entitySetName); //Edit Case Shareplex to HVR
        #endregion

        Dictionary<string, object> ExecuteStoredProcExecuteReader(string storedProcedure, object[] parameters);

        DataTable ExecuteToDataTable(string queryString, string tableName);
    }
}