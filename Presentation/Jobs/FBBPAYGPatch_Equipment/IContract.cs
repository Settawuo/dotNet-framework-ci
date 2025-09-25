using System.Collections.Generic;
using WBBContract.Commands;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.ExWebServices;
using WBBContract.Queries.PatchEquipment;
using WBBEntity.PanelModels.ExWebServiceModels;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBPAYGPatch_Equipment
{
    internal interface IContract
    {
        List<RetCheckSerialStatus> CheckSerialStatus(CheckSerialStatusQuery model);
        List<RetPatchEquipment> getdataPatchEquipment(PatchEquipmentQuery patchEquipment);
        IEnumerable<FbssConfigTBL> Get_FBSS_CONFIG_TBL_LOV(string _CON_TYPE, string _CON_NAME);
        string GetDataPatchSNsendmail(string FileName);
        NewRegistForSubmitFOAResponse CallWSNewRegistSAP(NewRegistForSubmitFOAQuery model);
        void TblUpdateData(UpdateFbssPatchDataConfigTblCommand queryUpdateDate);
        UpdateCPEResponse CallWSUpdateCPE(UpdateCPE setPatchupdateCPE);
        string[] SendMail(SendMailBatchPatchDataList sendmail);
        void UpdatePatchDataSN(FBBPaygPatchDataUpdateCommand commandupdate);
    }
}