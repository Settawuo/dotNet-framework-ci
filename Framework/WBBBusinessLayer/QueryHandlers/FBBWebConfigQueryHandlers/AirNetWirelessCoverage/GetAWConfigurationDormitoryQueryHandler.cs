using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.AirWirelessCoverage;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.AirNetWirelessCoverage
{
    public class GetAWConfigurationDormitoryQueryHandler : IQueryHandler<GetAWConfigurationDormitoryQuery, List<ConfigurationDormitoryData>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<ConfigurationDormitoryData> _ConfigurationDormitoryData;

        public GetAWConfigurationDormitoryQueryHandler(ILogger logger,
            IEntityRepository<ConfigurationDormitoryData> ConfigurationDormitoryData)
        {
            _logger = logger;
            _ConfigurationDormitoryData = ConfigurationDormitoryData;
        }

        public List<ConfigurationDormitoryData> Handle(GetAWConfigurationDormitoryQuery query)
        {
            try
            {

                var p_return_code = new OracleParameter();
                p_return_code.ParameterName = "p_return_code";
                p_return_code.Size = 2000;
                p_return_code.OracleDbType = OracleDbType.Varchar2;
                p_return_code.Direction = ParameterDirection.Output;

                var p_return_message = new OracleParameter();
                p_return_message.ParameterName = "p_return_message";
                p_return_message.Size = 2000;
                p_return_message.OracleDbType = OracleDbType.Varchar2;
                p_return_message.Direction = ParameterDirection.Output;

                var p_res_data = new OracleParameter();
                p_res_data.ParameterName = "p_res_data";
                p_res_data.OracleDbType = OracleDbType.RefCursor;
                p_res_data.Direction = ParameterDirection.Output;

                List<ConfigurationDormitoryData> executeResult = _ConfigurationDormitoryData.ExecuteReadStoredProc("WBB.PKG_FBBDORM_ADMIN001.PROC_SEARCH_DORMITORY",
                            new
                            {
                                p_region = query.Region,
                                p_province = query.DormitoryProvince,
                                p_dormitory_name = query.DormitoryName,

                                /// return //////
                                p_return_code = p_return_code,
                                p_return_message = p_return_message,

                                p_res_data = p_res_data

                            }).ToList();

                var Return_Code = p_return_code.Value != null ? p_return_code.Value : "-1";
                var Return_Message = p_return_message.Value != null ? p_return_message.Value : "error";

                return executeResult;

            }
            catch (Exception ex)
            {

                return null;
            }
        }
    }

    public class GetAWConfigurationDormitoryByIDQueryHandler : IQueryHandler<GetAWConfigurationDormitoryByIDQuery, ConfigurationDormitoryModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<ConfigurationDormitoryBuildingData> _ConfigurationDormitoryBuildingData;

        public GetAWConfigurationDormitoryByIDQueryHandler(ILogger logger,
            IEntityRepository<ConfigurationDormitoryBuildingData> ConfigurationDormitoryBuildingData)
        {
            _logger = logger;
            _ConfigurationDormitoryBuildingData = ConfigurationDormitoryBuildingData;
        }

        public ConfigurationDormitoryModel Handle(GetAWConfigurationDormitoryByIDQuery query)
        {
            try
            {

                var p_return_code = new OracleParameter();
                p_return_code.ParameterName = "p_return_code";
                p_return_code.Size = 2000;
                p_return_code.OracleDbType = OracleDbType.Varchar2;
                p_return_code.Direction = ParameterDirection.Output;

                var p_return_message = new OracleParameter();
                p_return_message.ParameterName = "p_return_message";
                p_return_message.Size = 2000;
                p_return_message.OracleDbType = OracleDbType.Varchar2;
                p_return_message.Direction = ParameterDirection.Output;

                // Detail

                var p_dormitory_name_th = new OracleParameter();
                p_dormitory_name_th.ParameterName = "p_dormitory_name_th";
                p_dormitory_name_th.Size = 2000;
                p_dormitory_name_th.OracleDbType = OracleDbType.Varchar2;
                p_dormitory_name_th.Direction = ParameterDirection.Output;

                var p_home_no_th = new OracleParameter();
                p_home_no_th.ParameterName = "p_home_no_th";
                p_home_no_th.Size = 2000;
                p_home_no_th.OracleDbType = OracleDbType.Varchar2;
                p_home_no_th.Direction = ParameterDirection.Output;

                var p_moo_th = new OracleParameter();
                p_moo_th.ParameterName = "p_moo_th";
                p_moo_th.Size = 2000;
                p_moo_th.OracleDbType = OracleDbType.Varchar2;
                p_moo_th.Direction = ParameterDirection.Output;

                var p_soi_th = new OracleParameter();
                p_soi_th.ParameterName = "p_soi_th";
                p_soi_th.Size = 2000;
                p_soi_th.OracleDbType = OracleDbType.Varchar2;
                p_soi_th.Direction = ParameterDirection.Output;

                var p_Street_th = new OracleParameter();
                p_Street_th.ParameterName = "p_Street_th";
                p_Street_th.Size = 2000;
                p_Street_th.OracleDbType = OracleDbType.Varchar2;
                p_Street_th.Direction = ParameterDirection.Output;

                var p_tumbol_th = new OracleParameter();
                p_tumbol_th.ParameterName = "p_tumbol_th";
                p_tumbol_th.Size = 2000;
                p_tumbol_th.OracleDbType = OracleDbType.Varchar2;
                p_tumbol_th.Direction = ParameterDirection.Output;

                var p_amphur_th = new OracleParameter();
                p_amphur_th.ParameterName = "p_amphur_th";
                p_amphur_th.Size = 2000;
                p_amphur_th.OracleDbType = OracleDbType.Varchar2;
                p_amphur_th.Direction = ParameterDirection.Output;

                var p_province_th = new OracleParameter();
                p_province_th.ParameterName = "p_province_th";
                p_province_th.Size = 2000;
                p_province_th.OracleDbType = OracleDbType.Varchar2;
                p_province_th.Direction = ParameterDirection.Output;

                var p_zipcode_th = new OracleParameter();
                p_zipcode_th.ParameterName = "p_zipcode_th";
                p_zipcode_th.Size = 2000;
                p_zipcode_th.OracleDbType = OracleDbType.Varchar2;
                p_zipcode_th.Direction = ParameterDirection.Output;

                // Detail ENG

                var p_dormitory_name_en = new OracleParameter();
                p_dormitory_name_en.ParameterName = "p_dormitory_name_en";
                p_dormitory_name_en.Size = 2000;
                p_dormitory_name_en.OracleDbType = OracleDbType.Varchar2;
                p_dormitory_name_en.Direction = ParameterDirection.Output;

                var p_home_no_en = new OracleParameter();
                p_home_no_en.ParameterName = "p_home_no_en";
                p_home_no_en.Size = 2000;
                p_home_no_en.OracleDbType = OracleDbType.Varchar2;
                p_home_no_en.Direction = ParameterDirection.Output;

                var p_moo_en = new OracleParameter();
                p_moo_en.ParameterName = "p_moo_en";
                p_moo_en.Size = 2000;
                p_moo_en.OracleDbType = OracleDbType.Varchar2;
                p_moo_en.Direction = ParameterDirection.Output;

                var p_soi_en = new OracleParameter();
                p_soi_en.ParameterName = "p_soi_en";
                p_soi_en.Size = 2000;
                p_soi_en.OracleDbType = OracleDbType.Varchar2;
                p_soi_en.Direction = ParameterDirection.Output;

                var p_Street_en = new OracleParameter();
                p_Street_en.ParameterName = "p_Street_en";
                p_Street_en.Size = 2000;
                p_Street_en.OracleDbType = OracleDbType.Varchar2;
                p_Street_en.Direction = ParameterDirection.Output;

                var p_tumbol_en = new OracleParameter();
                p_tumbol_en.ParameterName = "p_tumbol_en";
                p_tumbol_en.Size = 2000;
                p_tumbol_en.OracleDbType = OracleDbType.Varchar2;
                p_tumbol_en.Direction = ParameterDirection.Output;

                var p_amphur_en = new OracleParameter();
                p_amphur_en.ParameterName = "p_amphur_en";
                p_amphur_en.Size = 2000;
                p_amphur_en.OracleDbType = OracleDbType.Varchar2;
                p_amphur_en.Direction = ParameterDirection.Output;

                var p_province_en = new OracleParameter();
                p_province_en.ParameterName = "p_province_en";
                p_province_en.Size = 2000;
                p_province_en.OracleDbType = OracleDbType.Varchar2;
                p_province_en.Direction = ParameterDirection.Output;

                var p_zipcode_en = new OracleParameter();
                p_zipcode_en.ParameterName = "p_zipcode_en";
                p_zipcode_en.Size = 2000;
                p_zipcode_en.OracleDbType = OracleDbType.Varchar2;
                p_zipcode_en.Direction = ParameterDirection.Output;

                //--------
                var p_target_launch_dt = new OracleParameter();
                p_target_launch_dt.ParameterName = "p_target_launch_dt";
                p_target_launch_dt.Size = 2000;
                p_target_launch_dt.OracleDbType = OracleDbType.Varchar2;
                p_target_launch_dt.Direction = ParameterDirection.Output;
                var p_launch_dt = new OracleParameter();
                p_launch_dt.ParameterName = "p_launch_dt";
                p_launch_dt.Size = 2000;
                p_launch_dt.OracleDbType = OracleDbType.Varchar2;
                p_launch_dt.Direction = ParameterDirection.Output;
                var p_target_volumn = new OracleParameter();
                p_target_volumn.ParameterName = "p_target_volumn";
                p_target_volumn.Size = 2000;
                p_target_volumn.OracleDbType = OracleDbType.Varchar2;
                p_target_volumn.Direction = ParameterDirection.Output;
                var p_volumn = new OracleParameter();
                p_volumn.ParameterName = "p_volumn";
                p_volumn.Size = 2000;
                p_volumn.OracleDbType = OracleDbType.Varchar2;
                p_volumn.Direction = ParameterDirection.Output;
                var p_dorm_contract_name = new OracleParameter();
                p_dorm_contract_name.ParameterName = "p_dorm_contract_name";
                p_dorm_contract_name.Size = 2000;
                p_dorm_contract_name.OracleDbType = OracleDbType.Varchar2;
                p_dorm_contract_name.Direction = ParameterDirection.Output;
                var p_dorm_contract_email = new OracleParameter();
                p_dorm_contract_email.ParameterName = "p_dorm_contract_email";
                p_dorm_contract_email.Size = 2000;
                p_dorm_contract_email.OracleDbType = OracleDbType.Varchar2;
                p_dorm_contract_email.Direction = ParameterDirection.Output;
                var p_dorm_contract_phone = new OracleParameter();
                p_dorm_contract_phone.ParameterName = "p_dorm_contract_phone";
                p_dorm_contract_phone.Size = 2000;
                p_dorm_contract_phone.OracleDbType = OracleDbType.Varchar2;
                p_dorm_contract_phone.Direction = ParameterDirection.Output;

                //----

                // Curser

                var p_res_data = new OracleParameter();
                p_res_data.ParameterName = "p_res_data";
                p_res_data.OracleDbType = OracleDbType.RefCursor;
                p_res_data.Direction = ParameterDirection.Output;

                List<ConfigurationDormitoryBuildingData> executeResult = _ConfigurationDormitoryBuildingData.ExecuteReadStoredProc("WBB.PKG_FBBDORM_ADMIN001.PROC_EDIT_SEARCH_DORMITORY",
                            new
                            {
                                p_province = query.p_dormitory_id,

                                /// return //////
                                p_return_code = p_return_code,
                                p_return_message = p_return_message,

                                /// return detail ////

                                p_dormitory_name_th = p_dormitory_name_th,
                                p_home_no_th = p_home_no_th,
                                p_moo_th = p_moo_th,
                                p_soi_th = p_soi_th,
                                p_Street_th = p_Street_th,
                                p_tumbol_th = p_tumbol_th,
                                p_amphur_th = p_amphur_th,
                                p_province_th = p_province_th,
                                p_zipcode_th = p_zipcode_th,

                                /// return detail eng////

                                p_dormitory_name_en = p_dormitory_name_en,
                                p_home_no_en = p_home_no_en,
                                p_moo_en = p_moo_en,
                                p_soi_en = p_soi_en,
                                p_Street_en = p_Street_en,
                                p_tumbol_en = p_tumbol_en,
                                p_amphur_en = p_amphur_en,
                                p_province_en = p_province_en,
                                p_zipcode_en = p_zipcode_en,

                                p_target_launch_dt = p_target_launch_dt,
                                p_launch_dt = p_launch_dt,
                                p_target_volumn = p_target_volumn,
                                p_volumn = p_volumn,
                                p_dorm_contract_name = p_dorm_contract_name,
                                p_dorm_contract_email = p_dorm_contract_email,
                                p_dorm_contract_phone = p_dorm_contract_phone,

                                p_res_data = p_res_data

                            }).ToList();

                ConfigurationDormitoryModel executeResults = new ConfigurationDormitoryModel();
                executeResults.ConfigurationDormitoryBuildingDataList = executeResult;

                var Return_Code = p_return_code.Value != null ? p_return_code.Value : "-1";
                var Return_Message = p_return_message.Value != null ? p_return_message.Value : "error";
                executeResults.p_return_code = Return_Code.ToSafeString();
                executeResults.p_return_message = Return_Message.ToSafeString();
                // Detail TH
                var Dormitory_Name_TH = p_dormitory_name_th.Value != null ? p_dormitory_name_th.Value : "";
                var Home_No_TH = p_home_no_th.Value != null ? p_home_no_th.Value : "";
                var Moo_TH = p_moo_th.Value != null ? p_moo_th.Value : "";
                var Soi_TH = p_soi_th.Value != null ? p_soi_th.Value : "";
                var Street_TH = p_Street_th.Value != null ? p_Street_th.Value : "";
                var Tumbol_TH = p_tumbol_th.Value != null ? p_tumbol_th.Value : "";
                var Amphur_TH = p_amphur_th.Value != null ? p_amphur_th.Value : "";
                var Province_TH = p_province_th.Value != null ? p_province_th.Value : "";
                var Zipcode_TH = p_zipcode_th.Value != null ? p_zipcode_th.Value : "";
                executeResults.p_dormitory_name_th = Dormitory_Name_TH.ToSafeString();
                executeResults.p_home_no_th = Home_No_TH.ToSafeString();
                executeResults.p_moo_th = Moo_TH.ToSafeString();
                executeResults.p_soi_th = Soi_TH.ToSafeString();
                executeResults.p_Street_th = Street_TH.ToSafeString();
                executeResults.p_tumbol_th = Tumbol_TH.ToSafeString();
                executeResults.p_amphur_th = Amphur_TH.ToSafeString();
                executeResults.p_province_th = Province_TH.ToSafeString();
                executeResults.p_zipcode_th = Zipcode_TH.ToSafeString();
                // Detail ENG
                var Dormitory_Name_EN = p_dormitory_name_en.Value != null ? p_dormitory_name_en.Value : "";
                var Home_No_EN = p_home_no_en.Value != null ? p_home_no_en.Value : "";
                var Moo_EN = p_moo_en.Value != null ? p_moo_en.Value : "";
                var Soi_EN = p_soi_en.Value != null ? p_soi_en.Value : "";
                var Street_EN = p_Street_en.Value != null ? p_Street_en.Value : "";
                var Tumbol_EN = p_tumbol_en.Value != null ? p_tumbol_en.Value : "";
                var Amphur_EN = p_amphur_en.Value != null ? p_amphur_en.Value : "";
                var Province_EN = p_province_en.Value != null ? p_province_en.Value : "";
                var Zipcode_EN = p_zipcode_en.Value != null ? p_zipcode_en.Value : "";
                executeResults.p_dormitory_name_en = Dormitory_Name_EN.ToSafeString();
                executeResults.p_home_no_en = Home_No_EN.ToSafeString();
                executeResults.p_moo_en = Moo_EN.ToSafeString();
                executeResults.p_soi_en = Soi_EN.ToSafeString();
                executeResults.p_Street_en = Street_EN.ToSafeString();
                executeResults.p_tumbol_en = Tumbol_EN.ToSafeString();
                executeResults.p_amphur_en = Amphur_EN.ToSafeString();
                executeResults.p_province_en = Province_EN.ToSafeString();
                executeResults.p_zipcode_en = Zipcode_EN.ToSafeString();

                var Target_launch_dt = p_target_launch_dt.Value != null ? p_target_launch_dt.Value : "";
                var Launch_dt = p_launch_dt.Value != null ? p_launch_dt.Value : "";
                var Target_volumn = p_target_volumn.Value != null ? p_target_volumn.Value : "";
                var Volumn = p_volumn.Value != null ? p_volumn.Value : "";
                var Dorm_contract_name = p_dorm_contract_name.Value != null ? p_dorm_contract_name.Value : "";
                var Dorm_contract_email = p_dorm_contract_email.Value != null ? p_dorm_contract_email.Value : "";
                var Dorm_contract_phone = p_dorm_contract_phone.Value != null ? p_dorm_contract_phone.Value : "";


                executeResults.p_target_launch_dt = Target_launch_dt.ToSafeString();
                executeResults.p_launch_dt = Launch_dt.ToSafeString();
                executeResults.p_target_volumn = Target_volumn.ToSafeString();
                executeResults.p_volumn = Volumn.ToSafeString();
                executeResults.p_dorm_contract_name = Dorm_contract_name.ToSafeString();
                executeResults.p_dorm_contract_email = Dorm_contract_email.ToSafeString();
                executeResults.p_dorm_contract_phone = Dorm_contract_phone.ToSafeString();

                return executeResults;

            }
            catch (Exception ex)
            {

                return null;
            }
        }
    }
}
