using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using WBBBusinessLayer.CommandHandlers.ExWebServices.SAPFixedAsset;
using WBBBusinessLayer.FBBSAPOnlineMA2;
using WBBContract;
using WBBContract.Commands.ExWebServices.SAPFixedAsset;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    class PAYGLoadFileToTableEnhanceQueryHandler : IQueryHandler<PAYGLoadFileToTableEnhanceQuery, PAYGLoadFileToTableEnhanceListResult>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<PAYGLoadFileToTableEnhanceFileList> _objService;

        public PAYGLoadFileToTableEnhanceQueryHandler(ILogger logger, IEntityRepository<PAYGLoadFileToTableEnhanceFileList> objService
        )
        {
            _logger = logger;
            _objService = objService;
        }

        public PAYGLoadFileToTableEnhanceListResult Handle(PAYGLoadFileToTableEnhanceQuery query)
        {
            PAYGLoadFileToTableEnhanceListResult result = new PAYGLoadFileToTableEnhanceListResult();

            var historyLog = new FBB_HISTORY_LOG();
            try
            {
                _logger.Info("PAYGLoadFileToTableEnhanceQueryHandler : Start.");

                var ret_code = new OracleParameter();
                ret_code.ParameterName = "ret_code";
                ret_code.OracleDbType = OracleDbType.Int32;
                ret_code.Direction = ParameterDirection.Output;

                var return_file_name_data_cur = new OracleParameter();
                return_file_name_data_cur.ParameterName = "return_file_name_data_cur";
                return_file_name_data_cur.OracleDbType = OracleDbType.RefCursor;
                return_file_name_data_cur.Direction = ParameterDirection.Output;

                var flag = new OracleParameter();
                flag.ParameterName = "flag";
                flag.OracleDbType = OracleDbType.Varchar2;
                flag.Direction = ParameterDirection.Input;
                flag.Value = query.flag_check;

                var packageMappingObjectLoadFileToTable = new PackageMappingObjectModelLoadFileToTable
                {
                    FBBPAYG_LOADFILE_DATA_LIST =
                        (query.loadfile_enhance_list ?? new List<PAYGLoadFileToTableEnhanceFileList>())
                        .Select(a => new FBBPAYG_LOADFILE_DATA_LIST_Mapping
                        {
                            FILE_NAME = a.file_name,
                            FILE_DATA = a.file_data,
                            FILE_INDEX = a.file_index.ToSafeInteger()
                        }).ToArray()
                };
                var p_loadfile_data_list = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_loadfile_data_list", "FBBPAYG_LOADFILE_DATA_LIST", packageMappingObjectLoadFileToTable);



                var executeResult = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBBPAYG_LOADFILE_TO_TABLE.p_read_file_all_enhance",
                new object[]
                {
                        flag,
                        p_loadfile_data_list,

                        //return
                        return_file_name_data_cur,
                        ret_code
                });

                DataTable resp = new DataTable();
                List<PAYGLoadFileToTableEnhanceFileList> respList = new List<PAYGLoadFileToTableEnhanceFileList>();
                if (executeResult != null)
                {
                    resp = (DataTable)executeResult[0];
                    
                    if (query.flag_check == "getfilename")
                    {
                        var respGetFileList = resp.DataTableToList<PAYGLoadFileToTableEnhanceFileListResponse>();
                        respList = respGetFileList.Select(a => new PAYGLoadFileToTableEnhanceFileList() 
                        { 
                            file_name = a.filename
                        }).ToList();
                    }
                    else
                    {
                        respList = resp.DataTableToList<PAYGLoadFileToTableEnhanceFileList>();
                    }
                    result.Data = respList;
                }
                result.Return_Code = ret_code.Value.ToString().ToSafeInteger();


                return result;
            }
            catch (Exception ex)
            {
                _logger.Info("PAYGLoadFileToTableEnhanceQueryHandler : Error.");
                _logger.Info(ex.Message);

                result.Return_Code = -1;
                result.Return_Desc = "Error call save event service " + ex.Message;
                return result;

            }
        }

        public class PackageMappingObjectModelLoadFileToTable : INullable, IOracleCustomType
        {
            [OracleArrayMapping()]
            public FBBPAYG_LOADFILE_DATA_LIST_Mapping[] FBBPAYG_LOADFILE_DATA_LIST { get; set; }

            private bool objectIsNull;

            public bool IsNull
            {
                get { return objectIsNull; }
            }

            public static PackageMappingObjectModelLoadFileToTable Null
            {
                get
                {
                    var obj = new PackageMappingObjectModelLoadFileToTable();
                    obj.objectIsNull = true;
                    return obj;
                }
            }

            public void FromCustomObject(OracleConnection con, object udt)
            {
                OracleUdt.SetValue(con, udt, 0, FBBPAYG_LOADFILE_DATA_LIST);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                FBBPAYG_LOADFILE_DATA_LIST = (FBBPAYG_LOADFILE_DATA_LIST_Mapping[])OracleUdt.GetValue(con, udt, 0);
            }
        }


        [OracleCustomTypeMapping("FBBPAYG_LOADFILE_DATA_REC")]
        public class FBBPAYG_LOADFILE_DATA_LIST_MappingOracleTypeMappingFactory : IOracleCustomTypeFactory
        {
            public IOracleCustomType CreateObject()
            {
                return new FBBPAYG_LOADFILE_DATA_LIST_Mapping();
            }
        }

        [OracleCustomTypeMapping("FBBPAYG_LOADFILE_DATA_LIST")]
        public class FBBPAYG_LOADFILE_DATA_LIST_MappingObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
        {
            #region IOracleCustomTypeFactory Members

            public IOracleCustomType CreateObject()
            {
                return new PackageMappingObjectModelLoadFileToTable();
            }

            #endregion IOracleCustomTypeFactory Members

            #region IOracleArrayTypeFactory Members

            public Array CreateArray(int numElems)
            {
                return new FBBPAYG_LOADFILE_DATA_LIST_Mapping[numElems];
            }

            public Array CreateStatusArray(int numElems)
            {
                return null;
            }

            #endregion IOracleArrayTypeFactory Members
        }

        public class FBBPAYG_LOADFILE_DATA_LIST_Mapping : INullable, IOracleCustomType
        {
            private bool objectIsNull;

            #region Attribute Mapping


            [OracleObjectMapping("FILE_NAME")]
            public string FILE_NAME { get; set; }

            [OracleObjectMapping("FILE_DATA")]
            public string FILE_DATA { get; set; }

            [OracleObjectMapping("FILE_INDEX")]
            public int FILE_INDEX { get; set; }

            #endregion Attribute Mapping

            public static FBBPAYG_LOADFILE_DATA_LIST_Mapping Null
            {
                get
                {
                    var obj = new FBBPAYG_LOADFILE_DATA_LIST_Mapping();
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
                OracleUdt.SetValue(con, udt, "FILE_NAME", FILE_NAME);
                OracleUdt.SetValue(con, udt, "FILE_DATA", FILE_DATA);
                OracleUdt.SetValue(con, udt, "FILE_INDEX", FILE_INDEX);

            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                throw new NotImplementedException();
            }
        }
    }



}
