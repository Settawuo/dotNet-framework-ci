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
    public class UpdateReportInstallationCostbyOrderCommandHandler : ICommandHandler<UpdateReportInstallationCostbyOrderCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;

        public UpdateReportInstallationCostbyOrderCommandHandler(ILogger logger,
            IEntityRepository<string> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public void Handle(UpdateReportInstallationCostbyOrderCommand command)
        {
            try
            {
                var packageMappingObjectModel = new PackageMappingObjectModel
                {
                    PAYG_INS_RPT_ACCESS_LIST =
                        command.p_ACCESS_list.Select(
                            a => new PAYG_INS_RPT_ACCESS_LISTMapping
                            {
                                ACCESS_NUMBER = a.ACCESS_NUMBER
                            }).ToArray()
                };

                var packageMapping = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_lastmile_access_list", "WBB.PAYG_INS_RPT_ACCESS_LIST", packageMappingObjectModel);

                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();
                _logger.Info("StartPKG_PAYG_INSTALL_COST_RPT.p_update_by_order");

                if (command.p_VALIDATE_DIS.ToSafeString() == " ")
                {
                    command.p_VALIDATE_DIS = "0";
                }


                var executePassword = _objService.ExecuteStoredProc("WBB.PKG_PAYG_INSTALL_COST_RPT.p_update_by_order",
                    out paramOut,
                    new
                    {
                        packageMapping,
                        command.p_INTERFACE,
                        command.p_USER,
                        command.p_STATUS,
                        command.p_INVOICE_NO,
                        command.p_INVOICE_DT,
                        command.p_IR_DOC,

                        command.p_VALIDATE_DIS,
                        command.p_REASON,
                        command.p_REMARK,
                        command.p_REMARK_FOR_SUB,
                        command.p_TRANSFER_DT,
                        command.p_CHECK_IRDOC,
                        command.p_PERIOD_DATE,
                        ret_code,
                        ret_msg
                    });
                command.ret_code = ret_code.Value.ToSafeString();
                command.ret_msg = ret_msg.Value.ToSafeString();
                _logger.Info("EndPKG_PAYG_INSTALL_COST_RPT.p_update_by_order" + ret_msg);

            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                command.ret_code = "-1";
                command.ret_msg = "Error call UpdateReportInstallationCostbyOrderCommandHandler: " + ex.GetErrorMessage();
            }
        }

        #region Mapping FBB_LASTMILE_ACCESS_LIST Type Oracle

        public class PackageMappingObjectModel : INullable, IOracleCustomType
        {
            [OracleArrayMapping()]
            public PAYG_INS_RPT_ACCESS_LISTMapping[] PAYG_INS_RPT_ACCESS_LIST { get; set; }

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
                OracleUdt.SetValue(con, udt, 0, PAYG_INS_RPT_ACCESS_LIST);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                PAYG_INS_RPT_ACCESS_LIST = (PAYG_INS_RPT_ACCESS_LISTMapping[])OracleUdt.GetValue(con, udt, 0);
            }
        }

        [OracleCustomTypeMapping("WBB.PAYG_INS_RPT_ACCESS_REC")]
        public class PAYG_INS_RPT_ACCESS_LIST_MappingOracleTypeMappingFactory : IOracleCustomTypeFactory
        {
            public IOracleCustomType CreateObject()
            {
                return new PAYG_INS_RPT_ACCESS_LISTMapping();
            }
        }

        [OracleCustomTypeMapping("WBB.PAYG_INS_RPT_ACCESS_LIST")]
        public class PAYG_INS_RPT_ACCESS_LISTMappingObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
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
                return new PAYG_INS_RPT_ACCESS_LISTMapping[numElems];
            }

            public Array CreateStatusArray(int numElems)
            {
                return null;
            }

            #endregion IOracleArrayTypeFactory Members
        }

        public class PAYG_INS_RPT_ACCESS_LISTMapping : INullable, IOracleCustomType
        {
            private bool objectIsNull;

            #region Attribute Mapping

            [OracleObjectMapping("ACCESS_NUMBER")]
            public string ACCESS_NUMBER { get; set; }

            #endregion Attribute Mapping

            public static PAYG_INS_RPT_ACCESS_LISTMapping Null
            {
                get
                {
                    var obj = new PAYG_INS_RPT_ACCESS_LISTMapping();
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
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                throw new NotImplementedException();
            }
        }

        #endregion Mapping  FBB_LASTMILE_ACCESS_LIST Type Oracle
    }
}