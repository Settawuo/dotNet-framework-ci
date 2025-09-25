using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetListGroupMailSpecialAccountQueryHandler : IQueryHandler<GetListGroupMailSpecialAccountQuery, GetListGroupMailSpecialAccountModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<GetListGroupMailSpecialAccountModel> _objService;

        public GetListGroupMailSpecialAccountQueryHandler(ILogger logger, IEntityRepository<GetListGroupMailSpecialAccountModel> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public GetListGroupMailSpecialAccountModel Handle(GetListGroupMailSpecialAccountQuery query)
        {
            GetListGroupMailSpecialAccountModel executeResults = new GetListGroupMailSpecialAccountModel();

            try
            {
                var return_code = new OracleParameter();
                return_code.ParameterName = "RETURN_CODE";
                return_code.Size = 2000;
                return_code.OracleDbType = OracleDbType.Varchar2;
                return_code.Direction = ParameterDirection.Output;

                var return_message = new OracleParameter();
                return_message.ParameterName = "RETURN_MESSAGE";
                return_message.Size = 2000;
                return_message.OracleDbType = OracleDbType.Varchar2;
                return_message.Direction = ParameterDirection.Output;

                var list_mail_special_account = new OracleParameter();
                list_mail_special_account.ParameterName = "LIST_MAIL_SPECIAL_ACCOUNT";
                list_mail_special_account.OracleDbType = OracleDbType.RefCursor;
                list_mail_special_account.Direction = ParameterDirection.Output;

                _logger.Info("Start PKG_FBBOR004.PROC_LIST_MAIL_SPECIAL_ACCOUNT");

                var result = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBBOR004.PROC_LIST_MAIL_SPECIAL_ACCOUNT",
                    new object[]
                    {
                        //return code
                        return_code,
                        return_message,
                        list_mail_special_account
                    });

                if (result != null)
                {
                    executeResults.RETURN_CODE = result[0] != null ? result[0].ToString() : "-1";
                    executeResults.RETURN_MESSAGE = result[1] != null ? result[1].ToString() : "";

                    var d_list_mail_special_account = (DataTable)result[2];
                    var LIST_MAIL_SPECIAL_ACCOUNT = d_list_mail_special_account.DataTableToList<GroupMailSpecialAccount>();
                    executeResults.LIST_MAIL_SPECIAL_ACCOUNT = LIST_MAIL_SPECIAL_ACCOUNT;

                    _logger.Info("End PKG_FBBOR004.PROC_LIST_MAIL_SPECIAL_ACCOUNT " + executeResults.RETURN_MESSAGE);
                }

            }
            catch (Exception ex)
            {
                _logger.Info("Error call PKG_FBBOR004.PROC_LIST_MAIL_SPECIAL_ACCOUNT handles : " + ex.Message);

                executeResults.RETURN_CODE = "-1";
                executeResults.RETURN_MESSAGE = "Error";

                return null;
            }
            return executeResults;
        }
    }
}
