using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetCheckATVTopupReplaceQueryHandler : IQueryHandler<GetCheckATVTopupReplaceQuery, CheckATVTopupReplaceModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<object> _objService;

        public GetCheckATVTopupReplaceQueryHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<object> objService)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _objService = objService;
        }

        public CheckATVTopupReplaceModel Handle(GetCheckATVTopupReplaceQuery query)
        {
            InterfaceLogCommand log = null;
            CheckATVTopupReplaceModel executeResults = new CheckATVTopupReplaceModel();

            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TransactionId, "CheckATVTopupReplace", "GetCheckATVTopupReplaceQueryHandler", "", "FBB|" + query.FullUrl, "WEB");

                var P_FIBRENET_ID = new OracleParameter();
                P_FIBRENET_ID.ParameterName = "P_FIBRENET_ID";
                P_FIBRENET_ID.OracleDbType = OracleDbType.Varchar2;
                P_FIBRENET_ID.Direction = ParameterDirection.Input;
                P_FIBRENET_ID.Value = query.P_FIBRENET_ID;

                var P_FLAG_LANG = new OracleParameter();
                P_FLAG_LANG.ParameterName = "P_FLAG_LANG";
                P_FLAG_LANG.OracleDbType = OracleDbType.Varchar2;
                P_FLAG_LANG.Direction = ParameterDirection.Input;
                P_FLAG_LANG.Value = query.P_FLAG_LANG;

                var P_ADDRESS_ID = new OracleParameter();
                P_ADDRESS_ID.ParameterName = "P_ADDRESS_ID";
                P_ADDRESS_ID.OracleDbType = OracleDbType.Varchar2;
                P_ADDRESS_ID.Direction = ParameterDirection.Input;
                P_ADDRESS_ID.Value = query.P_ADDRESS_ID;

                var RETURN_CODE = new OracleParameter();
                RETURN_CODE.ParameterName = "RETURN_CODE";
                RETURN_CODE.OracleDbType = OracleDbType.Decimal;
                RETURN_CODE.Direction = ParameterDirection.Output;

                var RETURN_MESSAGE = new OracleParameter();
                RETURN_MESSAGE.ParameterName = "RETURN_MESSAGE";
                RETURN_MESSAGE.OracleDbType = OracleDbType.Varchar2;
                RETURN_MESSAGE.Size = 2000;
                RETURN_MESSAGE.Direction = ParameterDirection.Output;

                var RETURN_ERROR_FLAG = new OracleParameter();
                RETURN_ERROR_FLAG.ParameterName = "RETURN_ERROR_FLAG";
                RETURN_ERROR_FLAG.OracleDbType = OracleDbType.Varchar2;
                RETURN_ERROR_FLAG.Size = 2000;
                RETURN_ERROR_FLAG.Direction = ParameterDirection.Output;

                var RETURN_ERROR_MESSAGE = new OracleParameter();
                RETURN_ERROR_MESSAGE.ParameterName = "RETURN_ERROR_MESSAGE";
                RETURN_ERROR_MESSAGE.OracleDbType = OracleDbType.Varchar2;
                RETURN_ERROR_MESSAGE.Size = 2000;
                RETURN_ERROR_MESSAGE.Direction = ParameterDirection.Output;

                var RETURN_DROPDOWN = new OracleParameter();
                RETURN_DROPDOWN.ParameterName = "RETURN_DROPDOWN";
                RETURN_DROPDOWN.OracleDbType = OracleDbType.Decimal;
                RETURN_DROPDOWN.Direction = ParameterDirection.Output;

                var RETURN_SYMPTON_CODE = new OracleParameter();
                RETURN_SYMPTON_CODE.ParameterName = "RETURN_SYMPTON_CODE";
                RETURN_SYMPTON_CODE.OracleDbType = OracleDbType.Varchar2;
                RETURN_SYMPTON_CODE.Size = 2000;
                RETURN_SYMPTON_CODE.Direction = ParameterDirection.Output;

                var RETURN_SERIAL_CURROR = new OracleParameter();
                RETURN_SERIAL_CURROR.ParameterName = "RETURN_SERIAL_CURROR";
                RETURN_SERIAL_CURROR.OracleDbType = OracleDbType.RefCursor;
                RETURN_SERIAL_CURROR.Direction = ParameterDirection.Output;

                var fbbor050PlayboxMappingObjectModel = new Fbbor050PlayboxMappingObjectModel
                {
                    FBBOR050_PLAYBOX_ARRAY = query.Fbbor050PlayboxList.Select(
                            a => new FBBOR050_PLAYBOX_ARRAYMapping
                            {
                                CPE_TYPE = a.CPE_TYPE,
                                CPE_MODEL_NAME = a.CPE_MODEL_NAME,
                                STATUS_DESC = a.STATUS_DESC,
                                CPE_BRAND_NAME = a.CPE_BRAND_NAME,
                                CPE_MODEL_ID = a.CPE_MODEL_ID,
                                CPE_GROUP_TYPE = a.CPE_GROUP_TYPE,
                                SN_PATTERN = a.SN_PATTERN,
                                SERIAL_NO = a.SERIAL_NO,
                                STATUS = a.STATUS
                            }).ToArray()
                };

                var P_FBBOR050_COMPARE_ARRAY = OracleCustomTypeUtilities.CreateUDTArrayParameter("P_FBBOR050_COMPARE_ARRAY", "FBBOR050_PLAYBOX_ARRAY", fbbor050PlayboxMappingObjectModel);

                var result = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBBOR050.CHECK_ATV",
                     new object[]
                     {
                         P_FIBRENET_ID,
                         P_FLAG_LANG,
                         P_ADDRESS_ID,
                         P_FBBOR050_COMPARE_ARRAY,

                         //return code
                         RETURN_CODE,
                         RETURN_MESSAGE,
                         RETURN_ERROR_FLAG,
                         RETURN_ERROR_MESSAGE,
                         RETURN_DROPDOWN,
                         RETURN_SYMPTON_CODE,
                         RETURN_SERIAL_CURROR
                     });

                if (result != null)
                {
                    executeResults.RETURN_CODE = result[0] != null ? Convert.ToInt16(result[0].ToSafeString()) : -1;
                    executeResults.RETURN_MESSAGE = result[1] != null ? result[1].ToSafeString() : "Error";

                    if (executeResults.RETURN_CODE != -1)
                    {
                        executeResults.RETURN_ERROR_FLAG = result[2] != null ? result[2].ToSafeString() : "";
                        executeResults.RETURN_ERROR_MESSAGE = result[3] != null ? result[3].ToSafeString() : "";
                        executeResults.RETURN_DROPDOWN = result[4] != null ? Convert.ToInt16(result[4].ToSafeString()) : -1;
                        executeResults.RETURN_SYMPTON_CODE = result[5] != null ? result[5].ToSafeString() : "";
                        DataTable data1 = (DataTable)result[6];
                        executeResults.RETURN_SERIAL_CURROR = data1.DataTableToList<SerialATV>();
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResults, log, "Success", "", "");
                    }
                    else
                    {
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResults, log, "Failed", executeResults.RETURN_MESSAGE, "");
                    }
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResults, log, "Failed", "Error", "");
                }
            }
            catch (Exception ex)
            {
                _logger.Info("GetCheckATVTopupReplaceQueryHandler : Error.");
                _logger.Info(ex.Message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Error", ex.StackTrace, "");
                throw;
            }

            return executeResults;
        }

        #region Mapping FBBOR050_PLAYBOX_ARRAY Type Oracle
        public class Fbbor050PlayboxMappingObjectModel : INullable, IOracleCustomType
        {
            [OracleArrayMapping()]
            public FBBOR050_PLAYBOX_ARRAYMapping[] FBBOR050_PLAYBOX_ARRAY { get; set; }

            private bool objectIsNull;

            public bool IsNull
            {
                get { return objectIsNull; }
            }

            public static Fbbor050PlayboxMappingObjectModel Null
            {
                get
                {
                    var obj = new Fbbor050PlayboxMappingObjectModel();
                    obj.objectIsNull = true;
                    return obj;
                }
            }

            public void FromCustomObject(OracleConnection con, object udt)
            {
                OracleUdt.SetValue(con, udt, 0, FBBOR050_PLAYBOX_ARRAY);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                FBBOR050_PLAYBOX_ARRAY = (FBBOR050_PLAYBOX_ARRAYMapping[])OracleUdt.GetValue(con, udt, 0);
            }
        }

        [OracleCustomTypeMapping("FBBOR050_COMPARE_RECORD")]
        public class Fbbor050CompareOracleTypeMappingFactory : IOracleCustomTypeFactory
        {
            public IOracleCustomType CreateObject()
            {
                return new FBBOR050_PLAYBOX_ARRAYMapping();
            }
        }

        [OracleCustomTypeMapping("FBBOR050_PLAYBOX_ARRAY")]
        public class Fbbor050CompareObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
        {
            #region IOracleCustomTypeFactory Members

            public IOracleCustomType CreateObject()
            {
                return new Fbbor050PlayboxMappingObjectModel();
            }

            #endregion IOracleCustomTypeFactory Members

            #region IOracleArrayTypeFactory Members

            public Array CreateArray(int numElems)
            {
                return new FBBOR050_PLAYBOX_ARRAYMapping[numElems];
            }

            public Array CreateStatusArray(int numElems)
            {
                return null;
            }

            #endregion IOracleArrayTypeFactory Members
        }

        public class FBBOR050_PLAYBOX_ARRAYMapping : INullable, IOracleCustomType
        {
            private bool objectIsNull;

            #region Attribute Mapping

            [OracleObjectMapping("CPE_TYPE")]
            public string CPE_TYPE { get; set; }

            [OracleObjectMapping("CPE_MODEL_NAME")]
            public string CPE_MODEL_NAME { get; set; }

            [OracleObjectMapping("STATUS_DESC")]
            public string STATUS_DESC { get; set; }

            [OracleObjectMapping("CPE_BRAND_NAME")]
            public string CPE_BRAND_NAME { get; set; }

            [OracleObjectMapping("CPE_MODEL_ID")]
            public string CPE_MODEL_ID { get; set; }

            [OracleObjectMapping("CPE_GROUP_TYPE")]
            public string CPE_GROUP_TYPE { get; set; }

            [OracleObjectMapping("SN_PATTERN")]
            public string SN_PATTERN { get; set; }

            [OracleObjectMapping("SERIAL_NO")]
            public string SERIAL_NO { get; set; }

            [OracleObjectMapping("STATUS")]
            public string STATUS { get; set; }

            #endregion Attribute Mapping

            public static FBBOR050_PLAYBOX_ARRAYMapping Null
            {
                get
                {
                    var obj = new FBBOR050_PLAYBOX_ARRAYMapping();
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
                OracleUdt.SetValue(con, udt, "CPE_TYPE", CPE_TYPE);
                OracleUdt.SetValue(con, udt, "CPE_MODEL_NAME", CPE_MODEL_NAME);
                OracleUdt.SetValue(con, udt, "STATUS_DESC", STATUS_DESC);
                OracleUdt.SetValue(con, udt, "CPE_BRAND_NAME", CPE_BRAND_NAME);
                OracleUdt.SetValue(con, udt, "CPE_MODEL_ID", CPE_MODEL_ID);
                OracleUdt.SetValue(con, udt, "CPE_GROUP_TYPE", CPE_GROUP_TYPE);
                OracleUdt.SetValue(con, udt, "SN_PATTERN", SN_PATTERN);
                OracleUdt.SetValue(con, udt, "SERIAL_NO", SERIAL_NO);
                OracleUdt.SetValue(con, udt, "STATUS", STATUS);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                throw new NotImplementedException();
            }
        }

        #endregion Mapping  FBB_CONFIG_QUERY_ARRAY Type Oracle

    }
}
