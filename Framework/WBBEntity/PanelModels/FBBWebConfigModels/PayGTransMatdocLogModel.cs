using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    public class PayGTransMatdocLogModel
    {
    }

    public class PayGTransMatdocLogListResult
    {
        public int Return_Code { get; set; }
        public string Return_Desc { get; set; }
        //public List<PAYGTransAirnetFileList> Data { get; set; }
    }

    //public class PAYGTransAirnetList
    //{
    //    public string data1 { get; set; }
    //    public string Data { get; set; }
    //}
    //public class PAYGTransAirnetFileList
    //{
    //    public string file_name { get; set; }
    //    public string file_data { get; set; }
    //}

    //public class ConnectionNasPAYGListResult
    //{
    //    public int Return_Code { get; set; }
    //    public string Return_Desc { get; set; }
    //    public ConnectionNasPAYG NasTemp { get; set; }
    //    public ConnectionNasPAYG NasSap { get; set; }
    //    public ConnectionNasPAYG NasTarget { get; set; }
    //    public ConnectionNasPAYG NasSapNew { get; set; }
    //}
    //public class ConnectionNasPAYG
    //{
    //    public string Username { get; set; }
    //    public string Password { get; set; }
    //}
}
