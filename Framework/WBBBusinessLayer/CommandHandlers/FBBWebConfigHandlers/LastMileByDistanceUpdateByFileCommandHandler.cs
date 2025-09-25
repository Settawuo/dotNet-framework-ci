using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers
{
    public class LastMileByDistanceUpdateByFileCommandHandler : ICommandHandler<LastMileByDistanceUpdateByFileCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_HISTORY_LOG> _historyLog;
        private readonly IWBBUnitOfWork _uow;
        public LastMileByDistanceUpdateByFileCommandHandler(ILogger ILogger, IEntityRepository<string> objService, IEntityRepository<FBB_HISTORY_LOG> historyLog, IWBBUnitOfWork uow)
        {
            _logger = ILogger;
            _objService = objService;
            _historyLog = historyLog;
            _uow = uow;
        }

        public void Handle(LastMileByDistanceUpdateByFileCommand command)
        {
            var historyLog = new FBB_HISTORY_LOG();

            try
            {
                var packageMappingObjectModel = new PackageMappingObjectModel
                {
                    FBB_UPDATE_FILE_LIST =
                        command.p_file_list.Select(
                            a => new FBB_UPDATE_FILE_LISTMapping
                            {
                                ACC_NBR = a.ACC_NBR,
                                INVOICE_NO = a.INVOICE_NO,
                                INVOICE_DATE = a.INVOICE_DATE,
                                IR_DOC = a.IR_DOC,
                                REMARK = a.REMARK,
                                REMARK_FOR_SUB = a.REMARK_FOR_SUB,
                                VALIDATE_DIS = a.VALIDATE_DIS,
                                REASON = a.REASON,
                                TRANSFER_DATE = a.TRANSFER_DATE
                            }).ToArray()
                };

                var p_file_list = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_file_list", "FBB_UPDATE_FILE_LIST", packageMappingObjectModel);

                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();
                _logger.Info("StartPKG_FIXED_ASSET_LASTMILE.p_update_by_file");

                var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FIXED_ASSET_LASTMILE.p_update_by_file",
                    out paramOut,
                    new
                    {
                        command.p_INTERFACE,
                        command.p_USER,
                        command.p_STATUS,
                        command.p_filename,
                        p_file_list,
                        ret_code,
                        ret_msg
                    });
                command.ret_code = ret_code.Value.ToSafeString();
                command.ret_msg = ret_msg.Value.ToSafeString();
                _logger.Info("EndPKG_FIXED_ASSET_LASTMILE.p_update_by_file" + ret_msg);
                if (command.ret_code != "0")
                {
                    historyLog.HISTORY_ID = historyLog.HISTORY_ID;
                    historyLog.ACTION = ActionHistory.ADD.ToString();
                    historyLog.APPLICATION = "Lastmile Update By File Dispute";
                    historyLog.CREATED_BY = "Lastmile";
                    historyLog.CREATED_DATE = DateTime.Now;
                    historyLog.DESCRIPTION = command.ret_msg;
                    historyLog.REF_KEY = "LastmileUpdateByFile";
                    historyLog.REF_NAME = "NODEID";
                    _historyLog.Create(historyLog);
                    _uow.Persist();
                }
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                command.ret_code = "-1";
                command.ret_msg = "Error call LastMileByDistanceUpdateByFileCommandHandler Handler: " + ex.GetErrorMessage();
                if (command.ret_code != "0")
                {
                    historyLog.HISTORY_ID = historyLog.HISTORY_ID;
                    historyLog.ACTION = ActionHistory.ADD.ToString();
                    historyLog.APPLICATION = "Lastmile Update By File Dispute";
                    historyLog.CREATED_BY = "Lastmile";
                    historyLog.CREATED_DATE = DateTime.Now;
                    historyLog.DESCRIPTION = command.ret_msg;
                    historyLog.REF_KEY = "LastmileUpdateByFile";
                    historyLog.REF_NAME = "NODEID";
                    _historyLog.Create(historyLog);
                    _uow.Persist();
                }
            }

        }

        #region Mapping FBB_UPDATE_FILE_LIST Type Oracle

        public class PackageMappingObjectModel : INullable, IOracleCustomType
        {
            [OracleArrayMapping()]
            public FBB_UPDATE_FILE_LISTMapping[] FBB_UPDATE_FILE_LIST { get; set; }

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
                OracleUdt.SetValue(con, udt, 0, FBB_UPDATE_FILE_LIST);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                FBB_UPDATE_FILE_LIST = (FBB_UPDATE_FILE_LISTMapping[])OracleUdt.GetValue(con, udt, 0);
            }
        }

        [OracleCustomTypeMapping("FBB_UPDATE_FILE_REC")]
        public class FBB_UPDATE_FILE_LIST_MappingOracleTypeMappingFactory : IOracleCustomTypeFactory
        {
            public IOracleCustomType CreateObject()
            {
                return new FBB_UPDATE_FILE_LISTMapping();
            }
        }

        [OracleCustomTypeMapping("FBB_UPDATE_FILE_LIST")]
        public class FBB_UPDATE_FILE_LISTMappingObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
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
                return new FBB_UPDATE_FILE_LISTMapping[numElems];
            }

            public Array CreateStatusArray(int numElems)
            {
                return null;
            }

            #endregion IOracleArrayTypeFactory Members
        }

        public class FBB_UPDATE_FILE_LISTMapping : INullable, IOracleCustomType
        {
            private bool objectIsNull;

            #region Attribute Mapping

            [OracleObjectMapping("ACC_NBR")]
            public string ACC_NBR { get; set; }
            [OracleObjectMapping("INVOICE_NO")]
            public string INVOICE_NO { get; set; }
            [OracleObjectMapping("INVOICE_DATE")]
            public string INVOICE_DATE { get; set; }
            [OracleObjectMapping("IR_DOC")]
            public string IR_DOC { get; set; }
            [OracleObjectMapping("REMARK")]
            public string REMARK { get; set; }
            [OracleObjectMapping("REMARK_FOR_SUB")]
            public string REMARK_FOR_SUB { get; set; }
            [OracleObjectMapping("VALIDATE_DIS")]
            public decimal? VALIDATE_DIS { get; set; }
            [OracleObjectMapping("REASON")]
            public string REASON { get; set; }
            [OracleObjectMapping("TRANSFER_DATE")]
            public string TRANSFER_DATE { get; set; }

            #endregion Attribute Mapping

            public static FBB_UPDATE_FILE_LISTMapping Null
            {
                get
                {
                    var obj = new FBB_UPDATE_FILE_LISTMapping();
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
                OracleUdt.SetValue(con, udt, "INVOICE_NO", INVOICE_NO);
                OracleUdt.SetValue(con, udt, "INVOICE_DATE", INVOICE_DATE);
                OracleUdt.SetValue(con, udt, "IR_DOC", IR_DOC);
                OracleUdt.SetValue(con, udt, "REMARK", REMARK);
                OracleUdt.SetValue(con, udt, "REMARK_FOR_SUB", REMARK_FOR_SUB);
                OracleUdt.SetValue(con, udt, "VALIDATE_DIS", VALIDATE_DIS);
                OracleUdt.SetValue(con, udt, "REASON", REASON);
                OracleUdt.SetValue(con, udt, "TRANSFER_DATE", TRANSFER_DATE);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                throw new NotImplementedException();
            }
        }

        #endregion Mapping  FBB_UPDATE_FILE_LIST Type Oracle
    }
}
