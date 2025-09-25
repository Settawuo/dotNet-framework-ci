namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class CreateNewVoucherProjectCommand
    {
        public string voucher_project_group { get; set; }
        public string voucher_project_code { get; set; }
        public string voucher_project_des { get; set; }

        public string resultMessage { get; set; }
    }
}
