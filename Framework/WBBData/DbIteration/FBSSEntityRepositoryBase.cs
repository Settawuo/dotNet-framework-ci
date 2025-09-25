using LinqKit;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace WBBData.DbIteration
{
    public class FBSSEntityRepositoryBase<T> where T : class
    {
        private DbContext _context;
        private readonly IDbSet<T> _entity;

        //public EntityRepositoryBase(DbContext dbContext)
        //{
        //    if (dbContext == null)
        //        throw new ArgumentNullException("dbContext");

        //    _context = dbContext;
        //    _entity = _context.Set<T>();
        //}

        public FBSSEntityRepositoryBase(IFBSSDbFactory dbFactory)
        {
            DbFactory = dbFactory;
            _entity = DbContext.Set<T>();
        }

        protected IFBSSDbFactory DbFactory { get; private set; }

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
            catch (Exception)
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

        //public virtual List<TObj> Translate<TObj>(OracleDataReader dataReader, string entitySetName)
        //{
        //    return ((IObjectContextAdapter)_context).ObjectContext.Translate<TObj>(dataReader, entitySetName, MergeOption.AppendOnly).ToList();
        //}

        #endregion

        public virtual Dictionary<string, object> ExecuteStoredProcExecuteReader(string storedProcedure, object[] parameters)
        {
            Dictionary<string, object> arr = new Dictionary<string, object>();
            var dbCommand = _context.Database.Connection.CreateCommand();
            try
            {
                OracleParameter oracle = new OracleParameter();
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
                    var orcle = (OracleParameter)parameters[i];
                    if (orcle.Direction == ParameterDirection.Output)
                    {
                        if (orcle.OracleDbType == OracleDbType.Clob)
                        {
                            //DataTable dt = new DataTable();
                            //dt.Load(((OracleRefCursor)orcle.Value).GetDataReader());
                            arr.Add(parameters[i].ToString(), ((OracleClob)dbCommand.Parameters[orcle.ParameterName].Value).Value.ToString());
                        }
                        else
                        {
                            arr.Add(parameters[i].ToString(), orcle.Value);
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
            var dbConnection = (OracleConnection)_context.Database.Connection;
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
    }
}