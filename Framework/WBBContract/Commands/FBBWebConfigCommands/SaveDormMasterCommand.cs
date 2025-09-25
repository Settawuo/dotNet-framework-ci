using System.Collections.Generic;
namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class SaveDormMasterCommand
    {
        public SaveDormMasterCommand()
        {
            this.Save_return_code = -1;
            this.Save_return_msg = "";
            this.Save_complete = "";
            this.Update_status_fail = "";
            this.Save_fail = "";
        }
        public List<DormMasterData> DormMasterDataList { get; set; }

        public string Save_complete { get; set; }
        public string Update_status_fail { get; set; }
        public string Save_fail { get; set; }


        public decimal Save_return_code { get; set; }
        public string Save_return_msg { get; set; }
    }

    public class DormMasterData
    {
        public string Edit_dormitory_ID { get; set; }
        public string Save_dormitory_name_th { get; set; }
        public string Save_dormitory_name_en { get; set; }
        public string Save_HOME_NO_TH { get; set; }
        public string Save_MOO_TH { get; set; }
        public string Save_SOI_TH { get; set; }
        public string Save_STREET_NAME_TH { get; set; }
        public string Save_HOME_NO_EN { get; set; }
        public string Save_MOO_EN { get; set; }
        public string Save_SOI_EN { get; set; }
        public string Save_STREET_NAME_EN { get; set; }
        public string Save_TUMBON_TH { get; set; }
        public string Save_AMPHUR_TH { get; set; }
        public string Save_Province_TH { get; set; }
        public string Save_TUMBON_EN { get; set; }
        public string Save_AMPHUR_EN { get; set; }
        public string Save_Province_EN { get; set; }
        public string Save_Postcode_TH { get; set; }
        public string Save_Postcode_EN { get; set; }

        public string Save_target_launch_dt { get; set; }
        public string Save_launch_dt { get; set; }
        public string Save_target_volumn { get; set; }
        public string Save_volumn { get; set; }
        public string Save_dorm_contract_name { get; set; }
        public string Save_dorm_contract_email { get; set; }
        public string Save_dorm_contract_phone { get; set; }

        public string Save_building_th { get; set; }
        public string Save_building_en { get; set; }
        public string Save_room_amount { get; set; }

        public string User { get; set; }


    }

    public class EditDormMasterCommand
    {
        public string Edit_dormitory_ID { get; set; }
        public string Edit_dormitory_name_th { get; set; }
        public string Edit_dormitory_name_en { get; set; }
        public string Edit_HOME_NO_TH { get; set; }
        public string Edit_MOO_TH { get; set; }
        public string Edit_SOI_TH { get; set; }
        public string Edit_STREET_NAME_TH { get; set; }
        public string Edit_HOME_NO_EN { get; set; }
        public string Edit_MOO_EN { get; set; }
        public string Edit_SOI_EN { get; set; }
        public string Edit_STREET_NAME_EN { get; set; }
        public string Edit_TUMBON_TH { get; set; }
        public string Edit_AMPHUR_TH { get; set; }
        public string Edit_Province_TH { get; set; }
        public string Edit_TUMBON_EN { get; set; }
        public string Edit_AMPHUR_EN { get; set; }
        public string Edit_Province_EN { get; set; }
        public string Edit_Postcode_TH { get; set; }
        public string Edit_Postcode_EN { get; set; }

        public string Edit_target_launch_dt { get; set; }
        public string Edit_launch_dt { get; set; }
        public string Edit_target_volumn { get; set; }
        public string Edit_volumn { get; set; }
        public string Edit_dorm_contract_name { get; set; }
        public string Edit_dorm_contract_email { get; set; }
        public string Edit_dorm_contract_phone { get; set; }

        public string User { get; set; }
        public string Edit_return_code { get; set; }
        public string Edit_return_msg { get; set; }

        public string Save_building_th { get; set; }
        public string Save_building_en { get; set; }
        public string Save_room_amount { get; set; }

        public string Save_complete { get; set; }
        public string Update_status_fail { get; set; }
        public string Save_fail { get; set; }
        //   public List<DormMasterData> DormMasterDataList { get; set; }
    }

    public class EditBuildingCommand
    {
        public string Edit_dormitory_ID { get; set; }
        public string Edit_building_th { get; set; }
        public string Edit_building_en { get; set; }
        public string Edit_dormitory_no_th { get; set; }
        public string Edit_room_amount { get; set; }
        public string Edit_return_code { get; set; }
        public string Edit_return_msg { get; set; }
        public string User { get; set; }
        public string Save_complete { get; set; }
        public string Update_status_fail { get; set; }
        public string Save_fail { get; set; }
    }
    public class EditAddBuildingCommand
    {
        public string EditAdd_dormitory_ID { get; set; }
        public string EditAdd_building_th { get; set; }
        public string EditAdd_building_en { get; set; }
        public string EditAdd_room_amount { get; set; }
        public string Edit_return_code { get; set; }
        public string Edit_return_msg { get; set; }
        public string User { get; set; }
        public string Save_complete { get; set; }
        public string Update_status_fail { get; set; }
        public string Save_fail { get; set; }
    }

}
