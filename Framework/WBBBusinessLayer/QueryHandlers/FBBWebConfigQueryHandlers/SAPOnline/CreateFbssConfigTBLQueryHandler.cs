using System;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.SAPOnline
{
    public class CreateFbssConfigTBLQueryHandler : IQueryHandler<CreateFbssConfigTBLQuery, ReturnCreate>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBSS_CONFIG_TBL> _configTBL;

        public CreateFbssConfigTBLQueryHandler(
            ILogger logger,
            IEntityRepository<FBSS_CONFIG_TBL> configTBL,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _configTBL = configTBL;
            _uow = uow;
        }

        public ReturnCreate Handle(CreateFbssConfigTBLQuery query)
        {
            var _ReturnCreate = new ReturnCreate();
            try
            {
                var _config = new FBSS_CONFIG_TBL();

                if (query.ACTION == WBBContract.Commands.ActionType.Insert)
                {
                    if (this.Validate_FbssConfigTBL(query) == true)
                    {
                        // insert
                        var config = (from c in _configTBL.Get() select c);
                        int idmax = config.Max(c => c.CON_ID);
                        _config.CON_ID = idmax + 1;
                        _config.CON_TYPE = query.CON_TYPE.ToSafeString();
                        _config.CON_NAME = query.DISPLAY_VAL.ToSafeString();
                        _config.DISPLAY_VAL = query.DISPLAY_VAL.ToSafeString();
                        _config.VAL1 = query.DISPLAY_VAL.ToSafeString();
                        _config.VAL2 = query.VAL2.ToSafeString();
                        _config.VAL4 = query.VAL4.ToSafeString();
                        _config.ACTIVEFLAG = "Y";
                        _config.CREATED_BY = query.CREATED_BY.ToSafeString();
                        _config.CREATED_DATE = query.CREATED_DATE;
                        _config.UPDATED_BY = query.CREATED_BY.ToSafeString();
                        _config.UPDATED_DATE = query.CREATED_DATE;

                        _configTBL.Create(_config);
                        _ReturnCreate.RETURN_CODE = 0;
                        _ReturnCreate.RETURN_MSG = "success";
                        _uow.Persist(); // commit DB
                    }
                    else
                    {
                        _ReturnCreate.RETURN_CODE = -1;
                        _ReturnCreate.RETURN_MSG = "duplicate";
                    }
                }
                else if (query.ACTION == WBBContract.Commands.ActionType.Update)
                {
                    if (this.Validate_FbssConfigTBL(query) == true)
                    {
                        var con = _configTBL.Get(a => a.CON_ID == query.CON_ID).FirstOrDefault();

                        con.CON_ID = query.CON_ID;
                        con.DISPLAY_VAL = query.DISPLAY_VAL;
                        con.VAL1 = query.DISPLAY_VAL;
                        con.CON_NAME = query.DISPLAY_VAL;
                        con.UPDATED_BY = query.UPDATED_BY;
                        con.UPDATED_DATE = query.UPDATED_DATE;

                        // update
                        _configTBL.Update(con);
                        _ReturnCreate.RETURN_CODE = 0;
                        _ReturnCreate.RETURN_MSG = "success";
                        _uow.Persist(); // commit DB
                    }
                    else
                    {
                        _ReturnCreate.RETURN_CODE = -1;
                        _ReturnCreate.RETURN_MSG = "duplicate";
                    }
                }
                else if (query.ACTION == WBBContract.Commands.ActionType.Delete)
                {
                    // delete
                    var con = _configTBL.Get(a => a.CON_ID == query.CON_ID).FirstOrDefault();
                    _configTBL.Delete(con);
                    _ReturnCreate.RETURN_CODE = 0;
                    _ReturnCreate.RETURN_MSG = "success";
                    _uow.Persist(); // commit DB
                }

            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                _ReturnCreate.RETURN_CODE = -1;
                _ReturnCreate.RETURN_MSG = ex.GetErrorMessage();
            }
            return _ReturnCreate;
        }

        private bool Validate_FbssConfigTBL(CreateFbssConfigTBLQuery query)
        {
            bool result = true;
            var config = _configTBL.Get(w => w.CON_ID != query.CON_ID
                && w.CON_TYPE.ToUpper() == query.CON_TYPE.ToUpper()
                && (w.DISPLAY_VAL.ToUpper() == query.DISPLAY_VAL.ToUpper() || w.VAL1.ToUpper() == query.DISPLAY_VAL.ToUpper())
                );
            if (config.Count() > 0)
            {
                result = false;
            }

            return result;
        }
    }
}
