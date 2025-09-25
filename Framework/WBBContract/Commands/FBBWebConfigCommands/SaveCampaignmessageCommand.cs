using System.Collections.Generic;

namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class SaveCampaignmessageCommand
    {
        public SaveCampaignmessageCommand()
        {
            this.return_code = -1;
            this.return_msg = "";

            if (p_rec_campaign_netghbor == null)
                p_rec_campaign_netghbor = new List<FbbCampaignNeighborArrayModel>();
        }
        public string p_referral_name { get; set; }
        public string p_referral_surname { get; set; }
        public string p_referral_staff_id { get; set; }
        public string p_referral_internet_no { get; set; }
        public string p_referral_contact_mobile_no { get; set; }
        public string p_referral_neighbor_no { get; set; }

        public string p_location_code { get; set; }
        public string p_asc_code { get; set; }
        public string p_emp_id { get; set; }
        public string p_sales_rep { get; set; }

        //v18.10
        public string p_channal { get; set; }
        public string p_campaign { get; set; }
        public string p_url { get; set; }

        public List<FbbCampaignNeighborArrayModel> p_rec_campaign_netghbor { get; set; }
        //for return
        public decimal return_code { get; set; }
        public string return_msg { get; set; }
    }
    public class FbbCampaignNeighborArrayModel
    {
        public string language { get; set; }
        public string service_speed { get; set; }
        public string cust_name { get; set; }
        public string cust_surname { get; set; }
        public string contact_mobile_no { get; set; }
        public string is_ais_mobile { get; set; }
        public string contact_email { get; set; }
        public string address_type { get; set; }
        public string building_name { get; set; }
        public string house_no { get; set; }
        public string soi { get; set; }
        public string road { get; set; }
        public string sub_district { get; set; }
        public string district { get; set; }
        public string province { get; set; }
        public string postal_code { get; set; }
        public string contact_time { get; set; }

        //V17.3


    }
}
