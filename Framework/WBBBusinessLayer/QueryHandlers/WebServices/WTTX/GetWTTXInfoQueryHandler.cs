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
    public class GetWTTXInfoQueryHandler : IQueryHandler<GetWTTXInfoQuery, WTTXInfoModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public GetWTTXInfoQueryHandler(
            ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow
            )
        {
            _logger = logger;
            _intfLog = intfLog;
            _uow = uow;
        }

        public WTTXInfoModel Handle(GetWTTXInfoQuery query)
        {
            WTTXInfoModel result = new WTTXInfoModel();
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
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.transaction_id, "WTTxGridInfo", "WTTxInventory", "", "FBB", "WBB");

                wttxxGridInfo res = new wttxxGridInfo();
                using (WTTxInventoryService service = new WTTxInventoryService())
                {
                    res = service.queryWttxGridInfo(query.grid_id, query.ref_id);
                    if (res.respMsg.responseCode == "FBRP-0000")
                    {
                        if (!String.IsNullOrEmpty(res.onService))
                        {
                            result = new WTTXInfoModel()
                            {
                                GRIDID = res.gridId,
                                LATITUDE = res.latitude,
                                LONGITUDE = res.longitude,
                                REGION = res.region,
                                PROVINCEENG = res.provinceEng,
                                PROVINCETHA = res.provinceTha,
                                SCALEXKM = res.scaleXkm,
                                SCALEYKM = res.scaleYkm,
                                ONSERVICE = res.onService,
                                MAXBANDWITH = res.maxBandwith,
                                MAXCUSTOMER = res.maxCustomer,
                                NUMBEROFCUSTOMER = res.numberOfCustomer,
                                UTILIZATION = res.utilization,
                                LASTUPDATETIME = res.lastUpdateTime
                            };

                            result.RESULT_CODE = "0";
                            result.RESULT_MESSAGE = "";
                        }
                    }
                    else
                    {
                        result.RESULT_CODE = "1";
                        result.RESULT_MESSAGE = res.respMsg.responseMessage;
                    }
                }

                //result = new WTTXInfoModel()
                //{
                //    GRIDID = "123456789",
                //    LATITUDE = "16.75419025",
                //    LONGITUDE = "98.563648",
                //    REGION = "NR",
                //    PROVINCEENG = "TAK",
                //    PROVINCETHA = "ตาก",
                //    SCALEXKM = "5",
                //    SCALEYKM = "5",
                //    ONSERVICE = "YES",
                //    MAXBANDWITH = "20",
                //    MAXCUSTOMER = 15,
                //    NUMBEROFCUSTOMER = 16,
                //    UTILIZATION = "0",
                //    LASTUPDATETIME = ""
                //};

                result.TRANSACTION_ID = query.transaction_id;
                result.RESULT_CODE = "0";
                result.RESULT_MESSAGE = "";

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, res, log, "Success", "", "WBB");
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                if (log != null)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Error", ex.Message, "WBB");
                }

                result.RESULT_CODE = "-1";
                result.RESULT_MESSAGE = ex.GetErrorMessage();
            }

            return result;
        }
    }
}
