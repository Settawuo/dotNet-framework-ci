using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.Report;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.Report
{
    public class UpdateInterfaceQueryHandler : IQueryHandler<UpdateInterfaceQuery, LogInterfaceReportResponse>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<LogInterfaceReportResponse> _LogInterfaceReportResponse;

        public UpdateInterfaceQueryHandler(ILogger logger, IEntityRepository<LogInterfaceReportResponse> logInterfaceReportResponse)
        {
            _logger = logger;
            _LogInterfaceReportResponse = logInterfaceReportResponse;
        }

        public LogInterfaceReportResponse Handle(UpdateInterfaceQuery query)
        {
            LogInterfaceReportResponse result = new LogInterfaceReportResponse();
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

                var InterfaceIdListMappingObjectModel = new InterfaceIdListMappingObjectModel();
                if (query.INTERFACE_ID != null)
                {
                    InterfaceIdListMappingObjectModel.INTERFACE_ID_LIST = query.INTERFACE_ID.Select(m => new InterfaceId_Mapping_ArrayMapping
                    {
                        InterfaceId = m.ToSafeString()
                    }).ToArray();
                }

                var InterfaceIdListMapping = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_Interface_Id_List", "INTERFACE_ID_LIST", InterfaceIdListMappingObjectModel);

                var executeResult = _LogInterfaceReportResponse.ExecuteStoredProc("WBB.PKG_FBB_REPORT_LOG_INTERFACE.PROC_UPDATE_LOG_INTERFACE",
                  new
                  {
                      p_Interface_Id_List = InterfaceIdListMapping,

                      /// return //////
                      p_return_code = p_return_code,
                      p_return_message = p_return_message

                  });

                result.ReturnCode = p_return_code.Value != null ? p_return_code.Value.ToString() : "-1";
                result.ReturnDesc = p_return_message.Value.ToString();

                return result;

            }
            catch (Exception ex)
            {
                result.ReturnCode = "-1";
                result.ReturnDesc = "PKG_FBB_REPORT_LOG_INTERFACE.PROC_UPDATE_LOG_INTERFACE Error : " + ex.Message;

                return result;
            }
        }

        [OracleCustomTypeMappingAttribute("INTERFACE_ID_REC")]
        public class Package_MappingOracleTypeMappingFactory : IOracleCustomTypeFactory
        {
            public IOracleCustomType CreateObject()
            {
                return new InterfaceId_Mapping_ArrayMapping();
            }
        }

        [OracleCustomTypeMapping("INTERFACE_ID_LIST")]
        public class ProductMappingObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
        {
            #region IOracleCustomTypeFactory Members

            public IOracleCustomType CreateObject()
            {
                return new InterfaceIdListMappingObjectModel();
            }

            #endregion IOracleCustomTypeFactory Members

            #region IOracleArrayTypeFactory Members

            public Array CreateArray(int numElems)
            {
                return new InterfaceId_Mapping_ArrayMapping[numElems];
            }

            public Array CreateStatusArray(int numElems)
            {
                return null;
            }

            #endregion IOracleArrayTypeFactory Members
        }

        public class InterfaceId_Mapping_ArrayMapping : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        {
            private bool objectIsNull;

            #region Attribute Mapping

            [OracleObjectMappingAttribute("INTERFACE_ID")]
            public string InterfaceId { get; set; }

            #endregion Attribute Mapping

            public static InterfaceId_Mapping_ArrayMapping Null
            {
                get
                {
                    InterfaceId_Mapping_ArrayMapping obj = new InterfaceId_Mapping_ArrayMapping();
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
                OracleUdt.SetValue(con, udt, "INTERFACE_ID", InterfaceId);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                throw new NotImplementedException();
            }
        }

        public class InterfaceIdListMappingObjectModel : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        {
            [OracleArrayMapping()]
            public InterfaceId_Mapping_ArrayMapping[] INTERFACE_ID_LIST { get; set; }

            private bool objectIsNull;

            public bool IsNull
            {
                get { return objectIsNull; }
            }

            public static InterfaceIdListMappingObjectModel Null
            {
                get
                {
                    InterfaceIdListMappingObjectModel obj = new InterfaceIdListMappingObjectModel();
                    obj.objectIsNull = true;
                    return obj;
                }
            }

            public void FromCustomObject(OracleConnection con, object udt)
            {
                OracleUdt.SetValue(con, udt, 0, INTERFACE_ID_LIST);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                INTERFACE_ID_LIST = (InterfaceId_Mapping_ArrayMapping[])OracleUdt.GetValue(con, udt, 0);
            }
        }
    }
}
