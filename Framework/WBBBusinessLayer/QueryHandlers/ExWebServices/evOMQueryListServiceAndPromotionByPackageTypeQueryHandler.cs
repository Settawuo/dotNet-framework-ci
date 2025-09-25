using AIRNETEntity.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using WBBBusinessLayer.QueryHandlers.ExWebServices.FbbCpGw;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.ExWebServices;
using WBBContract.Queries.ExWebServices.ATN;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.ExWebServices
{
    public class evOMQueryListServiceAndPromotionByPackageTypeQueryHandler : IQueryHandler<evOMQueryListServiceAndPromotionByPackageTypeQuery, evOMQueryListServiceAndPromotionByPackageTypeModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow; // insert log
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _FBB_CFG_LOV;
        private readonly IAirNetEntityRepository<AIR_SFF_SERVICE_CODE> _AIR_SFF_SERVICE_CODE;
        private readonly IEntityRepository<FBB_CUST_PROFILE> _custProfile;
        private readonly IEntityRepository<FBB_CUST_PACKAGE> _custPackage;

        public evOMQueryListServiceAndPromotionByPackageTypeQueryHandler(ILogger logger, IWBBUnitOfWork uow, IEntityRepository<FBB_INTERFACE_LOG> intfLog, IEntityRepository<FBB_CFG_LOV> FBB_CFG_LOV, IAirNetEntityRepository<AIR_SFF_SERVICE_CODE> AIR_SFF_SERVICE_CODE, IEntityRepository<FBB_CUST_PROFILE> custProfile,
            IEntityRepository<FBB_CUST_PACKAGE> custPackage)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _FBB_CFG_LOV = FBB_CFG_LOV;
            _AIR_SFF_SERVICE_CODE = AIR_SFF_SERVICE_CODE;
            _custProfile = custProfile;
            _custPackage = custPackage;
        }

        public evOMQueryListServiceAndPromotionByPackageTypeModel Handle(evOMQueryListServiceAndPromotionByPackageTypeQuery query)
        {
            InterfaceLogCommand log = null;
            InterfaceLogCommand log2 = null;
            InterfaceLogCommand log3 = null;
            InterfaceLogCommand log4 = null;
            InterfaceLogCommand log5 = null;
            InterfaceLogCommand log6 = null;
            InterfaceLogCommand log7 = null;
            List<AIR_SFF_SERVICE_CODE> servicecodelist = new List<AIR_SFF_SERVICE_CODE>();
            evOMQueryListServiceAndPromotionByPackageTypeModel modelreturn = new evOMQueryListServiceAndPromotionByPackageTypeModel();
            List<evOMQueryListServiceAndPromotionByPackageTypeModel> tmpServiceDatas = new List<evOMQueryListServiceAndPromotionByPackageTypeModel>();
            List<promotionModel> promotionlist = new List<promotionModel>();
            promotionModel promotionmodel = new promotionModel();
            modelreturn.productCDContent = "";
            bool CheckHavePlayBox = false;

            List<serviceModel> servicelist = new List<serviceModel>();
            serviceModel servicemodel = new serviceModel();
            string errorValueNull = "";
            try
            {
                /* get promotion type and service type*/
                var inputparam = (from z in _FBB_CFG_LOV.Get()
                                  where z.DISPLAY_VAL == "evOMQueryListServiceAndPromotionByPackageType" && z.ACTIVEFLAG == "Y" && z.LOV_VAL5 == "FBBOR001"
                                  select z).ToList();

                var promotiontype = inputparam.Where(a => a.LOV_NAME == "promotionType").FirstOrDefault();
                var servicetype = inputparam.Where(a => a.LOV_NAME == "serviceType").FirstOrDefault();
                //17.6
                var playBoxValue = inputparam.Where(a => a.LOV_NAME == "CONFIG_TYPE_SERVICE").FirstOrDefault();
                /*Get limit of playbox*/
                modelreturn.L_NUMBER_OF_PLAYBOX = (from z in _FBB_CFG_LOV.Get()
                                                   where z.LOV_NAME == "L_NUMBER_OF_PLAYBOX"
                                                   select z.LOV_VAL1).FirstOrDefault();

                /*Get content*/
                var ContentList = (from z in _FBB_CFG_LOV.Get()
                                   where z.LOV_NAME == "PRODUCT_CD" && z.LOV_TYPE == "FBB_CONSTANT"
                                   select z.LOV_VAL1).ToList();

                /*get serice code list*/
                var query2 = (from z in _AIR_SFF_SERVICE_CODE.Get()
                             where z.PRODUCT_NAME == "FTTH" || z.PRODUCT_NAME == "VDSL" || z.PRODUCT_NAME == "FTTR"
                             select z).ToList();
                if (query2 != null && query2.Count > 0)
                {
                    servicecodelist = query2;
                }

                var query4 = (from z in _AIR_SFF_SERVICE_CODE.Get()
                              where z.PRODUCT_NAME == "PBOX"
                              select z.SERVICE_CODE).FirstOrDefault();

                var service_code_PBX = "";
                if (query4.Any())
                {
                    service_code_PBX = query4;
                }

                var request = new SFFServices.SffRequest();
                request.Event = "evOMQueryListServiceAndPromotionByPackageType";
                var paramArray = new SFFServices.Parameter[3];
                var param0 = new SFFServices.Parameter();
                var param1 = new SFFServices.Parameter();
                var param2 = new SFFServices.Parameter();


                param0.Name = "mobileNo";
                param0.Value = query.mobileNo;
                param1.Name = promotiontype.LOV_NAME;
                param1.Value = promotiontype.LOV_VAL1;
                param2.Name = servicetype.LOV_NAME;
                //param2.Value = servicetype.LOV_VAL1;
                param2.Value = !string.IsNullOrEmpty(query.serviceType) ? query.serviceType : servicetype.LOV_VAL1;


                paramArray[0] = param0;
                paramArray[1] = param1;
                paramArray[2] = param2;

                var paramList = new SFFServices.ParameterList();
                paramList.Parameter = paramArray;

                request.ParameterList = paramList;

                _logger.Info("Call evESeService SFF");
                _logger.Info("mobileNo: " + query.mobileNo + ", promotionType: " + promotiontype.LOV_VAL1 + ", serviceType: " + servicetype.LOV_VAL1);

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, request, query.mobileNo, "evOMQueryListServiceAndPromotionByPackageType", "evOMQueryListServiceAndPromotionByPackageType", query.idCard, "FBB|" + query.FullUrl, "");


                using (var service = new SFFServices.SFFServiceService())
                {
                    var data = service.ExecuteService(request);

                    if (data != null)
                    {
                        _logger.Info(data.ErrorMessage);
                        if (data.ErrorMessage != "" || data.ErrorMessage != null)
                        {
                            modelreturn.ErrorMessage = data.ErrorMessage;
                            var errSp = data.ErrorMessage.Trim().Split(':');
                            modelreturn.ErrorMessage = errSp[0];
                        }

                        foreach (var a in data.ParameterList.Parameter)
                        {
                            if (a.Name == "resultFlag")
                                modelreturn.resultFlag = a.Value;
                        }
                        if (modelreturn.resultFlag == "Y")
                        {
                            //ถ้า retrun resultFlag=="Y"--> Fail
                            string productType = "";
                            Boolean IsproductCD = false;
                            modelreturn.v_number_of_pb_number = 0;

                            int indexa = 0;
                            foreach (var a in data.ParameterList.ParameterList1)
                            {
                                evOMQueryListServiceAndPromotionByPackageTypeModel tmpServiceData = new evOMQueryListServiceAndPromotionByPackageTypeModel();

                                productType = "";
                                IsproductCD = false;

                                int indexb = 0;
                                promotionmodel = new promotionModel();

                                var ipcammera = false;
                                foreach (var b in a.Parameter)
                                {
                                    if (b.Name == "productCD")
                                    {
                                        IsproductCD = servicecodelist.Exists(x => x.SERVICE_CODE == b.Value);
                                        promotionmodel.productCD = b.Value;
                                        if (ContentList.Count > 0)
                                        {
                                            if (ContentList.Exists(t => t.ToString() == b.Value))
                                            {
                                                modelreturn.productCDContent = b.Value;
                                            }
                                        }
                                    }

                                    if (b.Name == "productType")
                                    {
                                        productType = b.Value;
                                        promotionmodel.productType = b.Value;
                                    }

                                    if (b.Name == "productClass")
                                    {
                                        promotionmodel.productClass = b.Value;
                                    }

                                    //R23.11
                                    if (b.Name == "productName")
                                    {
                                        if (b.Value == "AIS Cloud IP Camera")
                                        {
                                            ipcammera = true;
                                        }
                                    }

                                    if (productType == "Service" && IsproductCD)
                                    {
                                        // get address productType == "Service" and productCD exist in <<select service_code from air_sff_service_code where product_name in ('FTTH','VDSL');>>

                                        foreach (var c in a.ParameterList1)
                                        {

                                            if (c.Parameter[0].Value == "installAddress1")
                                            {
                                                try
                                                {
                                                    //modelreturn.v_installAddress1 = Breakword(c.Parameter[1].Value);
                                                    tmpServiceData.v_installAddress1 = Breakword(c.Parameter[1].Value);
                                                }
                                                catch (Exception ex)
                                                {
                                                    //modelreturn.v_installAddress1 = "";
                                                    tmpServiceData.v_installAddress1 = "";
                                                    errorValueNull += "| Promo ParameterList.ParameterList1[" + indexa + "][" + indexb + "]" + "installAddress1";
                                                }
                                            }
                                            if (c.Parameter[0].Value == "installAddress2")
                                            {
                                                try
                                                {
                                                    //modelreturn.v_installAddress2 = c.Parameter[1].Value;
                                                    tmpServiceData.v_installAddress2 = c.Parameter[1].Value;
                                                }
                                                catch (Exception ex)
                                                {
                                                    //modelreturn.v_installAddress2 = "";
                                                    tmpServiceData.v_installAddress2 = "";
                                                    errorValueNull += "| Promo ParameterList.ParameterList1[" + indexa + "][" + indexb + "]" + "installAddress2";
                                                }
                                            }
                                            if (c.Parameter[0].Value == "installAddress3")
                                            {
                                                try
                                                {
                                                    //modelreturn.v_installAddress3 = c.Parameter[1].Value;
                                                    tmpServiceData.v_installAddress3 = c.Parameter[1].Value;
                                                }
                                                catch (Exception ex)
                                                {
                                                    //modelreturn.v_installAddress3 = "";
                                                    tmpServiceData.v_installAddress3 = "";
                                                    errorValueNull += "| Promo ParameterList.ParameterList1[" + indexa + "][" + indexb + "]" + "installAddress3";
                                                }
                                            }
                                            if (c.Parameter[0].Value == "installAddress4")
                                            {
                                                try
                                                {
                                                    //modelreturn.v_installAddress4 = c.Parameter[1].Value;
                                                    tmpServiceData.v_installAddress4 = c.Parameter[1].Value;
                                                }
                                                catch (Exception ex)
                                                {
                                                    //modelreturn.v_installAddress4 = "";
                                                    tmpServiceData.v_installAddress4 = "";
                                                    errorValueNull += "| Promo ParameterList.ParameterList1[" + indexa + "][" + indexb + "]" + "installAddress4";
                                                }
                                            }
                                            if (c.Parameter[0].Value == "installAddress5")
                                            {
                                                try
                                                {
                                                    //modelreturn.v_installAddress5 = Breakword(c.Parameter[1].Value);
                                                    tmpServiceData.v_installAddress5 = Breakword(c.Parameter[1].Value);
                                                }
                                                catch (Exception ex)
                                                {
                                                    //modelreturn.v_installAddress5 = "";
                                                    tmpServiceData.v_installAddress5 = "";
                                                    errorValueNull += "| Promo ParameterList.ParameterList1[" + indexa + "][" + indexb + "]" + "installAddress5";
                                                }
                                            }
                                            if (c.Parameter[0].Value == "addressId")
                                            {
                                                try
                                                {
                                                    //modelreturn.addressId = Breakword(c.Parameter[1].Value);
                                                    tmpServiceData.addressId = Breakword(c.Parameter[1].Value);
                                                }
                                                catch (Exception ex)
                                                {
                                                    //modelreturn.addressId = "";
                                                    tmpServiceData.addressId = "";
                                                    errorValueNull += "| Promo ParameterList.ParameterList1[" + indexa + "][" + indexb + "]" + "addressId";
                                                }
                                            }
                                            if (c.Parameter[0].Value == "contactName")
                                            {
                                                try
                                                {
                                                    //modelreturn.contactName = Breakword(c.Parameter[1].Value);
                                                    tmpServiceData.contactName = Breakword(c.Parameter[1].Value);
                                                }
                                                catch (Exception ex)
                                                {
                                                    //modelreturn.contactName = "";
                                                    tmpServiceData.contactName = "";
                                                    errorValueNull += "| Promo ParameterList.ParameterList1[" + indexa + "][" + indexb + "]" + "contactName";
                                                }
                                            }
                                            if (c.Parameter[0].Value == "contactMobilePhone")
                                            {
                                                try
                                                {
                                                    //modelreturn.contactMobilePhone = Breakword(c.Parameter[1].Value);
                                                    tmpServiceData.contactMobilePhone = Breakword(c.Parameter[1].Value);
                                                }
                                                catch (Exception ex)
                                                {
                                                    //modelreturn.contactMobilePhone = "";
                                                    tmpServiceData.contactMobilePhone = "";
                                                    errorValueNull += "| Promo ParameterList.ParameterList1[" + indexa + "][" + indexb + "]" + "contactMobilePhone";
                                                }
                                            }
                                            if (c.Parameter[0].Value == "flowFlag")
                                            {
                                                try
                                                {
                                                    //modelreturn.flowFlag = Breakword(c.Parameter[1].Value);
                                                    tmpServiceData.flowFlag = Breakword(c.Parameter[1].Value);
                                                }
                                                catch (Exception ex)
                                                {
                                                    //modelreturn.flowFlag = "";
                                                    tmpServiceData.flowFlag = "";
                                                    errorValueNull += "| Promo ParameterList.ParameterList1[" + indexa + "][" + indexb + "]" + "flowFlag";
                                                }
                                            }

                                        }//end for                                       

                                    }//end if

                                    //V 17.6
                                    if (b.Name == "productName")
                                    {
                                        if (b.Value == playBoxValue.LOV_VAL1)
                                        {
                                            CheckHavePlayBox = true;
                                        }
                                    }

                                    //17.9 Speed boost
                                    if (b.Name == "endDate")
                                    {
                                        promotionmodel.endDate = b.Value;
                                    }

                                    if (b.Name == "startDate")
                                    {
                                        promotionmodel.startDate = b.Value;
                                        tmpServiceData.startDate = b.Value;
                                    }

                                    if (b.Name == "productStatus")
                                    {
                                        promotionmodel.productStatus = b.Value;
                                    }

                                    indexb++;
                                }

                                //R23.11
                                if (ipcammera)
                                {
                                    foreach (var c in a.ParameterList1)
                                    {
                                        if (c.Parameter[0].Value == "userAccount")
                                        {
                                            try
                                            {
                                                modelreturn.userAccount = Breakword(c.Parameter[1].Value);
                                            }
                                            catch (Exception ex)
                                            {
                                                //modelreturn.contactMobilePhone = "";
                                                tmpServiceData.userAccount = "";
                                                errorValueNull += "| Promo ParameterList.ParameterList1[" + indexa + "][" + indexb + "]" + "userAccount";
                                            }
                                        }


                                    }
                                }

                                promotionlist.Add(promotionmodel);

                                if (productType == "Service" && IsproductCD)
                                {
                                    tmpServiceDatas.Add(tmpServiceData);
                                }

                                //R20.5  
                                int indexbs = 0;
                                servicemodel = new serviceModel();
                                string productName = "";
                                var flagCheckFor3BB = (from z in _FBB_CFG_LOV.Get()
                                                       where z.LOV_NAME == "Existing_AccessType_Flag" && z.LOV_TYPE == "FBB_CONSTANT" && z.ACTIVEFLAG == "Y"
                                                       select z.LOV_VAL1).FirstOrDefault();
                                var lovProductName = (from z in _FBB_CFG_LOV.Get()
                                                      where z.LOV_NAME == "Existing_AccessType_AIS_3BB" && z.LOV_TYPE == "FBB_CONSTANT" && z.ACTIVEFLAG == "Y"
                                                      select z).ToList();
                                //log3 = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, flagCheckFor3BB, query.mobileNo, "flagCheckFor3BB", "flagCheckFor3BB", query.idCard, "FBB|" + query.FullUrl, "");
                                foreach (var b in a.Parameter)
                                {

                                    if (b.Name == "productName")
                                    {
                                        if (flagCheckFor3BB == "Y")
                                        {
                                            productName = b.Value;
                                            //log3 = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, productName, query.mobileNo, "productName", "productName", query.idCard, "FBB|" + query.FullUrl, "");
                                            //log4 = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, flagCheckFor3BB, query.mobileNo, "flagCheckFor3BB", "flagCheckFor3BB", query.idCard, "FBB|" + query.FullUrl, "");
                                        }
                                    }
                                    if (b.Name == "productType")
                                    {
                                        productType = b.Value;
                                        servicemodel.productType = b.Value;
                                    }

                                    if (b.Name == "startDate")
                                    {
                                        try
                                        {
                                            servicemodel.startDate = b.Value;

                                        }
                                        catch (Exception ex)
                                        {
                                            servicemodel.startDate = "";
                                            errorValueNull += "| service ParameterList.ParameterList1[" + indexa + "][" + indexbs + "]" + "startDate";
                                        }
                                    }

                                    if (b.Name == "endDate")
                                    {
                                        try
                                        {
                                            servicemodel.endDate = b.Value;
                                        }
                                        catch (Exception ex)
                                        {
                                            servicemodel.endDate = "";
                                            errorValueNull += "| service ParameterList.ParameterList1[" + indexa + "][" + indexbs + "]" + "endDate";
                                        }
                                    }

                                    if (productType == "Service")
                                    {
                                        foreach (var c in a.ParameterList1)
                                        {
                                            if (flagCheckFor3BB == "Y")
                                            {
                                                var accessType = lovProductName.FirstOrDefault(x => x.LOV_VAL1 == productName);
                                                //log5 = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, "productName : " + accessType, query.mobileNo, "accessTypeLov", "accessTypeLov", query.idCard, "FBB|" + query.FullUrl, "");
                                                if (accessType != null)
                                                {
                                                    servicemodel.accessType = accessType.LOV_VAL3;
                                                    //log6 = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, "accessType : " + servicemodel.accessType, query.mobileNo, "servicemodel", "accessType", query.idCard, "FBB|" + query.FullUrl, "");
                                                }
                                            }
                                            if (c.Parameter[0].Value == "accessType")
                                            {
                                                try
                                                {
                                                    if (flagCheckFor3BB != "Y")
                                                    {
                                                        servicemodel.accessType = string.IsNullOrEmpty(c.Parameter[1].Value) ? "" : c.Parameter[1].Value;
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    servicemodel.accessType = "";
                                                    errorValueNull += "| service ParameterList.ParameterList1[" + indexa + "][" + indexbs + "]" + "accessType";
                                                }
                                            }
                                            if (c.Parameter[0].Value == "networkProvider")
                                            {
                                                try
                                                {
                                                    if (c.Parameter[1].Value.Contains("3BB"))
                                                    {
                                                        c.Parameter[1].Value = "3BB";
                                                    }
                                                    servicemodel.networkProvider = string.IsNullOrEmpty(c.Parameter[1].Value) ? "" : c.Parameter[1].Value;
                                                }
                                                catch (Exception ex)
                                                {
                                                    servicemodel.networkProvider = "";
                                                    errorValueNull += "| service ParameterList.ParameterList1[" + indexa + "][" + indexbs + "]" + "networkProvider";
                                                }
                                            }
                                            if (c.Parameter[0].Value == "addressId")
                                            {
                                                try
                                                {
                                                    servicemodel.addressId = string.IsNullOrEmpty(c.Parameter[1].Value) ? "" : c.Parameter[1].Value;
                                                }
                                                catch (Exception ex)
                                                {
                                                    servicemodel.addressId = "";
                                                    errorValueNull += "| service ParameterList.ParameterList1[" + indexa + "][" + indexbs + "]" + "addressId";
                                                }
                                            }
                                            if (c.Parameter[0].Value == "gridId") //R22.04 WTTx
                                            {
                                                try
                                                {
                                                    servicemodel.gridId = string.IsNullOrEmpty(c.Parameter[1].Value) ? "" : c.Parameter[1].Value;
                                                }
                                                catch (Exception ex)
                                                {
                                                    servicemodel.gridId = "";
                                                    errorValueNull += "| service ParameterList.ParameterList1[" + indexa + "][" + indexbs + "]" + "gridId";
                                                }
                                            }

                                        }//end for
                                    }

                                    indexbs++;
                                }
                                servicelist.Add(servicemodel);
                                //end R20.5

                                indexa++;
                            }

                            string ServiceMaxStartDateTimeSTR = "";
                            if (tmpServiceDatas != null && tmpServiceDatas.Count > 0)
                            {
                                DateTime ServiceMaxStartDateTime = DateTime.Now;
                                bool GetServiceStartDateTime = true;
                                foreach (var item in tmpServiceDatas)
                                {
                                    CultureInfo provider = CultureInfo.InvariantCulture;
                                    DateTime CurDateTime = DateTime.Now;
                                    if (DateTime.TryParseExact(item.startDate, "dd/MM/yyyy HH:mm:ss", provider, DateTimeStyles.None, out CurDateTime))
                                    {
                                        if (GetServiceStartDateTime)
                                        {
                                            ServiceMaxStartDateTimeSTR = item.startDate;
                                            ServiceMaxStartDateTime = CurDateTime;
                                            GetServiceStartDateTime = false;
                                            modelreturn.v_installAddress1 = item.v_installAddress1;
                                            modelreturn.v_installAddress2 = item.v_installAddress2;
                                            modelreturn.v_installAddress3 = item.v_installAddress3;
                                            modelreturn.v_installAddress4 = item.v_installAddress4;
                                            modelreturn.v_installAddress5 = item.v_installAddress5;
                                            modelreturn.addressId = item.addressId;
                                            modelreturn.contactName = item.contactName;
                                            modelreturn.contactMobilePhone = item.contactMobilePhone;
                                            modelreturn.flowFlag = item.flowFlag;
                                        }
                                        else
                                        {
                                            if (ServiceMaxStartDateTime < CurDateTime)
                                            {
                                                ServiceMaxStartDateTimeSTR = item.startDate;
                                                ServiceMaxStartDateTime = CurDateTime;
                                                modelreturn.v_installAddress1 = item.v_installAddress1;
                                                modelreturn.v_installAddress2 = item.v_installAddress2;
                                                modelreturn.v_installAddress3 = item.v_installAddress3;
                                                modelreturn.v_installAddress4 = item.v_installAddress4;
                                                modelreturn.v_installAddress5 = item.v_installAddress5;
                                                modelreturn.addressId = item.addressId;
                                                modelreturn.contactName = item.contactName;
                                                modelreturn.contactMobilePhone = item.contactMobilePhone;
                                                modelreturn.flowFlag = item.flowFlag;
                                            }
                                        }
                                    }
                                }
                                modelreturn.v_installAddress = modelreturn.v_installAddress1 + " " + modelreturn.v_installAddress2 + " " + modelreturn.v_installAddress3 + " " + modelreturn.v_installAddress4 + " " + modelreturn.v_installAddress5;
                            }



                            List<promotionModel> tmpPromotionlist = promotionlist.Where(a => a.productType == "Promotion" && a.productClass == "Main").ToList();
                            string productCDMain = "";
                            DateTime MaxStartDateTime = DateTime.Now;
                            bool GetStartDateTime = true;
                            foreach (var item in tmpPromotionlist)
                            {
                                CultureInfo provider = CultureInfo.InvariantCulture;
                                DateTime CurDateTime = DateTime.Now;
                                if (DateTime.TryParseExact(item.startDate, "dd/MM/yyyy HH:mm:ss", provider, DateTimeStyles.None, out CurDateTime))
                                {
                                    if (GetStartDateTime)
                                    {
                                        MaxStartDateTime = CurDateTime;
                                        productCDMain = item.productCD.ToSafeString();
                                        GetStartDateTime = false;
                                    }
                                    else
                                    {
                                        if (MaxStartDateTime < CurDateTime)
                                        {
                                            MaxStartDateTime = CurDateTime;
                                            productCDMain = item.productCD.ToSafeString();
                                        }
                                    }
                                }
                            }

                            modelreturn.v_sff_main_promotionCD = productCDMain;

                            /// replacePlayboxCheckProductMainNotUse 17.7

                            var listProductMainNotUsePlaybox = (from z in _FBB_CFG_LOV.Get()
                                                                where z.LOV_TYPE == "FBB_CONSTANT" && z.ACTIVEFLAG == "Y" && z.LOV_NAME == "REPLACEPLAYBOX_CHECK_PACKAGE"
                                                                select z.LOV_VAL1).ToList();
                            modelreturn.replacePlayboxCheckProductMainNotUse = false;
                            if (listProductMainNotUsePlaybox != null && listProductMainNotUsePlaybox.Count > 0)
                            {
                                if (listProductMainNotUsePlaybox.Contains(productCDMain))
                                {
                                    modelreturn.replacePlayboxCheckProductMainNotUse = true;
                                }
                            }

                            /// end

                            modelreturn.v_number_of_pb_number = promotionlist.Where(a => a.productType == "Service" && a.productCD == service_code_PBX).Count();

                            //v17.6 
                            modelreturn.checkHavePlayBox = CheckHavePlayBox;

                            //17.9 Speed boost
                            modelreturn.ListPromotion = promotionlist;

                            //R20.7
                            List<serviceModel> servicelistTmp = servicelist.Where(l => l.productType == "Service" && l.startDate == ServiceMaxStartDateTimeSTR && l.endDate == "").ToList();

                            modelreturn.ListService = servicelistTmp;
                            modelreturn.errorValueNull = errorValueNull;

                            var sAccessType = "";
                            foreach (var iServiceList in servicelistTmp)
                            {
                                if (!string.IsNullOrEmpty(iServiceList.accessType)) { sAccessType = iServiceList.accessType.ToSafeString(); break; }
                            }

                            var sNetworkProvider = "";
                            foreach (var iServiceList in servicelistTmp)
                            {
                                if (!string.IsNullOrEmpty(iServiceList.networkProvider)) { sNetworkProvider = iServiceList.networkProvider.ToSafeString(); break; }
                            }

                            var sAddressId = "";
                            foreach (var iServiceList in servicelistTmp)
                            {
                                if (!string.IsNullOrEmpty(iServiceList.addressId)) { sAddressId = iServiceList.addressId.ToSafeString(); break; }
                            }

                            //R22.04 WTTx
                            var sGridId = "";
                            foreach (var iServiceList in servicelistTmp)
                            {
                                if (!string.IsNullOrEmpty(iServiceList.gridId)) { sGridId = iServiceList.gridId.ToSafeString(); break; }
                            }

                            //R21.09 Issue search AccessType,NetworkProvider,AddressId is null
                            List<serviceModel> servicelistTmp2 = servicelist.Where(l => l.productType == "Service").ToList();
                            if (string.IsNullOrEmpty(sAccessType))
                            {
                                foreach (var iServiceList in servicelistTmp2)
                                {
                                    if (!string.IsNullOrEmpty(iServiceList.accessType)) { sAccessType = iServiceList.accessType.ToSafeString(); break; }
                                }
                            }
                            if (string.IsNullOrEmpty(sNetworkProvider))
                            {
                                foreach (var iServiceList in servicelistTmp2)
                                {
                                    if (!string.IsNullOrEmpty(iServiceList.networkProvider)) { sNetworkProvider = iServiceList.networkProvider.ToSafeString(); break; }
                                }
                            }
                            if (string.IsNullOrEmpty(sAddressId))
                            {
                                foreach (var iServiceList in servicelistTmp2)
                                {
                                    if (!string.IsNullOrEmpty(iServiceList.addressId)) { sAddressId = iServiceList.addressId.ToSafeString(); break; }
                                }
                            }
                            if (string.IsNullOrEmpty(sGridId))//R22.04 WTTx
                            {
                                foreach (var iServiceList in servicelistTmp2)
                                {
                                    if (!string.IsNullOrEmpty(iServiceList.gridId)) { sGridId = iServiceList.gridId.ToSafeString(); break; }
                                }
                            }
                            //--End R21.09

                            //-- access_mode
                            modelreturn.access_mode = sAccessType.ToSafeString();
                            //-- owner_product
                            string tmpNetworkProvider = sNetworkProvider.ToUpper();
                            if (tmpNetworkProvider.Contains("3BB"))
                            {
                                sNetworkProvider = "3BB";
                            }
                            var datalog = "sNetworkProvider : " + sNetworkProvider + " | " + "sAccessType : " + sAccessType;
                            log7 = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, datalog, query.mobileNo, "sAccessType", "sAccessType", query.idCard, "FBB|" + query.FullUrl, "");
                            var query3 = (from z in _FBB_CFG_LOV.Get()
                                          where z.LOV_NAME == "MAPPING_OWNER_PRODUCT" && z.ACTIVEFLAG == "Y"
                                          && z.LOV_VAL1 == sAccessType && z.LOV_VAL2 == sNetworkProvider
                                          select z.LOV_VAL3).FirstOrDefault();
                            modelreturn.v_owner_product = query3.ToSafeString();
                            //-- package_subtype
                            var query5 = (from z in _FBB_CFG_LOV.Get()
                                          where z.LOV_NAME == "MAPPING_PRODUCT_SUBTYPE" && z.ACTIVEFLAG == "Y"
                                          && z.LOV_VAL1 == sAccessType
                                          select z.LOV_VAL2).FirstOrDefault();
                            modelreturn.v_package_subtype = query5.ToSafeString();
                            //end R20.5

                            //R20.6
                            modelreturn.addressId = sAddressId.ToSafeString();

                            //R22.04 WTTx
                            modelreturn.gridId = sGridId.ToSafeString();

                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, data, log, "Success", "", "");
                        }
                        else// service return flag N
                        {
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, data, log, "Failed", modelreturn.ErrorMessage.ToSafeString(), "");
                        }
                    }
                }

                return modelreturn;
            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, modelreturn, log, "Failed", ex.ToSafeString(), "");
                return modelreturn;
            }

        }
        private string Breakword(string input)
        {
            if (input == null) input = "";

            var templist = (from z in _FBB_CFG_LOV.Get()
                            where z.LOV_NAME == "REPLACE_INSTALL_ADDRESS"
                            select z.LOV_VAL1).FirstOrDefault();

            string regex = @templist;
            string output = Regex.Replace(input, regex, "");

            return output;
        }
    }

    public class evESQueryPersonalInformationQueryHandler : IQueryHandler<evESQueryPersonalInformationQuery, List<evESQueryPersonalInformationModel>>
    {
        private readonly List<string> CONF_ENABLE_OPTIONS = new List<string> { "2", "3", "4", "5" };
        private const string CONF_ENABLE_APIMAPPINGSKY = "ENABLE_EVESQUERYPERSONALINFORMATION_TOBE_APIMAPPINGSKY";
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow; // insert log
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IQueryHandler<GetCustomerProfileMobilePackageCurrentRequest, GetCustomerProfileMobilePackageCurrentResult> _mobilePackageCurrent;
        private readonly IQueryHandler<GetCustomerProfileServiceProfileRequest, GetCustomerProfileServiceProfileResult> _serviceProfile;
        private readonly IQueryHandler<GetCustomerProfileSubScriptionAccountRequest, GetCustomerProfileSubScriptionAccountResult> _subscriptionAccount;
        private readonly IQueryHandler<GetCustomerProfileSubScriptionProfileRequest, GetCustomerProfileSubScriptionProfileResult> _subScriptionProfile;

        public evESQueryPersonalInformationQueryHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_CFG_LOV> cfgLov,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IQueryHandler<GetCustomerProfileMobilePackageCurrentRequest, GetCustomerProfileMobilePackageCurrentResult> mobilePackageCurrent,
            IQueryHandler<GetCustomerProfileServiceProfileRequest, GetCustomerProfileServiceProfileResult> serviceProfile,
            IQueryHandler<GetCustomerProfileSubScriptionAccountRequest, GetCustomerProfileSubScriptionAccountResult> subscriptionAccount,
            IQueryHandler<GetCustomerProfileSubScriptionProfileRequest, GetCustomerProfileSubScriptionProfileResult> subScriptionProfile)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _cfgLov = cfgLov;
            _mobilePackageCurrent = mobilePackageCurrent;
            _serviceProfile = serviceProfile;
            _subscriptionAccount = subscriptionAccount;
            _subScriptionProfile = subScriptionProfile;
        }

        public List<evESQueryPersonalInformationModel> Handle(evESQueryPersonalInformationQuery query)
        {
            var targetOption = string.IsNullOrEmpty(query.option) ? string.Empty : query.option;
            var confApiMappingSky = _cfgLov.Get().Where(x => x.LOV_NAME == CONF_ENABLE_APIMAPPINGSKY).FirstOrDefault() ?? new FBB_CFG_LOV();
            if (confApiMappingSky?.ACTIVEFLAG == "Y"
                && CONF_ENABLE_OPTIONS.Any(x => x == targetOption))
            {
                return HandleBySky(query);
            }
            return HandleBySff(query);
        }

        private List<evESQueryPersonalInformationModel> HandleBySky(evESQueryPersonalInformationQuery query)
        {
            InterfaceLogCommand log = null;
            var result = new List<evESQueryPersonalInformationModel>();
            var success = "";
            var remark = "";

            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.mobileNo, "evESQueryPersonalInformation", "evESQueryPersonalInformation", query.mobileNo, "FBB|" + query.FullUrl, "");

                _ = int.TryParse(query.option, out int option);
                switch (option)
                {
                    //mobilePackageCurrent
                    case 2:
                        result = OnOption2(query.mobileNo);
                        break;

                    //serviceProfile
                    case 3:
                        result = OnOption3(query.mobileNo);
                        break;

                    //subScription-account
                    case 4:
                        result = OnOption4(query.mobileNo);
                        break;

                    //subScription-account+subScription-profile
                    case 5:
                        result = OnOption5(query.mobileNo);
                        break;

                    default:
                        throw new InvalidOperationException($"Error unable to detect query option('{query.option}')");
                }
                success = "Success";
            }
            catch (Exception ex)
            {
                _logger.Error($"Error when call {GetType().Name} : {ex.ToSafeString()}");
                success = "Failed";
                remark = ex.Message;
            }
            finally
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, success, remark, "");
            }
            return result;
        }

        private List<evESQueryPersonalInformationModel> OnOption2(string _msisdn)
        {
            var results = new List<evESQueryPersonalInformationModel>();
            var subScriptionProfile = GetSubScriptionProfile(_msisdn);
            var req = new GetCustomerProfileMobilePackageCurrentRequest
            {
                msisdn = _msisdn
            };
            var res = _mobilePackageCurrent.Handle(req);
            if (string.IsNullOrEmpty(res?.resultCode))
            {
                throw new InvalidOperationException($"Unknow result code for response from '{_mobilePackageCurrent.GetType().Name}'.");
            }
            if (ResultBaseATN.DefSuccessCodes.Contains(res?.resultCode))
            {
                var main = res?.resultData?.mobilePackageCurrent?.main ?? new CustomerProfileMobilePackageCurrentMain();
                results.Add(main.ToEvESQueryPersonalInformationModelOption2(subScriptionProfile));
                var ontop = res?.resultData?.mobilePackageCurrent?.ontop ?? new List<CustomerProfileMobilePackageCurrentOntop>();
                var ontopList = ontop.ToEvESQueryPersonalInformationModelOption2(subScriptionProfile);
                results.AddRange(ontopList);
            }
            _logger.Info($"{GetType().Name}.OnOption2 resultCode: {res?.resultCode}, resultList: {results?.Count}");
            return results;
        }

        private List<evESQueryPersonalInformationModel> OnOption3(string _msisdn)
        {
            var results = new List<evESQueryPersonalInformationModel>();
            var req = new GetCustomerProfileServiceProfileRequest
            {
                msisdn = _msisdn
            };
            var res = _serviceProfile.Handle(req);
            if (string.IsNullOrEmpty(res?.resultCode))
            {
                throw new InvalidOperationException($"Unknow result code for response from '{_serviceProfile.GetType().Name}'.");
            }
            if (ResultBaseATN.DefSuccessCodes.Contains(res?.resultCode))
            {
                var serviceProfiles = res?.resultData?.serviceProfile ?? new List<GetCustomerProfileServiceProfileItem>();
                results = serviceProfiles.ToEvESQueryPersonalInformationModelOption3();
            }
            _logger.Info($"{GetType().Name}.OnOption3 resultCode: {res?.resultCode}, resultList: {results?.Count}");
            return results;
        }

        private List<evESQueryPersonalInformationModel> OnOption4(string _msisdn)
        {
            var results = new List<evESQueryPersonalInformationModel>();
            var req = new GetCustomerProfileSubScriptionAccountRequest
            {
                msisdn = _msisdn
            };
            var res = _subscriptionAccount.Handle(req);
            if (string.IsNullOrEmpty(res?.resultCode))
            {
                throw new InvalidOperationException($"Unknow result code for response from '{_subscriptionAccount.GetType().Name}'.");
            }
            if (ResultBaseATN.DefSuccessCodes.Contains(res.resultCode))
            {
                results = res?.resultData?.subscriptionAccount?.ToEvESQueryPersonalInformationModelOption4();
            }
            _logger.Info($"{GetType().Name}.OnOption4 resultCode: {res?.resultCode}, resultList: {results?.Count}");
            return results;
        }

        private List<evESQueryPersonalInformationModel> OnOption5(string _msisdn)
        {
            var results = new List<evESQueryPersonalInformationModel>();
            var subScriptionProfile = GetSubScriptionProfile(_msisdn);

            //OnOption5_1
            var req1 = new GetCustomerProfileSubScriptionAccountRequest
            {
                msisdn = _msisdn
            };
            var res1 = _subscriptionAccount.Handle(req1);
            if (string.IsNullOrEmpty(res1?.resultCode))
            {
                throw new InvalidOperationException($"Unknow result code for response from '{_subscriptionAccount.GetType().Name}'.");
            }
            if (ResultBaseATN.DefSuccessCodes.Contains(res1?.resultCode))
            {
                var subScriptionAccount = res1?.resultData?.subscriptionAccount;
                results = subScriptionAccount.ToEvESQueryPersonalInformationModelOption5(subScriptionProfile);
            }
            _logger.Info($"{GetType().Name}.OnOption5 resultCode: {res1?.resultCode}, resultList: {results?.Count}");
            return results;
        }

        private GetCustomerProfileSubScriptionProfileItem GetSubScriptionProfile(string _msisdn)
        {
            var subScriptionProfile = new GetCustomerProfileSubScriptionProfileItem();

            //OnOption5_2
            var req2 = new GetCustomerProfileSubScriptionProfileRequest
            {
                key_name = "msisdn",
                key_value = _msisdn
            };
            var res2 = _subScriptionProfile.Handle(req2);
            if (string.IsNullOrEmpty(res2?.resultCode))
            {
                throw new InvalidOperationException($"Unknow result code for response from '{_subScriptionProfile.GetType().Name}'.");
            }
            if (ResultBaseATN.DefSuccessCodes.Contains(res2?.resultCode))
            {
                subScriptionProfile = res2?.resultData?.subScriptionProfile ?? new GetCustomerProfileSubScriptionProfileItem();
            }
            return subScriptionProfile;
        }


        public List<evESQueryPersonalInformationModel> HandleBySff(evESQueryPersonalInformationQuery query)
        {
            InterfaceLogCommand log = null;
            List<evESQueryPersonalInformationModel> result = new List<evESQueryPersonalInformationModel>();
            try
            {
                var objReq = new SFFServices.SffRequest();
                if (!string.IsNullOrEmpty(query.sourceSystem))
                {
                    objReq.ParameterList = new SFFServices.ParameterList()
                    {
                        Parameter = new SFFServices.Parameter[]
                          {
                                   new SFFServices.Parameter() { Name = "mobileNo", Value = query.mobileNo },
                                   new SFFServices.Parameter() { Name = "option", Value = query.option },
                                   new SFFServices.Parameter() { Name = "sourceSystem", Value = query.sourceSystem },
                          },
                        ParameterType = "String"
                    };
                    objReq.Event = "evESQueryPersonalInformation";
                }
                else
                {
                    objReq.ParameterList = new SFFServices.ParameterList()
                    {
                        Parameter = new SFFServices.Parameter[]
                           {
                                   new SFFServices.Parameter() { Name = "mobileNo", Value = query.mobileNo },
                                   new SFFServices.Parameter() { Name = "option", Value = query.option },
                           },
                        ParameterType = "String"
                    };
                    objReq.Event = "evESQueryPersonalInformation";
                }


                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, objReq, query.mobileNo, "evESQueryPersonalInformation", "evESQueryPersonalInformation", query.mobileNo, "FBB|" + query.FullUrl, "");

                using (var service = new SFFServices.SFFServiceService())
                {
                    var data = service.ExecuteService(objReq);

                    //var data2 = "<?xml version='1.0' encoding='utf-16'?> <SffResponse xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'> <ErrorMessage xmlns='java:th.co.ais.sff.domain.gm.vo.ws'/> <ParameterList xmlns='java:th.co.ais.sff.domain.gm.vo.ws'> <ParameterType xsi:nil='true'/> <Parameter> <Name>maxCounterOntop</Name> <Value>5</Value> </Parameter> <Parameter> <Name>remainCounterOntop</Name> <Value>5</Value> </Parameter> <Parameter> <Name>maxCounterMain</Name> <Value>3</Value> </Parameter> <Parameter> <Name>remainCounterMain</Name> <Value>3</Value> </Parameter> <ParameterList> <ParameterType xsi:nil='true'/> <Parameter> <Name>paymentMode</Name> <Value>Post-paid</Value> </Parameter> <Parameter> <Name>deviceContractFlg</Name> <Value>N</Value> </Parameter> <Parameter> <Name>productPkg</Name> <Value xsi:nil='true'/> </Parameter> <Parameter> <Name>remark</Name> <Value xsi:nil='true'/> </Parameter> <Parameter> <Name>descEng</Name> <Value>AIS PLAYBOX Bundling for box No. 1 for 24 Bill Cycles</Value> </Parameter> <Parameter> <Name>crmFlg</Name> <Value>N</Value> </Parameter> <Parameter> <Name>priceExclVat</Name> <Value>0.00</Value> </Parameter> <Parameter> <Name>pro5gflg</Name> <Value xsi:nil='true'/> </Parameter> <Parameter> <Name>productClass</Name> <Value>On-Top Extra</Value> </Parameter> <Parameter> <Name>netFlexiFlg</Name> <Value>N</Value> </Parameter> <Parameter> <Name>integrationName</Name> <Value>AIS PLAYBOX ATV Bundling 24 months for box no.1</Value> </Parameter> <Parameter> <Name>productAcctnCat</Name> <Value>R</Value> </Parameter> <Parameter> <Name>productCd</Name> <Value>P211009229</Value> </Parameter> <Parameter> <Name>dlRate</Name> <Value xsi:nil='true'/> </Parameter> <Parameter> <Name>shortNameEng</Name> <Value>AIS PLAYBOX Bundling for box No. 1</Value> </Parameter> <Parameter> <Name>startDt</Name> <Value>15/03/2022 17:43:30</Value> </Parameter> <Parameter> <Name>promotionName</Name> <Value>AIS PLAYBOX ATV Bundling 24 months for box no.1</Value> </Parameter> <Parameter> <Name>inStatementEng</Name> <Value>AIS PLAYBOX Bundling for box No. 1 for 24 Bill Cycles</Value> </Parameter> <Parameter> <Name>descThai</Name> <Value>ฟรีค่าบริการรายเดือน AIS PLAYBOX จุดที่ 1 นาน 24 รอบบิล</Value> </Parameter> <Parameter> <Name>shortNameThai</Name> <Value>ฟรีค่าบริการรายเดือน AIS PLAYBOX จุดที่ 1</Value> </Parameter> <Parameter> <Name>productSeq</Name> <Value>109</Value> </Parameter> <Parameter> <Name>priceInclVat</Name> <Value>0.00</Value> </Parameter> <Parameter> <Name>inStatementThai</Name> <Value>ฟรีค่าบริการรายเดือน AIS PLAYBOX จุดที่ 1 นาน 24 รอบบิล</Value> </Parameter> <Parameter> <Name>phxProductId</Name> <Value>O2110P211009229</Value> </Parameter> <Parameter> <Name>endDt</Name> <Value>16/03/2024 00:00:00</Value> </Parameter> <Parameter> <Name>bosId</Name> <Value xsi:nil='true'/> </Parameter> <Parameter> <Name>nextBillCycle</Name> <Value/> </Parameter> <Parameter> <Name>priceType</Name> <Value>Recurring</Value> </Parameter> <Parameter> <Name>prorateFlg</Name> <Value xsi:nil='true'/> </Parameter> <Parameter> <Name>url</Name> <Value>N</Value> </Parameter> <Parameter> <Name>monthlyFee</Name> <Value>0.00</Value> </Parameter> <Parameter> <Name>ulRate</Name> <Value xsi:nil='true'/> </Parameter> <Parameter> <Name>produuctGroup</Name> <Value>Monthly Fee</Value> </Parameter> <Parameter> <Name>bvPoint</Name> <Value xsi:nil='true'/> </Parameter> <Parameter> <Name>billCycle</Name> <Value/> </Parameter> <Parameter> <Name>bvDescription</Name> <Value xsi:nil='true'/> </Parameter> </ParameterList> <ParameterList> <ParameterType xsi:nil='true'/> <Parameter> <Name>paymentMode</Name> <Value>Post-paid</Value> </Parameter> <Parameter> <Name>deviceContractFlg</Name> <Value>N</Value> </Parameter> <Parameter> <Name>productPkg</Name> <Value>Penalty Package</Value> </Parameter> <Parameter> <Name>remark</Name> <Value xsi:nil='true'/> </Parameter> <Parameter> <Name>descEng</Name> <Value>Installation promotion charge 4,800 baht in case terminate the contract before 24 months. (Prorate)</Value> </Parameter> <Parameter> <Name>crmFlg</Name> <Value>N</Value> </Parameter> <Parameter> <Name>priceExclVat</Name> <Value>4485.98</Value> </Parameter> <Parameter> <Name>pro5gflg</Name> <Value>N</Value> </Parameter> <Parameter> <Name>productClass</Name> <Value>On-Top Extra</Value> </Parameter> <Parameter> <Name>netFlexiFlg</Name> <Value>N</Value> </Parameter> <Parameter> <Name>integrationName</Name> <Value>Installation Promotion Charge FTTH 4,800 THB (Prorate) - Contract 24 months</Value> </Parameter> <Parameter> <Name>productAcctnCat</Name> <Value>R</Value> </Parameter> <Parameter> <Name>productCd</Name> <Value>P19087344</Value> </Parameter> <Parameter> <Name>dlRate</Name> <Value xsi:nil='true'/> </Parameter> <Parameter> <Name>shortNameEng</Name> <Value>Discount for installation fee 4,800 THB (Prorate) for contract 24 months.</Value> </Parameter> <Parameter> <Name>startDt</Name> <Value>15/03/2022 17:43:40</Value> </Parameter> <Parameter> <Name>promotionName</Name> <Value>Installation Promotion Charge FTTH 4,800 THB (Prorate) - Contract 24 months</Value> </Parameter> <Parameter> <Name>inStatementEng</Name> <Value>Installation promotion charge 4,800 baht in case terminate the contract before 24 months. (Prorate)</Value> </Parameter> <Parameter> <Name>descThai</Name> <Value>เรียกคืนส่วนลดค่าติดตั้ง 4,800 บาท กรณียกเลิกก่อนครบอายุสัญญา 24 เดือน (Prorate)</Value> </Parameter> <Parameter> <Name>shortNameThai</Name> <Value>ส่วนลดค่าติดตั้ง 4,800 บาท (Prorate) เมื่อใช้บริการครบ 24 รอบบิล</Value> </Parameter> <Parameter> <Name>productSeq</Name> <Value xsi:nil='true'/> </Parameter> <Parameter> <Name>priceInclVat</Name> <Value>4800.00</Value> </Parameter> <Parameter> <Name>inStatementThai</Name> <Value>เรียกคืนส่วนลดค่าติดตั้ง 4,800 บาท กรณียกเลิกก่อนครบอายุสัญญา 24 เดือน (Prorate)</Value> </Parameter> <Parameter> <Name>phxProductId</Name> <Value>O1908P19087344</Value> </Parameter> <Parameter> <Name>endDt</Name> <Value>15/03/2024 17:43:27</Value> </Parameter> <Parameter> <Name>bosId</Name> <Value xsi:nil='true'/> </Parameter> <Parameter> <Name>nextBillCycle</Name> <Value/> </Parameter> <Parameter> <Name>priceType</Name> <Value>Usage</Value> </Parameter> <Parameter> <Name>prorateFlg</Name> <Value>N</Value> </Parameter> <Parameter> <Name>url</Name> <Value>https://fastsearch.ais.co.th/CSM-KB/AIS/Pages/results.aspx?a=.aspx&amp;k=P19087344</Value> </Parameter> <Parameter> <Name>monthlyFee</Name> <Value>4800.00</Value> </Parameter> <Parameter> <Name>ulRate</Name> <Value xsi:nil='true'/> </Parameter> <Parameter> <Name>produuctGroup</Name> <Value>Monthly Fee</Value> </Parameter> <Parameter> <Name>bvPoint</Name> <Value xsi:nil='true'/> </Parameter> <Parameter> <Name>billCycle</Name> <Value/> </Parameter> <Parameter> <Name>bvDescription</Name> <Value xsi:nil='true'/> </Parameter> </ParameterList> <ParameterList> <ParameterType xsi:nil='true'/> <Parameter> <Name>paymentMode</Name> <Value>Post-paid</Value> </Parameter> <Parameter> <Name>deviceContractFlg</Name> <Value>N</Value> </Parameter> <Parameter> <Name>productPkg</Name> <Value>FBB_DISCOUNT</Value> </Parameter> <Parameter> <Name>remark</Name> <Value xsi:nil='true'/> </Parameter> <Parameter> <Name>descEng</Name> <Value>AIS Fibre discount 50% for 24 months</Value> </Parameter> <Parameter> <Name>crmFlg</Name> <Value>N</Value> </Parameter> <Parameter> <Name>priceExclVat</Name> <Value>0.00</Value> </Parameter> <Parameter> <Name>pro5gflg</Name> <Value xsi:nil='true'/> </Parameter> <Parameter> <Name>productClass</Name> <Value>On-Top</Value> </Parameter> <Parameter> <Name>netFlexiFlg</Name> <Value>N</Value> </Parameter> <Parameter> <Name>integrationName</Name> <Value>FBB Purge Discount 50% for 24 months</Value> </Parameter> <Parameter> <Name>productAcctnCat</Name> <Value>R</Value> </Parameter> <Parameter> <Name>productCd</Name> <Value>P211109718</Value> </Parameter> <Parameter> <Name>dlRate</Name> <Value xsi:nil='true'/> </Parameter> <Parameter> <Name>shortNameEng</Name> <Value>FBB Purge Discount 50% for 24 months</Value> </Parameter> <Parameter> <Name>startDt</Name> <Value>15/03/2022 17:43:30</Value> </Parameter> <Parameter> <Name>promotionName</Name> <Value>FBB Purge Discount 50% for 24 months</Value> </Parameter> <Parameter> <Name>inStatementEng</Name> <Value>AIS Fibre discount 50% for 24 months</Value> </Parameter> <Parameter> <Name>descThai</Name> <Value>ส่วนลดแพ็กเกจ AIS Fibre 50% นาน 24 เดือน</Value> </Parameter> <Parameter> <Name>shortNameThai</Name> <Value>FBB Purge Discount 50% for 24 months</Value> </Parameter> <Parameter> <Name>productSeq</Name> <Value>107#108</Value> </Parameter> <Parameter> <Name>priceInclVat</Name> <Value>0.00</Value> </Parameter> <Parameter> <Name>inStatementThai</Name> <Value>ส่วนลดแพ็กเกจ AIS Fibre 50% นาน 24 เดือน</Value> </Parameter> <Parameter> <Name>phxProductId</Name> <Value>O2111P211109718</Value> </Parameter> <Parameter> <Name>endDt</Name> <Value>16/03/2024 00:00:00</Value> </Parameter> <Parameter> <Name>bosId</Name> <Value xsi:nil='true'/> </Parameter> <Parameter> <Name>nextBillCycle</Name> <Value/> </Parameter> <Parameter> <Name>priceType</Name> <Value>Recurring</Value> </Parameter> <Parameter> <Name>prorateFlg</Name> <Value xsi:nil='true'/> </Parameter> <Parameter> <Name>url</Name> <Value>N</Value> </Parameter> <Parameter> <Name>monthlyFee</Name> <Value>0.00</Value> </Parameter> <Parameter> <Name>ulRate</Name> <Value xsi:nil='true'/> </Parameter> <Parameter> <Name>produuctGroup</Name> <Value>Monthly Fee</Value> </Parameter> <Parameter> <Name>bvPoint</Name> <Value xsi:nil='true'/> </Parameter> <Parameter> <Name>billCycle</Name> <Value/> </Parameter> <Parameter> <Name>bvDescription</Name> <Value xsi:nil='true'/> </Parameter> </ParameterList> <ParameterList> <ParameterType xsi:nil='true'/> <Parameter> <Name>paymentMode</Name> <Value>Post-paid</Value> </Parameter> <Parameter> <Name>deviceContractFlg</Name> <Value>N</Value> </Parameter> <Parameter> <Name>productPkg</Name> <Value>AIS_ON_AIR</Value> </Parameter> <Parameter> <Name>remark</Name> <Value xsi:nil='true'/> </Parameter> <Parameter> <Name>descEng</Name> <Value>PLAY FAMILY Package free 24 months. Enjoy movies, series, varieties, cartoons and news from premium channels from AIS PLAY.</Value> </Parameter> <Parameter> <Name>crmFlg</Name> <Value>N</Value> </Parameter> <Parameter> <Name>priceExclVat</Name> <Value>0.00</Value> </Parameter> <Parameter> <Name>pro5gflg</Name> <Value xsi:nil='true'/> </Parameter> <Parameter> <Name>productClass</Name> <Value>On-Top</Value> </Parameter> <Parameter> <Name>netFlexiFlg</Name> <Value xsi:nil='true'/> </Parameter> <Parameter> <Name>integrationName</Name> <Value>NGB_PLAY FAMILY_ATV_Free_24Months</Value> </Parameter> <Parameter> <Name>productAcctnCat</Name> <Value>R</Value> </Parameter> <Parameter> <Name>productCd</Name> <Value>P211109688</Value> </Parameter> <Parameter> <Name>dlRate</Name> <Value xsi:nil='true'/> </Parameter> <Parameter> <Name>shortNameEng</Name> <Value>PLAY FAMILY Package free 24 months</Value> </Parameter> <Parameter> <Name>startDt</Name> <Value>15/03/2022 17:43:30</Value> </Parameter> <Parameter> <Name>promotionName</Name> <Value>NGB_PLAY FAMILY_ATV_Free_24Months</Value> </Parameter> <Parameter> <Name>inStatementEng</Name> <Value>PLAY FAMILY Package free 24 months. Enjoy movies, series, varieties, cartoons and news from premium channels from AIS PLAY.</Value> </Parameter> <Parameter> <Name>descThai</Name> <Value>แพ็กเกจ PLAY FAMILY ฟรีนาน 24 เดือน ดูหนัง ซีรีส์ วาไรตี้ การ์ตูน และ ข่าว จากช่องดังระดับโลก จาก AIS PLAY</Value> </Parameter> <Parameter> <Name>shortNameThai</Name> <Value>แพ็กเกจ PLAY FAMILY ฟรีนาน 24 เดือน</Value> </Parameter> <Parameter> <Name>productSeq</Name> <Value>100</Value> </Parameter> <Parameter> <Name>priceInclVat</Name> <Value>0.00</Value> </Parameter> <Parameter> <Name>inStatementThai</Name> <Value>แพ็กเกจ PLAY FAMILY ฟรีนาน 24 เดือน ดูหนัง ซีรีส์ วาไรตี้ การ์ตูน และ ข่าว จากช่องดังระดับโลก จาก AIS PLAY</Value> </Parameter> <Parameter> <Name>phxProductId</Name> <Value>O2111P211109688</Value> </Parameter> <Parameter> <Name>endDt</Name> <Value>15/03/2024 00:00:00</Value> </Parameter> <Parameter> <Name>bosId</Name> <Value xsi:nil='true'/> </Parameter> <Parameter> <Name>nextBillCycle</Name> <Value/> </Parameter> <Parameter> <Name>priceType</Name> <Value>Recurring</Value> </Parameter> <Parameter> <Name>prorateFlg</Name> <Value>N</Value> </Parameter> <Parameter> <Name>url</Name> <Value xsi:nil='true'/> </Parameter> <Parameter> <Name>monthlyFee</Name> <Value>0.00</Value> </Parameter> <Parameter> <Name>ulRate</Name> <Value xsi:nil='true'/> </Parameter> <Parameter> <Name>produuctGroup</Name> <Value>VAS Promotion</Value> </Parameter> <Parameter> <Name>bvPoint</Name> <Value xsi:nil='true'/> </Parameter> <Parameter> <Name>billCycle</Name> <Value/> </Parameter> <Parameter> <Name>bvDescription</Name> <Value xsi:nil='true'/> </Parameter> </ParameterList> <ParameterList> <ParameterType xsi:nil='true'/> <Parameter> <Name>paymentMode</Name> <Value>Post-paid</Value> </Parameter> <Parameter> <Name>deviceContractFlg</Name> <Value>N</Value> </Parameter> <Parameter> <Name>productPkg</Name> <Value xsi:nil='true'/> </Parameter> <Parameter> <Name>remark</Name> <Value xsi:nil='true'/> </Parameter> <Parameter> <Name>descEng</Name> <Value>POWER4 GIGA Special II 1000/500 Mbps Monthly fee 1,059 THB bundled with AIS PLAYBOX, get AIS postpaid unlimited Internet at max speed of 10 Mbps, voice 200 mins</Value> </Parameter> <Parameter> <Name>crmFlg</Name> <Value>N</Value> </Parameter> <Parameter> <Name>priceExclVat</Name> <Value>1059.00</Value> </Parameter> <Parameter> <Name>pro5gflg</Name> <Value xsi:nil='true'/> </Parameter> <Parameter> <Name>productClass</Name> <Value>Main</Value> </Parameter> <Parameter> <Name>netFlexiFlg</Name> <Value>N</Value> </Parameter> <Parameter> <Name>integrationName</Name> <Value>POWER4 GIGA Special II 1000/500 Mbps 1,059 THB 24 months</Value> </Parameter> <Parameter> <Name>productAcctnCat</Name> <Value>R</Value> </Parameter> <Parameter> <Name>productCd</Name> <Value>P220110464</Value> </Parameter> <Parameter> <Name>dlRate</Name> <Value>1000</Value> </Parameter> <Parameter> <Name>shortNameEng</Name> <Value>POWER4 GIGA Special II 1000/500 Mbps 1,059/M 24M</Value> </Parameter> <Parameter> <Name>startDt</Name> <Value>15/03/2022 17:43:30</Value> </Parameter> <Parameter> <Name>promotionName</Name> <Value>POWER4 GIGA Special II 1000/500 Mbps 1,059 THB 24 months</Value> </Parameter> <Parameter> <Name>inStatementEng</Name> <Value>POWER4 GIGA Special II 1000/500 Mbps Monthly fee 1,059 THB bundled with AIS PLAYBOX, get AIS postpaid unlimited Internet at max speed of 10 Mbps, voice 200 mins</Value> </Parameter> <Parameter> <Name>descThai</Name> <Value>แพ็กเกจ POWER4 GIGA Special II 1000/500 Mbps ค่าบริการรายเดือน 1,059 บาท พร้อมกล่อง AIS PLAYBOX และเน็ตมือถือไม่จำกัด ความเร็วสูงสุดไม่เกิน 10 Mbps, โทรฟรี 200 นาที</Value> </Parameter> <Parameter> <Name>shortNameThai</Name> <Value>POWER4 GIGA Special II 1000/500 Mbps 1,059/M 24M</Value> </Parameter> <Parameter> <Name>productSeq</Name> <Value>106</Value> </Parameter> <Parameter> <Name>priceInclVat</Name> <Value>1133.13</Value> </Parameter> <Parameter> <Name>inStatementThai</Name> <Value>แพ็กเกจ POWER4 GIGA Special II 1000/500 Mbps ค่าบริการรายเดือน 1,059 บาท พร้อมกล่อง AIS PLAYBOX และเน็ตมือถือไม่จำกัด ความเร็วสูงสุดไม่เกิน 10 Mbps, โทรฟรี 200 นาที</Value> </Parameter> <Parameter> <Name>phxProductId</Name> <Value>O2201P220110464</Value> </Parameter> <Parameter> <Name>endDt</Name> <Value>16/03/2024 00:00:00</Value> </Parameter> <Parameter> <Name>bosId</Name> <Value xsi:nil='true'/> </Parameter> <Parameter> <Name>nextBillCycle</Name> <Value/> </Parameter> <Parameter> <Name>priceType</Name> <Value>Recurring</Value> </Parameter> <Parameter> <Name>prorateFlg</Name> <Value xsi:nil='true'/> </Parameter> <Parameter> <Name>url</Name> <Value>N</Value> </Parameter> <Parameter> <Name>monthlyFee</Name> <Value>1133.13</Value> </Parameter> <Parameter> <Name>ulRate</Name> <Value>500</Value> </Parameter> <Parameter> <Name>produuctGroup</Name> <Value>Monthly Fee</Value> </Parameter> <Parameter> <Name>bvPoint</Name> <Value xsi:nil='true'/> </Parameter> <Parameter> <Name>billCycle</Name> <Value/> </Parameter> <Parameter> <Name>bvDescription</Name> <Value xsi:nil='true'/> </Parameter> <ParameterList> <ParameterType xsi:nil='true'/> <Parameter> <Name>fnName</Name> <Value>playboxBundling</Value> </Parameter> <Parameter> <Name>fnValue</Name> <Value>1</Value> </Parameter> <Parameter> <Name>fnHiddenFlg</Name> <Value>N</Value> </Parameter> </ParameterList> </ParameterList> </ParameterList> </SffResponse> ";

                    if (data != null)
                    {
                        InterfaceLogCommand log2 = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, objReq, query.mobileNo, "evESQueryPersonalInformation", "evESQueryPersonalInformationData", query.mobileNo, "FBB|" + query.FullUrl, "");
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, data, log2, "Success", "", "");

                        if (data.ErrorMessage != null && data.ErrorMessage != "")
                        {
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Failed", data.ErrorMessage.ToSafeString(), "");
                        }
                        else
                        {
                            if (data.ParameterList != null && data.ParameterList.Parameter != null)
                            {
                                evESQueryPersonalInformationModel subresult = new evESQueryPersonalInformationModel();
                                foreach (var item in data.ParameterList.Parameter)
                                {
                                    PropertyInfo propInfo = subresult.GetType().GetProperty(item.Name);
                                    if (propInfo != null)
                                    {
                                        if (item.Value == "" && propInfo.PropertyType == typeof(decimal))
                                        {
                                            item.Value = "0";
                                        }
                                        propInfo.SetValue(subresult, Convert.ChangeType(item.Value, propInfo.PropertyType), null);
                                    }
                                }
                                result.Add(subresult);
                            }

                            if (data.ParameterList != null && data.ParameterList.ParameterList1 != null)
                            {
                                foreach (var item in data.ParameterList.ParameterList1)
                                {
                                    evESQueryPersonalInformationModel subresult = new evESQueryPersonalInformationModel();
                                    foreach (var subitem in item.Parameter)
                                    {
                                        PropertyInfo propInfo = subresult.GetType().GetProperty(subitem.Name);
                                        if (propInfo != null)
                                        {
                                            if (subitem.Value == "" && propInfo.PropertyType == typeof(decimal))
                                            {
                                                subitem.Value = "0";
                                            }
                                            propInfo.SetValue(subresult, Convert.ChangeType(subitem.Value, propInfo.PropertyType), null);
                                        }
                                    }
                                    result.Add(subresult);
                                }
                            }
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "", "");
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                SffServiceConseHelper.EndInterfaceSffLog(_uow, _intfLog, result, log, "Failed", ex.ToSafeString());
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Failed", ex.ToSafeString(), "");
                return result;
            }
        }
    }

}
