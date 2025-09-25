using System.Collections.Generic;

namespace WBBEntity.PanelModels
{
    public class SaveChangeStatusBuildingModel
    {
        public SaveChangeStatusBuildingModel()
        {
            SaveChangeStatusBuildingList = new List<StatusBuilding>();
        }

        public string ReturnCode { get; set; }
        public string UpdateBy { get; set; }
        public string UpdateDate { get; set; }
        public string AddressId { get; set; }

        public List<StatusBuilding> SaveChangeStatusBuildingList { get; set; }

        public string isSuccessIM { get; set; }
        public string isSuccessSD { get; set; }

        public class StatusBuilding
        {
            public string AddressId { get; set; }
            public string ChkStatus { get; set; }
            public string ChkFttrStatus { get; set; }
            public string ChkStatusText { get; set; }
            public string ChkFttrStatusText { get; set; }
            public string Reason { get; set; }
            public bool isSaveReason { get; set; }
        }
    }
}
