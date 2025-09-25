using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;


namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class GetAddressIDRegisterBulkCorpQueryHandler : IQueryHandler<GetAddressIDRegisterBulkCorpQuery, RetGetAddrID>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<RetGetAddrID> _objService;

        public GetAddressIDRegisterBulkCorpQueryHandler(ILogger logger, IEntityRepository<RetGetAddrID> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public RetGetAddrID Handle(GetAddressIDRegisterBulkCorpQuery query)
        {
            RetGetAddrID resultData = new RetGetAddrID();
            try
            {
                _logger.Info("Start GetAddressIDRegisterBulkCorpQueryHandler");

                var output_return_code = new OracleParameter();
                output_return_code.ParameterName = "output_return_code";
                output_return_code.OracleDbType = OracleDbType.Decimal;
                output_return_code.Direction = ParameterDirection.Output;

                var output_return_message = new OracleParameter();
                output_return_message.ParameterName = "output_return_message";
                output_return_message.OracleDbType = OracleDbType.Varchar2;
                output_return_message.Size = 2000;
                output_return_message.Direction = ParameterDirection.Output;

                var executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_FBBBULK_CORP_REGISTER.PROC_CHECK_EVENT",
                            new
                            {
                                P_EVENT_CODE = query.P_EVENT_CODE,
                                P_ADDRESS_ID = query.P_ADDRESS_ID,

                                //  return code
                                output_return_code = output_return_code,
                                output_return_message = output_return_message

                            }).ToList();


                resultData.output_return_code = output_return_code.Value.ToSafeString();
                resultData.output_return_message = output_return_message.Value.ToSafeString();

                if (output_return_code.Value.ToSafeString() == "-1")
                {
                    _logger.Info(output_return_code + " " + output_return_message);
                    return resultData;

                }
                else if (output_return_code.Value.ToSafeString() == "0")
                {
                    _logger.Info(output_return_code + " " + output_return_message);
                    return resultData;
                }
                else
                {
                    _logger.Info("Error when call WBB.PKG_FBBBULK_CORP_REGISTER.PROC_CHECK_EVENT");
                    return null;
                }


            }
            catch (Exception ex)
            {
                _logger.Info("Error when call WBB.PKG_FBBBULK_CORP_REGISTER.PROC_CHECK_EVENT " + ex.Message);
                return null;
            }


        }

    }
}
