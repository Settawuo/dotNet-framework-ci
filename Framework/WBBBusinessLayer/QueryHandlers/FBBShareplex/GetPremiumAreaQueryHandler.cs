using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.FBBShareplex;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.ShareplexModels;

namespace WBBBusinessLayer.QueryHandlers.FBBShareplex
{
    public class GetPremiumAreaQueryHandler : IQueryHandler<GetPremiumAreaQuery, PremiumAreaModel>
    {
        private readonly ILogger _logger;
        private readonly IFBBShareplexEntityRepository<object> _fbbShareplexRepository;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public GetPremiumAreaQueryHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow,
            IFBBShareplexEntityRepository<object> fbbShareplexRepository)
        {
            _logger = logger;
            _fbbShareplexRepository = fbbShareplexRepository;
            _intfLog = intfLog;
            _uow = uow;
        }

        public PremiumAreaModel Handle(GetPremiumAreaQuery query)
        {
            InterfaceLogCommand log = null;
            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, "" + query.ClientIP, "QUERY_PREMIUM_AREA", "GetPremiumAreaQuery", null, "FBB|" + query.FullUrl, "WEB");

            PremiumAreaModel executeResults = new PremiumAreaModel();

            var SubDistrict = new OracleParameter();
            SubDistrict.ParameterName = "SubDistrict";
            SubDistrict.Size = 2000;
            SubDistrict.OracleDbType = OracleDbType.Varchar2;
            SubDistrict.Direction = ParameterDirection.Input;
            SubDistrict.Value = query.SubDistrict;

            var District = new OracleParameter();
            District.ParameterName = "District";
            District.Size = 2000;
            District.OracleDbType = OracleDbType.Varchar2;
            District.Direction = ParameterDirection.Input;
            District.Value = query.District;

            var Province = new OracleParameter();
            Province.ParameterName = "Province";
            Province.Size = 2000;
            Province.OracleDbType = OracleDbType.Varchar2;
            Province.Direction = ParameterDirection.Input;
            Province.Value = query.Province;

            var PostalCode = new OracleParameter();
            PostalCode.ParameterName = "PostalCode";
            PostalCode.Size = 2000;
            PostalCode.OracleDbType = OracleDbType.Varchar2;
            PostalCode.Direction = ParameterDirection.Input;
            PostalCode.Value = query.PostalCode;

            var Language = new OracleParameter();
            Language.ParameterName = "Language";
            Language.Size = 2000;
            Language.OracleDbType = OracleDbType.Varchar2;
            Language.Direction = ParameterDirection.Input;
            Language.Value = query.Language;

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

            var ReturnPremiumConfig = new OracleParameter();
            ReturnPremiumConfig.ParameterName = "ReturnPremiumConfig";
            ReturnPremiumConfig.OracleDbType = OracleDbType.RefCursor;
            ReturnPremiumConfig.Direction = ParameterDirection.Output;

            try
            {
                var result = _fbbShareplexRepository.ExecuteStoredProcMultipleCursor("FBBADM.PKG_QUERY_PREMIUM_AREA.QUERY_PREMIUM_AREA",
                    new object[]
                    {
                        SubDistrict,
                        District,
                        Province,
                        PostalCode,
                        Language,

                        // return code
                        ReturnCode,
                        ReturnMessage,
                        ReturnPremiumConfig

                    });

                if (result != null)
                {
                    executeResults.ReturnCode = result[0] != null ? result[0].ToString() : "-1";
                    executeResults.ReturnMessage = result[1] != null ? result[1].ToString() : "";

                    var d_ReturnPremiumConfig = (DataTable)result[2];
                    var RETURNPREMIUMCONFIG = d_ReturnPremiumConfig.DataTableToList<PremiumConfigModel>();
                    executeResults.ReturnPremiumConfig = RETURNPREMIUMCONFIG;

                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "", "");

                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, query, log, "Failed", "Failed", "");
                    executeResults.ReturnCode = "-1";
                    executeResults.ReturnMessage = "Error";

                }

            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);
                _logger.Info("Error call service FBBADM.PKG_QUERY_PREMIUM_AREA.QUERY_PREMIUM_AREA" + ex.Message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, query, log, "Failed", ex.Message, "");
                executeResults.ReturnCode = "-1";
                executeResults.ReturnMessage = "Error";
            }


            return executeResults;
        }
    }
}
