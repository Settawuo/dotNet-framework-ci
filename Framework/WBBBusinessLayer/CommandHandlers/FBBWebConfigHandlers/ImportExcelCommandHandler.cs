using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBEntity.PanelModels.StoredProc;
using WBBEntity.Extensions;
using WBBData.Repository;
using WBBBusinessLayer.Extension;
using WBBEntity.PanelModels;
using Oracle.DataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using WBBEntity.PanelModels.FBBWebConfigModels;
using WBBEntity.Models;
using WBBData.DbIteration;
using System.Globalization;

namespace WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers
{
    public class ImportExcelCommandHandler  : ICommandHandler<ImaportExcelCommand>
    {    
    
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_HISTORY_LOG> _historyLog;
        private readonly IWBBUnitOfWork _uow;

        public ImportExcelCommandHandler(ILogger logger, IEntityRepository<string> objService, IEntityRepository<FBB_HISTORY_LOG> historyLog, IWBBUnitOfWork uow)
        {
            _logger = logger;
            _objService = objService;
            _historyLog = historyLog;
            _uow = uow;
        }
                    
        public void Handle(ImaportExcelCommand command)
        {
            try
            {
                var apmodel = new APObjectModel();
                string[] formats= { "dd/MM/yyyy" };               
                apmodel.REC_CUST_CONTACT = command.Imex.Select(c => new Rec_Cust_APOracleTypeMapping
                {
                    basel2 = c.Base_L2.ToSafeString(),
                    sitename = c.Site_Name.ToSafeString(),
                    ap_name = c.AP_Name.ToSafeString(),
                    sector = c.Sector.ToSafeString(),
                    province = c.Province.ToSafeString(),
                    sub_district = c.Tumbon.ToSafeString(),
                    district = c.Aumphur.ToSafeString(),
                    region = c.Zone.ToSafeString(),
                    lat = c.Lat.ToSafeString(),
                    lng = c.Lon.ToSafeString(),
                    tower_type = c.Tower_Type.ToSafeString(),
                    tower_height = c.Tower_Height.ToSafeString(),//decimal.Parse(c.Tower_Height.ToSafeString()),
                    vlan = c.VLAN.ToSafeString(),
                    subnet_mask_26 = c.Subnet_Mask_26.ToSafeString(),
                    gateway = c.Gateway.ToSafeString(),
                    ap_comment = c.Comment.ToSafeString(),
                    ip_address = c.IP_Address.ToSafeString(),
                    status = c.Status.ToSafeString(),
                    implement_phase = c.Implement_Phase.ToSafeString(),                    
                    implement_date = string.IsNullOrEmpty(c.Implement_date.ToSafeString()) 
           ? (DateTime?) null 
           : DateTime.ParseExact(c.Implement_date.ToSafeString(), formats, new CultureInfo("en-US"), DateTimeStyles.None),//DateTime.ParseExact(c.Implement_date.ToSafeString(), formats, new CultureInfo("en-US"), DateTimeStyles.None),//DateTime.Parse(c.Implement_date.ToSafeString(), culture, System.Globalization.DateTimeStyles.AssumeLocal),
                    on_service_date = string.IsNullOrEmpty(c.Implement_date.ToSafeString()) 
           ? (DateTime?) null
           : DateTime.ParseExact(c.Onservice_date.ToSafeString(), formats, new CultureInfo("en-US"), DateTimeStyles.None),//DateTime.ParseExact(c.Onservice_date.ToSafeString(), formats, new CultureInfo("en-US"), DateTimeStyles.None),//DateTime.Parse(c.Onservice_date.ToSafeString(), culture, System.Globalization.DateTimeStyles.AssumeLocal),
                    po_number = c.PO_Number.ToSafeString(),
                    ap_company = c.AP_Company.ToSafeString(),
                    ap_lot = c.AP_Lot.ToSafeString(),
                   
                }).ToArray();

                //var ret_code = new OracleParameter();
                //ret_code.OracleDbType = OracleDbType.Decimal;
                //ret_code.Direction = ParameterDirection.Output;

                //var v_error_msg = new OracleParameter();
                //v_error_msg.OracleDbType = OracleDbType.Varchar2;
                //v_error_msg.Size = 2000;
                //v_error_msg.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var contact = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_REC_CFG_AP", "FBB_CFG_AP_ARRAY", apmodel);

                var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FBBCFG006.PROC_MAIN_FBBCFG006",

                    out paramOut,
                       new
                       {

                           p_file_name = command.filename,
                           p_user = command.user,
                           p_REC_CFG_AP = contact,
                           //return code
                           //Return_code = ret_code,
                           //Return_msg = v_error_msg

                       });

                //command.Return_Code = ret_code.Value != null ? Convert.ToInt32(ret_code.Value.ToSafeString()) : -1;
                //command.Return_Desc = v_error_msg.Value.ToSafeString();
                #region Add FBB_HISTORY_LOG
                var historyLogItem2 = new FBB_HISTORY_LOG();
                historyLogItem2.ACTION = ActionHistory.ADD.ToString();
                historyLogItem2.APPLICATION = "FBB_CFG006_1";
                historyLogItem2.CREATED_BY = command.user;
                historyLogItem2.CREATED_DATE = DateTime.Now;
                historyLogItem2.DESCRIPTION = "FileName: " + command.filename + ", " + "ImportDate: " + String.Format("{0:dd/MM/yyyy HH:mm:ss}", DateTime.Now); 
                historyLogItem2.REF_KEY = command.filename;
                historyLogItem2.REF_NAME = "FileName";
                _historyLog.Create(historyLogItem2);
                _uow.Persist();
                #endregion
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                command.Return_Code = -1;
                command.Return_Desc = "Error call service " + ex.GetErrorMessage();
            }  
        }
    }

    #region Mapping Rec_Cust_Contact Type Oracle
    public class APObjectModel : Oracle.DataAccess.Types.INullable, IOracleCustomType
    {
        [OracleArrayMapping()]
        public Rec_Cust_APOracleTypeMapping[] REC_CUST_CONTACT { get; set; }

        private bool objectIsNull;

        public bool IsNull
        {
            get { return objectIsNull; }
        }

        public static APObjectModel Null
        {
            get
            {
                APObjectModel obj = new APObjectModel();
                obj.objectIsNull = true;
                return obj;
            }
        }

        public void FromCustomObject(OracleConnection con, IntPtr pUdt)
        {
            OracleUdt.SetValue(con, pUdt, 0, REC_CUST_CONTACT);
        }

        public void ToCustomObject(OracleConnection con, IntPtr pUdt)
        {
            REC_CUST_CONTACT = (Rec_Cust_APOracleTypeMapping[])OracleUdt.GetValue(con, pUdt, 0);
        }
    }
    [OracleCustomTypeMappingAttribute("FBB_CFG_AP_RECORD")]
    public class Rec_Cust_ContactOracleTypeMappingFactory : IOracleCustomTypeFactory
    {
        public IOracleCustomType CreateObject()
        {
            return new Rec_Cust_APOracleTypeMapping();
        }
    }
    [OracleCustomTypeMapping("FBB_CFG_AP_ARRAY")]
    public class ContactObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
    {
        #region IOracleCustomTypeFactory Members
        public IOracleCustomType CreateObject()
        {
            return new APObjectModel();
        }

        #endregion

        #region IOracleArrayTypeFactory Members
        public Array CreateArray(int numElems)
        {
            return new Rec_Cust_APOracleTypeMapping[numElems];
        }

        public Array CreateStatusArray(int numElems)
        {
            return null;
        }

        #endregion
    }

    public class Rec_Cust_APOracleTypeMapping : Oracle.DataAccess.Types.INullable, IOracleCustomType
    {
        private bool objectIsNull;

        #region Attribute Mapping
        [OracleObjectMappingAttribute("BASEL2")]
        public string basel2 { get; set; }

        [OracleObjectMappingAttribute("SITENAME")]
        public string sitename { get; set; }

        [OracleObjectMappingAttribute("SUB_DISTRICT")]
        public string sub_district { get; set; }

        [OracleObjectMappingAttribute("DISTRICT")]
        public string district { get; set; }

        [OracleObjectMappingAttribute("PROVINCE")]
        public string province { get; set; }

        [OracleObjectMappingAttribute("REGION")]
        public string region { get; set; }

        [OracleObjectMappingAttribute("LAT")]
        public string lat { get; set; }

        [OracleObjectMappingAttribute("LNG")]
        public string lng { get; set; }

        [OracleObjectMappingAttribute("TOWER_TYPE")]
        public string tower_type { get; set; }

        [OracleObjectMappingAttribute("TOWER_HEIGHT")]
        public string tower_height { get; set; }

        [OracleObjectMappingAttribute("VLAN")]
        public string vlan { get; set; }

        [OracleObjectMappingAttribute("SUBNET_MASK_26")]
        public string subnet_mask_26 { get; set; }

        [OracleObjectMappingAttribute("GATEWAY")]
        public string gateway { get; set; }

        [OracleObjectMappingAttribute("AP_COMMENT")]
        public string ap_comment { get; set; }

        /// <summary>
        /// ////////////
        /// </summary>

        [OracleObjectMappingAttribute("AP_NAME")]
        public string ap_name { get; set; }

        [OracleObjectMappingAttribute("SECTOR")]
        public string sector { get; set; }

        [OracleObjectMappingAttribute("IP_ADDRESS")]
        public string ip_address { get; set; }

        [OracleObjectMappingAttribute("STATUS")]
        public string status { get; set; }

        [OracleObjectMappingAttribute("IMPLEMENT_PHASE")]
        public string implement_phase { get; set; }

        [OracleObjectMappingAttribute("IMPLEMENT_DATE")]
        public DateTime? implement_date { get; set; }

        [OracleObjectMappingAttribute("ON_SERVICE_DATE")]
        public DateTime? on_service_date { get; set; }

        [OracleObjectMappingAttribute("PO_NUMBER")]
        public string po_number { get; set; }

        [OracleObjectMappingAttribute("AP_COMPANY")]
        public string ap_company { get; set; }

        [OracleObjectMappingAttribute("AP_LOT")]
        public string ap_lot { get; set; }
        #endregion

        public static Rec_Cust_APOracleTypeMapping Null
        {
            get
            {
                Rec_Cust_APOracleTypeMapping obj = new Rec_Cust_APOracleTypeMapping();
                obj.objectIsNull = true;
                return obj;
            }
        }

        public bool IsNull
        {
            get { return objectIsNull; }
        }

        public void FromCustomObject(Oracle.DataAccess.Client.OracleConnection con, IntPtr pUdt)
        {
            OracleUdt.SetValue(con, pUdt, "BASEL2", basel2);
            OracleUdt.SetValue(con, pUdt, "SITENAME", sitename);
            OracleUdt.SetValue(con, pUdt, "SUB_DISTRICT", sub_district);
            OracleUdt.SetValue(con, pUdt, "DISTRICT", district);
            OracleUdt.SetValue(con, pUdt, "PROVINCE", province);
            OracleUdt.SetValue(con, pUdt, "REGION", region);
            OracleUdt.SetValue(con, pUdt, "LAT", lat);
            OracleUdt.SetValue(con, pUdt, "LNG", lng);
            OracleUdt.SetValue(con, pUdt, "TOWER_TYPE", tower_type);
            OracleUdt.SetValue(con, pUdt, "TOWER_HEIGHT", tower_height);
            OracleUdt.SetValue(con, pUdt, "VLAN", vlan);
            OracleUdt.SetValue(con, pUdt, "SUBNET_MASK_26", subnet_mask_26);
            OracleUdt.SetValue(con, pUdt, "GATEWAY", gateway);
            OracleUdt.SetValue(con, pUdt, "AP_COMMENT", ap_comment);

            OracleUdt.SetValue(con, pUdt, "AP_NAME", ap_name);
            OracleUdt.SetValue(con, pUdt, "SECTOR", sector);
            OracleUdt.SetValue(con, pUdt, "IP_ADDRESS", ip_address);
            OracleUdt.SetValue(con, pUdt, "STATUS", status);
            OracleUdt.SetValue(con, pUdt, "IMPLEMENT_PHASE", implement_phase);
            OracleUdt.SetValue(con, pUdt, "IMPLEMENT_DATE", implement_date);
            OracleUdt.SetValue(con, pUdt, "ON_SERVICE_DATE", on_service_date);
            OracleUdt.SetValue(con, pUdt, "PO_NUMBER", po_number);
            OracleUdt.SetValue(con, pUdt, "AP_COMPANY", ap_company);
            OracleUdt.SetValue(con, pUdt, "AP_LOT", ap_lot);           
        }

        public void ToCustomObject(Oracle.DataAccess.Client.OracleConnection con, IntPtr pUdt)
        {
            throw new NotImplementedException();
        }
    }
    #endregion
}


