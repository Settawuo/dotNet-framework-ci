using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class GetPaymentOrderIDQuery : IQuery<string>
    {

    }

    public class GetORDPendingPaymentQuery : IQuery<GetORDPendingPaymentModel>
    {
        public string p_payment_method { get; set; }
    }

    public class GetListORDDetailCreateQuery : IQuery<GetListORDDetailCreateModel>
    {
        public string p_order_id { get; set; }
    }

    public class CheckOrderPaymentStatusQuery : IQuery<string>
    {

    }

    public class GetORDPendingPaymentTimeOutQuery : IQuery<GetORDPendingPaymentTimeOutModel>
    {

    }


}
