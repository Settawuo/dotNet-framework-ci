using System.Collections.Generic;

namespace WBBEntity.PanelModels.ExWebServiceModels
{
    public class ListMaxMeshInstallmentRespond
    {
        public string ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
        public List<ListMaxMeshArray> ListMaxMeshArray { get; set; }
    }

    public class ListMaxMeshArray
    {
        public string EVENT { get; set; }
        public string MAXMESH { get; set; }
    }


}
