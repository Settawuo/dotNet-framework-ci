using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using WBBBusinessLayer.QueryHandlers.ExWebServices.FbbCpGw;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetMeshCheckTechnologyQueryHandler : IQueryHandler<GetMeshCheckTechnologyQuery, MeshCheckTechnologyModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<object> _objService;

        public GetMeshCheckTechnologyQueryHandler(ILogger logger, IWBBUnitOfWork uow, IEntityRepository<FBB_INTERFACE_LOG> intfLog, IEntityRepository<object> objService)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _objService = objService;
        }

        public MeshCheckTechnologyModel Handle(GetMeshCheckTechnologyQuery query)
        {
            MeshCheckTechnologyModel executeResults = new MeshCheckTechnologyModel();
            try
            {
                var log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.addressID, "CheckPackageDownloadSpeed", "FBBService", null, "FBB", "");

                var P_ADDRESS_ID = new OracleParameter();
                P_ADDRESS_ID.ParameterName = "p_address_id";
                P_ADDRESS_ID.Size = 2000;
                P_ADDRESS_ID.OracleDbType = OracleDbType.Varchar2;
                P_ADDRESS_ID.Direction = ParameterDirection.Input;
                P_ADDRESS_ID.Value = query.addressID;

                var PRODUCT_SUBTYPE = new OracleParameter();
                PRODUCT_SUBTYPE.ParameterName = "product_subtype";
                PRODUCT_SUBTYPE.Size = 2000;
                PRODUCT_SUBTYPE.OracleDbType = OracleDbType.Varchar2;
                PRODUCT_SUBTYPE.Direction = ParameterDirection.Output;

                var RETURN_CODE = new OracleParameter();
                RETURN_CODE.ParameterName = "return_code";
                RETURN_CODE.OracleDbType = OracleDbType.Decimal;
                RETURN_CODE.Direction = ParameterDirection.Output;

                var RETURN_MESSAGE = new OracleParameter();
                RETURN_MESSAGE.ParameterName = "return_message";
                RETURN_MESSAGE.Size = 2000;
                RETURN_MESSAGE.OracleDbType = OracleDbType.Varchar2;
                RETURN_MESSAGE.Direction = ParameterDirection.Output;

                _logger.Info("Start PKG_FBBOR041.CHECK_TECHNOLOGY");

                var result = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBBOR041.CHECK_TECHNOLOGY",
                    new object[]
                    {
                         P_ADDRESS_ID,

                         //return code
                         PRODUCT_SUBTYPE,
                         RETURN_CODE,
                         RETURN_MESSAGE
                    });

                executeResults.PRODUCT_SUBTYPE = result[0] != null ? result[0].ToSafeString() : "";
                executeResults.RETURN_CODE = result[1] != null ? result[1].ToSafeString() : "-1";
                executeResults.RETURN_MESSAGE = result[2] != null ? result[2].ToSafeString() : "error";

            }
            catch (Exception ex)
            {
                _logger.Info("Error call PKG_FBBOR041.CHECK_TECHNOLOGY handles : " + ex.Message);

                executeResults.RETURN_CODE = "-1";
                executeResults.RETURN_MESSAGE = "Error";

                return null;
            }
            return executeResults;
        }
    }

    public class GetCheckTechnologyQueryHandler : IQueryHandler<GetCheckTechnologyQuery, CheckTechnologyModel>
    {
        private readonly IEntityRepository<object> _objService;

        public GetCheckTechnologyQueryHandler(IEntityRepository<object> objService)
        {
            _objService = objService;
        }

        public CheckTechnologyModel Handle(GetCheckTechnologyQuery query)
        {
            CheckTechnologyModel executeResults = new CheckTechnologyModel();
            executeResults.PRODUCT_SUBTYPE = "";

            executeResults.PRODUCT_SUBTYPE = GetPackageListHelper.GetProductSubtype(_objService, query.P_OWNER_PRODUCT.ToSafeString(), query.P_ADDRESS_ID.ToSafeString());

            return executeResults;
        }
    }
}
