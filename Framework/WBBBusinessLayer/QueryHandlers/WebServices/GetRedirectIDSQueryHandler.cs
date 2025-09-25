using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetRedirectIDSQueryHandler : IQueryHandler<GetRedirectIDSQuery, List<GetRedirectIDSQueryModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<object> _objService;

        public GetRedirectIDSQueryHandler(ILogger logger, IEntityRepository<FBB_INTERFACE_LOG> intfLog, IWBBUnitOfWork uow, IEntityRepository<object> objService)
        {
            _logger = logger;
            _intfLog = intfLog;
            _uow = uow;
            _objService = objService;
        }

        public List<GetRedirectIDSQueryModel> Handle(GetRedirectIDSQuery query)
        {
            var dataTb = GetRedirectIDS();
            var dataJsonStr = JsonConvert.SerializeObject(dataTb, Formatting.Indented);
            var response = JsonConvert.DeserializeObject<List<GetRedirectIDSQueryModel>>(dataJsonStr);
            return response;
        }

        private DataTable GetRedirectIDS()
        {
            var pChannel = OracleParam("p_channel", "OFFICER");
            var rCode = OracleResult("RETURN_CODE", 107);
            var rCurror = OracleResult("RETURN_IDS_URL_CURROR", 121);
            var rMessage = OracleResult("RETURN_MESSAGE", 126);
            rMessage.Size = 2000;

            var result = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBB_IDS.GET_REDIRECT_IDS",
             new object[] { pChannel, rCode, rMessage, rCurror });

            return (DataTable)result[2];
        }

        private OracleParameter OracleParam(string param, string value)
            => new OracleParameter { ParameterName = param, Value = value };

        private OracleParameter OracleResult(string param, int type)
            => new OracleParameter { ParameterName = param, OracleDbType = (OracleDbType)type, Direction = ParameterDirection.Output };

    }
}
