using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class ReverseAssetQueryHandler : IQueryHandler<ReverseAssetQuery, List<ReverseAssetModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<ReverseAssetModel> _objService;
        private readonly IEntityRepository<FBB_HISTORY_LOG> _historyLog;
        private readonly IWBBUnitOfWork _uow;

        public ReverseAssetQueryHandler(ILogger logger, IEntityRepository<ReverseAssetModel> objService
            , IEntityRepository<FBB_HISTORY_LOG> historyLog, IWBBUnitOfWork uow)
        {
            _logger = logger;
            _objService = objService;
            _historyLog = historyLog;
            _uow = uow;
        }
        public List<ReverseAssetModel> Handle(ReverseAssetQuery query)
        {

            var historyLog = new FBB_HISTORY_LOG();

            try
            {


                var p_get_data_cur = new OracleParameter
                {
                    ParameterName = "p_get_data_cur",
                    OracleDbType = OracleDbType.RefCursor,
                    Direction = ParameterDirection.Output
                };


                List<ReverseAssetModel> executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_FBB_FOA_ORDER_MANAGEMENT.p_search_reverse_asset",
                      new
                      {
                          //   packageMappingReverse, 
                          query.p_ACCESS_NO,
                          query.p_ASSET_CODE,
                          p_get_data_cur,

                          //ret_msg = ret_msg,

                      }).ToList();
                return executeResult;
            }

            catch (Exception ex)
            {
                query.ret_msg = "Error ReverseAssetQuery PKG_FBB_FOA_ORDER_MANAGEMENT.p_search_reverse_asset  " + ex.Message.ToString();
                _logger.Info("Error ReverseAssetQuery Call PKG_FBB_FOA_ORDER_MANAGEMENT.p_search_reverse_asset : " + ex.Message);


                historyLog.HISTORY_ID = historyLog.HISTORY_ID;
                historyLog.ACTION = ActionHistory.ADD.ToString();
                historyLog.APPLICATION = "FBBConfig Reverse Asset";
                historyLog.CREATED_BY = "FBB Reverse Asset";
                historyLog.CREATED_DATE = DateTime.Now;
                historyLog.DESCRIPTION = query.ret_msg;
                historyLog.REF_KEY = "FBB Reverse Asset";
                historyLog.REF_NAME = "NODEID";
                _historyLog.Create(historyLog);
                _uow.Persist();
                return null;
            }


        }
    }

}
