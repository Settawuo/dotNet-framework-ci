using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.Text.RegularExpressions;
using WBBBusinessLayer.Extension;
using WBBBusinessLayer.FBSSOrderServices;
using WBBBusinessLayer.QueryHandlers;
using WBBBusinessLayer.QueryHandlers.WebServices.FBSS;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.FBSS;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.CommandHandlers.FBSS
{
    public class CreateOderPreRegisterCommandHandler : ICommandHandler<CreateOderPreRegisterCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;
        private readonly IEntityRepository<string> _seqNBR;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;
        private readonly IQueryHandler<GetTokenFbbQuery, GetTokenFbbModel> _queryProcessor;
        private readonly MemoryCache cache = MemoryCache.Default;
        public CreateOderPreRegisterCommandHandler(ILogger logger, IWBBUnitOfWork uow, IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_CFG_LOV> cfgLov, IEntityRepository<string> seqNBR, IEntityRepository<string> objService, IEntityRepository<FBB_CFG_LOV> lov, IQueryHandler<GetTokenFbbQuery, GetTokenFbbModel> queryProcessor)
        {
            _logger = logger;
            _intfLog = intfLog;
            _uow = uow;
            _cfgLov = cfgLov;
            _seqNBR = seqNBR;
            _objService = objService;
            _lov = lov;
            _queryProcessor = queryProcessor;
        }

        private List<prodAttrInfo> GetProdAttrInfoList(string NonPreMobile, string Subcontract, string PasswordDec)
        {
            List<prodAttrInfo> listProdAttrInfo = new List<prodAttrInfo>();

            try
            {
                for (int i = 1; i <= 9; i++)
                {
                    #region SQL
                    //SELECT LOV_NAME,LOV_VAL1 FROM fbb_cfg_lov 
                    //WHERE LOV_VAL5 = 'FBBDORM008'  AND LOV_TYPE = 'FBBDORM_PARAM_CREATEORDER'  
                    //AND  LOV_VAL4 = 'PROD_ATTR_VALUE_LIST'
                    //AND ORDER_BY =1"	
                    #endregion

                    var resultProdAttrInfo = from c in _cfgLov.Get()
                                             where c.LOV_VAL5 == "FBBDORM008"
                                             && c.LOV_TYPE == "FBBDORM_PARAM_CREATEORDER"
                                             && c.LOV_VAL4 == "PROD_ATTR_VALUE_LIST"
                                             && c.ORDER_BY == i
                                             select c;

                    prodAttrInfo itemProdAttrInfo = new prodAttrInfo();
                    itemProdAttrInfo.ATTR_CODE = resultProdAttrInfo.Any(a => a.LOV_NAME == "ATTR_CODE") ? resultProdAttrInfo.FirstOrDefault(a => a.LOV_NAME == "ATTR_CODE").LOV_VAL1 : "";
                    itemProdAttrInfo.ATTR_NAME = resultProdAttrInfo.Any(a => a.LOV_NAME == "ATTR_NAME") ? resultProdAttrInfo.FirstOrDefault(a => a.LOV_NAME == "ATTR_NAME").LOV_VAL1 : "";
                    if (i == 1)
                    {
                        itemProdAttrInfo.VALUE = NonPreMobile;
                    }

                    else if (i == 7)
                    {
                        itemProdAttrInfo.VALUE = Subcontract;
                    }
                    else
                    {
                        itemProdAttrInfo.VALUE = resultProdAttrInfo.Any(a => a.LOV_NAME == "VALUE") ? resultProdAttrInfo.FirstOrDefault(a => a.LOV_NAME == "VALUE").LOV_VAL1 : ""; ;
                    }
                    itemProdAttrInfo.DESCRIPTION = resultProdAttrInfo.Any(a => a.LOV_NAME == "DESCRIPTION") ? resultProdAttrInfo.FirstOrDefault(a => a.LOV_NAME == "DESCRIPTION").LOV_VAL1 : "";
                    listProdAttrInfo.Add(itemProdAttrInfo);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return listProdAttrInfo;
        }

        private List<productOrder> GetProductOrderList(string addressId, string Address, string appointmentDate, string timeSlot, string NonPreMobile, string Subcontract, string PasswordDec, string Customername, string Phone)
        {
            List<productOrder> listProductOrder = new List<productOrder>();

            var ret_seq = new OracleParameter();
            ret_seq.OracleDbType = OracleDbType.Varchar2;
            ret_seq.Size = 2000;
            ret_seq.Direction = ParameterDirection.Output;

            var outp = new List<object>();
            var paramOut = outp.ToArray();

            var executeResult = _objService.ExecuteStoredProc("WBB.pkg_fbbdorm_fbbdorm008.fbbdorm_sff_seq",
            out paramOut,
                new
                {
                    ret_seq = ret_seq

                });
            try
            {
                #region Get data from lov

                #region sql
                //SELECT LOV_NAME,LOV_VAL1 FROM fbb_cfg_lov 
                //WHERE LOV_VAL5 = 'FBBDORM008' AND LOV_TYPE = 'FBBDORM_PARAM_CREATEORDER' 
                //AND  LOV_VAL4 = 'PRODUCT_ORDER_LIST' AND ORDER_BY = 1
                #endregion

                var resultProductOrder = from c in _cfgLov.Get()
                                         where c.LOV_VAL5 == "FBBDORM008"
                                         && c.LOV_TYPE == "FBBDORM_PARAM_CREATEORDER"
                                         && c.LOV_VAL4 == "PRODUCT_ORDER_LIST"
                                         && c.ORDER_BY == 1
                                         select new { LOV_NAME = c.LOV_NAME, LOV_VAL1 = c.LOV_VAL1 };
                #endregion
                //string[] tempdate = appointmentDate.Split('-');
                //string resultdate = tempdate[2] + "-" + tempdate[1] + "-" + tempdate[0];
                productOrder itemProductOrder = new productOrder();
                itemProductOrder.ORDER_NBR = ret_seq.Value.ToString();
                itemProductOrder.EVENT_CODE = resultProductOrder.Any(a => a.LOV_NAME == "EVENT_CODE") ? resultProductOrder.FirstOrDefault(a => a.LOV_NAME == "EVENT_CODE").LOV_VAL1 : ""; ;
                itemProductOrder.RFS_DATE = DateTime.Now.ToString("yyyy-MM-dd");
                itemProductOrder.LINKMAN_LIST = GetLinkInfoList(Customername, Phone).ToArray();
                itemProductOrder.PRODUCT = GetProductInfo(addressId, NonPreMobile, Address, Subcontract, PasswordDec);
                //itemProductOrder.OLD_PRODUCT = ;
                itemProductOrder.APPOINTMENT_TIME = GetTimeSlotInfo(appointmentDate, timeSlot).ToArray();
                listProductOrder.Add(itemProductOrder);

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return listProductOrder;
        }

        private productInfo GetProductInfo(string addressId, string PreNonmobile, string Address, string Subcontract, string PasswordDec)
        {
            productInfo itemProductInfo = new productInfo();
            PreregisterModel model = new PreregisterModel();
            var aaa = "";
            try
            {
                #region SQL
                //SELECT LOV_NAME,LOV_VAL1 FROM fbb_cfg_lov 
                //WHERE LOV_VAL5 = 'FBBDORM008' 
                //AND LOV_TYPE = 'FBBDORM_PARAM_CREATEORDER' 
                //AND  LOV_VAL4 = 'PRODUCT' 
                //AND ORDER_BY = 1							
                #endregion

                var resultProductInfo = from c in _cfgLov.Get()
                                        where c.LOV_VAL5 == "FBBDORM008"
                                        && c.LOV_TYPE == "FBBDORM_PARAM_CREATEORDER"
                                        && c.LOV_VAL4 == "PRODUCT"
                                        && c.ORDER_BY == 1
                                        select c;


                itemProductInfo.ACC_NBR = PreNonmobile;
                itemProductInfo.ADDRESS = Address;
                itemProductInfo.ADDRESS_ID = addressId;
                itemProductInfo.AREA_CODE = "";
                itemProductInfo.AREA_NAME = "";
                itemProductInfo.PREFIX = "";
                itemProductInfo.PROD_ATTR_LIST = GetProdAttrInfoList(PreNonmobile, Subcontract, PasswordDec).ToArray();
                itemProductInfo.PROD_ID = 14090149;
                itemProductInfo.PROD_IDSpecified = true;
                itemProductInfo.PROD_SPEC_CODE = resultProductInfo.Any(a => a.LOV_NAME == "PROD_SPEC_CODE") ? resultProductInfo.FirstOrDefault(a => a.LOV_NAME == "PROD_SPEC_CODE").LOV_VAL1 : "";
                itemProductInfo.PROD_SPEC_NAME = resultProductInfo.Any(a => a.LOV_NAME == "PROD_SPEC_NAME") ? resultProductInfo.FirstOrDefault(a => a.LOV_NAME == "PROD_SPEC_NAME").LOV_VAL1 : ""; ;
                itemProductInfo.PROD_STATE = resultProductInfo.Any(a => a.LOV_NAME == "PROD_STATE") ? resultProductInfo.FirstOrDefault(a => a.LOV_NAME == "PROD_STATE").LOV_VAL1 : "";
                itemProductInfo.SERVICE_PWD = PasswordDec;

            }

            catch (Exception ex)
            {
                throw ex;
            }

            return itemProductInfo;
        }

        private List<linkInfo> GetLinkInfoList(string Customername, string Phone)
        {
            List<linkInfo> listLinkInfo = new List<linkInfo>();

            try
            {
                linkInfo itemLinkInfo = new FBSSOrderServices.linkInfo();
                itemLinkInfo.CONTACT_MOBILE_PHONE = Phone;
                itemLinkInfo.CONTACT_NAME = Customername;
                itemLinkInfo.CONTACT_TYPE = "1";
                listLinkInfo.Add(itemLinkInfo);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return listLinkInfo;
        }

        private List<timeslotInfo> GetTimeSlotInfo(string appointmentDate, string timeSlot)
        {
            List<timeslotInfo> listTimeSlotInfo = new List<timeslotInfo>();

            try
            {
                string[] tempTime = timeSlot.Split(' ');
                string resultTime = tempTime[0] + tempTime[1] + tempTime[2];
                timeslotInfo itemTimeSlotInfo = new timeslotInfo();
                string[] tempDate = appointmentDate.Split('-');
                string resultDate = tempDate[2] + '-' + tempDate[1] + '-' + tempDate[0];
                itemTimeSlotInfo.APPOINTMENT_DATE = resultDate;
                itemTimeSlotInfo.INSTALLATION_CAPACITY = "4/10";
                itemTimeSlotInfo.TIME_SLOT = resultTime;
                listTimeSlotInfo.Add(itemTimeSlotInfo);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return listTimeSlotInfo;
        }

        private custInfo GetCustInfo(string Cus_ID, string PreNonmobile, string Customername)
        {
            custInfo itemCusinfo = new custInfo();
            try
            {
                #region SQL
                //SELECT LOV_NAME,LOV_VAL1 FROM fbb_cfg_lov 
                //WHERE LOV_VAL5 = 'FBBDORM008' AND LOV_TYPE = 'FBBDORM_PARAM_CREATEORDER' 
                //AND  LOV_VAL4 = 'CUST' AND ORDER_BY = 1
                #endregion

                var returnCustInfo = from c in _cfgLov.Get()
                                     where c.LOV_VAL5 == "FBBDORM008"
                                     && c.LOV_TYPE == "FBBDORM_PARAM_CREATEORDER"
                                     && c.LOV_VAL4 == "CUST"
                                     && c.ORDER_BY == 1
                                     select c;

                string tempCUST_ID = PreNonmobile.Substring(2, 8); //ขึ้นรอบหน้า


                //itemCusinfo.ADDRESS = "";
                //itemCusinfo.CERT_EFF_DATE = "";
                //itemCusinfo.CERT_EXP_DATE = "";
                itemCusinfo.CERT_NBR = Cus_ID;//ขึ้นรอบหน้า
                //itemCusinfo.CERT_TYPE = "";
                itemCusinfo.CREATED_DATE = DateTime.Now.ToString("yyyy-MM-dd");
                itemCusinfo.CUST_ID = long.Parse(tempCUST_ID); //ขึ้นรอบหน้า
                itemCusinfo.CUST_IDSpecified = true;
                itemCusinfo.CUST_NAME = Customername;
                itemCusinfo.CUST_TYPE = returnCustInfo.Any(a => a.LOV_NAME == "CUST_TYPE") ? returnCustInfo.FirstOrDefault(a => a.LOV_NAME == "CUST_TYPE").LOV_VAL1 : "";
                itemCusinfo.EMAIL = "";
                //itemCusinfo.PARENT_ID = "";

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return itemCusinfo;
        }

        public void Handle(CreateOderPreRegisterCommand command2)
        {
            int countException = 0;
            InterfaceLogCommand log = null;
            PreregisterModel Premodel = new PreregisterModel();
            //Start Interface Log



            var ret_seq = new OracleParameter();
            ret_seq.OracleDbType = OracleDbType.Varchar2;
            ret_seq.Size = 2000;
            ret_seq.Direction = ParameterDirection.Output;

            var outp = new List<object>();
            var paramOut = outp.ToArray();
            _logger.Info("Callfbbdorm_sff_seq");
            var executeResult = _objService.ExecuteStoredProc("WBB.pkg_fbbdorm_fbbdorm008.fbbdorm_sff_seq",
            out paramOut,
                new
                {
                    ret_seq = ret_seq

                });
            _logger.Info("Endcallfbbdorm_sff_seq");
            command2.Up_FBBDORM_Order_No = ret_seq.Value.ToString();

            repeat:
            try
            {
                #region R24.10 Call Access Token FBSS
                string accessToken = string.Empty;
                string channel = FBSSAccessToken.channelFBB.ToUpper();
                accessToken = (string)cache.Get(channel); //Get cache

                if (string.IsNullOrEmpty(accessToken))
                {
                    string clientId = string.Empty;
                    string clientSecret = string.Empty;
                    string grantType = string.Empty;
                    var loveConfigList = _cfgLov.Get(lov => lov.ACTIVEFLAG.Equals("Y") && lov.LOV_TYPE.Equals("FBSS_Authen")).ToList();
                    if (loveConfigList != null && loveConfigList.Count() > 0)
                    {
                        clientId = loveConfigList.FirstOrDefault(t => t.LOV_NAME == channel) != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == channel).LOV_VAL1 : "";
                        clientSecret = loveConfigList.FirstOrDefault(t => t.LOV_NAME == channel) != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == channel).LOV_VAL2 : "";
                        grantType = loveConfigList.FirstOrDefault(t => t.LOV_NAME == channel) != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == channel).LOV_VAL4 : "";
                    }

                    var getToken = new GetTokenFbbQuery()
                    {
                        Channel = channel,
                        ParamGetoken = new ParametersGetoken()
                        {
                            client_id = clientId,
                            client_secret = clientSecret,
                            grant_type = grantType
                        }
                    };

                    var responseGetToken = _queryProcessor.Handle(getToken);
                    accessToken = (string)cache.Get(channel);
                }

                //log access token
                var logToken = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, accessToken, command2.PreNonmobile, "CreateOrderPreRegisterToken", "CreateOrderPreRegisterCommandHandlerToken", null, "FBB", "");
                
                if (!string.IsNullOrEmpty(accessToken))
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", logToken, "Success", "", "");
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", logToken, "Failed", "Access Token is Null", "");
                }
                #endregion
                _logger.Info("StartMakeData custOrderInfo");
                #region MakeData custOrderInfo  c 
                #region SQL
                //SELECT LOV_NAME,LOV_VAL1 FROM fbb_cfg_lov 
                //WHERE LOV_VAL5 = 'FBBDORM008' AND LOV_TYPE = 'FBBDORM_PARAM_CREATEORDER' 
                //AND  LOV_VAL4 = 'CUST_ORDER' AND ORDER_BY = 1				
                #endregion

                var resultCustOrderInfo = from c in _cfgLov.Get()
                                          where c.LOV_VAL5 == "FBBDORM008"
                                          && c.LOV_TYPE == "FBBDORM_PARAM_CREATEORDER"
                                          && c.LOV_VAL4 == "CUST_ORDER"
                                          && c.ORDER_BY == 1
                                          select c;

                var fbssCustorder = new custOrderInfo();
                fbssCustorder.CUST_ORDER_NBR = ret_seq.Value.ToString();
                fbssCustorder.APPLY_DATE = DateTime.Now.ToString("yyyy-MM-dd");
                fbssCustorder.CREATE_DATE = DateTime.Now.ToString("yyyy-MM-dd");
                fbssCustorder.ORG_NAME = resultCustOrderInfo.Any(a => a.LOV_NAME == "ORG_NAME") ? resultCustOrderInfo.FirstOrDefault(a => a.LOV_NAME == "ORG_NAME").LOV_VAL1 : "";
                fbssCustorder.STAFF_NAME = resultCustOrderInfo.Any(a => a.LOV_NAME == "STAFF_NAME") ? resultCustOrderInfo.FirstOrDefault(a => a.LOV_NAME == "STAFF_NAME").LOV_VAL1 : "";
                fbssCustorder.STAFF_CODE = resultCustOrderInfo.Any(a => a.LOV_NAME == "STAFF_CODE") ? resultCustOrderInfo.FirstOrDefault(a => a.LOV_NAME == "STAFF_CODE").LOV_VAL1 : "";
                fbssCustorder.RESERVED_ID = "";
                fbssCustorder.SUBCONTRACTOR_CODE = command2.Upsubcontractid;
                fbssCustorder.CUST = GetCustInfo(command2.Cus_ID, command2.PreNonmobile, command2.Customername);
                fbssCustorder.PRODUCT_ORDER_LIST = GetProductOrderList(command2.AddressId, command2.Address, command2.AppointmentDate, command2.TimeSlot, command2.PreNonmobile, command2.Subcontract, command2.PasswordDec, command2.Customername, command2.Phone).ToArray();
                _logger.Info("EndMakeData custOrderInfo");

                #endregion

                string RESULT_DESC = "";
                //       string result = "";
                //log = FBSSExtensions.StartInterfaceFBSSLog(_uow, _intfLog, fbssCustorder,command2.PreNonmobile, "CreateOrderPreRegister", "CreateOrderPreRegisterCommandHandler");
                log = FBSSExtensions.StartInterfaceFBSSLog(_uow, _intfLog, fbssCustorder, command2.PreNonmobile, "CreateOrderPreRegister", "CreateOrderPreRegisterCommandHandler");

                //R21.2 Endpoint FBSSOrderServices.OrderService
                var endpointFbssOrderServices = (from l in _lov.Get() where l.LOV_NAME == "FBSS_ORDER_SERVICES" select l.LOV_VAL1).ToList().FirstOrDefault();

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;

                using (var service = new FBSSAccessToken.CustomOrderService(accessToken))
                {
                    if (!string.IsNullOrEmpty(endpointFbssOrderServices)) service.Url = endpointFbssOrderServices;
                    Premodel.Pre_Result = service.createOrder(fbssCustorder, out RESULT_DESC);
                }
                FBSSExtensions.EndInterfaceFBSSLog(_uow, _intfLog, Premodel.Pre_Result, log, "Success", Premodel.Pre_Result + " " + RESULT_DESC);
                command2.Pre_Total_Results = Premodel.Pre_Result;
            }
            catch (WebException webEx)
            {
                //R24.10 Call Access Token FBSS Exception 
                if ((webEx.Response is HttpWebResponse response && (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)) && (countException == 0))
                {
                    countException++;
                    cache.Remove(FBSSAccessToken.channelFBB.ToUpper());
                    webEx = null;
                    goto repeat;
                }

                _logger.Info("error=" + webEx.GetErrorMessage());
                if (null != log)
                {
                    //FBSSExtensions.EndInterfaceFBSSLog(_uow, _intfLog, "", log, "Failed", ex.Message);
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Failed", webEx.Message, "");
                }

                throw webEx;
            }
            catch (Exception ex)
            {
                _logger.Info("error=" + ex.GetErrorMessage());
                if (null != log)
                {
                    //FBSSExtensions.EndInterfaceFBSSLog(_uow, _intfLog, "", log, "Failed", ex.Message);
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Failed", ex.Message, "");
                }

                throw ex;
            }
        }



    }
}
