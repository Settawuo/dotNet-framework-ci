using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Commands.ExWebServices.FbbPaygLMD;
using WBBData.Repository;
using WBBEntity.Extensions;

namespace WBBBusinessLayer.CommandHandlers.ExWebServices.FbbPaygLMD
{
    public class UpdateOrderStatusCommandHandler : ICommandHandler<UpdateOrderStatusCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;

        public UpdateOrderStatusCommandHandler(ILogger logger,
            IEntityRepository<string> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public void Handle(UpdateOrderStatusCommand command)
        {
            //pkg :: wbb.PKG_FIXED_ASSET_LASTMILE
            //Procedure :: p_update_by_invoice

            try
            {


                var p_TRANSACTION_ID = new OracleParameter
                {
                    ParameterName = "p_TRANSACTION_ID",
                    OracleDbType = OracleDbType.Varchar2,
                    Size = 2000,
                    Direction = ParameterDirection.Input,
                    Value = command.Transaction_ID.ToSafeString()
                };
                var p_STATUS = new OracleParameter
                {
                    ParameterName = "p_STATUS",
                    OracleDbType = OracleDbType.Varchar2,
                    Size = 2000,
                    Direction = ParameterDirection.Input,
                    Value = command.Status.ToSafeString()
                };
                var p_UPDATE_BY = new OracleParameter
                {
                    ParameterName = "p_UPDATE_BY",
                    OracleDbType = OracleDbType.Varchar2,
                    Size = 2000,
                    Direction = ParameterDirection.Input,
                    Value = command.Update_By.ToSafeString()
                };
                var p_UPDATE_DATE = new OracleParameter
                {
                    ParameterName = "p_UPDATE_DATE",
                    OracleDbType = OracleDbType.Date,
                    Size = 2000,
                    Direction = ParameterDirection.Input,
                    Value = command.Update_Date.ToDateTime()
                };
                #region param Ouput

                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Varchar2;
                ret_code.Size = 2000;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;

                #endregion

                var packgaemodel = new PackageObjectModel();
                packgaemodel.ORDER_LIST = command.Invoice_List.Select(c => new OrderListPackageOracleTypeMapping
                {
                    Invoice_no = c.Invoice_no.ToSafeString(),

                }).ToArray();

                var p_UPDATE_INVOICE_LIST = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_UPDATE_INVOICE_LIST", "FBB_UPDATE_STATUS_CUR", packgaemodel);

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FIXED_ASSET_LASTMILE.p_update_by_invoice",
                     out paramOut,
                        new
                        {
                            p_TRANSACTION_ID,
                            p_STATUS,
                            p_UPDATE_BY,
                            p_UPDATE_DATE,
                            p_UPDATE_INVOICE_LIST,
                            //return
                            ret_code,
                            ret_msg

                        });

                command.ret_code = ret_code.Value.ToSafeString();
                command.ret_msg = ret_msg.Value.ToSafeString();
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                command.ret_code = "-1";
                command.ret_msg = ex.Message.ToSafeString();
            }

        }

        #region Mapping REC_REG_PACKAGE Type Oracle
        public class PackageObjectModel : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        {
            [OracleArrayMapping()]
            public OrderListPackageOracleTypeMapping[] ORDER_LIST { get; set; }

            private bool objectIsNull;

            public bool IsNull
            {
                get { return objectIsNull; }
            }

            public static PackageObjectModel Null
            {
                get
                {
                    PackageObjectModel obj = new PackageObjectModel();
                    obj.objectIsNull = true;
                    return obj;
                }
            }

            public void FromCustomObject(OracleConnection con, object udt)
            {
                OracleUdt.SetValue(con, udt, 0, ORDER_LIST);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                ORDER_LIST = (OrderListPackageOracleTypeMapping[])OracleUdt.GetValue(con, udt, 0);
            }
        }

        [OracleCustomTypeMappingAttribute("FBB_UPDATE_STATUS_REC")]
        public class Rec_Reg_PackageOracleTypeMappingFactory : IOracleCustomTypeFactory
        {
            #region IOracleCustomTypeFactory Members

            public IOracleCustomType CreateObject()
            {
                return new OrderListPackageOracleTypeMapping();
            }

            #endregion
        }
        [OracleCustomTypeMapping("FBB_UPDATE_STATUS_CUR")]
        public class PackageObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
        {
            #region IOracleCustomTypeFactory Members
            public IOracleCustomType CreateObject()
            {
                return new PackageObjectModel();
            }

            #endregion

            #region IOracleArrayTypeFactory Members
            public Array CreateArray(int numElems)
            {
                return new OrderListPackageOracleTypeMapping[numElems];
            }

            public Array CreateStatusArray(int numElems)
            {
                return null;
            }

            #endregion
        }

        public class OrderListPackageOracleTypeMapping : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        {
            private bool objectIsNull;

            #region Attribute Mapping
            [OracleObjectMappingAttribute("INVOICE_NO")]
            public string Invoice_no { get; set; }

            #endregion

            public static OrderListPackageOracleTypeMapping Null
            {
                get
                {
                    OrderListPackageOracleTypeMapping obj = new OrderListPackageOracleTypeMapping();
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
                OracleUdt.SetValue(con, udt, "INVOICE_NO", Invoice_no);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                throw new NotImplementedException();
            }
        }
        #endregion
    }
}
