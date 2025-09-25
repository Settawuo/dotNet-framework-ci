using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.Repository;

namespace WBBBusinessLayer.CommandHandlers
{
    public class InsertFbbConsentLogQueryHandler : IQueryHandler<InsertFbbConsentLogQuery, string>
    {
        private readonly IEntityRepository<object> _storeRepository;

        public InsertFbbConsentLogQueryHandler(IEntityRepository<object> storeRepository)
        {
            _storeRepository = storeRepository;
        }

        public string Handle(InsertFbbConsentLogQuery command)
        {
            var outp = new List<object>();
            var paramOut = outp.ToArray();

            var output_return_code = new OracleParameter();
            output_return_code.OracleDbType = OracleDbType.Decimal;
            output_return_code.Direction = ParameterDirection.Output;

            var output_return_message = new OracleParameter();
            output_return_message.OracleDbType = OracleDbType.Varchar2;
            output_return_message.Size = 2000;
            output_return_message.Direction = ParameterDirection.Output;

            try
            {
                var channel_flag = command.employee_id == "" || command.employee_id == null ? "CUSTOMER" : "OFFICER";
                var executeResult = _storeRepository.ExecuteStoredProc("WBB.PKG_FBB_INSERT_LOG.INSERT_CONSENT_LOG",
                   out paramOut,
                   new
                   {
                       P_CREATED_BY = "WBB",
                       P_UPDATED_BY = "WBB",
                       P_INTERNET_NO = command.internet_no,
                       P_CHANNEL = channel_flag,
                       P_LOCATION_CODE = command.location_code,
                       P_ASC_CODE = command.asc_code,
                       P_EMPLOYEE_ID = command.employee_id,
                       P_EMPLOYEE_NAME = command.employee_name,
                       P_CONTACT_MOBILE = command.contact_mobile,
                       P_TYPE_FLAG = command.type_flag,
                       P_VALUE_FLAG = "Y",//
                       P_REF_ORDER_NO = "",
                       P_REMARK = "",
                       P_CLIENT_ID = command.ip_address,
                       RETURN_CODE = output_return_code,
                       RETURN_MESSAGE = output_return_message,

                   });
                return output_return_message.Value.ToString();


            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
