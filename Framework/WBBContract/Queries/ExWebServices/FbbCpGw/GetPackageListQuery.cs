using System.Collections.Generic;
using WBBContract.Queries.WebServices;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.ExWebServices.FbbCpGw
{
    public class GetPackageListQuery : IGetPackageListQuery,
        IQuery<List<PackageGroupModel>>
    {
        private string p_owner_product;
        public string P_OWNER_PRODUCT
        {
            get { return p_owner_product; }
            set { p_owner_product = value; }
        }

        private string p_product_subtype;
        public string P_PRODUCT_SUBTYPE
        {
            get { return p_product_subtype; }
            set { p_product_subtype = value; }
        }

        private string p_network_type;
        public string P_NETWORK_TYPE
        {
            get { return p_network_type; }
            set { p_network_type = value; }
        }

        private string p_service_day;
        public string P_SERVICE_DAY
        {
            get { return p_service_day; }
            set { p_service_day = value; }
        }

        private string p_package_for;
        public string P_PACKAGE_FOR
        {
            get { return p_package_for; }
            set { p_package_for = value; }
        }

        private string p_package_code;
        public string P_PACKAGE_CODE
        {
            get { return p_package_code; }
            set { p_package_code = value; }
        }

        private string transactionId;
        public string TransactionID
        {
            get { return transactionId; }
            set { transactionId = value; }
        }

        private bool isPartner;
        public bool IsPartner
        {
            get { return isPartner; }
            set { isPartner = value; }
        }

        private string partnerName;
        public string PartnerName
        {
            get { return partnerName; }
            set { partnerName = value; }
        }

        private string _P_Location_Code;
        public string P_Location_Code
        {
            get { return _P_Location_Code; }
            set { _P_Location_Code = value; }
        }

        private string _P_Asc_Code;
        public string P_Asc_Code
        {
            get { return _P_Asc_Code; }
            set { _P_Asc_Code = value; }
        }

        private string _P_Region;
        public string P_Region
        {
            get { return _P_Region; }
            set { _P_Region = value; }
        }

        private string _P_Province;
        public string P_Province
        {
            get { return _P_Province; }
            set { _P_Province = value; }
        }

        private string _P_District;
        public string P_District
        {
            get { return _P_District; }
            set { _P_District = value; }
        }
        private string _P_Sub_District;
        public string P_Sub_District
        {
            get { return _P_Sub_District; }
            set { _P_Sub_District = value; }
        }

        private string _P_Address_Type;
        public string P_Address_Type
        {
            get { return _P_Address_Type; }
            set { _P_Address_Type = value; }
        }

        private string _P_Building_Name;
        public string P_Building_Name
        {
            get { return _P_Building_Name; }
            set { _P_Building_Name = value; }
        }

        private string _P_Building_No;
        public string P_Building_No
        {
            get { return _P_Building_No; }
            set { _P_Building_No = value; }
        }

        private string _P_Partner_Type;
        public string P_Partner_Type
        {
            get { return _P_Partner_Type; }
            set { _P_Partner_Type = value; }
        }

        private string _P_Partner_SubType;
        public string P_Partner_SubType
        {
            get { return _P_Partner_SubType; }
            set { _P_Partner_SubType = value; }
        }

        private string _p_Serenade_Flag;
        public string P_Serenade_Flag
        {
            get { return _p_Serenade_Flag; }
            set { _p_Serenade_Flag = value; }
        }

        private string _p_Customer_Type;
        public string P_Customer_Type
        {
            get { return _p_Customer_Type; }
            set { _p_Customer_Type = value; }
        }

        public string P_Address_Id { get; set; }

        // Added by PatthamawadeeH. 28122016
        public string P_Plug_And_Play_Flag { get; set; }

        // 17.9
        public string P_Rental_Flag { get; set; }

        // 17.10
        public string P_Customer_subtype { get; set; }

        //R18.1
        public string P_Router_Flag { get; set; }

        //19.1
        public string P_FMPA_Flag { get; set; }

        //19.3
        public string P_Mobile_Price { get; set; }
        public string P_Service_Year { get; set; }
        public string P_Existing_Mobile { get; set; }

        public string P_Mobile_No { get; set; }

        public string P_EMPLOYEE_ID { get; set; }
        public string P_CVM_FLAG { get; set; }

        private string _sessionId;
        public string SessionId
        {
            get { return _sessionId; }
            set { _sessionId = value; }
        }
    }
}
