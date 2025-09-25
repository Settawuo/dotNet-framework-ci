using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.Objects;
using System.Linq;

namespace WBBData.DbIteration
{
    public class UnitOfWork : IWBBUnitOfWork
    {
        private readonly IWBBDbFactory _dbFactory;
        private DbContext _context;

        public UnitOfWork(IWBBDbFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        protected DbContext DbContext
        {
            get { return _context ?? (_context = _dbFactory.GetContext()); }
        }

        public void Persist()
        {
            try
            {
                DbContext.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        throw new Exception(string.Format("{0} : {1}",
                            validationError.PropertyName, validationError.ErrorMessage));
                    }
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var objContext = ((IObjectContextAdapter)DbContext).ObjectContext;
                objContext.Refresh((System.Data.Entity.Core.Objects.RefreshMode)RefreshMode.ClientWins, ex.Entries.Select(e => e.Entity));
                DbContext.SaveChanges();
            }
        }
    }
}