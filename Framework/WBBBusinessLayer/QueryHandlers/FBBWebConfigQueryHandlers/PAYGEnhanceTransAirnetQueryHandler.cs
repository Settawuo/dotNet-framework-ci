using AIRNETEntity.Extensions;
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
using WBBEntity.PanelModels.FBBWebConfigModels;
using OracleCustomTypeUtilities = AIRNETEntity.Extensions.OracleCustomTypeUtilities;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    class PAYGEnhanceTransAirnetQueryHandler : IQueryHandler<PAYGEnhanceTransAirnetQuery, PAYGTransAirnetListResult>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<PAYGTransAirnetFileList> _objService;

        public PAYGEnhanceTransAirnetQueryHandler(ILogger logger, IEntityRepository<PAYGTransAirnetFileList> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public PAYGTransAirnetListResult Handle(PAYGEnhanceTransAirnetQuery query)
        {

            PAYGTransAirnetListResult result = new PAYGTransAirnetListResult();

            try
            {
                _logger.Info("PAYGEnhanceTransAirnetQueryHandler : Start.");



                var packageMappingObjectModel = new PackageMappingObjectModel
                {
                    FBBPAYG_TRANS_AIRNET_LIST =
                        query.f_enchance_list.Select(
                           a => new FBBPAYG_TRANS_AIRNET_LISTMapping
                           {
                               FILE_NAME = a.file_name,
                               FILE_DATA = a.file_data

                           }).ToArray()
                };

                var FBBPAYG_AIRNET_DATA_LIST = OracleCustomTypeUtilities.CreateUDTArrayParameter("FBBPAYG_AIRNET_DATA_LIST", "WBB.FBBPAYG_AIRNET_DATA_LIST", packageMappingObjectModel);

                var p_airnet_data_list = new OracleParameter();
                p_airnet_data_list.OracleDbType = OracleDbType.Varchar2;
                p_airnet_data_list.Value = query.f_enchance_list;
                p_airnet_data_list.Direction = ParameterDirection.Input;

                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;

                var my_cur = new OracleParameter();
                my_cur.ParameterName = "return_airnet_data_cur";
                my_cur.OracleDbType = OracleDbType.RefCursor;
                my_cur.Direction = ParameterDirection.Output;

                //List<PAYGTransAirnetFileList> executeResult = _objService.ExecuteReadStoredProc("WBB.pkg_fbbpayg_airnet_inv_rec.p_fetch_trans_airnet_data",
                //        new
                //        {
                //            FBBPAYG_AIRNET_DATA_LIST,
                //            ret_code = ret_code,
                //            ret_msg = ret_msg,
                //            my_cur = my_cur
                //        }).ToList();

                //result.Return_Code = Convert.ToInt16(ret_code.Value.ToString());
                //result.Return_Desc = ret_msg.Value.ToString();
                //result.Data = executeResult;
                //return result;


                var executeResult = _objService.ExecuteStoredProcMultipleCursor("WBB.pkg_fbbpayg_airnet_inv_rec.p_fetch_trans_airnet_data",
                            new object[]
                            {
                                                FBBPAYG_AIRNET_DATA_LIST,
                                                ret_code = ret_code,
                                                ret_msg = ret_msg,
                                                my_cur = my_cur
                            }).ToList();

                result.Return_Code = Convert.ToInt16(ret_code.Value.ToString());
                result.Return_Desc = ret_msg.Value.ToString();

                DataTable resp = new DataTable();
                List<PAYGTransAirnetFileList> respList = new List<PAYGTransAirnetFileList>();
                if (executeResult[2] != null)
                {
                    resp = (DataTable)executeResult[2];
                    respList = resp.DataTableToList<PAYGTransAirnetFileList>();
                    result.Data = respList;
                    //returnForm.out_cursor = respList;
                }
                return result;

            }
            catch (Exception ex)
            {
                _logger.Info("PAYGTransAirnetQueryHandler : Error.");
                _logger.Info(ex.Message);

                result.Return_Code = -1;
                result.Return_Desc = "Error call save event service " + ex.Message;
                return result;
            }

        }
        public class PackageMappingObjectModel : INullable, IOracleCustomType
        {
            [OracleArrayMapping()]
            public FBBPAYG_TRANS_AIRNET_LISTMapping[] FBBPAYG_TRANS_AIRNET_LIST { get; set; }

            private bool objectIsNull;

            public bool IsNull
            {
                get { return objectIsNull; }
            }

            public static PackageMappingObjectModel Null
            {
                get
                {
                    var obj = new PackageMappingObjectModel();
                    obj.objectIsNull = true;
                    return obj;
                }
            }

            public void FromCustomObject(OracleConnection con, object udt)
            {
                OracleUdt.SetValue(con, udt, 0, FBBPAYG_TRANS_AIRNET_LIST);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                FBBPAYG_TRANS_AIRNET_LIST = (FBBPAYG_TRANS_AIRNET_LISTMapping[])OracleUdt.GetValue(con, udt, 0);
            }
        }

        [OracleCustomTypeMapping("WBB.FBBPAYG_AIRNET_DATA_REC")]
        public class FBBPAYG_TRANS_AIRNET_LISTMappingOracleTypeMappingFactory : IOracleCustomTypeFactory
        {
            public IOracleCustomType CreateObject()
            {
                return new FBBPAYG_TRANS_AIRNET_LISTMapping();
            }
        }

        [OracleCustomTypeMapping("WBB.FBBPAYG_AIRNET_DATA_LIST")]
        public class FBBPAYG_TRANS_AIRNET_LISTMappingObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
        {
            #region IOracleCustomTypeFactory Members

            public IOracleCustomType CreateObject()
            {
                return new PackageMappingObjectModel();
            }

            #endregion IOracleCustomTypeFactory Members

            #region IOracleArrayTypeFactory Members

            public Array CreateArray(int numElems)
            {
                return new FBBPAYG_TRANS_AIRNET_LISTMapping[numElems];
            }

            public Array CreateStatusArray(int numElems)
            {
                return null;
            }

            #endregion IOracleArrayTypeFactory Members
        }

        public class FBBPAYG_TRANS_AIRNET_LISTMapping : INullable, IOracleCustomType
        {
            private bool objectIsNull;

            #region Attribute Mapping


            [OracleObjectMapping("FILE_NAME")]
            public string FILE_NAME { get; set; }

            [OracleObjectMapping("FILE_DATA")]
            public string FILE_DATA { get; set; }

            #endregion Attribute Mapping

            public static FBBPAYG_TRANS_AIRNET_LISTMapping Null
            {
                get
                {
                    var obj = new FBBPAYG_TRANS_AIRNET_LISTMapping();
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
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                throw new NotImplementedException();
            }
        }

    }
}
