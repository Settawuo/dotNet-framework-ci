using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.ExWebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.ExWebServices
{
    public class ServiceLogNewRegisterHandler : IQueryHandler<ServiceLogNewRegisterQuery, ServiceLogNewRegisterModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<ServiceLogNewRegisterModel> _entityRepository;


        public ServiceLogNewRegisterHandler(ILogger logger, IEntityRepository<FBB_INTERFACE_LOG> intfLog, IEntityRepository<ServiceLogNewRegisterModel> entityRepository, IWBBUnitOfWork uow)
        {
            _logger = logger;
            _intfLog = intfLog;
            _entityRepository = entityRepository;
            _uow = uow;
        }

        public ServiceLogNewRegisterModel Handle(ServiceLogNewRegisterQuery query)
        {
            InterfaceLogCommand log = null;
            var entity = new ServiceLogNewRegisterModel();

            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.OrderNo, "get_servicelog", "pkg_fbb_change_pro_fieldwork.get_servicelog", null, "FBBExternal", "WebWobbegong");

                var p_order_no = new OracleParameter();
                p_order_no.ParameterName = "p_order_no";
                p_order_no.Size = 2000;
                p_order_no.OracleDbType = OracleDbType.Varchar2;
                p_order_no.Direction = ParameterDirection.Input;
                p_order_no.Value = query.OrderNo.ToSafeString();

                var p_service_name = new OracleParameter();
                p_service_name.ParameterName = "p_service_name";
                p_service_name.Size = 2000;
                p_service_name.OracleDbType = OracleDbType.Varchar2;
                p_service_name.Direction = ParameterDirection.Input;
                p_service_name.Value = query.ServiceName.ToSafeString();

                var return_code = new OracleParameter();
                return_code.ParameterName = "return_code";
                return_code.Size = 2000;
                return_code.OracleDbType = OracleDbType.Varchar2;
                return_code.Direction = ParameterDirection.Output;

                var return_message = new OracleParameter();
                return_message.ParameterName = "return_message";
                return_message.Size = 2000;
                return_message.OracleDbType = OracleDbType.Varchar2;
                return_message.Direction = ParameterDirection.Output;

                var return_service_data = new OracleParameter();
                return_service_data.ParameterName = "return_service_data";
                return_service_data.OracleDbType = OracleDbType.Clob;
                return_service_data.Direction = ParameterDirection.Output;

                Dictionary<string, object> result = _entityRepository.ExecuteStoredProcExecuteReader("WBB.pkg_fbb_change_pro_fieldwork.get_servicelog",
                     new object[]
                      {
                          //Parameter Input
                          p_order_no,
                          p_service_name,

                          //out
                          return_code,
                          return_message,
                          return_service_data
                      });

                entity.Return_Code = result.FirstOrDefault(x => x.Key == "return_code").Value.ToString();
                entity.Return_Message = result.FirstOrDefault(x => x.Key == "return_message").Value.ToString();
                entity.Return_Service_Data = result.FirstOrDefault(x => x.Key == "return_service_data").Value.ToString();

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, entity, log, "Success", "", "WebWobbegong");

                return entity;

            }
            catch (Exception ex)
            {
                _logger.Info("ex message" + ex.Message + " error inner" + ex.InnerException);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", "inner" + ex.InnerException + " Message:" + ex.Message, "");
            }

            return new ServiceLogNewRegisterModel();
        }
    }
}
