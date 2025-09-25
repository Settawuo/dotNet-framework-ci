using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
using WBBData.Repository;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.SAPOnline
{
    public class FBBReturnedFixedAssetQueryHandler : IQueryHandler<FBBReturnedFixedAssetQuery, FBBReturnedFixedAssetModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBBReturnedCpeModel> _objService;

        public FBBReturnedFixedAssetQueryHandler(ILogger logger
            , IEntityRepository<FBBReturnedCpeModel> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public FBBReturnedFixedAssetModel Handle(FBBReturnedFixedAssetQuery query)
        {
            var returnForm = new FBBReturnedFixedAssetModel();
            try
            {
                var ret_code = new OracleParameter
                {
                    OracleDbType = OracleDbType.Decimal,
                    Direction = ParameterDirection.Output
                };

                var cur = new OracleParameter
                {
                    OracleDbType = OracleDbType.RefCursor,
                    Direction = ParameterDirection.Output
                };

                _logger.Info("Start WBB.PKG_FBB_FOA_ORDER_MANAGEMENT.p_returned_cpe");

                var executeResult = _objService.ExecuteReadStoredProc(
                    "WBB.PKG_FBB_FOA_ORDER_MANAGEMENT.p_returned_cpe",
                    new
                    {
                        ret_code,
                        cur
                    }).ToList();

                _logger.Info("End WBB.PKG_FBB_FOA_ORDER_MANAGEMENT.p_returned_cpe");


                returnForm.ret_code = ret_code.Value != null ? ret_code.Value.ToString() : "-1";
                returnForm.cur = executeResult;

                return returnForm;
            }
            catch (Exception ex)
            {
                _logger.Info("Error call WBB.PKG_FBB_FOA_ORDER_MANAGEMENT.p_returned_cpe handler : " + ex.Message);
                returnForm.ret_code = ex.Message;

                return null;
            }
        }
    }
}
