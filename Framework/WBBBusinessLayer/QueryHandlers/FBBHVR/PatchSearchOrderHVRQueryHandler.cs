using Npgsql;
using NpgsqlTypes;
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
    public class PatchSearchOrderHVRQueryHandler : IQueryHandler<PatchSearchOrderHVRQuery, List<PatchSearchOrdersHVR>>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IFBBHVREntityRepository<object> _queryProcessor;

        public PatchSearchOrderHVRQueryHandler(ILogger logger, IWBBUnitOfWork uow
            , IFBBHVREntityRepository<object> queryProcessor)
        {
            _logger = logger;
            _uow = uow;
            _queryProcessor = queryProcessor;

        }
        public List<PatchSearchOrdersHVR> Handle(PatchSearchOrderHVRQuery query)
        {
            var ret_code_cur = new NpgsqlParameter();
            ret_code_cur.ParameterName = "ret_code_cur";
            ret_code_cur.NpgsqlDbType = NpgsqlDbType.Refcursor;
            ret_code_cur.Direction = ParameterDirection.InputOutput;
            ret_code_cur.Value = "ret_code_cur";


            var c_sn_list = new NpgsqlParameter();
            c_sn_list.ParameterName = "c_sn_list";
            c_sn_list.NpgsqlDbType = NpgsqlDbType.Refcursor;
            c_sn_list.Direction = ParameterDirection.InputOutput;
            c_sn_list.Value = "c_sn_list";


            var p_serial_list = new NpgsqlParameter();
            p_serial_list.ParameterName = "p_serial_list";
            p_serial_list.Direction = ParameterDirection.InputOutput;

            query.SerialNo2 = new List<PatchSearchOrdersHVR>();

            if (query.SerialNo != null)
            {
                foreach (var serialNo in query.SerialNo)
                {
                    var newPatchSearchOrder = new PatchSearchOrdersHVR
                    {
                        SN = serialNo,
                    };
                    query.SerialNo2.Add(newPatchSearchOrder);
                }

                p_serial_list.Value = query.SerialNo2.Select(
                                 a => new search_order_rec
                                 {
                                     sn = a.SN
                                 }).ToArray();
            }


            var executeResult = _queryProcessor.ExecuteStoredProcMultipleCursorNpgsql("fbbadm.pkg_fbb_payg_patch_sn_p_search_order",
                new object[]
                {
                    p_serial_list,
                    //Return
                  c_sn_list,
                  ret_code_cur
                }).ToList();



            List<PatchSearchOrdersHVR> patchSearchOrdersHVR = new List<PatchSearchOrdersHVR>();

            if (executeResult != null)
            {
                var result = (DataTable)executeResult[0];
                patchSearchOrdersHVR = result.DataTableToList<PatchSearchOrdersHVR>();
            }
            return patchSearchOrdersHVR;
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
