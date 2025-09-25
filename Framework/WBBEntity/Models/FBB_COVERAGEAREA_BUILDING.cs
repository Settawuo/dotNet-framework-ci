using System;
using System.ComponentModel;

namespace WBBEntity.Models
{
    public class FBB_COVERAGEAREA_BUILDING
    {
        public decimal CONTACT_ID { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime? UPDATED_DATE { get; set; }
        [DisplayName("Building")]
        public string BUILDING { get; set; }
        [DisplayName("Building EN")]
        public string BUILDING_EN { get; set; }
        [DisplayName("Building TH")]
        public string BUILDING_TH { get; set; }
        public string ACTIVE_FLAG { get; set; }
        [DisplayName("Install Note")]
        public string INSTALL_NOTE { get; set; }
    }
}
