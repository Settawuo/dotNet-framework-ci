using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class RegisterBulkCorpQueryHandler : IQueryHandler<RegisterBulkCorpQuery, RegisterBulkCorpData>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<BulkAddress> _objService;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;

        public RegisterBulkCorpQueryHandler(ILogger logger, IWBBUnitOfWork uow, IEntityRepository<BulkAddress> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog)
        {
            _logger = logger;
            _uow = uow;
            _objService = objService;
            _intfLog = intfLog;

        }

        public RegisterBulkCorpData Handle(RegisterBulkCorpQuery query)
        {
            try
            {
                _logger.Info("RegisterBulkCorpQueryHandler Start");
                var output_bulk_number = new OracleParameter();
                output_bulk_number.ParameterName = "OUTPUT_bulk_number";
                output_bulk_number.OracleDbType = OracleDbType.Varchar2;
                output_bulk_number.Size = 2000;
                output_bulk_number.Direction = ParameterDirection.Output;

                var output_return_code = new OracleParameter();
                output_return_code.ParameterName = "OUTPUT_return_code";
                output_return_code.OracleDbType = OracleDbType.Decimal;
                output_return_code.Direction = ParameterDirection.Output;

                var output_return_message = new OracleParameter();
                output_return_message.ParameterName = "OUTPUT_return_message";
                output_return_message.OracleDbType = OracleDbType.Varchar2;
                output_return_message.Size = 2000;
                output_return_message.Direction = ParameterDirection.Output;

                var p_res_data = new OracleParameter();
                p_res_data.ParameterName = "p_res_data";
                p_res_data.OracleDbType = OracleDbType.RefCursor;
                p_res_data.Direction = ParameterDirection.Output;

                _logger.Info("StartPROC_INSERT_REGISTER");

                var executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_FBBBULK_CORP_REGISTER.PROC_INSERT_REGISTER",

                        new
                        {
                            p_user = query.p_user,
                            p_asc_code = query.p_asc_code,
                            p_employee_id = query.p_employee_id,
                            p_location_code = query.p_location_code,
                            p_id_card_no = query.p_id_card_no,
                            p_account_category = query.p_account_category,
                            p_account_sub_category = query.p_account_sub_category,
                            p_id_card_type = query.p_id_card_type,
                            p_account_title = query.p_account_title,
                            p_account_name = query.p_account_name,
                            p_ca_house_no = query.p_ca_house_no,
                            p_ca_moo = query.p_ca_moo,
                            p_ca_mooban = query.p_ca_mooban,
                            p_ca_building_name = query.p_ca_building_name,
                            p_ca_floor = query.p_ca_floor,
                            p_ca_room = query.p_ca_room,
                            p_ca_soi = query.p_ca_soi,
                            p_ca_street = query.p_ca_street,
                            p_ca_sub_district = query.p_ca_sub_district,
                            p_ca_district = query.p_ca_district,
                            p_ca_province = query.p_ca_province,
                            p_ca_postcode = query.p_ca_postcode,
                            p_ca_phone = query.p_ca_phone,
                            p_ca_main_mobile = query.p_ca_main_mobile,
                            p_ba_language = query.p_ba_language,
                            p_ba_bill_name = query.p_ba_bill_name,
                            p_ba_bill_cycle = query.p_ba_bill_cycle,
                            p_ba_house_no = query.p_ba_house_no,
                            p_ba_moo = query.p_ba_moo,
                            p_ba_mooban = query.p_ba_mooban,
                            p_ba_building_name = query.p_ba_building_name,
                            p_ba_floor = query.p_ba_floor,
                            p_ba_room = query.p_ba_room,
                            p_ba_soi = query.p_ba_soi,
                            p_ba_street = query.p_ba_street,
                            p_ba_sub_district = query.p_ba_sub_district,
                            p_ba_district = query.p_ba_district,
                            p_ba_province = query.p_ba_province,
                            p_ba_postcode = query.p_ba_postcode,
                            p_ba_phone = query.p_ba_phone,
                            p_ba_main_mobile = query.p_ba_main_mobile,

                            //  return code

                            output_bulk_number = output_bulk_number,
                            output_return_code = output_return_code,
                            output_return_message = output_return_message,
                            p_res_data = p_res_data

                        }).ToList();
                RegisterBulkCorpData ResultData = new RegisterBulkCorpData();
                ResultData.BulkAddList = executeResult;

                var ret_bulk_no = output_bulk_number.Value.ToSafeString();

                if (null == ret_bulk_no || "" == ret_bulk_no)
                {
                    return null;
                }
                var ret_code = output_return_code.Value.ToSafeString();
                if (ret_code == null || ret_code == "")
                {
                    return null;
                }
                var ret_msg = output_return_code.Value.ToSafeString();

                ResultData.output_bulk_number = ret_bulk_no;
                ResultData.output_return_code = ret_code;
                ResultData.output_return_message = ret_msg;

                if (ret_code == "0") // return 0 pass value to screen 
                {
                    _logger.Info("EndPROC_INSERT_REGISTER" + query.output_return_message);
                    return ResultData;

                }
                else //return -1 error
                {
                    _logger.Info("Error return -1 call service WBB.PKG_FBBBULK_CORP_REGISTER.PROC_INSERT_REGISTER output msg: " + output_return_message);
                    return null;

                }


            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);
                _logger.Info("Error call service WBB.PKG_FBBBULK_CORP_REGISTER.PROC_INSERT_REGISTER" + ex.Message);

                return null;
            }
        }
    }
}
