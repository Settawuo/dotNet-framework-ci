using System.IO;
using System.Xml.Serialization;
using WBBContract;
using WBBContract.Queries.ExWebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.ExWebServices.SAPInventory
{
    public class RenewServiceQueryHandler : IQueryHandler<RenewServiceQuery, RenewServiceResponse>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        //private readonly IEntityRepository<XML_TEST> _fixAssetHisLog;
        private readonly IEntityRepository<FBB_FIXED_ASSET_HISTORY_LOG> _fixAssetHisLog;

        public RenewServiceQueryHandler(ILogger logger, IEntityRepository<FBB_FIXED_ASSET_HISTORY_LOG> fixAssetHisLog, IWBBUnitOfWork uow)
        {
            _logger = logger;
            _fixAssetHisLog = fixAssetHisLog;
            _uow = uow;

        }

        public RenewServiceResponse Handle(RenewServiceQuery model)
        {
            //var _main = (from m in _fixAssetHisLog.Get()
            //             where m.HISTORY_ID == 1987

            //             select new HistorySubmitFOAResend
            //             {
            //                 clobString = m.IN_FOA
            //             }).ToList();



            //var _main = (from m in _fixAssetHisLog.Get()
            //             //where m.ORDER_NO == "R1710006197663_1089657432"
            //             where m.FLAG == "B"
            //             select new HistorySubmitFOAResend
            //             {
            //                 ORDER_NO = m.ORDER_NO,
            //                 XML_DATA = m.XML_DATA,
            //                 FLAG = m.FLAG
            //             }).ToList();

            //foreach (var item in _main)
            //{
            //    var _cost = new XML_TEST();
            //    _cost.ORDER_NO = item.ORDER_NO;
            //    _cost.XML_DATA = item.XML_DATA;
            //    _cost.FLAG = "Z";

            //    var excModel = DeserializeXml<NewRegistForSubmitFOAQuery>(item.XML_DATA.ToString());

            //    var serviceList = new List<FBBSAPOnline1.NewRegistFOAServiceList>();
            //    foreach (var item2 in excModel.ServiceList)
            //    {
            //        var ddd = new FBBSAPOnline1.NewRegistFOAServiceList();
            //        ddd.ServiceName = item2.ServiceName;
            //        serviceList.Add(ddd);
            //    }

            //    List<FBBSAPOnline1.NewRegistFOAProductList> productList = new List<FBBSAPOnline1.NewRegistFOAProductList>();
            //    foreach (var item3 in excModel.ProductList)
            //    {
            //        var ll = new FBBSAPOnline1.NewRegistFOAProductList();
            //        ll.CompanyCode = item3.CompanyCode;
            //        ll.MaterialCode = item3.MaterialCode;
            //        ll.MovementType = item3.MovementType;
            //        ll.Plant = item3.Plant;
            //        ll.SerialNumber = item3.SerialNumber;
            //        ll.SNPattern = item3.SNPattern;
            //        ll.StorageLocation = item3.StorageLocation;
            //        productList.Add(ll);
            //    }

            //    var ss = new FBBSAPOnline1.NewRegistForSubmitFOAQuery();

            //    ss.Access_No = excModel.Access_No;
            //    ss.OrderNumber = excModel.OrderNumber;
            //    ss.SubcontractorCode = excModel.SubcontractorCode;
            //    ss.SubcontractorName = excModel.SubcontractorName;
            //    ss.ProductName = excModel.ProductName;
            //    ss.ServiceList = serviceList.ToArray();
            //    ss.OrderType = excModel.OrderType;
            //    ss.ProductList = productList.ToArray(); ;
            //    ss.SubmitFlag = excModel.SubmitFlag;
            //    ss.RejectReason = excModel.RejectReason;
            //    var conDate = excModel.FOA_Submit_date;
            //    //conDate = conDate.Substring(0, 19) + ".000+07:00";
            //    //conDate = conDate.Replace("+07:00", ".000+07:00");
            //    //conDate = conDate.Replace("+07:00", ".000+07:00");
            //    //conDate = conDate.Replace("+07:00", ".000+07:00");
            //    ss.FOA_Submit_date = conDate;
            //    ss.OLT_NAME = excModel.OLT_NAME;
            //    ss.BUILDING_NAME = excModel.BUILDING_NAME;
            //    ss.Mobile_Contact = excModel.Mobile_Contact;
            //    ss.ORG_ID = excModel.ORG_ID;
            //    ss.Address_ID = excModel.Address_ID;
            //    ss.Event_Flow_Flag = excModel.Event_Flow_Flag;
            //    ss.Reuse_Flag = excModel.Reuse_Flag;

            //    //var aa = new FBBSAPOnline1.FBBSAPOnline();
            //    //var res = aa.NewRegistForSubmitFOA(ss);
            //    //_logger.Info("res.result: " + res.result);
            //    _logger.Info("ORDER_NO: " + item.ORDER_NO);

            //    _fixAssetHisLog.Update(_cost);
            //    _uow.Persist();
            //}

            return null;
        }

        private static T DeserializeXml<T>(string xmlString)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (TextReader reader = new StringReader(xmlString))
            {
                return (T)serializer.Deserialize(reader);
            }
        }
    }
}
