namespace WBBContract.Commands
{
    public class RegAisRabbitCommand
    {
        public RegAisRabbitCommand()
        {
            this.Return_Code = -1;
            this.Return_Desc = "err";
        }

        // for return
        public int Return_Code { get; set; }
        public string Return_Desc { get; set; }

        // Properties
        public string IdCard { get; set; }
        public string Non_Mobile { get; set; }
        public string Email { get; set; }
    }
}
