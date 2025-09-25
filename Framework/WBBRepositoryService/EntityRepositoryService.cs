using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using WBBData.Repository;
using WBBData.DbIteration;

namespace WBBRepositoryService
{
    public class EntityRepositoryService<T> : IEntityRepositoryService<T> where T : class
    {
        private readonly IEntityRepository<T> _modelRepo;
        private readonly IUnitOfWork _unitOfWork;

        public EntityRepositoryService(IEntityRepository<T> modelRepo, IUnitOfWork unitOfWork)
        {
            _modelRepo = modelRepo;
            _unitOfWork = unitOfWork;
        }

        #region Implement Interface

        public IQueryable<T> Get()
        {
            return _modelRepo.GetAll();
        }

        public IQueryable<T> Get(Func<T, bool> where)
        {
            return _modelRepo.GetMany(where);
        }

        public IQueryable<T> SqlQuery(string query, object parameters = null)
        {
            return _modelRepo.SqlQuery(query, parameters);
        }

        public T GetByKey(decimal key)
        {
            return _modelRepo.GetByKey(key);
        }

        public void Create(T item)
        {
            _modelRepo.Add(item);
            //_unitOfWork.Commit();
        }

        public void Update(T item)
        {
            _modelRepo.Update(item);
            //_unitOfWork.Commit();
        }

        public void Delete(decimal key)
        {
            var item = this.GetByKey(key);

            if (null != item)
                _modelRepo.Delete(item);
            //_unitOfWork.Commit();
        }

        public void Delete(T item)
        {
            if (null != item)
                _modelRepo.Delete(item);
            //_unitOfWork.Commit();
        }

        public void Refresh(T item)
        {
            if (null != item)
                _modelRepo.Refresh(item);
        }

        public void Persist()
        {
            _unitOfWork.Commit();
        }

        public int ExecuteStoredProc(string storedProcedure, object parameters = null)
        {
            return _modelRepo.ExecuteStoredProc(storedProcedure, parameters);
        }

        public int ExecuteStoredProc(string storedProcedure, out object[] paramOut, object parameters = null)
        {
            return _modelRepo.ExecuteStoredProc(storedProcedure, out paramOut, parameters);
        }

        public IQueryable<T> ExecuteReadStoredProc(string storedProcedure, object parameters = null)
        {
            return _modelRepo.ExecuteReadStoredProc(storedProcedure, parameters);
        }

        public IQueryable ExecuteReadStoredProc(Type elementType, string storedProcedure, object parameters = null)
        {
            return _modelRepo.ExecuteReadStoredProc(elementType, storedProcedure, parameters);
        }

        private static bool IsInvalidOperationException(Exception ex)
        {
            while (ex != null)
            {
                if (ex is SystemException &&
                    ex.Message.Contains("invalidoperationexception"))
                {
                    return true;
                }

                ex = ex.InnerException;
            }

            return false;
        }

        #endregion

    }
}
