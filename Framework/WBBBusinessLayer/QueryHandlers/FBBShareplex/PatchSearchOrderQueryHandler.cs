using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;

namespace WBBBusinessLayer.QueryHandlers.FBBShareplex
{
    public class PatchSearchOrderQueryHandler : IQueryHandler<PatchSearchOrderQuery, List<RetPatchSearchOrders>>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IFBBShareplexEntityRepository<RetPatchSearchOrders> _queryProcessor;

        public PatchSearchOrderQueryHandler(ILogger logger, IWBBUnitOfWork uow
            , IFBBShareplexEntityRepository<RetPatchSearchOrders> queryProcessor)
        {
            _logger = logger;
            _uow = uow;
            _queryProcessor = queryProcessor;

        }
        public List<RetPatchSearchOrders> Handle(PatchSearchOrderQuery query)
        {
            var ret_code = new OracleParameter();
            ret_code.ParameterName = "ret_code";
            ret_code.OracleDbType = OracleDbType.Varchar2;
            ret_code.Size = 2000;
            ret_code.Direction = ParameterDirection.Output;

            var ret_msg = new OracleParameter();
            ret_msg.ParameterName = "ret_msg";
            ret_msg.OracleDbType = OracleDbType.Varchar2;
            ret_msg.Size = 2000;
            ret_msg.Direction = ParameterDirection.Output;

            var c_sn_list = new OracleParameter();
            c_sn_list.ParameterName = "c_sn_list";
            c_sn_list.OracleDbType = OracleDbType.RefCursor;
            c_sn_list.Direction = ParameterDirection.Output;

            var packageMappingObjectModel = new PackageMappingObjectModel
            {
                SN =
                     query.SerialNo.Select(
                         a => new SerialNumbers_Mapping
                         {
                             SN = a
                         }).ToArray()

            };

            var packageMapping = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_Serial_List", "FBBADM.SEARCH_ORDER_LIST", packageMappingObjectModel);


            var executeResult = _queryProcessor.ExecuteStoredProcMultipleCursor("FBBADM.PKG_FBB_PAYG_PATCH_SN.p_search_order",
                   new object[]
                   {
                       packageMapping,
                       //Return
                       c_sn_list,
                       ret_code,
                       ret_msg
                   }).ToList();
            List<RetPatchSearchOrders> retPatchSearchOrders = new List<RetPatchSearchOrders>();

            if (executeResult != null)
            {
                var result = (DataTable)executeResult[0];
                retPatchSearchOrders = result.DataTableToList<RetPatchSearchOrders>();
            }
            return retPatchSearchOrders;
        }


        #region PackageMappingObjectModel
        public class PackageMappingObjectModel : INullable, IOracleCustomType
        {
            [OracleArrayMapping()]
            public SerialNumbers_Mapping[] SN { get; set; }


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
                OracleUdt.SetValue(con, udt, 0, SN);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                SN = (SerialNumbers_Mapping[])OracleUdt.GetValue(con, udt, 0);
            }
        }

        [OracleCustomTypeMapping("FBBADM.SEARCH_ORDER_REC")]
        public class SerialNumbers_MappingOracleTypeMappingFactory : IOracleCustomTypeFactory
        {
            public IOracleCustomType CreateObject()
            {
                return new SerialNumbers_Mapping();
            }
        }

        [OracleCustomTypeMapping("FBBADM.SEARCH_ORDER_LIST")]
        public class SerialNumbers_MappingObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
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
                return new SerialNumbers_Mapping[numElems];
            }

            public Array CreateStatusArray(int numElems)
            {
                return null;
            }

            #endregion IOracleArrayTypeFactory Members
        }

        public class SerialNumbers_Mapping : INullable, IOracleCustomType
        {
            private bool objectIsNull;

            #region Attribute Mapping

            [OracleObjectMapping("SN")]
            public string SN { get; set; }
            #endregion Attribute Mapping

            public static SerialNumbers_Mapping Null
            {
                get
                {
                    var obj = new SerialNumbers_Mapping();
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
                OracleUdt.SetValue(con, udt, "SN", SN);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                throw new NotImplementedException();
            }
        }
        #endregion
    }
}
