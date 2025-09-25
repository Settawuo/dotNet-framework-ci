using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    public class ResendWssaps4Model
    {
        public string ACCESS_NUMBER { get; set; }
        public string IN_XML_FOA { get; set; }
        public string RESEND_STATUS { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
        public string UPDATED_DESC { get; set; }
    }

    public class mappingResendWssaps4Model
    {
        public string COL1 { get; set; }
        public string COL2 { get; set; }

    }


    public class mappingResendWssapslist4Model
    {
        public List<mappingResendWssaps4Model> mappinglistPlant { get; set; }
        public List<mappingResendWssaps4Model> mappinglistStorageLocation { get; set; }
        public List<mappingResendWssaps4Model> mappinglistMaterialCode { get; set; }
        public string ret_code { get; set; }
        public string ret_msg { get; set; }

    }



    public class ResendWssaps4Models
    {
        public string ACCESS_NUMBER { get; set; }
        public string IN_XML_FOA { get; set; }
        public string RESEND_STATUS { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
    }

    [XmlRoot("NewRegistForSubmitFOAQuery")]
    public class NewRegistFOAResendLogResendWssaps4
    {

        [XmlElement("Access_No")]
        public string Access_No { get; set; }


        [XmlElement("BUILDING_NAME")]
        public string BUILDING_NAME { get; set; }


        [XmlElement("FOA_Submit_date")]
        public string FOA_Submit_date { get; set; }


        [XmlElement("Mobile_Contact")]
        public string Mobile_Contact { get; set; }


        [XmlElement("OLT_NAME")]
        public string OLT_NAME { get; set; }

        [XmlElement("OrderNumber")]
        public string OrderNumber { get; set; }

        [XmlElement("OrderType")]
        public string OrderType { get; set; }



        //[XmlArray("ProductList")]
        //[XmlArrayItem("ProductList", typeof(NewRegistFOAResendLogProductList))]
        //[XmlAttribute("ProductList")]
        [XmlElement("ProductList")]
        public List<ProductList> ProductList { get; set; }


        [XmlElement("ProductName")]
        public string ProductName { get; set; }


        [XmlElement("RejectReason")]
        public string RejectReason { get; set; }


        [XmlElement("ServiceList")]
        public List<ServiceList> ServiceList { get; set; }


        [XmlElement("SubcontractorCode")]
        public string SubcontractorCode { get; set; }


        [XmlElement("SubcontractorName")]
        public string SubcontractorName { get; set; }


        [XmlElement("SubmitFlag")]
        public string SubmitFlag { get; set; }


        [XmlElement("Post_Date")]
        public string Post_Date { get; set; }


        [XmlElement("Address_ID")]
        public string Address_ID { get; set; }


        [XmlElement("ORG_ID")]
        public string ORG_ID { get; set; }


        [XmlElement("Reuse_Flag")]
        public string Reuse_Flag { get; set; }


        [XmlElement("Event_Flow_Flag")]
        public string Event_Flow_Flag { get; set; }
        //add new 18.06.28


        [XmlElement("UserName")]
        public string UserName { get; set; }
        //add new 31.07.2018


        [XmlElement("Subcontract_Type")]
        public string Subcontract_Type { get; set; }


        [XmlElement("Subcontract_Sub_Type")]
        public string Subcontract_Sub_Type { get; set; }


        [XmlElement("Request_Sub_Flag")]
        public string Request_Sub_Flag { get; set; }

        [XmlElement("Product_Owner")]
        public string Product_Owner { get; set; }

        [XmlElement("Main_Promo_Code")]
        public string Main_Promo_Code { get; set; }

        [XmlElement("Team_ID")]
        public string Team_ID { get; set; }


        [XmlElement("Sub_Access_Mode")]
        public string Sub_Access_Mode { get; set; }


        [XmlElement("ReturnMessage")]
        public string ReturnMessage { get; set; }
    }
    public class ProductListResendWssaps4
    {
        [XmlElement("NewRegistFOAProductList", typeof(NewRegistFOAResendLogProductList))] // no XmlArray/XmlArrayItem, just XmlElement
        public List<NewRegistFOAResendLogProductList> NewRegistFOAProductList { get; set; }
    }

    public class ServiceListResendWssaps4
    {

        [XmlElement("NewRegistFOAServiceList", typeof(NewRegistFOAResendLogServiceList))] // no XmlArray/XmlArrayItem, just XmlElement
        public List<NewRegistFOAResendLogServiceList> NewRegistFOAServiceList { get; set; }
    }

    [Serializable]
    public class NewRegistFOAResendLogServiceListResendWssaps4
    {
        [XmlElement("ServiceName")]
        public string ServiceName { get; set; }
    }

    [Serializable]
    public class NewRegistFOAResendLogProductListResendWssaps4
    {
        [XmlElement("SerialNumber")]
        public string SerialNumber { get; set; }


        [XmlElement("MaterialCode")]
        public string MaterialCode { get; set; }


        [XmlElement("CompanyCode")]
        public string CompanyCode { get; set; }


        [XmlElement("Plant")]
        public string Plant { get; set; }


        [XmlElement("StorageLocation")]
        public string StorageLocation { get; set; }


        [XmlElement("SNPattern")]
        public string SNPattern { get; set; }


        [XmlElement("MovementType")]
        public string MovementType { get; set; }
    }

}
