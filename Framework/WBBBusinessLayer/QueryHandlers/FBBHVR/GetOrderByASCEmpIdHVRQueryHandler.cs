using Newtonsoft.Json;
using Npgsql;
using NpgsqlTypes;
using Oracle.ManagedDataAccess.Client;
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
    public class GetOrderByASCEmpIdHVRQueryHandler : IQueryHandler<GetOrderByASCEmpIdDataHVRQuery, GetOrderByASCEmpIdModel>
    {
        
        #region New Version For Shareplex to HVR PostgreSQL R24.04

        private readonly ILogger _logger;
        private readonly IFBBHVREntityRepository<object> _fbbHVRRepository;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public GetOrderByASCEmpIdHVRQueryHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow,
            IFBBHVREntityRepository<object> fbbHVRRepository)
        {
            _logger = logger;
            _fbbHVRRepository = fbbHVRRepository;
            _intfLog = intfLog;
            _uow = uow;
        }

        public GetOrderByASCEmpIdModel Handle(GetOrderByASCEmpIdDataHVRQuery query)
        {
            InterfaceLogCommand log = null;
            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, "" + query.TransactionID, "GetOrderByASCEmpId", "GetOrderByASCEmpIdDataQuery", null, "FBB|", "WEB");

            GetOrderByASCEmpIdModel executeResults = new GetOrderByASCEmpIdModel();

            var asccode = new NpgsqlParameter();
            asccode.ParameterName = "asccode";
            asccode.Size = 2000;
            asccode.NpgsqlDbType = NpgsqlDbType.Text;
            asccode.Direction = ParameterDirection.Input;
            asccode.Value = query.ASCCode;

            var employeeid = new NpgsqlParameter();
            employeeid.ParameterName = "employeeid";
            employeeid.Size = 2000;
            employeeid.NpgsqlDbType = NpgsqlDbType.Text;
            employeeid.Direction = ParameterDirection.Input;
            employeeid.Value = query.EmployeeId;

            var datefrom = new NpgsqlParameter();
            datefrom.ParameterName = "datefrom";
            datefrom.Size = 2000;
            datefrom.NpgsqlDbType = NpgsqlDbType.Text;
            datefrom.Direction = ParameterDirection.Input;
            datefrom.Value = query.DateFrom;

            var dateto = new NpgsqlParameter();
            dateto.ParameterName = "dateto";
            dateto.Size = 2000;
            dateto.NpgsqlDbType = NpgsqlDbType.Text;
            dateto.Direction = ParameterDirection.Input;
            dateto.Value = query.DateTo;

            var locationcode = new NpgsqlParameter();
            locationcode.ParameterName = "locationcode";
            locationcode.Size = 2000;
            locationcode.NpgsqlDbType = NpgsqlDbType.Text;
            locationcode.Direction = ParameterDirection.Input;
            locationcode.Value = query.LocationCode;

            var multi_outputmessage = new NpgsqlParameter();
            multi_outputmessage.ParameterName = "multi_outputmessage";
            multi_outputmessage.NpgsqlDbType = NpgsqlDbType.Refcursor;
            multi_outputmessage.Direction = ParameterDirection.InputOutput;
            multi_outputmessage.Value = "multi_outputmessage";

            var returnorderdetail = new NpgsqlParameter();
            returnorderdetail.ParameterName = "returnorderdetail";
            returnorderdetail.NpgsqlDbType = NpgsqlDbType.Refcursor;
            returnorderdetail.Direction = ParameterDirection.InputOutput;
            returnorderdetail.Value = "returnorderdetail";

            try
            {
                var result = _fbbHVRRepository.ExecuteStoredProcMultipleCursorNpgsql("fbbadm.pkg_fbb_query_order_query_order_by_asc_empid",
                    new object[]
                    {
                        asccode,
                        employeeid,
                        datefrom,
                        dateto,
                        locationcode,

                        multi_outputmessage,
                        returnorderdetail
                    }).ToList();

                if (result != null)
                {
                    DataTable dtr1 = (DataTable)result[0];
                    DataTable dtr2 = (DataTable)result[1];
                    List<GetOrderByASCEmpIdModel> _cur1 = dtr1.ConvertDataTable<GetOrderByASCEmpIdModel>();
                    List<ReturnOrderDetailData> _cur2 = dtr2.ConvertDataTable<ReturnOrderDetailData>();


                    if (_cur2 != null && _cur2.Count > 0)
                    {
                        var _first = _cur1.FirstOrDefault();
                        executeResults.ReturnCode = _first.ReturnCode != null ? _first.ReturnCode.ToString() : "-1";
                        executeResults.ReturnMessage = _first.ReturnMessage != null ? _first.ReturnMessage.ToString() : "";
                        executeResults.TotalKeyIn = _first.TotalKeyIn != null ? _first.TotalKeyIn.ToString() : "";
                        executeResults.TotalComplete = _first.TotalComplete != null ? _first.TotalComplete.ToString() : "";
                        executeResults.TotalCancel = _first.TotalCancel != null ? _first.TotalCancel.ToString() : "";
                        executeResults.ReturnOrderDetail = _cur2.Select(t => new ReturnOrderDetailData()
                        {
                            OrderNo = t.OrderNo,
                            RegisterDate = t.RegisterDate,
                            CustomerName = t.CustomerName,
                            PrivilegeNo = t.PrivilegeNo,
                            MainPackage = t.MainPackage,
                            NotifyDetail = t.NotifyDetail,
                            NoteDetail = t.NoteDetail,
                            OrderExpireDate = t.OrderExpireDate,
                            AppointmentDate = t.AppointmentDate,
                            CancelDate = t.CancelDate,
                            RegisterChannel = t.RegisterChannel,
                            OrderStatus = t.OrderStatus
                        }).ToList();
                    }
                    else
                    {
                        var _first = _cur1.FirstOrDefault();
                        executeResults.ReturnCode = _first.ReturnCode != null ? _first.ReturnCode.ToString() : "-1";
                        executeResults.ReturnMessage = _first.ReturnMessage != null ? _first.ReturnMessage.ToString() : "";
                        executeResults.TotalKeyIn = _first.TotalKeyIn != null ? _first.TotalKeyIn.ToString() : "";
                        executeResults.TotalComplete = _first.TotalComplete != null ? _first.TotalComplete.ToString() : "";
                        executeResults.TotalCancel = _first.TotalCancel != null ? _first.TotalCancel.ToString() : "";
                        executeResults.ReturnMessage = string.IsNullOrEmpty(_cur1.FirstOrDefault().ReturnMessage) ? "No data found" : _cur1.FirstOrDefault().ReturnMessage;
                    }

                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ConvertToJson(result), log, "Success", "", "");

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
                _logger.Info("Error call service fbbadm.pkg_fbb_query_order_query_order_by_asc_empid" + ex.Message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, query, log, "Failed", ex.Message, "");
                executeResults.ReturnCode = "-1";
                executeResults.ReturnMessage = "Error";
            }

            return executeResults;
        }

        private string ConvertToJson<T>(T value)
        {
            if (value == null)
            {
                return "";
            }

            try
            {
                return JsonConvert.SerializeObject(value);
            }
            catch (Exception)
            {
                return "";
            }
        }

        #endregion New Version For Shareplex to HVR PostgreSQL R24.04
    }
}
