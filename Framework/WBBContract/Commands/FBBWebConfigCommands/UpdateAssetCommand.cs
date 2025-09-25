using System.Collections.Generic;

namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class UpdateAssetCommand
    {
        public List<FBB_Update_asset_list> p_Update_asset_list { get; set; }
        public List<FBB_Update_asset_list> p_Update_asset_list_success { get; set; }
        public List<FBB_Update_asset_list> p_Update_asset_list_Error { get; set; }
        public int ret_code { get; set; }
        public string ret_msg { get; set; }

    }
    public class FBB_Update_asset_list
    {
        public string p_Access_No { get; set; }
        public string p_Serial_Number { get; set; }
        public string p_Asset_Code { get; set; }
        public string p_Mat_Doc { get; set; }
        public string p_Doc_Year { get; set; }

    }
}
