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
    public class MinionGetListProfileContactQueryHandler :
        IQueryHandler<MinionGetListProfileContactQuery, List<MinionGetListProfileContactQueryModel>>
    {
        private readonly IAirNetEntityRepository<MinionGetListProfileContactQueryModel> _objService;

        public MinionGetListProfileContactQueryHandler(
            IAirNetEntityRepository<MinionGetListProfileContactQueryModel> objService)
        {
            _objService = objService;
        }

        public List<MinionGetListProfileContactQueryModel> Handle(MinionGetListProfileContactQuery query)
        {
            try
            {
                var ioResults = new OracleParameter
                {
                    ParameterName = "ioresults",
                    OracleDbType = OracleDbType.RefCursor,
                    Direction = ParameterDirection.Output
                };

                var executeResult = _objService.ExecuteReadStoredProc("AIR_ADMIN.PKG_AIROR904.LIST_PROFILE_CONTACT",
                    new
                    {
                        //in 
                        p_order_no = query.Order_No,
                        // Out
                        ioresults = ioResults

                    }).ToList();

                List<MinionGetListProfileContactQueryModel> minionGetListProfileContact = executeResult;
                return minionGetListProfileContact;

            }
            catch (Exception ex)
            {
                return new List<MinionGetListProfileContactQueryModel>
                {
                    new MinionGetListProfileContactQueryModel { ERROR_MSG = "Error call AIR_ADMIN.PKG_AIROR904.LIST_PROFILE_CONTACT " + ex.GetErrorMessage() }
                };
            }

        }
    }
}
