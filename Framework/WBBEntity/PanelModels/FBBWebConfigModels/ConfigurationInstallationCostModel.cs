using System;
using System.Collections.Generic;
namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    public class ConfigurationCostInstallationView
    {
        public List<CostInstallationtable1> CostInstallationtable1 { get; set; }
        public List<CostInstallationtable2> CostInstallationtable2 { get; set; }
        public List<CostInstallationtable3> CostInstallationtable3 { get; set; }
        public List<CostInstallationtable4> CostInstallationtable4 { get; set; }
        public List<CostInstallationtable5> CostInstallationtable5 { get; set; }
        public List<CostInstallationtable6> CostInstallationtable6 { get; set; }
    }

    public class ConfigCostResponse
    {
        public ConfigCostResponse()
        {
            if (curt1 == null)
            {
                curt1 = new List<CostInstallationtable1>();
            }

            if (curt2 == null)
            {
                curt2 = new List<CostInstallationtable2>();
            }
            if (curt3 == null)
            {
                curt3 = new List<CostInstallationtable3>();
            }
            if (curt4 == null)
            {
                curt4 = new List<CostInstallationtable4>();
            }
            if (curt5 == null)
            {
                curt5 = new List<CostInstallationtable5>();
            }
            if (curt6 == null)
            {
                curt6 = new List<CostInstallationtable6>();
            }


        }

        public string ret_code { get; set; }
        public string ret_msg { get; set; }
        public List<CostInstallationtable1> curt1 { get; set; }
        public List<CostInstallationtable2> curt2 { get; set; }
        public List<CostInstallationtable3> curt3 { get; set; }
        public List<CostInstallationtable4> curt4 { get; set; }
        public List<CostInstallationtable5> curt5 { get; set; }
        public List<CostInstallationtable6> curt6 { get; set; }

    }

    public class CostInstallationtable
    {

        public string TB_NAME { get; set; }
        public string SUBCONTTYPE { get; set; }
        public string SUBCONTRACT_LOCATION { get; set; }
        public string COMPANY_NAME { get; set; }
        public string RULE_ID { get; set; }

        public string ORD_TYPE { get; set; }
        public string TECH_TYPE { get; set; }
        public string DATE_FROM { get; set; }
        public string DATE_TO { get; set; }
        public string EXPDATE_FROM { get; set; }
        public string EXPDATE_TO { get; set; }
        public string PAGE_INDEX { get; set; }
        public string PAGE_SIZE { get; set; }

    }
    public class CostInstallationData
    {
        public string COMMAND { get; set; }
        public string TABLE { get; set; }
        public string RULD_ID { get; set; }
        public string RULE_NAME { get; set; }
        public string ORDER_TYPE { get; set; }
        public string SUBCONTRACT_TYPE { get; set; }
        public string SUBCONTRACT_LOCATION { get; set; }
        public string COMPANY_NAME { get; set; }
        public string[] SUBCONTRACT_SUB_TYPE { get; set; }
        public string VENDOR_CODE { get; set; }
        public string TECHNOLOGY { get; set; }
        public string TOTAL_PRICE { get; set; }
        public string EVENT_CODE { get; set; }
        public string ROOM_FLAG { get; set; }
        public string REUSE_FLAG { get; set; }
        public string DISTANCE_FROM { get; set; }
        public string DISTANCE_TO { get; set; }
        public string INDOOR_PRICE { get; set; }
        public string OUTDOOR_PRICE { get; set; }

        public string INTERNET_PRICE { get; set; }
        public string VOIP_PRICE { get; set; }
        public string PLAYBOX_PRICE { get; set; }
        public string MECH_PRICE { get; set; }
        public string ADDRESS_ID { get; set; }
        public string EVENT_TYPE { get; set; }
        public string EFFECTIVE_DATE { get; set; }
        public string EXPIRE_DATE { get; set; }
        public string SAME_DAY { get; set; }




    }
    public class CostInstallationtable1
    {
        public string RULEID { get; set; }
        public string RULE_NAME { get; set; }
        public string ORDER_TYPE { get; set; }
        public string SUBCONTRACT_TYPE { get; set; }
        public string SUBCONTRACT_SUB_TYPE { get; set; }
        public string VENDOR_CODE { get; set; }
        public string TECHNOLOGY { get; set; }
        // public float? TOTAL_PRICE { get; set; }
        public string TOTAL_PRICE { get; set; }
        public DateTime EFFECTIVE_DATE { get; set; }
        public string EFFECTIVE_DATE_TEXT { get; set; }
        public DateTime EXPIRE_DATE { get; set; }
        public string EXPIRE_DATE_TEXT { get; set; }
        public DateTime CREATE_DATE { get; set; }
        public string CREATE_DATE_TEXT { get; set; }
        public string CREATE_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
        public string UPDATED_DATE_TEXT { get; set; }
        public string UPDATED_BY { get; set; }
        public decimal RowNumber { get; set; }
        public decimal CNT { get; set; }
    }
    public class CostInstallationtable2
    {
        public string RULEID { get; set; }
        public string RULE_NAME { get; set; }
        public string ORDER_TYPE { get; set; }
        public string SUBCONTRACT_TYPE { get; set; }
        public string TECHNOLOGY { get; set; }
        public string INTERNET_PRICE { get; set; }
        public string VOIP_PRICE { get; set; }
        public string PLAYBOX_PRICE { get; set; }
        public string ADDRESS_ID { get; set; }
        public string MESH_PRICE { get; set; }
        public string EVENT_TYPE { get; set; }
        public DateTime EFFECTIVE_DATE { get; set; }
        public string EFFECTIVE_DATE_TEXT { get; set; }
        public DateTime EXPIRE_DATE { get; set; }
        public string EXPIRE_DATE_TEXT { get; set; }
        public DateTime CREATE_DATE { get; set; }
        public string CREATE_DATE_TEXT { get; set; }
        public string CREATE_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
        public string UPDATED_DATE_TEXT { get; set; }
        public string UPDATED_BY { get; set; }
        public string SAME_DAY { get; set; }

        public decimal RowNumber { get; set; }
        public decimal CNT { get; set; }
    }
    public class CostInstallationtable3
    {
        public string RULEID { get; set; }
        public string RULE_NAME { get; set; }
        public string ORDER_TYPE { get; set; }
        public string SUBCONTRACT_TYPE { get; set; }
        public string TECHNOLOGY { get; set; }
        public string EVENT_CODE { get; set; }
        public string ROOM_FLAG { get; set; }
        public string SAME_DAY { get; set; }
        //   public string REUSE_FLAG { get; set; }
        public string INTERNET_PRICE { get; set; }
        public string VOIP_PRICE { get; set; }
        public string PLAYBOX_PRICE { get; set; }
        public string MESH_PRICE { get; set; }
        public DateTime EFFECTIVE_DATE { get; set; }
        public string EFFECTIVE_DATE_TEXT { get; set; }
        public DateTime EXPIRE_DATE { get; set; }
        public string EXPIRE_DATE_TEXT { get; set; }
        public DateTime CREATE_DATE { get; set; }
        public string CREATE_DATE_TEXT { get; set; }
        public string CREATE_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
        public string UPDATED_DATE_TEXT { get; set; }
        public string UPDATED_BY { get; set; }

        public decimal RowNumber { get; set; }
        public decimal CNT { get; set; }
    }
    public class CostInstallationtable4
    {
        public string RULEID { get; set; }
        public string RULE_NAME { get; set; }
        public string SUBCONTRACT_TYPE { get; set; }
        public string SUBCONTRACT_LOCATION { get; set; }
        public string TECHNOLOGY { get; set; }
        public string ORDER_TYPE { get; set; }
        public string REUSE_FLAG { get; set; }
        public string DISTANCE_FROM { get; set; }
        public string DISTANCE_TO { get; set; }
        public string INDOOR_PRICE { get; set; }
        public string OUTDOOR_PRICE { get; set; }
        public string TOTAL_PRICE { get; set; }
        public DateTime EFFECTIVE_DATE { get; set; }
        public string EFFECTIVE_DATE_TEXT { get; set; }
        public DateTime EXPIRE_DATE { get; set; }
        public string EXPIRE_DATE_TEXT { get; set; }
        public DateTime CREATE_DATE { get; set; }
        public string CREATE_DATE_TEXT { get; set; }
        public string CREATE_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
        public string UPDATED_DATE_TEXT { get; set; }
        public string UPDATED_BY { get; set; }
        public string SAME_DAY { get; set; }
        public string COMPANY_NAME { get; set; }
        public decimal RowNumber { get; set; }
        public decimal CNT { get; set; }
    }
    public class CostInstallationtable5
    {
        public string RULEID { get; set; }
        public string RULE_NAME { get; set; }
        public string ORDER_TYPE { get; set; }
        public string SUBCONTRACT_TYPE { get; set; }
        public string SUBCONTRACT_SUB_TYPE { get; set; }
        public string TECHNOLOGY { get; set; }
        public string REUSE_FLAG { get; set; }
        public string TOTAL_PRICE { get; set; }
        public DateTime EFFECTIVE_DATE { get; set; }
        public string EFFECTIVE_DATE_TEXT { get; set; }
        public DateTime EXPIRE_DATE { get; set; }
        public string EXPIRE_DATE_TEXT { get; set; }
        public DateTime CREATE_DATE { get; set; }
        public string CREATE_DATE_TEXT { get; set; }
        public string CREATE_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
        public string UPDATED_DATE_TEXT { get; set; }
        public string UPDATED_BY { get; set; }
        public string SAME_DAY { get; set; }
        //public string SUBCONTRACT_LOCATION { get; set; }

        public decimal RowNumber { get; set; }
        public decimal CNT { get; set; }
    }
    public class CostInstallationtable6
    {
        public string RULEID { get; set; }
        public string RULE_NAME { get; set; }
        public string ORDER_TYPE { get; set; }


        public string TECHNOLOGY { get; set; }
        public string TOTAL_PRICE { get; set; }
        public DateTime EFFECTIVE_DATE { get; set; }
        public string EFFECTIVE_DATE_TEXT { get; set; }
        public DateTime EXPIRE_DATE { get; set; }
        public string EXPIRE_DATE_TEXT { get; set; }
        public DateTime CREATE_DATE { get; set; }
        public string CREATE_DATE_TEXT { get; set; }
        public string CREATE_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
        public string UPDATED_DATE_TEXT { get; set; }
        public string UPDATED_BY { get; set; }
        public string SAME_DAY { get; set; }

        public decimal RowNumber { get; set; }
        public decimal CNT { get; set; }
    }

    public class RULEBYTABLE
    {
        public string RULEID { get; set; }
        public string RULE_NAME { get; set; }
    }
}