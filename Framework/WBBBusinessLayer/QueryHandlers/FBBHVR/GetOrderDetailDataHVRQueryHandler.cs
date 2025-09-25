using Npgsql;
using NpgsqlTypes;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.FBBHVR;
using WBBContract.Queries.FBBShareplex;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.FBBHVRModels;
using WBBEntity.FBBShareplexModels;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.FBBHVR
{
    public class GetOrderDetailDataHVRQueryHandler : IQueryHandler<GetOrderDetailDataHVRQuery, GetOrderDetailModel>
    {
        
        #region New Version For Shareplex to HVR PostgreSQL R23.06

        private readonly ILogger _logger;
        private readonly IFBBHVREntityRepository<object> _fbbHVRRepository;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public GetOrderDetailDataHVRQueryHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow,
            IFBBHVREntityRepository<object> fbbHVRRepository)
        {
            _logger = logger;
            _fbbHVRRepository = fbbHVRRepository;
            _intfLog = intfLog;
            _uow = uow;
        }

        public GetOrderDetailModel Handle(GetOrderDetailDataHVRQuery query)
        {
            var result = new GetOrderDetailModel();
            InterfaceLogCommand log = null;
            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, "" + query.TransactionID, "GetOrderDetailData", "GetOrderDetailDataQuery", null, "FBB|", "WEB");

            List<HvrListOrderRecord> ListOrderReccord = query.ListOrder.Select(c => new HvrListOrderRecord
            {
                orderno = c.OrderNo.ToSafeString()
            }).ToList();

            List<HvrListCardNoRecord> ListCardNoReccord = query.ListCardNo.Select(c => new HvrListCardNoRecord
            {
                cardno = c.CardNo.ToSafeString()
            }).ToList();

            List<HvrListNonMobileNoRecord> ListNonMobileNoReccord = query.ListNonMobileNo.Select(c => new HvrListNonMobileNoRecord
            {
                nonmobileno = c.NonMobileNo.ToSafeString()
            }).ToList();

            List<HvrListContactMobileNoRecord> ListContactMobileNoReccord = query.ListContactMobileNo.Select(c => new HvrListContactMobileNoRecord
            {
                contactmobileno = c.ContactMobileNo.ToSafeString()
            }).ToList();

            var listorder = new NpgsqlParameter();
            listorder.ParameterName = "listorder";
            listorder.Direction = ParameterDirection.Input;
            listorder.Value = ListOrderReccord.ToArray();

            var listcardno = new NpgsqlParameter();
            listcardno.ParameterName = "listcardno";
            listcardno.Direction = ParameterDirection.Input;
            listcardno.Value = ListCardNoReccord.ToArray();

            var listnonmobileno = new NpgsqlParameter();
            listnonmobileno.ParameterName = "listnonmobileno";
            listnonmobileno.Direction = ParameterDirection.Input;
            listnonmobileno.Value = ListNonMobileNoReccord.ToArray();

            var listcontactmobileno = new NpgsqlParameter();
            listcontactmobileno.ParameterName = "listcontactmobileno";
            listcontactmobileno.Direction = ParameterDirection.Input;
            listcontactmobileno.Value = ListContactMobileNoReccord.ToArray();

            var customername = new NpgsqlParameter();
            customername.ParameterName = "customername";
            customername.Size = 2000;
            customername.NpgsqlDbType = NpgsqlDbType.Text;
            customername.Direction = ParameterDirection.Input;
            customername.Value = query.CustomerName.ToSafeString();

            var customerlastname = new NpgsqlParameter();
            customerlastname.ParameterName = "customerlastname";
            customerlastname.Size = 2000;
            customerlastname.NpgsqlDbType = NpgsqlDbType.Text;
            customerlastname.Direction = ParameterDirection.Input;
            customerlastname.Value = query.CustomerLastName.ToSafeString();

            var returncode_cur = new NpgsqlParameter();
            returncode_cur.ParameterName = "returncode_cur";
            returncode_cur.NpgsqlDbType = NpgsqlDbType.Refcursor;
            returncode_cur.Direction = ParameterDirection.InputOutput;
            returncode_cur.Value = "returncode_cur";

            var returnordercustomer = new NpgsqlParameter();
            returnordercustomer.ParameterName = "returnordercustomer";
            returnordercustomer.NpgsqlDbType = NpgsqlDbType.Refcursor;
            returnordercustomer.Direction = ParameterDirection.InputOutput;
            returnordercustomer.Value = "returnordercustomer";

            var returnorderpackage = new NpgsqlParameter();
            returnorderpackage.ParameterName = "returnorderpackage";
            returnorderpackage.NpgsqlDbType = NpgsqlDbType.Refcursor;
            returnorderpackage.Direction = ParameterDirection.InputOutput;
            returnorderpackage.Value = "returnorderpackage";

            var returnordercontact = new NpgsqlParameter();
            returnordercontact.ParameterName = "returnordercontact";
            returnordercontact.NpgsqlDbType = NpgsqlDbType.Refcursor;
            returnordercontact.Direction = ParameterDirection.InputOutput;
            returnordercontact.Value = "returnordercontact";

            var returnorderbillmedia = new NpgsqlParameter();
            returnorderbillmedia.ParameterName = "returnorderbillmedia";
            returnorderbillmedia.NpgsqlDbType = NpgsqlDbType.Refcursor;
            returnorderbillmedia.Direction = ParameterDirection.InputOutput;
            returnorderbillmedia.Value = "returnorderbillmedia";

            var returnbillingaddress = new NpgsqlParameter();
            returnbillingaddress.ParameterName = "returnbillingaddress";
            returnbillingaddress.NpgsqlDbType = NpgsqlDbType.Refcursor;
            returnbillingaddress.Direction = ParameterDirection.InputOutput;
            returnbillingaddress.Value = "returnbillingaddress";

            var returninstalladdress = new NpgsqlParameter();
            returninstalladdress.ParameterName = "returninstalladdress";
            returninstalladdress.NpgsqlDbType = NpgsqlDbType.Refcursor;
            returninstalladdress.Direction = ParameterDirection.InputOutput;
            returninstalladdress.Value = "returninstalladdress";

            var returnorderdocument = new NpgsqlParameter();
            returnorderdocument.ParameterName = "returnorderdocument";
            returnorderdocument.NpgsqlDbType = NpgsqlDbType.Refcursor;
            returnorderdocument.Direction = ParameterDirection.InputOutput;
            returnorderdocument.Value = "returnorderdocument";

            var returnforofficer = new NpgsqlParameter();
            returnforofficer.ParameterName = "returnforofficer";
            returnforofficer.NpgsqlDbType = NpgsqlDbType.Refcursor;
            returnforofficer.Direction = ParameterDirection.InputOutput;
            returnforofficer.Value = "returnforofficer";

            try
            {
                var executeResult = _fbbHVRRepository.ExecuteStoredProcMultipleCursorNpgsql("fbbadm.pkg_fbb_query_order_query_order_detail",
                    new object[]
                    {
                        listorder,
                        listcardno,
                        listnonmobileno,
                        listcontactmobileno,
                        customername,
                        customerlastname,

                        // return
                        returncode_cur,

                        returnordercustomer,
                        returnorderpackage,
                        returnordercontact,
                        returnorderbillmedia,
                        returnbillingaddress,
                        returninstalladdress,
                        returnorderdocument,
                        returnforofficer
                    }).ToList();

                if (executeResult != null)
                {
                    DataTable dtr1 = (DataTable)executeResult[0];
                    List<GetOrderDetailModel> _cur1 = dtr1.ConvertDataTable<GetOrderDetailModel>();

                    var _first = _cur1.FirstOrDefault();
                    result.ReturnCode = _first.ReturnCode != null ? _first.ReturnCode.ToString() : "-1";
                    result.ReturnMessage = _first.ReturnMessage != null ? _first.ReturnMessage.ToString() : "";

                    if (executeResult.Count >= 2)
                    {
                        DataTable d_ReturnOrderCustomerData = (DataTable)executeResult[1];
                        if (d_ReturnOrderCustomerData != null && d_ReturnOrderCustomerData.Rows.Count > 0)
                        {
                            List<OrderCustomerData> RETURNORDERCUSTOMER = d_ReturnOrderCustomerData.ConvertDataTable<OrderCustomerData>();
                            result.ReturnOrderCustomer = RETURNORDERCUSTOMER;
                        }
                    }
                    if (executeResult.Count >= 3)
                    {
                        DataTable d_ReturnOrderPackageData = (DataTable)executeResult[2];
                        if (d_ReturnOrderPackageData != null && d_ReturnOrderPackageData.Rows.Count > 0)
                        {
                            List<OrderPackageData> RETURNORDERPACKAGE = d_ReturnOrderPackageData.ConvertDataTable<OrderPackageData>();
                            result.ReturnOrderPackage = RETURNORDERPACKAGE;
                        }
                    }
                    if (executeResult.Count >= 4)
                    {
                        DataTable d_ReturnOrderContactData = (DataTable)executeResult[3];
                        if (d_ReturnOrderContactData != null && d_ReturnOrderContactData.Rows.Count > 0)
                        {
                            List<OrderContactData> RETURNORDERCONTACT = d_ReturnOrderContactData.ConvertDataTable<OrderContactData>();
                            result.ReturnOrderContact = RETURNORDERCONTACT;
                        }
                    }
                    if (executeResult.Count >= 5)
                    {
                        DataTable d_ReturnOrderBillMediaData = (DataTable)executeResult[4];
                        if (d_ReturnOrderBillMediaData != null && d_ReturnOrderBillMediaData.Rows.Count > 0)
                        {
                            List<OrderBillMediaData> RETURNORDERBILLMEDIA = d_ReturnOrderBillMediaData.ConvertDataTable<OrderBillMediaData>();
                            result.ReturnOrderBillMedia = RETURNORDERBILLMEDIA;
                        }
                    }
                    if (executeResult.Count >= 6)
                    {
                        DataTable d_ReturnBillingAddressData = (DataTable)executeResult[5];
                        if (d_ReturnBillingAddressData != null && d_ReturnBillingAddressData.Rows.Count > 0)
                        {
                            List<BillingAddressData> RETURNBILLINGADDRESS = d_ReturnBillingAddressData.ConvertDataTable<BillingAddressData>();
                            result.ReturnBillingAddress = RETURNBILLINGADDRESS;
                        }
                    }
                    if (executeResult.Count >= 7)
                    {
                        DataTable d_ReturnInstallAddressData = (DataTable)executeResult[6];
                        if (d_ReturnInstallAddressData != null && d_ReturnInstallAddressData.Rows.Count > 0)
                        {
                            List<InstallAddressData> RETURNINSTALLADDRESS = d_ReturnInstallAddressData.ConvertDataTable<InstallAddressData>();
                            result.ReturnInstallAddress = RETURNINSTALLADDRESS;
                        }
                    }
                    if (executeResult.Count >= 8)
                    {
                        DataTable d_ReturnOrderDocumentData = (DataTable)executeResult[7];
                        if (d_ReturnOrderDocumentData != null && d_ReturnOrderDocumentData.Rows.Count > 0)
                        {
                            List<OrderDocumentData> RETURNORDERDOCUMENT = d_ReturnOrderDocumentData.ConvertDataTable<OrderDocumentData>();
                            result.ReturnOrderDocument = RETURNORDERDOCUMENT;
                        }
                    }
                    if (executeResult.Count >= 9)
                    {
                        DataTable d_ReturnForOfficerData = (DataTable)executeResult[8];
                        if (d_ReturnForOfficerData != null && d_ReturnForOfficerData.Rows.Count > 0)
                        {
                            List<ForOfficerData> RETURNFOROFFICER = d_ReturnForOfficerData.ConvertDataTable<ForOfficerData>();
                            result.ReturnForOfficer = RETURNFOROFFICER;
                        }
                    }

                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "", "");

                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, query, log, "Failed", "Failed", "");
                    result.ReturnCode = "-1";
                    result.ReturnMessage = "Error";

                }

            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);
                _logger.Info("Error call service fbbadm.pkg_fbb_query_order_query_order_detail" + ex.Message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, query, log, "Failed", ex.Message, "");
                result.ReturnCode = "-1";
                result.ReturnMessage = "Error";
            }

            return result;
        }
        #endregion
    }
}
