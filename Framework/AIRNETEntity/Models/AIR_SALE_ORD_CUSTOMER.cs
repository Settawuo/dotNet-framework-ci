using System;

namespace AIRNETEntity.Models
{
    public class AIR_SALE_ORD_CUSTOMER
    {
        public string ORDER_NO { get; set; }
        public string CUSTOMER_ID { get; set; }
        public string TITLE_CODE { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string CUSTOMER_TYPE { get; set; }
        public string CUSTOMER_SUBTYPE { get; set; }
        public string TAX_ID { get; set; }
        public string STATUS { get; set; }
        public string ID_CARD_TYPE { get; set; }
        public string ID_CARD_NO { get; set; }
        public string REGISTRATION_NO { get; set; }
        public DateTime? ID_EXPIRED_DATE { get; set; }
        public string GENDER { get; set; }
        public string NATIONALITY_CODE { get; set; }
        public DateTime? BIRTH_DATE { get; set; }
        public string EDUCATION_CODE { get; set; }
        public string OCCUPATION_CODE { get; set; }
        public string SALARY_CODE { get; set; }
        public string EMAIL_ADDRESS { get; set; }
        public string WEBSITE { get; set; }
        public string BUSINESS_TYPE_CODE { get; set; }
        public DateTime UPD_DTM { get; set; }
        public string UPD_BY { get; set; }
        public string CA_BRANCH_NO { get; set; }
    }
}
