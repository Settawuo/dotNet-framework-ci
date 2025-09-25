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
    public class MinionGetListPackageBySffPromoQueryHandler : IQueryHandler<MinionGetListPackageBySffPromoQuery, MinionGetListPackageBySffPromoQueryModel>
    {
        private readonly IAirNetEntityRepository<PackageSffPromoModel> _objService;

        public MinionGetListPackageBySffPromoQueryHandler(IAirNetEntityRepository<PackageSffPromoModel> objService)
        {
            _objService = objService;
        }

        public MinionGetListPackageBySffPromoQueryModel Handle(MinionGetListPackageBySffPromoQuery query)
        {
            var minionGetListPackageBySffRomo = new MinionGetListPackageBySffPromoQueryModel();
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

                var executeResult = _objService.ExecuteReadStoredProc("AIR_ADMIN.PKG_AIROR905.LIST_PACKAGE_BY_SFFPROMO",
                   new
                   {
                       //in 
                       p_sff_promocode = query.P_SFF_PROMOCODE,
                       p_product_subtype = query.P_PRODUCT_SUBTYPE,
                       p_owner_product = query.P_OWNER_PRODUCT,
                       // Out
                       o_return_code = o_return_code,
                       ioresults = ioResults

                   }).ToList();

                minionGetListPackageBySffRomo.o_return_code = o_return_code.Value.ToSafeString() != "null" ? decimal.Parse(o_return_code.Value.ToSafeString()) : 0;
                minionGetListPackageBySffRomo.ListPackageSffPromoList = executeResult;
                return minionGetListPackageBySffRomo;
            }
            catch (Exception ex)
            {
                return new MinionGetListPackageBySffPromoQueryModel();
            }
        }
    }
}
