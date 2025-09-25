using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBBusinessLayer.FBSSOrderServices;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.FBBWebConfigQueries.AirWirelessCoverage;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.AirNetWirelessCoverage
{
    public class GetAWConfigurationEventQueryHandler : IQueryHandler<GetAWConfigurationEventQuery, List<ConfigurationEventData>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<ConfigurationEventData> _ConfigurationEventData;

        public GetAWConfigurationEventQueryHandler(ILogger logger,
            IEntityRepository<ConfigurationEventData> ConfigurationEventData)
        {
            _logger = logger;
            _ConfigurationEventData = ConfigurationEventData;
        }

        public List<ConfigurationEventData> Handle(GetAWConfigurationEventQuery query)
        {
            try
            {

                var o_return_code = new OracleParameter();
                o_return_code.ParameterName = "o_return_code";
                o_return_code.OracleDbType = OracleDbType.Decimal;
                o_return_code.Direction = ParameterDirection.Output;

                var ioResults = new OracleParameter();
                ioResults.ParameterName = "ioResults";
                ioResults.OracleDbType = OracleDbType.RefCursor;
                ioResults.Direction = ParameterDirection.Output;

                List<ConfigurationEventData> executeResult = _ConfigurationEventData.ExecuteReadStoredProc("WBB.PKG_FBBOR013.LIST_EVENT_EDIT",
                            new
                            {
                                p_event_code = query.EventCode,

                                /// return //////
                                o_return_code = o_return_code,

                                ioResults = ioResults

                            }).ToList();

                var Return_Code = o_return_code.Value != null ? Convert.ToInt32(o_return_code.Value.ToSafeString()) : -1;

                return executeResult;

            }
            catch (Exception ex)
            {

                return null;
            }
        }
    }

    public class GetAWConfigurationEventSearchQueryHandler : IQueryHandler<GetAWConfigurationEventSearchQuery, List<ConfigurationEventSearchData>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<ConfigurationEventSearchData> _ConfigurationEventSearchData;

        public GetAWConfigurationEventSearchQueryHandler(ILogger logger,
            IEntityRepository<ConfigurationEventSearchData> ConfigurationEventSearchData)
        {
            _logger = logger;
            _ConfigurationEventSearchData = ConfigurationEventSearchData;
        }

        public List<ConfigurationEventSearchData> Handle(GetAWConfigurationEventSearchQuery query)
        {
            try
            {

                var o_return_code = new OracleParameter();
                o_return_code.ParameterName = "o_return_code";
                o_return_code.OracleDbType = OracleDbType.Decimal;
                o_return_code.Direction = ParameterDirection.Output;

                var ioResults = new OracleParameter();
                ioResults.ParameterName = "ioResults";
                ioResults.OracleDbType = OracleDbType.RefCursor;
                ioResults.Direction = ParameterDirection.Output;

                List<ConfigurationEventSearchData> executeResult = _ConfigurationEventSearchData.ExecuteReadStoredProc("WBB.PKG_FBBOR013.LIST_EVENT_SEARCH",
                            new
                            {
                                p_event_code = query.EventCode,

                                /// return //////
                                o_return_code = o_return_code,

                                ioResults = ioResults

                            }).ToList();

                var Return_Code = o_return_code.Value != null ? Convert.ToInt32(o_return_code.Value.ToSafeString()) : -1;

                return executeResult;

            }
            catch (Exception ex)
            {

                return null;
            }
        }
    }

    public class GetCapabilityQueryHandler : IQueryHandler<GetCapabilityQuery, List<CapabilityData>>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG_ADMIN> _interfaceLog;

        public GetCapabilityQueryHandler(ILogger logger, IWBBUnitOfWork uow, IEntityRepository<FBB_INTERFACE_LOG_ADMIN> interfaceLog)
        {
            _logger = logger;
            _uow = uow;
            _interfaceLog = interfaceLog;
        }

        public List<CapabilityData> Handle(GetCapabilityQuery query)
        {
            InterfaceLogAdminCommand log = null;
            DateTime Curr_DateTime = DateTime.Now;
            log = InterfaceLogAdminServiceHelper.StartInterfaceAdminLog(_uow, _interfaceLog, query, Curr_DateTime.ToString("yyyyMMddHHmmss"), "GetCapabilityQueryHandler", "GetCapabilityQuery", "");
            List<CapabilityData> executeResult = new List<CapabilityData>();
            try
            {
                subcontract[] Capabilitys;
                using (var service = new OrderService())
                {
                    Capabilitys = service.queryCapability(query.Technology, query.Lang_Flag, query.Post_Code, query.Sub_District, query.Event_Start_Date, query.Event_End_Date);
                }
                if (Capabilitys != null)
                {
                    foreach (var Capability in Capabilitys)
                    {
                        List<CapabilityTeamData> capabilityTeamList = new List<CapabilityTeamData>();
                        CapabilityData capabilityData = new CapabilityData();
                        capabilityData.subcontract_Company_NameField = Capability.Subcontract_Company_Name;
                        capabilityData.subcontract_Location_CodeField = Capability.Subcontract_Location_Code;
                        Team[] CapabilityTeams = Capability.Team;
                        capabilityData.CapabilityTeamList = new List<CapabilityTeamData>();
                        foreach (var CapabilityTeam in CapabilityTeams)
                        {
                            List<CapabilityStaffData> capabilityStaffList = new List<CapabilityStaffData>();
                            CapabilityTeamData capabilityTeam = new CapabilityTeamData();
                            capabilityTeam.subcontract_Team_IdField = CapabilityTeam.Subcontract_Team_Id;
                            capabilityTeam.subcontract_Team_NameField = CapabilityTeam.Subcontract_Team_Name;
                            Staff[] CapabilityStaffs = CapabilityTeam.Staff;
                            capabilityTeam.staffField = new List<CapabilityStaffData>();
                            foreach (var CapabilityStaff in CapabilityStaffs)
                            {
                                CapabilityStaffData capabilityStaff = new CapabilityStaffData();
                                capabilityStaff.staff_CodeField = CapabilityStaff.Staff_Code;
                                capabilityStaff.staff_NameField = CapabilityStaff.Staff_Name;
                                capabilityTeam.staffField.Add(capabilityStaff);
                            }
                            capabilityData.CapabilityTeamList.Add(capabilityTeam);
                        }
                        executeResult.Add(capabilityData);
                    }
                }
                InterfaceLogAdminServiceHelper.EndInterfaceAdminLog(_uow, _interfaceLog, Capabilitys, log, "Success", "");
                return executeResult;
            }
            catch (Exception ex)
            {
                InterfaceLogAdminServiceHelper.EndInterfaceAdminLog(_uow, _interfaceLog, "", log, "Failed", ex.Message.ToString());
                return executeResult;
            }
        }
    }

    public class ValidateEngineerQueryHandler : IQueryHandler<ValidateEngineerQuery, List<ValidateStaff>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<ValidateStaff> _ValidateEngineer;

        public ValidateEngineerQueryHandler(ILogger logger,
            IEntityRepository<ValidateStaff> ValidateEngineer)
        {
            _logger = logger;
            _ValidateEngineer = ValidateEngineer;
        }

        public List<ValidateStaff> Handle(ValidateEngineerQuery query)
        {
            try
            {

                var ioresults = new OracleParameter();
                ioresults.ParameterName = "ioresults";
                ioresults.OracleDbType = OracleDbType.RefCursor;
                ioresults.Direction = ParameterDirection.Output;

                List<ValidateStaff> executeResult = _ValidateEngineer.ExecuteReadStoredProc("WBB.PKG_FBBOR013.validate_engineer",
                            new
                            {
                                p_install_staff_id = query.Install_Staff_Id,
                                p_install_staff_name = query.Install_Staff_Name,
                                p_event_start_date = query.Event_Start_Date,
                                p_event_end_date = query.Event_End_Date,

                                /// return //////

                                ioresults = ioresults

                            }).ToList();

                return executeResult;


            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

    }

    public class ReserveCapabilityQueryHandler : IQueryHandler<ReserveCapabilityQuery, string>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG_ADMIN> _interfaceLog;

        public ReserveCapabilityQueryHandler(ILogger logger, IWBBUnitOfWork uow, IEntityRepository<FBB_INTERFACE_LOG_ADMIN> interfaceLog)
        {
            _logger = logger;
            _uow = uow;
            _interfaceLog = interfaceLog;
        }

        public string Handle(ReserveCapabilityQuery query)
        {
            InterfaceLogAdminCommand log = null;
            DateTime Curr_DateTime = DateTime.Now;
            log = InterfaceLogAdminServiceHelper.StartInterfaceAdminLog(_uow, _interfaceLog, query, Curr_DateTime.ToString("yyyyMMddHHmmss"), "ReserveCapabilityQueryHandler", "ReserveCapabilityQuery", query.EventCode);
            try
            {
                string executeResult = "";
                string Error = "";
                reserveSubcontract ReserveSubcontract = new reserveSubcontract();
                ReserveSubcontract.Subcontract_Location_Code = query.ReserveSubcontract.Subcontract_Location_Code;
                ReserveSubcontract.Subcontract_Team_Id = query.ReserveSubcontract.Subcontract_Team_Id;
                ReserveSubcontract.Event_Date_From = query.ReserveSubcontract.Event_Date_From;
                ReserveSubcontract.Event_Date_To = query.ReserveSubcontract.Event_Date_To;
                ReserveSubcontract.Capacity_Amount = query.ReserveSubcontract.Capacity_Amount;
                ReserveSubcontract.POST_CODE = query.ReserveSubcontract.Post_Code;
                ReserveSubcontract.SUB_DISSTRICT = query.ReserveSubcontract.Sub_District;
                using (var service = new OrderService())
                {
                    executeResult = service.reserveCapability(query.Service_Option, ReserveSubcontract, out Error);
                }

                InterfaceLogAdminServiceHelper.EndInterfaceAdminLog(_uow, _interfaceLog, query, log, "Success", "");
                return executeResult;
            }
            catch (Exception ex)
            {
                InterfaceLogAdminServiceHelper.EndInterfaceAdminLog(_uow, _interfaceLog, query, log, "Failed", ex.Message.ToString());
                return "-1";
            }
        }
    }
}
