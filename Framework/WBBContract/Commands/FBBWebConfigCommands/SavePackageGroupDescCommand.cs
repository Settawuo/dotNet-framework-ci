namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class SavePackageGroupDescCommand
    {
        public string package_group { get; set; }
        public string package_group_desc_thai { get; set; }
        public string package_group_desc_eng { get; set; }
        public string user { get; set; }
        public string return_msg { get; set; }
        public decimal return_code { get; set; }
    }
}
