using WBBEntity.PanelModels;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetSaveOrderNewQuery : IQuery<SaveOrderNewModel>
    {
        private QuickWinPanelModel _QuickWinPanelModel;
        public QuickWinPanelModel QuickWinPanelModel
        {
            get { return _QuickWinPanelModel ?? (_QuickWinPanelModel = new QuickWinPanelModel()); }
            set { _QuickWinPanelModel = value; }
        }
    }
}
