using System;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.ExWebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.ExWebServices
{
    public class GetAisMobileQueryHandler : IQueryHandler<GetAisMobileServiceQuery, AisMobileModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_VSMP_LOG> _vsmpLog;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public GetAisMobileQueryHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_VSMP_LOG> vsmpLog, IWBBUnitOfWork uow)
        {
            _logger = logger;
            _vsmpLog = vsmpLog;
            _intfLog = intfLog;
            _uow = uow;
        }

        public AisMobileModel Handle(GetAisMobileServiceQuery query)
        {
            InterfaceLogCommand log = null;

            AisMobileModel aisMobileModel = new AisMobileModel();

            try
            {
                if (query.User == "TriplePlay")
                {
                    //log = VsmpServiceConseHelper.StartInterfaceVsmpLog(_uow, _intfLog,
                    //   query, query.TransactionId, "CheckPostORPrePaid", "GetAisMobileServiceQuery");
                    log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TransactionId, "CheckPostORPrePaid", "GetAisMobileServiceQuery", null, "FBB", "");
                }
                else
                {
                    //log = VsmpServiceConseHelper.StartInterfaceVsmpLog(_uow, _intfLog,
                    //    query, query.TransactionId, "InquiryVASSubscriber", "GetAisMobileServiceQuery");
                    log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TransactionId, "InquiryVASSubscriber", "GetAisMobileServiceQuery", null, "FBB", "");
                }

                query.Msisdn = "66" + query.Msisdn.Substring(1);

                //_logger.Info("Param is --> Msisdn : " + query.Msisdn +
                //    ", Opt1 : " + query.Opt1 + ", Opt2 : " + query.Opt2 +
                //    ", OrderDesc : " + query.OrderDesc + ", OrderRef : " + query.OrderRef +
                //    ", User : " + query.User + ", UserName : " + query.UserName);

                //_logger.Info("Before insert vsmmp_log");

                #region Insert FBB_VSMP_LOG before call service.
                var vsmpDataBefore = new FBB_VSMP_LOG()
                {
                    CREATED_BY = query.User,
                    CREATED_DATE = DateTime.Now,
                    MOBILE_NO = query.Msisdn,
                    ORDER_DESC = query.OrderDesc,
                    ORDER_REF = query.OrderRef,
                    USER_NAME = query.UserName,
                };

                _vsmpLog.Create(vsmpDataBefore);
                _uow.Persist();
                #endregion

                //_logger.Info("Insert vsmmp_log done");
                //_logger.Info("Call SubscriberwebService");

                #region Call SubscriberwebService.

                using (var service = new SubscriberWebService.SubscriberStateWebService())
                {
                    var resultVasSub = service.InquiryVASSubscriber(query.UserName,
                        query.OrderRef, query.OrderDesc, query.Msisdn, query.Opt1, query.Opt2);

                    if (resultVasSub != null)
                    {
                        aisMobileModel.IsSuccess = resultVasSub.OperationStatus.IsSuccess.ToSafeString();
                        aisMobileModel.Code = resultVasSub.OperationStatus.Code;
                        aisMobileModel.Description = resultVasSub.OperationStatus.Description;
                        aisMobileModel.TransactionID = resultVasSub.OperationStatus.TransactionID;
                        aisMobileModel.SpName = resultVasSub.Subscriber.spName;
                        aisMobileModel.Chm = resultVasSub.Subscriber.chm;
                        aisMobileModel.State = resultVasSub.Subscriber.state;

                        //_logger.Info("Parameter after call service is --> IsSuccess : " + resultVasSub.OperationStatus.IsSuccess.ToSafeString() +
                        //    ", Code : " + resultVasSub.OperationStatus.Code + ", Description : " + resultVasSub.OperationStatus.Description +
                        //    ", TransactionID : " + resultVasSub.OperationStatus.TransactionID + ", spName : " + resultVasSub.Subscriber.spName +
                        //    ", chm : " + resultVasSub.Subscriber.chm + ", state : " + resultVasSub.Subscriber.state);
                    }
                }

                #endregion

                //_logger.Info("Call SubscriberwebService done");
                //_logger.Info("After insert vsmmp_log");

                #region Update FBB_VSMP_LOG after call service.

                //var vsmpDataAfter = from vl in _vsmpLog.Get()
                //                    where vl.VSMP_ID == vsmpDataBefore
                //                    select vl;

                var vsmpDataAfter = _vsmpLog.GetByKey(vsmpDataBefore.VSMP_ID);

                vsmpDataAfter.VSMP_ISSUCCESS = aisMobileModel.IsSuccess.ToSafeString().ToUpper();
                vsmpDataAfter.VSMP_RETURN_CODE = aisMobileModel.Code.ToSafeString();
                vsmpDataAfter.VSMP_RETURN_DESC = aisMobileModel.Description.ToSafeString();
                vsmpDataAfter.VSMP_TRANSID = aisMobileModel.TransactionID.ToSafeString();
                vsmpDataAfter.SPNAME = aisMobileModel.SpName.ToSafeString();
                vsmpDataAfter.CHM = aisMobileModel.Chm.ToSafeString();
                vsmpDataAfter.STATE = aisMobileModel.State.ToSafeString();
                vsmpDataAfter.UPDATED_DATE = DateTime.Now;
                vsmpDataAfter.UPDATED_BY = query.User;
                _vsmpLog.Update(vsmpDataAfter);
                _uow.Persist();

                #endregion

                //_logger.Info("Insert vsmmp_log done");
                //VsmpServiceConseHelper.EndInterfaceVsmpLog(_uow, _intfLog, aisMobileModel, log,
                //        "Success", "");
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, aisMobileModel, log, "Success", "", "");

                return aisMobileModel;
            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, aisMobileModel, log, "Failed", ex.Message, "");

                _logger.Error("Error when call GetAisMobileQueryHandler : " + ex.InnerException.ToSafeString());
                return aisMobileModel;
            }
        }
    }
}
