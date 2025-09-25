using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetDetailMemberGerMemberQueryHandler : IQueryHandler<GetDetailMemberGerMemberQuery, DetailMemberGetMember>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<DetailMemberGetMember> _objService;

        public GetDetailMemberGerMemberQueryHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow,
            IEntityRepository<DetailMemberGetMember> objService)
        {
            _logger = logger;
            _intfLog = intfLog;
            _uow = uow;
            _objService = objService;
        }

        public DetailMemberGetMember Handle(GetDetailMemberGerMemberQuery query)
        {
            //R22.06.14062022
            InterfaceLogCommand log = null;

            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.p_internet_no, "GetDetailMemberGerMemberQuery", "GetDetailMemberGerMemberQueryHandler", null, "FBB", "");

                var p_internet_no = new OracleParameter();
                p_internet_no.ParameterName = "p_internet_no";
                p_internet_no.OracleDbType = OracleDbType.Varchar2;
                p_internet_no.Size = 2000;
                p_internet_no.Direction = ParameterDirection.Input;
                p_internet_no.Value = query.p_internet_no;

                var p_values1 = new OracleParameter();
                p_values1.ParameterName = "p_values1";
                p_values1.OracleDbType = OracleDbType.Varchar2;
                p_values1.Size = 2000;
                p_values1.Direction = ParameterDirection.Input;
                p_values1.Value = query.p_values1;

                var p_ContactListInfo = new OracleParameter();
                p_ContactListInfo.ParameterName = "p_ContactListInfo";
                p_ContactListInfo.OracleDbType = OracleDbType.Varchar2;
                p_ContactListInfo.Size = 2000;
                p_ContactListInfo.Direction = ParameterDirection.Input;
                p_ContactListInfo.Value = query.p_ContactListInfo;

                var p_language = new OracleParameter();
                p_language.ParameterName = "p_language";
                p_language.OracleDbType = OracleDbType.Varchar2;
                p_language.Size = 2000;
                p_language.Direction = ParameterDirection.Input;
                p_language.Value = query.p_language;

                var return_code = new OracleParameter();
                return_code.ParameterName = "return_code";
                return_code.OracleDbType = OracleDbType.Decimal;
                return_code.Direction = ParameterDirection.Output;

                var return_message = new OracleParameter();
                return_message.ParameterName = "return_message";
                return_message.OracleDbType = OracleDbType.Varchar2;
                return_message.Size = 2000;
                return_message.Direction = ParameterDirection.Output;

                var p_detail_mgm = new OracleParameter();
                p_detail_mgm.ParameterName = "p_detail_mgm";
                p_detail_mgm.OracleDbType = OracleDbType.RefCursor;
                p_detail_mgm.Direction = ParameterDirection.Output;

                var resultExecute = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBBOR021.PROC_DETAIL_MGM",
                   new object[]
                   {
                       //in 
                       p_internet_no,
                       p_values1,
                       p_ContactListInfo,
                       p_language,

                       /// Out
                       return_code,
                       return_message,
                       p_detail_mgm
                   });

                DetailMemberGetMember dataDetailMemberGetMember = new DetailMemberGetMember();

                if (resultExecute != null)
                {
                    DataTable dtSendSmsMGMRespones = (DataTable)resultExecute[2];
                    List<DetailMemberGetMember> sendSmsMGMlList = dtSendSmsMGMRespones.DataTableToList<DetailMemberGetMember>();
                    dataDetailMemberGetMember = sendSmsMGMlList.FirstOrDefault();
                }

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, dataDetailMemberGetMember, log, "Success", "", "");
                return dataDetailMemberGetMember;
            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Error", ex.GetErrorMessage(), "");
                return new DetailMemberGetMember();
            }
        }
    }
}
