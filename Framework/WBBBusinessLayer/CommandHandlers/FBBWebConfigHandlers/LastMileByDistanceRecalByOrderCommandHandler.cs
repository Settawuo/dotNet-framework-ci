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
    public class LastMileByDistanceRecalByOrderCommandHandler : ICommandHandler<LastMileByDistanceRecalByOrderCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<ReturnCurRecalModel> _objService;
        private readonly IEntityRepository<FBB_HISTORY_LOG> _historyLog;
        private readonly IWBBUnitOfWork _uow;
        public LastMileByDistanceRecalByOrderCommandHandler(ILogger ILogger, IEntityRepository<ReturnCurRecalModel> objService, IEntityRepository<FBB_HISTORY_LOG> historyLog, IWBBUnitOfWork uow)
        {
            _logger = ILogger;
            _objService = objService;
            _historyLog = historyLog;
            _uow = uow;
        }
        public void Handle(LastMileByDistanceRecalByOrderCommand command)
        {
            var historyLog = new FBB_HISTORY_LOG();
            try
            {

                var packageMappingObjectModel = new PackageMappingObjectModel
                {
                    FBB_RECAL_LMR_LIST =
                        command.p_recal_access_list.Select(
                           a => new FBB_RECAL_LMR_LISTMapping
                           {
                               ACCESS_NUMBER = a.ACCESS_NUMBER,
                               ORDER_NO = a.ORDER_NO,
                               NEW_RULE_ID = a.NEW_RULE_ID,
                               REMARK = a.REMARK,
                               DISTANCE = a.DISTANCE,
                               FLAG_RECAL = a.FLAG_RECAL,
                               REASON = a.REASON

                           }).ToArray()
                };

                var p_recal_access_list = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_recal_access_list", "FBB_RECAL_LMR_LIST", packageMappingObjectModel);

                var ret_code = new OracleParameter();
                ret_code.ParameterName = "ret_code";
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.ParameterName = "ret_msg";
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;

                var return_subpayment_cur = new OracleParameter();
                return_subpayment_cur.ParameterName = "return_subpayment_cur";
                return_subpayment_cur.OracleDbType = OracleDbType.RefCursor;
                return_subpayment_cur.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();
                _logger.Info("StartPKG_FIXED_ASSET_LASTMILE.p_recal_order");

                //var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FIXED_ASSET_LASTMILE.p_re_cal_by_order",
                command.return_subpayment_cur = _objService.ExecuteReadStoredProc("WBB.PKG_FIXED_ASSET_LASTMILE.p_recal_order",
                    new
                    {
                        p_recal_access_list,
                        //command.p_NEW_RULE_ID,
                        command.p_USER,
                        //command.p_REMARK,
                        ret_code,
                        ret_msg,
                        return_subpayment_cur
                    }).ToList();
                command.ret_code = ret_code.Value.ToSafeString();
                command.ret_msg = ret_msg.Value.ToSafeString();

                _logger.Info("EndPKG_FIXED_ASSET_LASTMILE.p_recal_order" + ret_msg);
                if (command.ret_code != "0")
                {
                    historyLog.HISTORY_ID = historyLog.HISTORY_ID;
                    historyLog.ACTION = ActionHistory.ADD.ToString();
                    historyLog.APPLICATION = "Lastmile Re-Cal Distance";
                    historyLog.CREATED_BY = "Lastmile";
                    historyLog.CREATED_DATE = DateTime.Now;
                    historyLog.DESCRIPTION = command.ret_msg + " ret_code!=0";
                    historyLog.REF_KEY = "LastmileReCal";
                    historyLog.REF_NAME = "NODEID";
                    _historyLog.Create(historyLog);
                    _uow.Persist();
                }
            }
            catch (Exception ex)
            {
                historyLog.HISTORY_ID = historyLog.HISTORY_ID;
                historyLog.ACTION = ActionHistory.ADD.ToString();
                historyLog.APPLICATION = "Lastmile Re-Cal Distance";
                historyLog.CREATED_BY = "Lastmile";
                historyLog.CREATED_DATE = DateTime.Now;
                historyLog.DESCRIPTION = "Exception Pk Lastmile Re-cal By ORder CommandHandler " + ex.GetErrorMessage().ToSafeString();
                historyLog.REF_KEY = "LastmileReCal";
                historyLog.REF_NAME = "NODEID";
                _historyLog.Create(historyLog);
                _uow.Persist();

                _logger.Info(ex.GetErrorMessage());
                command.ret_code = "-1";
                command.ret_msg = "Error call LastMileBy Distance Re-cal By ORder CommandHandler : " + ex.GetErrorMessage();
            }

        }

        #region Mapping FBB_RECAL_LMR_LIST Type Oracle

        public class PackageMappingObjectModel : INullable, IOracleCustomType
        {
            [OracleArrayMapping()]
            public FBB_RECAL_LMR_LISTMapping[] FBB_RECAL_LMR_LIST { get; set; }

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
                OracleUdt.SetValue(con, udt, 0, FBB_RECAL_LMR_LIST);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                FBB_RECAL_LMR_LIST = (FBB_RECAL_LMR_LISTMapping[])OracleUdt.GetValue(con, udt, 0);
            }
        }

        [OracleCustomTypeMapping("FBB_RECAL_LMR_REC")]
        public class FBB_RECAL_LMR_LIST_MappingOracleTypeMappingFactory : IOracleCustomTypeFactory
        {
            public IOracleCustomType CreateObject()
            {
                return new FBB_RECAL_LMR_LISTMapping();
            }
        }

        [OracleCustomTypeMapping("FBB_RECAL_LMR_LIST")]
        public class FBB_RECAL_LMR_LISTMappingObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
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
                return new FBB_RECAL_LMR_LISTMapping[numElems];
            }

            public Array CreateStatusArray(int numElems)
            {
                return null;
            }

            #endregion IOracleArrayTypeFactory Members
        }

        public class FBB_RECAL_LMR_LISTMapping : INullable, IOracleCustomType
        {
            private bool objectIsNull;

            #region Attribute Mapping

            [OracleObjectMapping("ACCESS_NUMBER")]
            public string ACCESS_NUMBER { get; set; }
            [OracleObjectMapping("ORDER_NO")]
            public string ORDER_NO { get; set; }
            [OracleObjectMapping("NEW_RULE_ID")]
            public string NEW_RULE_ID { get; set; }
            //[OracleObjectMapping("IR_DOC")]
            //public string IR_DOC { get; set; }
            [OracleObjectMapping("REMARK")]
            public string REMARK { get; set; }
            [OracleObjectMapping("DISTANCE")]
            public string DISTANCE { get; set; }
            [OracleObjectMapping("FLAG_RECAL")]
            public string FLAG_RECAL { get; set; }
            [OracleObjectMapping("REASON")]
            public string REASON { get; set; }
            //[OracleObjectMapping("REMARK_FOR_SUB")]
            //public string REMARK_FOR_SUB { get; set; }
            //[OracleObjectMapping("VALIDATE_DIS")]
            //public decimal? VALIDATE_DIS { get; set; }
            //[OracleObjectMapping("REASON")]
            //public string REASON { get; set; }
            //[OracleObjectMapping("TRANSFER_DATE")]
            //public string TRANSFER_DATE { get; set; }

            #endregion Attribute Mapping

            public static FBB_RECAL_LMR_LISTMapping Null
            {
                get
                {
                    var obj = new FBB_RECAL_LMR_LISTMapping();
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
                OracleUdt.SetValue(con, udt, "ACCESS_NUMBER", ACCESS_NUMBER);
                OracleUdt.SetValue(con, udt, "ORDER_NO", ORDER_NO);
                OracleUdt.SetValue(con, udt, "NEW_RULE_ID", NEW_RULE_ID);
                //OracleUdt.SetValue(con, udt, "IR_DOC", IR_DOC);
                OracleUdt.SetValue(con, udt, "REMARK", REMARK);
                OracleUdt.SetValue(con, udt, "DISTANCE", DISTANCE);
                OracleUdt.SetValue(con, udt, "FLAG_RECAL", FLAG_RECAL);
                OracleUdt.SetValue(con, udt, "REASON", REASON);
                //OracleUdt.SetValue(con, udt, "REMARK_FOR_SUB", REMARK_FOR_SUB);
                //OracleUdt.SetValue(con, udt, "VALIDATE_DIS", VALIDATE_DIS);
                //OracleUdt.SetValue(con, udt, "REASON", REASON);
                //OracleUdt.SetValue(con, udt, "TRANSFER_DATE", TRANSFER_DATE);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                throw new NotImplementedException();
            }
        }

        #endregion Mapping  FBB_RECAL_LMR_LIST Type Oracle
    }
}
