using System;
using System.Collections.Generic;

namespace WBBEntity.PanelModels
{
    public class ListPackageDisplayModel : IEquatable<ListPackageDisplayModel>
    {
        public string non_mobile_no { get; set; }
        public string ref_row_id { get; set; }
        public string package_state { get; set; }
        public decimal package_seq { get; set; }
        public string package_subseq { get; set; }
        public string font_type { get; set; }
        public decimal line_seq { get; set; }
        public string package_display_th { get; set; }
        public string package_display_en { get; set; }

        // Update 17.3
        public string sff_promotion_code { get; set; }
        public string startdt { get; set; }
        public string enddt { get; set; }

        public string package_description_th { get; set; }
        public string package_description_en { get; set; }

        public string package_type_desc { get; set; }
        public string product_subtype { get; set; }
        public string product_subtype1 { get; set; }
        public decimal? sub_seq { get; set; }

        public bool Equals(ListPackageDisplayModel other)
        {
            if (sff_promotion_code == other.sff_promotion_code &&
                package_display_th == other.package_display_th &&
                package_display_en == other.package_display_en &&
                package_description_th == other.package_description_th &&
                package_description_en == other.package_description_en &&
                package_type_desc == other.package_type_desc &&
                product_subtype == other.product_subtype &&
                product_subtype1 == other.product_subtype1 &&
                package_state == other.package_state &&
                package_seq == other.package_seq &&
                package_subseq == other.package_subseq &&
                startdt == other.startdt &&
                enddt == other.enddt)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            int hashsff_promotion_code = sff_promotion_code == null ? 0 : sff_promotion_code.GetHashCode();
            int hashpackage_display_th = package_display_th == null ? 0 : package_display_th.GetHashCode();
            int hashpackage_display_en = package_display_en == null ? 0 : package_display_en.GetHashCode();
            int hashpackage_description_th = package_description_th == null ? 0 : package_description_th.GetHashCode();
            int hashpackage_description_en = package_description_en == null ? 0 : package_description_en.GetHashCode();
            int hashpackage_type_desc = package_type_desc == null ? 0 : package_type_desc.GetHashCode();
            int hashproduct_subtype = product_subtype == null ? 0 : product_subtype.GetHashCode();
            int hashproduct_subtype1 = product_subtype1 == null ? 0 : product_subtype1.GetHashCode();
            int hashpackage_state = package_state == null ? 0 : package_state.GetHashCode();
            int hashpackage_seq = package_seq == 0 ? 0 : package_seq.GetHashCode();
            int hashpackage_subseq = package_subseq == null ? 0 : package_subseq.GetHashCode();
            int hashstartdt = startdt == null ? 0 : startdt.GetHashCode();
            int hashenddt = enddt == null ? 0 : enddt.GetHashCode();

            return hashsff_promotion_code ^ hashpackage_display_th ^ hashpackage_display_en ^ hashpackage_description_th ^ hashpackage_description_en ^
                   hashpackage_type_desc ^ hashproduct_subtype ^ hashproduct_subtype1 ^ hashpackage_state ^ hashpackage_seq ^ hashpackage_subseq ^ hashstartdt ^ hashenddt;
        }

    }

    public class ListPackageChangeOnlineModel
    {
        public string RESULT_CODE { get; set; }
        public string RESULT_DESC { get; set; }
        public string TRANSACTION_ID { get; set; }
        public List<ListPackageChangeModel> LIST_PACKAGE_BY_CHANGE { get; set; }
    }

    public class ListPackageChangeModel
    {

        public string auto_mapping_code { get; set; }
        public string serenade_flag { get; set; }
        public string package_code { get; set; }
        public decimal recurring_charge { get; set; }
        public decimal pre_recurring_charge { get; set; }
        public decimal initiation_charge { get; set; }
        public decimal pre_initiation_charge { get; set; }
        public string technology { get; set; }
        public string download_speed { get; set; }
        public string upload_speed { get; set; }
        public string package_type { get; set; }
        public string product_subtype { get; set; }
        public string owner_product { get; set; }
        public string discount_type { get; set; }
        public Nullable<decimal> discount_value { get; set; }
        public Nullable<decimal> discount_day { get; set; }
        public string sff_promotion_code { get; set; }
        public string sff_promotion_bill_tha { get; set; }
        public string sff_promotion_bill_eng { get; set; }
        public string package_group { get; set; }
        public string seq { get; set; }
        public string project_name { get; set; }
        public string display_seq { get; set; }
        public string display_flag { get; set; }
        public string mapping_code { get; set; }
        public string package_type_desc { get; set; }
        public string sff_word_in_statement_tha { get; set; }
        public string sff_word_in_statement_eng { get; set; }
        public string package_group_seq { get; set; }
        public string sub_seq { get; set; }
        public string auto_mapping_promotion_code { get; set; }

        //R20.6 ChangePromotionCheckRight
        public string package_display_eng { get; set; }
        public string package_display_tha { get; set; }

        //R17.02.2021 ForRessponse Res API
        public string price_charge { get; set; }
        public string pre_price_charge { get; set; }
        public string sff_product_name { get; set; }
        public string send_sff_flag { get; set; }

        //R21.11 add wording package desc and remark
        public string package_group_desc_tha { get; set; }
        public string package_group_desc_eng { get; set; }
        public string package_remark_tha { get; set; }
        public string package_remark_eng { get; set; }
    }

    public class ListChangePackageModel
    {
        public string error_code { get; set; }
        public string error_msg { get; set; }
        public string order_no { get; set; }
        public string non_mobile_no { get; set; }
        public string relate_mobile { get; set; }
        public string sff_promotion_code { get; set; }
        public string action_status { get; set; }
        public string package_state { get; set; }
        public string project_name { get; set; }
        public string product_seq { get; set; }
        public string old_relate_mobile { get; set; }
        public string bundling_mobile_action { get; set; }

        // R20.6 Add by Aware : Atipon
        public string send_sff_flag { get; set; }
        public string wordingChangePromotionSummary { get; set; }
        public string new_project_name { get; set; }
        public string new_project_name_opt { get; set; }
        public string new_mobile_check_right { get; set; }
        public string new_mobile_check_right_opt { get; set; }
        public string new_mobile_get_benefit { get; set; }
        public string new_mobile_get_benefit_opt { get; set; }

        public string change_mobile_benefit_from { get; set; }
        public string change_mobile_benefit_to { get; set; }
        public string change_benefit_type_from { get; set; }
        public string change_benefit_type_to { get; set; }
    }
}
