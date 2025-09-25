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
    public class GetCheckPromotionTopupReplaceQueryHandler : IQueryHandler<GetCheckPromotionTopupReplaceQuery, CheckPromotionTopupReplaceModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<object> _objService;

        public GetCheckPromotionTopupReplaceQueryHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<object> objService)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _objService = objService;
        }

        public CheckPromotionTopupReplaceModel Handle(GetCheckPromotionTopupReplaceQuery query)
        {
            InterfaceLogCommand log = null;
            CheckPromotionTopupReplaceModel executeResults = new CheckPromotionTopupReplaceModel();

            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TransactionId, "CheckPromotionTopupReplace", "GetCheckPromotionTopupReplaceQueryHandler", "", "FBB|" + query.FullUrl, "WEB");

                var P_FLAG_LANG = new OracleParameter();
                P_FLAG_LANG.ParameterName = "P_FLAG_LANG";
                P_FLAG_LANG.OracleDbType = OracleDbType.Varchar2;
                P_FLAG_LANG.Direction = ParameterDirection.Input;
                P_FLAG_LANG.Value = query.P_FLAG_LANG;

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

                var fbbContractDeviceMappingObjectModel = new FbbContractDeviceMappingObjectModel
                {
                    FBB_CONTRACT_DEVICE_ARRAY = query.SffPromotionCodeList.Select(a => new FBB_CONTRACT_DEVICE_ARRAYMapping { PROMOTION_CODE = a.PROMOTION_CODE }).ToArray()
                };

                var P_PROMOTION_ARRAY = OracleCustomTypeUtilities.CreateUDTArrayParameter("P_PROMOTION_ARRAY", "FBB_CONTRACT_DEVICE_ARRAY", fbbContractDeviceMappingObjectModel);

                var result = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBBOR050.CHECK_PROMOTION",
                     new object[]
                     {
                         P_FLAG_LANG,
                         P_PROMOTION_ARRAY,

                         //return code
                         RETURN_CODE,
                         RETURN_MESSAGE,
                         RETURN_ERROR_FLAG,
                         RETURN_ERROR_MESSAGE
                     });

                if (result != null)
                {
                    executeResults.RETURN_CODE = result[0] != null ? Convert.ToInt16(result[0].ToSafeString()) : -1;
                    executeResults.RETURN_MESSAGE = result[1] != null ? result[1].ToSafeString() : "Error";

                    if (executeResults.RETURN_CODE != -1)
                    {
                        executeResults.RETURN_ERROR_FLAG = result[2] != null ? result[2].ToSafeString() : "";
                        executeResults.RETURN_ERROR_MESSAGE = result[3] != null ? result[3].ToSafeString() : "";
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
                _logger.Info("GetCheckPromotionTopupReplaceQueryHandler : Error.");
                _logger.Info(ex.Message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Error", ex.StackTrace, "");
                throw;
            }

            return executeResults;
        }

        #region Mapping FBB_CONTRACT_DEVICE_ARRAY Type Oracle

        public class FbbContractDeviceMappingObjectModel : INullable, IOracleCustomType
        {
            [OracleArrayMapping()]
            public FBB_CONTRACT_DEVICE_ARRAYMapping[] FBB_CONTRACT_DEVICE_ARRAY { get; set; }

            private bool objectIsNull;

            public bool IsNull
            {
                get { return objectIsNull; }
            }

            public static FbbContractDeviceMappingObjectModel Null
            {
                get
                {
                    var obj = new FbbContractDeviceMappingObjectModel();
                    obj.objectIsNull = true;
                    return obj;
                }
            }

            public void FromCustomObject(OracleConnection con, object udt)
            {
                OracleUdt.SetValue(con, udt, 0, FBB_CONTRACT_DEVICE_ARRAY);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                FBB_CONTRACT_DEVICE_ARRAY = (FBB_CONTRACT_DEVICE_ARRAYMapping[])OracleUdt.GetValue(con, udt, 0);
            }
        }

        [OracleCustomTypeMapping("FBB_CONTRACT_DEVICE_RECORD")]
        public class FbbContractDeviceOracleTypeMappingFactory : IOracleCustomTypeFactory
        {
            public IOracleCustomType CreateObject()
            {
                return new FBB_CONTRACT_DEVICE_ARRAYMapping();
            }
        }

        [OracleCustomTypeMapping("FBB_CONTRACT_DEVICE_ARRAY")]
        public class FbbContractDeviceObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
        {
            #region IOracleCustomTypeFactory Members

            public IOracleCustomType CreateObject()
            {
                return new FbbContractDeviceMappingObjectModel();
            }

            #endregion IOracleCustomTypeFactory Members

            #region IOracleArrayTypeFactory Members

            public Array CreateArray(int numElems)
            {
                return new FBB_CONTRACT_DEVICE_ARRAYMapping[numElems];
            }

            public Array CreateStatusArray(int numElems)
            {
                return null;
            }

            #endregion IOracleArrayTypeFactory Members
        }

        public class FBB_CONTRACT_DEVICE_ARRAYMapping : INullable, IOracleCustomType
        {
            private bool objectIsNull;

            #region Attribute Mapping

            [OracleObjectMapping("PROMOTION_CODE")]
            public string PROMOTION_CODE { get; set; }

            #endregion Attribute Mapping

            public static FBB_CONTRACT_DEVICE_ARRAYMapping Null
            {
                get
                {
                    var obj = new FBB_CONTRACT_DEVICE_ARRAYMapping();
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
                OracleUdt.SetValue(con, udt, "PROMOTION_CODE", PROMOTION_CODE);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                throw new NotImplementedException();
            }
        }

        #endregion Mapping  FBB_CONFIG_QUERY_ARRAY Type Oracle
    }
}
