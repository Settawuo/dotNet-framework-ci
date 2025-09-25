using System;
using System.Data.Entity;

namespace WBBData.DbIteration
{
    public interface IDbFactory : IDisposable
    {
        DbContext GetContext();
    }
}
