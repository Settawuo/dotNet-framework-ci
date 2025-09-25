using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{

    public class ResendSsaps4Query : IQuery<List<ResendWssaps4Model>>
    {
        public string p_date_start { get; set; }
        public string p_date_to { get; set; }


    }

    public class mappingResendSsaps4Query : IQuery<mappingResendWssapslist4Model>
    {
        public string p_plant { get; set; }
        public string p_storage_location { get; set; }

        public string p_material_code { get; set; }
    }

}
