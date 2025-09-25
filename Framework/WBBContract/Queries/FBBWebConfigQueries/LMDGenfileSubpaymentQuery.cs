using System.Collections.Generic;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class LMDGenfileSubpaymentQuery : IQuery<LMDGenfileSubpaymentReturn>
    {
        public string flag_check { get; set; }
    }

    public class LMDGenfileSubpaymentReturn
    {
        public string ret_code { get; set; }
        public string ret_msg { get; set; }
        public List<LMDGenfileListCursor> out_cursor { get; set; }
    }

    public class LMDGenfileListCursor
    {
        public string FILE_NAME { get; set; }
        public string FILE_DATA { get; set; }
    }
}
