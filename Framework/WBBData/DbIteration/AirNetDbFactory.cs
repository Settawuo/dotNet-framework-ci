using AIRNETEntity.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data.Common;
using System.Data.Entity;

namespace WBBData.DbIteration
{
    public class AirNetDbFactory : IDisposable, IAIRDbFactory
    {
        private DbContext _context;
        private DbConnection _conn;
        private string _connString = "";
        private string _dbSchema = "SBN";

        public AirNetDbFactory()
        {
        }

        public AirNetDbFactory(DbConnection conn, string dbSchema)
        {
            _conn = conn;
            _dbSchema = dbSchema;
        }

        public AirNetDbFactory(string connString, string dbSchema)
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

            return _context ?? (_context = new Context(_conn, _dbSchema));
        }

        public void Dispose()
        {
            if (_context != null)
                _context.Dispose();
        }
    }
}
