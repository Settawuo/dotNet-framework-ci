namespace WBBContract.Queries.FBBWebConfigQueries.AirWirelessCoverage
{
    public class GetLastGiftVoucherLotByVoucherMasterIDQuery : IQuery<long>
    {
        public long Voucher_Master_ID { get; set; }
    }
}
