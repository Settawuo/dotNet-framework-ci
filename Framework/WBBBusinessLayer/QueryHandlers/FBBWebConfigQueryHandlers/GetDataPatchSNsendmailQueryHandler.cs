using Oracle.ManagedDataAccess.Client;
using System.Data;
using WBBContract;
using WBBContract.Queries.PatchEquipment;
using WBBData.Repository;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class GetDataPatchSNsendmailQueryHandler : IQueryHandler<GetDataPatchSNsendmailQuery, string>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<object> _entityRepository;

        public GetDataPatchSNsendmailQueryHandler(ILogger logger,
            IEntityRepository<object> entityRepository)
        {
            _logger = logger;
            //   _objService = objService;
            _entityRepository = entityRepository;
        }

        public string Handle(GetDataPatchSNsendmailQuery query)
        {
            var ret_code = new OracleParameter();
            ret_code.ParameterName = "ret_code";
            ret_code.OracleDbType = OracleDbType.Varchar2;
            ret_code.Size = 2000;
            ret_code.Direction = ParameterDirection.Output;

            var ret_msg = new OracleParameter();
            ret_msg.ParameterName = "ret_msg";
            ret_msg.OracleDbType = OracleDbType.Varchar2;
            ret_msg.Size = 2000;
            ret_msg.Direction = ParameterDirection.Output;

            var p_EMAIL = new OracleParameter();
            p_EMAIL.ParameterName = "p_EMAIL";
            p_EMAIL.Size = 2000;
            p_EMAIL.OracleDbType = OracleDbType.Varchar2;
            p_EMAIL.Direction = ParameterDirection.Output;

            var p_FILENAME = new OracleParameter();
            p_FILENAME.ParameterName = "p_FILENAME";
            p_FILENAME.Size = 2000;
            p_FILENAME.OracleDbType = OracleDbType.Varchar2;
            p_FILENAME.Direction = ParameterDirection.Input;
            p_FILENAME.Value = query.FileName;

            var executeResult = _entityRepository.ExecuteStoredProcExecuteReader("WBB.PKG_FBB_PAYG_PATCH_SN.p_search_patch_sn_sendmail",
                  new object[]
                   {
                       p_FILENAME,

					   //Return
					   p_EMAIL,
                       ret_code,
                       ret_msg
                   });

            return p_EMAIL.Value.ToString();

        }
    }
}
