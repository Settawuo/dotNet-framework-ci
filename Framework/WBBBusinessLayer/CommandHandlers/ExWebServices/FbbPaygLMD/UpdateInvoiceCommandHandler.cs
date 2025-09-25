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
    public class UpdateInvoiceCommandHandler : ICommandHandler<UpdateInvoiceCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;

        public UpdateInvoiceCommandHandler(ILogger logger,
            IEntityRepository<string> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public void Handle(UpdateInvoiceCommand command)
        {
            //pkg :: wbb.PKG_FIXED_ASSET_LASTMILE
            //Procedure :: p_update_invoice

            try
            {
                #region parameter Input

                var p_ACTION = new OracleParameter
                {
                    ParameterName = "p_ACTION",
                    OracleDbType = OracleDbType.Varchar2,
                    Size = 2000,
                    Direction = ParameterDirection.Input,
                    Value = command.Action.ToSafeString()
                };
                var p_TRANSACTION_ID = new OracleParameter
                {
                    ParameterName = "p_TRANSACTION_ID",
                    OracleDbType = OracleDbType.Varchar2,
                    Size = 2000,
                    Direction = ParameterDirection.Input,
                    Value = command.Transaction_ID.ToSafeString()
                };


                var p_INVOICE_NO_OLD = new OracleParameter
                {
                    ParameterName = "p_INVOICE_NO_OLD",
                    OracleDbType = OracleDbType.Varchar2,
                    Size = 2000,
                    Direction = ParameterDirection.Input,
                    Value = command.Invoice_no_Old.ToSafeString()
                };

                var p_INVOICE_NO_NEW = new OracleParameter
                {
                    ParameterName = "p_INVOICE_NO_NEW",
                    OracleDbType = OracleDbType.Varchar2,
                    Size = 2000,
                    Direction = ParameterDirection.Input,
                    Value = command.Invoice_no_New.ToSafeString()
                };

                var p_INVOICE_DATE = new OracleParameter
                {
                    ParameterName = "p_INVOICE_DATE",
                    OracleDbType = OracleDbType.Date,
                    Direction = ParameterDirection.Input,
                    Value = command.Invoice_date.ToDate()
                };

                var p_CREATE_BY = new OracleParameter
                {
                    ParameterName = "p_CREATE_BY",
                    OracleDbType = OracleDbType.Varchar2,
                    Size = 2000,
                    Direction = ParameterDirection.Input,
                    Value = command.Create_By.ToSafeString()
                };

                var p_CREATE_DATE = new OracleParameter
                {
                    ParameterName = "p_CREATE_DATE",
                    OracleDbType = OracleDbType.Date,
                    Direction = ParameterDirection.Input,
                    Value = command.Create_Date.ToDateTime()
                };
                var packgaemodel = new PackageObjectModel();
                packgaemodel.ORDER_LIST = command.Order_list.Select(c => new OrderListPackageOracleTypeMapping
                {
                    Access_No = c.Access_No.ToSafeString(),
                    Order_No = c.Order_no.ToSafeString()

                }).ToArray();
                var p_CREATE_INVOICE_LIST = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_CREATE_INVOICE_LIST", "FBB_CREATE_INVOICE_CUR", packgaemodel);

                #endregion
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

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FIXED_ASSET_LASTMILE.p_update_invoice",
                     out paramOut,
                        new
                        {
                            p_TRANSACTION_ID,
                            p_ACTION,
                            p_INVOICE_NO_OLD,
                            p_INVOICE_NO_NEW,
                            p_INVOICE_DATE,
                            p_CREATE_INVOICE_LIST,
                            p_CREATE_BY,
                            p_CREATE_DATE,
                            //return
                            ret_code,
                            ret_msg
                        });

                command.ret_code = ret_code.Value.ToSafeString();
                command.ret_msg = ret_msg.Value.ToSafeString();
            }
            catch (Exception ex)
            {
                command.ret_code = "-1";
                command.ret_msg = ex.Message;
                _logger.Info(ex.GetErrorMessage());
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

        [OracleCustomTypeMappingAttribute("FBB_CREATE_INVOICE_REC")]
        public class Rec_Reg_PackageOracleTypeMappingFactory : IOracleCustomTypeFactory
        {
            #region IOracleCustomTypeFactory Members

            public IOracleCustomType CreateObject()
            {
                return new OrderListPackageOracleTypeMapping();
            }

            #endregion
        }
        [OracleCustomTypeMapping("FBB_CREATE_INVOICE_CUR")]
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
            [OracleObjectMappingAttribute("ACCESS_NO")]
            public string Access_No { get; set; }

            [OracleObjectMappingAttribute("ORDER_NO")]
            public string Order_No { get; set; }

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
                OracleUdt.SetValue(con, udt, "ACCESS_NO", Access_No);
                OracleUdt.SetValue(con, udt, "ORDER_NO", Order_No);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                throw new NotImplementedException();
            }
        }
        #endregion
    }
}
