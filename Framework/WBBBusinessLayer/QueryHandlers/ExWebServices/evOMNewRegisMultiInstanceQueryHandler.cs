using System;
using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.ExWebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.ExWebServiceModels;
namespace WBBBusinessLayer.QueryHandlers.ExWebServices
{
    public class evOMNewRegisMultiInstanceQueryHandler : IQueryHandler<evOMNewRegisMultiInstanceQuery, evOMNewRegisMultiInstanceModel>
    {

        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_LOV> _lovService;
        private readonly IEntityRepository<FBB_SFF_CHKPROFILE_LOG> _sffLog;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CUST_PACKAGE> _custPackage;
        private readonly IEntityRepository<FBB_CUST_PROFILE> _custProfile;
        private readonly IEntityRepository<FBB_REGISTER> _register;
        private readonly IEntityRepository<FBB_PACKAGE_TRAN> _packageTran;
        private readonly IWBBUnitOfWork _uow;

        public evOMNewRegisMultiInstanceQueryHandler(ILogger logger,
            IEntityRepository<FBB_CFG_LOV> lovService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_SFF_CHKPROFILE_LOG> sffLog, IWBBUnitOfWork uow,
            IEntityRepository<FBB_CUST_PACKAGE> custPackage,
            IEntityRepository<FBB_CUST_PROFILE> custProfile,
            IEntityRepository<FBB_REGISTER> register,
            IEntityRepository<FBB_PACKAGE_TRAN> packageTran)
        {
            _logger = logger;
            _lovService = lovService;
            _sffLog = sffLog;
            _intfLog = intfLog;
            _uow = uow;
            _custPackage = custPackage;
            _custProfile = custProfile;
            _register = register;
            _packageTran = packageTran;
        }

        public evOMNewRegisMultiInstanceModel Handle(evOMNewRegisMultiInstanceQuery query)
        {
            InterfaceLogCommand log = null;
            var result = new evOMNewRegisMultiInstanceModel();
            IEntityRepository<FBB_SFF_CHKPROFILE_LOG> sffLog;
            try
            {
                var request = new SFFServices.SffRequest();
                // request.Event = "evOMNewRegisMultiInstance";

                #region Map Input Parameter

                var objReqParam = new SFFServices.Parameter[]
                {
                    new SFFServices.Parameter() { Name = "referenceNo", Value = query.referenceNo.ToSafeString() },
                    new SFFServices.Parameter() { Name = "accountCat", Value = query.accountCat.ToSafeString() },
                    new SFFServices.Parameter() { Name = "accountSubCat", Value = query.accountSubCat.ToSafeString() },
                    new SFFServices.Parameter() { Name = "idCardType", Value = query.idCardType.ToSafeString() },
                    new SFFServices.Parameter() { Name = "idCardNo", Value = query.idCardNo.ToSafeString() },
                    new SFFServices.Parameter() { Name = "titleName", Value = query.titleName.ToSafeString() },
                    new SFFServices.Parameter() { Name = "firstName", Value = query.firstName.ToSafeString() },
                    new SFFServices.Parameter() { Name = "lastName", Value = query.lastName.ToSafeString() },
                    new SFFServices.Parameter() { Name = "saName", Value = query.saName.ToSafeString() },
                    new SFFServices.Parameter() { Name = "baName", Value = query.baName.ToSafeString() },
                    new SFFServices.Parameter() { Name = "caNumber", Value = query.caNumber.ToSafeString() },
                    new SFFServices.Parameter() { Name = "baNumber", Value = query.baNumber.ToSafeString() },
                    new SFFServices.Parameter() { Name = "saNumber", Value = query.saNumber.ToSafeString() },
                    new SFFServices.Parameter() { Name = "birthdate", Value = query.birthdate.ToSafeString() },
                    new SFFServices.Parameter() { Name = "gender", Value = query.gender.ToSafeString() },
                    new SFFServices.Parameter() { Name = "billName", Value = query.billName.ToSafeString() },
                    new SFFServices.Parameter() { Name = "billCycle", Value = query.billCycle.ToSafeString() },
                    new SFFServices.Parameter() { Name = "billLanguage", Value = query.billLanguage.ToSafeString() },
                    new SFFServices.Parameter() { Name = "engFlag", Value = query.engFlag.ToSafeString() },
                    new SFFServices.Parameter() { Name = "accHomeNo", Value = query.accHomeNo.ToSafeString() },
                    new SFFServices.Parameter() { Name = "accBuildingName", Value = query.accBuildingName.ToSafeString() },
                    new SFFServices.Parameter() { Name = "accFloor", Value = query.accFloor.ToSafeString() },
                    new SFFServices.Parameter() { Name = "accRoom", Value = query.accRoom.ToSafeString() },
                    new SFFServices.Parameter() { Name = "accMoo", Value = query.accMoo.ToSafeString() },
                    new SFFServices.Parameter() { Name = "accMooBan", Value = query.accMooBan.ToSafeString() },
                    new SFFServices.Parameter() { Name = "accSoi", Value = query.accSoi.ToSafeString() },
                    new SFFServices.Parameter() { Name = "accStreet", Value = query.accStreet.ToSafeString() },
                    new SFFServices.Parameter() { Name = "accTumbol", Value = query.accTumbol.ToSafeString() },
                    new SFFServices.Parameter() { Name = "accAmphur", Value = query.accAmphur.ToSafeString() },
                    new SFFServices.Parameter() { Name = "accProvince", Value = query.accProvince.ToSafeString() },
                    new SFFServices.Parameter() { Name = "accZipCode", Value = query.accZipCode.ToSafeString() },
                    new SFFServices.Parameter() { Name = "billHomeNo", Value = query.billHomeNo.ToSafeString() },
                    new SFFServices.Parameter() { Name = "billBuildingName", Value = query.billBuildingName.ToSafeString() },
                    new SFFServices.Parameter() { Name = "billFloor", Value = query.billFloor.ToSafeString() },
                    new SFFServices.Parameter() { Name = "billRoom", Value = query.billRoom.ToSafeString() },
                    new SFFServices.Parameter() { Name = "billMoo", Value = query.billMoo.ToSafeString() },
                    new SFFServices.Parameter() { Name = "billMooBan", Value = query.billMooBan.ToSafeString() },
                    new SFFServices.Parameter() { Name = "billSoi", Value = query.billSoi.ToSafeString() },
                    new SFFServices.Parameter() { Name = "billStreet", Value = query.billStreet.ToSafeString() },
                    new SFFServices.Parameter() { Name = "billTumbol", Value = query.billTumbol.ToSafeString() },
                    new SFFServices.Parameter() { Name = "billAmphur", Value = query.billAmphur.ToSafeString() },
                    new SFFServices.Parameter() { Name = "billProvince", Value = query.billProvince.ToSafeString() },
                    new SFFServices.Parameter() { Name = "billZipCode", Value = query.billZipCode.ToSafeString() },
                    new SFFServices.Parameter() { Name = "userId", Value = query.userId.ToSafeString() },
                    new SFFServices.Parameter() { Name = "dealerLocationCode", Value = query.dealerLocationCode.ToSafeString() },
                    new SFFServices.Parameter() { Name = "ascCode", Value = query.ascCode.ToSafeString() },
                    new SFFServices.Parameter() { Name = "orderReason", Value = query.orderReason.ToSafeString() },
                    new SFFServices.Parameter() { Name = "remark", Value = query.remark.ToSafeString() },
                    new SFFServices.Parameter() { Name = "saVatName", Value = query.saVatName.ToSafeString() },
                    new SFFServices.Parameter() { Name = "saVatAddress1", Value = query.saVatAddress1.ToSafeString() },
                    new SFFServices.Parameter() { Name = "saVatAddress2", Value = query.saVatAddress2.ToSafeString() },
                    new SFFServices.Parameter() { Name = "saVatAddress3", Value = query.saVatAddress3.ToSafeString() },
                    new SFFServices.Parameter() { Name = "saVatAddress4", Value = query.saVatAddress4.ToSafeString() },
                    new SFFServices.Parameter() { Name = "saVatAddress5", Value = query.saVatAddress5.ToSafeString() },
                    new SFFServices.Parameter() { Name = "saVatAddress6", Value = query.saVatAddress6.ToSafeString() },
                    new SFFServices.Parameter() { Name = "contactFirstName", Value = query.contactFirstName.ToSafeString() },
                    new SFFServices.Parameter() { Name = "contactLastName", Value = query.contactLastName.ToSafeString() },
                    new SFFServices.Parameter() { Name = "contactTitle", Value = query.contactTitle.ToSafeString() },
                    new SFFServices.Parameter() { Name = "mobileNumberContact", Value = query.mobileNumberContact.ToSafeString() },
                    new SFFServices.Parameter() { Name = "phoneNumberContact", Value = query.phoneNumberContact.ToSafeString() },
                    new SFFServices.Parameter() { Name = "emailAddress", Value = query.emailAddress.ToSafeString() },
                    new SFFServices.Parameter() { Name = "saHomeNo", Value = query.saHomeNo.ToSafeString() },
                    new SFFServices.Parameter() { Name = "saBuildingName", Value = query.saBuildingName.ToSafeString() },
                    new SFFServices.Parameter() { Name = "saFloor", Value = query.saFloor.ToSafeString() },
                    new SFFServices.Parameter() { Name = "saRoom", Value = query.saRoom.ToSafeString() },
                    new SFFServices.Parameter() { Name = "saMoo", Value = query.saMoo.ToSafeString() },
                    new SFFServices.Parameter() { Name = "saMooBan", Value = query.saMooBan.ToSafeString() },
                    new SFFServices.Parameter() { Name = "saSoi", Value = query.saSoi.ToSafeString() },
                    new SFFServices.Parameter() { Name = "saStreet", Value = query.saStreet.ToSafeString() },
                    new SFFServices.Parameter() { Name = "saTumbol", Value = query.saTumbol.ToSafeString() },
                    new SFFServices.Parameter() { Name = "saAmphur", Value = query.saAmphur.ToSafeString() },
                    new SFFServices.Parameter() { Name = "saProvince", Value = query.saProvince.ToSafeString() },
                    new SFFServices.Parameter() { Name = "saZipCode", Value = query.saZipCode.ToSafeString() },
                    new SFFServices.Parameter() { Name = "orderType", Value = query.orderType.ToSafeString() },
                    new SFFServices.Parameter() { Name = "channel", Value = query.channel.ToSafeString() },
                    new SFFServices.Parameter() { Name = "projectName", Value = query.projectName.ToSafeString() },
                    new SFFServices.Parameter() { Name = "caBranchNo", Value = query.caBranchNo.ToSafeString() },
                    new SFFServices.Parameter() { Name = "saBranchNo", Value = query.saBranchNo.ToSafeString() },
                    new SFFServices.Parameter() { Name = "subcontractor", Value = query.subcontractor.ToSafeString() },
                    new SFFServices.Parameter() { Name = "installStaffID", Value = query.installStaffID.ToSafeString() },
                    new SFFServices.Parameter() { Name = "employeeID", Value = query.employeeID.ToSafeString() },
                    new SFFServices.Parameter() { Name = "sourceSystem", Value = query.sourceSystem.ToSafeString()},
                    new SFFServices.Parameter() { Name = "billMedia", Value = query.billMedia.ToSafeString()},
                }
               .ToList();


                var listoflist = new List<SFFServices.ParameterList>();
                SFFServices.ParameterList plist = new SFFServices.ParameterList()
                {
                    Parameter = new SFFServices.Parameter[] //List INSTANCE
                        {
                            new SFFServices.Parameter() { Name = "productInstance", Value =query.bulklistinstancecur[0].p_productInstance.ToSafeString()  },
                            new SFFServices.Parameter() { Name = "mobileNo", Value = query.bulklistinstancecur[0].p_mobileNo.ToSafeString() },
                            new SFFServices.Parameter() { Name = "simSerialNo", Value = query.bulklistinstancecur[0].p_simSerialNo.ToSafeString()},
                            new SFFServices.Parameter() { Name = "provinceCode", Value = query.bulklistinstancecur[0].p_provinceCode.ToSafeString()}
                        },

                };

                var SubParam = new List<SFFServices.ParameterList>();

                var paramVDSL = new SFFServices.ParameterList[query.bulkvdsl.Count];

                for (Int32 i = 0; i <= query.bulkvdsl.Count - 1; i++)//VDSL
                {
                    var objParamService = new List<SFFServices.Parameter>();
                    objParamService.Add(new SFFServices.Parameter() { Name = "serviceCode", Value = query.bulkvdsl[i].p_sff_product_cd.ToSafeString() });

                    var templist = new SFFServices.ParameterList();
                    var paramArray = new SFFServices.Parameter[27];
                    var param1 = new SFFServices.Parameter();
                    var param2 = new SFFServices.Parameter();
                    var param3 = new SFFServices.Parameter();
                    var param4 = new SFFServices.Parameter();
                    var param5 = new SFFServices.Parameter();
                    var param6 = new SFFServices.Parameter();
                    var param7 = new SFFServices.Parameter();
                    var param8 = new SFFServices.Parameter();
                    var param9 = new SFFServices.Parameter();
                    var param10 = new SFFServices.Parameter();
                    var param11 = new SFFServices.Parameter();
                    var param12 = new SFFServices.Parameter();
                    var param13 = new SFFServices.Parameter();
                    var param14 = new SFFServices.Parameter();
                    var param15 = new SFFServices.Parameter();
                    var param16 = new SFFServices.Parameter();
                    var param17 = new SFFServices.Parameter();
                    var param18 = new SFFServices.Parameter();
                    var param19 = new SFFServices.Parameter();
                    var param20 = new SFFServices.Parameter();
                    var param21 = new SFFServices.Parameter();

                    var param22 = new SFFServices.Parameter();
                    var param23 = new SFFServices.Parameter();
                    var param24 = new SFFServices.Parameter();
                    var param25 = new SFFServices.Parameter();
                    var param26 = new SFFServices.Parameter();
                    var param27 = new SFFServices.Parameter();

                    param1.Name = "dpName";
                    param1.Value = query.bulkvdsl[i].p_dpName.ToSafeString();

                    param2.Name = "dpPort";
                    param2.Value = query.bulkvdsl[i].p_dpPort.ToSafeString();

                    param3.Name = "dslamName";
                    param3.Value = query.bulkvdsl[i].p_dslamName.ToSafeString();

                    param4.Name = "dslamPort";
                    param4.Value = query.bulkvdsl[i].p_dslamPort.ToSafeString();

                    param5.Name = "ia";
                    param5.Value = query.bulkvdsl[i].p_ia.ToSafeString();

                    param6.Name = "installAddress1";
                    param6.Value = query.bulkvdsl[i].p_installAddress1.ToSafeString();

                    param7.Name = "installAddress2";
                    param7.Value = query.bulkvdsl[i].p_installAddress2.ToSafeString();

                    param8.Name = "installAddress3";
                    param8.Value = query.bulkvdsl[i].p_installAddress3.ToSafeString();

                    param9.Name = "installAddress4";
                    param9.Value = query.bulkvdsl[i].p_installAddress4.ToSafeString();

                    param10.Name = "installAddress5";
                    param10.Value = query.bulkvdsl[i].p_installAddress5.ToSafeString();

                    param11.Name = "latitude";
                    param11.Value = query.bulkvdsl[i].p_latitude.ToSafeString();

                    param12.Name = "longitude";
                    param12.Value = query.bulkvdsl[i].p_longitude.ToSafeString();

                    param13.Name = "networkProvider";
                    param13.Value = query.bulkvdsl[i].p_networkProvider.ToSafeString();

                    param14.Name = "orderNo";
                    param14.Value = query.bulkvdsl[i].p_orderNo.ToSafeString();

                    param15.Name = "password";
                    param15.Value = query.bulkvdsl[i].p_password.ToSafeString();

                    param16.Name = "relateNumber";
                    param16.Value = query.bulkvdsl[i].p_relateNumber.ToSafeString();

                    param17.Name = "type";
                    param17.Value = query.bulkvdsl[i].p_type.ToSafeString();

                    param18.Name = "contactName";
                    param18.Value = query.bulkvdsl[i].p_contactName.ToSafeString();

                    param19.Name = "contactMobilePhone";
                    param19.Value = query.bulkvdsl[i].p_contactMobilePhone.ToSafeString();

                    param20.Name = "addressId";
                    param20.Value = query.bulkvdsl[i].p_addressId.ToSafeString();

                    param21.Name = "flowFlag";
                    param21.Value = query.bulkvdsl[i].p_flowFlag.ToSafeString();

                    //new 2017/07/18
                    param22.Name = "p_fixip";
                    param22.Value = query.bulkvdsl[i].p_fixip.ToSafeString();

                    param23.Name = "p_appointmentdate";
                    param23.Value = query.bulkvdsl[i].p_appointmentdate.ToSafeString();

                    param24.Name = "p_installationCapacity";
                    param24.Value = query.bulkvdsl[i].p_installationCapacity.ToSafeString();

                    param25.Name = "p_phoneFlag";
                    param25.Value = query.bulkvdsl[i].p_phoneFlag.ToSafeString();

                    param26.Name = "p_reservedId";
                    param26.Value = query.bulkvdsl[i].p_reservedId.ToSafeString();

                    param27.Name = "p_timeSlot";
                    param27.Value = query.bulkvdsl[i].p_timeSlot.ToSafeString();

                    paramArray[0] = param1;
                    paramArray[1] = param2;
                    paramArray[2] = param3;
                    paramArray[3] = param4;
                    paramArray[4] = param5;
                    paramArray[5] = param6;
                    paramArray[6] = param7;
                    paramArray[7] = param8;
                    paramArray[8] = param9;
                    paramArray[9] = param10;
                    paramArray[10] = param11;
                    paramArray[11] = param12;
                    paramArray[12] = param13;
                    paramArray[13] = param14;
                    paramArray[14] = param15;
                    paramArray[15] = param16;
                    paramArray[16] = param17;
                    paramArray[17] = param18;
                    paramArray[18] = param19;
                    paramArray[19] = param20;
                    paramArray[20] = param21;

                    paramArray[21] = param22;
                    paramArray[22] = param23;
                    paramArray[23] = param24;
                    paramArray[24] = param25;
                    paramArray[25] = param26;
                    paramArray[26] = param27;

                    templist.Parameter = paramArray;
                    paramVDSL[i] = templist;

                    SFFServices.ParameterList paramVDSLList = new SFFServices.ParameterList()
                    {
                        Parameter = objParamService.ToArray(),
                        ParameterList1 = paramVDSL
                    };

                    SubParam.Add(paramVDSLList);
                }


                var paramVDSLRouterL = new SFFServices.ParameterList[query.bulkvdslrouter.Count];
                for (Int32 i = 0; i <= query.bulkvdslrouter.Count - 1; i++)//VDSL ROUTER
                {
                    var objParamServiceVDSLR = new List<SFFServices.Parameter>();
                    objParamServiceVDSLR.Add(new SFFServices.Parameter() { Name = "serviceCode", Value = query.bulkvdslrouter[i].p_sff_product_cd.ToSafeString() });

                    var templist = new SFFServices.ParameterList();
                    var paramArray = new SFFServices.Parameter[7];
                    var param1 = new SFFServices.Parameter();
                    var param2 = new SFFServices.Parameter();
                    var param3 = new SFFServices.Parameter();
                    var param4 = new SFFServices.Parameter();
                    var param5 = new SFFServices.Parameter();
                    var param6 = new SFFServices.Parameter();
                    var param7 = new SFFServices.Parameter();

                    param1.Name = "accessType";
                    param1.Value = query.bulkvdslrouter[i].p_accessType.ToSafeString();

                    param2.Name = "brand";
                    param2.Value = query.bulkvdslrouter[i].p_brand.ToSafeString();

                    param3.Name = "macAddress";
                    param3.Value = query.bulkvdslrouter[i].p_macAddress.ToSafeString();

                    param4.Name = "meterialCode";
                    param4.Value = query.bulkvdslrouter[i].p_meterialCode.ToSafeString();

                    param5.Name = "model";
                    param5.Value = query.bulkvdslrouter[i].p_model.ToSafeString();

                    param6.Name = "serialNo";
                    param6.Value = query.bulkvdslrouter[i].p_serialNo.ToSafeString();

                    param7.Name = "subContractor";
                    param7.Value = query.bulkvdslrouter[i].p_subContractor.ToSafeString();

                    paramArray[0] = param1;
                    paramArray[1] = param2;
                    paramArray[2] = param3;
                    paramArray[3] = param4;
                    paramArray[4] = param5;
                    paramArray[5] = param6;
                    paramArray[6] = param7;

                    templist.Parameter = paramArray;
                    paramVDSLRouterL[i] = templist;

                    SFFServices.ParameterList paramVDSLRouter = new SFFServices.ParameterList()
                    {
                        Parameter = objParamServiceVDSLR.ToArray(),
                        ParameterList1 = paramVDSLRouterL
                    };

                    SubParam.Add(paramVDSLRouter);
                }

                var paramAPPOINT = new SFFServices.ParameterList[query.bulkappoint.Count];
                for (Int32 i = 0; i <= query.bulkappoint.Count - 1; i++)//APPOINTMENT
                {
                    var objParamServiceAPPOINT = new List<SFFServices.Parameter>();
                    objParamServiceAPPOINT.Add(new SFFServices.Parameter() { Name = "serviceCode", Value = query.bulkappoint[i].p_sff_product_cd.ToSafeString() });

                    var templist = new SFFServices.ParameterList();
                    var paramArray = new SFFServices.Parameter[11];
                    var param1 = new SFFServices.Parameter();
                    var param2 = new SFFServices.Parameter();
                    var param3 = new SFFServices.Parameter();
                    var param4 = new SFFServices.Parameter();
                    var param5 = new SFFServices.Parameter();
                    var param6 = new SFFServices.Parameter();
                    var param7 = new SFFServices.Parameter();
                    var param8 = new SFFServices.Parameter();
                    var param9 = new SFFServices.Parameter();
                    var param10 = new SFFServices.Parameter();
                    var param11 = new SFFServices.Parameter();

                    param1.Name = "sysmptom";
                    param1.Value = query.bulkappoint[i].p_sysmptom.ToSafeString();

                    param2.Name = "appointmentDate";
                    param2.Value = query.bulkappoint[i].p_appointmentDate.ToSafeString();

                    param3.Name = "fbbContactNo1";
                    param3.Value = query.bulkappoint[i].p_fbbContactNo1.ToSafeString();

                    param4.Name = "fbbContactNo2";
                    param4.Value = query.bulkappoint[i].p_fbbContactNo2.ToSafeString();

                    param5.Name = "installationCapacity";
                    param5.Value = query.bulkappoint[i].p_installationCapacity.ToSafeString();

                    param6.Name = "playBoxAmount";
                    param6.Value = query.bulkappoint[i].p_playBoxAmount.ToSafeString();

                    param7.Name = "remarkForSubcontract";
                    param7.Value = query.bulkappoint[i].p_remarkForSubcontract.ToSafeString();

                    param8.Name = "reservedId";
                    param8.Value = query.bulkappoint[i].p_reservedId.ToSafeString();

                    param9.Name = "reservedPort";
                    param9.Value = query.bulkappoint[i].p_reservedPort.ToSafeString();

                    param10.Name = "timeslot";
                    param10.Value = query.bulkappoint[i].p_timeslot.ToSafeString();

                    param11.Name = "urgentFlag";
                    param11.Value = query.bulkappoint[i].p_urgentFlag.ToSafeString();

                    paramArray[0] = param1;
                    paramArray[1] = param2;
                    paramArray[2] = param3;
                    paramArray[3] = param4;
                    paramArray[4] = param5;
                    paramArray[5] = param6;
                    paramArray[6] = param7;
                    paramArray[7] = param8;
                    paramArray[8] = param9;
                    paramArray[9] = param10;
                    paramArray[10] = param11;

                    templist.Parameter = paramArray;
                    paramAPPOINT[i] = templist;

                    SFFServices.ParameterList paramAPPOINTM = new SFFServices.ParameterList()
                    {
                        Parameter = objParamServiceAPPOINT.ToArray(),
                        ParameterList1 = paramAPPOINT
                    };

                    SubParam.Add(paramAPPOINTM);
                }

                for (Int32 i = 0; i <= query.bulksffpromotioncur.Count - 1; i++)//Promotion Main
                {
                    var objParamProMain = new List<SFFServices.Parameter>();
                    objParamProMain.Add(new SFFServices.Parameter() { Name = "promotion", Value = query.bulksffpromotioncur[i].p_sff_product_cd.ToSafeString() });
                    SFFServices.ParameterList paramProMain = new SFFServices.ParameterList()
                    {
                        Parameter = objParamProMain.ToArray()
                    };

                    SubParam.Add(paramProMain);
                }

                for (Int32 i = 0; i <= query.bulksffpromotionontopcur.Count - 1; i++) //Promotion Ontop
                {
                    var objParamOntop = new List<SFFServices.Parameter>();
                    objParamOntop.Add(new SFFServices.Parameter() { Name = "promotion", Value = query.bulksffpromotionontopcur[i].p_sff_product_cd.ToSafeString() });
                    SFFServices.ParameterList paramOntop = new SFFServices.ParameterList()
                    {
                        Parameter = objParamOntop.ToArray()
                    };
                    SubParam.Add(paramOntop);
                }


                plist.ParameterList1 = SubParam.ToArray();

                listoflist.Add(plist);

                request = new SFFServices.SffRequest()
                {
                    Event = "evOMNewRegisMultiInstance",
                    ParameterList = new SFFServices.ParameterList()
                    {
                        Parameter = objReqParam.ToArray(),
                        ParameterList1 = listoflist.ToArray()
                    }
                };

                #endregion

                _logger.Info("Call evOMNewRegisMultiInstance SFF");
                // _logger.Info("");

                //Start InterfaceLog
                // log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, request, query.referenceNo, "evOMNewRegisMultiInstanceQuery", "evOMNewRegisMultiInstance", query.idCardNo, "FBB", "");

                using (var service = new SFFServices.SFFServiceService())
                {
                    var data = service.ExecuteService(request);

                    if (data != null)
                    {
                        if (data.ErrorMessage == null)
                        {
                            string returncode = "";
                            string retuenmsg = "";

                            var response = new SFFServices.SffResponse();
                            foreach (var itemData in data.ParameterList.Parameter)
                            {
                                if (itemData.Name == "result")
                                {
                                    returncode = itemData.Value;
                                }
                                else if (itemData.Name == "errorReason")
                                {
                                    retuenmsg = itemData.Value;
                                }
                            }

                            if (returncode == "Fail")
                            {
                                result.ReturnCode = "-1";
                                result.ReturnMessage = "Service Sff is " + retuenmsg;
                            }
                            else
                            {
                                result.ReturnCode = "0";
                                result.ReturnMessage = retuenmsg;
                            }
                        }
                    }
                }

                #region Insert FBB_SFF_CHKPROFILE_LOG

                //var chkProfileLog = new FBB_SFF_CHKPROFILE_LOG
                //{
                //    INAPPLICATION = "FBBDORM",
                //    //INMOBILENO = query.inMobileNo,
                //    //INIDCARDNO = query.inIDCardNo,
                //    //INIDCARDTYPE = query.inIDCardType,
                //    //OUTBUILDINGNAME = result.outBuildingName,
                //    //OUTFLOOR = result.outFloor,
                //    //OUTHOUSENUMBER = result.outHouseNumber,
                //    //OUTMOO = result.outMoo,
                //    //OUTSOI = result.outSoi,
                //    //OUTSTREETNAME = result.outStreetName,
                //    //OUTEMAIL = result.outEmail,
                //    //OUTPROVINCE = result.outProvince,
                //    //OUTAMPHUR = result.outAmphur,
                //    //OUTTUMBOL = result.outTumbol,
                //    //OUTACCOUNTNUMBER = result.outAccountNumber,
                //    //OUTSERVICEACCOUNTNUMBER = result.outServiceAccountNumber,
                //    //OUTBILLINGACCOUNTNUMBER = result.outBillingAccountNumber,
                //    //OUTBIRTHDATE = result.outBirthDate,
                //    //OUTACCOUNTNAME = result.outAccountName,
                //    //OUTPRIMARYCONTACTFIRSTNAME = result.outPrimaryContactFirstName,
                //    //OUTCONTACTLASTNAME = result.outContactLastName,
                //    //ERRORMESSAGE = result.outErrorMessage,
                //    //OUTPRODUCTNAME = result.outProductName,
                //    //OUTSERVICEYEAR = result.outServiceYear,
                //    //CREATED_BY = "FBBDORM",
                //    //CREATED_DATE = DateTime.Now,
                //    //OUTMOOBAN = result.outMooban,
                //    //OUTACCOUNTSUBCATEGORY = result.outAccountSubCategory,
                //    //OUTPARAMETER2 = result.outParameter2,
                //    //OUTPOSTALCODE = result.outPostalCode,
                //    //OUTTITLE = result.outTitle,
                //    //TRANSACTION_ID = query.inMobileNo,
                //    //OUTFULLADDRESS = result.outFullAddress,

                //    //PROJECTNAME = result.outProductName,
                //    //VATADDRESS1 = result.vatAddress1,
                //    //VATADDRESS2 = result.vatAddress2,
                //    //VATADDRESS3 = result.vatAddress3,
                //    //VATADDRESS4 = result.vatAddress4,
                //    //VATADDRESS5 = result.vatAddress5,
                //    //VATPOSTALCD = result.vatPostalCd
                //};

                //sffLog.Create(chkProfileLog);
                //_uow.Persist();
                #endregion

                // End InterfaceLog
                //  InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", result.ReturnMessage, "");
            }
            catch (Exception ex)
            {
                result.ReturnCode = "-1";
                result.ReturnMessage = ex.Message.ToString();
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Failed", result.ReturnMessage, "");
                throw ex;
            }

            return result;
        }
    }
}
