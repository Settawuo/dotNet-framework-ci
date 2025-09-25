using AIRNETEntity.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetPackageSequenceHandler : IQueryHandler<GetPackageSequenceQuery, GetPackageSequenceModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IAirNetEntityRepository<object> _objService;



        public GetPackageSequenceHandler(ILogger logger,
        IWBBUnitOfWork uow,
        IEntityRepository<FBB_INTERFACE_LOG> intfLog,
        IAirNetEntityRepository<object> objService)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _objService = objService;
        }

        public GetPackageSequenceModel Handle(GetPackageSequenceQuery query)
        {
            var result = GetPackageSequenceHelper.GetPackageSequence(_logger, _uow, _intfLog, _objService, query);
            return result;
        }
    }

    public static class GetPackageSequenceHelper
    {
        public static GetPackageSequenceModel GetPackageSequence(ILogger logger,
        IWBBUnitOfWork _uow,
        IEntityRepository<FBB_INTERFACE_LOG> _intfLog,
        IAirNetEntityRepository<object> _objService,
            GetPackageSequenceQuery query)
        {
            InterfaceLogCommand log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.P_FIBRENET_ID, "GetPackageSequence", "GetPackageSequenceHandler", null, "FBB", "");

            GetPackageSequenceModel executeResults = new GetPackageSequenceModel();
            try
            {

                var P_ADDRESS_ID = new OracleParameter();
                P_ADDRESS_ID.ParameterName = "P_ADDRESS_ID";
                P_ADDRESS_ID.Size = 2000;
                P_ADDRESS_ID.OracleDbType = OracleDbType.Varchar2;
                P_ADDRESS_ID.Direction = ParameterDirection.Input;
                P_ADDRESS_ID.Value = query.P_ADDRESS_ID;

                var P_ACCESS_MODE = new OracleParameter();
                P_ACCESS_MODE.ParameterName = "P_ACCESS_MODE";
                P_ACCESS_MODE.Size = 2000;
                P_ACCESS_MODE.OracleDbType = OracleDbType.Varchar2;
                P_ACCESS_MODE.Direction = ParameterDirection.Input;
                P_ACCESS_MODE.Value = query.P_ACCESS_MODE;

                var P_PROMOTION_CODE = new OracleParameter();
                P_PROMOTION_CODE.ParameterName = "P_PROMOTION_CODE";
                P_PROMOTION_CODE.Size = 2000;
                P_PROMOTION_CODE.OracleDbType = OracleDbType.Varchar2;
                P_PROMOTION_CODE.Direction = ParameterDirection.Input;
                P_PROMOTION_CODE.Value = query.P_PROMOTION_CODE;

                var RETURN_CODE = new OracleParameter();
                RETURN_CODE.ParameterName = "RETURN_CODE";
                RETURN_CODE.OracleDbType = OracleDbType.Decimal;
                RETURN_CODE.Direction = ParameterDirection.Output;

                var RETURN_MESSAGE = new OracleParameter();
                RETURN_MESSAGE.ParameterName = "RETURN_MESSAGE";
                RETURN_MESSAGE.Size = 2000;
                RETURN_MESSAGE.OracleDbType = OracleDbType.Varchar2;
                RETURN_MESSAGE.Direction = ParameterDirection.Output;

                var RETURN_DISPLAY_CURROR = new OracleParameter();
                RETURN_DISPLAY_CURROR.ParameterName = "RETURN_DISPLAY_CURROR";
                RETURN_DISPLAY_CURROR.OracleDbType = OracleDbType.RefCursor;
                RETURN_DISPLAY_CURROR.Direction = ParameterDirection.Output;

                var result = _objService.ExecuteStoredProcMultipleCursor("AIR_ADMIN.PKG_DISPLAY_CP.DISPLAY_CHANGE_PRO",
                    new object[]
                    {
                         P_ADDRESS_ID,
                         P_ACCESS_MODE,
                         P_PROMOTION_CODE,

                         //return code
                         RETURN_CODE,
                         RETURN_MESSAGE,
                         RETURN_DISPLAY_CURROR
                    });

                string returnCode = result[0] != null ? result[1].ToSafeString() : "-1";
                string returnMessage = result[1] != null ? result[2].ToSafeString() : "error";
                if (result.Count > 2)
                {
                    var d_Return = (DataTable)result[2];
                    executeResults = d_Return.DataTableToList<GetPackageSequenceModel>().FirstOrDefault();
                    if (executeResults == null)
                    {
                        executeResults = new GetPackageSequenceModel();
                        executeResults.PACKAGE_SEQ = 0;
                    }
                }

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResults, log, "Success", "", "");
            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResults, log, "Failed", ex.GetBaseException().ToString(), "");
            }


            return executeResults;
        }
    }

    public class CheckUsePoinBySFFPromotionCodeHandler : IQueryHandler<CheckUsePoinBySFFPromotionCodeQuery, CheckUsePoinBySFFPromotionCodeModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_LOV> _FBB_CFG_LOV;
        private readonly IAirNetEntityRepository<AIR_FBB_NEW_PACKAGE_MASTER> _AIR_FBB_NEW_PACKAGE_MASTER;

        public CheckUsePoinBySFFPromotionCodeHandler(ILogger logger,
            IEntityRepository<FBB_CFG_LOV> FBB_CFG_LOV,
        IAirNetEntityRepository<AIR_FBB_NEW_PACKAGE_MASTER> AIR_FBB_NEW_PACKAGE_MASTER)
        {
            _logger = logger;
            _FBB_CFG_LOV = FBB_CFG_LOV;
            _AIR_FBB_NEW_PACKAGE_MASTER = AIR_FBB_NEW_PACKAGE_MASTER;
        }

        public CheckUsePoinBySFFPromotionCodeModel Handle(CheckUsePoinBySFFPromotionCodeQuery query)
        {
            CheckUsePoinBySFFPromotionCodeModel result = new CheckUsePoinBySFFPromotionCodeModel();
            result.PRE_PRICE_CHARGE = 0;

            var configData = from P in _FBB_CFG_LOV.Get()
                             where P.LOV_NAME == "AIS_POINT_PROMOTION_CODE"
                             && P.LOV_VAL1 == query.SFF_PROMOTION_CODE
                             select P.LOV_VAL2;
            if (configData != null)
            {
                string sffConfigCode = configData.FirstOrDefault();
                var pointData = from P in _AIR_FBB_NEW_PACKAGE_MASTER.Get()
                                where P.SFF_PROMOTION_CODE == sffConfigCode
                                select new CheckUsePoinBySFFPromotionCodeModel
                                {
                                    PRE_PRICE_CHARGE = P.PRE_PRICE_CHARGE
                                };
                if (pointData.Any())
                {
                    result = pointData.FirstOrDefault();
                }
            }
            return result;
        }
    }
}
