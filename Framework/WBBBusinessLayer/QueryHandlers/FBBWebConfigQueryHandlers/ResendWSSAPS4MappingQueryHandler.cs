using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class ResendWSSAPS4MappingQueryHandler : IQueryHandler<mappingResendSsaps4Query, mappingResendWssapslist4Model>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<object> _objService;

        public ResendWSSAPS4MappingQueryHandler(ILogger logger, IEntityRepository<object> objService)
        {
            _logger = logger;
            _objService = objService;
        }
        public mappingResendWssapslist4Model Handle(mappingResendSsaps4Query query)
        {


            mappingResendWssapslist4Model executeResults = new mappingResendWssapslist4Model();


            try
            {

                var p_plant = new OracleParameter();
                p_plant.ParameterName = "p_plant";
                p_plant.OracleDbType = OracleDbType.Varchar2;
                p_plant.Size = 1000;
                p_plant.Direction = ParameterDirection.Input;
                p_plant.Value = query.p_plant.ToSafeString();



                var p_storage_location = new OracleParameter();
                p_storage_location.ParameterName = "p_storage_location";
                p_storage_location.OracleDbType = OracleDbType.Varchar2;
                p_storage_location.Size = 1000;
                p_storage_location.Direction = ParameterDirection.Input;
                p_storage_location.Value = query.p_storage_location.ToSafeString();

                var p_material_code = new OracleParameter();
                p_material_code.ParameterName = "p_material_code";
                p_material_code.OracleDbType = OracleDbType.Varchar2;
                p_material_code.Size = 1000;
                p_material_code.Direction = ParameterDirection.Input;
                p_material_code.Value = query.p_material_code.ToSafeString();

                var ret_code = new OracleParameter
                {
                    OracleDbType = OracleDbType.Decimal,
                    Direction = ParameterDirection.Output
                };
                var ret_msg = new OracleParameter
                {
                    OracleDbType = OracleDbType.Varchar2,
                    Direction = ParameterDirection.Output
                };

                
                var cur_mapping_material_code = new OracleParameter
                {
                    ParameterName = "cur_mapping_material_code",
                    OracleDbType = OracleDbType.RefCursor,
                    Direction = ParameterDirection.Output
                };

                var cur_mapping_plant = new OracleParameter
                {
                    ParameterName = "cur_mapping_plant",
                    OracleDbType = OracleDbType.RefCursor,
                    Direction = ParameterDirection.Output
                };
                var cur_mapping_storage_location = new OracleParameter
                {
                    ParameterName = "cur_mapping_storage_location",
                    OracleDbType = OracleDbType.RefCursor,
                    Direction = ParameterDirection.Output
                };

                _logger.Info("Start ResendFoa Query Handler Call  : PKG_FBBPAYG_FOA_RESEND_ORDER  ");
                // List<LosttranModel> executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_FBB_FOA_ORDER_MANAGEMENT.p_get_lostTran_log",

                var result =  _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBBPAYG_FOA_RESEND_ORDER.p_get_mapping_saps4",
                new object[]
                {
                    p_plant,
                    p_storage_location,
                    p_material_code,


                 
                   
                    cur_mapping_plant,
                    cur_mapping_storage_location,
                     cur_mapping_material_code,
                    ret_code ,
                    ret_msg ,

                });
                _logger.Info("End ResendFoa Query Handler Call  : ");

             


                DataTable mappinglistPlant = (DataTable)result[0];
                List<mappingResendWssaps4Model> p_list_cur = mappinglistPlant.DataTableToList<mappingResendWssaps4Model>();
                executeResults.mappinglistPlant = p_list_cur;

                DataTable mappinglistStorageLocation = (DataTable)result[1];
                List<mappingResendWssaps4Model> p_list_cur2 = mappinglistStorageLocation.DataTableToList<mappingResendWssaps4Model>();
                executeResults.mappinglistStorageLocation = p_list_cur2;


                DataTable mappinglistMaterialCode = (DataTable)result[2];
                List<mappingResendWssaps4Model> p_list_cur3 = mappinglistMaterialCode.DataTableToList<mappingResendWssaps4Model>();
                executeResults.mappinglistMaterialCode = p_list_cur3;

                executeResults.ret_code = result[3] != null ? result[3].ToString() : "-1";
                executeResults.ret_msg = result[4].ToString();


                _logger.Info("End  " + executeResults.ret_code);

                return executeResults;



            }
            catch (Exception ex)
            {

                _logger.Info("Error ResendFoaQueryHandler Call  : " + ex.Message);
                return null;
            }


        }
    }
}
