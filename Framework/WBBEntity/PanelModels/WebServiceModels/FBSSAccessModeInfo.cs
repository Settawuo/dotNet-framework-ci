using System;
using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class FBSSAccessModeInfo
    {
        public string AccessMode { get; set; }

        public List<ResourceInfo> ResourceList { get; set; }

        public DateTime? InserviceDate { get; set; }
    }

    public class ResourceInfo
    {
        public string ResourceName { get; set; }
    }
}
