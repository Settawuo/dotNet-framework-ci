using LinqKit;
using Npgsql;
using NpgsqlTypes;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using WBBData.DbIteration;
using WBBEntity.Extensions;

namespace WBBData.Repository
{
    public class FBBHVREntityRepository<T> : IFBBHVREntityRepository<T> where T : class
    {

        private DbContext _context;
        private readonly IDbSet<T> _entity;

        public FBBHVREntityRepository(IFBBHVRDbFactory dbFactory)
        {
            DbFactory = dbFactory;
            _entity = DbContext.Set<T>();
        }

        //public EntityRepositoryBase(DbContext dbContext)
        //{
        //    if (dbContext == null)
        //        throw new ArgumentNullException("dbContext");

        //    _context = dbContext;
        //    _entity = _context.Set<T>();
        //}

        protected IDbFactory DbFactory { get; private set; }

        protected DbContext DbContext
        {
            get { return _context ?? (_context = DbFactory.GetContext()); }
        }

        public virtual void Create(T entity)
        {
            _entity.Add(entity);
        }

        public virtual void Update(T entity)
        {
            _context.Entry(entity).State = (System.Data.Entity.EntityState)System.Data.EntityState.Modified;
        }

        public virtual void Update(T entity, T oldEntity)
        {
            if (entity == null)
            {
                throw new ArgumentException("Cannot add a null entity.");
            }

            var entry = _context.Entry<T>(entity);

            if (entry.State == (System.Data.Entity.EntityState)System.Data.EntityState.Detached)
            {
                if (oldEntity != null)
                {
                    var attachedEntry = _context.Entry(oldEntity);
                    attachedEntry.CurrentValues.SetValues(entity);
                }
                else
                {
                    entry.State = (System.Data.Entity.EntityState)System.Data.EntityState.Modified; // This should attach entity
                }
            }
        }

        public virtual void Delete(T entity)
        {
            _entity.Remove(entity);
        }

        public virtual void Delete(Func<T, bool> where)
        {
            IQueryable<T> entities = _entity.Where<T>(where).AsQueryable<T>();

            foreach (T entity in entities)
                _entity.Remove(entity);
        }

        public virtual T GetByKey(decimal key)
        {
            return _entity.Find(key);
        }

        public virtual IQueryable<T> Get()
        {
            return _entity.AsQueryable().AsNoTracking();
        }

        public virtual IQueryable<T> Get(Expression<Func<T, bool>> where)
        {
            return _entity.AsExpandable().Where<T>(where).AsQueryable<T>().AsNoTracking();
        }

        public virtual IQueryable<T> SqlQuery(string query, object parameters = null)
        {
            return _context.Database.SqlQuery<T>(query, parameters).AsQueryable<T>();
        }

        public virtual void Refresh(T entity)
        {
            _context.Entry<T>(entity).Reload();
        }

        public virtual int ExecuteStoredProc(string storedProcedure, object parameters = null)
        {
            return _context.Database.ExecuteSqlCommandSmart(storedProcedure, parameters);
        }

        public virtual int ExecuteStoredProc(string storedProcedure, out object[] paramOut, object parameters = null)
        {
            return _context.Database.ExecuteSqlCommandSmart(storedProcedure, out paramOut, parameters);
        }

        public virtual IQueryable<T> ExecuteReadStoredProc(string storedProcedure, object parameters = null)
        {
            return _context.Database.SqlQuerySmart<T>(storedProcedure, parameters).AsQueryable<T>();
        }

        public virtual IQueryable ExecuteReadStoredProc(Type elementType, string storedProcedure, object parameters = null)
        {
            return _context.Database.SqlQuerySmart(elementType, storedProcedure, parameters).AsQueryable();
        }

        #region Output Multiple Cursor
        public virtual List<object> ExecuteStoredProcMultipleCursor(string storedProcedure, object[] parameters)
        {
            List<object> arr = new List<object>();
            var dbCommand = _context.Database.Connection.CreateCommand();
            int result = 0;
            try
            {
                OracleParameter oracle = new OracleParameter();
                if (dbCommand.Connection.State != ConnectionState.Open)
                    dbCommand.Connection.Open();

                dbCommand.CommandText = storedProcedure;
                dbCommand.CommandType = CommandType.StoredProcedure;
                dbCommand.Parameters.AddRange(parameters);

                dbCommand.ExecuteNonQuery();

                for (int i = 0; i < parameters.Length; i++)
                {
                    var orcle = (OracleParameter)parameters[i];
                    if (orcle.Direction == ParameterDirection.Output)
                    {
                        if (orcle.OracleDbType == OracleDbType.RefCursor)
                        {
                            DataTable dt = new DataTable();
                            dt.Load(((OracleRefCursor)orcle.Value).GetDataReader());
                            arr.Add(dt);
                        }
                        else
                        {
                            arr.Add(orcle.Value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = -1;
                arr = null;
            }
            finally
            {
                // dbCommand.Connection.Close();
                //dbCommand.Connection.Dispose();
            }


            return arr;
        }

        #endregion

        #region R23.06 For Shareplex to HVR PostgreSQL

        public virtual List<object> ExecuteStoredProcMultipleCursorNpgsql(string storedProcedure, object[] parameters)
        {
            List<object> arr = new List<object>();
            List<string> _refcur = new List<string>();

            DbConnection connection = _context.Database.Connection;
            NpgsqlConnection npgsqlConnection = connection as NpgsqlConnection;

            int result = 0;
            try
            {
                NpgsqlParameter npgsql = new NpgsqlParameter();
                if (npgsqlConnection.State != ConnectionState.Open)
                    npgsqlConnection.Open();

                var _args = PrepareOraArguments(storedProcedure, parameters, callType: CallType.Function);

                if (_args.Item2 != null)
                    _refcur = _args.Item2.GetCursor();

                using (var _transaction = npgsqlConnection.BeginTransaction())
                {
                    for (int i = 0; i < _args.Item3.Count; i++)
                    {
                        _transaction.Connection?.MapCompositeFactory(_args.Item3[i]);
                    }
                    var dbCommand = new NpgsqlCommand(_args.Item1, npgsqlConnection);
                    dbCommand.CommandType = CommandType.Text;
                    dbCommand.CommandTimeout = 3000;//seconds
                    dbCommand.AddParameter(_args.Item2);

                    dbCommand.ExecuteNonQuery();

                    if (_refcur.Count > 0)
                    {
                        var _reader = dbCommand.FetchExecuteReader(_refcur);
                        arr.AddRange(_reader);
                    }

                    for (int i = 0; i < dbCommand.Parameters.Count; i++)
                    {
                        var _npgsql = dbCommand.Parameters[i];
                        if (_npgsql.Direction == ParameterDirection.Input || _npgsql.NpgsqlDbType == NpgsqlDbType.Refcursor) continue;
                        if (_npgsql.Direction == ParameterDirection.InputOutput || _npgsql.Direction == ParameterDirection.Output)
                        {
                            _args.Item2[i].Value = dbCommand.Parameters[i];
                        }
                    }
                    _transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                result = -1;
                arr = null;
            }
            finally
            {
                // dbCommand.Connection.Close();
                //dbCommand.Connection.Dispose();
            }


            return arr;
        }
        #endregion R23.06 For Shareplex to HVR PostgreSQL

        //public virtual List<TObj> Translate<TObj>(OracleDataReader dataReader, string entitySetName)
        //{
        //    return ((IObjectContextAdapter)_context).ObjectContext.Translate<TObj>(dataReader, entitySetName, MergeOption.AppendOnly).ToList();
        //}



        public virtual Dictionary<string, object> ExecuteStoredProcExecuteReader(string storedProcedure, object[] parameters)
        {
            Dictionary<string, object> arr = new Dictionary<string, object>();
            var dbCommand = _context.Database.Connection.CreateCommand();
            try
            {
                NpgsqlParameter oracle = new NpgsqlParameter();
                if (dbCommand.Connection.State != ConnectionState.Open)
                    dbCommand.Connection.Open();
                dbCommand.CommandText = storedProcedure;
                dbCommand.CommandType = CommandType.StoredProcedure;
                dbCommand.Parameters.AddRange(parameters);
                var reader = dbCommand.ExecuteReader();

                //var test = ((OracleClob)dbCommand.Parameters["str_pdf_html"].Value).Value.ToString();
                //dbCommand.ExecuteNonQuery();

                for (int i = 0; i < parameters.Length; i++)
                {
                    var npg = (NpgsqlParameter)parameters[i];
                    if (npg.Direction == ParameterDirection.Output)
                    {
                        if (npg.NpgsqlDbType == NpgsqlDbType.Text)
                        {
                            //DataTable dt = new DataTable();
                            //dt.Load(((OracleRefCursor)orcle.Value).GetDataReader());
                            arr.Add(parameters[i].ToString(), ((OracleClob)dbCommand.Parameters[npg.ParameterName].Value).Value.ToString());
                        }
                        else
                        {
                            arr.Add(parameters[i].ToString(), npg.Value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                arr = null;
            }
            finally
            {
                dbCommand.Connection.Close();
            }


            return arr;
        }

        public virtual DataTable ExecuteToDataTable(string queryString, string tableName)
        {
            DataTable rtnDataTable;
            var dbConnection = (NpgsqlConnection)_context.Database.Connection;
            try
            {
                if (dbConnection.State != ConnectionState.Open)
                    dbConnection.Open();

                using (var dbCommand = dbConnection.CreateCommand())
                {
                    dbCommand.CommandText = queryString;
                    dbCommand.CommandType = CommandType.Text;
                    var dr = dbCommand.ExecuteReader(CommandBehavior.CloseConnection);
                    var dt = new DataTable(tableName);
                    dt.Load(dr);
                    rtnDataTable = dt.Copy();
                }


                //using (var oracleDataAdapter = new OracleDataAdapter(queryString, dbConnection))
                //{
                //    oracleDataAdapter.Fill(rtnDataTable);
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (dbConnection.State == ConnectionState.Open)
                    dbConnection.Close();
            }

            return rtnDataTable;
        }

        public virtual DataTable ExecuteToDataTableNpgsql(string queryString, string tableName)
        {
            DataTable rtnDataTable;
            var dbConnection = (NpgsqlConnection)_context.Database.Connection;
            try
            {
                if (dbConnection.State != ConnectionState.Open)
                    dbConnection.Open();

                using (var dbCommand = dbConnection.CreateCommand())
                {
                    dbCommand.CommandText = queryString;
                    dbCommand.CommandType = CommandType.Text;
                    var dr = dbCommand.ExecuteReader(CommandBehavior.CloseConnection);
                    var dt = new DataTable(tableName);
                    dt.Load(dr);
                    rtnDataTable = dt.Copy();
                }


                //using (var oracleDataAdapter = new OracleDataAdapter(queryString, dbConnection))
                //{
                //    oracleDataAdapter.Fill(rtnDataTable);
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (dbConnection.State == ConnectionState.Open)
                    dbConnection.Close();
            }

            return rtnDataTable;
        }

        //public EntityRepository(DbContext dbContext)
        //    : base(dbContext)
        //{ }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="entitySetName"></param>
        /// <returns></returns>
        /// public IEnumerable<T> Translate(NpgsqlDataReader dataReader, string entitySetName)
        public IEnumerable<T> Translate(NpgsqlDataReader dataReader, string entitySetName)
        {
            return DbContext != null
                ? ((IObjectContextAdapter)DbContext).ObjectContext.Translate<T>(dataReader, entitySetName, (System.Data.Entity.Core.Objects.MergeOption)MergeOption.AppendOnly).ToList()
                : new List<T>();
        }

        private Tuple<string, List<NpgsqlParameter>, List<string>> PrepareOraArguments(string storedProcedure, object[] parameters, CallType callType = CallType.StoredProcedure)
        {
            var parameterNames = new List<string>();
            var arguments = new List<NpgsqlParameter>();
            var arguments_udt = new List<string>();
            var strbegin = callType == CallType.Function ? "SELECT" : "CALL";
            if (parameters != null)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    NpgsqlParameter outParam = (NpgsqlParameter)parameters[i];
                    parameterNames.Add("@" + outParam.ParameterName);
                    arguments.Add(outParam);
                    var outType = outParam.Value?.GetType();
                    if (!outType.FullName.StartsWith("System"))
                    {
                        var _name = outType.Name.Contains("[]") ? outType.Name.Replace("[]", "") : outType.Name;
                        arguments_udt.Add(_name);
                    }
                    else if (outType.Name.Contains("List"))
                    {
                        var ArgType = outType.GenericTypeArguments[0];
                        var _name = ArgType.Name.Contains("[]") ? ArgType.Name.Replace("[]", "") : ArgType.Name;
                        arguments_udt.Add(_name);
                    }
                }


                var param = parameterNames.Count > 0 ? string.Join(", ", parameterNames) : "";
                storedProcedure = $"{strbegin} {storedProcedure} ({param});";

            }
            else
            {
                storedProcedure = string.Format($"{strbegin} {storedProcedure} ()");
            }
            return Tuple.Create(storedProcedure, arguments, arguments_udt);
        }

        enum CallType
        {
            StoredProcedure,
            Function
        }
    }
}
