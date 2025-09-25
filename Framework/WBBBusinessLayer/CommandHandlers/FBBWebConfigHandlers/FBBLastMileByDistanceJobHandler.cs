using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBData.Repository;
using WBBEntity.Extensions;

namespace WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers
{
    public class FBBLastMileByDistanceJobHandler : ICommandHandler<FBBLastMileByDistanceJobCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;

        public FBBLastMileByDistanceJobHandler(IEntityRepository<string> objService, ILogger logger)
        {
            _logger = logger;
            _objService = objService;
        }

        public void Handle(FBBLastMileByDistanceJobCommand command)
        {
            var packageMappingObjectModel = new PackageMappingObjectLastMileModel
            {
                P_LIST_FIXED_OM0101 =
                       command.P_LIST_FIXED_OM0101.Select(
                          a => new FBB_FIXED_OM010_LISTMapping
                          {
                              ACC_NBR = a.ACC_NBR,
                              USER_NAME = a.USER_NAME,
                              SBC_CPY = a.SBC_CPY,
                              PRODUCT_NAME = a.PRODUCT_NAME,
                              ON_TOP1 = a.ON_TOP1,
                              ON_TOP2 = a.ON_TOP2,
                              VOIP_NUMBER = a.VOIP_NUMBER,
                              SERVICE_PACK_NAME = a.SERVICE_PACK_NAME,
                              ORD_NO = a.ORD_NO,
                              ORD_TYPE = a.ORD_TYPE,
                              ORDER_SFF = a.ORDER_SFF,
                              APPOINTMENT_DATE = a.APPOINTMENT_DATE,
                              SFF_ACTIVE_DATE = a.SFF_ACTIVE_DATE,
                              APPROVE_JOB_FBSS_DATE = a.APPROVE_JOB_FBSS_DATE,
                              COMPLETED_DATE = a.COMPLETED_DATE,
                              ORDER_STATUS = a.ORDER_STATUS,
                              REJECT_REASON = a.REJECT_REASON,
                              MATERIAL_CODE_CPESN = a.MATERIAL_CODE_CPESN,
                              CPE_SN = a.CPE_SN,
                              CPE_MODE = a.CPE_MODE,
                              MATERIAL_CODE_STBSN = a.MATERIAL_CODE_STBSN,
                              STB_SN = a.STB_SN,
                              MATERIAL_CODE_ATASN = a.MATERIAL_CODE_ATASN,
                              ATA_SN = a.ATA_SN,
                              MATERIAL_CODE_WIFIROUTESN = a.MATERIAL_CODE_WIFIROUTESN,
                              WIFI_ROUTER_SN = a.WIFI_ROUTER_SN,
                              STO_LOCATION = a.STO_LOCATION,
                              VENDOR_CODE = a.VENDOR_CODE,
                              FOA_REJECT_REASON = a.FOA_REJECT_REASON,
                              RE_APPOINTMENT_REASON = a.RE_APPOINTMENT_REASON,
                              PHASE_PO = a.PHASE_PO,
                              SFF_SUBMITTED_DATE = a.SFF_SUBMITTED_DATE,
                              EVENT_CODE = a.EVENT_CODE,
                              REGION = a.REGION,
                              TOTAL_FEE = a.TOTAL_FEE,
                              FEE_CODE = a.FEE_CODE,
                              ADDR_ID = a.ADDR_ID,
                              ADDR_NAME_TH = a.ADDR_NAME_TH,
                              TRANSFER_DATE = a.TRANSFER_DATE,
                              FILE_NAME = a.FILE_NAME,
                              TOTAL_ROW = a.TOTAL_ROW,
                              USER_CODE = a.USER_CODE

                          }).ToArray()
            };
            var ret_code = new OracleParameter();
            ret_code.OracleDbType = OracleDbType.Int32;
            ret_code.Direction = ParameterDirection.Output;

            var ret_msg = new OracleParameter();
            ret_msg.OracleDbType = OracleDbType.Varchar2;
            ret_msg.Size = 2000;
            ret_msg.Direction = ParameterDirection.Output;

            var outp = new List<object>();
            var paramOut = outp.ToArray();

            var P_LIST_FIXED_OM0101 = OracleCustomTypeUtilities.CreateUDTArrayParameter("P_LIST_FIXED_OM0101", "FBB_FIXED_OM010_LIST", packageMappingObjectModel);

            try
            {
                var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FBBPAYG_LOAD_OM010.P_INSERT_UPDATE_OM",
                   out paramOut,
                   new
                   {
                       P_LIST_FIXED_OM0101,
                       P_RET_MSG = ret_msg,
                       P_RET_CODE = ret_code
                   });

                command.ret_code = ret_code.Value.ToSafeString();
                command.ret_msg = ret_msg.Value.ToSafeString();

            }
            catch (Exception ex)
            {
                command.ret_code = "2";
                command.ret_msg = ex.Message;
                _logger.Info(ex.GetErrorMessage());
            }



        }
    }
    #region Mapping FBB_LastMileByDistanceJob_LIST Type Oracle
    public class PackageMappingObjectLastMileModel : INullable, IOracleCustomType
    {
        [OracleArrayMapping()]
        public FBB_FIXED_OM010_LISTMapping[] P_LIST_FIXED_OM0101 { get; set; }

        private bool objectIsNull;
        public bool IsNull
        {
            get { return objectIsNull; }
        }

        public static PackageMappingObjectLastMileModel Null
        {
            get
            {
                var obj = new PackageMappingObjectLastMileModel();
                obj.objectIsNull = true;
                return obj;
            }
        }

        public void FromCustomObject(OracleConnection con, object udt)
        {
            OracleUdt.SetValue(con, udt, 0, P_LIST_FIXED_OM0101);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            P_LIST_FIXED_OM0101 = (FBB_FIXED_OM010_LISTMapping[])OracleUdt.GetValue(con, udt, 0);
        }
    }

    [OracleCustomTypeMapping("FBB_FIXED_OM010_REC")]
    public class FBB_FIXED_OM010_LIST_MappingOracleTypeMappingFactory : IOracleCustomTypeFactory
    {
        public IOracleCustomType CreateObject()
        {
            return new FBB_FIXED_OM010_LISTMapping();
        }
    }
    [OracleCustomTypeMapping("FBB_FIXED_OM010_LIST")]
    public class FBB_FIXED_OM010_LISTMappingObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
    {
        #region IOracleCustomTypeFactory Members

        public IOracleCustomType CreateObject()
        {
            return new PackageMappingObjectLastMileModel();
        }

        #endregion IOracleCustomTypeFactory Members

        #region IOracleArrayTypeFactory Members

        public Array CreateArray(int numElems)
        {
            return new FBB_FIXED_OM010_LISTMapping[numElems];
        }

        public Array CreateStatusArray(int numElems)
        {
            return null;
        }

        #endregion IOracleArrayTypeFactory Members
    }
    public class FBB_FIXED_OM010_LISTMapping : INullable, IOracleCustomType
    {
        private bool objectIsNull;

        #region Attribute Mapping    

        [OracleObjectMapping("ACC_NBR")]
        public string ACC_NBR { get; set; }

        [OracleObjectMapping("USER_NAME")]
        public string USER_NAME { get; set; }

        [OracleObjectMapping("SBC_CPY")]
        public string SBC_CPY { get; set; }

        [OracleObjectMapping("PRODUCT_NAME")]
        public string PRODUCT_NAME { get; set; }

        [OracleObjectMapping("ON_TOP1")]
        public string ON_TOP1 { get; set; }

        [OracleObjectMapping("ON_TOP2")]
        public string ON_TOP2 { get; set; }

        [OracleObjectMapping("VOIP_NUMBER")]
        public string VOIP_NUMBER { get; set; }

        [OracleObjectMapping("SERVICE_PACK_NAME")]
        public string SERVICE_PACK_NAME { get; set; }

        [OracleObjectMapping("ORD_NO")]
        public string ORD_NO { get; set; }

        [OracleObjectMapping("ORD_TYPE")]
        public string ORD_TYPE { get; set; }

        [OracleObjectMapping("ORDER_SFF")]
        public string ORDER_SFF { get; set; }

        [OracleObjectMapping("APPOINTMENT_DATE")]
        public DateTime? APPOINTMENT_DATE { get; set; }

        [OracleObjectMapping("SFF_ACTIVE_DATE")]
        public DateTime? SFF_ACTIVE_DATE { get; set; }

        [OracleObjectMapping("APPROVE_JOB_FBSS_DATE")]
        public DateTime? APPROVE_JOB_FBSS_DATE { get; set; }

        [OracleObjectMapping("COMPLETED_DATE")]
        public DateTime? COMPLETED_DATE { get; set; }

        [OracleObjectMapping("ORDER_STATUS")]
        public string ORDER_STATUS { get; set; }

        [OracleObjectMapping("REJECT_REASON")]
        public string REJECT_REASON { get; set; }

        [OracleObjectMapping("MATERIAL_CODE_CPESN")]
        public string MATERIAL_CODE_CPESN { get; set; }

        [OracleObjectMapping("CPE_SN")]
        public string CPE_SN { get; set; }

        [OracleObjectMapping("CPE_MODE")]
        public string CPE_MODE { get; set; }

        [OracleObjectMapping("MATERIAL_CODE_STBSN")]
        public string MATERIAL_CODE_STBSN { get; set; }

        [OracleObjectMapping("STB_SN")]
        public string STB_SN { get; set; }

        [OracleObjectMapping("MATERIAL_CODE_ATASN")]
        public string MATERIAL_CODE_ATASN { get; set; }

        [OracleObjectMapping("ATA_SN")]
        public string ATA_SN { get; set; }

        [OracleObjectMapping("MATERIAL_CODE_WIFIROUTESN")]
        public string MATERIAL_CODE_WIFIROUTESN { get; set; }

        [OracleObjectMapping("WIFI_ROUTER_SN")]
        public string WIFI_ROUTER_SN { get; set; }

        [OracleObjectMapping("STO_LOCATION")]
        public string STO_LOCATION { get; set; }

        [OracleObjectMapping("VENDOR_CODE")]
        public string VENDOR_CODE { get; set; }

        [OracleObjectMapping("FOA_REJECT_REASON")]
        public string FOA_REJECT_REASON { get; set; }

        [OracleObjectMapping("RE_APPOINTMENT_REASON")]
        public string RE_APPOINTMENT_REASON { get; set; }

        [OracleObjectMapping("PHASE_PO")]
        public string PHASE_PO { get; set; }

        [OracleObjectMapping("SFF_SUBMITTED_DATE")]
        public DateTime? SFF_SUBMITTED_DATE { get; set; }

        [OracleObjectMapping("EVENT_CODE")]
        public string EVENT_CODE { get; set; }

        [OracleObjectMapping("REGION")]
        public string REGION { get; set; }

        [OracleObjectMapping("TOTAL_FEE")]
        public decimal? TOTAL_FEE { get; set; }

        [OracleObjectMapping("FEE_CODE")]
        public string FEE_CODE { get; set; }

        [OracleObjectMapping("ADDR_ID")]
        public string ADDR_ID { get; set; }

        [OracleObjectMapping("ADDR_NAME_TH")]
        public string ADDR_NAME_TH { get; set; }

        [OracleObjectMapping("TRANSFER_DATE")]
        public DateTime? TRANSFER_DATE { get; set; }

        [OracleObjectMapping("FILE_NAME")]
        public string FILE_NAME { get; set; }

        [OracleObjectMapping("TOTAL_ROW")]
        public decimal? TOTAL_ROW { get; set; }

        [OracleObjectMapping("USER_CODE")]
        public string USER_CODE { get; set; }

        #endregion Attribute Mapping

        public static FBB_FIXED_OM010_LISTMapping Null
        {
            get
            {
                var obj = new FBB_FIXED_OM010_LISTMapping();
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
            OracleUdt.SetValue(con, udt, "ACC_NBR", ACC_NBR);
            OracleUdt.SetValue(con, udt, "USER_NAME", USER_NAME);
            OracleUdt.SetValue(con, udt, "SBC_CPY", SBC_CPY);
            OracleUdt.SetValue(con, udt, "PRODUCT_NAME", PRODUCT_NAME);
            OracleUdt.SetValue(con, udt, "ON_TOP1", ON_TOP1);
            OracleUdt.SetValue(con, udt, "ON_TOP2", ON_TOP2);
            OracleUdt.SetValue(con, udt, "VOIP_NUMBER", VOIP_NUMBER);
            OracleUdt.SetValue(con, udt, "SERVICE_PACK_NAME", SERVICE_PACK_NAME);
            OracleUdt.SetValue(con, udt, "ORD_NO", ORD_NO);
            OracleUdt.SetValue(con, udt, "ORD_TYPE", ORD_TYPE);
            OracleUdt.SetValue(con, udt, "ORDER_SFF", ORDER_SFF);
            OracleUdt.SetValue(con, udt, "APPOINTMENT_DATE", APPOINTMENT_DATE);
            OracleUdt.SetValue(con, udt, "SFF_ACTIVE_DATE", SFF_ACTIVE_DATE);
            OracleUdt.SetValue(con, udt, "APPROVE_JOB_FBSS_DATE", APPROVE_JOB_FBSS_DATE);
            OracleUdt.SetValue(con, udt, "COMPLETED_DATE", COMPLETED_DATE);
            OracleUdt.SetValue(con, udt, "ORDER_STATUS", ORDER_STATUS);
            OracleUdt.SetValue(con, udt, "REJECT_REASON", REJECT_REASON);
            OracleUdt.SetValue(con, udt, "MATERIAL_CODE_CPESN", MATERIAL_CODE_CPESN);
            OracleUdt.SetValue(con, udt, "CPE_SN", CPE_SN);
            OracleUdt.SetValue(con, udt, "CPE_MODE", CPE_MODE);
            OracleUdt.SetValue(con, udt, "MATERIAL_CODE_STBSN", MATERIAL_CODE_STBSN);
            OracleUdt.SetValue(con, udt, "STB_SN", STB_SN);
            OracleUdt.SetValue(con, udt, "MATERIAL_CODE_ATASN", MATERIAL_CODE_ATASN);
            OracleUdt.SetValue(con, udt, "ATA_SN", ATA_SN);
            OracleUdt.SetValue(con, udt, "MATERIAL_CODE_WIFIROUTESN", MATERIAL_CODE_WIFIROUTESN);
            OracleUdt.SetValue(con, udt, "WIFI_ROUTER_SN", WIFI_ROUTER_SN);
            OracleUdt.SetValue(con, udt, "STO_LOCATION", STO_LOCATION);
            OracleUdt.SetValue(con, udt, "VENDOR_CODE", VENDOR_CODE);
            OracleUdt.SetValue(con, udt, "FOA_REJECT_REASON", FOA_REJECT_REASON);
            OracleUdt.SetValue(con, udt, "RE_APPOINTMENT_REASON", RE_APPOINTMENT_REASON);
            OracleUdt.SetValue(con, udt, "PHASE_PO", PHASE_PO);
            OracleUdt.SetValue(con, udt, "SFF_SUBMITTED_DATE", SFF_SUBMITTED_DATE);
            OracleUdt.SetValue(con, udt, "EVENT_CODE", EVENT_CODE);
            OracleUdt.SetValue(con, udt, "REGION", REGION);
            OracleUdt.SetValue(con, udt, "TOTAL_FEE", TOTAL_FEE);
            OracleUdt.SetValue(con, udt, "FEE_CODE", FEE_CODE);
            OracleUdt.SetValue(con, udt, "ADDR_ID", ADDR_ID);
            OracleUdt.SetValue(con, udt, "ADDR_NAME_TH", ADDR_NAME_TH);
            OracleUdt.SetValue(con, udt, "TRANSFER_DATE", TRANSFER_DATE);
            OracleUdt.SetValue(con, udt, "FILE_NAME", FILE_NAME);
            OracleUdt.SetValue(con, udt, "TOTAL_ROW", TOTAL_ROW);
            OracleUdt.SetValue(con, udt, "USER_CODE", USER_CODE);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            #region test
            //ACC_NBR = (string)OracleUdt.GetValue(con, udt, "ACC_NBR");
            //USER_NAME = (string)OracleUdt.GetValue(con, udt, "USER_NAME");
            //SBC_CPY = (string)OracleUdt.GetValue(con, udt, "SBC_CPY");
            //PRODUCT_NAME = (string)OracleUdt.GetValue(con, udt, "PRODUCT_NAME");
            //ON_TOP1 = (string)OracleUdt.GetValue(con, udt, "ON_TOP1");
            //ON_TOP2 = (string)OracleUdt.GetValue(con, udt, "ON_TOP2");
            //VOIP_NUMBER = (string)OracleUdt.GetValue(con, udt, "VOIP_NUMBER");
            //SERVICE_PACK_NAME = (string)OracleUdt.GetValue(con, udt, "SERVICE_PACK_NAME");
            //ORD_NO = (string)OracleUdt.GetValue(con, udt, "ORD_NO");
            //ORD_TYPE = (string)OracleUdt.GetValue(con, udt, "ORD_TYPE");
            //ORDER_SFF = (string)OracleUdt.GetValue(con, udt, "ORDER_SFF");
            //APPOINTMENT_DATE = (DateTime)OracleUdt.GetValue(con, udt, "APPOINTMENT_DATE");
            //SFF_ACTIVE_DATE = (DateTime)OracleUdt.GetValue(con, udt, "SFF_ACTIVE_DATE");
            //APPROVE_JOB_FBSS_DATE = (DateTime)OracleUdt.GetValue(con, udt, "APPROVE_JOB_FBSS_DATE");
            //COMPLETED_DATE = (DateTime)OracleUdt.GetValue(con, udt, "COMPLETED_DATE");
            //ORDER_STATUS = (string)OracleUdt.GetValue(con, udt, "ORDER_STATUS");
            //REJECT_REASON = (string)OracleUdt.GetValue(con, udt, "REJECT_REASON");
            //MATERIAL_CODE_CPESN = (string)OracleUdt.GetValue(con, udt, "MATERIAL_CODE_CPESN");
            //CPE_SN = (string)OracleUdt.GetValue(con, udt, "CPE_SN");
            //CPE_MODE = (string)OracleUdt.GetValue(con, udt, "CPE_MODE");
            //MATERIAL_CODE_STBSN = (string)OracleUdt.GetValue(con, udt, "MATERIAL_CODE_STBSN");
            //STB_SN = (string)OracleUdt.GetValue(con, udt, "STB_SN");
            //MATERIAL_CODE_ATASN = (string)OracleUdt.GetValue(con, udt, "MATERIAL_CODE_ATASN");
            //ATA_SN = (string)OracleUdt.GetValue(con, udt, "ATA_SN");
            //MATERIAL_CODE_WIFIROUTESN = (string)OracleUdt.GetValue(con, udt, "MATERIAL_CODE_WIFIROUTESN");
            //WIFI_ROUTER_SN = (string)OracleUdt.GetValue(con, udt, "WIFI_ROUTER_SN");
            //STO_LOCATION = (string)OracleUdt.GetValue(con, udt, "STO_LOCATION");
            //VENDOR_CODE = (string)OracleUdt.GetValue(con, udt, "VENDOR_CODE");
            //FOA_REJECT_REASON = (string)OracleUdt.GetValue(con, udt, "FOA_REJECT_REASON");
            //RE_APPOINTMENT_REASON = (string)OracleUdt.GetValue(con, udt, "RE_APPOINTMENT_REASON");
            //PHASE_PO = (string)OracleUdt.GetValue(con, udt, "PHASE_PO");
            //SFF_SUBMITTED_DATE = (DateTime)OracleUdt.GetValue(con, udt, "SFF_SUBMITTED_DATE");
            //EVENT_CODE = (string)OracleUdt.GetValue(con, udt, "EVENT_CODE");
            //REGION = (string)OracleUdt.GetValue(con, udt, "REGION");
            //TOTAL_FEE = (decimal)OracleUdt.GetValue(con, udt, "TOTAL_FEE");
            //FEE_CODE = (string)OracleUdt.GetValue(con, udt, "FEE_CODE");
            //ADDR_ID = (string)OracleUdt.GetValue(con, udt, "ADDR_ID");
            //ADDR_NAME_TH = (string)OracleUdt.GetValue(con, udt, "ADDR_NAME_TH");
            //TRANSFER_DATE = (DateTime)OracleUdt.GetValue(con, udt, "TRANSFER_DATE");
            //TOTAL_ROW = (int)OracleUdt.GetValue(con, udt, "TOTAL_ROW");
            #endregion

            throw new NotImplementedException();
        }
    }
    #endregion Mapping FBB_LastMileByDistanceJob_LIST Type Oracle
}
