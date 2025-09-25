using System.Collections.Generic;

namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class SaveConfigPackagePage1Command
    {
        public SaveConfigPackagePage1Command()
        {
            this.return_code = -1;
            this.return_msg = "";
        }

        public string service_option { get; set; }
        public string user { get; set; }
        public string package_code { get; set; }
        public string package_type { get; set; }
        public string package_class { get; set; }
        public string sale_start_date { get; set; }
        public string sale_end_date { get; set; }
        public decimal pre_initiation_charge { get; set; }
        public decimal initiation_charge { get; set; }
        public decimal pre_recurring_charge { get; set; }
        public decimal recurring_charge { get; set; }
        public string package_name_tha { get; set; }
        public string package_name_eng { get; set; }
        public string sff_promotion_code { get; set; }
        public string sff_promotion_bill_tha { get; set; }
        public string sff_promotion_bill_eng { get; set; }
        public string download_speed { get; set; }
        public string upload_speed { get; set; }
        public string discount_type { get; set; }
        public decimal discount_value { get; set; }
        public decimal discount_day { get; set; }
        public string vas_service { get; set; }
        public string product_subtype2 { get; set; }
        public string technology { get; set; }

        private List<AirPackageDetail> _AirPackageDetail;
        public List<AirPackageDetail> airPackageDetail
        {
            get { return _AirPackageDetail; }
            set { _AirPackageDetail = value; }
        }

        private List<AirPackageVendor> _AirPackageVendor;
        public List<AirPackageVendor> airPackageVendor
        {
            get { return _AirPackageVendor; }
            set { _AirPackageVendor = value; }
        }

        // for return
        public decimal return_code { get; set; }
        public string return_msg { get; set; }

    }

    public class AirPackageDetail
    {
        public string service_option { get; set; }
        public string package_code { get; set; }
        public string product_type { get; set; }
        public string product_subtype { get; set; }
        public string product_subtype3 { get; set; }
        public string package_group { get; set; }
        public string network_type { get; set; }
        public decimal service_day_stary { get; set; }
        public decimal service_day_end { get; set; }
    }

    public class AirPackageVendor
    {
        public string service_option { get; set; }
        public string owner_product { get; set; }
    }
}
