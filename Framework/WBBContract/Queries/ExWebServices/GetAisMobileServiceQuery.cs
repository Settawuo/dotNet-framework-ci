using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBContract.Queries.ExWebServices
{
    public enum VsmpChargeMode
    {
        PostPaid = 0,
        PrePaid = 1,
        CreditLimit = 2,
    }

    public class GetAisMobileServiceQuery : IQuery<AisMobileModel>
    {
        public string UserName { get; set; }
        public string OrderRef { get; set; }
        public string OrderDesc { get; set; }
        public string Msisdn { get; set; }
        public string Opt1 { get; set; }
        public string Opt2 { get; set; }
        public string User { get; set; }

        public string TransactionId { get; set; }
    }
}
