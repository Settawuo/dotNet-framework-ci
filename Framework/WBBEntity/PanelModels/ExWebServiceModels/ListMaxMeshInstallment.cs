using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WBBEntity.PanelModels.ExWebServiceModels
{
    public class ListMaxMeshInstallment
    {
        public string ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
        public List<ListMaxMeshArray> listMaxMeshArray { get; set; }
    }

    public class ListMaxMeshArray
    {
        public string EVENT { get; set; }
        public string MAXMESH { get; set; }
    }
}
