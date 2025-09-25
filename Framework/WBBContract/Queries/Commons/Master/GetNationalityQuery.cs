using System.Collections.Generic;
using WBBEntity.PanelModels;

namespace WBBContract.Queries.Commons.Master
{
    public class GetNationalityQuery : IQuery<List<NationalityModel>>
    {
        public int CurrentCulture { get; set; }
    }
}
