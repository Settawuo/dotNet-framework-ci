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
    public class MinionGetListProfileAddressQueryHandler :
        IQueryHandler<MinionGetListProfileAddressQuery, List<MinionGetListProfileAddressQueryModel>>
    {
        private readonly IAirNetEntityRepository<MinionGetListProfileAddressQueryModel> _objService;

        public MinionGetListProfileAddressQueryHandler(
            IAirNetEntityRepository<MinionGetListProfileAddressQueryModel> objService)
        {
            _objService = objService;
        }

        public List<MinionGetListProfileAddressQueryModel> Handle(MinionGetListProfileAddressQuery query)
        {
            try
            {
                var ioResults = new OracleParameter
                {
                    ParameterName = "ioresults",
                    OracleDbType = OracleDbType.RefCursor,
                    Direction = ParameterDirection.Output
                };

                var executeResult = _objService.ExecuteReadStoredProc("AIR_ADMIN.PKG_AIROR904.LIST_PROFILE_ADDRESS",
                    new
                    {
                        //in 
                        p_order_no = query.Order_No,
                        // Out
                        ioresults = ioResults

                    }).ToList();

                List<MinionGetListProfileAddressQueryModel> minionGetListProfileAddress = executeResult;
                return minionGetListProfileAddress;

            }
            catch (Exception ex)
            {
                return new List<MinionGetListProfileAddressQueryModel>
                {
                    new MinionGetListProfileAddressQueryModel { ERROR_MSG = "Error call AIR_ADMIN.PKG_AIROR904.LIST_PROFILE_ADDRESS " + ex.GetErrorMessage() }
                };
            }

        }
    }
}