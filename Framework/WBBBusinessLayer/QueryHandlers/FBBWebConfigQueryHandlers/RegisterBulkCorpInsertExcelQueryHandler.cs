using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class RegisterBulkCorpInsertExcelQueryHandler : IQueryHandler<RegisterBulkCorpInsertExcelQuery, ReturnInsertExcelData>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<ReturnInsertExcelData> _objService;

        public RegisterBulkCorpInsertExcelQueryHandler(ILogger logger, IEntityRepository<ReturnInsertExcelData> objService)
        {
            _logger = logger;
            _objService = objService;
        }
        public ReturnInsertExcelData Handle(RegisterBulkCorpInsertExcelQuery query)
        {
            try
            {
                var output_bulk_no = new OracleParameter();
                output_bulk_no.ParameterName = "output_bulk_no";
                output_bulk_no.OracleDbType = OracleDbType.Varchar2;
                output_bulk_no.Size = 2000;
                output_bulk_no.Direction = ParameterDirection.Output;

                var ret_code = new OracleParameter();
                ret_code.ParameterName = "output_return_code";
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.ParameterName = "output_return_message";
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;



                var executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_FBBBULK_CORP_REGISTER.PROC_INSERT_EXCEL",
                            new
                            {
                                p_no = query.p_no,
                                p_installaddress1 = query.p_installaddress1,
                                p_installaddress2 = query.p_installaddress2,
                                p_installaddress3 = query.p_installaddress3,
                                p_installaddress4 = query.p_installaddress4,
                                p_installaddress5 = query.p_installaddress5,
                                p_latitude = query.p_latitude,
                                p_longitude = query.p_longitude,
                                p_install_date = query.p_install_date,
                                //p_dpname = query.p_dpname,
                                //p_installationcapacity = query.p_installationcapacity,
                                //p_ia = query.p_ia,
                                //p_password = query.p_password,

                                p_user = query.p_user,
                                p_file_name = query.p_file_name,
                                p_file_size = query.p_file_size,
                                p_total_row = query.p_total_row,
                                p_bulk_no = query.p_bulk_no,
                                p_technology_install = query.p_technology_install,
                                p_address_id = query.p_address_id,
                                //p_install_date = query.p_install_date,
                                p_event_code = query.p_event_code,
                                p_contact_first_name = query.p_contact_first_name,
                                p_contact_last_name = query.p_contact_last_name,
                                p_contact_phone = query.p_contact_phone,
                                p_contact_mobile = query.p_contact_mobile,
                                p_contact_email = query.p_contact_email,

                                pm_sff_promotion_code = query.pm_sff_promotion_code,
                                pm_package_class = query.pm_package_class,
                                pm_sff_promotion_bill_tha = query.pm_sff_promotion_bill_tha,
                                pm_sff_promotion_bill_eng = query.pm_sff_promotion_bill_eng,
                                pm_package_name_tha = query.pm_package_name_tha,
                                pm_recurring_charge = query.pm_recurring_charge,
                                pm_pre_initiation_charge = query.pm_pre_initiation_charge,
                                pm_initiation_charge = query.pm_initiation_charge,
                                pm_download_speed = query.pm_download_speed,
                                pm_upload_speed = query.pm_upload_speed,
                                pm_product_type = query.pm_product_type,
                                pm_owner_product = query.pm_owner_product,
                                pm_product_subtype = query.pm_product_subtype,
                                pm_product_subtype2 = query.pm_product_subtype2,
                                pm_technology = query.pm_technology,
                                pm_package_group = query.pm_package_group,
                                pm_package_code = query.pm_package_code,

                                pi_sff_promotion_code = query.pi_sff_promotion_code,
                                pi_package_class = query.pi_package_class,
                                pi_sff_promotion_bill_tha = query.pi_sff_promotion_bill_tha,
                                pi_sff_promotion_bill_eng = query.pi_sff_promotion_bill_eng,
                                pi_package_name_tha = query.pi_package_name_tha,
                                pi_recurring_charge = query.pi_recurring_charge,
                                pi_pre_initiation_charge = query.pi_pre_initiation_charge,
                                pi_initiation_charge = query.pi_initiation_charge,
                                pi_download_speed = query.pi_download_speed,
                                pi_upload_speed = query.pi_upload_speed,
                                pi_product_type = query.pi_product_type,
                                pi_owner_product = query.pi_owner_product,
                                pi_product_subtype = query.pi_product_subtype,
                                pi_product_subtype2 = query.pi_product_subtype2,
                                pi_technology = query.pi_technology,
                                pi_package_group = query.pi_package_group,
                                pi_package_code = query.pi_package_code,

                                pv_sff_promotion_code = query.pv_sff_promotion_code,
                                pv_package_class = query.pv_package_class,
                                pv_sff_promotion_bill_tha = query.pv_sff_promotion_bill_tha,
                                pv_sff_promotion_bill_eng = query.pv_sff_promotion_bill_eng,
                                pv_package_name_tha = query.pv_package_name_tha,
                                pv_recurring_charge = query.pv_recurring_charge,
                                pv_pre_initiation_charge = query.pv_pre_initiation_charge,
                                pv_initiation_charge = query.pv_initiation_charge,
                                pv_download_speed = query.pv_download_speed,
                                pv_upload_speed = query.pv_upload_speed,
                                pv_product_type = query.pv_product_type,
                                pv_owner_product = query.pv_owner_product,
                                pv_product_subtype = query.pv_product_subtype,
                                pv_product_subtype2 = query.pv_product_subtype2,
                                pv_technology = query.pv_technology,
                                pv_package_group = query.pv_package_group,
                                pv_package_code = query.pv_package_code,

                                s1_service_code = query.s1_service_code,
                                s1_product_name = query.s1_product_name,
                                s2_service_code = query.s2_service_code,
                                s2_product_name = query.s2_product_name,
                                s3_service_code = query.s3_service_code,//new
                                s3_product_name = query.s3_product_name,//new

                                pod_sff_promotion_code = query.pod_sff_promotion_code,
                                pod_package_class = query.pod_package_class,
                                pod_sff_promotion_bill_tha = query.pod_sff_promotion_bill_tha,
                                pod_sff_promotion_bill_eng = query.pod_sff_promotion_bill_eng,
                                pod_package_name_tha = query.pod_package_name_tha,
                                pod_recurring_charge = query.pod_recurring_charge,
                                pod_pre_initiation_charge = query.pod_pre_initiation_charge,
                                pod_initiation_charge = query.pod_initiation_charge,
                                pod_download_speed = query.pod_download_speed,
                                pod_upload_speed = query.pod_upload_speed,
                                pod_product_type = query.pod_product_type,
                                pod_owner_product = query.pod_owner_product,
                                pod_product_subtype = query.pod_product_subtype,
                                pod_product_subtype2 = query.pod_product_subtype2,
                                pod_technology = query.pod_technology,
                                pod_package_group = query.pod_package_group,
                                pod_package_code = query.pod_package_code,

                                //  return code
                                output_bulk_no = output_bulk_no,
                                ret_code = ret_code,
                                ret_msg = ret_msg


                            }).ToList();

                ReturnInsertExcelData ResultData = new ReturnInsertExcelData();

                var return_bulk_no = output_bulk_no.Value.ToString();
                var return_code = ret_code.Value.ToString();
                var return_msg = ret_msg.Value.ToString();

                if (null == return_bulk_no || "" == return_bulk_no)
                {
                    return null;
                }

                if (return_code == null || return_code == "")
                {
                    return null;
                }

                ResultData.output_bulk_no = return_bulk_no;
                ResultData.output_return_code = return_code;
                ResultData.output_return_message = return_msg;

                if (return_code == "0")
                {
                    _logger.Info("EndPROC_INSERT_EXCEL" + return_msg);
                    return ResultData;

                }
                else
                {
                    _logger.Info("Error return -1 call service WBB.PKG_FBBBULK_CORP_REGISTER.PROC_INSERT_EXCEL output msg: " + return_msg);
                    return null;
                }


            }
            catch (Exception ex)
            {
                _logger.Info("Error when WBB.PKG_FBBBULK_CORP_REGISTER.PROC_INSERT_EXCEL output msg: " + ex.InnerException);

                return null;
            }
        }
    }
}
