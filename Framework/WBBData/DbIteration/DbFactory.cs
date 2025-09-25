using Oracle.ManagedDataAccess.Client;
using System;
using System.Data.Common;
using System.Data.Entity;
using WBBEntity.Models;

namespace WBBData.DbIteration
{
    public class DbFactory : IDisposable, IWBBDbFactory
    {
        private DbContext _context;
        private DbConnection _conn;
        private string _connString = "";
        private string _dbSchema = "SBNWF";

        public DbFactory()
        {
        }

        public DbFactory(DbConnection conn, string dbSchema)
        {
            _conn = conn;
            _dbSchema = dbSchema;
        }

        public DbFactory(string connString, string dbSchema)
        {
            _connString = connString;
            _dbSchema = dbSchema;
        }

        public DbContext GetContext()
        {
            if (!string.IsNullOrEmpty(_connString))
            {
                _conn = new OracleConnection();
                _conn.ConnectionString = _connString;
            }
            //if (null != _conn && _conn.State != ConnectionState.Open)
            //    _conn.Open();

            return _context ?? (_context = new Context(_conn, _dbSchema));
            //return null;
        }

        //protected override void DisposeContext()
        //{
        //    if (_context != null)
        //        _context.Dispose();
        //}

        public void Dispose()
        {
            //if (null != _conn && _conn.State != ConnectionState.Closed)
            //_conn.Close();

            if (_context != null)
                _context.Dispose();
        }
    }
}