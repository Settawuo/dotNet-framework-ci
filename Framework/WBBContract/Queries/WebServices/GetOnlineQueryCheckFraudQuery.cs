using System.Collections.Generic;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    //R23.05 CheckFraud
    public class GetOnlineQueryCheckFraudQuery : IQuery<GetOnlineQueryCheckFraudQueryModel>
    {
        public string ONLINEAUTH_TOKEN { get; set; }
        public string CONTACT_MOBILE_NO { get; set; }                        // "0860000000",
        public string CUSTOMER_TYPE { get; set; }                                 // "Residentail",
        public string AIS_FLAG { get; set; }                                                // "Y",
        public string CUSTOMER_NAME { get; set; }                                // "Test test",
        public string ID_CARD { get; set; }                                                 // "0000000000000",
        public string CARD_TYPE { get; set; }                                            // "บัตรประชาชน",
        public string AIS_MOBILE_SEGMENT { get; set; }                       // "Classic",
        public string SERVICE_MONTH { get; set; }                                  // "3",
        public string SERVIVE_YEAR { get; set; }                                      // "0",
        public string SERVICE_LEVEL { get; set; }                                     // "L",
        public string AR_BALANCE_CA { get; set; }                                  // "0",
        public string LIMIT_MOBILE { get; set; }                                       // "",
        public string CHARGE_TYPE { get; set; }                                      // "Pre Paid",
        public string SLOT_WAITING_LIST_FLAG { get; set; }                // "Y",
        public string INSTALL_REGION { get; set; }                                  // "CB",
        public string USE_ID_CARD_ADDRESS_FLAG { get; set; }        // "Y",
        public InstallAddressInfo INSTALL_ADDRESS_INFO { get; set; }
        //{ get => _install_address_info; set => _install_address_info = value; }
        public string SALE_CHANNEL { get; set; }             //"ACC Shop",
        public string LOCATION_REGION { get; set; }       //"CB",
        public string LOCATION_PROVINCE { get; set; }  //"กรุงเทพ",
        public string LOCATION_CODE { get; set; }           //"1001",
        public string ASC_CODE { get; set; }                       //"3",
        public string ENTRY_FEE { get; set; }                     //"800",
        public string CS_NOTE { get; set; }                          //"test",
        public string ADDRESS_ID { get; set; }                   //"123",
        public List<Promotionlist> PROMOTIONLIST { get; set; }  // { get => _promotionlist; set => _promotionlist = value; }
        public List<string> OFFERING { get; set; }         // ["CVM","CSM","PURGE"],
        public string OPERATOR { get; set; }                   //"3BB",
        public string EMPLOYEE_CODE { get; set; }                   //"555555",
        public string SALE_REPRESENTATIVE { get; set; }                   //"test",


        public string PROMOTIONLISTJSON { get; set; }
        public string OFFERINGJSON { get; set; }
    }

    public class InstallAddressInfo
    {
        public string HOUSE_NO { get; set; }              // "11/12",
        public string MOO_NO { get; set; }                  //"",
        public string MOO_BAAN { get; set; }            //"เทส",
        public string BUILDING_NAME { get; set; }   //"พหลเพรส
        public string FLOOR { get; set; }                      //"55",
        public string ROOM { get; set; }                       //"56",
        public string SOI { get; set; }                            //"57",
        public string TUMBON { get; set; }                  //"58",
        public string AMPHUR { get; set; }                  //"สายลม",
        public string PROVICNE { get; set; }               //"พญาไท",
        public string ZIP_CODE { get; set; }                //"กรุงเทพ",
    }

    public class Promotionlist
    {
        public string PROMOTION_CODE { get; set; }  //"P12345232",
        public string PROMOTION_NAME { get; set; } //"FBB Discount 500 THB for 24 months",
        public string PROMOTION_TYPE { get; set; }   //"On-Top",
        public string PROMOTION_PRICE { get; set; }  //"999.33"
    }
}