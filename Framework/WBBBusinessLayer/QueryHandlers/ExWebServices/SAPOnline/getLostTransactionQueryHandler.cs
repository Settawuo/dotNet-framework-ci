using System;
using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.ExWebServices;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.ExWebServices.SAPOnline
{

    public class getLostTransactionQueryHandler : IQueryHandler<lostTranQuery, List<lostTranQueryResponse>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG_PAYG> _PAYG;
        private readonly IEntityRepository<FBSS_FIXED_OM010_RPT> _OM;
        private readonly IEntityRepository<FBSS_FOA_SUBMIT_ORDER> _SORD;
        private readonly IEntityRepository<lostTranQueryResponse> _objService;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;
        public getLostTransactionQueryHandler(
             IEntityRepository<lostTranQueryResponse> objService
               , ILogger logger

               //    IEntityRepository<FBSS_FOA_SUBMIT_ORDER> SUBMITORD,
               , IEntityRepository<FBB_INTERFACE_LOG_PAYG> PAYG
                , IEntityRepository<FBSS_FIXED_OM010_RPT> OM
              , IEntityRepository<FBSS_FOA_SUBMIT_ORDER> SORD
             , IEntityRepository<FBB_CFG_LOV> cfgLov
              )
        {
            _logger = logger;

            // _SUBMITORD = SUBMITORD;
            _PAYG = PAYG;
            _OM = OM;
            _SORD = SORD;
            _cfgLov = cfgLov;

        }
        public List<getOM> getByACCESSNO(string ACCESSNO)
        {

            _logger.Info("GetLostTransaction ByACCESSNO");
            List<getOM> OM = new List<getOM>();
            var dd = (from o in _OM.Get()
                      join s in _SORD.Get() on o.ORD_NO equals s.ORDER_NO_SFF into d2
                      from f in d2.DefaultIfEmpty()


                      where o.ACC_NBR.Equals(ACCESSNO)
                      // where  o.SFF_ACTIVE_DATE > dategetfile && o.SFF_ACTIVE_DATE <= DateTime.Now 
                      select new getOM
                      {
                          OMACCNO = o.ACC_NBR,
                          OMORDNO = o.ORD_NO,
                          SMACCNO = f.ACCESS_NUMBER,
                          SMORDNO = f.ORDER_NO,

                      }).ToList();
            return dd;
        }
        public List<getOM> getByDate(string configDate)
        {
            _logger.Debug(String.Format("GetLostTransaction ByDate"));
            _logger.Info("GetLostTransaction ByDate");
            DateTime dategetfile = DateTime.Now.AddDays(-int.Parse(configDate)).Date;

            List<getOM> OM = new List<getOM>();
            var dd = (from o in _OM.Get()
                      join s in _SORD.Get() on o.ORD_NO equals s.ORDER_NO_SFF into d2
                      from f in d2.DefaultIfEmpty()


                          //  where o.ACC_NBR.Equals(ACCESSNO)
                      where o.SFF_ACTIVE_DATE > dategetfile && o.SFF_ACTIVE_DATE <= DateTime.Now
                      select new getOM
                      {
                          OMACCNO = o.ACC_NBR,
                          OMORDNO = o.ORD_NO,
                          SMACCNO = f.ACCESS_NUMBER,
                          SMORDNO = f.ORDER_NO,

                      }).ToList();
            return dd;
        }
        public List<lostTranQueryResponse> Handle(lostTranQuery model)
        {
            _logger.Info("Start GetLostTransaction");
            _logger.Debug(String.Format("Start GetLostTransaction"));
            string configDate = ""; string ByACCESS = "";

            var resultLov = from item in _cfgLov.Get()
                            where item.LOV_TYPE == "WS_LOST" && item.LOV_NAME == "TRANSACTION_LOST"
                            select item;

            foreach (var c in resultLov)
            {

                if (c.LOV_VAL1 != null || c.LOV_VAL1 != "")
                {
                    ByACCESS = c.LOV_VAL1.ToSafeString();
                }
                if (c.DISPLAY_VAL != null || c.DISPLAY_VAL != "")
                {
                    configDate = c.DISPLAY_VAL.ToSafeString();
                }


            }
            List<getOM> getLOSTTRAN = new List<getOM>();

            var XMLList = new List<lostTranQueryResponse>();
            string _ACC_NBR = string.Empty; string _ORDERNO = string.Empty;   //string XMLResult = string.Empty;


            string result = string.Empty;
            if (ByACCESS != "")
            {
                getLOSTTRAN = getByACCESSNO(ByACCESS);
            }
            else
            {
                getLOSTTRAN = getByDate(configDate);
            }

            _logger.Debug(String.Format("Total Record" + getLOSTTRAN.Count));
            _logger.Info("Total Record" + getLOSTTRAN.Count);
            foreach (var v in getLOSTTRAN)
            {
                if (v.SMACCNO == null && v.SMORDNO == null)
                {
                    _ACC_NBR = v.OMACCNO;
                    _ORDERNO = v.OMORDNO;
                    var XMLDATA = (from o in _PAYG.Get()
                                       //                join r in _SUBMITORD.Get()
                                       //                on o.ORDER_SFF equals r.ORDER_NO_SFF
                                       //                // start left join  
                                       //                into a
                                   where o.IN_XML_PARAM.Contains(_ACC_NBR) && o.IN_XML_PARAM.Contains(_ORDERNO) && o.METHOD_NAME.Equals("FOA Send")

                                   select new { o.IN_XML_PARAM }).FirstOrDefault();
                    if (string.IsNullOrEmpty(XMLDATA.IN_XML_PARAM.ToString()))
                    {


                    }
                    else
                    {
                        string XMLResult = XMLDATA.IN_XML_PARAM.ToString();
                        //  CallWebservice(XMLResult);
                        var ddd = new lostTranQueryResponse
                        {
                            XMLRESULT = XMLResult,
                        };

                        XMLList.Add(ddd);
                    }
                }

            }


            return XMLList;
        }


    }
    public class getOM
    {
        public string OMACCNO { get; set; }
        public string OMORDNO { get; set; }
        public string SMACCNO { get; set; }
        public string SMORDNO { get; set; }
        public string SFFDATE { get; set; }
    }
}