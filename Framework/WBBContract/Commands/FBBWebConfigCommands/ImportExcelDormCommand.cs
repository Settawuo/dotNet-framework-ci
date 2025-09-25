namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class ImportExcelDormCommand
    {
        //public List<IPDexportlist> Imex;
        public string filename { get; set; }
        public string user { get; set; }
        public string Return_Code { get; set; }
        public string Return_Desc { get; set; }
        public string Dormitory_Name { get; set; }
        public string Dormitory_NameTH { get; set; }
        public string Building_Name { get; set; }
        public string Floor { get; set; }
        public string Room { get; set; }
        public string Fibrenet_id { get; set; }
        public string Pin { get; set; }
    }
}
