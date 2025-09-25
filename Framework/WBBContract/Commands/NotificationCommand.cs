using WBBEntity.PanelModels;

namespace WBBContract.Commands
{
    public class NotificationCommand
    {
        public int CurrentCulture { get; set; }
        public string CustomerId { get; set; }
        public decimal RunningNo { get; set; }

        private EmailModel _EmailModel;
        public EmailModel EmailModel
        {
            get { return _EmailModel ?? (_EmailModel = new EmailModel()); }
            set { _EmailModel = value; }
        }

        public string ImpersonateUser { get; set; }
        public string ImpersonatePass { get; set; }
        public string ImpersonateIP { get; set; }

        public decimal ReturnCode { get; set; }
        public string ReturnDesc { get; set; }

    }
}
