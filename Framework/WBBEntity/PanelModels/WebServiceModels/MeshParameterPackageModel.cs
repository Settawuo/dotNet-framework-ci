using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class MeshParameterPackageModel
    {
        public MeshParameterPackageModel()
        {
            if (RES_COMPLETE_CUR == null)
                RES_COMPLETE_CUR = new List<ParameterPackage>();
        }

        public string RETURN_CODE { get; set; }
        public string RETURN_MESSAGE { get; set; }
        public List<ParameterPackage> RES_COMPLETE_CUR { get; set; }
    }

    public class ParameterPackage
    {
        public string SFF_PROMOCODE { get; set; }
        public string PACKAGE_SUBTYPE { get; set; }
        public string PACKAGE_OWNER { get; set; }
        public string VAS_SERVICE { get; set; }

    }
}
