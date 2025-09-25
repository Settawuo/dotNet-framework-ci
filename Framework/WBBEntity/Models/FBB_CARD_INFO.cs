using System;
using System.ComponentModel;

namespace WBBEntity.Models
{
    public partial class FBB_CARD_INFO
    {


        public decimal CARDID { get; set; }

        public decimal DSLAMID { get; set; }
        [DisplayName("Card Number")]
        public decimal CARDNUMBER { get; set; }
        [DisplayName("CardModelId")]
        public decimal CARDMODELID { get; set; }
        public string ACTIVEFLAG { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
        [DisplayName("Reserve Technology")]
        public string RESERVE_TECHNOLOGY { get; set; }
        public string BUILDING { get; set; }
    }
}
