using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetMeshCompareDeviceQueryHandler : IQueryHandler<GetMeshCompareDeviceQuery, MeshCompareDeviceModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<object> _objService;

        public GetMeshCompareDeviceQueryHandler(ILogger logger, IWBBUnitOfWork uow, IEntityRepository<FBB_INTERFACE_LOG> intfLog, IEntityRepository<object> objService)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _objService = objService;
        }

        public MeshCompareDeviceModel Handle(GetMeshCompareDeviceQuery query)
        {
            MeshCompareDeviceModel executeResults = new MeshCompareDeviceModel();
            var log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.FibrenetID, "GetMeshCompareDeviceQuery", "FBBService", null, "FBB", "");

            try
            {

                var P_FIBRENET_ID = new OracleParameter();
                P_FIBRENET_ID.ParameterName = "p_fibrenet_id";
                P_FIBRENET_ID.Size = 2000;
                P_FIBRENET_ID.OracleDbType = OracleDbType.Varchar2;
                P_FIBRENET_ID.Direction = ParameterDirection.Input;
                P_FIBRENET_ID.Value = query.FibrenetID;

                var P_CHANNEL = new OracleParameter();
                P_CHANNEL.ParameterName = "p_channel";
                P_CHANNEL.Size = 2000;
                P_CHANNEL.OracleDbType = OracleDbType.Varchar2;
                P_CHANNEL.Direction = ParameterDirection.Input;
                P_CHANNEL.Value = query.Channel;

                var P_FLAG_PENALTY = new OracleParameter();
                P_FLAG_PENALTY.ParameterName = "p_flag_penalty";
                P_FLAG_PENALTY.Size = 2000;
                P_FLAG_PENALTY.OracleDbType = OracleDbType.Varchar2;
                P_FLAG_PENALTY.Direction = ParameterDirection.Input;
                P_FLAG_PENALTY.Value = query.flagPenalty;

                var P_FLAG_LANG = new OracleParameter();
                P_FLAG_LANG.ParameterName = "p_flag_lang";
                P_FLAG_LANG.Size = 2000;
                P_FLAG_LANG.OracleDbType = OracleDbType.Varchar2;
                P_FLAG_LANG.Direction = ParameterDirection.Input;
                P_FLAG_LANG.Value = query.lang;

                var P_REGISTER_DATE = new OracleParameter();
                P_REGISTER_DATE.ParameterName = "p_register_date";
                P_REGISTER_DATE.Size = 2000;
                P_REGISTER_DATE.OracleDbType = OracleDbType.Varchar2;
                P_REGISTER_DATE.Direction = ParameterDirection.Input;
                P_REGISTER_DATE.Value = query.RegisterDate;

                var P_COUNT_CONTRACT_FBB = new OracleParameter();
                P_COUNT_CONTRACT_FBB.ParameterName = "p_count_contract_fbb";
                P_COUNT_CONTRACT_FBB.Size = 2000;
                P_COUNT_CONTRACT_FBB.OracleDbType = OracleDbType.Varchar2;
                P_COUNT_CONTRACT_FBB.Direction = ParameterDirection.Input;
                P_COUNT_CONTRACT_FBB.Value = query.CountContractFbb;

                var P_CONTRACT_FLAG_FBB = new OracleParameter();
                P_CONTRACT_FLAG_FBB.ParameterName = "p_contract_flag_fbb";
                P_CONTRACT_FLAG_FBB.Size = 2000;
                P_CONTRACT_FLAG_FBB.OracleDbType = OracleDbType.Varchar2;
                P_CONTRACT_FLAG_FBB.Direction = ParameterDirection.Input;
                P_CONTRACT_FLAG_FBB.Value = query.ContractFlagFbb;

                var P_FBB_LIMIT_CONTRACT = new OracleParameter();
                P_FBB_LIMIT_CONTRACT.ParameterName = "p_fbb_limit_contract";
                P_FBB_LIMIT_CONTRACT.Size = 2000;
                P_FBB_LIMIT_CONTRACT.OracleDbType = OracleDbType.Varchar2;
                P_FBB_LIMIT_CONTRACT.Direction = ParameterDirection.Input;
                P_FBB_LIMIT_CONTRACT.Value = query.FBBLimitContract;

                var compareModel = new CompareModel();

                compareModel.REC_REG_PACKAGE = query.CompareArray.Select(c => new Rec_Reg_PackageOracleTypeMapping
                {
                    CPE_TYPE = c.CPE_TYPE.ToSafeString(),
                    CPE_MODEL_NAME = c.CPE_MODEL_NAME.ToSafeString(),
                    STATUS_DESC = c.STATUS_DESC.ToSafeString(),
                    CPE_BRAND_NAME = c.CPE_BRAND_NAME.ToSafeString(),
                    CPE_MODEL_ID = c.CPE_MODEL_ID.ToSafeString(),
                    CPE_GROUP_TYPE = c.CPE_GROUP_TYPE.ToSafeString(),
                    SN_PATTERN = c.SN_PATTERN.ToSafeString()
                }).ToArray();

                var P_FBBOR041_COMPARE_ARRAY = OracleCustomTypeUtilities.CreateUDTArrayParameter("P_FBBOR041_COMPARE_ARRAY", "FBBOR041_COMPARE_CPE_ARRAY", compareModel);

                var RETURN_CODE = new OracleParameter();
                RETURN_CODE.ParameterName = "return_code";
                RETURN_CODE.OracleDbType = OracleDbType.Decimal;
                RETURN_CODE.Direction = ParameterDirection.Output;

                var RETURN_MESSAGE = new OracleParameter();
                RETURN_MESSAGE.ParameterName = "return_message";
                RETURN_MESSAGE.Size = 2000;
                RETURN_MESSAGE.OracleDbType = OracleDbType.Varchar2;
                RETURN_MESSAGE.Direction = ParameterDirection.Output;

                var RETURN_CALL = new OracleParameter();
                RETURN_CALL.ParameterName = "return_compare_curror";
                RETURN_CALL.OracleDbType = OracleDbType.RefCursor;
                RETURN_CALL.Direction = ParameterDirection.Output;

                _logger.Info("Start PKG_FBBOR041.PACKAGE_PARAMETER");

                var result = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBBOR041.COMPARE_DEVICE",
                    new object[]
                    {
                         P_FIBRENET_ID,
                         P_CHANNEL,
                         P_FLAG_PENALTY,
                         P_FLAG_LANG,
                         P_REGISTER_DATE,
                         P_COUNT_CONTRACT_FBB,
                         P_CONTRACT_FLAG_FBB,
                         P_FBB_LIMIT_CONTRACT,
                         P_FBBOR041_COMPARE_ARRAY,
                         //return code
                         RETURN_CODE,
                         RETURN_MESSAGE,
                         RETURN_CALL
                    });
                executeResults.RETURN_CODE = result[0] != null ? result[0].ToSafeString() : "-1";
                executeResults.RETURN_MESSAGE = result[1] != null ? result[1].ToSafeString() : "error";
                if (executeResults.RETURN_CODE != "-1")
                {
                    DataTable data1 = (DataTable)result[2];
                    executeResults.RES_COMPLETE_CUR = data1.DataTableToList<CompareDevice>();
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResults.RES_COMPLETE_CUR, log, "Success", "", "");
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResults.RETURN_MESSAGE, log, "Failed", executeResults.RETURN_MESSAGE, "");
                }

            }
            catch (Exception ex)
            {
                _logger.Info("Error call PKG_FBBOR041.CompareDevice handles : " + ex.Message);

                executeResults.RETURN_CODE = "-1";
                executeResults.RETURN_MESSAGE = "Error";
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex.Message, log, "Failed", ex.StackTrace, "");

                return null;
            }
            return executeResults;
        }
    }

    #region Mapping REC_REG_PACKAGE Type Oracle
    public class CompareModel : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
    {
        [OracleArrayMapping()]
        public Rec_Reg_PackageOracleTypeMapping[] REC_REG_PACKAGE { get; set; }

        private bool objectIsNull;

        public bool IsNull
        {
            get { return objectIsNull; }
        }

        public static CompareModel Null
        {
            get
            {
                CompareModel obj = new CompareModel();
                obj.objectIsNull = true;
                return obj;
            }
        }

        public void FromCustomObject(OracleConnection con, object udt)
        {
            OracleUdt.SetValue(con, udt, 0, REC_REG_PACKAGE);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            REC_REG_PACKAGE = (Rec_Reg_PackageOracleTypeMapping[])OracleUdt.GetValue(con, udt, 0);
        }
    }

    [OracleCustomTypeMappingAttribute("FBBOR041_COMPARE_RECORD")]
    public class Rec_Reg_PackageOracleTypeMappingFactory : IOracleCustomTypeFactory
    {
        #region IOracleCustomTypeFactory Members

        public IOracleCustomType CreateObject()
        {
            return new Rec_Reg_PackageOracleTypeMapping();
        }

        #endregion
    }
    [OracleCustomTypeMapping("FBBOR041_COMPARE_CPE_ARRAY")]
    public class CompareModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
    {
        #region IOracleCustomTypeFactory Members
        public IOracleCustomType CreateObject()
        {
            return new CompareModel();
        }

        #endregion

        #region IOracleArrayTypeFactory Members
        public Array CreateArray(int numElems)
        {
            return new Rec_Reg_PackageOracleTypeMapping[numElems];
        }

        public Array CreateStatusArray(int numElems)
        {
            return null;
        }

        #endregion
    }

    public class Rec_Reg_PackageOracleTypeMapping : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
    {
        private bool objectIsNull;

        #region Attribute Mapping

        [OracleObjectMappingAttribute("CPE_TYPE")]
        public string CPE_TYPE { get; set; }

        [OracleObjectMappingAttribute("CPE_MODEL_NAME")]
        public string CPE_MODEL_NAME { get; set; }

        [OracleObjectMappingAttribute("STATUS_DESC")]
        public string STATUS_DESC { get; set; }

        [OracleObjectMappingAttribute("CPE_BRAND_NAME")]
        public string CPE_BRAND_NAME { get; set; }

        [OracleObjectMappingAttribute("CPE_MODEL_ID")]
        public string CPE_MODEL_ID { get; set; }

        [OracleObjectMappingAttribute("CPE_GROUP_TYPE")]
        public string CPE_GROUP_TYPE { get; set; }

        [OracleObjectMappingAttribute("SN_PATTERN")]
        public string SN_PATTERN { get; set; }


        #endregion

        public static Rec_Reg_PackageOracleTypeMapping Null
        {
            get
            {
                Rec_Reg_PackageOracleTypeMapping obj = new Rec_Reg_PackageOracleTypeMapping();
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
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            throw new NotImplementedException();
        }
    }
    #endregion

}
