using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Minions;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Minions;

namespace WBBBusinessLayer.Minions
{

    public class MinionGetListPackageByChangeQueryHandler : IQueryHandler<MinionGetListPackageByChangeQuery, MinionGetListPackageByChangeQueryModel>
    {
        private readonly IAirNetEntityRepository<PackageByChangeModel> _objService;

        public MinionGetListPackageByChangeQueryHandler(IAirNetEntityRepository<PackageByChangeModel> objService)
        {
            _objService = objService;
        }


        public MinionGetListPackageByChangeQueryModel Handle(MinionGetListPackageByChangeQuery query)
        {
            var minionGetListPackageByChange = new MinionGetListPackageByChangeQueryModel();
            try
            {
                var o_return_code = new OracleParameter
                {
                    OracleDbType = OracleDbType.Decimal,
                    ParameterName = "o_return_code",
                    Direction = ParameterDirection.Output
                };

                var ioResults = new OracleParameter
                {
                    ParameterName = "ioresults",
                    OracleDbType = OracleDbType.RefCursor,
                    Direction = ParameterDirection.Output
                };

                var executeResult = _objService.ExecuteReadStoredProc("AIR_ADMIN.PKG_AIROR905.LIST_PACKAGE_BY_CHANGE",
                   new
                   {
                       //in 
                       p_owner_product = query.owner_product,
                       p_package_for = query.package_for,
                       p_serenade_flag = query.serenade_flag,
                       p_ref_row_id = query.ref_row_id,
                       p_customer_type = query.customer_type,
                       p_address_id = query.address_id,
                       // Out
                       o_return_code = o_return_code,
                       ioresults = ioResults

                   }).ToList();

                minionGetListPackageByChange.o_return_code = o_return_code.Value.ToSafeString() != "null" ? decimal.Parse(o_return_code.Value.ToSafeString()) : 0;
                minionGetListPackageByChange.ListPackageChangeList = executeResult;
                return minionGetListPackageByChange;
            }
            catch (Exception ex)
            {
                return new MinionGetListPackageByChangeQueryModel();
            }
        }

    }
}
