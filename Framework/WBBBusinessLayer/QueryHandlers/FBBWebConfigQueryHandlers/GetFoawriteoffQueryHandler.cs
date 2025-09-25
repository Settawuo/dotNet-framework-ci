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
    public class GetFoawriteoffQueryHandler : IQueryHandler<GetFoawriteoffQuery, List<FBSSFixedAssetSnAct>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBSSFixedAssetSnAct> _objService;

        public GetFoawriteoffQueryHandler(ILogger logger, IEntityRepository<FBSSFixedAssetSnAct> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public List<FBSSFixedAssetSnAct> Handle(GetFoawriteoffQuery query)
        {
            try
            {
                var ret_code = new OracleParameter();
                ret_code.ParameterName = "ret_code";
                ret_code.OracleDbType = OracleDbType.Decimal;
                //  ret_code.Size = 2000;
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
                    "WBB.PKG_FBB_Order_WriteOff.p_get_write_off",
                    new
                    {
                        p_access_number = query.p_access_number,
                        p_serialNumber = query.p_serialNumber,
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
                query.ret_msg = "PKG_FBB_Order_WriteOff.p_get_write_off Error : " + ex.Message;

                return null;
            }
        }
    }
}
