using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class GetFormatFileNameFraudQueryHandle : IQueryHandler<GetFormatFileNameFraudQuery, FileFormatFraudModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FileFormatFraudModel> _objService;


        public GetFormatFileNameFraudQueryHandle(ILogger logger, IEntityRepository<FileFormatFraudModel> objService)
        {
            _logger = logger;
            _objService = objService;

        }
        public FileFormatFraudModel Handle(GetFormatFileNameFraudQuery query)
        {
            try
            {
                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_message = new OracleParameter();
                ret_message.OracleDbType = OracleDbType.Varchar2;
                ret_message.Size = 2000;
                ret_message.Direction = ParameterDirection.Output;

                var ret_file_name = new OracleParameter();
                ret_file_name.OracleDbType = OracleDbType.Varchar2;
                ret_file_name.Size = 2000;
                ret_file_name.Direction = ParameterDirection.Output;

                FileFormatFraudModel executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_FBBOR004_FRAUD.PROC_GEN_FILE_NAME_FRAUD",
                  new
                  {
                      p_id_card_no = query.ID_CardNo,

                      ///// return //////
                      ret_code = ret_code,
                      ret_message = ret_message,
                      ret_file_name = ret_file_name

                  }).FirstOrDefault();

                var Return_Code = ret_code.Value != null ? Convert.ToInt32(ret_code.Value.ToSafeString()) : -1;
                var Return_Desc = ret_message.Value.ToSafeString();
                FileFormatFraudModel returnData = new FileFormatFraudModel();
                returnData.file_name = ret_file_name.Value.ToSafeString();

                return returnData;
            }
            catch (Exception ex)
            {
                return new FileFormatFraudModel();
            }
        }
    }
}
