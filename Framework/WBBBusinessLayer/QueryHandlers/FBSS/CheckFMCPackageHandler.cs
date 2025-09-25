using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.FBSS;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.FBSSModels;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.FBSS
{
    public class CheckFMCPackageHandler : IQueryHandler<CheckFMCPackageDataQuery, CheckFMCPackageModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<ReturnConfigTimeslotModel> _objService;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;

        public CheckFMCPackageHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<ReturnConfigTimeslotModel> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog)
        {
            _logger = logger;
            _uow = uow;
            _objService = objService;
            _intfLog = intfLog;
        }

        public CheckFMCPackageModel Handle(CheckFMCPackageDataQuery query)
        {
            InterfaceLogCommand log = null;
            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TransactionID, "CheckFMCPackage", "CheckFMCPackageHandler", null, "FBB", "");

            var ReturnCode = new OracleParameter();
            ReturnCode.ParameterName = "ReturnCode";
            ReturnCode.OracleDbType = OracleDbType.Varchar2;
            ReturnCode.Size = 2000;
            ReturnCode.Direction = ParameterDirection.Output;

            var ReturnMessage = new OracleParameter();
            ReturnMessage.ParameterName = "ReturnMessage";
            ReturnMessage.OracleDbType = OracleDbType.Varchar2;
            ReturnMessage.Size = 2000;
            ReturnMessage.Direction = ParameterDirection.Output;

            var ProjectName = new OracleParameter();
            ProjectName.ParameterName = "ProjectName";
            ProjectName.OracleDbType = OracleDbType.Varchar2;
            ProjectName.Size = 2000;
            ProjectName.Direction = ParameterDirection.Output;

            var OntopPackage = new OracleParameter();
            OntopPackage.ParameterName = "OntopPackage";
            OntopPackage.OracleDbType = OracleDbType.Varchar2;
            OntopPackage.Size = 2000;
            OntopPackage.Direction = ParameterDirection.Output;



            CheckFMCPackageModel Result = new CheckFMCPackageModel();

            try
            {
                var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FBBOR004.PROC_CHECK_FMC_PACKAGE",
                     new
                     {
                         p_mobile_price_excl_vat = query.p_mobile_price_excl_vat.ToSafeString(),
                         p_project_name = query.p_project_name.ToSafeString(),
                         p_sff_promotion_code = query.p_sff_promotion_code.ToSafeString(),
                         // out
                         ReturnCode = ReturnCode,
                         ReturnMessage = ReturnMessage,
                         ProjectName = ProjectName,
                         OntopPackage = OntopPackage
                     });

                Result.ReturnCode = (ReturnCode.Value == null || ReturnCode.Value.ToSafeString() == "null") ? "" : ReturnCode.Value.ToSafeString();
                Result.ReturnMessage = (ReturnMessage.Value == null || ReturnMessage.Value.ToSafeString() == "null") ? "" : ReturnMessage.Value.ToSafeString();
                Result.ProjectName = (ProjectName.Value == null || ProjectName.Value.ToSafeString() == "null") ? "" : ProjectName.Value.ToSafeString();
                Result.OntopPackage = (OntopPackage.Value == null || OntopPackage.Value.ToSafeString() == "null") ? "" : OntopPackage.Value.ToSafeString();
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, Result, log, "Success", "", "");

            }
            catch (Exception ex)
            {
                Result.ReturnCode = "-1";
                Result.ReturnMessage = ex.Message;
                Result.ProjectName = "";
                Result.OntopPackage = "";
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", "inner" + ex.InnerException + " Message:" + ex.Message, "");
            }

            return Result;
        }

    }
}
