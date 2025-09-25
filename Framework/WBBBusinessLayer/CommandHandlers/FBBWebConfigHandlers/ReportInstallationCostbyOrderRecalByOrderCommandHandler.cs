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
    public class ReportInstallationCostbyOrderRecalByOrderCommandHandler : ICommandHandler<ReportInstallationRecalByOrderCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<ReportInstallationReturnCurRecalModel> _objService;
        private readonly IEntityRepository<FBB_HISTORY_LOG> _historyLog;
        private readonly IWBBUnitOfWork _uow;
        public ReportInstallationCostbyOrderRecalByOrderCommandHandler(ILogger ILogger, IEntityRepository<ReportInstallationReturnCurRecalModel> objService, IEntityRepository<FBB_HISTORY_LOG> historyLog, IWBBUnitOfWork uow)
        {
            _logger = ILogger;
            _objService = objService;
            _historyLog = historyLog;
            _uow = uow;
        }
        public void Handle(ReportInstallationRecalByOrderCommand command)
        {
            var historyLog = new FBB_HISTORY_LOG();
            try
            {

                var packageMappingObjectModel = new PackageMappingObjectModel
                {
                    FBBPAYG_RECAL_LIST =
                        command.p_recal_access_list.Select(
                           a => new FBBPAYG_RECAL_LISTMapping
                           {
                               ACCESS_NUMBER = a.ACCESS_NUMBER,
                               ORDER_NO = a.ORDER_NO,
                               //NEW_RULE_ID = a.NEW_RULE_ID,
                               REMARK = a.REMARK,
                               DISTANCE = a.DISTANCE.ToSafeDecimal(),
                               FLAG_RECAL = a.FLAG_RECAL,
                               REASON = a.REASON,
                               LOOKUP_ID = a.LOOKUP_ID,
                               ONTOP_LOOKUP_ID = a.ONTOP_LOOKUP_ID,
                               RECAL_TOTAL_COST = a.RECAL_TOTAL_COST.ToSafeDecimal()

                           }).ToArray()
                };

                var p_recal_access_list = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_recal_access_list", "WBB.FBBPAYG_RECAL_LIST", packageMappingObjectModel);

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
                command.return_subpayment_cur = _objService.ExecuteReadStoredProc("WBB.PKG_PAYG_INSTALL_COST_RPT.p_recal_revamp",
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
                    historyLog.APPLICATION = "ReportInstallation Re-Cal Distance";
                    historyLog.CREATED_BY = "ReportInstallation";
                    historyLog.CREATED_DATE = DateTime.Now;
                    historyLog.DESCRIPTION = command.ret_msg + " ret_code!=0";
                    historyLog.REF_KEY = "ReportInstallationReCal";
                    historyLog.REF_NAME = "NODEID";
                    _historyLog.Create(historyLog);
                    _uow.Persist();
                }
            }
            catch (Exception ex)
            {
                historyLog.HISTORY_ID = historyLog.HISTORY_ID;
                historyLog.ACTION = ActionHistory.ADD.ToString();
                historyLog.APPLICATION = "ReportInstallation Re-Cal Distance";
                historyLog.CREATED_BY = "ReportInstallation";
                historyLog.CREATED_DATE = DateTime.Now;
                historyLog.DESCRIPTION = "Exception Pk ReportInstallation Re-cal By ORder CommandHandler " + ex.GetErrorMessage().ToSafeString();
                historyLog.REF_KEY = "ReportInstallationReCal";
                historyLog.REF_NAME = "NODEID";
                _historyLog.Create(historyLog);
                _uow.Persist();

                _logger.Info(ex.GetErrorMessage());
                command.ret_code = "-1";
                command.ret_msg = "Error call Report Installation Cost by Order Re-cal By ORder CommandHandler : " + ex.GetErrorMessage();
            }

        }

        #region Mapping FBB_RECAL_LMR_LIST Type Oracle

        public class PackageMappingObjectModel : INullable, IOracleCustomType
        {
            [OracleArrayMapping()]
            public FBBPAYG_RECAL_LISTMapping[] FBBPAYG_RECAL_LIST { get; set; }

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
                OracleUdt.SetValue(con, udt, 0, FBBPAYG_RECAL_LIST);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                FBBPAYG_RECAL_LIST = (FBBPAYG_RECAL_LISTMapping[])OracleUdt.GetValue(con, udt, 0);
            }
        }

        [OracleCustomTypeMapping("WBB.FBBPAYG_RECAL_REC")]
        public class FBBPAYG_RECAL_LIST_MappingOracleTypeMappingFactory : IOracleCustomTypeFactory
        {
            public IOracleCustomType CreateObject()
            {
                return new FBBPAYG_RECAL_LISTMapping();
            }
        }

        [OracleCustomTypeMapping("WBB.FBBPAYG_RECAL_LIST")]
        public class FBBPAYG_RECAL_LISTMappingObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
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
                return new FBBPAYG_RECAL_LISTMapping[numElems];
            }

            public Array CreateStatusArray(int numElems)
            {
                return null;
            }

            #endregion IOracleArrayTypeFactory Members
        }

        public class FBBPAYG_RECAL_LISTMapping : INullable, IOracleCustomType
        {
            private bool objectIsNull;

            #region Attribute Mapping


            [OracleObjectMapping("ACCESS_NUMBER")]
            public string ACCESS_NUMBER { get; set; }

            [OracleObjectMapping("ORDER_NO")]
            public string ORDER_NO { get; set; }

            [OracleObjectMapping("FLAG_RECAL")]
            public string FLAG_RECAL { get; set; }

            [OracleObjectMapping("LOOKUP_ID")]
            public string LOOKUP_ID { get; set; }

            [OracleObjectMapping("ONTOP_LOOKUP_ID")]
            public string ONTOP_LOOKUP_ID { get; set; }

            [OracleObjectMapping("RECAL_TOTAL_COST")]
            public decimal RECAL_TOTAL_COST { get; set; }

            [OracleObjectMapping("DISTANCE")]
            public decimal DISTANCE { get; set; }

            [OracleObjectMapping("REASON")]
            public string REASON { get; set; }

            [OracleObjectMapping("REMARK")]
            public string REMARK { get; set; }
            //[OracleObjectMapping("REMARK_FOR_SUB")]
            //public string REMARK_FOR_SUB { get; set; }
            //[OracleObjectMapping("VALIDATE_DIS")]
            //public decimal? VALIDATE_DIS { get; set; }
            //[OracleObjectMapping("REASON")]
            //public string REASON { get; set; }
            //[OracleObjectMapping("TRANSFER_DATE")]
            //public string TRANSFER_DATE { get; set; }

            #endregion Attribute Mapping

            public static FBBPAYG_RECAL_LISTMapping Null
            {
                get
                {
                    var obj = new FBBPAYG_RECAL_LISTMapping();
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
                OracleUdt.SetValue(con, udt, "FLAG_RECAL", FLAG_RECAL);
                OracleUdt.SetValue(con, udt, "LOOKUP_ID", LOOKUP_ID);
                OracleUdt.SetValue(con, udt, "ONTOP_LOOKUP_ID", ONTOP_LOOKUP_ID);
                OracleUdt.SetValue(con, udt, "RECAL_TOTAL_COST", RECAL_TOTAL_COST);
                OracleUdt.SetValue(con, udt, "DISTANCE", DISTANCE);
                OracleUdt.SetValue(con, udt, "REASON", REASON);
                OracleUdt.SetValue(con, udt, "REMARK", REMARK);
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
