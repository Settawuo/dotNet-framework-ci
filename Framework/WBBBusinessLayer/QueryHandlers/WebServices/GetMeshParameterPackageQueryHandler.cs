using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetMeshParameterPackageQueryHandler : IQueryHandler<GetMeshParameterPackageQuery, MeshParameterPackageModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<object> _objService;

        public GetMeshParameterPackageQueryHandler(ILogger logger, IWBBUnitOfWork uow, IEntityRepository<FBB_INTERFACE_LOG> intfLog, IEntityRepository<object> objService)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _objService = objService;
        }

        public MeshParameterPackageModel Handle(GetMeshParameterPackageQuery query)
        {
            MeshParameterPackageModel executeResults = new MeshParameterPackageModel();

            try
            {
                //var log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TransactionId, "CheckPackageDownloadSpeed", "FBBService", null, "FBB", "");

                var P_FIBRENET_ID = new OracleParameter();
                P_FIBRENET_ID.ParameterName = "p_fibrenet_id";
                P_FIBRENET_ID.Size = 2000;
                P_FIBRENET_ID.OracleDbType = OracleDbType.Varchar2;
                P_FIBRENET_ID.Direction = ParameterDirection.Input;
                P_FIBRENET_ID.Value = query.FibrenetID;

                var P_SFF_PROMOTION_CODE = new OracleParameter();
                P_SFF_PROMOTION_CODE.ParameterName = "p_sff_promotion_code";
                P_SFF_PROMOTION_CODE.Size = 2000;
                P_SFF_PROMOTION_CODE.OracleDbType = OracleDbType.Varchar2;
                P_SFF_PROMOTION_CODE.Direction = ParameterDirection.Input;
                P_SFF_PROMOTION_CODE.Value = query.PromotionCode;

                var RETURN_CODE = new OracleParameter();
                RETURN_CODE.ParameterName = "return_code";
                RETURN_CODE.OracleDbType = OracleDbType.Decimal;
                RETURN_CODE.Direction = ParameterDirection.Output;

                var RETURN_MESSAGE = new OracleParameter();
                RETURN_MESSAGE.ParameterName = "return_message";
                RETURN_MESSAGE.Size = 2000;
                RETURN_MESSAGE.OracleDbType = OracleDbType.Varchar2;
                RETURN_MESSAGE.Direction = ParameterDirection.Output;

                var RETURN_CALL = new OracleParameter();
                RETURN_CALL.ParameterName = "return_call";
                RETURN_CALL.OracleDbType = OracleDbType.RefCursor;
                RETURN_CALL.Direction = ParameterDirection.Output;

                _logger.Info("Start PKG_FBBOR041.PACKAGE_PARAMETER");

                var result = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBBOR041.PACKAGE_PARAMETER",
                    new object[]
                    {
                         P_FIBRENET_ID,
                         P_SFF_PROMOTION_CODE,

                         //return code
                         RETURN_CODE,
                         RETURN_MESSAGE,
                         RETURN_CALL
                    });

                executeResults.RETURN_CODE = result[0] != null ? result[0].ToSafeString() : "-1";
                executeResults.RETURN_MESSAGE = result[1] != null ? result[1].ToSafeString() : "error";
                if (executeResults.RETURN_CODE != "-1")
                {
                    DataTable data1 = (DataTable)result[2];
                    executeResults.RES_COMPLETE_CUR = data1.DataTableToList<ParameterPackage>();
                    //InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, listDataForAppointment, log, "Success", "", "");
                }
                else
                {
                    //InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, Return_Message, log, "Failed", Return_Message, "");
                }

            }
            catch (Exception ex)
            {
                _logger.Info("Error call PKG_FBBOR041.PACKAGE_PARAMETER handles : " + ex.Message);

                executeResults.RETURN_CODE = "-1";
                executeResults.RETURN_MESSAGE = "Error";

                return null;
            }
            return executeResults;
        }
    }
}
