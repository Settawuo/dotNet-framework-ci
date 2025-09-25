using AIRNETEntity.StoredProc;

namespace WBBContract.Queries.WebServices.Tracking
{
    public class TechnicianTrackingModel : GotoTrackModel
    {
        public GotoTrackModel AsTotractModel()
        {
            return (GotoTrackModel)this;
        }
    }
}
