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
    public class MinionGetListPackageByServiceQueryHandler : IQueryHandler<MinionGetListPackageByServiceQuery, MinionGetListPackageByServiceQueryModel>
    {
        private readonly IAirNetEntityRepository<ListPackageModel> _objService;

        public MinionGetListPackageByServiceQueryHandler(IAirNetEntityRepository<ListPackageModel> objService)
        {
            _objService = objService;
        }

        public MinionGetListPackageByServiceQueryModel Handle(MinionGetListPackageByServiceQuery query)
        {
            var minionGetListPackageByService = new MinionGetListPackageByServiceQueryModel();
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

                var executeResult = _objService.ExecuteReadStoredProc("AIR_ADMIN.PKG_AIROR905.LIST_PACKAGE_BY_SERVICE",
                   new
                   {
                       //in 
                       p_owner_product = query.P_OWNER_PRODUCT,
                       p_product_subtype = query.P_PRODUCT_SUBTYPE,
                       p_network_type = query.P_NETWORK_TYPE,
                       p_service_day = query.P_SERVICE_DAY,
                       p_package_for = query.P_PACKAGE_FOR,
                       p_package_code = query.P_PACKAGE_CODE,
                       p_location_code = query.P_Location_Code,
                       p_asc_code = query.P_Asc_Code,
                       p_partner_type = query.P_Partner_Type,
                       p_partner_subtype = query.P_Partner_SubType,
                       p_region = query.P_Region,
                       p_province = query.P_Province,
                       p_district = query.P_District,
                       p_sub_district = query.P_Sub_District,
                       p_address_type = query.P_Address_Type,
                       p_building_name = query.P_Building_Name,
                       p_building_no = query.P_Building_No,
                       p_serenade_flag = query.P_Serenade_Flag,
                       p_customer_type = query.P_Customer_Type,
                       p_address_id = query.P_Address_Id,
                       p_plug_and_play_flag = query.P_Plug_And_Play_Flag,
                       p_rental_flag = query.P_rental_flag,
                       p_customer_subtype = query.P_customer_subtype,

                       // Out
                       o_return_code = o_return_code,
                       ioresults = ioResults

                   }).ToList();

                minionGetListPackageByService.o_return_code = o_return_code.Value.ToSafeString() != "null" ? decimal.Parse(o_return_code.Value.ToSafeString()) : 0;
                minionGetListPackageByService.ListPackageByServiceList = executeResult;
                return minionGetListPackageByService;
            }
            catch (Exception ex)
            {
                return new MinionGetListPackageByServiceQueryModel();
            }
        }
    }
}
