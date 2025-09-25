using System;
using System.Collections.Generic;

namespace WBBContract.Queries.PatchEquipment
{
    /// <summary>
    /// get data form database payg
    /// </summary>
    public class PatchEquipmentQuery : IQuery<List<RetPatchEquipment>>
    {
        public string CreateDateFrom { get; set; }
        public string CreateDateTo { get; set; }
        public string PatchStatus { get; set; }
        public string FileName { get; set; }
        public string SerialNo { get; set; }
        public string SerialStatus { get; set; }
        public string InternetNo { get; set; }
        public string Flag { get; set; }
    }

    public class RetPatchEquipment
    {
        public int NO { get; set; }
        public string FILE_NAME { get; set; }
        public string SERIAL_NO { get; set; }
        public string INTERNET_NO { get; set; }
        public string SERIAL_STATUS { get; set; }
        public string STORAGE_LOCATION { get; set; }
        public string SUBMIT_DATE { get; set; } //format dd/MM/yyyy
        public string _SUBMIT_DATE
        {
            get
            {
                return SUBMIT_DATE != null ? $"{SUBMIT_DATE.Replace("/", "")}" : ""; //format ddMMyyyy
            }
        }
        public string POSTING_DATE { get; set; }
        public string _POSTING_DATE
        {
            get
            {
                return POSTING_DATE != null ? $"{POSTING_DATE.Replace("/", "")}" : ""; //format ddMMyyyy
            }
        }
        public string POST_DATE { get; set; }
        public string _POST_DATE
        {
            get
            {
                return POST_DATE != null ? $"{POST_DATE.Replace("/", "")}" : "";
            }
        }
        public string PATCH_STATUS { get; set; }
        public string PATCH_STATUS_DESC { get; set; }
        public string CREATE_DATE { get; set; } //dd/mm/yyyy
        public string CREATED_DATE { get; set; }
        public string _CREATE_DATE
        {
            get
            {
                return CREATE_DATE != null ? $"{CREATE_DATE.Replace("/", "")}" : ""; //ddmmyyyy
            }
        }
        public string CREATE_BY { get; set; }
        public string UPDATE_DATE { get; set; }
        public string UPDATE_BY { get; set; }
        public string MOVEMENT_TYPE { get; set; }
        public string FOA_CODE { get; set; }
        public string LOCATION_CODE { get; set; }
        public string REMARK { get; set; }
    }

    /// <summary>
    /// get data form database shareplex
    /// </summary>
    public class CheckSerialStatusQuery : IQuery<List<RetCheckSerialStatus>>
    {
        public List<CheckSerialStatus> checkSerials { get; set; }
    }

    public class CheckSerialStatusHVRQuery : IQuery<List<RetCheckSerialStatus>>
    {
        public List<CheckSerialStatus> checkSerials { get; set; }
    }

    public class CheckSerialStatus
    {
        public string SERIAL_NUMBER { get; set; }
        public string INTERNET_NO { get; set; }
        public string SERIAL_STATUS { get; set; }
        public string FOA_CODE { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string CREATE_DATE { get; set; }
        public DateTime POSTING_DATE { get; set; }
        public string POST_DATE { get; set; }
        public string MOVEMENT_TYPE { get; set; }
        public string LOCATION_CODE { get; set; }
    }

    public class RetCheckSerialStatus
    {
        public string SN { get; set; }
        public string INTERNET_NO { get; set; }
        public string STATUS { get; set; }
        public string FOA_CODE { get; set; }
        public string CREATED_DATE { get; set; }
        public string POSTING_DATE { get; set; }
        public string MOVEMENT_TYPE { get; set; }
        public string LOCATION_CODE { get; set; }
        public string REMARK { get; set; }
    }


    public class GetProductListSharePlexQuery : IQuery<List<ProductListSharePlex>>
    {
        public string INTERNET_NO { get; set; }
        public string SERIAL_NO { get; set; }
        public string MOVEMENT_TYPE { get; set; }
    }
    public class GetProductListHVRQuery : IQuery<List<ProductListHVR>>
    {
        public string INTERNET_NO { get; set; }
        public string SERIAL_NO { get; set; }
        public string MOVEMENT_TYPE { get; set; }
    }

    public class ProductListSharePlex
    {
        public string INTERNET_NO { get; set; }
        public string PRODUCT_NAME { get; set; }
        public string SERIAL_NO { get; set; }
        public string MATERIAL_CODE { get; set; }
        public string COM_CODE { get; set; }
        public string PLANT { get; set; }
        public string STORAGE_LOCATION { get; set; }
        public string SN_PATTERN { get; set; }
        public string MOVEMENT_TYPE { get; set; }
        public string REJECT_REASON { get; set; }
    }

    public class ProductListHVR
    {
        public string INTERNET_NO { get; set; }
        public string PRODUCT_NAME { get; set; }
        public string SERIAL_NO { get; set; }
        public string MATERIAL_CODE { get; set; }
        public string COM_CODE { get; set; }
        public string PLANT { get; set; }
        public string STORAGE_LOCATION { get; set; }
        public string SN_PATTERN { get; set; }
        public string MOVEMENT_TYPE { get; set; }
        public string REJECT_REASON { get; set; }
    }



    public class GetDataPatchSNsendmailQuery : IQuery<string>
    {
        public string FileName { get; set; }
    }

    public class GetDataPatchSNsendmailResult
    {
        public string MailTo { get; set; }
    }

    public class RetGetDataPatchSNsendmail
    {
        public string FILE_NAME { get; set; }
        public string MAIL_TO { get; set; }
        public string REMARK { get; set; }
        public DateTime? CREATE_DATE { get; set; }
        public string CREATE_BY { get; set; }
        public DateTime? UPDATE_DATE { get; set; }
        public string UPDATE_BY { get; set; }
    }


    /// <summary>
    /// Update CPE
    /// </summary>
    public class UpdateCPE : IQuery<UpdateCPEResponse>
    {
        public string ACTION { get; set; }
        public CPEList CPE_LIST { get; set; }
    }

    public class CPEList
    {
        public string SERIAL_NO { get; set; }
        public string STATUS { get; set; }
        public string SN_PATTERN { get; set; }
        public string LOCATION { get; set; }
        public string PLANT { get; set; }
        public string ACCESS_NO { get; set; }
        public string SERVICE_NAME { get; set; }
        public string DEVICE_TYPE { get; set; }
        public string DESCRIPTION { get; set; }
        public string MATERIAL { get; set; }
        public string CREATE_BY { get; set; }
        public string CREATE_DATE { get; set; }
    }

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false)]
    public partial class Envelope
    {

        private object headerField;

        private EnvelopeBody bodyField;

        /// <remarks/>
        public object Header
        {
            get
            {
                return this.headerField;
            }
            set
            {
                this.headerField = value;
            }
        }

        /// <remarks/>
        public EnvelopeBody Body
        {
            get
            {
                return this.bodyField;
            }
            set
            {
                this.bodyField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public partial class EnvelopeBody
    {

        private UpdateCPEResponse updateCPEResponseField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://oss.zsmart.ztesoft.com/om/webservice/types/")]
        public UpdateCPEResponse UpdateCPEResponse
        {
            get
            {
                return this.updateCPEResponseField;
            }
            set
            {
                this.updateCPEResponseField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://oss.zsmart.ztesoft.com/om/webservice/types/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://oss.zsmart.ztesoft.com/om/webservice/types/", IsNullable = false)]
    public partial class UpdateCPEResponse
    {

        private string cASECODEField;

        private RESULT_LIST rESULT_LISTField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "")]
        public string CASECODE
        {
            get
            {
                return this.cASECODEField;
            }
            set
            {
                this.cASECODEField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "")]
        public RESULT_LIST RESULT_LIST
        {
            get
            {
                return this.rESULT_LISTField;
            }
            set
            {
                this.rESULT_LISTField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class RESULT_LIST
    {

        private string sERIAL_NOField;

        private string oPERATION_RESULTField;

        private string rESULT_DESCRIPTIONField;

        /// <remarks/>
        public string SERIAL_NO
        {
            get
            {
                return this.sERIAL_NOField;
            }
            set
            {
                this.sERIAL_NOField = value;
            }
        }

        /// <remarks/>
        public string OPERATION_RESULT
        {
            get
            {
                return this.oPERATION_RESULTField;
            }
            set
            {
                this.oPERATION_RESULTField = value;
            }
        }

        /// <remarks/>
        public string RESULT_DESCRIPTION
        {
            get
            {
                return this.rESULT_DESCRIPTIONField;
            }
            set
            {
                this.rESULT_DESCRIPTIONField = value;
            }
        }
    }




}
