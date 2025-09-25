using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using WBBContract;
using WBBContract.Queries.ExWebServices;
using WBBData.Repository;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.ExWebServices.LastmilePayment
{
    public class LastmilePaymentQueryHandler : IQueryHandler<LastmilePaymentQuery, LastmilePaymentResponse>
    {

        private readonly ILogger _logger;
        //private readonly IEntityRepository<FBB_INTERFACE_LOG_PAYG> _intfLog;
        //private readonly IEntityRepository<FBB_FIXED_ASSET_HISTORY_LOG> _hisLog;
        //private readonly IWBBUnitOfWork _uow;
        //private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;
        //private readonly IEntityRepository<FBSS_FIXED_ASSET_CONFIG> _fixAssConfig;
        private readonly IEntityRepository<string> _objService;
        //private readonly IEntityRepository<InsertFoaInfoParmModel> _objFoaService;
        //private readonly IEntityRepository<FBBPAYG_WFM_SUBCONTRACTOR> _subContractor;

        public LastmilePaymentQueryHandler(
            ILogger logger
            //,IEntityRepository<FBB_INTERFACE_LOG_PAYG> intfLog,
            //IEntityRepository<FBB_FIXED_ASSET_HISTORY_LOG> hisLog,
            //IWBBUnitOfWork uow,
            //IEntityRepository<FBB_CFG_LOV> cfgLov,
            //IEntityRepository<FBSS_FIXED_ASSET_CONFIG> fixAssConfig,
            , IEntityRepository<string> objService
            //,IEntityRepository<InsertFoaInfoParmModel> objFoaService,
            //IEntityRepository<FBBPAYG_WFM_SUBCONTRACTOR> subContractor
            )
        {
            _logger = logger;
            //_intfLog = intfLog;
            //_hisLog = hisLog;  
            //_uow = uow;
            //_cfgLov = cfgLov;
            //_fixAssConfig = fixAssConfig;
            _objService = objService;
            //_objFoaService = objFoaService;
            //_subContractor = subContractor;
        }

        public LastmilePaymentResponse Handle(LastmilePaymentQuery model)
        {
            //InterfaceLogPayGCommand log = null;
            //HistoryLogCommand hLog = null;
            //bool flagSap = true;
            //  bool flagSff = true;
            LastmilePaymentResponse LastmilePaymentResponseResult = new LastmilePaymentResponse();


            if (model.ACTION_DATE != null && model.ACTION_DATE != "")
            {
                try
                {
                    DateTime dtFOA_Submit_date = DateTime.ParseExact(model.ACTION_DATE, "yyyy-MM-ddTHH:mm:ss.fffzzz", CultureInfo.InvariantCulture);
                    //FOA_Submit_date = dtFOA_Submit_date.ToString("dd/MM/yyyy HHmmss");
                    //  DateTime dtFOA_Submit_date = DateTime.ParseExact(model.FOA_Submit_date, "dd/MM/yyyy hh:mm:ss", CultureInfo.InvariantCulture);
                    model.ACTION_DATE = dtFOA_Submit_date.ToString("dd/MM/yyyy HHmmss");
                }
                catch
                {
                    DateTime dtFOA_Submit_date = DateTime.ParseExact(model.ACTION_DATE, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    //FOA_Submit_date = dtFOA_Submit_date.ToString("dd/MM/yyyy HHmmss");
                    model.ACTION_DATE = dtFOA_Submit_date.ToString("dd/MM/yyyy HHmmss");
                }
            }

            //Parameter Input
            var p_TRANS_ID = new OracleParameter();
            p_TRANS_ID.ParameterName = "p_TRANS_ID";
            p_TRANS_ID.Size = 2000;
            p_TRANS_ID.OracleDbType = OracleDbType.Varchar2;
            p_TRANS_ID.Direction = ParameterDirection.Input;
            p_TRANS_ID.Value = model.TRANSACTION_ID;

            var p_ORDER_NO = new OracleParameter();
            p_ORDER_NO.ParameterName = "p_ORDER_NO";
            p_ORDER_NO.Size = 2000;
            p_ORDER_NO.OracleDbType = OracleDbType.Varchar2;
            p_ORDER_NO.Direction = ParameterDirection.Input;
            p_ORDER_NO.Value = model.ORDER_NO;

            var P_ORDER_TYPE = new OracleParameter();
            P_ORDER_TYPE.ParameterName = "P_ORDER_TYPE";
            P_ORDER_TYPE.Size = 2000;
            P_ORDER_TYPE.OracleDbType = OracleDbType.Varchar2;
            P_ORDER_TYPE.Direction = ParameterDirection.Input;
            P_ORDER_TYPE.Value = model.ORDER_TYPE;


            var P_REUSED_FLAG = new OracleParameter();
            P_REUSED_FLAG.ParameterName = "P_REUSED_FLAG";
            P_REUSED_FLAG.Size = 2000;
            P_REUSED_FLAG.OracleDbType = OracleDbType.Varchar2;
            P_REUSED_FLAG.Direction = ParameterDirection.Input;
            P_REUSED_FLAG.Value = model.REUSED_FLAG;


            var p_ACCESS_NO = new OracleParameter();
            p_ACCESS_NO.ParameterName = "p_ACCESS_NO";
            p_ACCESS_NO.Size = 2000;
            p_ACCESS_NO.OracleDbType = OracleDbType.Varchar2;
            p_ACCESS_NO.Direction = ParameterDirection.Input;
            p_ACCESS_NO.Value = model.ACCESS_NO;

            var p_DIST_REAL = new OracleParameter();
            p_DIST_REAL.ParameterName = "p_DIST_REAL";
            p_DIST_REAL.Size = 2000;
            p_DIST_REAL.OracleDbType = OracleDbType.Varchar2;
            p_DIST_REAL.Direction = ParameterDirection.Input;
            p_DIST_REAL.Value = model.REAL_DISTANCE;


            var p_DIST_GOOGLE = new OracleParameter();
            p_DIST_GOOGLE.ParameterName = "p_DIST_GOOGLE";
            p_DIST_GOOGLE.Size = 2000;
            p_DIST_GOOGLE.OracleDbType = OracleDbType.Varchar2;
            p_DIST_GOOGLE.Direction = ParameterDirection.Input;
            p_DIST_GOOGLE.Value = model.MAP_DISTANCE;

            var p_DIST_DISPLACE = new OracleParameter();
            p_DIST_DISPLACE.ParameterName = "p_DIST_DISPLACE";
            p_DIST_DISPLACE.Size = 2000;
            p_DIST_DISPLACE.OracleDbType = OracleDbType.Varchar2;
            p_DIST_DISPLACE.Direction = ParameterDirection.Input;
            p_DIST_DISPLACE.Value = model.DISP_DISTANCE;

            var P_ESRI_DISTANCE = new OracleParameter();
            P_ESRI_DISTANCE.ParameterName = "P_ESRI_DISTANCE";
            P_ESRI_DISTANCE.Size = 2000;
            P_ESRI_DISTANCE.OracleDbType = OracleDbType.Varchar2;
            P_ESRI_DISTANCE.Direction = ParameterDirection.Input;
            P_ESRI_DISTANCE.Value = model.ESRI_DISTANCE;

            var P_BUILDING_TYPE = new OracleParameter();
            P_BUILDING_TYPE.ParameterName = "P_BUILDING_TYPE";
            P_BUILDING_TYPE.Size = 2000;
            P_BUILDING_TYPE.OracleDbType = OracleDbType.Varchar2;
            P_BUILDING_TYPE.Direction = ParameterDirection.Input;
            P_BUILDING_TYPE.Value = model.BUILDING_TYPE;

            var P_USER_ID = new OracleParameter();
            P_USER_ID.ParameterName = "P_USER_ID";
            P_USER_ID.Size = 2000;
            P_USER_ID.OracleDbType = OracleDbType.Varchar2;
            P_USER_ID.Direction = ParameterDirection.Input;
            P_USER_ID.Value = model.USER_ID;

            var p_POST_SAP_STATUS = new OracleParameter();
            p_POST_SAP_STATUS.ParameterName = "p_POST_SAP_STATUS";
            p_POST_SAP_STATUS.Size = 2000;
            p_POST_SAP_STATUS.OracleDbType = OracleDbType.Varchar2;
            p_POST_SAP_STATUS.Direction = ParameterDirection.Input;
            p_POST_SAP_STATUS.Value = model.REUSED_FLAG;

            var p_CREATE_DT = new OracleParameter();
            p_CREATE_DT.ParameterName = "p_create_dt";
            p_CREATE_DT.Size = 2000;
            p_CREATE_DT.OracleDbType = OracleDbType.Varchar2;
            p_CREATE_DT.Direction = ParameterDirection.Input;
            p_CREATE_DT.Value = model.ACTION_DATE;

            var p_CREATE_BY = new OracleParameter();
            p_CREATE_BY.ParameterName = "p_CREATE_BY";
            p_CREATE_BY.Size = 2000;
            p_CREATE_BY.OracleDbType = OracleDbType.Varchar2;
            p_CREATE_BY.Direction = ParameterDirection.Input;
            p_CREATE_BY.Value = model.USER_ID;

            var p_UPDATE_DT = new OracleParameter();
            p_UPDATE_DT.ParameterName = "p_UPDATE_DT";
            p_UPDATE_DT.Size = 2000;
            p_UPDATE_DT.OracleDbType = OracleDbType.Varchar2;
            p_UPDATE_DT.Direction = ParameterDirection.Input;
            p_UPDATE_DT.Value = model.LAST_UPDATE_DATE;

            var p_UPDATE_BY = new OracleParameter();
            p_UPDATE_BY.ParameterName = "p_UPDATE_BY";
            p_UPDATE_BY.Size = 2000;
            p_UPDATE_BY.OracleDbType = OracleDbType.Varchar2;
            p_UPDATE_BY.Direction = ParameterDirection.Input;
            p_UPDATE_BY.Value = model.LAST_UPDATE_BY;

            //   Output
            var ret_code = new OracleParameter();
            ret_code.ParameterName = "ret_code";
            ret_code.OracleDbType = OracleDbType.Int32;
            ret_code.Direction = ParameterDirection.Output;

            var ret_msg = new OracleParameter();
            ret_msg.ParameterName = "ret_msg";
            ret_msg.OracleDbType = OracleDbType.Varchar2;
            ret_msg.Direction = ParameterDirection.Output;

            var outp = new List<object>();
            var paramOut = outp.ToArray();
            LastmilePaymentResponse LastmilePaymentResponseResponseResult = new LastmilePaymentResponse();
            ///============================
            try
            {


                var result = _objService.ExecuteStoredProc("WBB.PKG_FIXED_ASSET_LASTMILE.p_upsert_distance_info",
                  out paramOut, new
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
                      //Parameter Output 
                      ret_code,
                      ret_msg
                  });

                LastmilePaymentResponseResponseResult.RESULT_DESCRIPTION = ret_msg.Value.ToString();
                LastmilePaymentResponseResponseResult.RESULT_CODE = ret_code.Value.ToString() != "null" ? ret_code.Value.ToString() : "0";

                if (LastmilePaymentResponseResponseResult.RESULT_CODE != null)
                {

                    if (LastmilePaymentResponseResponseResult.RESULT_CODE.Equals("0"))
                    {
                        // LastmilePaymentResponseResponseResult.RESULT_CODE = "0";
                        LastmilePaymentResponseResponseResult.RESULT_DESCRIPTION = "Success";

                        // return LastmilePaymentResponseResponseResult;
                    }
                    else
                    {
                        LastmilePaymentResponseResponseResult.RESULT_CODE = "-1";
                        LastmilePaymentResponseResponseResult.RESULT_DESCRIPTION = "Not Success";
                    }
                }// END IF CHECK RESULE
            }
            catch (Exception ex)
            {
                _logger.Info("" + ex.Message.ToString());
                LastmilePaymentResponseResponseResult.RESULT_CODE = "-1";
                LastmilePaymentResponseResponseResult.RESULT_DESCRIPTION = ex.Message.ToString();
            }
            return LastmilePaymentResponseResponseResult;
        }
    }
}
