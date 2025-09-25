using System.Collections.Generic;

namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class FBBPaygPatchDataUpdateCommand
    {
        public string Transaction_Id { get; set; }
        public object FileName { get; set; }
        public string patchstatus { get; set; }
        public string serialno { get; set; }
        public int returnCode { get; set; }
        public string returnMsg { get; set; }
        public string FullUrl { get; set; }
        public string remark { get; set; }
        public string CREATE_BY { get; set; }
        public string snstatus { get; set; }
    }
    public class FBBPaygPatchDataListUpdateCommand
    {
        public string SERIAL_NO { get; set; }
        public string FILE_NAME { get; set; }
        public string PATCH_STATUS { get; set; }
        public string CREATE_BY { get; set; }
        public int ret_code { get; set; }
        public string ret_msg { get; set; }
        public List<FBBPaygPatchDataListUpdateCommand> p_Product_List { get; set; }
    }
    public class FBBPaygPatchDataInsertCommand
    {
        public string Transaction_Id { get; set; }
        public string FileName { get; set; }
        public string serialno { get; set; }
        public string internetno { get; set; }
        public string movementtype { get; set; }
        public string submitdate { get; set; }
        public string postdate { get; set; }
        public string serialstatus { get; set; }
        public string patchstatus { get; set; }
        public string patchstatusdesc { get; set; }
        public string createdate { get; set; }
        public string createby { get; set; }
        public string updatedate { get; set; }
        public string updateby { get; set; }
        public int returnCode { get; set; }
        public List<PatchCPEModel> p_Product_List { get; set; }
        public string returnMsg { get; set; }
        public string FullUrl { get; set; }
    }

    public class FBBPaygPatchDataInsertSendMailCommand
    {
        public string FILE_NAME { get; set; }
        public string EMAIL { get; set; }
        public string USER_NAME { get; set; }
        public string ACTIVE_FLAG { get; set; }
        public string ret_code { get; set; }
        public string ret_msg { get; set; }
    }
    public class PatchCPEModel
    {
        public string FILE_NAME { get; set; }
        public string SERIAL_NO { get; set; }
        public string INTERNET_NO { get; set; }
        public string STORAGE_LOCATION { get; set; }
        public string MOVEMENT_TYPE { get; set; }
        public string FOA_CODE { get; set; }
        public string SUBMIT_DATE { get; set; }
        public string POST_DATE { get; set; }
        public string SERIAL_STATUS { get; set; }
        public string PATCH_STATUS { get; set; }
        public string PATCH_STATUS_DES { get; set; }
        public string CREATE_BY { get; set; }
        public string REMARK { get; set; }
    }
}
