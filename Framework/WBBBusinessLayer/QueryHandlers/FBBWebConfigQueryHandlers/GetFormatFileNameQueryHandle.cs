using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class GetFormatFileNameQueryHandle : IQueryHandler<GetFormatFileNameQuery, List<FileFormatModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FileFormatModel> _PicFormat;


        public GetFormatFileNameQueryHandle(ILogger logger, IEntityRepository<FileFormatModel> PicFormat)
        {
            _logger = logger;
            _PicFormat = PicFormat;

        }

        public List<FileFormatModel> Handle(GetFormatFileNameQuery query)
        {
            try
            {
                var picturemodel = new PictureObjectModel();

                picturemodel.REC_REG_PACKAGE = query.ListFilename.Select(p => new PicturePackageOracleTypeMapping
                {
                    file_name = p.FileName.ToSafeString()
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

                List<FileFormatModel> executeResult = _PicFormat.ExecuteReadStoredProc("WBB.PKG_FBBOR004.PROC_GEN_FILE_NAME",
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
                return new List<FileFormatModel>();
            }

        }
    }

    #region Mapping ImageFilePACKAGE Type Oracle
    public class PictureObjectModel : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
    {
        [OracleArrayMapping()]
        public PicturePackageOracleTypeMapping[] REC_REG_PACKAGE { get; set; }

        private bool objectIsNull;

        public bool IsNull
        {
            get { return objectIsNull; }
        }

        public static PictureObjectModel Null
        {
            get
            {
                PictureObjectModel obj = new PictureObjectModel();
                obj.objectIsNull = true;
                return obj;
            }
        }

        public void FromCustomObject(OracleConnection con, object udt)
        {
            OracleUdt.SetValue(con, udt, 0, REC_REG_PACKAGE);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            REC_REG_PACKAGE = (PicturePackageOracleTypeMapping[])OracleUdt.GetValue(con, udt, 0);
        }
    }

    [OracleCustomTypeMappingAttribute("FBB_REG_BROWSE_FILE_RECORD")]
    public class PicturePackageOracleTypeMappingFactory : IOracleCustomTypeFactory
    {
        #region IOracleCustomTypeFactory Members

        public IOracleCustomType CreateObject()
        {
            return new PicturePackageOracleTypeMapping();
        }

        #endregion
    }
    [OracleCustomTypeMapping("FBB_REG_BROWSE_FILE_ARRAY")]
    public class PictureObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
    {
        #region IOracleCustomTypeFactory Members
        public IOracleCustomType CreateObject()
        {
            return new PictureObjectModel();
        }

        #endregion

        #region IOracleArrayTypeFactory Members
        public Array CreateArray(int numElems)
        {
            return new PicturePackageOracleTypeMapping[numElems];
        }

        public Array CreateStatusArray(int numElems)
        {
            return null;
        }

        #endregion
    }

    public class PicturePackageOracleTypeMapping : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
    {
        private bool objectIsNull;

        #region Attribute Mapping
        [OracleObjectMappingAttribute("FILE_NAME")]
        public string file_name { get; set; }

        #endregion

        public static PicturePackageOracleTypeMapping Null
        {
            get
            {
                PicturePackageOracleTypeMapping obj = new PicturePackageOracleTypeMapping();
                obj.objectIsNull = true;
                return obj;
            }
        }

        public bool IsNull
        {
            get { return objectIsNull; }
        }

        public void FromCustomObject(OracleConnection con, object udt)
        {
            OracleUdt.SetValue(con, udt, "FILE_NAME", file_name);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            throw new NotImplementedException();
        }
    }
    #endregion
}
