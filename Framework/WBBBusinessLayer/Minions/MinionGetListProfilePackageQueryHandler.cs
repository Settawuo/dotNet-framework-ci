using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Minions;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Minions;

namespace WBBBusinessLayer.Minions
{
    public class MinionGetListProfilePackageQueryHandler :
        IQueryHandler<MinionGetListProfilePackageQuery, List<MinionGetListProfilePackageQueryModel>>
    {
        private readonly IAirNetEntityRepository<MinionGetListProfilePackageQueryModel> _objService;

        public MinionGetListProfilePackageQueryHandler(
            IAirNetEntityRepository<MinionGetListProfilePackageQueryModel> objService)
        {
            _objService = objService;
        }

        public List<MinionGetListProfilePackageQueryModel> Handle(MinionGetListProfilePackageQuery query)
        {
            try
            {
                var ioResults = new OracleParameter
                {
                    ParameterName = "ioresults",
                    OracleDbType = OracleDbType.RefCursor,
                    Direction = ParameterDirection.Output
                };

                var executeResult = _objService.ExecuteReadStoredProc("AIR_ADMIN.PKG_AIROR904.LIST_PROFILE_PACKAGE",
                    new
                    {
                        //in 
                        p_order_no = query.Order_No,
                        // Out
                        ioresults = ioResults

                    }).ToList();

                List<MinionGetListProfilePackageQueryModel> minionGetListProfileCutomer = executeResult;
                return minionGetListProfileCutomer;

            }
            catch (Exception ex)
            {
                return new List<MinionGetListProfilePackageQueryModel>
                {
                    new MinionGetListProfilePackageQueryModel { ERROR_MSG = "Error call AIR_ADMIN.PKG_AIROR904.LIST_PROFILE_PACKAGE " + ex.GetErrorMessage() }
                };
            }

        }
    }
}
