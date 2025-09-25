using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class GetSubContractEmailInfoQueryHandler : IQueryHandler<GetSubContractEmailInfoQuery, List<SubContractEmailInfoModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<SubContractEmailInfoModel> _objService;

        public GetSubContractEmailInfoQueryHandler(ILogger logger, IEntityRepository<SubContractEmailInfoModel> objService)
        {
            _logger = logger;
            _objService = objService;
        }
        public List<SubContractEmailInfoModel> Handle(GetSubContractEmailInfoQuery query)
        {
            try
            {
                var ret_code = new OracleParameter();
                ret_code.ParameterName = "ret_code";
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.ParameterName = "ret_msg";
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;

                var cur = new OracleParameter();
                cur.OracleDbType = OracleDbType.RefCursor;
                cur.Direction = ParameterDirection.Output;
                cur.ParameterName = "cur";

                var result = _objService.ExecuteReadStoredProc(
                    "WBB.PKG_SUBCONTRACT_EMAIL_INFO.p_get_subcontract",
                    new
                    {
                        p_subcontract_name = query.p_subcontract_name,
                        p_storage = query.p_storage,
                        p_subcontract_code = query.p_subcontract_code,
                        p_Action_flag = query.p_Action_flag,
                        cur,
                        ret_code,
                        ret_msg

                    }).ToList();
                query.ret_code = ret_code.Value != null ? ret_code.Value.ToString() : "-1";
                query.ret_msg = ret_msg.Value.ToString();
                return result;
            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);

                query.ret_code = "-1";
                query.ret_msg = "PKG_SUBCONTRACT_EMAIL_INFO.p_get_subcontract Error : " + ex.Message;

                return null;
            }
        }
    }
}
