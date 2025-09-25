using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class GetDataCallSapWriteOffQuery : IQuery<ReturnCallSapWriteOffModel>
    {


        public List<WriteOffQueryList> WriteOffQueryListModels { get; set; }
        //return value
        public string ret_code { get; set; }
        public string ret_msg { get; set; }
    }
    public class WriteOffQueryList
    {
        public string Access_No { get; set; }
        public string OrderType { get; set; }
        public string SerialNumber { get; set; }
        public string MaterialCode { get; set; }
        public string CompanyCode { get; set; }
        public string Plant { get; set; }
        public string StorageLocation { get; set; }
        public string SNPattern { get; set; }
        public string SNStatus { get; set; }
        public string Create_by { get; set; }

    }
}
