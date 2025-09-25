using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
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
    public class GetCustomerServiceProfileHandler : IQueryHandler<CustomerProfileInfoQuery, CustomerProfileInfoModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<CustomerProfileInfoModel> _entityRepository;

        public GetCustomerServiceProfileHandler(ILogger logger, IEntityRepository<FBB_INTERFACE_LOG> intfLog, IEntityRepository<CustomerProfileInfoModel> entityRepository, IWBBUnitOfWork uow)
        {
            _logger = logger;
            _intfLog = intfLog;
            _entityRepository = entityRepository;
            _uow = uow;
        }

        public CustomerProfileInfoModel Handle(CustomerProfileInfoQuery query)
        {
            InterfaceLogCommand log = null;
            var entity = new CustomerProfileInfoModel();

            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.InternetNo, "list_customer_service_profile", "pkg_fbb_change_pro_fieldwork.list_customer_service_profile", null, "FBBExternal", "WebWobbegong");

                var p_internet_no = new OracleParameter();
                p_internet_no.ParameterName = "p_internet_no";
                p_internet_no.Size = 2000;
                p_internet_no.OracleDbType = OracleDbType.Varchar2;
                p_internet_no.Direction = ParameterDirection.Input;
                p_internet_no.Value = query.InternetNo.ToSafeString();

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

                var list_customer_info = new OracleParameter();
                list_customer_info.ParameterName = "list_customer_info";
                list_customer_info.OracleDbType = OracleDbType.RefCursor;
                list_customer_info.Direction = ParameterDirection.Output;

                var list_service_info = new OracleParameter();
                list_service_info.ParameterName = "list_service_info";
                list_service_info.OracleDbType = OracleDbType.RefCursor;
                list_service_info.Direction = ParameterDirection.Output;

                var result = _entityRepository.ExecuteStoredProcMultipleCursor("WBB.pkg_fbb_change_pro_fieldwork.list_customer_service_profile",
                     new object[]
                      {
                          //Parameter Input
                          p_internet_no,

                          //out
                          return_code,
                          return_message,
                          list_customer_info,
                          list_service_info

                      });

                if (result != null)
                {
                    entity.Return_Code = return_code.Value.ToSafeString();
                    entity.Return_Message = return_message.Value.ToSafeString();

                    var d_list_customer_info_cur = (DataTable)result[2];
                    var list_customer_info_cur = d_list_customer_info_cur.DataTableToList<List_Customer_Info>();
                    entity.list_customer_info = list_customer_info_cur;

                    var d_list_service_info_cur = (DataTable)result[3];
                    var list_service_info_cur = d_list_service_info_cur.DataTableToList<List_Service_Info>();
                    entity.list_service_info = list_service_info_cur;
                }

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, entity, log, "Success", "", "WebWobbegong");

                return entity;

            }
            catch (Exception ex)
            {
                _logger.Info("ex message" + ex.Message + " error inner" + ex.InnerException);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", "inner" + ex.InnerException + " Message:" + ex.Message, "");
            }

            return new CustomerProfileInfoModel();
        }
    }
}
