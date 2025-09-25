namespace WBBContract.Commands
{
    public class AvailableVOIPIPCommand
    {


        public AvailableVOIPIPCommand()
        {
            this.Return_Code = -1;
            this.Return_Desc = "";
        }

        public string IP_ADDRESS { get; set; }
        public string REFF_USER { get; set; }
        public string REFF_KEY { get; set; }
        public int Return_Code { get; set; }
        public string Return_Desc { get; set; }
        public string FlaxData { get; set; }


    }


}
