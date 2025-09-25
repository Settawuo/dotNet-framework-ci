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
    public class ExistingRegisterBulkCorpQueryHandler : IQueryHandler<ExistingRegisterBulkCorpQuery, returnExistRegister>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<returnExistRegister> _objService;

        public ExistingRegisterBulkCorpQueryHandler(ILogger logger, IEntityRepository<returnExistRegister> objService)
        {
            _logger = logger;
            _objService = objService;
        }
        public returnExistRegister Handle(ExistingRegisterBulkCorpQuery query)
        {
            try
            {
                var output_bulk_no = new OracleParameter();
                output_bulk_no.ParameterName = "output_bulk_no";
                output_bulk_no.OracleDbType = OracleDbType.Varchar2;
                output_bulk_no.Size = 2000;
                output_bulk_no.Direction = ParameterDirection.Output;

                var output_return_code = new OracleParameter();
                output_return_code.ParameterName = "output_return_code";
                output_return_code.OracleDbType = OracleDbType.Varchar2;
                output_return_code.Size = 2000;
                output_return_code.Direction = ParameterDirection.Output;

                var output_return_message = new OracleParameter();
                output_return_message.ParameterName = "output_return_message";
                output_return_message.OracleDbType = OracleDbType.Varchar2;
                output_return_message.Size = 2000;
                output_return_message.Direction = ParameterDirection.Output;


                var executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_FBBBULK_CORP_REGISTER.PROC_EXISTING_REGISTER",
                            new
                            {
                                p_user = query.p_user,
                                p_bulk_number = query.p_bulk_number,
                                p_errormessage = query.p_errormessage,
                                p_total = query.p_total,
                                p_rownum = query.p_rownum,
                                p_rowid = query.p_rowid,
                                p_accntno = query.p_accntno,
                                p_accntclass = query.p_accntclass,
                                p_name = query.p_name,
                                p_idcardnum = query.p_idcardnum,
                                p_idcardtype = query.p_idcardtype,
                                p_contactbirthdt = query.p_contactbirthdt,
                                p_statuscd = query.p_statuscd,
                                p_accntcategory = query.p_accntcategory,
                                p_accntsubcategory = query.p_accntsubcategory,
                                p_mainphone = query.p_mainphone,
                                p_mainmobile = query.p_mainmobile,
                                p_legalflg = query.p_legalflg,
                                p_houseno = query.p_houseno,
                                p_buildingname = query.p_buildingname,
                                p_floor = query.p_floor,
                                p_room = query.p_room,
                                p_moo = query.p_moo,
                                p_mooban = query.p_mooban,
                                p_streetname = query.p_streetname,
                                p_soi = query.p_soi,
                                p_zipcode = query.p_zipcode,
                                p_tumbol = query.p_tumbol,
                                p_amphur = query.p_amphur,
                                p_provincename = query.p_provincename,
                                p_country = query.p_country,
                                p_vatname = query.p_vatname,
                                p_vatrate = query.p_vatrate,
                                p_vataddress1 = query.p_vataddress1,
                                p_vataddress2 = query.p_vataddress2,
                                p_vataddress3 = query.p_vataddress3,
                                p_vataddress4 = query.p_vataddress4,
                                p_vataddress5 = query.p_vataddress5,
                                p_vatpostalcd = query.p_vatpostalcd,
                                p_accounttitle = query.p_accounttitle,

                                //  return code
                                output_bulk_no = output_bulk_no,
                                output_return_code = output_return_code,
                                output_return_message = output_return_message

                            }).ToList();
                returnExistRegister ResultData = new returnExistRegister();

                var ret_bulk_no = output_bulk_no.Value.ToString();

                if (null == ret_bulk_no || "" == ret_bulk_no)
                {
                    return null;
                }
                var ret_code = output_return_code.Value.ToString();
                if (ret_code == null || ret_code == "")
                {
                    return null;
                }
                var ret_msg = output_return_message.Value.ToString();

                ResultData.output_bulk_no = ret_bulk_no;
                ResultData.output_return_code = Int32.Parse(ret_code);
                ResultData.output_return_message = ret_msg;

                if (ret_code == "0") // return 0 pass value to screen 
                {
                    _logger.Info("EndPROC_EXISTING_REGISTER " + output_return_message);
                    return ResultData;

                }
                else //return -1 error
                {
                    _logger.Info("Error return -1 call service WBB.PKG_FBBBULK_CORP_REGISTER.PROC_EXISTING_REGISTER output msg: " + output_return_message);
                    return null;

                }


            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);
                _logger.Info("Error call service WBB.PKG_FBBBULK_CORP_REGISTER.PROC_EXISTING_REGISTER" + ex.Message);

                return null;
            }
        }

    }
}
