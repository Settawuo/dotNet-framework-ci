namespace WBBContract.Commands.FBSS
{
    public class FBSSSubContractTimeslotInsertTableCommand
    {
        public string Filepath { get; set; }
        public string Filename { get; set; }
        public string Bufferdata { get; set; }
        public string DataDate { get; set; }

        public decimal Recode { get; set; }
        public string Remessage { get; set; }
    }

}
