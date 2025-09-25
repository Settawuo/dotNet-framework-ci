using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.FBBShareplex;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.FBBShareplexModels;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.FBBShareplex
{
    public class GetOrderDetailDataQueryHandler : IQueryHandler<GetOrderDetailDataQuery, GetOrderDetailModel>
    {
        #region Old Version Shareplex
        private readonly ILogger _logger;
        private readonly IFBBShareplexEntityRepository<object> _fbbShareplexRepository;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public GetOrderDetailDataQueryHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow,
            IFBBShareplexEntityRepository<object> fbbShareplexRepository)
        {
            _logger = logger;
            _fbbShareplexRepository = fbbShareplexRepository;
            _intfLog = intfLog;
            _uow = uow;
        }

        public GetOrderDetailModel Handle(GetOrderDetailDataQuery query)
        {
            InterfaceLogCommand log = null;
            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, "" + query.TransactionID, "GetOrderDetailData", "GetOrderDetailDataQuery", null, "FBB|", "WEB");

            GetOrderDetailModel executeResults = new GetOrderDetailModel();

            var fbbListOrderObjectModel = new ListOrderObjectModel();
            fbbListOrderObjectModel.ListOrder = query.ListOrder.Select(a => new ListOrderOracleTypeMapping
            {
                OrderNo = a.OrderNo.ToSafeString()
            }).ToArray();

            var fbbListOrder = OracleCustomTypeUtilities.CreateUDTArrayParameter("ListOrder", "FBBADM.FBB_LIST_ORDER_ARRAY", fbbListOrderObjectModel);

            var fbbListCardNoObjectModel = new ListCardNoObjectModel();
            fbbListCardNoObjectModel.ListCardNo = query.ListCardNo.Select(a => new ListCardNoOracleTypeMapping
            {
                CardNo = a.CardNo.ToSafeString()
            }).ToArray();

            var fbbListCardNo = OracleCustomTypeUtilities.CreateUDTArrayParameter("ListCardNo", "FBBADM.FBB_LIST_CARD_NO_ARRAY", fbbListCardNoObjectModel);


            var fbbListNonMobileNoObjectModel = new ListNonMobileNoObjectModel();
            fbbListNonMobileNoObjectModel.ListNonMobileNo = query.ListNonMobileNo.Select(a => new ListNonMobileNoOracleTypeMapping
            {
                NonMobileNo = a.NonMobileNo.ToSafeString()
            }).ToArray();

            var fbbListNonMobileNo = OracleCustomTypeUtilities.CreateUDTArrayParameter("ListNonMobileNo", "FBBADM.FBB_LIST_NON_MOBILE_NO_ARRAY", fbbListNonMobileNoObjectModel);

            var fbbListContactMobileNoObjectModel = new ListContactMobileNoObjectModel();
            fbbListContactMobileNoObjectModel.ListContactMobileNo = query.ListContactMobileNo.Select(a => new ListContactMobileNoOracleTypeMapping
            {
                ContactMobileNo = a.ContactMobileNo.ToSafeString()
            }).ToArray();

            var fbbListContactMobileNo = OracleCustomTypeUtilities.CreateUDTArrayParameter("ListContactMobileNo", "FBBADM.FBB_LIST_CONTACT_MOBILE_ARRAY", fbbListContactMobileNoObjectModel);

            var CustomerName = new OracleParameter();
            CustomerName.ParameterName = "CustomerName";
            CustomerName.Size = 2000;
            CustomerName.OracleDbType = OracleDbType.Varchar2;
            CustomerName.Direction = ParameterDirection.Input;
            CustomerName.Value = query.CustomerName;

            var CustomerLastName = new OracleParameter();
            CustomerLastName.ParameterName = "CustomerLastName";
            CustomerLastName.Size = 2000;
            CustomerLastName.OracleDbType = OracleDbType.Varchar2;
            CustomerLastName.Direction = ParameterDirection.Input;
            CustomerLastName.Value = query.CustomerLastName;

            var ReturnCode = new OracleParameter();
            ReturnCode.ParameterName = "ReturnCode";
            ReturnCode.OracleDbType = OracleDbType.Varchar2;
            ReturnCode.Size = 2000;
            ReturnCode.Direction = ParameterDirection.Output;

            var ReturnMessage = new OracleParameter();
            ReturnMessage.ParameterName = "ReturnMessage";
            ReturnMessage.OracleDbType = OracleDbType.Varchar2;
            ReturnMessage.Size = 2000;
            ReturnMessage.Direction = ParameterDirection.Output;

            var ReturnOrderCustomer = new OracleParameter();
            ReturnOrderCustomer.ParameterName = "ReturnOrderCustomer";
            ReturnOrderCustomer.OracleDbType = OracleDbType.RefCursor;
            ReturnOrderCustomer.Direction = ParameterDirection.Output;

            var ReturnOrderPackage = new OracleParameter();
            ReturnOrderPackage.ParameterName = "ReturnOrderPackage";
            ReturnOrderPackage.OracleDbType = OracleDbType.RefCursor;
            ReturnOrderPackage.Direction = ParameterDirection.Output;

            var ReturnOrderContact = new OracleParameter();
            ReturnOrderContact.ParameterName = "ReturnOrderContact";
            ReturnOrderContact.OracleDbType = OracleDbType.RefCursor;
            ReturnOrderContact.Direction = ParameterDirection.Output;

            var ReturnOrderBillMedia = new OracleParameter();
            ReturnOrderBillMedia.ParameterName = "ReturnOrderBillMedia";
            ReturnOrderBillMedia.OracleDbType = OracleDbType.RefCursor;
            ReturnOrderBillMedia.Direction = ParameterDirection.Output;

            var ReturnBillingAddress = new OracleParameter();
            ReturnBillingAddress.ParameterName = "ReturnBillingAddress";
            ReturnBillingAddress.OracleDbType = OracleDbType.RefCursor;
            ReturnBillingAddress.Direction = ParameterDirection.Output;

            var ReturnInstallAddress = new OracleParameter();
            ReturnInstallAddress.ParameterName = "ReturnInstallAddress";
            ReturnInstallAddress.OracleDbType = OracleDbType.RefCursor;
            ReturnInstallAddress.Direction = ParameterDirection.Output;

            var ReturnOrderDocument = new OracleParameter();
            ReturnOrderDocument.ParameterName = "ReturnOrderDocument";
            ReturnOrderDocument.OracleDbType = OracleDbType.RefCursor;
            ReturnOrderDocument.Direction = ParameterDirection.Output;

            var ReturnForOfficer = new OracleParameter();
            ReturnForOfficer.ParameterName = "ReturnForOfficer";
            ReturnForOfficer.OracleDbType = OracleDbType.RefCursor;
            ReturnForOfficer.Direction = ParameterDirection.Output;

            try
            {
                var result = _fbbShareplexRepository.ExecuteStoredProcMultipleCursor("FBBADM.PKG_FBB_QUERY_ORDER.QUERY_ORDER_DETAIL",
                    new object[]
                    {
                        fbbListOrder,
                        fbbListCardNo,
                        fbbListNonMobileNo,
                        fbbListContactMobileNo,
                        CustomerName,
                        CustomerLastName,

                        // return
                        ReturnCode,
                        ReturnMessage,

                        ReturnOrderCustomer,
                        ReturnOrderPackage,
                        ReturnOrderContact,
                        ReturnOrderBillMedia,
                        ReturnBillingAddress,
                        ReturnInstallAddress,
                        ReturnOrderDocument,
                        ReturnForOfficer
                    });

                if (result != null)
                {
                    executeResults.ReturnCode = result[0] != null ? result[0].ToString() : "-1";
                    executeResults.ReturnMessage = result[1] != null ? result[1].ToString() : "";

                    if (result.Count >= 3)
                    {
                        var d_ReturnOrderCustomerData = (DataTable)result[2];
                        if (d_ReturnOrderCustomerData != null && d_ReturnOrderCustomerData.Rows.Count > 0)
                        {
                            var RETURNORDERCUSTOMER = d_ReturnOrderCustomerData.DataTableToList<OrderCustomerData>();
                            executeResults.ReturnOrderCustomer = RETURNORDERCUSTOMER;
                        }
                    }
                    if (result.Count >= 4)
                    {
                        var d_ReturnOrderPackageData = (DataTable)result[3];
                        if (d_ReturnOrderPackageData != null && d_ReturnOrderPackageData.Rows.Count > 0)
                        {
                            var RETURNORDERPACKAGE = d_ReturnOrderPackageData.DataTableToList<OrderPackageData>();
                            executeResults.ReturnOrderPackage = RETURNORDERPACKAGE;
                        }
                    }
                    if (result.Count >= 5)
                    {
                        var d_ReturnOrderContactData = (DataTable)result[4];
                        if (d_ReturnOrderContactData != null && d_ReturnOrderContactData.Rows.Count > 0)
                        {
                            var RETURNORDERCONTACT = d_ReturnOrderContactData.DataTableToList<OrderContactData>();
                            executeResults.ReturnOrderContact = RETURNORDERCONTACT;
                        }
                    }
                    if (result.Count >= 6)
                    {
                        var d_ReturnOrderBillMediaData = (DataTable)result[5];
                        if (d_ReturnOrderBillMediaData != null && d_ReturnOrderBillMediaData.Rows.Count > 0)
                        {
                            var RETURNORDERBILLMEDIA = d_ReturnOrderBillMediaData.DataTableToList<OrderBillMediaData>();
                            executeResults.ReturnOrderBillMedia = RETURNORDERBILLMEDIA;
                        }
                    }
                    if (result.Count >= 7)
                    {
                        var d_ReturnBillingAddressData = (DataTable)result[6];
                        if (d_ReturnBillingAddressData != null && d_ReturnBillingAddressData.Rows.Count > 0)
                        {
                            var RETURNBILLINGADDRESS = d_ReturnBillingAddressData.DataTableToList<BillingAddressData>();
                            executeResults.ReturnBillingAddress = RETURNBILLINGADDRESS;
                        }
                    }
                    if (result.Count >= 8)
                    {
                        var d_ReturnInstallAddressData = (DataTable)result[7];
                        if (d_ReturnInstallAddressData != null && d_ReturnInstallAddressData.Rows.Count > 0)
                        {
                            var RETURNINSTALLADDRESS = d_ReturnInstallAddressData.DataTableToList<InstallAddressData>();
                            executeResults.ReturnInstallAddress = RETURNINSTALLADDRESS;
                        }
                    }
                    if (result.Count >= 9)
                    {
                        var d_ReturnOrderDocumentData = (DataTable)result[8];
                        if (d_ReturnOrderDocumentData != null && d_ReturnOrderDocumentData.Rows.Count > 0)
                        {
                            var RETURNORDERDOCUMENT = d_ReturnOrderDocumentData.DataTableToList<OrderDocumentData>();
                            executeResults.ReturnOrderDocument = RETURNORDERDOCUMENT;
                        }
                    }
                    if (result.Count >= 10)
                    {
                        var d_ReturnForOfficerData = (DataTable)result[9];
                        if (d_ReturnForOfficerData != null && d_ReturnForOfficerData.Rows.Count > 0)
                        {
                            var RETURNFOROFFICER = d_ReturnForOfficerData.DataTableToList<ForOfficerData>();
                            executeResults.ReturnForOfficer = RETURNFOROFFICER;
                        }
                    }

                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "", "");

                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, query, log, "Failed", "Failed", "");
                    executeResults.ReturnCode = "-1";
                    executeResults.ReturnMessage = "Error";

                }

            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);
                _logger.Info("Error call service FBBADM.PKG_FBB_QUERY_ORDER.QUERY_ORDER_DETAIL" + ex.Message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, query, log, "Failed", ex.Message, "");
                executeResults.ReturnCode = "-1";
                executeResults.ReturnMessage = "Error";
            }

            return executeResults;
        }
        #endregion

        #region New Version For Shareplex to HVR PostgreSQL R23.06

        //private readonly ILogger _logger;
        //private readonly IFBBHVREntityRepository<object> _fbbHVRRepository;
        //private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        //private readonly IWBBUnitOfWork _uow;

        //public GetOrderDetailDataQueryHandler(ILogger logger,
        //    IEntityRepository<FBB_INTERFACE_LOG> intfLog,
        //    IWBBUnitOfWork uow,
        //    IFBBHVREntityRepository<object> fbbHVRRepository)
        //{
        //    _logger = logger;
        //    _fbbHVRRepository = fbbHVRRepository;
        //    _intfLog = intfLog;
        //    _uow = uow;
        //}

        //public GetOrderDetailModel Handle(GetOrderDetailDataQuery query)
        //{
        //    var result = new GetOrderDetailModel();
        //    InterfaceLogCommand log = null;
        //    log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, "" + query.TransactionID, "GetOrderDetailData", "GetOrderDetailDataQuery", null, "FBB|", "WEB");

        //    List<HvrListOrderRecord> ListOrderReccord = query.ListOrder.Select(c => new HvrListOrderRecord
        //    {
        //        orderno = c.OrderNo.ToSafeString()
        //    }).ToList();

        //    List<HvrListCardNoRecord> ListCardNoReccord = query.ListCardNo.Select(c => new HvrListCardNoRecord
        //    {
        //        cardno = c.CardNo.ToSafeString()
        //    }).ToList();

        //    List<HvrListNonMobileNoRecord> ListNonMobileNoReccord = query.ListNonMobileNo.Select(c => new HvrListNonMobileNoRecord
        //    {
        //        nonmobileno = c.NonMobileNo.ToSafeString()
        //    }).ToList();

        //    List<HvrListContactMobileNoRecord> ListContactMobileNoReccord = query.ListContactMobileNo.Select(c => new HvrListContactMobileNoRecord
        //    {
        //        contactmobileno = c.ContactMobileNo.ToSafeString()
        //    }).ToList();

        //    var listorder = new NpgsqlParameter();
        //    listorder.ParameterName = "listorder";
        //    listorder.Direction = ParameterDirection.Input;
        //    listorder.Value = ListOrderReccord.ToArray();

        //    var listcardno = new NpgsqlParameter();
        //    listcardno.ParameterName = "listcardno";
        //    listcardno.Direction = ParameterDirection.Input;
        //    listcardno.Value = ListCardNoReccord.ToArray();

        //    var listnonmobileno = new NpgsqlParameter();
        //    listnonmobileno.ParameterName = "listnonmobileno";
        //    listnonmobileno.Direction = ParameterDirection.Input;
        //    listnonmobileno.Value = ListNonMobileNoReccord.ToArray();

        //    var listcontactmobileno = new NpgsqlParameter();
        //    listcontactmobileno.ParameterName = "listcontactmobileno";
        //    listcontactmobileno.Direction = ParameterDirection.Input;
        //    listcontactmobileno.Value = ListContactMobileNoReccord.ToArray();

        //    var customername = new NpgsqlParameter();
        //    customername.ParameterName = "customername";
        //    customername.Size = 2000;
        //    customername.NpgsqlDbType = NpgsqlDbType.Text;
        //    customername.Direction = ParameterDirection.Input;
        //    customername.Value = query.CustomerName.ToSafeString();

        //    var customerlastname = new NpgsqlParameter();
        //    customerlastname.ParameterName = "customerlastname";
        //    customerlastname.Size = 2000;
        //    customerlastname.NpgsqlDbType = NpgsqlDbType.Text;
        //    customerlastname.Direction = ParameterDirection.Input;
        //    customerlastname.Value = query.CustomerLastName.ToSafeString();

        //    var returncode_cur = new NpgsqlParameter();
        //    returncode_cur.ParameterName = "returncode_cur";
        //    returncode_cur.NpgsqlDbType = NpgsqlDbType.Refcursor;
        //    returncode_cur.Direction = ParameterDirection.InputOutput;
        //    returncode_cur.Value = "returncode_cur";

        //    var returnordercustomer = new NpgsqlParameter();
        //    returnordercustomer.ParameterName = "returnordercustomer";
        //    returnordercustomer.NpgsqlDbType = NpgsqlDbType.Refcursor;
        //    returnordercustomer.Direction = ParameterDirection.InputOutput;
        //    returnordercustomer.Value = "returnordercustomer";

        //    var returnorderpackage = new NpgsqlParameter();
        //    returnorderpackage.ParameterName = "returnorderpackage";
        //    returnorderpackage.NpgsqlDbType = NpgsqlDbType.Refcursor;
        //    returnorderpackage.Direction = ParameterDirection.InputOutput;
        //    returnorderpackage.Value = "returnorderpackage";

        //    var returnordercontact = new NpgsqlParameter();
        //    returnordercontact.ParameterName = "returnordercontact";
        //    returnordercontact.NpgsqlDbType = NpgsqlDbType.Refcursor;
        //    returnordercontact.Direction = ParameterDirection.InputOutput;
        //    returnordercontact.Value = "returnordercontact";

        //    var returnorderbillmedia = new NpgsqlParameter();
        //    returnorderbillmedia.ParameterName = "returnorderbillmedia";
        //    returnorderbillmedia.NpgsqlDbType = NpgsqlDbType.Refcursor;
        //    returnorderbillmedia.Direction = ParameterDirection.InputOutput;
        //    returnorderbillmedia.Value = "returnorderbillmedia";

        //    var returnbillingaddress = new NpgsqlParameter();
        //    returnbillingaddress.ParameterName = "returnbillingaddress";
        //    returnbillingaddress.NpgsqlDbType = NpgsqlDbType.Refcursor;
        //    returnbillingaddress.Direction = ParameterDirection.InputOutput;
        //    returnbillingaddress.Value = "returnbillingaddress";

        //    var returninstalladdress = new NpgsqlParameter();
        //    returninstalladdress.ParameterName = "returninstalladdress";
        //    returninstalladdress.NpgsqlDbType = NpgsqlDbType.Refcursor;
        //    returninstalladdress.Direction = ParameterDirection.InputOutput;
        //    returninstalladdress.Value = "returninstalladdress";

        //    var returnorderdocument = new NpgsqlParameter();
        //    returnorderdocument.ParameterName = "returnorderdocument";
        //    returnorderdocument.NpgsqlDbType = NpgsqlDbType.Refcursor;
        //    returnorderdocument.Direction = ParameterDirection.InputOutput;
        //    returnorderdocument.Value = "returnorderdocument";

        //    var returnforofficer = new NpgsqlParameter();
        //    returnforofficer.ParameterName = "returnforofficer";
        //    returnforofficer.NpgsqlDbType = NpgsqlDbType.Refcursor;
        //    returnforofficer.Direction = ParameterDirection.InputOutput;
        //    returnforofficer.Value = "returnforofficer";

        //    try
        //    {
        //        var executeResult = _fbbHVRRepository.ExecuteStoredProcMultipleCursorNpgsql("fbbadm.pkg_fbb_query_order_query_order_detail",
        //            new object[]
        //            {
        //                listorder,
        //                listcardno,
        //                listnonmobileno,
        //                listcontactmobileno,
        //                customername,
        //                customerlastname,

        //                // return
        //                returncode_cur,

        //                returnordercustomer,
        //                returnorderpackage,
        //                returnordercontact,
        //                returnorderbillmedia,
        //                returnbillingaddress,
        //                returninstalladdress,
        //                returnorderdocument,
        //                returnforofficer
        //            }).ToList();

        //        if (executeResult != null)
        //        {
        //            DataTable dtr1 = (DataTable)executeResult[0];
        //            List<GetOrderDetailModel> _cur1 = dtr1.ConvertDataTable<GetOrderDetailModel>();

        //            var _first = _cur1.FirstOrDefault();
        //            result.ReturnCode = _first.ReturnCode != null ? _first.ReturnCode.ToString() : "-1";
        //            result.ReturnMessage = _first.ReturnMessage != null ? _first.ReturnMessage.ToString() : "";

        //            if (executeResult.Count >= 2)
        //            {
        //                DataTable d_ReturnOrderCustomerData = (DataTable)executeResult[1];
        //                if (d_ReturnOrderCustomerData != null && d_ReturnOrderCustomerData.Rows.Count > 0)
        //                {
        //                    List<OrderCustomerData> RETURNORDERCUSTOMER = d_ReturnOrderCustomerData.ConvertDataTable<OrderCustomerData>();
        //                    result.ReturnOrderCustomer = RETURNORDERCUSTOMER;
        //                }
        //            }
        //            if (executeResult.Count >= 3)
        //            {
        //                DataTable d_ReturnOrderPackageData = (DataTable)executeResult[2];
        //                if (d_ReturnOrderPackageData != null && d_ReturnOrderPackageData.Rows.Count > 0)
        //                {
        //                    List<OrderPackageData> RETURNORDERPACKAGE = d_ReturnOrderPackageData.ConvertDataTable<OrderPackageData>();
        //                    result.ReturnOrderPackage = RETURNORDERPACKAGE;
        //                }
        //            }
        //            if (executeResult.Count >= 4)
        //            {
        //                DataTable d_ReturnOrderContactData = (DataTable)executeResult[3];
        //                if (d_ReturnOrderContactData != null && d_ReturnOrderContactData.Rows.Count > 0)
        //                {
        //                    List<OrderContactData> RETURNORDERCONTACT = d_ReturnOrderContactData.ConvertDataTable<OrderContactData>();
        //                    result.ReturnOrderContact = RETURNORDERCONTACT;
        //                }
        //            }
        //            if (executeResult.Count >= 5)
        //            {
        //                DataTable d_ReturnOrderBillMediaData = (DataTable)executeResult[4];
        //                if (d_ReturnOrderBillMediaData != null && d_ReturnOrderBillMediaData.Rows.Count > 0)
        //                {
        //                    List<OrderBillMediaData> RETURNORDERBILLMEDIA = d_ReturnOrderBillMediaData.ConvertDataTable<OrderBillMediaData>();
        //                    result.ReturnOrderBillMedia = RETURNORDERBILLMEDIA;
        //                }
        //            }
        //            if (executeResult.Count >= 6)
        //            {
        //                DataTable d_ReturnBillingAddressData = (DataTable)executeResult[5];
        //                if (d_ReturnBillingAddressData != null && d_ReturnBillingAddressData.Rows.Count > 0)
        //                {
        //                    List<BillingAddressData> RETURNBILLINGADDRESS = d_ReturnBillingAddressData.ConvertDataTable<BillingAddressData>();
        //                    result.ReturnBillingAddress = RETURNBILLINGADDRESS;
        //                }
        //            }
        //            if (executeResult.Count >= 7)
        //            {
        //                DataTable d_ReturnInstallAddressData = (DataTable)executeResult[6];
        //                if (d_ReturnInstallAddressData != null && d_ReturnInstallAddressData.Rows.Count > 0)
        //                {
        //                    List<InstallAddressData> RETURNINSTALLADDRESS = d_ReturnInstallAddressData.ConvertDataTable<InstallAddressData>();
        //                    result.ReturnInstallAddress = RETURNINSTALLADDRESS;
        //                }
        //            }
        //            if (executeResult.Count >= 8)
        //            {
        //                DataTable d_ReturnOrderDocumentData = (DataTable)executeResult[7];
        //                if (d_ReturnOrderDocumentData != null && d_ReturnOrderDocumentData.Rows.Count > 0)
        //                {
        //                    List<OrderDocumentData> RETURNORDERDOCUMENT = d_ReturnOrderDocumentData.ConvertDataTable<OrderDocumentData>();
        //                    result.ReturnOrderDocument = RETURNORDERDOCUMENT;
        //                }
        //            }
        //            if (executeResult.Count >= 9)
        //            {
        //                DataTable d_ReturnForOfficerData = (DataTable)executeResult[8];
        //                if (d_ReturnForOfficerData != null && d_ReturnForOfficerData.Rows.Count > 0)
        //                {
        //                    List<ForOfficerData> RETURNFOROFFICER = d_ReturnForOfficerData.ConvertDataTable<ForOfficerData>();
        //                    result.ReturnForOfficer = RETURNFOROFFICER;
        //                }
        //            }

        //            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "", "");

        //        }
        //        else
        //        {
        //            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, query, log, "Failed", "Failed", "");
        //            result.ReturnCode = "-1";
        //            result.ReturnMessage = "Error";

        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Info(ex.Message);
        //        _logger.Info("Error call service FBBADM.PKG_FBB_QUERY_ORDER.QUERY_ORDER_DETAIL" + ex.Message);
        //        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, query, log, "Failed", ex.Message, "");
        //        result.ReturnCode = "-1";
        //        result.ReturnMessage = "Error";
        //    }

        //    return result;
        //}
        #endregion
    }


    #region Mapping ListOrder Type Oracle
    public class ListOrderObjectModel : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
    {
        [OracleArrayMapping()]
        public ListOrderOracleTypeMapping[] ListOrder { get; set; }

        private bool objectIsNull;

        public bool IsNull
        {
            get { return objectIsNull; }
        }

        public static ListOrderObjectModel Null
        {
            get
            {
                ListOrderObjectModel obj = new ListOrderObjectModel();
                obj.objectIsNull = true;
                return obj;
            }
        }

        public void FromCustomObject(OracleConnection con, object udt)
        {
            OracleUdt.SetValue(con, udt, 0, ListOrder);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            ListOrder = (ListOrderOracleTypeMapping[])OracleUdt.GetValue(con, udt, 0);
        }
    }

    [OracleCustomTypeMappingAttribute("FBBADM.FBB_LIST_ORDER_RECORD")]
    public class ListOrderOracleTypeMappingFactory : IOracleCustomTypeFactory
    {
        #region IOracleCustomTypeFactory Members

        public IOracleCustomType CreateObject()
        {
            return new ListOrderOracleTypeMapping();
        }

        #endregion IOracleCustomTypeFactory Members
    }

    [OracleCustomTypeMapping("FBBADM.FBB_LIST_ORDER_ARRAY")]
    public class ListOrderObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
    {
        #region IOracleCustomTypeFactory Members

        public IOracleCustomType CreateObject()
        {
            return new ListOrderObjectModel();
        }

        #endregion IOracleCustomTypeFactory Members

        #region IOracleArrayTypeFactory Members

        public Array CreateArray(int numElems)
        {
            return new ListOrderOracleTypeMapping[numElems];
        }

        public Array CreateStatusArray(int numElems)
        {
            return null;
        }

        #endregion IOracleArrayTypeFactory Members
    }

    public class ListOrderOracleTypeMapping : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
    {
        private bool objectIsNull;

        #region Attribute Mapping

        [OracleObjectMappingAttribute("ORDERNO")]
        public string OrderNo { get; set; }

        #endregion Attribute Mapping

        public static ListOrderOracleTypeMapping Null
        {
            get
            {
                ListOrderOracleTypeMapping obj = new ListOrderOracleTypeMapping();
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
            OracleUdt.SetValue(con, udt, "ORDERNO", OrderNo);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            throw new NotImplementedException();
        }
    }
    #endregion Mapping ListOrder Type Oracle

    #region Mapping ListCardNo Type Oracle
    public class ListCardNoObjectModel : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
    {
        [OracleArrayMapping()]
        public ListCardNoOracleTypeMapping[] ListCardNo { get; set; }

        private bool objectIsNull;

        public bool IsNull
        {
            get { return objectIsNull; }
        }

        public static ListCardNoObjectModel Null
        {
            get
            {
                ListCardNoObjectModel obj = new ListCardNoObjectModel();
                obj.objectIsNull = true;
                return obj;
            }
        }

        public void FromCustomObject(OracleConnection con, object udt)
        {
            OracleUdt.SetValue(con, udt, 0, ListCardNo);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            ListCardNo = (ListCardNoOracleTypeMapping[])OracleUdt.GetValue(con, udt, 0);
        }
    }

    [OracleCustomTypeMappingAttribute("FBBADM.FBB_LIST_CARD_NO_RECORD")]
    public class ListCardNoOracleTypeMappingFactory : IOracleCustomTypeFactory
    {
        #region IOracleCustomTypeFactory Members

        public IOracleCustomType CreateObject()
        {
            return new ListCardNoOracleTypeMapping();
        }

        #endregion IOracleCustomTypeFactory Members
    }

    [OracleCustomTypeMapping("FBBADM.FBB_LIST_CARD_NO_ARRAY")]
    public class ListCardNoObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
    {
        #region IOracleCustomTypeFactory Members

        public IOracleCustomType CreateObject()
        {
            return new ListCardNoObjectModel();
        }

        #endregion IOracleCustomTypeFactory Members

        #region IOracleArrayTypeFactory Members

        public Array CreateArray(int numElems)
        {
            return new ListCardNoOracleTypeMapping[numElems];
        }

        public Array CreateStatusArray(int numElems)
        {
            return null;
        }

        #endregion IOracleArrayTypeFactory Members
    }

    public class ListCardNoOracleTypeMapping : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
    {
        private bool objectIsNull;

        #region Attribute Mapping

        [OracleObjectMappingAttribute("CARDNO")]
        public string CardNo { get; set; }

        #endregion Attribute Mapping

        public static ListCardNoOracleTypeMapping Null
        {
            get
            {
                ListCardNoOracleTypeMapping obj = new ListCardNoOracleTypeMapping();
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
            OracleUdt.SetValue(con, udt, "CARDNO", CardNo);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            throw new NotImplementedException();
        }
    }
    #endregion Mapping ListCardNo Type Oracle

    #region Mapping ListNonMobileNo Type Oracle
    public class ListNonMobileNoObjectModel : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
    {
        [OracleArrayMapping()]
        public ListNonMobileNoOracleTypeMapping[] ListNonMobileNo { get; set; }

        private bool objectIsNull;

        public bool IsNull
        {
            get { return objectIsNull; }
        }

        public static ListNonMobileNoObjectModel Null
        {
            get
            {
                ListNonMobileNoObjectModel obj = new ListNonMobileNoObjectModel();
                obj.objectIsNull = true;
                return obj;
            }
        }

        public void FromCustomObject(OracleConnection con, object udt)
        {
            OracleUdt.SetValue(con, udt, 0, ListNonMobileNo);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            ListNonMobileNo = (ListNonMobileNoOracleTypeMapping[])OracleUdt.GetValue(con, udt, 0);
        }
    }

    [OracleCustomTypeMappingAttribute("FBBADM.FBB_LIST_NON_MOBILE_NO_RECORD")]
    public class ListNonMobileNoOracleTypeMappingFactory : IOracleCustomTypeFactory
    {
        #region IOracleCustomTypeFactory Members

        public IOracleCustomType CreateObject()
        {
            return new ListNonMobileNoOracleTypeMapping();
        }

        #endregion IOracleCustomTypeFactory Members
    }

    [OracleCustomTypeMapping("FBBADM.FBB_LIST_NON_MOBILE_NO_ARRAY")]
    public class ListNonMobileNoObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
    {
        #region IOracleCustomTypeFactory Members

        public IOracleCustomType CreateObject()
        {
            return new ListNonMobileNoObjectModel();
        }

        #endregion IOracleCustomTypeFactory Members

        #region IOracleArrayTypeFactory Members

        public Array CreateArray(int numElems)
        {
            return new ListNonMobileNoOracleTypeMapping[numElems];
        }

        public Array CreateStatusArray(int numElems)
        {
            return null;
        }

        #endregion IOracleArrayTypeFactory Members
    }

    public class ListNonMobileNoOracleTypeMapping : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
    {
        private bool objectIsNull;

        #region Attribute Mapping

        [OracleObjectMappingAttribute("NONMOBILENO")]
        public string NonMobileNo { get; set; }

        #endregion Attribute Mapping

        public static ListNonMobileNoOracleTypeMapping Null
        {
            get
            {
                ListNonMobileNoOracleTypeMapping obj = new ListNonMobileNoOracleTypeMapping();
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
            OracleUdt.SetValue(con, udt, "NONMOBILENO", NonMobileNo);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            throw new NotImplementedException();
        }
    }
    #endregion Mapping ListNonMobileNo Type Oracle

    #region Mapping ListContactMobileNo Type Oracle
    public class ListContactMobileNoObjectModel : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
    {
        [OracleArrayMapping()]
        public ListContactMobileNoOracleTypeMapping[] ListContactMobileNo { get; set; }

        private bool objectIsNull;

        public bool IsNull
        {
            get { return objectIsNull; }
        }

        public static ListContactMobileNoObjectModel Null
        {
            get
            {
                ListContactMobileNoObjectModel obj = new ListContactMobileNoObjectModel();
                obj.objectIsNull = true;
                return obj;
            }
        }

        public void FromCustomObject(OracleConnection con, object udt)
        {
            OracleUdt.SetValue(con, udt, 0, ListContactMobileNo);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            ListContactMobileNo = (ListContactMobileNoOracleTypeMapping[])OracleUdt.GetValue(con, udt, 0);
        }
    }

    [OracleCustomTypeMappingAttribute("FBBADM.FBB_LIST_CONTACT_MOBILE_RECORD")]
    public class ListContactMobileNoOracleTypeMappingFactory : IOracleCustomTypeFactory
    {
        #region IOracleCustomTypeFactory Members

        public IOracleCustomType CreateObject()
        {
            return new ListContactMobileNoOracleTypeMapping();
        }

        #endregion IOracleCustomTypeFactory Members
    }

    [OracleCustomTypeMapping("FBBADM.FBB_LIST_CONTACT_MOBILE_ARRAY")]
    public class ListContactMobileNoObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
    {
        #region IOracleCustomTypeFactory Members

        public IOracleCustomType CreateObject()
        {
            return new ListContactMobileNoObjectModel();
        }

        #endregion IOracleCustomTypeFactory Members

        #region IOracleArrayTypeFactory Members

        public Array CreateArray(int numElems)
        {
            return new ListContactMobileNoOracleTypeMapping[numElems];
        }

        public Array CreateStatusArray(int numElems)
        {
            return null;
        }

        #endregion IOracleArrayTypeFactory Members
    }

    public class ListContactMobileNoOracleTypeMapping : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
    {
        private bool objectIsNull;

        #region Attribute Mapping

        [OracleObjectMappingAttribute("CONTACTMOBILENO")]
        public string ContactMobileNo { get; set; }

        #endregion Attribute Mapping

        public static ListContactMobileNoOracleTypeMapping Null
        {
            get
            {
                ListContactMobileNoOracleTypeMapping obj = new ListContactMobileNoOracleTypeMapping();
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
            OracleUdt.SetValue(con, udt, "CONTACTMOBILENO", ContactMobileNo);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            throw new NotImplementedException();
        }
    }
    #endregion Mapping ListContactMobileNo Type Oracle

}
