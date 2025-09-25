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
    public class MinionGetListProfileSplitterQueryHandler :
        IQueryHandler<MinionGetListProfileSplitterQuery, List<MinionGetListProfileSplitterQueryModel>>
    {
        private readonly IAirNetEntityRepository<MinionGetListProfileSplitterQueryModel> _objService;

        public MinionGetListProfileSplitterQueryHandler(
            IAirNetEntityRepository<MinionGetListProfileSplitterQueryModel> objService)
        {
            _objService = objService;
        }

        public List<MinionGetListProfileSplitterQueryModel> Handle(MinionGetListProfileSplitterQuery query)
        {
            try
            {
                var ioResults = new OracleParameter
                {
                    ParameterName = "ioresults",
                    OracleDbType = OracleDbType.RefCursor,
                    Direction = ParameterDirection.Output
                };

                var executeResult = _objService.ExecuteReadStoredProc("AIR_ADMIN.PKG_AIROR904.LIST_PROFILE_SPLITTER",
                    new
                    {
                        //in 
                        p_order_no = query.Order_No,
                        // Out
                        ioresults = ioResults

                    }).ToList();

                List<MinionGetListProfileSplitterQueryModel> minionGetListProfileCutomer = executeResult;
                return minionGetListProfileCutomer;

            }
            catch (Exception ex)
            {
                return new List<MinionGetListProfileSplitterQueryModel>
                {
                    new MinionGetListProfileSplitterQueryModel { ERROR_MSG = "Error call AIR_ADMIN.PKG_AIROR904.LIST_PROFILE_SPLITTER " + ex.GetErrorMessage() }
                };
            }

        }
    }
}
