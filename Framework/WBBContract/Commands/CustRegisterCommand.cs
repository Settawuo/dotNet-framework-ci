using System;
using WBBEntity.PanelModels;

namespace WBBContract.Commands
{
    public class CustRegisterCommand
    {
        private QuickWinPanelModel _QuickWinPanelModel;
        public QuickWinPanelModel QuickWinPanelModel
        {
            get { return _QuickWinPanelModel ?? (_QuickWinPanelModel = new QuickWinPanelModel()); }
            set { _QuickWinPanelModel = value; }
        }

        public string CustomerId { get; set; }

        public int CurrentCulture { get; set; }

        public string InterfaceCode { get; set; }
        public string InterfaceDesc { get; set; }
        public string InterfaceOrder { get; set; }

        public string CreateBy { get; set; }
        public string CreateTS { get; set; }

        public string UpdateBy { get; set; }
        public DateTime UpdateTS { get; set; }

        /// <summary>
        /// a result that return from coverage checking.
        /// </summary>
        public decimal? CoverageResultId { get; set; }

        //update 17.1
        public string ClientIP { get; set; }
    }

    public class CustRegisterJobCommand
    {
        public string RETURN_IA_NO { get; set; }
        public string RETURN_ORDER_NO { get; set; }
        public string PAYMENTMETHOD { get; set; }
        public string TRANSACTIONID_IN { get; set; }
        public string TRANSACTIONID { get; set; }
        public string PLUG_AND_PLAY_FLAG { get; set; }
        public string REGISTER_TYPE { get; set; }
        public string RESERVED_PORT_ID { get; set; }
        public string ACCESS_MODE { get; set; }
        public string FullUrl { get; set; }
        public string ClientIP { get; set; }

        public string INTERNET_NO { get; set; }

        // return

        public string CUSTOMERID { get; set; }
    }
}
