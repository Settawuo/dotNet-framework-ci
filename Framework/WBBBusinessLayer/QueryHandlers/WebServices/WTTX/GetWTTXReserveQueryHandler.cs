using System;
using System.Linq;
using System.Xml;
using WBBBusinessLayer.WTTxInventory;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices.WTTX;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices.WTTX
{
    public class GetWTTXReserveQueryHandler : IQueryHandler<GetWTTXReserveQuery, WTTXReserveModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public GetWTTXReserveQueryHandler(
            ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow
            )
        {
            _logger = logger;
            _intfLog = intfLog;
            _uow = uow;
        }

        public WTTXReserveModel Handle(GetWTTXReserveQuery query)
        {
            WTTXReserveModel result = new WTTXReserveModel();
            InterfaceLogCommand log = null;

            try
            {
                string refID = "";
                var lastData = _intfLog.Get(o => o.SERVICE_NAME == "WTTxInventory").OrderByDescending(o => o.INTERFACE_ID).FirstOrDefault();
                if (lastData == null)
                    refID = String.Format("TID-{0:000000000}", 1);
                else
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(lastData.IN_XML_PARAM);
                    var nodes = xmlDoc.GetElementsByTagName("ref_id");
                    string lastRefID = nodes.Count == 0 ? lastRefID = String.Format("TID-{0:000000000}", 0) : nodes[0].InnerText;

                    int nextNum = int.Parse(lastRefID.Split('-')[1]) + 1;
                    if (nextNum > 999999999)
                        nextNum = 1;

                    refID = String.Format("TID-{0:000000000}", nextNum);
                }
                query.ref_id = refID;
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.transaction_id, "WTTxGridReserve", "WTTxInventory", "", "FBB", "WEB");

                responseMsg res = new responseMsg();
                using (WTTxInventoryService service = new WTTxInventoryService())
                {
                    res = service.reserveWttxGridInfo(query.gridId, query.reservedExpTime, query.ref_id);
                    if (res.responseCode == "FBRP-0000")
                    {
                        result = new WTTXReserveModel()
                        {
                            reservedId = res.reservedId,
                            responseCode = res.responseCode,
                            responseMessage = res.responseMessage
                        };
                        result.RESULT_CODE = "0";
                        result.RESULT_MESSAGE = "";
                    }
                    else
                    {
                        result.RESULT_CODE = "1";
                        result.RESULT_MESSAGE = res.responseMessage;
                    }
                }
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, res, log, "Success", "", "FBB");


                ////TODO: Remove when web service can work.
                //result = new WTTXReserveModel()
                //{
                //    reservedId = "8f9bf847-5ce3-43cc-a8f9-5d258cf3016f",
                //    responseCode = "FBRP-0000",
                //    responseMessage = "SUCCESS",
                //};
                ////result.TRANSACTION_ID = transactionID;
                //result.RESULT_CODE = "0";
                //result.RESULT_MESSAGE = "";

                //InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "", "FBB");
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                if (log != null)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Error", ex.Message, "FBB");
                }

                result.RESULT_CODE = "-1";
                result.RESULT_MESSAGE = ex.GetErrorMessage();
            }

            return result;
        }
    }
}
