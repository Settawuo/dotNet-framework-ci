using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.Commons.Master;
using WBBData.Repository;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.Commons.Master
{
    public class GetEventDetailQueryHandler : IQueryHandler<GetEventDetailQuery, EventDetailModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;

        public GetEventDetailQueryHandler(ILogger logger, IEntityRepository<string> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public EventDetailModel Handle(GetEventDetailQuery query)
        {
            var result = new EventDetailModel();
            try
            {
                var InstallDate = "";
                if (query.DatePeriodEN == "")
                {
                    if (query.DatePeriodTH.Any())
                    {
                        string dd, MM;
                        int yyyy = 0;
                        dd = query.DatePeriodTH.Substring(0, 2);
                        MM = query.DatePeriodTH.Substring(3, 2);
                        yyyy = Convert.ToInt32(query.DatePeriodTH.Substring(6, 4));
                        int year = yyyy - 543;

                        InstallDate = dd + "/" + MM + "/" + year;
                    }
                }
                else
                {
                    InstallDate = query.DatePeriodEN;
                }

                var sub_location_id = new OracleParameter();
                sub_location_id.OracleDbType = OracleDbType.Varchar2;
                sub_location_id.Size = 2000;
                sub_location_id.Direction = ParameterDirection.Output;

                var sub_contract_name = new OracleParameter();
                sub_contract_name.OracleDbType = OracleDbType.Varchar2;
                sub_contract_name.Size = 2000;
                sub_contract_name.Direction = ParameterDirection.Output;

                var install_staff_id = new OracleParameter();
                install_staff_id.OracleDbType = OracleDbType.Varchar2;
                install_staff_id.Size = 2000;
                install_staff_id.Direction = ParameterDirection.Output;

                var install_staff_name = new OracleParameter();
                install_staff_name.OracleDbType = OracleDbType.Varchar2;
                install_staff_name.Size = 2000;
                install_staff_name.Direction = ParameterDirection.Output;

                var time_slot = new OracleParameter();
                time_slot.OracleDbType = OracleDbType.Varchar2;
                time_slot.Size = 2000;
                time_slot.Direction = ParameterDirection.Output;

                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Varchar2;
                ret_code.Size = 2000;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;


                var outp = new List<object>();
                var paramOut = outp.ToArray();
                _logger.Info("Packagelist_event_detail");

                var executePassword = _objService.ExecuteStoredProc("WBB.pkg_fbbor013.list_event_detail",
                out paramOut,
                  new
                  {
                      p_event_code = query.EventCode.ToString(),
                      p_install_date = InstallDate,
                      p_id_card_no = query.IDCardNo,

                      //  return code
                      o_sub_location_id = sub_location_id,
                      o_sub_contract_name = sub_contract_name,
                      o_install_staff_id = install_staff_id,
                      o_install_staff_name = install_staff_name,
                      o_time_slot = time_slot,
                      p_return_code = ret_code,
                      p_return_message = ret_msg

                  });
                string contract_name = sub_contract_name.Value.ToString();
                string timeSlote = time_slot.Value.ToString();

                if (contract_name == "null")//(result.TIME_SLOT == "")
                {
                    if (query.Technology == "VDSL")
                    {
                        result.SUB_CONTRACT_NAME = "";
                        result.SUB_LOCATION_ID = "";
                        result.INSTALL_STAFF_ID = "";
                        result.INSTALL_STAFF_NAME = "";
                        result.TIME_SLOT = "08:00-10:00";
                    }
                    else if (query.Technology == "FTTH")
                    {
                        result.SUB_CONTRACT_NAME = "";
                        result.SUB_LOCATION_ID = "";
                        result.INSTALL_STAFF_ID = "";
                        result.INSTALL_STAFF_NAME = "";
                        result.TIME_SLOT = "09:00-13:00";

                    }
                }
                else if (timeSlote == "null")
                {
                    if (query.Technology == "VDSL")
                    {
                        result.SUB_CONTRACT_NAME = "";
                        result.SUB_LOCATION_ID = "";
                        result.INSTALL_STAFF_ID = "";
                        result.INSTALL_STAFF_NAME = "";
                        result.TIME_SLOT = "08:00-10:00";
                    }
                    else if (query.Technology == "FTTH")
                    {
                        result.SUB_CONTRACT_NAME = "";
                        result.SUB_LOCATION_ID = "";
                        result.INSTALL_STAFF_ID = "";
                        result.INSTALL_STAFF_NAME = "";
                        result.TIME_SLOT = "09:00-13:00";
                    }
                }
                else
                {
                    result.SUB_CONTRACT_NAME = sub_contract_name.Value.ToString();
                    result.SUB_LOCATION_ID = sub_location_id.Value.ToString();
                    result.INSTALL_STAFF_ID = install_staff_id.Value.ToString();
                    result.INSTALL_STAFF_NAME = install_staff_name.Value.ToString();
                    result.TIME_SLOT = time_slot.Value.ToString();
                    result.RETURN_CODE = ret_code.Value.ToString();
                    result.RETURN_MSG = ret_msg.Value.ToString();
                }

                _logger.Info("EndPackagelist_event_detail" + result.RETURN_MSG);


            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message.ToString());
                result.RETURN_CODE = "-1";
                result.RETURN_MSG = "Error call event detail Handler: " + ex.Message.ToString();
            }

            return result;
        }

    }
}
