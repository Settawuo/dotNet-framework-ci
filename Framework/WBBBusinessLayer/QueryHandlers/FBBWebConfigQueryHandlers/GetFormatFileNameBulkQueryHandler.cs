using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class GetFormatFileNameBulkQueryHandler : IQueryHandler<GetFormatFileNameBulkQuery, List<FileFormatModelBulk>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FileFormatModelBulk> _PicFormat;

        public GetFormatFileNameBulkQueryHandler(ILogger logger, IEntityRepository<FileFormatModelBulk> PicFormat)
        {
            _logger = logger;
            _PicFormat = PicFormat;

        }

        public List<FileFormatModelBulk> Handle(GetFormatFileNameBulkQuery query)
        {
            try
            {
                var picturemodel = new PictureObjectModel();

                picturemodel.REC_REG_PACKAGE = query.ListFilenameBulk.Select(p => new PicturePackageOracleTypeMapping
                {
                    file_name = p.FileNameBulk.ToSafeString()
                }).ToArray();

                var listfilename = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_REC_BROWSE_FILE", "FBB_REG_BROWSE_FILE_ARRAY", picturemodel);

                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_message = new OracleParameter();
                ret_message.OracleDbType = OracleDbType.Varchar2;
                ret_message.Size = 2000;
                ret_message.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var cursor = new OracleParameter();
                cursor.OracleDbType = OracleDbType.RefCursor;
                cursor.Direction = ParameterDirection.Output;

                List<FileFormatModelBulk> executeResult = _PicFormat.ExecuteReadStoredProc("WBB.PKG_FBBOR004.PROC_GEN_FILE_NAME",
                  new
                  {
                      p_lang = query.language,
                      p_id_card_type = query.ID_CardType,
                      p_id_card_no = query.ID_CardNo,
                      p_REC_BROWSE_FILE = listfilename,

                      ///// return //////
                      ret_code = ret_code,
                      ret_message = ret_message,
                      ret_file_name = cursor

                  }).ToList();

                var Return_Code = ret_code.Value != null ? Convert.ToInt32(ret_code.Value.ToSafeString()) : -1;
                var Return_Desc = ret_message.Value.ToSafeString();

                return executeResult;
            }
            catch (Exception ex)
            {
                _logger.Info("Error when call GetFormatFileNameBulkQueryHandler " + ex.Message);
                return new List<FileFormatModelBulk>();
            }

        }
    }



}
