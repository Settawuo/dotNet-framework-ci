using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;
namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class GetDupFileDormQueryHandlers : IQueryHandler<GetDupFileDorm, Dupfile>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_MESSAGE_LOG> _messagelog;
        private readonly IEntityRepository<Dupfile> _Dupfile;

        public GetDupFileDormQueryHandlers(ILogger logger, IEntityRepository<FBB_MESSAGE_LOG> messagelog, IEntityRepository<Dupfile> Dupfile)
        {
            _logger = logger;
            _messagelog = messagelog;
            _Dupfile = Dupfile;
        }
        public Dupfile Handle(GetDupFileDorm query)
        {
            Dupfile result = new Dupfile();
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


                var executeResult = _Dupfile.ExecuteStoredProc("WBB.PKG_FBBDORM_ADMIN001.PROC_CHECK_DUP_FILE",
                  new
                  {
                      p_file_name = query.file_name.ToSafeString(),
                      p_user_import = query.user.ToSafeString(),

                      /// return //////
                      p_return_code = p_return_code,
                      p_return_message = p_return_message

                  });
                query.results = p_return_code.Value != null ? Convert.ToInt32(p_return_code.Value.ToSafeString()) : -1;
                result.result = query.results;
                return result;

            }
            catch (Exception ex)
            {
                result.result = -1;
                return result;
            }

        }
    }
}