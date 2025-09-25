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
    public class MinionGetListProfileCustomerQueryHandler :
        IQueryHandler<MinionGetListProfileCustomerQuery, List<MinionGetListProfileCustomerQueryModel>>
    {
        private readonly IAirNetEntityRepository<MinionGetListProfileCustomerQueryModel> _objService;

        public MinionGetListProfileCustomerQueryHandler(
            IAirNetEntityRepository<MinionGetListProfileCustomerQueryModel> objService)
        {
            _objService = objService;
        }

        public List<MinionGetListProfileCustomerQueryModel> Handle(MinionGetListProfileCustomerQuery query)
        {
            try
            {
                var ioResults = new OracleParameter
                {
                    ParameterName = "ioresults",
                    OracleDbType = OracleDbType.RefCursor,
                    Direction = ParameterDirection.Output
                };

                var executeResult = _objService.ExecuteReadStoredProc("AIR_ADMIN.PKG_AIROR904.LIST_PROFILE_CUSTOMER",
                    new
                    {
                        //in 
                        p_order_no = query.Order_No,
                        // Out
                        ioresults = ioResults

                    }).ToList();

                List<MinionGetListProfileCustomerQueryModel> minionGetListProfileCutomer = executeResult;
                return minionGetListProfileCutomer;

            }
            catch (Exception ex)
            {
                return new List<MinionGetListProfileCustomerQueryModel>
                {
                    new MinionGetListProfileCustomerQueryModel { ERROR_MSG = "Error call AIR_ADMIN.PKG_AIROR904.LIST_PROFILE_CUSTOMER " + ex.GetErrorMessage() }
                };
            }

        }
    }
}
