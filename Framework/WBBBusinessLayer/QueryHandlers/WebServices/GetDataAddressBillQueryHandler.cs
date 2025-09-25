using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{


    public class GetDataAddressBillQueryHandler : IQueryHandler<GetDataAddressBillQuery, GetDataAddressBillModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<GetDataAddressBillModel> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public GetDataAddressBillQueryHandler(ILogger logger, IEntityRepository<GetDataAddressBillModel> objService, IEntityRepository<FBB_INTERFACE_LOG> intfLog, IWBBUnitOfWork uow)
        {
            _logger = logger;
            _objService = objService;
            _intfLog = intfLog;
            _uow = uow;
        }

        public GetDataAddressBillModel Handle(GetDataAddressBillQuery query)
        {
            InterfaceLogCommand log = null;
            GetDataAddressBillModel result = new GetDataAddressBillModel();

            try
            {

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.transaction_Id, "GetDataAddressBillQuery", "GetDataAddressBillQueryHandler", "", "FBB", "");

                var p_houseNo = new OracleParameter();
                p_houseNo.ParameterName = "p_houseNo";
                p_houseNo.OracleDbType = OracleDbType.Varchar2;
                p_houseNo.Direction = ParameterDirection.Input;
                p_houseNo.Value = query.p_houseNo;

                var p_Moo = new OracleParameter();
                p_Moo.ParameterName = "p_Moo";
                p_Moo.OracleDbType = OracleDbType.Varchar2;
                p_Moo.Direction = ParameterDirection.Input;
                p_Moo.Value = query.p_Moo;

                var p_Mooban = new OracleParameter();
                p_Mooban.ParameterName = "p_Mooban";
                p_Mooban.OracleDbType = OracleDbType.Varchar2;
                p_Mooban.Direction = ParameterDirection.Input;
                p_Mooban.Value = query.p_Mooban;

                var p_Room = new OracleParameter();
                p_Room.ParameterName = "p_Room";
                p_Room.OracleDbType = OracleDbType.Varchar2;
                p_Room.Direction = ParameterDirection.Input;
                p_Room.Value = query.p_Room;

                var p_Floor = new OracleParameter();
                p_Floor.ParameterName = "p_Floor";
                p_Floor.OracleDbType = OracleDbType.Varchar2;
                p_Floor.Direction = ParameterDirection.Input;
                p_Floor.Value = query.p_Floor;

                var p_buildingName = new OracleParameter();
                p_buildingName.ParameterName = "p_buildingName";
                p_buildingName.OracleDbType = OracleDbType.Varchar2;
                p_buildingName.Direction = ParameterDirection.Input;
                p_buildingName.Value = query.p_buildingName;

                var p_Soi = new OracleParameter();
                p_Soi.ParameterName = "p_Soi";
                p_Soi.OracleDbType = OracleDbType.Varchar2;
                p_Soi.Direction = ParameterDirection.Input;
                p_Soi.Value = query.p_Soi;

                var p_streetName = new OracleParameter();
                p_streetName.ParameterName = "p_streetName";
                p_streetName.OracleDbType = OracleDbType.Varchar2;
                p_streetName.Direction = ParameterDirection.Input;
                p_streetName.Value = query.p_streetName;

                var p_Tumbol = new OracleParameter();
                p_Tumbol.ParameterName = "p_Tumbol";
                p_Tumbol.OracleDbType = OracleDbType.Varchar2;
                p_Tumbol.Direction = ParameterDirection.Input;
                p_Tumbol.Value = query.p_Tumbol;

                var p_Amphur = new OracleParameter();
                p_Amphur.ParameterName = "p_Amphur";
                p_Amphur.OracleDbType = OracleDbType.Varchar2;
                p_Amphur.Direction = ParameterDirection.Input;
                p_Amphur.Value = query.p_Amphur;

                var p_provinceName = new OracleParameter();
                p_provinceName.ParameterName = "p_provinceName";
                p_provinceName.OracleDbType = OracleDbType.Varchar2;
                p_provinceName.Direction = ParameterDirection.Input;
                p_provinceName.Value = query.p_provinceName;

                var p_zipCode = new OracleParameter();
                p_zipCode.ParameterName = "p_zipCode";
                p_zipCode.OracleDbType = OracleDbType.Varchar2;
                p_zipCode.Direction = ParameterDirection.Input;
                p_zipCode.Value = query.p_zipCode;


                var return_code = new OracleParameter();
                return_code.ParameterName = "RETURN_CODE";
                return_code.OracleDbType = OracleDbType.Int32;
                return_code.Direction = ParameterDirection.Output;


                var return_message = new OracleParameter();
                return_message.ParameterName = "RETURN_MESSAGE";
                return_message.OracleDbType = OracleDbType.Varchar2;
                return_message.Direction = ParameterDirection.Output;

                var return_address_curror = new OracleParameter();
                return_address_curror.ParameterName = "RETURN_ADDRESS_CURROR";
                return_address_curror.OracleDbType = OracleDbType.RefCursor;
                return_address_curror.Direction = ParameterDirection.Output;

                var executeResult = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBBOR051.QUERY_DATA_ADDRESS_BILL",
                        new object[]
                        {
                            p_houseNo,
                            p_Moo,
                            p_Mooban,
                            p_Room,
                            p_Floor,
                            p_buildingName,
                            p_Soi,
                            p_streetName,
                            p_Tumbol,
                            p_Amphur,
                            p_provinceName,
                            p_zipCode,

                            //out
                            return_code,
                            return_message,
                            return_address_curror

                        });

                DataTable data1 = (DataTable)executeResult[2];
                result.address_curror = data1.DataTableToList<BillAddressList>();

                result.return_code = executeResult[0] != null ? executeResult[0].ToSafeString() : "-1";
                result.return_message = executeResult[1] != null ? executeResult[1].ToSafeString() : "error";

                if (result.return_code != "-1")
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "", "");
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Failed", result.return_message, "");
                }

            }
            catch (Exception ex)
            {

                if (null != log)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
                }

                result.return_code = "-1";
                result.return_message = "error GetDataAddressBillQueryHandler " + ex.Message;


            }

            return result;
        }
    }
}
