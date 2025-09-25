using Npgsql;
using NpgsqlTypes;
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
    public class CheckSerialStatusHVRQueryHandler : IQueryHandler<CheckSerialStatusHVRQuery, List<RetCheckSerialStatus>>
    {
        private readonly IFBBHVREntityRepository<object> _repo;
        public CheckSerialStatusHVRQueryHandler(IFBBHVREntityRepository<object> repo)
        {
            _repo = repo;
        }
        public List<RetCheckSerialStatus> Handle(CheckSerialStatusHVRQuery query)
        {
            List<RetCheckSerialStatus> ccc = new List<RetCheckSerialStatus>();

            var multi_ret_code = new NpgsqlParameter();
            multi_ret_code.ParameterName = "multi_ret_code";
            multi_ret_code.NpgsqlDbType = NpgsqlDbType.Refcursor;
            multi_ret_code.Direction = ParameterDirection.InputOutput;
            multi_ret_code.Value = "multi_ret_code";

            var cur = new NpgsqlParameter();
            cur.ParameterName = "cur";
            cur.NpgsqlDbType = NpgsqlDbType.Refcursor;
            cur.Direction = ParameterDirection.InputOutput;
            cur.Value = "cur";

            var p_serial_list = new NpgsqlParameter();
            p_serial_list.ParameterName = "p_serial_list";
            p_serial_list.Direction = ParameterDirection.Input;
            p_serial_list.Value = query.checkSerials.Select(
                 a => new patch_sn_rec
                 {
                     internet_no = a.INTERNET_NO,
                     sn = a.SERIAL_NUMBER,
                     status = a.SERIAL_STATUS,
                     foa_code = a.FOA_CODE,
                     created_date = a.CREATE_DATE,
                     posting_date = a.POST_DATE,
                     movement_type = a.MOVEMENT_TYPE,
                     location_code = a.LOCATION_CODE
                 }).ToArray();
            
            var executeResult = _repo.ExecuteStoredProcMultipleCursorNpgsql("fbbadm.pkg_fbb_payg_patch_sn_p_search_patch_sn",
                   new object[]
                   {
                       //List
                       p_serial_list,
                       
                       //Return
                       cur,
                       multi_ret_code
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

    public class GetProductListHVRQueryHandler : IQueryHandler<GetProductListHVRQuery, List<ProductListHVR>>
    {
        private readonly IFBBHVREntityRepository<ProductListHVR> _repo;
        public GetProductListHVRQueryHandler(IFBBHVREntityRepository<ProductListHVR> repo)
        {
            _repo = repo;
        }

        public List<ProductListHVR> Handle(GetProductListHVRQuery query)
        {
            var ret_code_cur = new NpgsqlParameter();
            ret_code_cur.ParameterName = "ret_code_cur";
            ret_code_cur.NpgsqlDbType = NpgsqlDbType.Refcursor;
            ret_code_cur.Direction = ParameterDirection.InputOutput;
            ret_code_cur.Value = "ret_code_cur";

            var c_product_cur = new NpgsqlParameter();
            c_product_cur.ParameterName = "c_product_cur";
            c_product_cur.NpgsqlDbType = NpgsqlDbType.Refcursor;
            c_product_cur.Direction = ParameterDirection.InputOutput;
            c_product_cur.Value = "c_product_cur";

            var p_internet_no = new NpgsqlParameter();
            p_internet_no.ParameterName = "p_internet_no";
            p_internet_no.Direction = ParameterDirection.Input;
            p_internet_no.Value = query.INTERNET_NO;
            p_internet_no.NpgsqlDbType = NpgsqlDbType.Varchar;

            var p_serial_no = new NpgsqlParameter();
            p_serial_no.ParameterName = "p_serial_no";
            p_serial_no.Direction = ParameterDirection.Input;
            p_serial_no.Value = query.SERIAL_NO;
            p_serial_no.NpgsqlDbType = NpgsqlDbType.Varchar;

            var p_movement_type = new NpgsqlParameter();
            p_movement_type.ParameterName = "p_movement_type";
            p_movement_type.Direction = ParameterDirection.Input;
            p_movement_type.Value = query.MOVEMENT_TYPE;
            p_movement_type.NpgsqlDbType = NpgsqlDbType.Varchar;


            var executeResult = _repo.ExecuteStoredProcMultipleCursorNpgsql("fbbadm.pkg_fbb_payg_patch_sn_p_get_product_list",
                new object[]
                {
                    p_internet_no,
                    p_serial_no,
                    p_movement_type,

                    //Return
                    c_product_cur,
                    ret_code_cur
                });

            if (executeResult != null)
            {
                DataTable resp = (DataTable)executeResult[0];
                List<ProductListHVR> respList = resp.DataTableToList<ProductListHVR>();
                return respList;
            }
            else
            {
                return new List<ProductListHVR>();
            }
        }
    }
}
