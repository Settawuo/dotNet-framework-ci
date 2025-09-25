using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers
{
    public class UpdateAssetCommandHandler : ICommandHandler<UpdateAssetCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_HISTORY_LOG> _historyLog;
        private readonly IEntityRepository<FBSS_HISTORY_LOG> _fbsshistoryLog;
        private readonly IWBBUnitOfWork _uow;
        public UpdateAssetCommandHandler(ILogger logger,
            IEntityRepository<string> objService,
            IEntityRepository<FBB_HISTORY_LOG> historyLog,
            IEntityRepository<FBSS_HISTORY_LOG> fbsshistoryLog,
            IWBBUnitOfWork uow
            )
        {
            _logger = logger;
            _objService = objService;
            _historyLog = historyLog;
            _fbsshistoryLog = fbsshistoryLog;
            _uow = uow;

        }

        public void Handle(UpdateAssetCommand command)
        {
            var historyLog = new FBB_HISTORY_LOG();
            var fbsshistoryLog = new FBSS_HISTORY_LOG();
            try
            {
                #region  Parameter
                var ret_Code = new OracleParameter();
                ret_Code.OracleDbType = OracleDbType.Decimal;
                ret_Code.Direction = ParameterDirection.Output;

                var ret_Msg = new OracleParameter();
                ret_Msg.OracleDbType = OracleDbType.Varchar2;
                ret_Msg.Size = 2000;
                ret_Msg.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var packageMappingObjectModelUpdateAsset = new PackageMappingObjectModelUpdateAsset
                {
                    FBB_UPDATE_ASSET_LIST =
                        command.p_Update_asset_list.Select(
                            a => new FBB_UPDATE_ASSET_LIST_MappingUpdateAsset
                            {
                                ACCESS_NO = a.p_Access_No.ToSafeString(),
                                SERIAL_NUMBER = a.p_Serial_Number.ToSafeString(),
                                ASSET_CODE = a.p_Asset_Code.ToSafeString(),
                                MAT_DOC = a.p_Mat_Doc.ToSafeString(),
                                DOC_YEAR = a.p_Doc_Year.ToSafeString()

                            }).ToArray()
                };
                var packageMapping = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_Update_asset_list", "FBB_UPDATE_ASSET_LIST", packageMappingObjectModelUpdateAsset);

                #endregion
                var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FBB_FOA_ORDER_MANAGEMENT.p_update_asset",
               out paramOut,
                 new
                 {
                     packageMapping,
                     // out 
                     ret_code = ret_Code,
                     ret_msg = ret_Msg

                 });
                command.ret_code = (ret_Code.Value.ToString() != null) ? int.Parse(ret_Code.Value.ToString()) : -1;
                command.ret_msg = ret_Msg.Value.ToSafeString();

                XDocument listSuccessXDocument = new XDocument();

                XDocument listErrorXDocument = new XDocument();
                string LogNodeSuccess = "", LogNodeError = "";

                //Convert Xml for write success 
                listSuccessXDocument = XDocument.Parse(command.p_Update_asset_list_success.DumpToXml());
                if (command.p_Update_asset_list_success.Count() == 0)
                {
                    LogNodeSuccess = "Data Not Found";
                }
                //Convert Xml for write error

                listErrorXDocument = XDocument.Parse(command.p_Update_asset_list_Error.DumpToXml());
                if (command.p_Update_asset_list_Error.Count() == 0)
                {
                    LogNodeError = "Data Not Found";
                }
                //
                decimal idmax = 0;


                //  var MaxTranId = (from c in _fbsshistoryLog.Get() select c);
                // string idMaxTranId = Maxidmax.Max(c => c.IN_TRANSACTION_ID);

                DateTime dt = DateTime.Now;
                string TranId = dt.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                idmax = GetId();

                fbsshistoryLog.INTERFACE_ID = idmax;
                fbsshistoryLog.IN_TRANSACTION_ID = TranId;
                fbsshistoryLog.METHOD_NAME = "ConfirmUpdateAssetFromFile";
                fbsshistoryLog.CREATED_BY = "FBB Update Asset";
                fbsshistoryLog.REQUEST_STATUS = "Success";
                fbsshistoryLog.CREATED_DATE = DateTime.Now;
                fbsshistoryLog.IN_XML_PARAM = listSuccessXDocument.ToSafeString();
                fbsshistoryLog.SERVICE_NAME = "UpdateAssetCommandHandler";
                fbsshistoryLog.URL_LINE = "UpdateAsset/Index";
                fbsshistoryLog.INTERFACE_NODE = LogNodeSuccess;
                _fbsshistoryLog.Create(fbsshistoryLog);
                _uow.Persist();

                // --------------------insert log error 

                idmax = GetId();
                fbsshistoryLog.INTERFACE_ID = idmax;
                fbsshistoryLog.IN_TRANSACTION_ID = TranId;
                fbsshistoryLog.METHOD_NAME = "ConfirmUpdateAssetFromFile";
                fbsshistoryLog.CREATED_BY = "FBB Update Asset";
                fbsshistoryLog.REQUEST_STATUS = "Error";
                fbsshistoryLog.CREATED_DATE = DateTime.Now;
                fbsshistoryLog.IN_XML_PARAM = listErrorXDocument.ToSafeString();
                fbsshistoryLog.SERVICE_NAME = "UpdateAssetCommandHandler";
                fbsshistoryLog.URL_LINE = "UpdateAsset/Index";
                fbsshistoryLog.INTERFACE_NODE = LogNodeError;
                _fbsshistoryLog.Create(fbsshistoryLog);
                _uow.Persist();

            }
            catch (Exception ex)
            {
                command.ret_code = -1;
                command.ret_msg = ex.GetErrorMessage().ToSafeString();

                _logger.Info("Error WBB.PKG_FBB_FOA_ORDER_MANAGEMENT.p_update_asset " + ex.GetErrorMessage());
                historyLog.HISTORY_ID = historyLog.HISTORY_ID;
                historyLog.ACTION = ActionHistory.ADD.ToString();
                historyLog.APPLICATION = "FBBConfig Update Asset";
                historyLog.CREATED_BY = "FBB Update Asset";
                historyLog.CREATED_DATE = DateTime.Now;
                historyLog.DESCRIPTION = "Error WBB.PKG_FBB_FOA_ORDER_MANAGEMENT.p_update_asset " + ex.GetErrorMessage();
                historyLog.REF_KEY = "FBB Update Asset";
                historyLog.REF_NAME = "NODEID";
                _historyLog.Create(historyLog);
                _uow.Persist();
            }
        }
        public decimal GetId()
        {
            var historyLog = new FBB_HISTORY_LOG();
            decimal idmax = 0;
            try
            {
                var Maxidmax = (from c in _fbsshistoryLog.Get() select c);
                var CheckData = Maxidmax.ToList();

                if (CheckData.Count > 0)
                {
                    idmax = Maxidmax.Max(c => c.INTERFACE_ID);
                }
                idmax += 1;
                return idmax;

            }
            catch (Exception ex)
            {

                _logger.Info("Error  GetId fro interface_id" + ex.GetErrorMessage());
                historyLog.HISTORY_ID = historyLog.HISTORY_ID;
                historyLog.ACTION = ActionHistory.ADD.ToString();
                historyLog.APPLICATION = "FBBConfig Update Asset";
                historyLog.CREATED_BY = "FBB Update Asset";
                historyLog.CREATED_DATE = DateTime.Now;
                historyLog.DESCRIPTION = "Error  GetId fro interface_id " + ex.GetErrorMessage();
                historyLog.REF_KEY = "FBB Update Asset";
                historyLog.REF_NAME = "NODEID";
                _historyLog.Create(historyLog);
                _uow.Persist();
                return 0;
            }

        }

    }
    #region PackageMappingObjectModelUpdateAsset
    public class PackageMappingObjectModelUpdateAsset : INullable, IOracleCustomType
    {
        [OracleArrayMapping()]
        public FBB_UPDATE_ASSET_LIST_MappingUpdateAsset[] FBB_UPDATE_ASSET_LIST { get; set; }

        private bool objectIsNull;

        public bool IsNull
        {
            get { return objectIsNull; }
        }

        public static PackageMappingObjectModelUpdateAsset Null
        {
            get
            {
                var obj = new PackageMappingObjectModelUpdateAsset();
                obj.objectIsNull = true;
                return obj;
            }
        }

        public void FromCustomObject(OracleConnection con, object udt)
        {
            OracleUdt.SetValue(con, udt, 0, FBB_UPDATE_ASSET_LIST);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            FBB_UPDATE_ASSET_LIST = (FBB_UPDATE_ASSET_LIST_MappingUpdateAsset[])OracleUdt.GetValue(con, udt, 0);
        }
    }

    [OracleCustomTypeMapping("FBB_UPDATE_ASSET_REC")]
    public class FBB_UPDATE_ASSET_LIST_MappingUpdateAssetOracleTypeMappingFactory : IOracleCustomTypeFactory
    {
        public IOracleCustomType CreateObject()
        {
            return new FBB_UPDATE_ASSET_LIST_MappingUpdateAsset();
        }
    }

    [OracleCustomTypeMapping("FBB_UPDATE_ASSET_LIST")]
    public class FBB_UPDATE_ASSET_LIST_MappingUpdateAssetObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
    {
        #region IOracleCustomTypeFactory Members

        public IOracleCustomType CreateObject()
        {
            return new PackageMappingObjectModelUpdateAsset();
        }

        #endregion IOracleCustomTypeFactory Members

        #region IOracleArrayTypeFactory Members

        public Array CreateArray(int numElems)
        {
            return new FBB_UPDATE_ASSET_LIST_MappingUpdateAsset[numElems];
        }

        public Array CreateStatusArray(int numElems)
        {
            return null;
        }

        #endregion IOracleArrayTypeFactory Members
    }

    public class FBB_UPDATE_ASSET_LIST_MappingUpdateAsset : INullable, IOracleCustomType
    {
        private bool objectIsNull;

        #region Attribute Mapping

        [OracleObjectMapping("ACCESS_NO")]
        public string ACCESS_NO { get; set; }

        [OracleObjectMapping("SERIAL_NUMBER")]
        public string SERIAL_NUMBER { get; set; }

        [OracleObjectMapping("ASSET_CODE")]
        public string ASSET_CODE { get; set; }

        [OracleObjectMapping("MAT_DOC")]
        public string MAT_DOC { get; set; }

        [OracleObjectMapping("DOC_YEAR")]
        public string DOC_YEAR { get; set; }

        #endregion Attribute Mapping

        public static FBB_UPDATE_ASSET_LIST_MappingUpdateAsset Null
        {
            get
            {
                var obj = new FBB_UPDATE_ASSET_LIST_MappingUpdateAsset();
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
            OracleUdt.SetValue(con, udt, "ACCESS_NO", ACCESS_NO);
            OracleUdt.SetValue(con, udt, "SERIAL_NUMBER", SERIAL_NUMBER);
            OracleUdt.SetValue(con, udt, "ASSET_CODE", ASSET_CODE);
            OracleUdt.SetValue(con, udt, "MAT_DOC", MAT_DOC);
            OracleUdt.SetValue(con, udt, "DOC_YEAR", DOC_YEAR);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            throw new NotImplementedException();
        }
    }
    #endregion
}
