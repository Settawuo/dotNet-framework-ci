using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using WBBContract;
using WBBContract.Queries.ExWebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.ExWebServices
{
    public class GetPatchAdressESRIQueryHandler : IQueryHandler<GetPatchAdressESRIQuery, PatchAdressESRIModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<object> _objService;

        public GetPatchAdressESRIQueryHandler(ILogger logger, IWBBUnitOfWork uow, IEntityRepository<FBB_INTERFACE_LOG> intfLog, IEntityRepository<object> objService)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _objService = objService;
        }

        public PatchAdressESRIModel Handle(GetPatchAdressESRIQuery query)
        {
            //InterfaceLogCommand log = null;
            PatchAdressESRIModel executeResults = new PatchAdressESRIModel();
            try
            {
                //log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, "", "PatchAdressESRI", "GetPatchAdressESRIQueryHandler", null, "FBB", "");

                var P_SUB_DISTRICT = new OracleParameter();
                P_SUB_DISTRICT.ParameterName = "P_SUB_DISTRICT";
                P_SUB_DISTRICT.Size = 2000;
                P_SUB_DISTRICT.OracleDbType = OracleDbType.Varchar2;
                P_SUB_DISTRICT.Direction = ParameterDirection.Input;
                P_SUB_DISTRICT.Value = query.SubDistrict.ToSafeString();

                var P_DISTRICT = new OracleParameter();
                P_DISTRICT.ParameterName = "P_DISTRICT";
                P_DISTRICT.Size = 2000;
                P_DISTRICT.OracleDbType = OracleDbType.Varchar2;
                P_DISTRICT.Direction = ParameterDirection.Input;
                P_DISTRICT.Value = query.District.ToSafeString();

                var P_PROVINCE = new OracleParameter();
                P_PROVINCE.ParameterName = "P_PROVINCE";
                P_PROVINCE.Size = 2000;
                P_PROVINCE.OracleDbType = OracleDbType.Varchar2;
                P_PROVINCE.Direction = ParameterDirection.Input;
                P_PROVINCE.Value = query.Province.ToSafeString();

                var P_LANG = new OracleParameter();
                P_LANG.ParameterName = "P_LANG";
                P_LANG.Size = 2000;
                P_LANG.OracleDbType = OracleDbType.Varchar2;
                P_LANG.Direction = ParameterDirection.Input;
                P_LANG.Value = query.Lang.ToSafeString();

                var RETURN_CODE = new OracleParameter();
                RETURN_CODE.ParameterName = "RETURN_CODE";
                RETURN_CODE.Size = 10;
                RETURN_CODE.OracleDbType = OracleDbType.Varchar2;
                RETURN_CODE.Direction = ParameterDirection.Output;

                var RETURN_MESSAGE = new OracleParameter();
                RETURN_MESSAGE.ParameterName = "RETURN_MESSAGE";
                RETURN_MESSAGE.Size = 2000;
                RETURN_MESSAGE.OracleDbType = OracleDbType.Varchar2;
                RETURN_MESSAGE.Direction = ParameterDirection.Output;

                var LIST_ADDRESS = new OracleParameter();
                LIST_ADDRESS.ParameterName = "list_address";
                LIST_ADDRESS.OracleDbType = OracleDbType.RefCursor;
                LIST_ADDRESS.Direction = ParameterDirection.Output;

                _logger.Info("Start PKG_FBB_QUERY_CONFIG_TIMESLOT.PROC_PATCH_ADDRESS_ESRI");

                var result = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBB_QUERY_CONFIG_TIMESLOT.PROC_PATCH_ADDRESS_ESRI",
                    new object[]
                    {
                         P_SUB_DISTRICT,
                         P_DISTRICT,
                         P_PROVINCE,
                         P_LANG,

                         //return code
                         RETURN_CODE,
                         RETURN_MESSAGE,
                         LIST_ADDRESS
                    });

                executeResults.RETURN_CODE = result[0] != null ? result[0].ToSafeString() : "-1";
                executeResults.RETURN_MESSAGE = result[1] != null ? result[1].ToSafeString() : "Error";

                if (executeResults.RETURN_CODE != "-1")
                {
                    DataTable data1 = (DataTable)result[2];
                    executeResults.list_address = data1.DataTableToList<PatchAdressESRIData>();
                    //InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResults, log, "Success", "", "");
                }
                else
                {
                    executeResults.list_address = null;
                    //InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResults, log, "Failed", executeResults.RETURN_MESSAGE, "");
                }

            }
            catch (Exception ex)
            {
                _logger.Info("Error call PKG_FBB_QUERY_CONFIG_TIMESLOT.PROC_PATCH_ADDRESS_ESRI handles : " + ex.Message);

                executeResults.RETURN_CODE = "-1";
                executeResults.RETURN_MESSAGE = "Error call PKG_FBB_QUERY_CONFIG_TIMESLOT.PROC_PATCH_ADDRESS_ESRI handles : " + ex.Message;
                executeResults.list_address = null;

                //InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResults.RETURN_MESSAGE, log, "Failed", executeResults.RETURN_MESSAGE, "");
            }
            return executeResults;
        }
    }
}
