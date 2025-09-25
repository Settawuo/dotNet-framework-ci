using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.Repository;
using WBBEntity.Extensions;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class DeductionUpddateSendSMSFlagHandler : IQueryHandler<DeductionUpddateSendSMSFlagQuery, string>
    {
        private readonly IEntityRepository<object> _objService;

        public DeductionUpddateSendSMSFlagHandler(IEntityRepository<object> objService)
        {
            _objService = objService;
        }

        public string Handle(DeductionUpddateSendSMSFlagQuery query)
        {
            string results;
            try
            {
                var p_transaction_id = new OracleParameter();
                p_transaction_id.ParameterName = "p_transaction_id";
                p_transaction_id.Size = 2000;
                p_transaction_id.OracleDbType = OracleDbType.Varchar2;
                p_transaction_id.Direction = ParameterDirection.Input;
                p_transaction_id.Value = query.p_transaction_id;

                var p_mobile_no = new OracleParameter();
                p_mobile_no.ParameterName = "p_mobile_no";
                p_mobile_no.Size = 2000;
                p_mobile_no.OracleDbType = OracleDbType.Varchar2;
                p_mobile_no.Direction = ParameterDirection.Input;
                p_mobile_no.Value = query.p_nonmobile_no;

                var p_sms_flag = new OracleParameter();
                p_sms_flag.ParameterName = "p_sms_flag";
                p_sms_flag.Size = 2000;
                p_sms_flag.OracleDbType = OracleDbType.Varchar2;
                p_sms_flag.Direction = ParameterDirection.Input;
                p_sms_flag.Value = query.p_sms_flag;

                var p_update_by = new OracleParameter();
                p_update_by.ParameterName = "p_update_by";
                p_update_by.Size = 2000;
                p_update_by.OracleDbType = OracleDbType.Varchar2;
                p_update_by.Direction = ParameterDirection.Input;
                p_update_by.Value = query.p_update_by;

                var ret_code = new OracleParameter();
                ret_code.ParameterName = "ret_code";
                ret_code.Size = 2000;
                ret_code.OracleDbType = OracleDbType.Varchar2;
                ret_code.Direction = ParameterDirection.Output;

                var ret_message = new OracleParameter();
                ret_message.ParameterName = "ret_message";
                ret_message.Size = 2000;
                ret_message.OracleDbType = OracleDbType.Varchar2;
                ret_message.Direction = ParameterDirection.Output;

                var result = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBB_REGISTER_PAYMENT.PROC_UPD_DEDUCTION_SMS_FLAG",
                    new object[]
                    {
                        p_transaction_id,
                        p_mobile_no,
                        p_sms_flag,
                        p_update_by,
                         //return code
                         ret_code,
                         ret_message,
                    });

                var res_code = result[0] != null ? result[0].ToSafeString() : "-1";
                var res_message = result[1] != null ? result[1].ToSafeString() : "error";

                if (res_code == "0")
                {
                    results = "Y";
                }
                else
                {
                    results = "N";
                }
                //var dataForUpdate = (from r in _FBB_REGISTER_PENDING_DEDUCTION.Get()
                //            where r.TRANSACTION_ID == query.p_transaction_id
                //            && r.NON_MOBILE_NO == query.p_nonmobile_no
                //            select r).FirstOrDefault();
                //if (dataForUpdate != null)
                //{
                //    dataForUpdate.SEND_SMS_FLAG = "Y";
                //    _FBB_REGISTER_PENDING_DEDUCTION.Update(dataForUpdate);
                //    _uow.Persist();
                //    results = "Y";
                //}
            }
            catch (Exception ex)
            {
                results = "Error";
            }
            return results;
        }
    }
}
