using System.Collections.Generic;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.ExWebServices.FbbCpGw
{
    public class GetSubContractTimeSlotQuery : CpGateWayQueryBase, IQuery<List<FBSSTimeSlot>>
    {
        public int Days { get; set; }

        public string District { get; set; }

        public string InstallationDate { get; set; }

        public string Language { get; set; }

        public string PostalCode { get; set; }

        public string Province { get; set; }

        public string ServiceCode { get; set; }

        public string SubDistrict { get; set; }
    }
}