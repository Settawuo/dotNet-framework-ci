using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetPathPdfQueryHeandle : IQueryHandler<GetPathPdfQuery, GetPathPdfModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<GetPathPdfModel> _pathPdf;


        public GetPathPdfQueryHeandle(ILogger logger, IEntityRepository<GetPathPdfModel> pathPdf)
        {
            _logger = logger;
            _pathPdf = pathPdf;

        }


        public GetPathPdfModel Handle(GetPathPdfQuery query)
        {
            try
            {

                var retCode = new OracleParameter
                {
                    OracleDbType = OracleDbType.Varchar2,
                    Size = 2000,
                    Direction = ParameterDirection.Output
                };

                var retMessage = new OracleParameter
                {
                    OracleDbType = OracleDbType.Varchar2,
                    Size = 2000,
                    Direction = ParameterDirection.Output
                };

                var retPathPdf = new OracleParameter
                {
                    OracleDbType = OracleDbType.Varchar2,
                    Size = 2000,
                    Direction = ParameterDirection.Output
                };

                object[] paramOut;
                _pathPdf.ExecuteStoredProc("WBB.PKG_FBBOR005.PROC_GET_PATH_PDF",
                    out paramOut,
                    new
                    {
                        p_order_no = query.OderNo,

                        //// return 
                        p_return_code = retCode,
                        p_return_message = retMessage,
                        p_path_pdf = retPathPdf

                    });

                //var Return_Code = ret_code.Value != null ? Convert.ToInt32(ret_code.Value.ToSafeString()) : -1;
                //var Return_Desc = ret_message.Value.ToSafeString();

                return new GetPathPdfModel { PathPdf = retPathPdf.Value == null || retPathPdf.Value.ToString() == "null" ? string.Empty : retPathPdf.Value.ToString() };
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());

                return new GetPathPdfModel();
            }

        }

    }
}
