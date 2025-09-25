using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using WBBContract;
using WBBContract.Commands.ExWebServices.SAPFixedAsset;
using WBBData.Repository;
using WBBEntity.Extensions;

namespace WBBBusinessLayer.CommandHandlers.ExWebServices.SAPFixedAsset
{
    public class SubmitFOAErrorLogCommandHandler : ICommandHandler<SumitFOAErrorLogCommand>
    {
        private readonly ILogger _logger;

        private readonly IEntityRepository<string> _objService;


        public SubmitFOAErrorLogCommandHandler(ILogger ILogger, IEntityRepository<string> objService)
        {
            _objService = objService;
            _logger = ILogger;
        }
        public void Handle(SumitFOAErrorLogCommand command)
        {
            try
            {
                var culture = CultureInfo.GetCultureInfo("en-US");
                _logger.Info("startInsertErrorLog");


                var bufferData = new OracleParameter();
                bufferData.OracleDbType = OracleDbType.Clob;
                bufferData.Value = command.in_xml_foa;
                bufferData.Direction = ParameterDirection.Input;

                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Varchar2;
                ret_code.Size = 2000;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;
                DateTime dt = DateTime.Now;
                string dd = dt.ToString("dd/MM/yyyy HHmmss", culture);
                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var execute = _objService.ExecuteStoredProc("WBB.PKG_FBB_FOA_ORDER_MANAGEMENT.p_insert_foa_resend_error_log",
                    out paramOut,
                    new
                    {

                        p_access_number = command.access_number,
                        p_in_xml_foa = bufferData,
                        p_resend_status = command.resend_status,
                        p_created_by = command.created_by,
                        p_created_date = dd,
                        p_updated_by = command.updated_by,
                        p_updated_date = dd,
                        p_updated_desc = command.updated_desc,
                        ret_code,
                        ret_msg

                    });
                command.ret_code = ret_code.Value.ToSafeString();
                command.ret_msg = ret_msg.Value.ToSafeString();



            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                _logger.Info("inserterror.p_add_configuration_cost" + msg);

            }

        }
    }
}
