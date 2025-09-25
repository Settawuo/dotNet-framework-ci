using System.Collections.Generic;

namespace WBBEntity.PanelModels
{
    public class BillingAddressModel
    {
        public string ReturnCode { get; set; }
        public string UpdateBy { get; set; }
        public string UpdateDate { get; set; }
        public string ReturnRelated { get; set; }
        public string ErrorMessage { get; set; }
        public List<LovValueModel> BillCycleLov { get; set; } = new List<LovValueModel>();

        public BillingAddressParam GetBillingAddress { get; set; } = new BillingAddressParam();

        public BillingAddressResponse GetBillingAddressResponse { get; set; } = new BillingAddressResponse();
        public List<BillingAddressResponse> GetBillingAddressResponseList { get; set; } = new List<BillingAddressResponse>();

        public class BillingAddressParam
        {
            public string MobileNo { get; set; }
            public string IdCard { get; set; }
            public string Mode { get; set; }
            public string languageCode { get; set; }

        }

        public class BillingAddressResponse
        {
            public string BaNo { get; set; }
            public string MobileNo { get; set; } // Best MobileNo 0000000000
            public string MobileNoDisplay { get; set; } // Best MobileNo 000-XXX-0000
            public string MobileNoGroup { get; set; } // 0000000000, 0000000001
            public string MobileNoGroupDisplay { get; set; } // 000-XXX-0000, 000-XXX-0001
            public string IdCard { get; set; }
            public string HomeId { get; set; }
            public string Moo { get; set; }
            public string Mooban { get; set; }
            public string Building { get; set; }
            public string Floor { get; set; }
            public string Room { get; set; }
            public string Soi { get; set; }
            public string Street { get; set; }
            public string Tumbon { get; set; }
            public string Ampur { get; set; }
            public string Province { get; set; }
            public string ZipCode { get; set; }
            public string ZipCodeRowId { get; set; }
            public string ChannelViewBill { get; set; }
            public string BillCycleInfo { get; set; }

            //R23.08 eBill Code
            public string BillMedia { get; set; }

            // BillCycle == lov_name
            public string BillCycle { get; set; }

            public string BillCycleFirstDay { get; set; }
            public string BillCycleLastDay { get; set; }

            public string AddressDisplay { get; set; }

        }
    }
}
