using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBContract.QueryModels.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetPackageDetailByNonMobileQueryHandler : IQueryHandler<GetPackageDetailByNonMobileQuery, GetPackageDetailByNonMobileQueryModel>
    {
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<object> _objService;

        public GetPackageDetailByNonMobileQueryHandler(IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<object> objService)
        {
            _uow = uow;
            _intfLog = intfLog;
            _objService = objService;
        }

        public GetPackageDetailByNonMobileQueryModel Handle(GetPackageDetailByNonMobileQuery query)
        {
            GetPackageDetailByNonMobileQueryModel executeResults = new GetPackageDetailByNonMobileQueryModel();
            var log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.P_FIBRENET_ID, "GetPackageDetailByNonMobileQuery", "GetPackageDetailByNonMobileQueryHandler", null, "FBB", "");

            try
            {

                var P_FIBRENET_ID = new OracleParameter();
                P_FIBRENET_ID.ParameterName = "p_fibrenet_id";
                P_FIBRENET_ID.Size = 2000;
                P_FIBRENET_ID.OracleDbType = OracleDbType.Varchar2;
                P_FIBRENET_ID.Direction = ParameterDirection.Input;
                P_FIBRENET_ID.Value = query.P_FIBRENET_ID;

                var fbbor051PackageMappingObjectModel = new Fbbor051PackageMappingObjectModel();

                fbbor051PackageMappingObjectModel.FBBOR051_PACKAGE_ARRAY = query.P_FBBOR051_PACKAGE_ARRAY.Select(c => new FBBOR051_PACKAGE_ARRAYMapping
                {
                    fibrenetId = c.fibrenetId,
                    productCD = c.productCD,
                    productClass = c.productClass,
                    productType = c.productType,
                    startDate = c.startDate,
                    endDate = c.endDate
                }).ToArray();

                var P_FBBOR051_PACKAGE_ARRAY = OracleCustomTypeUtilities.CreateUDTArrayParameter("P_FBBOR051_PACKAGE_ARRAY", "FBBOR051_PACKAGE_ARRAY", fbbor051PackageMappingObjectModel);

                var RETURN_CODE = new OracleParameter();
                RETURN_CODE.ParameterName = "RETURN_CODE";
                RETURN_CODE.OracleDbType = OracleDbType.Decimal;
                RETURN_CODE.Direction = ParameterDirection.Output;

                var RETURN_MESSAGE = new OracleParameter();
                RETURN_MESSAGE.ParameterName = "RETURN_MESSAGE";
                RETURN_MESSAGE.Size = 2000;
                RETURN_MESSAGE.OracleDbType = OracleDbType.Varchar2;
                RETURN_MESSAGE.Direction = ParameterDirection.Output;

                var RETURN_PACKAGE_MAIN = new OracleParameter();
                RETURN_PACKAGE_MAIN.ParameterName = "RETURN_PACKAGE_MAIN";
                RETURN_PACKAGE_MAIN.OracleDbType = OracleDbType.RefCursor;
                RETURN_PACKAGE_MAIN.Direction = ParameterDirection.Output;

                var RETURN_PACKAGE_DISCOUNT = new OracleParameter();
                RETURN_PACKAGE_DISCOUNT.ParameterName = "RETURN_PACKAGE_DISCOUNT";
                RETURN_PACKAGE_DISCOUNT.OracleDbType = OracleDbType.RefCursor;
                RETURN_PACKAGE_DISCOUNT.Direction = ParameterDirection.Output;

                var RETURN_PACKAGE_CONTENT = new OracleParameter();
                RETURN_PACKAGE_CONTENT.ParameterName = "RETURN_PACKAGE_CONTENT";
                RETURN_PACKAGE_CONTENT.OracleDbType = OracleDbType.RefCursor;
                RETURN_PACKAGE_CONTENT.Direction = ParameterDirection.Output;

                var RETURN_PACKAGE_VAS = new OracleParameter();
                RETURN_PACKAGE_VAS.ParameterName = "RETURN_PACKAGE_VAS";
                RETURN_PACKAGE_VAS.OracleDbType = OracleDbType.RefCursor;
                RETURN_PACKAGE_VAS.Direction = ParameterDirection.Output;

                var RETURN_PACKAGE_INSTALL = new OracleParameter();
                RETURN_PACKAGE_INSTALL.ParameterName = "RETURN_PACKAGE_INSTALL";
                RETURN_PACKAGE_INSTALL.OracleDbType = OracleDbType.RefCursor;
                RETURN_PACKAGE_INSTALL.Direction = ParameterDirection.Output;

                var result = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBBOR051.QUERY_PACKAGE_DETAIL",
                    new object[]
                    {
                         P_FIBRENET_ID,
                         P_FBBOR051_PACKAGE_ARRAY,
                         //return code
                         RETURN_CODE,
                         RETURN_MESSAGE,
                         RETURN_PACKAGE_MAIN,
                         RETURN_PACKAGE_DISCOUNT,
                         RETURN_PACKAGE_CONTENT,
                         RETURN_PACKAGE_VAS,
                         RETURN_PACKAGE_INSTALL
                    });
                executeResults.RETURN_CODE = result[0] != null ? result[0].ToSafeString() : "-1";
                executeResults.RETURN_MESSAGE = result[1] != null ? result[1].ToSafeString() : "error";
                if (executeResults.RETURN_CODE != "-1")
                {
                    DataTable data1 = (DataTable)result[2];
                    executeResults.PACKAGE_MAIN = data1.DataTableToList<WelcomePackageMain>();
                    //DataTable data2 = (DataTable)result[3];
                    //executeResults.PACKAGE_DISCOUNT = data2.DataTableToList<WelcomePackageDiscount>();
                    //DataTable data3 = (DataTable)result[4];
                    //executeResults.PACKAGE_CONTENT = data3.DataTableToList<WelcomePackageContent>();
                    //DataTable data4 = (DataTable)result[5];
                    //executeResults.PACKAGE_VAS = data4.DataTableToList<WelcomePackageVas>();
                    //DataTable data5 = (DataTable)result[6];
                    //executeResults.PACKAGE_INSTALL = data5.DataTableToList<WelcomePackageInstall>();
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResults, log, "Success", "", "");
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResults.RETURN_MESSAGE, log, "Failed", executeResults.RETURN_MESSAGE, "");
                }

            }
            catch (Exception ex)
            {
                executeResults.RETURN_CODE = "-1";
                executeResults.RETURN_MESSAGE = "Error";
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex.Message, log, "Failed", ex.StackTrace, "");

                return null;
            }
            return executeResults;
        }

        #region Mapping FBBOR051_PACKAGE_ARRAY Type Oracle

        public class Fbbor051PackageMappingObjectModel : INullable, IOracleCustomType
        {
            [OracleArrayMapping()]
            public FBBOR051_PACKAGE_ARRAYMapping[] FBBOR051_PACKAGE_ARRAY { get; set; }

            private bool objectIsNull;

            public bool IsNull
            {
                get { return objectIsNull; }
            }

            public static Fbbor051PackageMappingObjectModel Null
            {
                get
                {
                    var obj = new Fbbor051PackageMappingObjectModel();
                    obj.objectIsNull = true;
                    return obj;
                }
            }

            public void FromCustomObject(OracleConnection con, object udt)
            {
                OracleUdt.SetValue(con, udt, 0, FBBOR051_PACKAGE_ARRAY);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                FBBOR051_PACKAGE_ARRAY = (FBBOR051_PACKAGE_ARRAYMapping[])OracleUdt.GetValue(con, udt, 0);
            }
        }

        [OracleCustomTypeMapping("FBBOR051_PACKAGE_RECORD")]
        public class FBBOR051_PACKAGE_MappingOracleTypeMappingFactory : IOracleCustomTypeFactory
        {
            public IOracleCustomType CreateObject()
            {
                return new FBBOR051_PACKAGE_ARRAYMapping();
            }
        }

        [OracleCustomTypeMapping("FBBOR051_PACKAGE_ARRAY")]
        public class Fbbor051PackageMappingObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
        {
            #region IOracleCustomTypeFactory Members

            public IOracleCustomType CreateObject()
            {
                return new Fbbor051PackageMappingObjectModel();
            }

            #endregion IOracleCustomTypeFactory Members

            #region IOracleArrayTypeFactory Members

            public Array CreateArray(int numElems)
            {
                return new FBBOR051_PACKAGE_ARRAYMapping[numElems];
            }

            public Array CreateStatusArray(int numElems)
            {
                return null;
            }

            #endregion IOracleArrayTypeFactory Members
        }

        public class FBBOR051_PACKAGE_ARRAYMapping : INullable, IOracleCustomType
        {
            private bool objectIsNull;

            #region Attribute Mapping

            [OracleObjectMapping("FIBRENETID")]
            public string fibrenetId { get; set; }

            [OracleObjectMapping("PRODUCTCD")]
            public string productCD { get; set; }

            [OracleObjectMapping("PRODUCTCLASS")]
            public string productClass { get; set; }

            [OracleObjectMapping("PRODUCTTYPE")]
            public string productType { get; set; }

            [OracleObjectMapping("STARTDATE")]
            public string startDate { get; set; }

            [OracleObjectMapping("ENDDATE")]
            public string endDate { get; set; }

            #endregion Attribute Mapping

            public static FBBOR051_PACKAGE_ARRAYMapping Null
            {
                get
                {
                    var obj = new FBBOR051_PACKAGE_ARRAYMapping();
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
                OracleUdt.SetValue(con, udt, "FIBRENETID", fibrenetId);
                OracleUdt.SetValue(con, udt, "PRODUCTCD", productCD);
                OracleUdt.SetValue(con, udt, "PRODUCTCLASS", productClass);
                OracleUdt.SetValue(con, udt, "PRODUCTTYPE", productType);
                OracleUdt.SetValue(con, udt, "STARTDATE", startDate);
                OracleUdt.SetValue(con, udt, "ENDDATE", endDate);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                throw new NotImplementedException();
            }
        }

        #endregion Mapping  FBBOR051_PACKAGE_ARRAY Type Oracle

    }


}
