using System.Collections.Generic;
using WBBEntity.PanelModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class GetFormatFileNameFraudQuery : IQuery<FileFormatFraudModel>
    {
        public string ID_CardNo { get; set; }

    }
}
