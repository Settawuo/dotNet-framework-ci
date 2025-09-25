using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBBContract;
using WBBContract.Commands;
using WBBData.Repository;
using WBBEntity.Extensions;

namespace WBBBusinessLayer.CommandHandlers
{
    public class GenPasswordCommandHandler : ICommandHandler<GenPasswordCommand>
    {
        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<string> _objServiceAir;
        private readonly IEntityRepository<string> _objService;
        public GenPasswordCommandHandler(ILogger logger, IAirNetEntityRepository<string> objServiceAir, IEntityRepository<string> objService)
        {
            _logger = logger;
            _objServiceAir = objServiceAir;
            _objService = objService;
        }
        public void Handle(GenPasswordCommand command)
        {
            try
            {
                var o_return_code = new OracleParameter();
                o_return_code.OracleDbType = OracleDbType.Decimal;
                o_return_code.Direction  = ParameterDirection.Output;

                var o_password = new OracleParameter();
                o_password.OracleDbType = OracleDbType.Varchar2;
                o_password.Size = 2000;
                o_password.Direction = ParameterDirection.Output;

                var o_return_message = new OracleParameter();
                o_return_message.OracleDbType = OracleDbType.Varchar2;
                o_return_message.Size = 2000;
                o_return_message.Direction = ParameterDirection.Output;

                var data = new OracleParameter();
                data.OracleDbType = OracleDbType.Varchar2;
                data.Size = 2000;
                data.Direction = ParameterDirection.Output;

                var result = new OracleParameter();
                result.OracleDbType = OracleDbType.Varchar2;
                result.Size = 2000;
                result.Direction = ParameterDirection.Output;

                //var p_product_type = new OracleParameter();
                //p_product_type.OracleDbType = OracleDbType.Varchar2;
                //p_product_type.Size = 2000;
                //p_product_type.Direction = ParameterDirection.Input;

                //var p_customer_id = new OracleParameter();
                //p_customer_id.OracleDbType = OracleDbType.Varchar2;
                //p_customer_id.Size = 2000;
                //p_customer_id.Direction = ParameterDirection.Input;

                //var p_user_name = new OracleParameter();
                //p_user_name.OracleDbType = OracleDbType.Varchar2;
                //p_user_name.Size = 2000;
                //p_user_name.Direction = ParameterDirection.Input;

                var o_error_msg = new OracleParameter();
                o_error_msg.OracleDbType = OracleDbType.Varchar2;
                o_error_msg.Size = 2000;
                o_error_msg.Direction = ParameterDirection.Output;

                var o_ia_no = new OracleParameter();
                o_ia_no.OracleDbType = OracleDbType.Varchar2;
                o_ia_no.Size = 2000;
                o_ia_no.Direction = ParameterDirection.Output;
                
                
                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var executePassword = _objServiceAir.ExecuteStoredProc("AIR_ADMIN.PKG_AIROR901.GEN_PASSWORD",
                out paramOut,
                  new
                  {
                      o_return_code = o_return_code,
                      o_return_message = o_return_message,
                      o_password = o_password
                  });

               command.Genpassword= o_password.Value.ToString();

          
                var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FBBDORM_PASSWORD.DECRYPT",
                out paramOut,
                  new
                  {
                      data = command.Genpassword,
                      result = result
                  });

                command.PasswordDec = result.Value.ToString();

                //var executeIANO = _objServiceAir.ExecuteStoredProc("AIR_ADMIN.PKG_AIROR900.GEN_IA_NO",
                //out paramOut,
                //  new
                //  {
                //            p_product_type = command.ProductType.ToSafeString(),
                //            p_customer_id = command.CustID.ToSafeString(),
                //            p_user_name = command.UserName.ToSafeString(),
                //            o_error_msg = o_error_msg,
                //            o_ia_no = o_ia_no
                //  });

                //command.IA_NO = o_ia_no.Value.ToString();


            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                command.ret_code = -1;
                command.ret_msg = "Error call SavePreregister Handler: " + ex.GetErrorMessage(); 
            }
        }
    }
}
