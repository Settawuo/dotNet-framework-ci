using System;

namespace WBBEntity.Models
{
    public partial class FBB_EMPLOYEE
    {
        public string EMP_PIN { get; set; }
        public string EMP_USER_NAME { get; set; }
        public string CREATE_BY { get; set; }
        public Nullable<System.DateTime> CREATE_DTM { get; set; }
        public string ACTIVE_FLAG { get; set; }
    }
}
