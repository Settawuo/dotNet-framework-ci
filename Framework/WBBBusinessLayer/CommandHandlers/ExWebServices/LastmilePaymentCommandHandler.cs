using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Globalization;
using WBBContract;
using WBBContract.Commands.ExWebServices;
using WBBData.Repository;
using WBBEntity.Extensions;



namespace WBBBusinessLayer.CommandHandlers.ExWebServices
{
    public class LastmilePaymentCommandHandler : ICommandHandler<LastmilePaymentCommand>
    {
        private readonly ILogger _logger;
        // private readonly IEntityRepository<LastmilePaymentModel> _LastmilePaymentModel; 
        //  private readonly ICommandHandler<LastmilePaymentCommand> _LastmilePaymentCommandHandler;
        private readonly IEntityRepository<string> _objService;
        public LastmilePaymentCommandHandler(
            ILogger logger
            // , IEntityRepository<LastmilePaymentModel> LastmilePaymentModel
            // , ICommandHandler<LastmilePaymentCommand> LastmilePaymentCommandHandler
            , IEntityRepository<string> objService
            )
        {
            _logger = logger;
            // _LastmilePaymentModel = LastmilePaymentModel; 
            // _LastmilePaymentCommandHandler = LastmilePaymentCommandHandler;
            _objService = objService;
        }
        public void Handle(LastmilePaymentCommand command)
        {
            if (command.ACTION_DATE != null && command.ACTION_DATE != "")
            {
                try
                {
                    DateTime dtFOA_Submit_date = DateTime.ParseExact(command.ACTION_DATE, "yyyy-MM-ddTHH:mm:ss.fffzzz", CultureInfo.InvariantCulture);
                    //FOA_Submit_date = dtFOA_Submit_date.ToString("dd/MM/yyyy HHmmss");
                    //  DateTime dtFOA_Submit_date = DateTime.ParseExact(model.FOA_Submit_date, "dd/MM/yyyy hh:mm:ss", CultureInfo.InvariantCulture);
                    command.ACTION_DATE = dtFOA_Submit_date.ToString("dd/MM/yyyy HHmmss");
                }
                catch
                {
                    DateTime dtFOA_Submit_date = DateTime.ParseExact(command.ACTION_DATE, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    //FOA_Submit_date = dtFOA_Submit_date.ToString("dd/MM/yyyy HHmmss");
                    command.ACTION_DATE = dtFOA_Submit_date.ToString("dd/MM/yyyy HHmmss");
                }
            }

            //Parameter Input
            var p_TRANS_ID = new OracleParameter();
            p_TRANS_ID.ParameterName = "p_TRANS_ID";
            p_TRANS_ID.Size = 250;
            p_TRANS_ID.OracleDbType = OracleDbType.Varchar2;
            p_TRANS_ID.Direction = ParameterDirection.Input;
            p_TRANS_ID.Value = command.TRANSACTION_ID;

            var p_ORDER_NO = new OracleParameter();
            p_ORDER_NO.ParameterName = "p_ORDER_NO";
            p_ORDER_NO.Size = 250;
            p_ORDER_NO.OracleDbType = OracleDbType.Varchar2;
            p_ORDER_NO.Direction = ParameterDirection.Input;
            p_ORDER_NO.Value = command.ORDER_NO;

            var P_ORDER_TYPE = new OracleParameter();
            P_ORDER_TYPE.ParameterName = "P_ORDER_TYPE";
            P_ORDER_TYPE.Size = 250;
            P_ORDER_TYPE.OracleDbType = OracleDbType.Varchar2;
            P_ORDER_TYPE.Direction = ParameterDirection.Input;
            P_ORDER_TYPE.Value = command.ORDER_TYPE;


            var P_REUSED_FLAG = new OracleParameter();
            P_REUSED_FLAG.ParameterName = "P_REUSED_FLAG";
            P_REUSED_FLAG.Size = 250;
            P_REUSED_FLAG.OracleDbType = OracleDbType.Varchar2;
            P_REUSED_FLAG.Direction = ParameterDirection.Input;
            P_REUSED_FLAG.Value = command.REUSED_FLAG;


            var p_ACCESS_NO = new OracleParameter();
            p_ACCESS_NO.ParameterName = "p_ACCESS_NO";
            p_ACCESS_NO.Size = 250;
            p_ACCESS_NO.OracleDbType = OracleDbType.Varchar2;
            p_ACCESS_NO.Direction = ParameterDirection.Input;
            p_ACCESS_NO.Value = command.ACCESS_NO;

            var p_DIST_REAL = new OracleParameter();
            p_DIST_REAL.ParameterName = "p_DIST_REAL";
            p_DIST_REAL.Size = 250;
            p_DIST_REAL.OracleDbType = OracleDbType.Varchar2;
            p_DIST_REAL.Direction = ParameterDirection.Input;
            p_DIST_REAL.Value = command.REAL_DISTANCE;


            var p_DIST_GOOGLE = new OracleParameter();
            p_DIST_GOOGLE.ParameterName = "p_DIST_GOOGLE";
            p_DIST_GOOGLE.Size = 250;
            p_DIST_GOOGLE.OracleDbType = OracleDbType.Varchar2;
            p_DIST_GOOGLE.Direction = ParameterDirection.Input;
            p_DIST_GOOGLE.Value = command.MAP_DISTANCE;

            var p_DIST_DISPLACE = new OracleParameter();
            p_DIST_DISPLACE.ParameterName = "p_DIST_DISPLACE";
            p_DIST_DISPLACE.Size = 250;
            p_DIST_DISPLACE.OracleDbType = OracleDbType.Varchar2;
            p_DIST_DISPLACE.Direction = ParameterDirection.Input;
            p_DIST_DISPLACE.Value = command.DISP_DISTANCE;

            var P_ESRI_DISTANCE = new OracleParameter();
            P_ESRI_DISTANCE.ParameterName = "P_ESRI_DISTANCE";
            P_ESRI_DISTANCE.Size = 250;
            P_ESRI_DISTANCE.OracleDbType = OracleDbType.Varchar2;
            P_ESRI_DISTANCE.Direction = ParameterDirection.Input;
            P_ESRI_DISTANCE.Value = command.ESRI_DISTANCE;

            var P_BUILDING_TYPE = new OracleParameter();
            P_BUILDING_TYPE.ParameterName = "P_BUILDING_TYPE";
            P_BUILDING_TYPE.Size = 250;
            P_BUILDING_TYPE.OracleDbType = OracleDbType.Varchar2;
            P_BUILDING_TYPE.Direction = ParameterDirection.Input;
            P_BUILDING_TYPE.Value = command.BUILDING_TYPE;

            var P_USER_ID = new OracleParameter();
            P_USER_ID.ParameterName = "P_USER_ID";
            P_USER_ID.Size = 250;
            P_USER_ID.OracleDbType = OracleDbType.Varchar2;
            P_USER_ID.Direction = ParameterDirection.Input;
            P_USER_ID.Value = command.USER_ID;

            var p_POST_SAP_STATUS = new OracleParameter();
            p_POST_SAP_STATUS.ParameterName = "p_POST_SAP_STATUS";
            p_POST_SAP_STATUS.Size = 250;
            p_POST_SAP_STATUS.OracleDbType = OracleDbType.Varchar2;
            p_POST_SAP_STATUS.Direction = ParameterDirection.Input;
            p_POST_SAP_STATUS.Value = command.REUSED_FLAG;

            var p_CREATE_DT = new OracleParameter();
            p_CREATE_DT.ParameterName = "p_create_dt";
            p_CREATE_DT.Size = 250;
            p_CREATE_DT.OracleDbType = OracleDbType.Varchar2;
            p_CREATE_DT.Direction = ParameterDirection.Input;
            p_CREATE_DT.Value = command.ACTION_DATE;

            //R19.03
            var p_REQUEST_DISTANCE = new OracleParameter();
            p_REQUEST_DISTANCE.ParameterName = "p_REQUEST_DISTANCE";
            p_REQUEST_DISTANCE.Size = 250;
            p_REQUEST_DISTANCE.OracleDbType = OracleDbType.Varchar2;
            p_REQUEST_DISTANCE.Direction = ParameterDirection.Input;
            p_REQUEST_DISTANCE.Value = command.REQUEST_DISTANCE;

            var p_APPROVE_DISTANCE = new OracleParameter();
            p_APPROVE_DISTANCE.ParameterName = "p_APPROVE_DISTANCE";
            p_APPROVE_DISTANCE.Size = 250;
            p_APPROVE_DISTANCE.OracleDbType = OracleDbType.Varchar2;
            p_APPROVE_DISTANCE.Direction = ParameterDirection.Input;
            p_APPROVE_DISTANCE.Value = command.APPROVE_DISTANCE;

            var p_APPROVE_STAFF = new OracleParameter();
            p_APPROVE_STAFF.ParameterName = "p_APPROVE_STAFF";
            p_APPROVE_STAFF.Size = 250;
            p_APPROVE_STAFF.OracleDbType = OracleDbType.Varchar2;
            p_APPROVE_STAFF.Direction = ParameterDirection.Input;
            p_APPROVE_STAFF.Value = command.APPROVE_STAFF;

            var p_APPROVE_STATUS = new OracleParameter();
            p_APPROVE_STATUS.ParameterName = "p_APPROVE_STATUS";
            p_APPROVE_STATUS.Size = 250;
            p_APPROVE_STATUS.OracleDbType = OracleDbType.Varchar2;
            p_APPROVE_STATUS.Direction = ParameterDirection.Input;
            p_APPROVE_STATUS.Value = command.APPROVE_STATUS;
            
            var p_PRODUCT_OWNER = new OracleParameter();
            p_PRODUCT_OWNER.ParameterName = "p_PRODUCT_OWNER";
            p_PRODUCT_OWNER.Size = 250;
            p_PRODUCT_OWNER.OracleDbType = OracleDbType.Varchar2;
            p_PRODUCT_OWNER.Direction = ParameterDirection.Input;
            p_PRODUCT_OWNER.Value = command.PRODUCT_OWNER;

            //End R19.03

            var p_CREATE_BY = new OracleParameter();
            p_CREATE_BY.ParameterName = "p_CREATE_BY";
            p_CREATE_BY.Size = 250;
            p_CREATE_BY.OracleDbType = OracleDbType.Varchar2;
            p_CREATE_BY.Direction = ParameterDirection.Input;
            p_CREATE_BY.Value = command.USER_ID;

            var p_UPDATE_DT = new OracleParameter();
            p_UPDATE_DT.ParameterName = "p_UPDATE_DT";
            p_UPDATE_DT.Size = 250;
            p_UPDATE_DT.OracleDbType = OracleDbType.Varchar2;
            p_UPDATE_DT.Direction = ParameterDirection.Input;
            p_UPDATE_DT.Value = command.LAST_UPDATE_DATE;

            var p_UPDATE_BY = new OracleParameter();
            p_UPDATE_BY.ParameterName = "p_UPDATE_BY";
            p_UPDATE_BY.Size = 250;
            p_UPDATE_BY.OracleDbType = OracleDbType.Varchar2;
            p_UPDATE_BY.Direction = ParameterDirection.Input;
            p_UPDATE_BY.Value = command.LAST_UPDATE_BY;

            //   Output
            var ret_code = new OracleParameter();
            ret_code.ParameterName = "ret_code";
            ret_code.OracleDbType = OracleDbType.Int32;
            ret_code.Direction = ParameterDirection.Output;

            var ret_msg = new OracleParameter();
            ret_msg.ParameterName = "ret_msg";
            ret_msg.Size = 250;
            ret_msg.OracleDbType = OracleDbType.Varchar2;
            ret_msg.Direction = ParameterDirection.Output;
            ///============================
            try
            {


                var result = _objService.ExecuteStoredProc("WBB.PKG_FIXED_ASSET_LASTMILE.p_upsert_distance_info",
                 new
                 {
                     //Parameter Input
                     p_TRANS_ID,
                     p_ORDER_NO,
                     P_ORDER_TYPE,
                     P_REUSED_FLAG,
                     p_ACCESS_NO,

                     p_DIST_REAL,
                     p_DIST_GOOGLE,
                     p_DIST_DISPLACE,

                     P_ESRI_DISTANCE,
                     P_BUILDING_TYPE,
                     P_USER_ID,
                     p_POST_SAP_STATUS,

                     p_CREATE_DT,
                     p_CREATE_BY,
                     p_UPDATE_DT,
                     p_UPDATE_BY,

                     //R19.03
                     p_REQUEST_DISTANCE,
                     p_APPROVE_DISTANCE,
                     p_APPROVE_STAFF,
                     p_APPROVE_STATUS,
                     //End R19.03
                     p_PRODUCT_OWNER,

                     //Parameter Output 
                     ret_code,
                     ret_msg
                 });
                command.RESULT_DESCRIPTION = ret_msg.Value.ToSafeString() != "null" ? ret_msg.Value.ToSafeString() : "Success";
                command.RESULT_CODE = ret_code.Value.ToSafeString() != "null" ? ret_code.Value.ToSafeString() : "0";


            }
            catch (Exception ex)
            {
                _logger.Info("Error : WBB.PKG_FIXED_ASSET_LASTMILE.p_upsert_distance_info :" + ex.Message);
                command.RESULT_CODE = "-1";
                command.RESULT_DESCRIPTION = "Error save Lastmi service " + ex.Message;
            }
        }
    }


}
