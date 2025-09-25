namespace WBBContract.Queries.WebServices.Tracking
{
    public class TechnicianTrackingQuery : IQuery<TechnicianTrackingModel>
    {
        public string prefixUrl { get; set; }
        public string idcard { get; set; }
        public string fibrenetId { get; set; }
        public string language { get; set; }
    }
}
