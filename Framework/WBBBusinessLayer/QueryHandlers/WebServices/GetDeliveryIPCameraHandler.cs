using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
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
    public class GetDeliveryIPCameraHandler : IQueryHandler<GetDeliveryIPCameraQuery, GetDeliveryIPCameraModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<object> _objService;

        public GetDeliveryIPCameraHandler(ILogger logger, IWBBUnitOfWork uow, IEntityRepository<FBB_INTERFACE_LOG> intfLog, IEntityRepository<object> objService)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _objService = objService;
        }

        public GetDeliveryIPCameraModel Handle(GetDeliveryIPCameraQuery query)
        {
            var log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, "", "GetDeliveryIPCamera", "GetDeliveryIPCameraHandler", null, "FBB", "");

            GetDeliveryIPCameraModel results = new GetDeliveryIPCameraModel();
            try
            {
                var p_province = new OracleParameter();
                p_province.ParameterName = "p_province";
                p_province.Size = 2000;
                p_province.OracleDbType = OracleDbType.Varchar2;
                p_province.Direction = ParameterDirection.Input;
                p_province.Value = query.province;


                var ret_code = new OracleParameter();
                ret_code.ParameterName = "RETURN_CODE";
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_message = new OracleParameter();
                ret_message.ParameterName = "RETURN_MESSAGE";
                ret_message.Size = 2000;
                ret_message.OracleDbType = OracleDbType.Varchar2;
                ret_message.Direction = ParameterDirection.Output;

                var return_delivery = new OracleParameter();
                return_delivery.ParameterName = "RETURN_DELIVERY_CURROR";
                return_delivery.OracleDbType = OracleDbType.RefCursor;
                return_delivery.Direction = ParameterDirection.Output;

                var result = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBB_DELIVERY.GET_DELIVERY_TEXT",
                    new object[]
                    {
                        p_province,

                         //return code
                         ret_code,
                         ret_message,
                         return_delivery
                    });

                results.RETURN_CODE = result[0] != null ? result[0].ToSafeString() : "-1";
                results.RETURN_MESSAGE = result[1] != null ? result[1].ToSafeString() : "error";
                if (results.RETURN_CODE != "-1")
                {
                    DataTable data = (DataTable)result[2];
                    var dataJsonStr = JsonConvert.SerializeObject(data, Formatting.Indented);
                    var response = JsonConvert.DeserializeObject<List<GetdeliveryIPCameraList>>(dataJsonStr);
                    results.RETURN_DELIVERY_CURROR = response;
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, results, log, "Success", "", "");
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, results, log, "Failed", results.RETURN_MESSAGE, "");
                }

            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex.Message, log, "Failed", ex.Message, "");
                results.RETURN_CODE = "-1";
                results.RETURN_MESSAGE = "Error";
            }
            return results;
        }
    }
}
