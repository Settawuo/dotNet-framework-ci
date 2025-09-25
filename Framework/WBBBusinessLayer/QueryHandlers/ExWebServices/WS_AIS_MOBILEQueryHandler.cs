using System;
using System.Linq;
using WBBContract;
using WBBContract.Queries.ExWebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.ExWebServices
{
    public class WS_AIS_MOBILEQueryHandler : IQueryHandler<GetWSAISMOBILESeviceQuery, string>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_VSMP_LOG> _lovService_FBB_VSMP_LOG;
        private readonly IEntityRepository<FBB_COVERAGEAREA_RESULT> _FBB_COVERAGEAREA_RESULT;
        private readonly IEntityRepository<FBB_SFF_CHKPROFILE_LOG> _FBB_SFF_CHKPROFILE_LOG;

        //protected Stopwatch timer;
        private readonly IWBBUnitOfWork _uow;

        public WS_AIS_MOBILEQueryHandler(ILogger logger, IEntityRepository<FBB_VSMP_LOG> lovService_FBB_VSMP_LOG,
            IEntityRepository<FBB_COVERAGEAREA_RESULT> FBB_COVERAGEAREA_RESULT,
            IEntityRepository<FBB_SFF_CHKPROFILE_LOG> FBB_SFF_CHKPROFILE_LOG,
           IWBBUnitOfWork uow)
        {
            _logger = logger;
            _lovService_FBB_VSMP_LOG = lovService_FBB_VSMP_LOG;
            _FBB_COVERAGEAREA_RESULT = FBB_COVERAGEAREA_RESULT;
            _FBB_SFF_CHKPROFILE_LOG = FBB_SFF_CHKPROFILE_LOG;
            _uow = uow;
        }

        public string Handle(GetWSAISMOBILESeviceQuery query)
        {
            _logger.Info("SubscriberStateWebService Start");
            //var tempRegisDate = DateTime.Now.ToString();
            //var regisDate = DateTime.ParseExact(tempRegisDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            string Result_ID = "";
            if (query.LanguageSender == "1")
            {
                if (query.ResultID != "")
                {
                    Result_ID = query.ResultID;
                }
            }
            else
            {
                if (query.SffProfileLogID != "")
                {
                    Result_ID = query.SffProfileLogID;
                }
            }

            _logger.Info("SubscriberStateWebService Result ID: " + Result_ID);

            /// var model = new GetListWsisMobileQuery();
            ///CultureInfo ci = CultureInfo.InvariantCulture;

            string timeNow = DateTime.Now.ToString("HHmmss");

            //var date1 = DateTime.ParseExact(daytotate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            //var spitTimedata = date1.ToString().Split(' ');
            //string subdatedat = spitTimedata[1];
            //string Resubdatedat = subdatedat.Replace(":", "");
            string orderRef = Result_ID.ToSafeString() + timeNow; //+ Resubdatedat;
            string ModbileSub = query.Msisdn.Substring(1, query.Msisdn.Length - 1);
            string ModbileSubpre = query.Msisdn.Substring(0, 1);

            string Modbile = "";
            if (ModbileSubpre == "0")
            {
                Modbile = "66" + ModbileSub;
            }
            else
            {
                Modbile = query.Msisdn;
            }

            #region Tye calll Webservice

            try
            {
                var Insert_Model_FBB_VSMP_LOG = new FBB_VSMP_LOG
                {
                    ORDER_REF = orderRef,
                    MOBILE_NO = Modbile,
                    USER_NAME = "FBB",
                    ORDER_DESC = "query sub",
                    SPNAME = "",
                    CHM = "",
                    VSMP_RETURN_CODE = "",
                    VSMP_RETURN_DESC = "",
                    VSMP_TRANSID = "",
                    VSMP_ISSUCCESS = "",
                    STATE = "",
                    CREATED_BY = query.User,
                    CREATED_DATE = DateTime.Now
                };

                _lovService_FBB_VSMP_LOG.Create(Insert_Model_FBB_VSMP_LOG);
                _uow.Persist();

                _logger.Info("SubscriberStateWebService Persist Passed.");
                using (var service = new SubscriberWebService.SubscriberStateWebService())
                {
                    var a = service.InquiryVASSubscriber(Insert_Model_FBB_VSMP_LOG.USER_NAME, Insert_Model_FBB_VSMP_LOG.ORDER_REF, Insert_Model_FBB_VSMP_LOG.ORDER_DESC, Insert_Model_FBB_VSMP_LOG.MOBILE_NO, "", "");

                    _logger.Info("Call Service SubscriberStateWebService");
                    _logger.Info("Username: " + query.Username + ", OrderRef: " + query.OrderRef + ", OrderDesc: " + query.OrderDesc +
                        "Msisdn" + query.Msisdn);

                    var data = a;
                    if (data != null)
                    {
                        var VSMP_ID = Insert_Model_FBB_VSMP_LOG.VSMP_ID;

                        _logger.Info("SubscriberStateWebService_Insert_OKKKKKK555" + "  VSMP_ID :::" + VSMP_ID);
                        var UpdateOrdRef = (from up in _lovService_FBB_VSMP_LOG.Get()
                                            where up.VSMP_ID == VSMP_ID
                                            select up).FirstOrDefault();

                        _logger.Info("Updated VSMP ID: " + UpdateOrdRef.VSMP_ID + " ,Order Ref: " + UpdateOrdRef.ORDER_REF);
                        _logger.Info(", Log Cass Susess" + " ,VSMP_ISSUCCESS ::" + a.OperationStatus.IsSuccess.ToString().ToUpper() + " ,VSMP_RETURN_CODE ::"
                            + a.OperationStatus.Code + "  ,VSMP_RETURN_DESC ::" + "OK" + "  ,SPNAME ::" + a.Subscriber.spName +
                            " ,STATE ::" + a.Subscriber.state + " , STATE ::" + a.Subscriber.state +
                            " , VSMP_TRANSID ::" + a.OperationStatus.TransactionID + " VSMP_RETURN_DESC" + a.OperationStatus.Description);
                        if (UpdateOrdRef != null)
                        {
                            _logger.Info("SubscriberStateWebServiceOKKKKKK2Update");
                            UpdateOrdRef.VSMP_ISSUCCESS = a.OperationStatus.IsSuccess.ToString().ToUpper();
                            UpdateOrdRef.VSMP_RETURN_CODE = a.OperationStatus.Code;
                            UpdateOrdRef.VSMP_RETURN_DESC = a.OperationStatus.Description;
                            UpdateOrdRef.VSMP_TRANSID = a.OperationStatus.TransactionID;
                            UpdateOrdRef.SPNAME = a.Subscriber.spName;
                            UpdateOrdRef.CHM = a.Subscriber.chm;
                            UpdateOrdRef.STATE = a.Subscriber.state;
                            UpdateOrdRef.UPDATED_BY = query.User;
                            UpdateOrdRef.UPDATED_DATE = DateTime.Now;

                            _lovService_FBB_VSMP_LOG.Update(UpdateOrdRef, _lovService_FBB_VSMP_LOG.GetByKey(VSMP_ID));
                            _uow.Persist();

                            _logger.Info("SubscriberStateWebService_Updata_OKKKKKK2");
                        }

                        _logger.Info("SubscriberStateWebService_Updata_OKKKKKK2::" + UpdateOrdRef.VSMP_ISSUCCESS + " ,,UpdateOrdRef.STATE ::" + UpdateOrdRef.STATE + ",  UpdateOrdRef.STATE ::" + UpdateOrdRef.CHM);

                        if (UpdateOrdRef.VSMP_ISSUCCESS == "TRUE" && UpdateOrdRef.STATE == "1" && UpdateOrdRef.CHM == "0")
                        {
                            _logger.Info("model.Check_onloave  =====" + "(TRUE)");
                            return "TRUE";
                        }
                        else
                        {
                            _logger.Info("model.Check_onloave  =====" + "( FLASE)");
                            return "FALSE";
                        }

                        //model.out_IsSuccess = UpdateOrdRef.VSMP_ISSUCCESS;
                        //model.ou_chm = UpdateOrdRef.CHM;
                        //model.ou_state = UpdateOrdRef.STATE;
                        //query.out_IsSuccess = a.OperationStatus.IsSuccess.ToString().ToUpper();
                    }
                    else
                    {
                        _logger.Info("SubscriberStateWebService return null.");
                        _logger.Info("Username: " + query.Username + ", OrderRef: " + query.OrderRef + ", OrderDesc: " + query.OrderDesc +
                          ", Msisdn: " + query.Msisdn);
                        return "";
                    }

                    //return null;
                }
            }
            catch (Exception ex)
            {
                _logger.Info("Exeption message: " + ex.Message + " ,Inner Exeption: " + ex.InnerException);
                return null;
            }

            #endregion Tye calll Webservice
        }
    }
}