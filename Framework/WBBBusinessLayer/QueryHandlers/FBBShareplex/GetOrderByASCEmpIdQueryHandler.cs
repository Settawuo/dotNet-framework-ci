using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
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
    public class GetOrderByASCEmpIdQueryHandler : IQueryHandler<GetOrderByASCEmpIdDataQuery, GetOrderByASCEmpIdModel>
    {
        #region Old Version Shareplex
        private readonly ILogger _logger;
        private readonly IFBBShareplexEntityRepository<object> _fbbShareplexRepository;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public GetOrderByASCEmpIdQueryHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow,
            IFBBShareplexEntityRepository<object> fbbShareplexRepository)
        {
            _logger = logger;
            _fbbShareplexRepository = fbbShareplexRepository;
            _intfLog = intfLog;
            _uow = uow;
        }

        public GetOrderByASCEmpIdModel Handle(GetOrderByASCEmpIdDataQuery query)
        {
            InterfaceLogCommand log = null;
            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, "" + query.TransactionID, "GetOrderByASCEmpId", "GetOrderByASCEmpIdDataQuery", null, "FBB|", "WEB");

            GetOrderByASCEmpIdModel executeResults = new GetOrderByASCEmpIdModel();

            var ASCCode = new OracleParameter();
            ASCCode.ParameterName = "ASCCode";
            ASCCode.Size = 2000;
            ASCCode.OracleDbType = OracleDbType.Varchar2;
            ASCCode.Direction = ParameterDirection.Input;
            ASCCode.Value = query.ASCCode;

            var EmployeeId = new OracleParameter();
            EmployeeId.ParameterName = "EmployeeId";
            EmployeeId.Size = 2000;
            EmployeeId.OracleDbType = OracleDbType.Varchar2;
            EmployeeId.Direction = ParameterDirection.Input;
            EmployeeId.Value = query.EmployeeId;

            var DateFrom = new OracleParameter();
            DateFrom.ParameterName = "DateFrom";
            DateFrom.Size = 2000;
            DateFrom.OracleDbType = OracleDbType.Varchar2;
            DateFrom.Direction = ParameterDirection.Input;
            DateFrom.Value = query.DateFrom;

            var DateTo = new OracleParameter();
            DateTo.ParameterName = "DateTo";
            DateTo.Size = 2000;
            DateTo.OracleDbType = OracleDbType.Varchar2;
            DateTo.Direction = ParameterDirection.Input;
            DateTo.Value = query.DateTo;

            var LocationCode = new OracleParameter();
            LocationCode.ParameterName = "LocationCode";
            LocationCode.Size = 2000;
            LocationCode.OracleDbType = OracleDbType.Varchar2;
            LocationCode.Direction = ParameterDirection.Input;
            LocationCode.Value = query.LocationCode;

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

            var TotalKeyIn = new OracleParameter();
            TotalKeyIn.ParameterName = "TotalKeyIn";
            TotalKeyIn.OracleDbType = OracleDbType.Varchar2;
            TotalKeyIn.Size = 2000;
            TotalKeyIn.Direction = ParameterDirection.Output;

            var TotalComplete = new OracleParameter();
            TotalComplete.ParameterName = "TotalComplete";
            TotalComplete.OracleDbType = OracleDbType.Varchar2;
            TotalComplete.Size = 2000;
            TotalComplete.Direction = ParameterDirection.Output;

            var TotalCancel = new OracleParameter();
            TotalCancel.ParameterName = "TotalCancel";
            TotalCancel.OracleDbType = OracleDbType.Varchar2;
            TotalCancel.Size = 2000;
            TotalCancel.Direction = ParameterDirection.Output;

            var ReturnOrderDetail = new OracleParameter();
            ReturnOrderDetail.ParameterName = "ReturnOrderDetail";
            ReturnOrderDetail.OracleDbType = OracleDbType.RefCursor;
            ReturnOrderDetail.Direction = ParameterDirection.Output;

            try
            {
                var result = _fbbShareplexRepository.ExecuteStoredProcMultipleCursor("FBBADM.PKG_FBB_QUERY_ORDER.QUERY_ORDER_BY_ASC_EMPID",
                    new object[]
                    {
                        ASCCode,
                        EmployeeId,
                        DateFrom,
                        DateTo,
                        LocationCode,

                        // return code
                        ReturnCode,
                        ReturnMessage,
                        TotalKeyIn,
                        TotalComplete,
                        TotalCancel,
                        ReturnOrderDetail
                    });

                if (result != null)
                {
                    executeResults.ReturnCode = result[0] != null ? result[0].ToString() : "-1";
                    executeResults.ReturnMessage = result[1] != null ? result[1].ToString() : "";
                    executeResults.TotalKeyIn = result[2] != null ? result[2].ToString() : "";
                    executeResults.TotalComplete = result[3] != null ? result[3].ToString() : "";
                    executeResults.TotalCancel = result[4] != null ? result[4].ToString() : "";

                    var d_ReturnOrderDetailData = (DataTable)result[5];
                    var RETURNORDERDETAILDATA = d_ReturnOrderDetailData.DataTableToList<ReturnOrderDetailData>();
                    executeResults.ReturnOrderDetail = RETURNORDERDETAILDATA;

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
                _logger.Info("Error call service FBBADM.PKG_FBB_QUERY_ORDER.QUERY_ORDER_BY_ASC_EMPID" + ex.Message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, query, log, "Failed", ex.Message, "");
                executeResults.ReturnCode = "-1";
                executeResults.ReturnMessage = "Error";
            }

            return executeResults;
        }
        #endregion Old Version Shareplex

        #region New Version For Shareplex to HVR PostgreSQL R23.06

        //private readonly ILogger _logger;
        //private readonly IFBBHVREntityRepository<object> _fbbHVRRepository;
        //private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        //private readonly IWBBUnitOfWork _uow;

        //public GetOrderByASCEmpIdQueryHandler(ILogger logger,
        //    IEntityRepository<FBB_INTERFACE_LOG> intfLog,
        //    IWBBUnitOfWork uow,
        //    IFBBHVREntityRepository<object> fbbHVRRepository)
        //{
        //    _logger = logger;
        //    _fbbHVRRepository = fbbHVRRepository;
        //    _intfLog = intfLog;
        //    _uow = uow;
        //}

        //public GetOrderByASCEmpIdModel Handle(GetOrderByASCEmpIdDataQuery query)
        //{
        //    InterfaceLogCommand log = null;
        //    log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, "" + query.TransactionID, "GetOrderByASCEmpId", "GetOrderByASCEmpIdDataQuery", null, "FBB|", "WEB");

        //    GetOrderByASCEmpIdModel executeResults = new GetOrderByASCEmpIdModel();

        //    var asccode = new NpgsqlParameter();
        //    asccode.ParameterName = "asccode";
        //    asccode.Size = 2000;
        //    asccode.NpgsqlDbType = NpgsqlDbType.Text;
        //    asccode.Direction = ParameterDirection.Input;
        //    asccode.Value = query.ASCCode;

        //    var employeeid = new NpgsqlParameter();
        //    employeeid.ParameterName = "employeeid";
        //    employeeid.Size = 2000;
        //    employeeid.NpgsqlDbType = NpgsqlDbType.Text;
        //    employeeid.Direction = ParameterDirection.Input;
        //    employeeid.Value = query.EmployeeId;

        //    var datefrom = new NpgsqlParameter();
        //    datefrom.ParameterName = "datefrom";
        //    datefrom.Size = 2000;
        //    datefrom.NpgsqlDbType = NpgsqlDbType.Text;
        //    datefrom.Direction = ParameterDirection.Input;
        //    datefrom.Value = query.DateFrom;

        //    var dateto = new NpgsqlParameter();
        //    dateto.ParameterName = "dateto";
        //    dateto.Size = 2000;
        //    dateto.NpgsqlDbType = NpgsqlDbType.Text;
        //    dateto.Direction = ParameterDirection.Input;
        //    dateto.Value = query.DateTo;

        //    var locationcode = new NpgsqlParameter();
        //    locationcode.ParameterName = "locationcode";
        //    locationcode.Size = 2000;
        //    locationcode.NpgsqlDbType = NpgsqlDbType.Text;
        //    locationcode.Direction = ParameterDirection.Input;
        //    locationcode.Value = query.LocationCode;

        //    var multi_outputmessage = new NpgsqlParameter();
        //    multi_outputmessage.ParameterName = "multi_outputmessage";
        //    multi_outputmessage.NpgsqlDbType = NpgsqlDbType.Refcursor;
        //    multi_outputmessage.Direction = ParameterDirection.InputOutput;
        //    multi_outputmessage.Value = "multi_outputmessage";

        //    var returnorderdetail = new NpgsqlParameter();
        //    returnorderdetail.ParameterName = "returnorderdetail";
        //    returnorderdetail.NpgsqlDbType = NpgsqlDbType.Refcursor;
        //    returnorderdetail.Direction = ParameterDirection.InputOutput;
        //    returnorderdetail.Value = "returnorderdetail";

        //    try
        //    {
        //        var result = _fbbHVRRepository.ExecuteStoredProcMultipleCursorNpgsql("fbbadm.pkg_fbb_query_order_query_order_by_asc_empid",
        //            new object[]
        //            {
        //                asccode,
        //                employeeid,
        //                datefrom,
        //                dateto,
        //                locationcode,

        //                multi_outputmessage,
        //                returnorderdetail
        //            }).ToList();

        //        if (result != null)
        //        {
        //            DataTable dtr1 = (DataTable)result[0];
        //            DataTable dtr2 = (DataTable)result[1];
        //            List<GetOrderByASCEmpIdModel> _cur1 = dtr1.ConvertDataTable<GetOrderByASCEmpIdModel>();
        //            List<ReturnOrderDetailData> _cur2 = dtr2.ConvertDataTable<ReturnOrderDetailData>();


        //            if (_cur2 != null && _cur2.Count > 0)
        //            {
        //                var _first = _cur1.FirstOrDefault();
        //                executeResults.ReturnCode = _first.ReturnCode != null ? _first.ReturnCode.ToString() : "-1";
        //                executeResults.ReturnMessage = _first.ReturnMessage != null ? _first.ReturnMessage.ToString() : "";
        //                executeResults.TotalKeyIn = _first.TotalKeyIn != null ? _first.TotalKeyIn.ToString() : "";
        //                executeResults.TotalComplete = _first.TotalComplete != null ? _first.TotalComplete.ToString() : "";
        //                executeResults.TotalCancel = _first.TotalCancel != null ? _first.TotalCancel.ToString() : "";
        //                executeResults.ReturnOrderDetail = _cur2.Select(t => new ReturnOrderDetailData()
        //                {
        //                    OrderNo = t.OrderNo,
        //                    RegisterDate = t.RegisterDate,
        //                    CustomerName = t.CustomerName,
        //                    PrivilegeNo = t.PrivilegeNo,
        //                    MainPackage = t.MainPackage,
        //                    NotifyDetail = t.NotifyDetail,
        //                    NoteDetail = t.NoteDetail,
        //                    OrderExpireDate = t.OrderExpireDate,
        //                    AppointmentDate = t.AppointmentDate,
        //                    CancelDate = t.CancelDate,
        //                    RegisterChannel = t.RegisterChannel,
        //                    OrderStatus = t.OrderStatus
        //                }).ToList();
        //            }
        //            else
        //            {
        //                var _first = _cur1.FirstOrDefault();
        //                executeResults.ReturnCode = _first.ReturnCode != null ? _first.ReturnCode.ToString() : "-1";
        //                executeResults.ReturnMessage = _first.ReturnMessage != null ? _first.ReturnMessage.ToString() : "";
        //                executeResults.TotalKeyIn = _first.TotalKeyIn != null ? _first.TotalKeyIn.ToString() : "";
        //                executeResults.TotalComplete = _first.TotalComplete != null ? _first.TotalComplete.ToString() : "";
        //                executeResults.TotalCancel = _first.TotalCancel != null ? _first.TotalCancel.ToString() : "";
        //                executeResults.ReturnMessage = string.IsNullOrEmpty(_cur1.FirstOrDefault().ReturnMessage) ? "No data found" : _cur1.FirstOrDefault().ReturnMessage;
        //            }

        //            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ConvertToJson(result), log, "Success", "", "");

        //        }
        //        else
        //        {
        //            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, query, log, "Failed", "Failed", "");
        //            executeResults.ReturnCode = "-1";
        //            executeResults.ReturnMessage = "Error";

        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Info(ex.Message);
        //        _logger.Info("Error call service FBBADM.PKG_FBB_QUERY_ORDER.QUERY_ORDER_BY_ASC_EMPID" + ex.Message);
        //        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, query, log, "Failed", ex.Message, "");
        //        executeResults.ReturnCode = "-1";
        //        executeResults.ReturnMessage = "Error";
        //    }

        //    return executeResults;
        //}

        //private string ConvertToJson<T>(T value)
        //{
        //    if (value == null)
        //    {
        //        return "";
        //    }

        //    try
        //    {
        //        return JsonConvert.SerializeObject(value);
        //    }
        //    catch (Exception)
        //    {
        //        return "";
        //    }
        //}

        #endregion New Version For Shareplex to HVR PostgreSQL R23.06
    }
}
