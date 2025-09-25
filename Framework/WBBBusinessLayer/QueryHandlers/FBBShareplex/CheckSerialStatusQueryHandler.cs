using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.PatchEquipment;
using WBBData.Repository;
using WBBEntity.Extensions;

namespace WBBBusinessLayer.QueryHandlers.FBBShareplex
{
    public class CheckSerialStatusQueryHandler : IQueryHandler<CheckSerialStatusQuery, List<RetCheckSerialStatus>>
    {
        private readonly IFBBShareplexEntityRepository<object> _repo;
        public CheckSerialStatusQueryHandler(IFBBShareplexEntityRepository<object> repo)
        {
            _repo = repo;
        }

        public List<RetCheckSerialStatus> Handle(CheckSerialStatusQuery query)
        {

            //"FBBADM.PKG_FBB_PAYG_PATCH_SN.p_search_patch_sn"
            //var stringQuery = $"select state from oam.TSP_EQUIPMENT where bar_code = '{query.checkSerials}';";
            // var xxx =_repo.SqlQuery(stringQuery);
            List<RetCheckSerialStatus> ccc = new List<RetCheckSerialStatus>();

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

            var cur = new OracleParameter();
            cur.ParameterName = "cur";
            cur.OracleDbType = OracleDbType.RefCursor;
            cur.Direction = ParameterDirection.Output;

            var packageMappingObjectModel = new PackageMappingObjectModel
            {
                CheckSerialStatusList =
                     query.checkSerials.Select(
                         a => new CheckSerialStatusList_Mapping
                         {
                             INTERNET_NO = a.INTERNET_NO,
                             SN = a.SERIAL_NUMBER,
                             STATUS = a.SERIAL_STATUS,
                             FOA_CODE = a.FOA_CODE,
                             CREATED_DATE = a.CREATE_DATE,
                             POSTING_DATE = a.POST_DATE,
                             MOVEMENT_TYPE = a.MOVEMENT_TYPE,
                             LOCATION_CODE = a.LOCATION_CODE
                         }).ToArray()
            };
            var packageMapping = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_Serial_List", "FBBADM.PATCH_SN_LIST", packageMappingObjectModel);

            var executeResult = _repo.ExecuteStoredProcMultipleCursor("FBBADM.PKG_FBB_PAYG_PATCH_SN.p_search_patch_sn",
                   new object[]
                   {
                       //List
                       packageMapping,

                       //Return
                       cur,
                       ret_code,
                       ret_msg
                   });

            if (executeResult != null)
            {
                DataTable resp = (DataTable)executeResult[0];
                List<RetCheckSerialStatus> respList = resp.DataTableToList<RetCheckSerialStatus>();
                return respList;
            }
            else
            {
                return new List<RetCheckSerialStatus>();
            }

        }


        #region PackageMappingObjectModel
        public class PackageMappingObjectModel : INullable, IOracleCustomType
        {
            [OracleArrayMapping()]
            public CheckSerialStatusList_Mapping[] CheckSerialStatusList { get; set; }


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
                OracleUdt.SetValue(con, udt, 0, CheckSerialStatusList);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                CheckSerialStatusList = (CheckSerialStatusList_Mapping[])OracleUdt.GetValue(con, udt, 0);
            }
        }

        [OracleCustomTypeMapping("FBBADM.PATCH_SN_REC")]
        public class CheckSerialStatusList_MappingOracleTypeMappingFactory : IOracleCustomTypeFactory
        {
            public IOracleCustomType CreateObject()
            {
                return new CheckSerialStatusList_Mapping();
            }
        }

        [OracleCustomTypeMapping("FBBADM.PATCH_SN_LIST")]
        public class CheckSerialStatusList_MappingObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
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
                return new CheckSerialStatusList_Mapping[numElems];
            }

            public Array CreateStatusArray(int numElems)
            {
                return null;
            }

            #endregion IOracleArrayTypeFactory Members
        }

        public class CheckSerialStatusList_Mapping : INullable, IOracleCustomType
        {
            private bool objectIsNull;

            #region Attribute Mapping

            [OracleObjectMapping("INTERNET_NO")]
            public string INTERNET_NO { get; set; }

            [OracleObjectMapping("SN")]
            public string SN { get; set; }

            [OracleObjectMapping("STATUS")]
            public string STATUS { get; set; }

            [OracleObjectMapping("FOA_CODE")]
            public string FOA_CODE { get; set; }

            [OracleObjectMapping("CREATED_DATE")]
            public string CREATED_DATE { get; set; }

            [OracleObjectMapping("POSTING_DATE")]
            public string POSTING_DATE { get; set; }

            [OracleObjectMapping("MOVEMENT_TYPE")]
            public string MOVEMENT_TYPE { get; set; }

            [OracleObjectMapping("LOCATION_CODE")]
            public string LOCATION_CODE { get; set; }

            #endregion Attribute Mapping

            public static CheckSerialStatusList_Mapping Null
            {
                get
                {
                    var obj = new CheckSerialStatusList_Mapping();
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
                OracleUdt.SetValue(con, udt, "INTERNET_NO", INTERNET_NO);
                OracleUdt.SetValue(con, udt, "SN", SN);
                OracleUdt.SetValue(con, udt, "STATUS", STATUS);
                OracleUdt.SetValue(con, udt, "FOA_CODE", FOA_CODE);
                OracleUdt.SetValue(con, udt, "CREATED_DATE", CREATED_DATE);
                OracleUdt.SetValue(con, udt, "POSTING_DATE", POSTING_DATE);
                OracleUdt.SetValue(con, udt, "MOVEMENT_TYPE", MOVEMENT_TYPE);
                OracleUdt.SetValue(con, udt, "LOCATION_CODE", LOCATION_CODE);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                throw new NotImplementedException();
            }
        }
        #endregion
    }

    public class GetProductListSharePlexQueryHandler : IQueryHandler<GetProductListSharePlexQuery, List<ProductListSharePlex>>
    {
        private readonly IFBBShareplexEntityRepository<ProductListSharePlex> _repo;
        public GetProductListSharePlexQueryHandler(IFBBShareplexEntityRepository<ProductListSharePlex> repo)
        {
            _repo = repo;
        }

        public List<ProductListSharePlex> Handle(GetProductListSharePlexQuery query)
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

            var c_product_cur = new OracleParameter();
            c_product_cur.ParameterName = "c_product_cur";
            c_product_cur.OracleDbType = OracleDbType.RefCursor;
            c_product_cur.Direction = ParameterDirection.Output;

            var executeResult = _repo.ExecuteReadStoredProc("FBBADM.PKG_FBB_PAYG_PATCH_SN.p_get_product_list",
                   new
                   {
                       p_INTERNET_NO = query.INTERNET_NO,
                       p_SERIAL_NO = query.SERIAL_NO,
                       p_MOVEMENT_TYPE = query.MOVEMENT_TYPE,

                       //Return
                       c_product_cur,
                       ret_code,
                       ret_msg
                   }).ToList();
            return executeResult;
        }
    }
}
