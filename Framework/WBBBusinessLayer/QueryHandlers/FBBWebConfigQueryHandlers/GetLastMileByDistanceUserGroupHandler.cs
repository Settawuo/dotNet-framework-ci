using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class GetLastMileByDistanceUserGroupHandler : IQueryHandler<GetUserGroupQuery, LastMileByDistanceUserGroupModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<LastMileByDistanceUserGroupModel> _objService;

        public GetLastMileByDistanceUserGroupHandler(ILogger logger, IEntityRepository<LastMileByDistanceUserGroupModel> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public LastMileByDistanceUserGroupModel Handle(GetUserGroupQuery query)
        {
            try
            {
                var install_staff_name = new OracleParameter();
                install_staff_name.OracleDbType = OracleDbType.Varchar2;
                install_staff_name.Direction = ParameterDirection.Output;

                var v_user_group = new OracleParameter
                {
                    ParameterName = "v_user_group",
                    OracleDbType = OracleDbType.Varchar2,
                    Size = 20,
                    Direction = ParameterDirection.Output
                };

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                _objService.ExecuteStoredProc("WBB.PKG_FIXED_ASSET_LASTMILE.p_get_user_group",
                    out paramOut,
                    new
                    {
                        query.p_USER_NAME,
                        v_user_group
                    });

                var result = new LastMileByDistanceUserGroupModel();
                result.GROUP_NAME = v_user_group.Value.ToString();

                return result;
            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);
                return null;
            }
        }
    }
}
