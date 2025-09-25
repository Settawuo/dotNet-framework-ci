using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using WBBBusinessLayer.QueryHandlers;
using WBBContract;
using WBBContract.Commands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.CommandHandlers
{
    public class CustRegisterCommandHandler : ICommandHandler<CustRegisterCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly ICommandHandler<SaveOutgoingMessageCommand> _comandSaveMessage;

        public CustRegisterCommandHandler(ILogger logger
            , IEntityRepository<string> objService,
            IEntityRepository<FBB_CFG_LOV> lov,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog, ICommandHandler<SaveOutgoingMessageCommand> comandSaveMessage)
        {
            _logger = logger;
            _objService = objService;
            _lov = lov;
            _uow = uow;
            _intfLog = intfLog;
            _comandSaveMessage = comandSaveMessage;
        }

        public void Handle(CustRegisterCommand command)
        {
            InterfaceLogCommand log = null;
            InterfaceLogCommand log2 = null; //R22.11
            SaveOutgoingMessageCommand outgoingOrderNewCommand = null;
            var transactionIdLogChangPack = "";
            try
            {

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, command.QuickWinPanelModel.CoveragePanelModel.P_MOBILE, "CustRegisterCommand", "CustRegisterCommandHandler", null, "FBB", command.CreateBy);

                try
                {
                    //TODO: R19.5 Insert order log for change pack field work
                    if (command.CreateBy.ToSafeString().IndexOf("WEBX", System.StringComparison.Ordinal) >= 0)
                    {
                        transactionIdLogChangPack = command.QuickWinPanelModel.TransactionID.ToSafeString();
                    }
                    else
                    {
                        transactionIdLogChangPack = command.QuickWinPanelModel.SessionId;
                    }

                    outgoingOrderNewCommand = new SaveOutgoingMessageCommand
                    {
                        MethodName = Constants.SbnWebService.CUSTREGISTERCOMMAND.ToString(),
                        Action = ActionType.Insert,
                        SoapXml = command.DumpToXml(),
                        OrderRowId = "",
                        MobileNo = "",
                        AirOrderNo = command.InterfaceOrder,
                        TransactionId = transactionIdLogChangPack
                    };
                    _comandSaveMessage.Handle(outgoingOrderNewCommand);
                }
                catch (Exception ex)
                {
                    var logx = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, "Insert Error = " + ex.GetBaseException(), command.QuickWinPanelModel.no, "SaveOutgoingMessageCommand", "CustRegisterCommandHandler", null, "FBB", "FBB");
                }

                PackageObjectModel packgaemodel = new PackageObjectModel();
                PictureObjectModel picturemodel = new PictureObjectModel();
                SplitterObjectModel splitterModel = new SplitterObjectModel();
                CPEListObjectModel cpeModel = new CPEListObjectModel();
                CustInsightObjectModel custInsightModel = new CustInsightObjectModel();
                FBBRegistDcontractRecordObjectModel dcontractModel = new FBBRegistDcontractRecordObjectModel();

                picturemodel.REC_REG_PACKAGE = command.QuickWinPanelModel.CustomerRegisterPanelModel.ListImageFile.Select(p => new PicturePackageOracleTypeMapping
                {
                    file_name = p.FileName.ToSafeString()
                }).ToArray();

                List<PackageModel> packgaemodelTMP = command.QuickWinPanelModel.SummaryPanelModel.PackageModelList.OrderBy(p => p.PACKAGE_SERVICE_CODE).ThenBy(p => p.PACKAGE_TYPE).ToList();

                var sumaryPanel = command.QuickWinPanelModel.SummaryPanelModel;

                if (sumaryPanel.TOPUP == "1" && sumaryPanel.PackageModel.SelectPlayBox_Flag == "1")
                {
                    var packageModelList = new List<PackageModel>();

                    //fix get only Package register playbox
                    packgaemodelTMP.RemoveAll(item => item.PRODUCT_SUBTYPE == "VOIP");

                    if (!string.IsNullOrEmpty(command.QuickWinPanelModel.RegisterPlayboxNumber) &&
                        Convert.ToInt16(command.QuickWinPanelModel.RegisterPlayboxNumber) > 0)
                    {
                        // register Playbox 2,3,....

                        for (var i = 0; i < Convert.ToInt16(command.QuickWinPanelModel.RegisterPlayboxNumber); i++)
                        {
                            var iExt = i.ToSafeString();
                            var playboxItem =
                                command.QuickWinPanelModel.MulitPlaybox.SingleOrDefault(
                                    item => item.RowNumber == iExt) ?? new MulitPlayboxModel();
                            var ext =
                                playboxItem.InstallProductSubType.Substring(
                                    playboxItem.InstallProductSubType.Length - 4);
                            foreach (
                                var packageModel in
                                    packgaemodelTMP.Where(item => item.SERVICE_CODE == playboxItem.ServiceCode)
                                )
                            {
                                packageModel.PLAYBOX_EXT = ext;
                                packageModelList.Add(packageModel);
                            }
                        }
                    }
                    else
                    {
                        //register Playbox Main
                        //remove package Playbox 2,3,....  from list package

                        packgaemodelTMP.RemoveAll(
                               item =>
                                   !string.IsNullOrEmpty(item.MAPPING_PRODUCT) &&
                                   item.MAPPING_PRODUCT.Substring(0, 1) == "E");

                        packageModelList = packgaemodelTMP;

                    }

                    packgaemodel.REC_REG_PACKAGE = packageModelList.Select(c => new Rec_Reg_PackageOracleTypeMapping
                    {
                        package_code = c.SFF_PROMOTION_CODE.ToSafeString(),
                        package_class = c.PACKAGE_TYPE_DESC.ToSafeString(),
                        package_group = c.PACKAGE_GROUP.ToSafeString(),
                        product_subtype = c.PRODUCT_SUBTYPE.ToSafeString(),
                        technology = c.TECHNOLOGY.ToSafeString(),
                        package_name = c.SFF_PRODUCT_NAME.ToSafeString(),
                        recurring_charge = c.PRICE_CHARGE.GetValueOrDefault(),
                        initiation_charge = c.PRE_PRICE_CHARGE.GetValueOrDefault(),
                        discount_initiation = c.DISCOUNT_INITIATION_CHARGE.GetValueOrDefault(),
                        package_bill_tha = c.SFF_WORD_IN_STATEMENT_THA.ToSafeString(),
                        package_bill_eng = c.SFF_WORD_IN_STATEMENT_ENG.ToSafeString(),
                        download_speed = c.DOWNLOAD_SPEED.ToSafeString(),
                        upload_speed = c.UPLOAD_SPEED.ToSafeString(),
                        owner_product = c.OWNER_PRODUCT.ToSafeString(),
                        voip_ip = c.VOIP_IP.ToSafeString(),
                        idd_flag = c.IDD_FLAG.ToSafeString(),
                        fax_flag = c.FAX_FLAG.ToSafeString(),
                        mobile_forward = c.MOBILE_FORWARD.ToSafeString()
                    }).ToArray();
                }
                else
                {
                    packgaemodel.REC_REG_PACKAGE = packgaemodelTMP.Select(c => new Rec_Reg_PackageOracleTypeMapping
                    {
                        package_code = c.SFF_PROMOTION_CODE.ToSafeString(),
                        package_class = c.PACKAGE_TYPE_DESC.ToSafeString(),
                        package_group = c.PACKAGE_GROUP.ToSafeString(),
                        product_subtype = c.PRODUCT_SUBTYPE.ToSafeString(),
                        technology = c.TECHNOLOGY.ToSafeString(),
                        package_name = c.SFF_PRODUCT_NAME.ToSafeString(),
                        recurring_charge = c.PRICE_CHARGE.GetValueOrDefault(),
                        initiation_charge = c.PRE_PRICE_CHARGE.GetValueOrDefault(),
                        discount_initiation = c.DISCOUNT_INITIATION_CHARGE.GetValueOrDefault(),
                        package_bill_tha = c.SFF_WORD_IN_STATEMENT_THA.ToSafeString(),
                        package_bill_eng = c.SFF_WORD_IN_STATEMENT_ENG.ToSafeString(),
                        download_speed = c.DOWNLOAD_SPEED.ToSafeString(),
                        upload_speed = c.UPLOAD_SPEED.ToSafeString(),
                        owner_product = c.OWNER_PRODUCT.ToSafeString(),
                        voip_ip = c.VOIP_IP.ToSafeString(),
                        idd_flag = c.IDD_FLAG.ToSafeString(),
                        fax_flag = c.FAX_FLAG.ToSafeString(),
                        mobile_forward = c.MOBILE_FORWARD.ToSafeString()
                    }).ToArray();
                }

                if (command.QuickWinPanelModel.CoverageAreaResultModel.SPLITTER_LIST != null)
                {
                    splitterModel.REC_CUST_SPLITTER = command.QuickWinPanelModel.CoverageAreaResultModel.SPLITTER_LIST.Select(p => new Rec_Cust_SplitterOracleTypeMapping
                    {
                        splitter_name = p.Splitter_Name.ToSafeString(),
                        distance = p.Distance,
                        distance_type = p.Distance_Type.ToSafeString(),
                        resource_type = "SPLITTER"
                    }).ToArray();
                }
                else if (command.QuickWinPanelModel.CoverageAreaResultModel.RESOURCE_LIST != null)
                {
                    splitterModel.REC_CUST_SPLITTER = command.QuickWinPanelModel.CoverageAreaResultModel.RESOURCE_LIST.Select(p => new Rec_Cust_SplitterOracleTypeMapping
                    {
                        splitter_name = p.Dslam_Name.ToSafeString(),
                        distance = 0,
                        distance_type = "",
                        resource_type = "DSLAM"
                    }).ToArray();
                }
                else
                {
                    List<Rec_Cust_SplitterOracleTypeMapping> ltmp = new List<Rec_Cust_SplitterOracleTypeMapping>()
                    {
                        //new Rec_Cust_SplitterOracleTypeMapping() { splitter_name = "", distance = 0, distance_type = ""}
                    };
                    splitterModel.REC_CUST_SPLITTER = ltmp.ToArray();
                }
                //20.2
                if (command.QuickWinPanelModel.CoveragePanelModel.WTTX_COVERAGE_RESULT == "YES")
                {
                    if (command.QuickWinPanelModel.CustomerRegisterPanelModel.WTTx_Info != null)
                    {
                        cpeModel.CPE_LIST_PACKAGE = command.QuickWinPanelModel.CustomerRegisterPanelModel.WTTx_Info.Select(p => new CPE_List_PackageOracleTypeMapping
                        {
                            cpe_type = p.cpe_type.ToSafeString(),
                            serial_no = p.SN.ToSafeString(),
                            mac_address = p.CPE_MAC_ADDR.ToSafeString(),

                            //20.4
                            status_desc = p.STATUS_DESC.ToSafeString(),
                            model_name = p.CPE_MODEL_NAME.ToSafeString(),
                            company_code = p.CPE_COMPANY_CODE.ToSafeString(),
                            cpe_plant = p.CPE_PLANT.ToSafeString(),
                            storage_location = p.CPE_STORAGE_LOCATION.ToSafeString(),
                            material_code = p.CPE_MATERIAL_CODE.ToSafeString(),
                            register_date = p.REGISTER_DATE.ToSafeString(),
                            fibrenet_id = p.FIBRENET_ID.ToSafeString(),
                            sn_pattern = p.SN_PATTERN.ToSafeString(),
                            ship_to = p.SHIP_TO.ToSafeString(),
                            warranty_start_date = p.WARRANTY_START_DATE.ToSafeString(),
                            warranty_end_date = p.WARRANTY_END_DATE.ToSafeString()
                        }).ToArray();
                    }
                    else
                    {
                        List<CPE_List_PackageOracleTypeMapping> ltmps = new List<CPE_List_PackageOracleTypeMapping>()
                        {
                            //
                        };
                        cpeModel.CPE_LIST_PACKAGE = ltmps.ToArray();
                    }
                }
                else
                {
                    if (command.QuickWinPanelModel.CustomerRegisterPanelModel.CPE_Info != null)
                    {
                        cpeModel.CPE_LIST_PACKAGE = command.QuickWinPanelModel.CustomerRegisterPanelModel.CPE_Info.Select(p => new CPE_List_PackageOracleTypeMapping
                        {
                            cpe_type = p.cpe_type.ToSafeString(),
                            serial_no = p.SN.ToSafeString(),
                            mac_address = p.CPE_MAC_ADDR.ToSafeString(),

                            //20.4
                            status_desc = p.STATUS_DESC.ToSafeString(),
                            model_name = p.CPE_MODEL_NAME.ToSafeString(),
                            company_code = p.CPE_COMPANY_CODE.ToSafeString(),
                            cpe_plant = p.CPE_PLANT.ToSafeString(),
                            storage_location = p.CPE_STORAGE_LOCATION.ToSafeString(),
                            material_code = p.CPE_MATERIAL_CODE.ToSafeString(),
                            register_date = p.REGISTER_DATE.ToSafeString(),
                            fibrenet_id = p.FIBRENET_ID.ToSafeString(),
                            sn_pattern = p.SN_PATTERN.ToSafeString(),
                            ship_to = p.SHIP_TO.ToSafeString(),
                            warranty_start_date = p.WARRANTY_START_DATE.ToSafeString(),
                            warranty_end_date = p.WARRANTY_END_DATE.ToSafeString()
                        }).ToArray();
                    }
                    else
                    {
                        List<CPE_List_PackageOracleTypeMapping> ltmps = new List<CPE_List_PackageOracleTypeMapping>()
                        {
                            //
                        };
                        cpeModel.CPE_LIST_PACKAGE = ltmps.ToArray();
                    }
                }

                var quickWinModel = command.QuickWinPanelModel;
                //var quickWinModel = ForTest();  

                // check null.
                if (null == quickWinModel)
                    throw new Exception("Model is null.");
                if (null == quickWinModel.CoveragePanelModel)
                    throw new Exception("CoveragePanelModel is null.");
                if (null == quickWinModel.DisplayPackagePanelModel)
                    throw new Exception("DisplayPackagePanelModel is null.");
                if (null == quickWinModel.CustomerRegisterPanelModel)
                    throw new Exception("CustomerRegisterPanelModel is null.");
                if (null == quickWinModel.SummaryPanelModel)
                    throw new Exception("SummaryPanelModel is null.");

                var cpm = quickWinModel.CoveragePanelModel;
                var dpm = quickWinModel.DisplayPackagePanelModel;
                var crpm = quickWinModel.CustomerRegisterPanelModel;
                var spm = quickWinModel.SummaryPanelModel;
                var oim = quickWinModel.OfficerInfoPanelModel;
                var langFlag = (command.CurrentCulture.IsThaiCulture() ? "THA" : "ENG");

                if (crpm.ListCustomerInsight != null && crpm.ListCustomerInsight.Count() > 0)
                {
                    custInsightModel.CustInsight = crpm.ListCustomerInsight.Select(p => new CustInsightOracleTypeMapping
                    {
                        group_id = p.GROUP_ID.ToSafeString(),
                        group_name_th = p.GROUP_NAME_TH.ToSafeString(),
                        group_name_en = p.GROUP_NAME_EN.ToSafeString(),
                        question_id = p.QUESTION_ID.ToSafeString(),
                        question_th = p.QUESTION_TH.ToSafeString(),
                        question_en = p.QUESTION_EN.ToSafeString(),
                        answer_id = p.ANSWER_ID.ToSafeString(),
                        answer_th = p.ANSWER_TH.ToSafeString(),
                        answer_en = p.ANSWER_EN.ToSafeString(),
                        answer_value_th = p.ANSWER_VALUE_TH.ToSafeString(),
                        answer_value_en = p.ANSWER_VALUE_EN.ToSafeString(),
                        parent_answer_id = p.PARENT_ANSWER_ID.ToSafeString(),
                        action_wfm = p.ACTION_WFM.ToSafeString(),
                        action_foa = p.ACTION_FOA.ToSafeString()
                    }).ToArray();
                }
                else
                {
                    List<CustInsightOracleTypeMapping> ltmps = new List<CustInsightOracleTypeMapping>()
                    {
                        //
                    };
                    custInsightModel.CustInsight = ltmps.ToArray();
                }

                if (crpm.ListDcontract != null && crpm.ListDcontract.Count > 0)
                {
                    dcontractModel.dcontract = crpm.ListDcontract.Select(p => new FBB_Regist_Dcontract_RecordOracleTypeMapping
                    {
                        PRODUCT_SUBTYPE = p.PRODUCT_SUBTYPE.ToSafeString(),
                        PBOX_EXT = p.PBOX_EXT.ToSafeString(),
                        TDM_CONTRACT_ID = p.TDM_CONTRACT_ID.ToSafeString(),
                        TDM_RULE_ID = p.TDM_RULE_ID.ToSafeString(),
                        TDM_PENALTY_ID = p.TDM_PENALTY_ID.ToSafeString(),
                        TDM_PENALTY_GROUP_ID = p.TDM_PENALTY_GROUP_ID.ToSafeString(),
                        DURATION = p.DURATION.ToSafeString(),
                        CONTRACT_FLAG = p.CONTRACT_FLAG.ToSafeString(),
                        DEVICE_COUNT = p.DEVICE_COUNT.ToSafeString()
                    }).ToArray();
                }
                else
                {
                    List<FBB_Regist_Dcontract_RecordOracleTypeMapping> ltmps = new List<FBB_Regist_Dcontract_RecordOracleTypeMapping>()
                    {
                        //
                    };
                    dcontractModel.dcontract = ltmps.ToArray();
                }

                var register = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_REC_REG_PACKAGE", "FBB_REG_PACKAGE_ARRAY", packgaemodel);
                var listfilename = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_REC_UPLOAD_FILE", "FBB_REG_UPLOAD_FILE_ARRAY", picturemodel);
                var splitter = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_REC_SPLITTER_LIST", "FBB_REG_SPLITTER_LIST_ARRAY", splitterModel);
                var listcpe = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_REC_CPE_LIST", "FBB_REG_CPE_LIST_ARRAY", cpeModel);
                var listCustInsight = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_REC_CUST_INSIGHT", "FBB_REG_CUST_INSIGHT_ARRAY", custInsightModel);
                var listDcontract = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_REC_DCONTRACT", "FBB_REC_DONTRACT_ARRAY", dcontractModel);

                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var v_error_msg = new OracleParameter();
                v_error_msg.OracleDbType = OracleDbType.Varchar2;
                v_error_msg.Size = 2000;
                v_error_msg.Direction = ParameterDirection.Output;

                var v_cust_id = new OracleParameter();
                v_cust_id.OracleDbType = OracleDbType.Varchar2;
                v_cust_id.Size = 2000;
                v_cust_id.Direction = ParameterDirection.Output;

                var title = crpm.L_TITLE.ToSafeString();//(spm.VAS_FLAG.Equals("2") ? "คุณ" : crpm.L_TITLE.ToSafeString());//crpm.L_TITLE.ToSafeString();//(spm.VAS_FLAG.Equals("2") ? crpm.L_TITLE.ToSafeString() : crpm.L_TITLE_CODE.ToSafeString());

                var firstName = (crpm.CateType.ToSafeString().Equals("R") ? crpm.L_FIRST_NAME : crpm.L_GOVERNMENT_NAME);
                var lastName = (crpm.CateType.ToSafeString().Equals("R") ? crpm.L_LAST_NAME : "");

                var cTitle = (crpm.CateType.ToSafeString().Equals("R") ? crpm.L_TITLE_CODE.ToSafeString() : "");
                if (spm.VAS_FLAG.ToSafeString() == "2") { cTitle = crpm.L_TITLE.ToSafeString(); }

                var tempcontact = crpm.L_CONTACT_PERSON.ToSafeString().Split();
                var ctemtfirst = "";
                var ctemtlast = "";
                if (tempcontact.Count() > 1)
                {
                    ctemtfirst = tempcontact[0];
                    for (var i = 1; i < tempcontact.Count(); i++)
                    {
                        ctemtlast = ctemtlast + tempcontact[i] + " ";
                    }
                }
                else
                {
                    ctemtfirst = tempcontact[0];
                    ctemtlast = ".";
                }

                var cFirstName = (crpm.CateType.ToSafeString().Equals("R") ? crpm.L_FIRST_NAME : ctemtfirst);
                var cLastName = (crpm.CateType.ToSafeString().Equals("R") ? crpm.L_LAST_NAME : ctemtlast.Trim());

                var phoneNo = crpm.L_MOBILE.ToSafeString() == "" ? crpm.L_CONTACT_PHONE.ToSafeString() : crpm.L_MOBILE.ToSafeString();

                if (spm.VAS_FLAG.ToSafeString() == "2" || spm.TOPUP.ToSafeString() == "1")
                {
                    if (crpm.L_HOME_PHONE == null)
                        crpm.L_HOME_PHONE = "";
                    if (crpm.L_OR == null)
                        crpm.L_OR = "";
                }

                var cardNo = (crpm.L_CARD_TYPE.ToSafeString().Equals("TAX_ID") ? "" : crpm.L_CARD_NO);
                var taxId = (crpm.L_CARD_TYPE.ToSafeString().Equals("TAX_ID") ? crpm.L_CARD_NO : "");

                var cusNat = (crpm.CateType.ToSafeString().Equals("R") ? crpm.L_NATIONALITY : "");
                if (spm.VAS_FLAG.ToSafeString() == "2" || spm.TOPUP.ToSafeString() == "1") { cusNat = "THAI"; }

                if (spm.VAS_FLAG.ToSafeString() != "2" || spm.TOPUP.ToSafeString() == "1")
                {
                    var addressTypeWire = "";
                    if (!string.IsNullOrEmpty(cpm.BuildingType) && cpm.BuildingType.Equals("B"))
                    {
                        addressTypeWire = cpm.Address.L_BUILD_NAME;
                    }
                    else
                    {
                        addressTypeWire = cpm.Address.L_MOOBAN;
                    }
                }

                var floorNo = (string.IsNullOrEmpty(cpm.L_FLOOR_CONDO) ? cpm.L_FLOOR_VILLAGE : cpm.L_FLOOR_CONDO);

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var birthDateString = string.Empty;
                var birthDate = new DateTime();

                if (string.IsNullOrEmpty(crpm.L_BIRTHDAY)) crpm.L_BIRTHDAY = "0/0/";

                var date = DateTime.TryParseExact(crpm.L_BIRTHDAY.ToSafeString(), "dd/MM/yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out birthDate);

                if (command.CurrentCulture.IsThaiCulture())
                {
                    if (birthDate > DateTime.MinValue.AddYears(543))
                        birthDateString = birthDate.AddYears(-543).ToDateDisplayText();
                }
                else
                {
                    if (birthDate > DateTime.MinValue)
                        birthDateString = birthDate.ToDateDisplayText();
                }

                var split_BD = crpm.L_BIRTHDAY.ToSafeString().Split('/');
                if (!date && split_BD.Length > 2)
                {
                    if (split_BD.Any())
                    {
                        if (command.CurrentCulture.IsThaiCulture())
                        {
                            if (split_BD[2] != "")
                            {
                                if (DateTime.IsLeapYear((split_BD[2].ToSafeInteger() - 543)))
                                {
                                    if (split_BD[1] == "02")
                                        if (split_BD[0] == "29")
                                        {
                                            if (spm.VAS_FLAG.ToSafeString() != "2" && spm.TOPUP.ToSafeString() == "")
                                            {
                                                birthDate = new DateTime(split_BD[2].ToSafeInteger() - 543, 2, 29);
                                                birthDateString = birthDate.ToString("dd/MM/yyyy");
                                                crpm.L_BIRTHDAY = birthDateString;
                                            }
                                            else
                                            {
                                                birthDate = new DateTime(split_BD[2].ToSafeInteger(), 2, 29);
                                                birthDateString = birthDate.ToString("dd/MM/yyyy");
                                                crpm.L_BIRTHDAY = birthDateString;
                                            }
                                        }
                                }
                            }
                            else
                            {
                                birthDateString = "";
                                crpm.L_BIRTHDAY = birthDateString;
                            }
                        }
                        else
                        {
                            if (split_BD[2] != "")
                            {
                                if (DateTime.IsLeapYear(split_BD[2].ToSafeInteger()))
                                {
                                    if (split_BD[1] == "02")
                                        if (split_BD[0] == "29")
                                        {
                                            birthDate = new DateTime(split_BD[2].ToSafeInteger(), 2, 29);
                                            birthDateString = birthDate.ToString("dd/MM/yyyy");
                                            crpm.L_BIRTHDAY = birthDateString;
                                        }
                                }
                            }
                            else
                            {
                                birthDateString = "";
                                crpm.L_BIRTHDAY = birthDateString;
                            }
                        }
                    }
                    else
                    {
                        birthDateString = "";
                        crpm.L_BIRTHDAY = birthDateString;
                    }

                }

                var condoType = "";
                var condoDirection = "";
                var condoLimit = "";
                var condoArea = "";
                var homeType = "";
                var homeArea = "";

                if (spm.VAS_FLAG.ToSafeString() != "2" && spm.TOPUP.ToSafeString() == "" && cpm.AccessMode.ToSafeString() == "FWA")
                {
                    condoType = crpm.L_BUILD_CONDO.ToSafeString();
                    condoDirection = crpm.L_TERRACE_DIRECTION.ToSafeString();
                    condoLimit = crpm.L_NUM_OF_FLOOR.ToSafeString();
                    condoArea = crpm.L_CONDO_AREA.ToSafeString();
                    homeType = crpm.L_TYPE_ADDR.ToSafeString();
                    homeArea = crpm.L_HOUSE_AREA.ToSafeString();

                    if (!string.IsNullOrEmpty(cpm.BuildingType) && cpm.BuildingType.Equals("B"))
                    {
                        homeType = "";
                        homeArea = "";
                    }
                    else
                    {
                        condoType = "";
                        condoDirection = "";
                        condoLimit = "";
                        condoArea = "";
                    }
                }


                if (spm.VAS_FLAG.ToSafeString() == "2" || spm.TOPUP.ToSafeString() == "1")
                {
                    if (crpm.SubCateType != "")
                    {
                        crpm.SubCateType = crpm.SubCateType.Substring(0, 1);
                    }
                    crpm.AddressPanelModelSetup.ZIPCODE_ID = crpm.AddressPanelModelSetup.L_ZIPCODE;
                    crpm.AddressPanelModelSendDoc.ZIPCODE_ID = crpm.AddressPanelModelSendDoc.L_ZIPCODE;
                    crpm.AddressPanelModelVat.ZIPCODE_ID = crpm.AddressPanelModelVat.L_ZIPCODE;
                }
                if (crpm.CateType.ToSafeString().Equals("R"))
                {

                    crpm.AddressPanelModelVat.L_HOME_NUMBER_2 = crpm.AddressPanelModelSendDocIDCard.L_HOME_NUMBER_2;
                    crpm.AddressPanelModelVat.L_SOI = crpm.AddressPanelModelSendDocIDCard.L_SOI;
                    crpm.AddressPanelModelVat.L_MOO = crpm.AddressPanelModelSendDocIDCard.L_MOO;
                    crpm.AddressPanelModelVat.L_MOOBAN = crpm.AddressPanelModelSendDocIDCard.L_MOOBAN;
                    crpm.AddressPanelModelVat.L_BUILD_NAME = crpm.AddressPanelModelSendDocIDCard.L_BUILD_NAME;
                    crpm.AddressPanelModelVat.L_FLOOR = crpm.AddressPanelModelSendDocIDCard.L_FLOOR;
                    crpm.AddressPanelModelVat.L_ROOM = crpm.AddressPanelModelSendDocIDCard.L_ROOM;
                    crpm.AddressPanelModelVat.L_ROAD = crpm.AddressPanelModelSendDocIDCard.L_ROAD;
                    crpm.AddressPanelModelVat.ZIPCODE_ID = crpm.AddressPanelModelSendDocIDCard.ZIPCODE_ID;
                }

                #region check cabasa

                var CA_ID = "";
                var SA_ID = "";
                var BA_ID = "";
                var P_AIS_MOBILE = "";
                var P_AIS_NONMOBILE = "";
                var Productname = "";
                var ServiceYear = "";

                if (cpm.P_MOBILE.ToSafeString() != "")
                {
                    cpm.P_MOBILE = cpm.P_MOBILE.Replace("|", "");
                    if (cpm.P_MOBILE != "")
                    {
                        if (cpm.P_MOBILE.ToSafeString().Substring(0, 1) == "0")
                        {
                            P_AIS_NONMOBILE = "";
                            P_AIS_MOBILE = cpm.P_MOBILE.ToSafeString();
                        }
                        else if (cpm.P_MOBILE.ToSafeString().Substring(0, 1) != "0")
                        {
                            P_AIS_MOBILE = "";
                            P_AIS_NONMOBILE = cpm.P_MOBILE.ToSafeString();
                        }
                    }
                }

                //R21.10 MOU
                if (cpm.SAVEORDER_MOU_FLAG == "Y")
                {
                    P_AIS_NONMOBILE = crpm.FIBRE_ID.ToSafeString();
                }

                if (spm.VAS_FLAG.ToSafeString() == "1" || (spm.VAS_FLAG.ToSafeString() == "0" && spm.TOPUP.ToSafeString() == ""))
                {
                    Productname = cpm.SffProductName.ToSafeString();
                    ServiceYear = cpm.SffServiceYear.ToSafeString();


                    if (cpm.P_MOBILE.ToSafeString() == "")
                    {
                        CA_ID = "";
                        SA_ID = "";
                        BA_ID = "";
                    }
                    else
                    {
                        if (quickWinModel.CoveragePanelModel.ChargeType == "PREPAID")
                        {
                            CA_ID = "";
                            SA_ID = "";
                            BA_ID = "";
                        }
                        else if (cpm.BillingSystem.ToSafeString() == "BOS" && quickWinModel.CoveragePanelModel.ChargeType == "POSTPAID")
                        {
                            if (quickWinModel.CoveragePanelModel.BundlingSpecialFlag == "Y" || quickWinModel.CoveragePanelModel.BundlingMainFlag == "Y")
                            {
                                CA_ID = cpm.CA_ID.ToSafeString(); ;
                                SA_ID = cpm.SA_ID.ToSafeString(); ;
                                BA_ID = "";
                            }
                            else
                            {
                                CA_ID = "";
                                SA_ID = "";
                                BA_ID = "";
                            }
                        }
                        else
                        {
                            CA_ID = cpm.CA_ID.ToSafeString();//cpm.CA_ID.ToSafeString();
                            SA_ID = cpm.SA_ID.ToSafeString();//cpm.SA_ID.ToSafeString();
                            BA_ID = "";
                        }
                        //if (cpm.P_MOBILE.ToSafeString().Substring(0, 1) == "0" && cpm.SffProductName.ToSafeString() == "3G")
                        //{
                        ////if (spm.PackageModel.PACKAGE_TYPE.ToSafeString() == "Main")
                        ////{
                        ////    CA_ID = cpm.CA_ID.ToSafeString();//cpm.CA_ID.ToSafeString();
                        ////    SA_ID = "";
                        ////    BA_ID = "";
                        ////}
                        ////else if (spm.PackageModel.PACKAGE_TYPE.ToSafeString() == "Bundle")
                        ////{
                        //CA_ID = cpm.CA_ID.ToSafeString();//cpm.CA_ID.ToSafeString();
                        //SA_ID = cpm.SA_ID.ToSafeString();//cpm.SA_ID.ToSafeString();
                        //BA_ID = cpm.BA_ID.ToSafeString();//cpm.BA_ID.ToSafeString();
                        ////}


                        //if (spm.PackageModel.PACKAGE_TYPE.ToSafeString() == "Main")
                        //{
                        //    if (spm.PackageModelList[0].PACKAGE_GROUP.ToSafeString().Contains("Triple-Play"))
                        //    {
                        //        CA_ID = "";
                        //        SA_ID = "";
                        //        BA_ID = "";

                        //    }

                        //    else
                        //    {
                        //        CA_ID = cpm.CA_ID.ToSafeString();
                        //        SA_ID = "";
                        //        BA_ID = "";
                        //    }
                        //}
                        //else if (spm.PackageModelList[0].PACKAGE_TYPE.ToSafeString() == "Bundle")
                        //{
                        //    CA_ID = cpm.CA_ID.ToSafeString();
                        //    SA_ID = cpm.SA_ID.ToSafeString();
                        //    BA_ID = cpm.BA_ID.ToSafeString();
                        //}
                        //}
                        //else if (cpm.P_MOBILE.ToSafeString().Substring(0, 1) == "0" && cpm.SffProductName.ToSafeString() != "3G")
                        //{
                        //    CA_ID = cpm.CA_ID.ToSafeString();
                        //    SA_ID = "";
                        //    BA_ID = "";
                        //}
                        //else if (cpm.P_MOBILE.ToSafeString().Substring(0, 2) == "88" || cpm.P_MOBILE.ToSafeString().Substring(0, 2) == "89")
                        //{
                        //    CA_ID = cpm.CA_ID.ToSafeString();//cpm.CA_ID.ToSafeString();
                        //    SA_ID = "";
                        //    BA_ID = "";
                        //}

                    }
                }
                else if (spm.VAS_FLAG.ToSafeString() == "2" || spm.TOPUP.ToSafeString() == "1")
                {
                    Productname = cpm.SffProductName;
                    ServiceYear = cpm.SffServiceYear;
                    CA_ID = cpm.CA_ID;//cpm.CA_ID.ToSafeString();
                    SA_ID = cpm.SA_ID;//cpm.SA_ID.ToSafeString();
                    BA_ID = cpm.BA_ID;//cpm.BA_ID.ToSafeString();
                }
                else if (spm.VAS_FLAG.ToSafeString() == "3")
                {
                    if (cpm.BillingSystem.ToSafeString() == "BOS" && quickWinModel.CoveragePanelModel.ChargeType == "POSTPAID")
                    {
                        if (quickWinModel.CoveragePanelModel.BundlingSpecialFlag == "Y" || quickWinModel.CoveragePanelModel.BundlingMainFlag == "Y")
                        {
                            CA_ID = cpm.CA_ID.ToSafeString(); ;
                            SA_ID = cpm.SA_ID.ToSafeString(); ;
                            BA_ID = "";
                        }
                        else
                        {
                            CA_ID = "";
                            SA_ID = "";
                            BA_ID = "";
                        }
                    }
                    else
                    {
                        CA_ID = cpm.CA_ID.ToSafeString();//cpm.CA_ID.ToSafeString();
                        SA_ID = cpm.SA_ID.ToSafeString();//cpm.SA_ID.ToSafeString();
                        BA_ID = "";
                    }
                }
                else
                {
                    CA_ID = "";
                    SA_ID = "";
                    BA_ID = "";
                }

                var register_type = "CUSTOMER_REGISTER";
                if (spm.VAS_FLAG.ToSafeString() == "1")
                {
                    register_type = "INTERNAL_REGISTER";
                }
                else if (spm.VAS_FLAG.ToSafeString() == "2")
                {
                    register_type = "INTERNAL_VAS_REGISTER";
                }
                else if (spm.TOPUP.ToSafeString() == "1")
                {
                    register_type = "CUSTOMER_REGISTER_VAS";

                }
                else if (spm.VAS_FLAG.ToSafeString() == "3")
                {
                    register_type = "CUSTOMER_REGISTER_TRIPLEPLAY";
                }
                else if (spm.VAS_FLAG.ToSafeString() == "4")
                {
                    register_type = "CUSTOMER_REGISTER_ONTOP_PLAYBOX";
                }
                else if (quickWinModel.TopUp.ToSafeString() == "5")
                {
                    register_type = "OFFICER_REGISTER";
                }
                else if (spm.VAS_FLAG.ToSafeString() == "6")
                {
                    register_type = "STAFF_REGISTER";
                }
                else if (quickWinModel.TopUp.ToSafeString() == "7")
                {
                    register_type = "ENGINEER_REGISTER";
                }
                else if (spm.VAS_FLAG.ToSafeString() == "8")
                {
                    register_type = "SELL_ROUTER";
                }


                if (quickWinModel.ExistingFlag.ToSafeString() == "MESH" || quickWinModel.ExistingFlag.ToSafeString() == "MESH:MENU")
                {
                    register_type = "TOPUP_MESH";
                }
                //var register_Device = "";
                //if (quickWinModel.Register_device == "WEB_BROWSER")
                //{
                //    register_Device = "WEB BROWSER";
                //}
                // else if(quickWinModel.Register_device == "MOBILE_APP")
                //{
                //    register_Device = "MOBILE APP";
                //}
                //else if (quickWinModel.Register_device == "MOBILE_WEB")
                //{
                //    register_Device = "MOBILE WEB";
                //}

                if (cpm.CVRID.ToSafeString() == "sym")
                {
                    cpm.CVRID = "0";
                }
                #endregion

                //// phone flag
                if (cpm.L_HAVE_FIXED_LINE.ToSafeString() == "")
                {
                    cpm.L_HAVE_FIXED_LINE = "N";
                }

                if (command.InterfaceDesc.ToSafeString().Length > 100)
                {
                    command.InterfaceDesc = command.InterfaceDesc.ToSafeString().Substring(0, 99);
                }
                /// Is Serenade

                string[] inputparam = (from z in _lov.Get()
                                       where z.LOV_NAME == "SERENADE_MOBILE_SEGMENT" && z.ACTIVEFLAG == "Y" && z.LOV_TYPE == "FBB_CONSTANT"
                                       select z.LOV_VAL1).ToArray();

                bool IsSerenade = false;
                string mSeg = quickWinModel.CoveragePanelModel.MobileSegment != null ? quickWinModel.CoveragePanelModel.MobileSegment.ToUpper() : "";
                string NetworkType = quickWinModel.CoveragePanelModel.NetworkType != null ? quickWinModel.CoveragePanelModel.NetworkType.ToUpper() : "";

                string mSegPre = quickWinModel.CoveragePanelModel.Mobile_Segment != null ? quickWinModel.CoveragePanelModel.Mobile_Segment.ToUpper() : "";
                if (inputparam.Contains(mSeg) || inputparam.Contains(mSegPre))
                    IsSerenade = true;

                //ถ้าเป็นเบอร์ prepaid
                if (quickWinModel.CoveragePanelModel.ChargeType == "PREPAID")
                {
                    //Productname = "3G|PREPAID";
                    //เป็น Serenade
                    if (IsSerenade)
                    {
                        Productname = cpm.NetworkType.ToSafeString() + "|PREPAID|SERENADE";
                    }
                    else
                    {
                        Productname = cpm.NetworkType.ToSafeString() + "|PREPAID";
                    }
                }
                //ถ้าเป็นเบอร์ postpaid
                else if (quickWinModel.CoveragePanelModel.ChargeType == "POSTPAID")
                {
                    //เป็น Serenade
                    if (IsSerenade)
                    {
                        Productname = cpm.NetworkType.ToSafeString() + "|POSTPAID|SERENADE";
                    }
                    else
                    {
                        Productname = cpm.NetworkType.ToSafeString() + "|POSTPAID";
                    }
                }
                else
                    Productname = cpm.NetworkType.ToSafeString();

                //TODO: Splitter Management
                var splitterFlag = string.Empty;
                var reservedId = string.Empty;
                var spacialRemark = string.Empty;

                if (!string.IsNullOrEmpty(command.QuickWinPanelModel.FlowFlag)
                    && command.QuickWinPanelModel.CoveragePanelModel.AccessMode == "FTTH")
                {
                    splitterFlag = command.QuickWinPanelModel.SplitterFlag;
                    reservedId = command.QuickWinPanelModel.ReservationId;
                    var configNoteForCs =
                        _lov.Get().FirstOrDefault(
                            item => item.LOV_NAME == "SaveOrderNew" &&
                                    item.LOV_VAL1 == command.QuickWinPanelModel.SplitterFlagFirstTime &&
                                    item.LOV_VAL2 == command.QuickWinPanelModel.SplitterFlag) ?? new FBB_CFG_LOV();
                    spacialRemark = configNoteForCs.LOV_VAL3;
                }
                else if (command.QuickWinPanelModel.CoveragePanelModel.AccessMode == "VDSL")
                {
                    splitterFlag = command.QuickWinPanelModel.SplitterFlag;
                    reservedId = command.QuickWinPanelModel.ReservationId;
                    var configNoteForCs =
                        _lov.Get().FirstOrDefault(
                            item => item.LOV_NAME == "SaveOrderNew" &&
                                    item.DISPLAY_VAL == "SPECIAL_REMARK_FTTB" &&
                                    item.LOV_VAL1 == command.QuickWinPanelModel.SplitterFlagFirstTime
                    ) ?? new FBB_CFG_LOV();
                    spacialRemark = configNoteForCs.LOV_VAL3;
                }
                else if (command.QuickWinPanelModel.CoveragePanelModel.AccessMode == "WTTx")
                {
                    splitterFlag = command.QuickWinPanelModel.SplitterFlag;
                    reservedId = command.QuickWinPanelModel.ReservationId;
                    var configNoteForCs =
                        _lov.Get().FirstOrDefault(
                            item => item.LOV_NAME == "SaveOrderNew" &&
                                    item.DISPLAY_VAL == "SPECIAL_REMARK_WTTX"
                    ) ?? new FBB_CFG_LOV();
                    spacialRemark = configNoteForCs.LOV_VAL1;
                }

                if (!string.IsNullOrEmpty(command.QuickWinPanelModel.SpecialRemark) && (quickWinModel.ExistingFlag.ToSafeString() == "MESH" || quickWinModel.ExistingFlag.ToSafeString() == "MESH:MENU"))
                {
                    spacialRemark = command.QuickWinPanelModel.SpecialRemark;
                }

                //TODO: eStatement
                //18.2 eBill
                var lovEstatement = (from z in _lov.Get()
                                     where z.LOV_NAME == "ESTATEMENT_STATUS" && z.ACTIVEFLAG == "Y" && z.LOV_TYPE == "FBB_CONSTANT"
                                     select z).FirstOrDefault() ?? new FBB_CFG_LOV();

                var billMedia = string.Empty;
                if (!string.IsNullOrEmpty(command.QuickWinPanelModel.CustomerRegisterPanelModel.EBillFlag))
                {
                    if (command.QuickWinPanelModel.CustomerRegisterPanelModel.EBillFlag == "0")
                    {
                        billMedia = lovEstatement.LOV_VAL2;
                    }
                    else if (command.QuickWinPanelModel.CustomerRegisterPanelModel.EBillFlag == "1")
                    {
                        billMedia = lovEstatement.LOV_VAL3;
                    }
                    else if (command.QuickWinPanelModel.CustomerRegisterPanelModel.EBillFlag == "2")
                    {
                        billMedia = lovEstatement.LOV_VAL1;
                    }
                }

                //17.7 MGM
                var MemberGetMemberInfo = quickWinModel.CoveragePanelModel.CoverageMemberGetMember;

                //18.1 FTTB Sell Router
                string strPlug_and_play_flag = "";
                if (!string.IsNullOrEmpty(crpm.L_EVENT_CODE.ToSafeString()))
                {
                    strPlug_and_play_flag = crpm.Plug_and_play_flag.ToSafeString();
                }
                if (!string.IsNullOrEmpty(crpm.RouterFlag))
                {
                    if (crpm.RouterFlag == "S")
                    {
                        strPlug_and_play_flag = "4";
                    }
                    else if (crpm.RouterFlag == "M")
                    {
                        strPlug_and_play_flag = "3";
                    }
                }
                int dev_price = 0;
                if (!string.IsNullOrEmpty(quickWinModel.CustomerRegisterPanelModel.p_dev_price))
                {
                    dev_price = quickWinModel.CustomerRegisterPanelModel.p_dev_price.ToSafeInteger();
                }

                //18.6 SCPE

                if (crpm.SCPE_USE_LOC_CODE == "Y")
                {
                    crpm.L_LOC_CODE = "";
                }
                if (crpm.SCPE_USE_LOC_CODE == "N")
                {
                    crpm.L_LOC_CODE = crpm.SCPE_LOC_CODE;
                    crpm.SCPE_LOC_CODE = "";
                }

                if (crpm.SCPE_USE_ASC_CODE == "Y")
                {
                    crpm.L_ASC_CODE = "";
                }
                if (crpm.SCPE_USE_ASC_CODE == "N")
                {
                    crpm.L_ASC_CODE = crpm.SCPE_ASC_CODE;
                    crpm.SCPE_ASC_CODE = "";
                }

                //20.2
                string strInstallDate = "";
                DateTime installDate = new DateTime();
                DateTime? installDateNull = null;

                if (crpm.L_INSTALL_DATE.ToSafeString() != "")
                {
                    var split_InstallDate = crpm.L_INSTALL_DATE.ToSafeString().Split('/');
                    int installYear = split_InstallDate[2].ToSafeInteger();
                    if (installYear > 2500)
                        installYear -= 543;
                    strInstallDate = split_InstallDate[0].ToSafeString() + "/" + split_InstallDate[1].ToSafeString() + "/" + installYear.ToSafeString();
                    DateTime.TryParseExact(strInstallDate, "dd/MM/yyyy",
                            CultureInfo.GetCultureInfo("en-US"), DateTimeStyles.None, out installDate);
                    installDateNull = installDate;
                }
                else
                {
                    //R21.7 
                    DateTime dateNowTemp = DateTime.Now;
                    DateTime dateNowSetInstall = new DateTime();
                    DateTimeFormatInfo usDtfi = new CultureInfo("en-US", false).DateTimeFormat;
                    dateNowSetInstall = Convert.ToDateTime(dateNowTemp.Date, usDtfi);
                    DateTime.TryParseExact(dateNowSetInstall.ToDisplayText("dd/MM/yyyy"), "dd/MM/yyyy",
                            CultureInfo.GetCultureInfo("en-US"), DateTimeStyles.None, out installDate);
                    installDateNull = installDate;
                }

                #region Data for Log Send ExecuteStoredProc
                //22.11

                CustRegisterforLogModel custForLogModel = new CustRegisterforLogModel
                {
                    #region T.FBB_REGISTER
                    // PARAMETER FOR T.FBB_REGISTER
                    p_cust_name = string.Format("{0} {1}", firstName.Trim(), lastName.Trim()),
                    p_cust_id_card_type = crpm.L_CARD_TYPE.ToSafeString(),
                    p_cust_id_card_num = crpm.L_CARD_NO.ToSafeString(),
                    p_cust_category = crpm.CateType.ToSafeString(),
                    p_cust_sub_category = crpm.SubCateType.ToSafeString(),
                    p_cust_gender = crpm.L_GENDER.ToSafeString(),
                    p_cust_birth_dt = birthDateString.ToDate(),
                    p_cust_nationality = cusNat,
                    p_cust_title = title.ToSafeString(),
                    p_contact_first_name = cFirstName.ToSafeString().Trim(),
                    p_contact_last_name = cLastName.ToSafeString().Trim(),
                    p_contact_home_phone = crpm.L_HOME_PHONE.ToSafeString(),
                    p_contact_mobile_phone1 = phoneNo,
                    p_contact_mobile_phone2 = crpm.L_OR.ToSafeString(),
                    p_contact_email = crpm.L_EMAIL.ToSafeString(),
                    p_contact_time = crpm.L_SPECIFIC_TIME.ToSafeString(),
                    p_sale_rep = crpm.L_SALE_REP.ToSafeString(),
                    p_asc_code = crpm.L_ASC_CODE.ToSafeString(),
                    p_employee_id = crpm.L_STAFF_ID.ToSafeString(),
                    p_location_code = crpm.L_LOC_CODE.ToSafeString(),
                    p_cs_note = crpm.L_FOR_CS_TEAM.ToSafeString(),
                    p_condo_type = condoType.ToSafeString(),
                    p_condo_direction = condoDirection.ToSafeString(),
                    p_condo_limit = condoLimit.ToSafeString(),
                    p_condo_area = condoArea.ToSafeString(),
                    p_home_type = homeType.ToSafeString(),
                    p_home_area = homeArea.ToSafeString(),
                    p_document_type = crpm.DocType.ToSafeString(),
                    p_remark = crpm.L_REMARK.ToSafeString(),
                    p_cvr_id = "",//cpm.CVRID.ToSafeString(),
                    p_cvr_node = "",//cpm.CVR_NODE.ToSafeString(),
                    p_cvr_tower = "",//cpm.CVR_TOWER.ToSafeString(),
                    p_return_code = command.InterfaceCode.ToSafeString(),
                    p_return_message = command.InterfaceDesc.ToSafeString(),
                    p_return_order = command.InterfaceOrder.ToSafeString(),

                    #region R23.04 Billing Address Billing address
                    p_channel_receive_bill = crpm.CHANNEL_RECEIVE_BILL.ToSafeString(),
                    p_condition_new_doc = crpm.CONDITION_NEW_DOC.ToSafeString(),
                    p_bill_mobile_no = crpm.BILL_MOBILE_NO.ToSafeString(),
                    p_bill_cycle_info = crpm.BILL_CYCLE_INFO.ToSafeString(),
                    p_bill_channel_info = crpm.BILL_CHANNEL_INFO.ToSafeString(),

                    //R23.08 Add Parameter
                    p_bill_sum_avg_day = crpm.BILL_SUM_AVG_DAY.ToSafeDecimal(),
                    p_bill_sum_avg = crpm.BILL_SUM_AVG.ToSafeDecimal(),
                    p_bill_avg_per_day = crpm.BILL_AVG_PER_DAY.ToSafeDecimal(),
                    p_bill_total = crpm.BILL_TOTAL.ToSafeDecimal(),
                    p_bill_sum_total = crpm.BILL_SUM_TOTAL.ToSafeDecimal(),
                    #endregion

                    p_Phone_Flag = cpm.L_HAVE_FIXED_LINE.ToSafeString(),
                    p_Time_Slot = crpm.FBSSTimeSlot.TimeSlot.ToSafeString(),
                    p_Installation_Capacity = crpm.FBSSTimeSlot.InstallationCapacity.ToSafeString(),
                    p_Address_Id = cpm.Address.AddressId.ToSafeString(),
                    p_access_mode = cpm.AccessMode.ToSafeString(),
                    p_service_code = cpm.ServiceCode.ToSafeString(),
                    p_event_code = crpm.L_EVENT_CODE.ToSafeString(),

                    #endregion

                    #region T.FBB_ADDRESS
                    // PARAMETER FOR T.FBB_ADDRESS
                    p_lang = langFlag.ToSafeString(),
                    #endregion

                    #region install address
                    // Install address
                    p_install_house_no = crpm.AddressPanelModelSetup.L_HOME_NUMBER_2.ToSafeString(),
                    p_install_soi = crpm.AddressPanelModelSetup.L_SOI.ToSafeString(),
                    p_install_moo = crpm.AddressPanelModelSetup.L_MOO.ToSafeString(),
                    p_install_mooban = crpm.AddressPanelModelSetup.L_MOOBAN.ToSafeString(),
                    p_install_building_name = crpm.AddressPanelModelSetup.L_BUILD_NAME.ToSafeString(),
                    p_install_floor = crpm.AddressPanelModelSetup.L_FLOOR.ToSafeString(),
                    p_install_room = crpm.AddressPanelModelSetup.L_ROOM.ToSafeString(),
                    p_install_street_name = crpm.AddressPanelModelSetup.L_ROAD.ToSafeString(),
                    p_install_zipcode_id = crpm.AddressPanelModelSetup.ZIPCODE_ID.ToSafeString(),
                    #endregion

                    #region Billing address
                    // Billing address
                    p_bill_house_no = crpm.AddressPanelModelSendDoc.L_HOME_NUMBER_2.ToSafeString(),
                    p_bill_soi = crpm.AddressPanelModelSendDoc.L_SOI.ToSafeString(),
                    p_bill_moo = crpm.AddressPanelModelSendDoc.L_MOO.ToSafeString(),
                    p_bill_mooban = crpm.AddressPanelModelSendDoc.L_MOOBAN.ToSafeString(),
                    p_bill_building_name = crpm.AddressPanelModelSendDoc.L_BUILD_NAME.ToSafeString(),
                    p_bill_floor = crpm.AddressPanelModelSendDoc.L_FLOOR.ToSafeString(),
                    p_bill_room = crpm.AddressPanelModelSendDoc.L_ROOM.ToSafeString(),
                    p_bill_street_name = crpm.AddressPanelModelSendDoc.L_ROAD.ToSafeString(),
                    p_bill_zipcode_id = crpm.AddressPanelModelSendDoc.ZIPCODE_ID.ToSafeString(),
                    p_bill_ckecked = crpm.BillChecked.ToSafeString(),
                    #endregion

                    #region vat address

                    // Vat address
                    p_vat_house_no = crpm.AddressPanelModelVat.L_HOME_NUMBER_2.ToSafeString(),
                    p_vat_soi = crpm.AddressPanelModelVat.L_SOI.ToSafeString(),
                    p_vat_moo = crpm.AddressPanelModelVat.L_MOO.ToSafeString(),
                    p_vat_mooban = crpm.AddressPanelModelVat.L_MOOBAN.ToSafeString(),
                    p_vat_building_name = crpm.AddressPanelModelVat.L_BUILD_NAME.ToSafeString(),
                    p_vat_floor = crpm.AddressPanelModelVat.L_FLOOR.ToSafeString(),
                    p_vat_room = crpm.AddressPanelModelVat.L_ROOM.ToSafeString(),
                    p_vat_street_name = crpm.AddressPanelModelVat.L_ROAD.ToSafeString(),
                    p_vat_zipcode_id = crpm.AddressPanelModelVat.ZIPCODE_ID.ToSafeString(),
                    p_vat_ckecked = crpm.VatChecked.ToSafeString(),
                    #endregion

                    p_result_id = command.CoverageResultId.ToSafeDecimal(),

                    #region for new vas
                    p_ca_id = CA_ID,
                    p_sa_id = SA_ID,
                    p_ba_id = BA_ID,
                    p_ais_mobile = P_AIS_MOBILE, // ค่าจากหน้าบอย 
                    p_ais_non_mobile = P_AIS_NONMOBILE, // ค่าจากบอย
                    p_network_type = Productname,
                    p_service_year = ServiceYear,
                    p_request_install_date = installDateNull,
                    p_register_type = register_type,
                    p_install_address_1 = crpm.installAddress1.ToSafeString(),
                    p_install_address_2 = crpm.installAddress2.ToSafeString(),
                    p_install_address_3 = crpm.installAddress3.ToSafeString(),
                    p_install_address_4 = crpm.installAddress4.ToSafeString(),
                    p_install_address_5 = crpm.installAddress5.ToSafeString(),
                    p_number_of_pb = crpm.pbox_count.ToSafeString(),
                    p_convergence_flag = crpm.convergence_flag.ToSafeString(),
                    p_single_bill_flag = "",
                    p_time_slot_id = crpm.FBSSTimeSlot.TimeSlotId.ToSafeString(),
                    p_guid = quickWinModel.hdTransactionGuid.ToSafeString(),
                    p_voucher_pin = crpm.L_VOUCHER_PIN.ToSafeString(),
                    p_sub_location_id = crpm.AddressPanelModelVat.SUB_LOCATION_ID.ToSafeString(),
                    p_sub_contract_name = crpm.AddressPanelModelVat.SUB_CONTRACT_NAME.ToSafeString(),
                    p_install_staff_id = crpm.AddressPanelModelVat.INSTALL_STAFF_ID.ToSafeString(),
                    p_install_staff_name = crpm.AddressPanelModelVat.INSTALL_STAFF_NAME.ToSafeString(),
                    p_site_code = quickWinModel.SiteCode.ToSafeString(),
                    p_flow_flag = quickWinModel.FlowFlag.ToSafeString(),
                    p_vat_address_1 = crpm.vatAddress1.ToSafeString(),
                    p_vat_address_2 = crpm.vatAddress2.ToSafeString(),
                    p_vat_address_3 = crpm.vatAddress3.ToSafeString(),
                    p_vat_address_4 = crpm.vatAddress4.ToSafeString(),
                    p_vat_address_5 = crpm.vatAddress5.ToSafeString(),
                    p_vat_postcode = crpm.vatPostalCd.ToSafeString(),
                    p_address_flag = quickWinModel.address_flag.ToSafeString(),
                    p_relate_project_name = crpm.Project_name.ToSafeString(),
                    p_register_device = quickWinModel.Register_device.ToSafeString(),
                    p_browser_type = quickWinModel.Browser_type.ToSafeString(),
                    p_reserved_id = spm.RESERVED_ID.ToSafeString(),
                    p_job_order_type = crpm.JOB_ORDER_TYPE,
                    p_assign_job = crpm.ASSIGN_RULE,
                    p_old_isp = crpm.L_OLD_ISP.ToSafeString(),
                    p_client_ip = command.ClientIP.ToSafeString(),
                    p_splitter_flag = splitterFlag,
                    p_reserved_port_id = reservedId,
                    p_special_remark = spacialRemark,
                    p_source_system = "",
                    p_bill_media = billMedia,
                    p_pre_order_no = MemberGetMemberInfo.RefferenceNo.ToSafeString(),
                    p_voucher_desc = MemberGetMemberInfo.VoucherDesc.ToSafeString(),
                    p_campaign_project_name = MemberGetMemberInfo.CampaignProjectName.ToSafeString(),
                    p_pre_order_chanel = quickWinModel.RegisterChannel.ToSafeString(),
                    p_rental_flag = crpm.RentalFlag.ToSafeString(),
                    p_plug_and_play_flag = strPlug_and_play_flag,
                    p_dev_project_code = quickWinModel.CustomerRegisterPanelModel.p_dev_project_code,
                    p_dev_bill_to = quickWinModel.CustomerRegisterPanelModel.p_dev_bill_to,
                    p_dev_po_no = quickWinModel.CustomerRegisterPanelModel.PO_NO,
                    p_tmp_location_code = crpm.SCPE_LOC_CODE.ToSafeString(),
                    p_tmp_asc_code = crpm.SCPE_ASC_CODE.ToSafeString(),
                    p_partner_type = crpm.outType.ToSafeString(),
                    p_partner_subtype = crpm.outSubType.ToSafeString(),
                    p_mobile_by_asc = crpm.outMobileNo.ToSafeString(),
                    p_location_name = crpm.PartnerName.ToSafeString(),
                    P_PAYMENTMETHOD = quickWinModel.PayMentMethod.ToSafeString(),
                    P_TRANSACTIONID_IN = quickWinModel.PayMentOrderID.ToSafeString(),
                    P_TRANSACTIONID = quickWinModel.PayMentTranID.ToSafeString(),
                    p_sub_access_mode = crpm.SUB_ACCESS_MODE.ToSafeString(),
                    p_request_sub_flag = crpm.REQUEST_SUB_FLAG.ToSafeString(),
                    p_premium_flag = crpm.PREMIUM_FLAG.ToSafeString(),
                    p_relate_mobile_segment = crpm.RELATE_MOBILE_SEGMENT.ToSafeString(),
                    p_ref_ur_no = crpm.REF_UR_NO.ToSafeString(),
                    p_location_email_by_region = crpm.LOCATION_EMAIL_BY_REGION.ToSafeString(),
                    p_sale_staff_name = crpm.EMP_NAME.ToSafeString(),
                    p_dopa_flag = crpm.FlagDopaSubmit.ToSafeString(),
                    p_request_cs_verify_doc = crpm.FlagVarifyDocuments.ToSafeString(),
                    p_facereccog_flag = crpm.FlagFaceRecognitionSubmit.ToSafeString(),
                    p_special_account_name = crpm.SpecialAccountName.ToSafeString(),
                    p_special_account_no = crpm.SpecialAccountNo.ToSafeString(),
                    p_special_account_enddate = crpm.SpecialAccountEnddate.ToSafeString(),
                    p_special_account_group_email = crpm.SpecialAccountGroupEmail.ToSafeString(),
                    p_special_account_flag = crpm.SpecialAccountFlag.ToSafeString(),
                    p_existing_mobile_flag = crpm.Existing_Mobile.ToSafeString(),
                    p_pre_survey_date = crpm.PreSurveyDate.ToSafeString(),
                    p_pre_survey_timeslot = crpm.PreSurveyTimeslot.ToSafeString(),
                    p_replace_onu = crpm.replace_onu.ToSafeString(),
                    p_replace_wifi = crpm.replace_wifi.ToSafeString(),
                    p_number_of_mesh = crpm.mesh_count.ToSafeString(),
                    p_company_name = (oim.outTitle.ToSafeString() + " " + oim.outCompanyName.ToSafeString()).Trim(),
                    p_distribution_channel = oim.outDistChn.ToSafeString(),
                    p_channel_sales_group = oim.outChnSales.ToSafeString(),
                    p_shop_type = oim.outShopType.ToSafeString(),
                    p_shop_segment = oim.outOperatorClass.ToSafeString(),
                    p_asc_name = oim.outASCTitleThai.ToSafeString() + oim.outASCPartnerName.ToSafeString(),
                    p_asc_member_category = oim.outMemberCategory.ToSafeString(),
                    p_asc_position = oim.outPosition.ToSafeString(),
                    p_location_region = oim.outLocationRegion.ToSafeString(),
                    p_location_sub_region = oim.outLocationSubRegion.ToSafeString(),
                    p_employee_name = (oim.THFirstName.ToSafeString() + " " + oim.THLastName.ToSafeString()).Trim(),
                    p_customerpurge = "",
                    p_exceptentryfee = "",
                    p_secondinstallation = "",
                    p_amendment_flag = crpm.ServiceLevel_Flag.ToSafeString(),
                    p_service_level = crpm.ServiceLevel.ToSafeString(),
                    p_first_install_date = crpm.FBSSTimeSlot.FirstInstallDate.ToSafeString(),
                    p_first_time_slot = crpm.FBSSTimeSlot.FirstTimeSlot.ToSafeString(),
                    p_line_temp_id = crpm.LINE_TEMP_ID.ToSafeString(),
                    p_non_res_flag = crpm.Non_Res_Flag.ToSafeString(),
                    p_fmc_special_flag = cpm.SAVEORDER_FMC_SPECIAL_FLAG,
                    p_criteria_mobile = P_AIS_MOBILE,
                    p_remark_for_subcontract = crpm.Remark_For_Subcontract.ToSafeString(),
                    p_online_flag = crpm.Online_Flag.ToSafeString(),
                    p_transaction_staff = crpm.StaffPrivilegeBypass_TransactionID.ToSafeString(),
                    P_SPECIAL_SKILL = "",
                    P_TDM_CONTRACT_ID = crpm.TDMContractId.ToSafeString(),
                    P_TDM_DURATION = crpm.Duration.ToSafeString(),
                    P_TDM_CONTRACT_Flag = crpm.ContractFlag.ToSafeString(),
                    P_TDM_PENALTY_GROUP_ID = crpm.TDMPenaltyGroupId.ToSafeString(),
                    P_TDM_PENALTY_ID = crpm.TDMPenaltyId.ToSafeString(),
                    P_TDM_RULE_ID = crpm.TDMRuleId.ToSafeString(),
                    #endregion

                    p_latitude = cpm.L_LAT,
                    p_longtitude = cpm.L_LONG,

                    p_REC_REG_PACKAGE = packgaemodel,
                    p_REC_UPLOAD_FILE = picturemodel,
                    p_REC_SPLITTER_LIST = splitterModel,
                    p_REC_CPE_LIST = cpeModel,
                    p_REC_CUST_INSIGHT = custInsightModel,
                    p_REC_DCONTRACT = dcontractModel
                };

                log2 = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, custForLogModel, command.QuickWinPanelModel.CoveragePanelModel.P_MOBILE, "CustRegisterCommand", "CustRegisterCommandHandler(ExecuteStoredProc)", null, "FBB", command.CreateBy);

                #endregion

                var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FBBOR004.FBBOR004_TR",
                    out paramOut,
                       new
                       {
                           #region T.FBB_REGISTER
                           // PARAMETER FOR T.FBB_REGISTER
                           p_cust_name = string.Format("{0} {1}", firstName.Trim(), lastName.Trim()),
                           p_cust_id_card_type = crpm.L_CARD_TYPE.ToSafeString(),
                           p_cust_id_card_num = crpm.L_CARD_NO.ToSafeString(),
                           p_cust_category = crpm.CateType.ToSafeString(),
                           p_cust_sub_category = crpm.SubCateType.ToSafeString(),
                           p_cust_gender = crpm.L_GENDER.ToSafeString(),
                           p_cust_birth_dt = birthDateString.ToDate(),
                           p_cust_nationality = cusNat,
                           p_cust_title = title.ToSafeString(),
                           p_contact_first_name = cFirstName.ToSafeString().Trim(),
                           p_contact_last_name = cLastName.ToSafeString().Trim(),
                           p_contact_home_phone = crpm.L_HOME_PHONE.ToSafeString(),
                           p_contact_mobile_phone1 = phoneNo,
                           p_contact_mobile_phone2 = crpm.L_OR.ToSafeString(),
                           p_contact_email = crpm.L_EMAIL.ToSafeString(),
                           p_contact_time = crpm.L_SPECIFIC_TIME.ToSafeString(),
                           p_sale_rep = crpm.L_SALE_REP.ToSafeString(),
                           p_asc_code = crpm.L_ASC_CODE.ToSafeString(),
                           p_employee_id = crpm.L_STAFF_ID.ToSafeString(),
                           p_location_code = crpm.L_LOC_CODE.ToSafeString(),
                           p_cs_note = crpm.L_FOR_CS_TEAM.ToSafeString(),
                           p_condo_type = condoType.ToSafeString(),
                           p_condo_direction = condoDirection.ToSafeString(),
                           p_condo_limit = condoLimit.ToSafeString(),
                           p_condo_area = condoArea.ToSafeString(),
                           p_home_type = homeType.ToSafeString(),
                           p_home_area = homeArea.ToSafeString(),
                           p_document_type = crpm.DocType.ToSafeString(),
                           p_remark = crpm.L_REMARK.ToSafeString(),
                           p_cvr_id = "",//cpm.CVRID.ToSafeString(),
                           p_cvr_node = "",//cpm.CVR_NODE.ToSafeString(),
                           p_cvr_tower = "",//cpm.CVR_TOWER.ToSafeString(),
                           p_return_code = command.InterfaceCode.ToSafeString(),
                           p_return_message = command.InterfaceDesc.ToSafeString(),
                           p_return_order = command.InterfaceOrder.ToSafeString(),

                           #region R23.04 Billing Address Billing address
                           p_channel_receive_bill = crpm.CHANNEL_RECEIVE_BILL.ToSafeString(),
                           p_condition_new_doc = crpm.CONDITION_NEW_DOC.ToSafeString(),
                           p_bill_mobile_no = crpm.BILL_MOBILE_NO.ToSafeString(),
                           p_bill_cycle_info = crpm.BILL_CYCLE_INFO.ToSafeString(),
                           p_bill_channel_info = crpm.BILL_CHANNEL_INFO.ToSafeString(),

                           //R23.08 Add Parameter
                           p_bill_sum_avg_day = crpm.BILL_SUM_AVG_DAY.ToSafeDecimal(),
                           p_bill_sum_avg = crpm.BILL_SUM_AVG.ToSafeDecimal(),
                           p_bill_avg_per_day = crpm.BILL_AVG_PER_DAY.ToSafeDecimal(),
                           p_bill_total = crpm.BILL_TOTAL.ToSafeDecimal(),
                           p_bill_sum_total = crpm.BILL_SUM_TOTAL.ToSafeDecimal(),
                           #endregion

                           p_Phone_Flag = cpm.L_HAVE_FIXED_LINE.ToSafeString(),
                           p_Time_Slot = crpm.FBSSTimeSlot.TimeSlot.ToSafeString(),
                           p_Installation_Capacity = crpm.FBSSTimeSlot.InstallationCapacity.ToSafeString(),
                           p_Address_Id = cpm.Address.AddressId.ToSafeString(),
                           p_access_mode = cpm.AccessMode.ToSafeString(),
                           p_service_code = cpm.ServiceCode.ToSafeString(),
                           p_event_code = crpm.L_EVENT_CODE.ToSafeString(),

                           #endregion

                           #region T.FBB_ADDRESS
                           // PARAMETER FOR T.FBB_ADDRESS
                           p_lang = langFlag.ToSafeString(),
                           #endregion

                           #region install address
                           // Install address
                           p_install_house_no = crpm.AddressPanelModelSetup.L_HOME_NUMBER_2.ToSafeString(),
                           p_install_soi = crpm.AddressPanelModelSetup.L_SOI.ToSafeString(),
                           p_install_moo = crpm.AddressPanelModelSetup.L_MOO.ToSafeString(),
                           p_install_mooban = crpm.AddressPanelModelSetup.L_MOOBAN.ToSafeString(),
                           p_install_building_name = crpm.AddressPanelModelSetup.L_BUILD_NAME.ToSafeString(),
                           p_install_floor = crpm.AddressPanelModelSetup.L_FLOOR.ToSafeString(),
                           p_install_room = crpm.AddressPanelModelSetup.L_ROOM.ToSafeString(),
                           p_install_street_name = crpm.AddressPanelModelSetup.L_ROAD.ToSafeString(),
                           p_install_zipcode_id = crpm.AddressPanelModelSetup.ZIPCODE_ID.ToSafeString(),
                           #endregion

                           #region Billing address
                           // Billing address
                           p_bill_house_no = crpm.AddressPanelModelSendDoc.L_HOME_NUMBER_2.ToSafeString(),
                           p_bill_soi = crpm.AddressPanelModelSendDoc.L_SOI.ToSafeString(),
                           p_bill_moo = crpm.AddressPanelModelSendDoc.L_MOO.ToSafeString(),
                           p_bill_mooban = crpm.AddressPanelModelSendDoc.L_MOOBAN.ToSafeString(),
                           p_bill_building_name = crpm.AddressPanelModelSendDoc.L_BUILD_NAME.ToSafeString(),
                           p_bill_floor = crpm.AddressPanelModelSendDoc.L_FLOOR.ToSafeString(),
                           p_bill_room = crpm.AddressPanelModelSendDoc.L_ROOM.ToSafeString(),
                           p_bill_street_name = crpm.AddressPanelModelSendDoc.L_ROAD.ToSafeString(),
                           p_bill_zipcode_id = crpm.AddressPanelModelSendDoc.ZIPCODE_ID.ToSafeString(),
                           p_bill_ckecked = crpm.BillChecked.ToSafeString(),
                           #endregion

                           #region vat address

                           // Vat address
                           p_vat_house_no = crpm.AddressPanelModelVat.L_HOME_NUMBER_2.ToSafeString(),
                           p_vat_soi = crpm.AddressPanelModelVat.L_SOI.ToSafeString(),
                           p_vat_moo = crpm.AddressPanelModelVat.L_MOO.ToSafeString(),
                           p_vat_mooban = crpm.AddressPanelModelVat.L_MOOBAN.ToSafeString(),
                           p_vat_building_name = crpm.AddressPanelModelVat.L_BUILD_NAME.ToSafeString(),
                           p_vat_floor = crpm.AddressPanelModelVat.L_FLOOR.ToSafeString(),
                           p_vat_room = crpm.AddressPanelModelVat.L_ROOM.ToSafeString(),
                           p_vat_street_name = crpm.AddressPanelModelVat.L_ROAD.ToSafeString(),
                           p_vat_zipcode_id = crpm.AddressPanelModelVat.ZIPCODE_ID.ToSafeString(),
                           p_vat_ckecked = crpm.VatChecked.ToSafeString(),
                           #endregion

                           p_result_id = command.CoverageResultId.ToSafeDecimal(),

                           #region for new vas
                           p_ca_id = CA_ID,
                           p_sa_id = SA_ID,
                           p_ba_id = BA_ID,
                           p_ais_mobile = P_AIS_MOBILE, // ค่าจากหน้าบอย 
                           p_ais_non_mobile = P_AIS_NONMOBILE, // ค่าจากบอย
                           p_network_type = Productname,
                           p_service_year = ServiceYear,
                           p_request_install_date = installDateNull,
                           p_register_type = register_type,
                           p_install_address_1 = crpm.installAddress1.ToSafeString(),
                           p_install_address_2 = crpm.installAddress2.ToSafeString(),
                           p_install_address_3 = crpm.installAddress3.ToSafeString(),
                           p_install_address_4 = crpm.installAddress4.ToSafeString(),
                           p_install_address_5 = crpm.installAddress5.ToSafeString(),
                           p_number_of_pb = crpm.pbox_count.ToSafeString(),
                           p_convergence_flag = crpm.convergence_flag.ToSafeString(),
                           p_single_bill_flag = "",
                           p_time_slot_id = crpm.FBSSTimeSlot.TimeSlotId.ToSafeString(),
                           p_guid = quickWinModel.hdTransactionGuid.ToSafeString(),
                           p_voucher_pin = crpm.L_VOUCHER_PIN.ToSafeString(),
                           p_sub_location_id = crpm.AddressPanelModelVat.SUB_LOCATION_ID.ToSafeString(),
                           p_sub_contract_name = crpm.AddressPanelModelVat.SUB_CONTRACT_NAME.ToSafeString(),
                           p_install_staff_id = crpm.AddressPanelModelVat.INSTALL_STAFF_ID.ToSafeString(),
                           p_install_staff_name = crpm.AddressPanelModelVat.INSTALL_STAFF_NAME.ToSafeString(),
                           p_site_code = quickWinModel.SiteCode.ToSafeString(),
                           p_flow_flag = quickWinModel.FlowFlag.ToSafeString(),
                           p_vat_address_1 = crpm.vatAddress1.ToSafeString(),
                           p_vat_address_2 = crpm.vatAddress2.ToSafeString(),
                           p_vat_address_3 = crpm.vatAddress3.ToSafeString(),
                           p_vat_address_4 = crpm.vatAddress4.ToSafeString(),
                           p_vat_address_5 = crpm.vatAddress5.ToSafeString(),
                           p_vat_postcode = crpm.vatPostalCd.ToSafeString(),
                           p_address_flag = quickWinModel.address_flag.ToSafeString(),
                           p_relate_project_name = crpm.Project_name.ToSafeString(),
                           p_register_device = quickWinModel.Register_device.ToSafeString(),
                           p_browser_type = quickWinModel.Browser_type.ToSafeString(),
                           p_reserved_id = spm.RESERVED_ID.ToSafeString(),
                           p_job_order_type = crpm.JOB_ORDER_TYPE,
                           p_assign_job = crpm.ASSIGN_RULE,
                           p_old_isp = crpm.L_OLD_ISP.ToSafeString(),
                           p_client_ip = command.ClientIP.ToSafeString(),
                           p_splitter_flag = splitterFlag,
                           p_reserved_port_id = reservedId,
                           p_special_remark = spacialRemark,
                           p_source_system = "",
                           p_bill_media = billMedia,
                           p_pre_order_no = MemberGetMemberInfo.RefferenceNo.ToSafeString(),
                           p_voucher_desc = MemberGetMemberInfo.VoucherDesc.ToSafeString(),
                           p_campaign_project_name = MemberGetMemberInfo.CampaignProjectName.ToSafeString(),
                           p_pre_order_chanel = quickWinModel.RegisterChannel.ToSafeString(),
                           p_rental_flag = crpm.RentalFlag.ToSafeString(),
                           p_plug_and_play_flag = strPlug_and_play_flag,
                           p_dev_project_code = quickWinModel.CustomerRegisterPanelModel.p_dev_project_code,
                           p_dev_bill_to = quickWinModel.CustomerRegisterPanelModel.p_dev_bill_to,
                           p_dev_po_no = quickWinModel.CustomerRegisterPanelModel.PO_NO,
                           p_tmp_location_code = crpm.SCPE_LOC_CODE.ToSafeString(),
                           p_tmp_asc_code = crpm.SCPE_ASC_CODE.ToSafeString(),
                           p_partner_type = crpm.outType.ToSafeString(),
                           p_partner_subtype = crpm.outSubType.ToSafeString(),
                           p_mobile_by_asc = crpm.outMobileNo.ToSafeString(),
                           p_location_name = crpm.PartnerName.ToSafeString(),
                           P_PAYMENTMETHOD = quickWinModel.PayMentMethod.ToSafeString(),
                           P_TRANSACTIONID_IN = quickWinModel.PayMentOrderID.ToSafeString(),
                           P_TRANSACTIONID = quickWinModel.PayMentTranID.ToSafeString(),
                           p_sub_access_mode = crpm.SUB_ACCESS_MODE.ToSafeString(),
                           p_request_sub_flag = crpm.REQUEST_SUB_FLAG.ToSafeString(),
                           p_premium_flag = crpm.PREMIUM_FLAG.ToSafeString(),
                           p_relate_mobile_segment = crpm.RELATE_MOBILE_SEGMENT.ToSafeString(),
                           p_ref_ur_no = crpm.REF_UR_NO.ToSafeString(),
                           p_location_email_by_region = crpm.LOCATION_EMAIL_BY_REGION.ToSafeString(),
                           p_sale_staff_name = crpm.EMP_NAME.ToSafeString(),
                           p_dopa_flag = crpm.FlagDopaSubmit.ToSafeString(),
                           p_request_cs_verify_doc = crpm.FlagVarifyDocuments.ToSafeString(),
                           p_facereccog_flag = crpm.FlagFaceRecognitionSubmit.ToSafeString(),
                           p_special_account_name = crpm.SpecialAccountName.ToSafeString(),
                           p_special_account_no = crpm.SpecialAccountNo.ToSafeString(),
                           p_special_account_enddate = crpm.SpecialAccountEnddate.ToSafeString(),
                           p_special_account_group_email = crpm.SpecialAccountGroupEmail.ToSafeString(),
                           p_special_account_flag = crpm.SpecialAccountFlag.ToSafeString(),
                           p_existing_mobile_flag = crpm.Existing_Mobile.ToSafeString(),
                           p_pre_survey_date = crpm.PreSurveyDate.ToSafeString(),
                           p_pre_survey_timeslot = crpm.PreSurveyTimeslot.ToSafeString(),
                           p_replace_onu = crpm.replace_onu.ToSafeString(),
                           p_replace_wifi = crpm.replace_wifi.ToSafeString(),
                           p_number_of_mesh = crpm.mesh_count.ToSafeString(),
                           p_company_name = (oim.outTitle.ToSafeString() + " " + oim.outCompanyName.ToSafeString()).Trim(),
                           p_distribution_channel = oim.outDistChn.ToSafeString(),
                           p_channel_sales_group = oim.outChnSales.ToSafeString(),
                           p_shop_type = oim.outShopType.ToSafeString(),
                           p_shop_segment = oim.outOperatorClass.ToSafeString(),
                           p_asc_name = oim.outASCTitleThai.ToSafeString() + oim.outASCPartnerName.ToSafeString(),
                           p_asc_member_category = oim.outMemberCategory.ToSafeString(),
                           p_asc_position = oim.outPosition.ToSafeString(),
                           p_location_region = oim.outLocationRegion.ToSafeString(),
                           p_location_sub_region = oim.outLocationSubRegion.ToSafeString(),
                           p_employee_name = (oim.THFirstName.ToSafeString() + " " + oim.THLastName.ToSafeString()).Trim(),
                           p_customerpurge = "",
                           p_exceptentryfee = "",
                           p_secondinstallation = "",
                           p_amendment_flag = crpm.ServiceLevel_Flag.ToSafeString(),
                           p_service_level = crpm.ServiceLevel.ToSafeString(),
                           p_first_install_date = crpm.FBSSTimeSlot.FirstInstallDate.ToSafeString(),
                           p_first_time_slot = crpm.FBSSTimeSlot.FirstTimeSlot.ToSafeString(),
                           p_line_temp_id = crpm.LINE_TEMP_ID.ToSafeString(),
                           p_non_res_flag = crpm.Non_Res_Flag.ToSafeString(),
                           p_fmc_special_flag = cpm.SAVEORDER_FMC_SPECIAL_FLAG,
                           p_criteria_mobile = P_AIS_MOBILE,
                           p_remark_for_subcontract = crpm.Remark_For_Subcontract.ToSafeString(),
                           p_online_flag = crpm.Online_Flag.ToSafeString(),
                           p_transaction_staff = crpm.StaffPrivilegeBypass_TransactionID.ToSafeString(),
                           P_SPECIAL_SKILL = "",
                           P_TDM_CONTRACT_ID = crpm.TDMContractId.ToSafeString(),
                           P_TDM_DURATION = crpm.Duration.ToSafeString(),
                           P_TDM_CONTRACT_Flag = crpm.ContractFlag.ToSafeString(),
                           P_TDM_PENALTY_GROUP_ID = crpm.TDMPenaltyGroupId.ToSafeString(),
                           P_TDM_PENALTY_ID = crpm.TDMPenaltyId.ToSafeString(),
                           P_TDM_RULE_ID = crpm.TDMRuleId.ToSafeString(),
                           #endregion

                           p_latitude = cpm.L_LAT,
                           p_longtitude = cpm.L_LONG,

                           p_REC_REG_PACKAGE = register,
                           p_REC_UPLOAD_FILE = listfilename,
                           p_REC_SPLITTER_LIST = splitter,
                           p_REC_CPE_LIST = listcpe,
                           p_REC_CUST_INSIGHT = listCustInsight,
                           p_REC_DCONTRACT = listDcontract,

                           // Return Code
                           ret_code = ret_code,
                           v_error_msg = v_error_msg,
                           v_cust_id = v_cust_id

                       });

                command.CustomerId = ((OracleParameter)(paramOut[paramOut.Count() - 1])).Value.ToSafeString();
                //}

                //_logger.Info(ex.GetErrorMessage());


                //TODO: R19.5 Update order log for change pack field work
                try
                {
                    var saveOutgoing = new SaveOutgoingMessageCommand
                    {
                        MethodName = Constants.SbnWebService.CUSTREGISTERCOMMAND.ToString(),
                        Action = ActionType.Update,
                        SoapXml = "",
                        OrderRowId = outgoingOrderNewCommand.OrderRowId,
                        MobileNo = command.CustomerId,
                        AirOrderNo = command.InterfaceOrder,
                        TransactionId = transactionIdLogChangPack
                    };
                    _comandSaveMessage.Handle(saveOutgoing);
                }
                catch (Exception ex)
                {
                    var logx = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, "Update Error =" + ex.GetBaseException(), command.QuickWinPanelModel.no, "SaveOutgoingMessageCommand", "CustRegisterCommandHandler", null, "FBB", "FBB");
                }

                string out_ret_code = ret_code != null ? ret_code.Value.ToSafeString() : "-1";
                string out_v_error_msg = v_error_msg != null ? v_error_msg.Value.ToSafeString() : "";
                string out_xml_param = "";
                string out_v_cust_id = v_cust_id != null ? v_cust_id.Value.ToSafeString() : "";//22.11
                if (out_ret_code != "-1")
                {
                    out_xml_param = ((OracleParameter)(paramOut[paramOut.Count() - 2])).Value.ToSafeString();
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, out_xml_param, log, "Success", "", "");
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, out_v_cust_id, log2, "Success", "", "");//22.11
                }
                else
                {
                    out_xml_param = ((OracleParameter)(paramOut[paramOut.Count() - 2])).Value.ToSafeString();
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, out_xml_param, log, "Failed", out_v_error_msg, "");
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, out_v_cust_id, log2, "Failed", out_v_error_msg, "");//22.11
                }
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.Message, "");
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log2, "Failed", ex.Message, "");//22.11
            }
        }

        private QuickWinPanelModel ForTest()
        {
            var q = new QuickWinPanelModel();
            var cpm = q.CoveragePanelModel;
            var dpm = q.DisplayPackagePanelModel;
            var crpm = q.CustomerRegisterPanelModel;
            var spm = q.SummaryPanelModel;

            //spm.PackageModel.PACKAGE_CLASS_MAIN = "Main";
            //spm.PackageModel.PACKAGE_CLASS_ONTOP = null;
            var a = new PackageModel();
            a.PACKAGE_CODE = "00001"; //not null
            a.PACKAGE_CLASS = "Main"; // Ontop // Bundle //not null
            a.PACKAGE_GROUP = "Home";
            a.PRODUCT_SUBTYPE = "SWiFi"; //not null
            a.TECHNOLOGY = "SWiFi";
            a.PACKAGE_NAME = "Home 15/15"; //not null
            a.RECURRING_CHARGE = 0;
            a.INITIATION_CHARGE = 0;
            a.DISCOUNT_INITIATION_CHARGE = 0;
            a.SFF_PROMOTION_BILL_THA = "";
            a.SFF_PROMOTION_BILL_ENG = "";
            a.DOWNLOAD_SPEED = "";
            a.UPLOAD_SPEED = "";
            a.OWNER_PRODUCT = "SWiFi"; //not null
            a.VOIP_IP = "";
            a.IDD_FLAG = ""; // Y/N
            a.FAX_FLAG = ""; // Y/N
            spm.PackageModelList.Add(a);

            a = new PackageModel();
            a.PACKAGE_CODE = "00002";
            a.PACKAGE_CLASS = "Ontop";
            a.PACKAGE_GROUP = "Home";
            a.PRODUCT_SUBTYPE = "Swifi";
            a.TECHNOLOGY = "SWiFi";
            a.PACKAGE_NAME = "Installation Fee";
            a.RECURRING_CHARGE = 1;
            a.INITIATION_CHARGE = 1;
            a.DISCOUNT_INITIATION_CHARGE = 1;
            a.SFF_PROMOTION_BILL_THA = "";
            a.SFF_PROMOTION_BILL_ENG = "";
            a.DOWNLOAD_SPEED = "";
            a.OWNER_PRODUCT = "SWiFi";
            a.VOIP_IP = "";
            a.UPLOAD_SPEED = "";
            a.IDD_FLAG = "";
            a.FAX_FLAG = "";
            spm.PackageModelList.Add(a);

            a = new PackageModel();
            a.PACKAGE_CODE = "00003";
            a.PACKAGE_CLASS = "Bundle";
            a.PACKAGE_GROUP = "";
            a.PRODUCT_SUBTYPE = "Swifi";
            a.TECHNOLOGY = "SWiFi";
            a.PACKAGE_NAME = "HappinessX4";
            a.RECURRING_CHARGE = 1;
            a.INITIATION_CHARGE = 1;
            a.DISCOUNT_INITIATION_CHARGE = 1;
            a.SFF_PROMOTION_BILL_THA = "";
            a.SFF_PROMOTION_BILL_ENG = "";
            a.DOWNLOAD_SPEED = "";
            a.OWNER_PRODUCT = "SWiFi";
            a.VOIP_IP = "";
            a.UPLOAD_SPEED = "";
            a.IDD_FLAG = "";
            a.FAX_FLAG = "";
            spm.PackageModelList.Add(a);

            crpm.CateType = "R";
            crpm.L_TITLE = "คุณ";
            crpm.L_FIRST_NAME = "แสวงชัย";
            crpm.L_LAST_NAME = "สาโรจน์";
            crpm.L_CONTACT_PERSON = "คุณ แสวงชัย สาโรจน์";

            crpm.L_CARD_TYPE = "ID_CARD";
            crpm.L_CARD_NO = "1100600103254";
            crpm.L_GENDER = "M";
            crpm.L_BIRTHDAY = "01/01/1987";
            crpm.L_MOBILE = "0857320657";
            crpm.L_OR = "0857320657";

            crpm.L_HOME_PHONE = "-";
            crpm.L_EMAIL = "game@mail.com";
            crpm.L_SPECIFIC_TIME = "08:00-19:00";
            crpm.L_NATIONALITY = "ไทย";
            crpm.L_REMARK = "เกม";
            crpm.AddressPanelModelSetup.L_HOME_NUMBER_1 = "11";
            crpm.AddressPanelModelSetup.L_HOME_NUMBER_2 = "12";
            crpm.AddressPanelModelSetup.L_MOO = "2";
            crpm.AddressPanelModelSetup.L_BUILD_NAME = "เกมทาวเวอร์";
            crpm.AddressPanelModelSetup.L_FLOOR = "23";
            crpm.AddressPanelModelSetup.L_ROOM = "104";
            crpm.AddressPanelModelSetup.L_MOOBAN = null;
            crpm.AddressPanelModelSetup.L_SOI = null;
            crpm.AddressPanelModelSetup.L_ROAD = null;
            crpm.AddressPanelModelSetup.ZIPCODE_ID = "6A3C56EE181B61C0E0440000BEA816B7";

            cpm.L_LAT = "10.12215";
            cpm.L_LONG = "100.32326";
            crpm.L_ASC_CODE = null;
            crpm.L_STAFF_ID = "38867";
            crpm.L_LOC_CODE = null;
            crpm.L_SALE_REP = null;
            crpm.L_FOR_CS_TEAM = null;
            cpm.L_RESULT = "Y";
            cpm.L_FLOOR_CONDO = "4-8 ชั้น";
            crpm.L_TOP_TERRACE = null;
            crpm.L_TERRACE = null;
            crpm.L_NORTH = "เหนือ";
            crpm.L_SOUTH = null;
            crpm.L_EAST = null;
            crpm.L_WEST = null;

            crpm.L_BUILDING = "อาคารสูง";
            crpm.L_TREE = null;
            crpm.L_BILLBOARD = null;
            crpm.L_EXPRESSWAY = null;
            cpm.Address.L_BUILD_NAME = "เอทาวน์";
            cpm.L_BUILD_NAME = "เกมทาวเวอร์";

            crpm.L_TYPE_ADDR = "";
            cpm.L_FLOOR_CONDO = "23";

            cpm.CVRID = "3";
            cpm.CVR_NODE = "คอนโดลุมพินี สุขุมวิท 77";
            cpm.CVR_TOWER = "CONDOMINIUM";

            return q;
        }
    }

    public class CustRegisterJobCommandHandler : ICommandHandler<CustRegisterJobCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;

        public CustRegisterJobCommandHandler(ILogger logger
            , IEntityRepository<string> objService,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog)
        {
            _logger = logger;
            _objService = objService;
            _uow = uow;
            _intfLog = intfLog;
        }

        public void Handle(CustRegisterJobCommand command)
        {
            InterfaceLogCommand log = null;
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow,
                    _intfLog,
                    command,
                    string.IsNullOrEmpty(command.INTERNET_NO) ? command.TRANSACTIONID : command.INTERNET_NO,
                    "CustRegisterJob",
                    "CustRegisterJobCommandHandler",
                    null,
                    "FBB",
                    "");

                var packgaemodel = new PackageObjectModel
                {
                    REC_REG_PACKAGE = new Rec_Reg_PackageOracleTypeMapping[0]
                };

                var picturemodel = new PictureObjectModel
                {
                    REC_REG_PACKAGE = new PicturePackageOracleTypeMapping[0]
                };

                var splitterModel = new SplitterObjectModel
                {
                    REC_CUST_SPLITTER = new Rec_Cust_SplitterOracleTypeMapping[0]
                };

                var cpeModel = new CPEListObjectModel
                {
                    CPE_LIST_PACKAGE = new CPE_List_PackageOracleTypeMapping[0]
                };

                var custInsightModel = new CustInsightObjectModel
                {
                    CustInsight = new CustInsightOracleTypeMapping[0]
                };

                var dcontractModel = new FBBRegistDcontractRecordObjectModel
                {
                    dcontract = new FBB_Regist_Dcontract_RecordOracleTypeMapping[0]
                };

                var register = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_REC_REG_PACKAGE", "FBB_REG_PACKAGE_ARRAY", packgaemodel);
                var listfilename = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_REC_UPLOAD_FILE", "FBB_REG_UPLOAD_FILE_ARRAY", picturemodel);
                var splitter = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_REC_SPLITTER_LIST", "FBB_REG_SPLITTER_LIST_ARRAY", splitterModel);
                var listcpe = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_REC_CPE_LIST", "FBB_REG_CPE_LIST_ARRAY", cpeModel);
                var listCustInsight = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_REC_CUST_INSIGHT", "FBB_REG_CUST_INSIGHT_ARRAY", custInsightModel);
                var listdcontract = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_REC_DCONTRACT", "FBB_REC_DONTRACT_ARRAY", dcontractModel);

                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var v_error_msg = new OracleParameter();
                v_error_msg.OracleDbType = OracleDbType.Varchar2;
                v_error_msg.Size = 2000;
                v_error_msg.Direction = ParameterDirection.Output;

                var v_cust_id = new OracleParameter();
                v_cust_id.OracleDbType = OracleDbType.Varchar2;
                v_cust_id.Size = 2000;
                v_cust_id.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FBBOR004.FBBOR004_TR",
                    out paramOut,
                       new
                       {
                           p_cust_name = "",
                           p_cust_id_card_type = "",
                           p_cust_id_card_num = "",
                           p_cust_category = "",
                           p_cust_sub_category = "",
                           p_cust_gender = "",
                           p_cust_birth_dt = "",
                           p_cust_nationality = "",
                           p_cust_title = "",
                           p_contact_first_name = "",
                           p_contact_last_name = "",
                           p_contact_home_phone = "",
                           p_contact_mobile_phone1 = "",
                           p_contact_mobile_phone2 = "",
                           p_contact_email = "",
                           p_contact_time = "",
                           p_sale_rep = "",
                           p_asc_code = "",
                           p_employee_id = "",
                           p_location_code = "",
                           p_cs_note = "",
                           p_condo_type = "",
                           p_condo_direction = "",
                           p_condo_limit = "",
                           p_condo_area = "",
                           p_home_type = "",
                           p_home_area = "",
                           p_document_type = "",
                           p_remark = "",
                           p_cvr_id = "",
                           p_cvr_node = "",
                           p_cvr_tower = "",
                           p_return_code = "",
                           p_return_message = "",
                           p_return_order = command.RETURN_ORDER_NO,


                           #region R23.04 Billing Address Billing address
                           p_channel_receive_bill = "",
                           p_condition_new_doc = "",
                           p_bill_mobile_no = "",
                           p_bill_cycle_info = "",
                           p_bill_channel_info = "",

                           //R23.08 Add Parameter
                           p_bill_sum_avg_day = 0,
                           p_bill_sum_avg = 0,
                           p_bill_avg_per_day = 0,
                           p_bill_total = 0,
                           p_bill_sum_total = 0,
                           #endregion

                           p_Phone_Flag = "",
                           p_Time_Slot = "",
                           p_Installation_Capacity = "",
                           p_Address_Id = "",
                           p_access_mode = command.ACCESS_MODE.ToSafeString(),
                           p_service_code = "",
                           p_event_code = "",
                           p_lang = "",
                           p_install_house_no = "",
                           p_install_soi = "",
                           p_install_moo = "",
                           p_install_mooban = "",
                           p_install_building_name = "",
                           p_install_floor = "",
                           p_install_room = "",
                           p_install_street_name = "",
                           p_install_zipcode_id = "",
                           p_bill_house_no = "",
                           p_bill_soi = "",
                           p_bill_moo = "",
                           p_bill_mooban = "",
                           p_bill_building_name = "",
                           p_bill_floor = "",
                           p_bill_room = "",
                           p_bill_street_name = "",
                           p_bill_zipcode_id = "",
                           p_bill_ckecked = "",
                           p_vat_house_no = "",
                           p_vat_soi = "",
                           p_vat_moo = "",
                           p_vat_mooban = "",
                           p_vat_building_name = "",
                           p_vat_floor = "",
                           p_vat_room = "",
                           p_vat_street_name = "",
                           p_vat_zipcode_id = "",
                           p_vat_ckecked = "",
                           p_result_id = 0,
                           p_ca_id = "",
                           p_sa_id = "",
                           p_ba_id = "",
                           p_ais_mobile = "",
                           p_ais_non_mobile = "",
                           p_network_type = "",
                           p_service_year = "",
                           p_request_install_date = "",
                           p_register_type = command.REGISTER_TYPE.ToSafeString(),
                           p_install_address_1 = "",
                           p_install_address_2 = "",
                           p_install_address_3 = "",
                           p_install_address_4 = "",
                           p_install_address_5 = "",
                           p_number_of_pb = "",
                           p_convergence_flag = "",
                           p_single_bill_flag = "",
                           p_time_slot_id = "",
                           p_guid = "",
                           p_voucher_pin = "",
                           p_sub_location_id = "",
                           p_sub_contract_name = "",
                           p_install_staff_id = "",
                           p_install_staff_name = "",
                           p_site_code = "",
                           p_flow_flag = "",
                           p_vat_address_1 = "",
                           p_vat_address_2 = "",
                           p_vat_address_3 = "",
                           p_vat_address_4 = "",
                           p_vat_address_5 = "",
                           p_vat_postcode = "",
                           p_address_flag = "",
                           p_relate_project_name = "",
                           p_register_device = "",
                           p_browser_type = "",
                           p_reserved_id = "",
                           p_job_order_type = "",
                           p_assign_job = "",
                           p_old_isp = "",
                           p_client_ip = "",
                           p_splitter_flag = "",
                           p_reserved_port_id = command.RESERVED_PORT_ID.ToSafeString(),
                           p_special_remark = "",
                           p_source_system = "",
                           p_bill_media = "",
                           p_pre_order_no = "",
                           p_voucher_desc = "",
                           p_campaign_project_name = "",
                           p_pre_order_chanel = "",
                           p_rental_flag = "",
                           p_plug_and_play_flag = command.PLUG_AND_PLAY_FLAG,
                           p_dev_project_code = "",
                           p_dev_bill_to = "",
                           p_dev_po_no = "",
                           p_tmp_location_code = "",
                           p_tmp_asc_code = "",
                           p_partner_type = "",
                           p_partner_subtype = "",
                           p_mobile_by_asc = "",
                           p_location_name = "",
                           P_PAYMENTMETHOD = command.PAYMENTMETHOD.ToSafeString(),
                           P_TRANSACTIONID_IN = command.TRANSACTIONID_IN.ToSafeString(),
                           P_TRANSACTIONID = command.TRANSACTIONID.ToSafeString(),
                           p_sub_access_mode = "",
                           p_request_sub_flag = "",
                           p_premium_flag = "",
                           p_relate_mobile_segment = "",
                           p_ref_ur_no = "",
                           p_location_email_by_region = "",
                           p_sale_staff_name = "",
                           p_dopa_flag = "",
                           p_request_cs_verify_doc = "",
                           p_facereccog_flag = "",
                           p_special_account_name = "",
                           p_special_account_no = "",
                           p_special_account_enddate = "",
                           p_special_account_group_email = "",
                           p_special_account_flag = "",
                           p_existing_mobile_flag = "",
                           p_pre_survey_date = "",
                           p_pre_survey_timeslot = "",
                           p_replace_onu = "",
                           p_replace_wifi = "",
                           p_number_of_mesh = "",
                           p_company_name = "",
                           p_distribution_channel = "",
                           p_channel_sales_group = "",
                           p_shop_type = "",
                           p_shop_segment = "",
                           p_asc_name = "",
                           p_asc_member_category = "",
                           p_asc_position = "",
                           p_location_region = "",
                           p_location_sub_region = "",
                           p_employee_name = "",
                           p_customerpurge = "",
                           p_exceptentryfee = "",
                           p_secondinstallation = "",
                           p_amendment_flag = "",
                           p_service_level = "",
                           p_first_install_date = "",
                           p_first_time_slot = "",
                           p_line_temp_id = "",
                           p_non_res_flag = "",
                           p_fmc_special_flag = "",
                           p_criteria_mobile = "",
                           p_remark_for_subcontract = "",
                           p_online_flag = "",
                           p_transaction_staff = "",
                           P_SPECIAL_SKILL = "",
                           P_TDM_CONTRACT_ID = "",
                           P_TDM_DURATION = "",
                           P_TDM_CONTRACT_Flag = "",
                           P_TDM_PENALTY_GROUP_ID = "",
                           P_TDM_PENALTY_ID = "",
                           P_TDM_RULE_ID = "",
                           p_latitude = "",
                           p_longtitude = "",

                           p_REC_REG_PACKAGE = register,
                           p_REC_UPLOAD_FILE = listfilename,
                           p_REC_SPLITTER_LIST = splitter,
                           p_REC_CPE_LIST = listcpe,
                           p_REC_CUST_INSIGHT = listCustInsight,
                           p_REC_DCONTRACT = listdcontract,

                           // Return Code
                           ret_code = ret_code,
                           v_error_msg = v_error_msg,
                           v_cust_id = v_cust_id
                       });
                command.CUSTOMERID = ((OracleParameter)(paramOut[paramOut.Count() - 1])).Value.ToSafeString();

                string out_ret_code = ret_code != null ? ret_code.Value.ToSafeString() : "-1";
                string out_v_error_msg = v_error_msg != null ? v_error_msg.Value.ToSafeString() : "";
                string out_xml_param = "";
                if (out_ret_code != "-1")
                {
                    out_xml_param = ((OracleParameter)(paramOut[paramOut.Count() - 2])).Value.ToSafeString();
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, out_xml_param, log, "Success", "", "");
                }
                else
                {
                    out_xml_param = ((OracleParameter)(paramOut[paramOut.Count() - 2])).Value.ToSafeString();
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, out_xml_param, log, "Failed", out_v_error_msg, "");
                }

            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.Message, "");

            }
        }

    }

    #region Mapping REC_REG_PACKAGE Type Oracle
    public class PackageObjectModel : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
    {
        [OracleArrayMapping()]
        public Rec_Reg_PackageOracleTypeMapping[] REC_REG_PACKAGE { get; set; }

        private bool objectIsNull;

        public bool IsNull
        {
            get { return objectIsNull; }
        }

        public static PackageObjectModel Null
        {
            get
            {
                PackageObjectModel obj = new PackageObjectModel();
                obj.objectIsNull = true;
                return obj;
            }
        }

        public void FromCustomObject(OracleConnection con, object udt)
        {
            OracleUdt.SetValue(con, udt, 0, REC_REG_PACKAGE);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            REC_REG_PACKAGE = (Rec_Reg_PackageOracleTypeMapping[])OracleUdt.GetValue(con, udt, 0);
        }
    }

    [OracleCustomTypeMappingAttribute("FBB_REG_PACKAGE_RECORD")]
    public class Rec_Reg_PackageOracleTypeMappingFactory : IOracleCustomTypeFactory
    {
        #region IOracleCustomTypeFactory Members

        public IOracleCustomType CreateObject()
        {
            return new Rec_Reg_PackageOracleTypeMapping();
        }

        #endregion
    }
    [OracleCustomTypeMapping("FBB_REG_PACKAGE_ARRAY")]
    public class PackageObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
    {
        #region IOracleCustomTypeFactory Members
        public IOracleCustomType CreateObject()
        {
            return new PackageObjectModel();
        }

        #endregion

        #region IOracleArrayTypeFactory Members
        public Array CreateArray(int numElems)
        {
            return new Rec_Reg_PackageOracleTypeMapping[numElems];
        }

        public Array CreateStatusArray(int numElems)
        {
            return null;
        }

        #endregion
    }

    public class Rec_Reg_PackageOracleTypeMapping : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
    {
        private bool objectIsNull;

        #region Attribute Mapping
        [OracleObjectMappingAttribute("PACKAGE_CODE")]
        public string package_code { get; set; }

        [OracleObjectMappingAttribute("PACKAGE_CLASS")]
        public string package_class { get; set; }

        [OracleObjectMappingAttribute("PACKAGE_GROUP")]
        public string package_group { get; set; }

        [OracleObjectMappingAttribute("PRODUCT_SUBTYPE")]
        public string product_subtype { get; set; }

        [OracleObjectMappingAttribute("TECHNOLOGY")]
        public string technology { get; set; }

        [OracleObjectMappingAttribute("PACKAGE_NAME")]
        public string package_name { get; set; }

        [OracleObjectMappingAttribute("RECURRING_CHARGE")]
        public decimal recurring_charge { get; set; }

        [OracleObjectMappingAttribute("INITIATION_CHARGE")]
        public decimal initiation_charge { get; set; }

        [OracleObjectMappingAttribute("DISCOUNT_INITIATION")]
        public decimal discount_initiation { get; set; }

        [OracleObjectMappingAttribute("PACKAGE_BILL_THA")]
        public string package_bill_tha { get; set; }

        [OracleObjectMappingAttribute("PACKAGE_BILL_ENG")]
        public string package_bill_eng { get; set; }

        [OracleObjectMappingAttribute("DOWNLOAD_SPEED")]
        public string download_speed { get; set; }

        [OracleObjectMappingAttribute("UPLOAD_SPEED")]
        public string upload_speed { get; set; }

        [OracleObjectMappingAttribute("OWNER_PRODUCT")]
        public string owner_product { get; set; }

        [OracleObjectMappingAttribute("VOIP_IP")]
        public string voip_ip { get; set; }

        [OracleObjectMappingAttribute("IDD_FLAG")]
        public string idd_flag { get; set; }

        [OracleObjectMappingAttribute("FAX_FLAG")]
        public string fax_flag { get; set; }

        [OracleObjectMappingAttribute("MOBILE_FORWARD")]
        public string mobile_forward { get; set; }


        #endregion

        public static Rec_Reg_PackageOracleTypeMapping Null
        {
            get
            {
                Rec_Reg_PackageOracleTypeMapping obj = new Rec_Reg_PackageOracleTypeMapping();
                obj.objectIsNull = true;
                return obj;
            }
        }

        public bool IsNull
        {
            get { return objectIsNull; }
        }

        public void FromCustomObject(OracleConnection con, object udt)
        {
            OracleUdt.SetValue(con, udt, "PACKAGE_CODE", package_code);
            OracleUdt.SetValue(con, udt, "PACKAGE_CLASS", package_class);
            OracleUdt.SetValue(con, udt, "PACKAGE_GROUP", package_group);
            OracleUdt.SetValue(con, udt, "PRODUCT_SUBTYPE", product_subtype);
            OracleUdt.SetValue(con, udt, "TECHNOLOGY", technology);
            OracleUdt.SetValue(con, udt, "PACKAGE_NAME", package_name);
            OracleUdt.SetValue(con, udt, "RECURRING_CHARGE", recurring_charge);
            OracleUdt.SetValue(con, udt, "INITIATION_CHARGE", initiation_charge);
            OracleUdt.SetValue(con, udt, "DISCOUNT_INITIATION", discount_initiation);
            OracleUdt.SetValue(con, udt, "PACKAGE_BILL_THA", package_bill_tha);
            OracleUdt.SetValue(con, udt, "PACKAGE_BILL_ENG", package_bill_eng);
            OracleUdt.SetValue(con, udt, "DOWNLOAD_SPEED", download_speed);
            OracleUdt.SetValue(con, udt, "UPLOAD_SPEED", upload_speed);
            OracleUdt.SetValue(con, udt, "OWNER_PRODUCT", owner_product);
            OracleUdt.SetValue(con, udt, "VOIP_IP", voip_ip);
            OracleUdt.SetValue(con, udt, "IDD_FLAG", idd_flag);
            OracleUdt.SetValue(con, udt, "FAX_FLAG", fax_flag);
            OracleUdt.SetValue(con, udt, "MOBILE_FORWARD", mobile_forward);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            throw new NotImplementedException();
        }
    }
    #endregion

    #region Mapping ImageFilePACKAGE Type Oracle
    public class PictureObjectModel : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
    {
        [OracleArrayMapping()]
        public PicturePackageOracleTypeMapping[] REC_REG_PACKAGE { get; set; }

        private bool objectIsNull;

        public bool IsNull
        {
            get { return objectIsNull; }
        }

        public static PictureObjectModel Null
        {
            get
            {
                PictureObjectModel obj = new PictureObjectModel();
                obj.objectIsNull = true;
                return obj;
            }
        }

        public void FromCustomObject(OracleConnection con, object udt)
        {
            OracleUdt.SetValue(con, udt, 0, REC_REG_PACKAGE);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            REC_REG_PACKAGE = (PicturePackageOracleTypeMapping[])OracleUdt.GetValue(con, udt, 0);
        }
    }

    [OracleCustomTypeMappingAttribute("FBB_REG_UPLOAD_FILE_RECORD")]
    public class PicturePackageOracleTypeMappingFactory : IOracleCustomTypeFactory
    {
        #region IOracleCustomTypeFactory Members

        public IOracleCustomType CreateObject()
        {
            return new PicturePackageOracleTypeMapping();
        }

        #endregion
    }
    [OracleCustomTypeMapping("FBB_REG_UPLOAD_FILE_ARRAY")]
    public class PictureObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
    {
        #region IOracleCustomTypeFactory Members
        public IOracleCustomType CreateObject()
        {
            return new PictureObjectModel();
        }

        #endregion

        #region IOracleArrayTypeFactory Members
        public Array CreateArray(int numElems)
        {
            return new PicturePackageOracleTypeMapping[numElems];
        }

        public Array CreateStatusArray(int numElems)
        {
            return null;
        }

        #endregion
    }

    public class PicturePackageOracleTypeMapping : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
    {
        private bool objectIsNull;

        #region Attribute Mapping
        [OracleObjectMappingAttribute("FILE_NAME")]
        public string file_name { get; set; }

        #endregion

        public static PicturePackageOracleTypeMapping Null
        {
            get
            {
                PicturePackageOracleTypeMapping obj = new PicturePackageOracleTypeMapping();
                obj.objectIsNull = true;
                return obj;
            }
        }

        public bool IsNull
        {
            get { return objectIsNull; }
        }

        public void FromCustomObject(OracleConnection con, object udt)
        {
            OracleUdt.SetValue(con, udt, "FILE_NAME", file_name);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            throw new NotImplementedException();
        }
    }
    #endregion

    #region Mapping Rec_Cust_Splitter Type Oracle
    public class SplitterObjectModel : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
    {
        [OracleArrayMapping()]
        public Rec_Cust_SplitterOracleTypeMapping[] REC_CUST_SPLITTER { get; set; }

        private bool objectIsNull;

        public bool IsNull
        {
            get { return objectIsNull; }
        }

        public static SplitterObjectModel Null
        {
            get
            {
                SplitterObjectModel obj = new SplitterObjectModel();
                obj.objectIsNull = true;
                return obj;
            }
        }

        public void FromCustomObject(OracleConnection con, object udt)
        {
            OracleUdt.SetValue(con, udt, 0, REC_CUST_SPLITTER);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            REC_CUST_SPLITTER = (Rec_Cust_SplitterOracleTypeMapping[])OracleUdt.GetValue(con, udt, 0);
        }
    }

    [OracleCustomTypeMappingAttribute("FBB_REG_SPLITTER_LIST_RECORD")]
    public class Rec_Cust_SplitterOracleTypeMappingFactory : IOracleCustomTypeFactory
    {
        #region IOracleCustomTypeFactory Members

        public IOracleCustomType CreateObject()
        {
            return new Rec_Cust_SplitterOracleTypeMapping();
        }

        #endregion IOracleCustomTypeFactory Members
    }

    [OracleCustomTypeMapping("FBB_REG_SPLITTER_LIST_ARRAY")]
    public class SplitterObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
    {
        #region IOracleCustomTypeFactory Members

        public IOracleCustomType CreateObject()
        {
            return new SplitterObjectModel();
        }

        #endregion IOracleCustomTypeFactory Members

        #region IOracleArrayTypeFactory Members

        public Array CreateArray(int numElems)
        {
            return new Rec_Cust_SplitterOracleTypeMapping[numElems];
        }

        public Array CreateStatusArray(int numElems)
        {
            return null;
        }

        #endregion IOracleArrayTypeFactory Members
    }

    public class Rec_Cust_SplitterOracleTypeMapping : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
    {
        private bool objectIsNull;

        #region Attribute Mapping

        [OracleObjectMappingAttribute("SPLITTER_NAME")]
        public string splitter_name { get; set; }

        [OracleObjectMappingAttribute("DISTANCE")]
        public decimal distance { get; set; }

        [OracleObjectMappingAttribute("DISTANCE_TYPE")]
        public string distance_type { get; set; }

        [OracleObjectMappingAttribute("RESOURCE_TYPE")]
        public string resource_type { get; set; }

        #endregion Attribute Mapping

        public static Rec_Cust_SplitterOracleTypeMapping Null
        {
            get
            {
                Rec_Cust_SplitterOracleTypeMapping obj = new Rec_Cust_SplitterOracleTypeMapping();
                obj.objectIsNull = true;
                return obj;
            }
        }

        public bool IsNull
        {
            get { return objectIsNull; }
        }

        public void FromCustomObject(OracleConnection con, object udt)
        {
            OracleUdt.SetValue(con, udt, "SPLITTER_NAME", splitter_name);
            OracleUdt.SetValue(con, udt, "DISTANCE", distance);
            OracleUdt.SetValue(con, udt, "DISTANCE_TYPE", distance_type);
            OracleUdt.SetValue(con, udt, "RESOURCE_TYPE", resource_type);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            throw new NotImplementedException();
        }
    }
    #endregion Mapping Rec_Cust_Splitter Type Oracle

    #region Mapping FBB_REG_CPE_LIST_ARRAY Type Oracle

    public class CPEListObjectModel : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
    {
        [OracleArrayMapping()]
        public CPE_List_PackageOracleTypeMapping[] CPE_LIST_PACKAGE { get; set; }

        private bool objectIsNull;

        public bool IsNull
        {
            get { return objectIsNull; }
        }

        public static CPEListObjectModel Null
        {
            get
            {
                CPEListObjectModel obj = new CPEListObjectModel();
                obj.objectIsNull = true;
                return obj;
            }
        }

        public void FromCustomObject(OracleConnection con, object udt)
        {
            OracleUdt.SetValue(con, udt, 0, CPE_LIST_PACKAGE);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            CPE_LIST_PACKAGE = (CPE_List_PackageOracleTypeMapping[])OracleUdt.GetValue(con, udt, 0);
        }
    }

    [OracleCustomTypeMappingAttribute("FBB_REG_CPE_LIST_RECORD")]
    public class Air_Package_DetailOracleTypeMappingFactory : IOracleCustomTypeFactory
    {
        public IOracleCustomType CreateObject()
        {
            return new CPE_List_PackageOracleTypeMapping();
        }
    }

    [OracleCustomTypeMapping("FBB_REG_CPE_LIST_ARRAY")]
    public class AirPackageDetailObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
    {
        #region IOracleCustomTypeFactory Members

        public IOracleCustomType CreateObject()
        {
            return new CPEListObjectModel();
        }

        #endregion IOracleCustomTypeFactory Members

        #region IOracleArrayTypeFactory Members

        public Array CreateArray(int numElems)
        {
            return new CPE_List_PackageOracleTypeMapping[numElems];
        }

        public Array CreateStatusArray(int numElems)
        {
            return null;
        }

        #endregion IOracleArrayTypeFactory Members
    }

    public class CPE_List_PackageOracleTypeMapping : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
    {
        private bool objectIsNull;

        #region Attribute Mapping

        [OracleObjectMappingAttribute("CPE_TYPE")]
        public string cpe_type { get; set; }

        [OracleObjectMappingAttribute("SERIAL_NO")]
        public string serial_no { get; set; }

        [OracleObjectMappingAttribute("MAC_ADDRESS")]
        public string mac_address { get; set; }

        //20.4
        [OracleObjectMappingAttribute("STATUS_DESC")]
        public string status_desc { get; set; }

        [OracleObjectMappingAttribute("MODEL_NAME")]
        public string model_name { get; set; }

        [OracleObjectMappingAttribute("COMPANY_CODE")]
        public string company_code { get; set; }

        [OracleObjectMappingAttribute("CPE_PLANT")]
        public string cpe_plant { get; set; }

        [OracleObjectMappingAttribute("STORAGE_LOCATION")]
        public string storage_location { get; set; }

        [OracleObjectMappingAttribute("MATERIAL_CODE")]
        public string material_code { get; set; }

        [OracleObjectMappingAttribute("REGISTER_DATE")]
        public string register_date { get; set; }

        [OracleObjectMappingAttribute("FIBRENET_ID")]
        public string fibrenet_id { get; set; }

        [OracleObjectMappingAttribute("SN_PATTERN")]
        public string sn_pattern { get; set; }

        [OracleObjectMappingAttribute("SHIP_TO")]
        public string ship_to { get; set; }

        [OracleObjectMappingAttribute("WARRANTY_START_DATE")]
        public string warranty_start_date { get; set; }

        [OracleObjectMappingAttribute("WARRANTY_END_DATE")]
        public string warranty_end_date { get; set; }

        #endregion Attribute Mapping

        public static CPE_List_PackageOracleTypeMapping Null
        {
            get
            {
                CPE_List_PackageOracleTypeMapping obj = new CPE_List_PackageOracleTypeMapping();
                obj.objectIsNull = true;
                return obj;
            }
        }

        public bool IsNull
        {
            get { return objectIsNull; }
        }

        public void FromCustomObject(OracleConnection con, object udt)
        {
            OracleUdt.SetValue(con, udt, "CPE_TYPE", cpe_type);
            OracleUdt.SetValue(con, udt, "SERIAL_NO", serial_no);
            OracleUdt.SetValue(con, udt, "MAC_ADDRESS", mac_address);
            //20.4
            OracleUdt.SetValue(con, udt, "STATUS_DESC", status_desc);
            OracleUdt.SetValue(con, udt, "MODEL_NAME", model_name);
            OracleUdt.SetValue(con, udt, "COMPANY_CODE", company_code);
            OracleUdt.SetValue(con, udt, "CPE_PLANT", cpe_plant);
            OracleUdt.SetValue(con, udt, "STORAGE_LOCATION", storage_location);
            OracleUdt.SetValue(con, udt, "MATERIAL_CODE", material_code);
            OracleUdt.SetValue(con, udt, "REGISTER_DATE", register_date);
            OracleUdt.SetValue(con, udt, "FIBRENET_ID", fibrenet_id);
            OracleUdt.SetValue(con, udt, "SN_PATTERN", sn_pattern);
            OracleUdt.SetValue(con, udt, "SHIP_TO", ship_to);
            OracleUdt.SetValue(con, udt, "WARRANTY_START_DATE", warranty_start_date);
            OracleUdt.SetValue(con, udt, "WARRANTY_END_DATE", warranty_end_date);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            throw new NotImplementedException();
        }
    }

    #endregion Mapping  Air_Package_Detail_Array Type Oracle

    #region Mapping FBB_REG_CUST_INSIGHT_ARRAY Type Oracle

    public class CustInsightObjectModel : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
    {
        [OracleArrayMapping()]
        public CustInsightOracleTypeMapping[] CustInsight { get; set; }

        private bool objectIsNull;

        public bool IsNull
        {
            get { return objectIsNull; }
        }

        public static CustInsightObjectModel Null
        {
            get
            {
                CustInsightObjectModel obj = new CustInsightObjectModel();
                obj.objectIsNull = true;
                return obj;
            }
        }

        public void FromCustomObject(OracleConnection con, object udt)
        {
            OracleUdt.SetValue(con, udt, 0, CustInsight);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            CustInsight = (CustInsightOracleTypeMapping[])OracleUdt.GetValue(con, udt, 0);
        }
    }

    [OracleCustomTypeMappingAttribute("FBB_REG_CUST_INSIGHT_RECORD")]
    public class CustInsightOracleTypeMappingFactory : IOracleCustomTypeFactory
    {
        public IOracleCustomType CreateObject()
        {
            return new CustInsightOracleTypeMapping();
        }
    }

    [OracleCustomTypeMapping("FBB_REG_CUST_INSIGHT_ARRAY")]
    public class CustInsightObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
    {
        #region IOracleCustomTypeFactory Members

        public IOracleCustomType CreateObject()
        {
            return new CustInsightObjectModel();
        }

        #endregion IOracleCustomTypeFactory Members

        #region IOracleArrayTypeFactory Members

        public Array CreateArray(int numElems)
        {
            return new CustInsightOracleTypeMapping[numElems];
        }

        public Array CreateStatusArray(int numElems)
        {
            return null;
        }

        #endregion IOracleArrayTypeFactory Members
    }

    public class CustInsightOracleTypeMapping : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
    {
        private bool objectIsNull;

        #region Attribute Mapping

        [OracleObjectMappingAttribute("GROUP_ID")]
        public string group_id { get; set; }

        [OracleObjectMappingAttribute("GROUP_NAME_TH")]
        public string group_name_th { get; set; }

        [OracleObjectMappingAttribute("GROUP_NAME_EN")]
        public string group_name_en { get; set; }

        [OracleObjectMappingAttribute("QUESTION_ID")]
        public string question_id { get; set; }

        [OracleObjectMappingAttribute("QUESTION_TH")]
        public string question_th { get; set; }

        [OracleObjectMappingAttribute("QUESTION_EN")]
        public string question_en { get; set; }

        [OracleObjectMappingAttribute("ANSWER_ID")]
        public string answer_id { get; set; }

        [OracleObjectMappingAttribute("ANSWER_TH")]
        public string answer_th { get; set; }

        [OracleObjectMappingAttribute("ANSWER_EN")]
        public string answer_en { get; set; }

        [OracleObjectMappingAttribute("ANSWER_VALUE_TH")]
        public string answer_value_th { get; set; }

        [OracleObjectMappingAttribute("ANSWER_VALUE_EN")]
        public string answer_value_en { get; set; }

        [OracleObjectMappingAttribute("PARENT_ANSWER_ID")]
        public string parent_answer_id { get; set; }

        [OracleObjectMappingAttribute("ACTION_WFM")]
        public string action_wfm { get; set; }

        [OracleObjectMappingAttribute("ACTION_FOA")]
        public string action_foa { get; set; }

        #endregion Attribute Mapping

        public static CustInsightOracleTypeMapping Null
        {
            get
            {
                CustInsightOracleTypeMapping obj = new CustInsightOracleTypeMapping();
                obj.objectIsNull = true;
                return obj;
            }
        }

        public bool IsNull
        {
            get { return objectIsNull; }
        }

        public void FromCustomObject(OracleConnection con, object udt)
        {
            OracleUdt.SetValue(con, udt, "GROUP_ID", group_id);
            OracleUdt.SetValue(con, udt, "GROUP_NAME_TH", group_name_th);
            OracleUdt.SetValue(con, udt, "GROUP_NAME_EN", group_name_en);
            OracleUdt.SetValue(con, udt, "QUESTION_ID", question_id);
            OracleUdt.SetValue(con, udt, "QUESTION_TH", question_th);
            OracleUdt.SetValue(con, udt, "QUESTION_EN", question_en);
            OracleUdt.SetValue(con, udt, "ANSWER_ID", answer_id);
            OracleUdt.SetValue(con, udt, "ANSWER_TH", answer_th);
            OracleUdt.SetValue(con, udt, "ANSWER_EN", answer_en);
            OracleUdt.SetValue(con, udt, "ANSWER_VALUE_TH", answer_value_th);
            OracleUdt.SetValue(con, udt, "ANSWER_VALUE_EN", answer_value_en);
            OracleUdt.SetValue(con, udt, "PARENT_ANSWER_ID", parent_answer_id);
            OracleUdt.SetValue(con, udt, "ACTION_WFM", action_wfm);
            OracleUdt.SetValue(con, udt, "ACTION_FOA", action_foa);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            throw new NotImplementedException();
        }
    }

    #endregion Mapping  FBB_REG_CUST_INSIGHT_ARRAY Type Oracle

    #region Mapping FBB_REGIST_DCONTRACT Type Oracle
    public class FBBRegistDcontractRecordObjectModel : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
    {
        [OracleArrayMapping()]
        public FBB_Regist_Dcontract_RecordOracleTypeMapping[] dcontract { get; set; }

        private bool objectIsNull;

        public bool IsNull
        {
            get { return objectIsNull; }
        }

        public static FBBRegistDcontractRecordObjectModel Null
        {
            get
            {
                FBBRegistDcontractRecordObjectModel obj = new FBBRegistDcontractRecordObjectModel();
                obj.objectIsNull = true;
                return obj;
            }
        }

        public void FromCustomObject(OracleConnection con, object udt)
        {
            OracleUdt.SetValue(con, udt, 0, dcontract);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            dcontract = (FBB_Regist_Dcontract_RecordOracleTypeMapping[])OracleUdt.GetValue(con, udt, 0);
        }
    }

    [OracleCustomTypeMappingAttribute("FBB_REG_DONTRACT_RECORD")]
    public class FBB_Regist_Dcontract_RecordOracleTypeMappingFactory : IOracleCustomTypeFactory
    {
        #region IOracleCustomTypeFactory Members

        public IOracleCustomType CreateObject()
        {
            return new FBB_Regist_Dcontract_RecordOracleTypeMapping();
        }

        #endregion
    }
    [OracleCustomTypeMapping("FBB_REC_DONTRACT_ARRAY")]
    public class FBB_Regist_Dcontract_RecordObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
    {
        #region IOracleCustomTypeFactory Members
        public IOracleCustomType CreateObject()
        {
            return new FBBRegistDcontractRecordObjectModel();
        }

        #endregion

        #region IOracleArrayTypeFactory Members
        public Array CreateArray(int numElems)
        {
            return new FBB_Regist_Dcontract_RecordOracleTypeMapping[numElems];
        }

        public Array CreateStatusArray(int numElems)
        {
            return null;
        }

        #endregion
    }

    public class FBB_Regist_Dcontract_RecordOracleTypeMapping : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
    {
        private bool objectIsNull;

        #region Attribute Mapping
        [OracleObjectMappingAttribute("PRODUCT_SUBTYPE")]
        public string PRODUCT_SUBTYPE { get; set; }
        [OracleObjectMappingAttribute("PBOX_EXT")]
        public string PBOX_EXT { get; set; }
        [OracleObjectMappingAttribute("TDM_CONTRACT_ID")]
        public string TDM_CONTRACT_ID { get; set; }
        [OracleObjectMappingAttribute("TDM_RULE_ID")]
        public string TDM_RULE_ID { get; set; }
        [OracleObjectMappingAttribute("TDM_PENALTY_ID")]
        public string TDM_PENALTY_ID { get; set; }
        [OracleObjectMappingAttribute("TDM_PENALTY_GROUP_ID")]
        public string TDM_PENALTY_GROUP_ID { get; set; }
        [OracleObjectMappingAttribute("DURATION")]
        public string DURATION { get; set; }
        [OracleObjectMappingAttribute("CONTRACT_FLAG")]
        public string CONTRACT_FLAG { get; set; }
        [OracleObjectMappingAttribute("DEVICE_COUNT")]
        public string DEVICE_COUNT { get; set; }

        #endregion

        public static FBB_Regist_Dcontract_RecordOracleTypeMapping Null
        {
            get
            {
                FBB_Regist_Dcontract_RecordOracleTypeMapping obj = new FBB_Regist_Dcontract_RecordOracleTypeMapping();
                obj.objectIsNull = true;
                return obj;
            }
        }

        public bool IsNull
        {
            get { return objectIsNull; }
        }

        public void FromCustomObject(OracleConnection con, object udt)
        {
            OracleUdt.SetValue(con, udt, "PRODUCT_SUBTYPE", PRODUCT_SUBTYPE);
            OracleUdt.SetValue(con, udt, "PBOX_EXT", PBOX_EXT);
            OracleUdt.SetValue(con, udt, "TDM_CONTRACT_ID", TDM_CONTRACT_ID);
            OracleUdt.SetValue(con, udt, "TDM_RULE_ID", TDM_RULE_ID);
            OracleUdt.SetValue(con, udt, "TDM_PENALTY_ID", TDM_PENALTY_ID);
            OracleUdt.SetValue(con, udt, "TDM_PENALTY_GROUP_ID", TDM_PENALTY_GROUP_ID);
            OracleUdt.SetValue(con, udt, "DURATION", DURATION);
            OracleUdt.SetValue(con, udt, "CONTRACT_FLAG", CONTRACT_FLAG);
            OracleUdt.SetValue(con, udt, "DEVICE_COUNT", DEVICE_COUNT);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            throw new NotImplementedException();
        }
    }
    #endregion

    #region  Data for Log Send ExecuteStoredProc
    //22.11
    public class CustRegisterforLogModel
    {
        // PARAMETER FOR T.FBB_REGISTER
        public string p_cust_name { get; set; }
        public string p_cust_id_card_type { get; set; }
        public string p_cust_id_card_num { get; set; }
        public string p_cust_category { get; set; }
        public string p_cust_sub_category { get; set; }
        public string p_cust_gender { get; set; }
        public DateTime? p_cust_birth_dt { get; set; }
        public string p_cust_nationality { get; set; }
        public string p_cust_title { get; set; }
        public string p_contact_first_name { get; set; }
        public string p_contact_last_name { get; set; }
        public string p_contact_home_phone { get; set; }
        public string p_contact_mobile_phone1 { get; set; }
        public string p_contact_mobile_phone2 { get; set; }
        public string p_contact_email { get; set; }
        public string p_contact_time { get; set; }
        public string p_sale_rep { get; set; }
        public string p_asc_code { get; set; }
        public string p_employee_id { get; set; }
        public string p_location_code { get; set; }
        public string p_cs_note { get; set; }
        public string p_condo_type { get; set; }
        public string p_condo_direction { get; set; }
        public string p_condo_limit { get; set; }
        public string p_condo_area { get; set; }
        public string p_home_type { get; set; }
        public string p_home_area { get; set; }
        public string p_document_type { get; set; }
        public string p_remark { get; set; }
        public string p_cvr_id { get; set; }
        public string p_cvr_node { get; set; }
        public string p_cvr_tower { get; set; }
        public string p_return_code { get; set; }
        public string p_return_message { get; set; }
        public string p_return_order { get; set; }
        public string p_Phone_Flag { get; set; }
        public string p_Time_Slot { get; set; }
        public string p_Installation_Capacity { get; set; }
        public string p_Address_Id { get; set; }
        public string p_access_mode { get; set; }
        public string p_service_code { get; set; }
        public string p_event_code { get; set; }

        //R23.08 Add Paramiter Billing
        public decimal p_bill_sum_avg_day { get; set; }
        public decimal p_bill_sum_avg { get; set; }
        public decimal p_bill_avg_per_day { get; set; }
        public decimal p_bill_total { get; set; }
        public decimal p_bill_sum_total { get; set; }
        // PARAMETER FOR T.FBB_ADDRESS
        public string p_lang { get; set; }

        // Install address
        public string p_install_house_no { get; set; }
        public string p_install_soi { get; set; }
        public string p_install_moo { get; set; }
        public string p_install_mooban { get; set; }
        public string p_install_building_name { get; set; }
        public string p_install_floor { get; set; }
        public string p_install_room { get; set; }
        public string p_install_street_name { get; set; }
        public string p_install_zipcode_id { get; set; }

        // Billing address
        public string p_bill_house_no { get; set; }
        public string p_bill_soi { get; set; }
        public string p_bill_moo { get; set; }
        public string p_bill_mooban { get; set; }
        public string p_bill_building_name { get; set; }
        public string p_bill_floor { get; set; }
        public string p_bill_room { get; set; }
        public string p_bill_street_name { get; set; }
        public string p_bill_zipcode_id { get; set; }
        public string p_bill_ckecked { get; set; }

        //R23.04 Billing Address
        //ช่องทางการรับเอกสาร
        public string p_channel_receive_bill { get; set; }
        //เก็บค่าเงื่อนไขใน Drop Down list
        public string p_condition_new_doc { get; set; }
        //เก็บเเบอร์โทรสำหรับบิลที่เลือก
        public string p_bill_mobile_no { get; set; }
        //ข้อมูลรอบบิล
        public string p_bill_cycle_info { get; set; }
        //ข้อมูลการรับบิล
        public string p_bill_channel_info { get; set; }

        // Vat address
        public string p_vat_house_no { get; set; }
        public string p_vat_soi { get; set; }
        public string p_vat_moo { get; set; }
        public string p_vat_mooban { get; set; }
        public string p_vat_building_name { get; set; }
        public string p_vat_floor { get; set; }
        public string p_vat_room { get; set; }
        public string p_vat_street_name { get; set; }
        public string p_vat_zipcode_id { get; set; }
        public string p_vat_ckecked { get; set; }

        public decimal p_result_id { get; set; }

        //for new vas
        public string p_ca_id { get; set; }
        public string p_sa_id { get; set; }
        public string p_ba_id { get; set; }
        public string p_ais_mobile { get; set; } // ค่าจากหน้าบอย 
        public string p_ais_non_mobile { get; set; } // ค่าจากบอย
        public string p_network_type { get; set; }
        public string p_service_year { get; set; }
        public DateTime? p_request_install_date { get; set; }
        public string p_register_type { get; set; }
        public string p_install_address_1 { get; set; }
        public string p_install_address_2 { get; set; }
        public string p_install_address_3 { get; set; }
        public string p_install_address_4 { get; set; }
        public string p_install_address_5 { get; set; }
        public string p_number_of_pb { get; set; }
        public string p_convergence_flag { get; set; }
        public string p_single_bill_flag { get; set; }
        public string p_time_slot_id { get; set; }
        public string p_guid { get; set; }
        public string p_voucher_pin { get; set; }
        public string p_sub_location_id { get; set; }
        public string p_sub_contract_name { get; set; }
        public string p_install_staff_id { get; set; }
        public string p_install_staff_name { get; set; }
        public string p_site_code { get; set; }
        public string p_flow_flag { get; set; }
        public string p_vat_address_1 { get; set; }
        public string p_vat_address_2 { get; set; }
        public string p_vat_address_3 { get; set; }
        public string p_vat_address_4 { get; set; }
        public string p_vat_address_5 { get; set; }
        public string p_vat_postcode { get; set; }
        public string p_address_flag { get; set; }
        public string p_relate_project_name { get; set; }
        public string p_register_device { get; set; }
        public string p_browser_type { get; set; }
        public string p_reserved_id { get; set; }
        public string p_job_order_type { get; set; }
        public string p_assign_job { get; set; }
        public string p_old_isp { get; set; }
        public string p_client_ip { get; set; }
        public string p_splitter_flag { get; set; }
        public string p_reserved_port_id { get; set; }
        public string p_special_remark { get; set; }
        public string p_source_system { get; set; }
        public string p_bill_media { get; set; }
        public string p_pre_order_no { get; set; }
        public string p_voucher_desc { get; set; }
        public string p_campaign_project_name { get; set; }
        public string p_pre_order_chanel { get; set; }
        public string p_rental_flag { get; set; }
        public string p_plug_and_play_flag { get; set; }
        public string p_dev_project_code { get; set; }
        public string p_dev_bill_to { get; set; }
        public string p_dev_po_no { get; set; }
        public string p_tmp_location_code { get; set; }
        public string p_tmp_asc_code { get; set; }
        public string p_partner_type { get; set; }
        public string p_partner_subtype { get; set; }
        public string p_mobile_by_asc { get; set; }
        public string p_location_name { get; set; }
        public string P_PAYMENTMETHOD { get; set; }
        public string P_TRANSACTIONID_IN { get; set; }
        public string P_TRANSACTIONID { get; set; }
        public string p_sub_access_mode { get; set; }
        public string p_request_sub_flag { get; set; }
        public string p_premium_flag { get; set; }
        public string p_relate_mobile_segment { get; set; }
        public string p_ref_ur_no { get; set; }
        public string p_location_email_by_region { get; set; }
        public string p_sale_staff_name { get; set; }
        public string p_dopa_flag { get; set; }
        public string p_request_cs_verify_doc { get; set; }
        public string p_facereccog_flag { get; set; }
        public string p_special_account_name { get; set; }
        public string p_special_account_no { get; set; }
        public string p_special_account_enddate { get; set; }
        public string p_special_account_group_email { get; set; }
        public string p_special_account_flag { get; set; }
        public string p_existing_mobile_flag { get; set; }
        public string p_pre_survey_date { get; set; }
        public string p_pre_survey_timeslot { get; set; }
        public string p_replace_onu { get; set; }
        public string p_replace_wifi { get; set; }
        public string p_number_of_mesh { get; set; }
        public string p_company_name { get; set; }
        public string p_distribution_channel { get; set; }
        public string p_channel_sales_group { get; set; }
        public string p_shop_type { get; set; }
        public string p_shop_segment { get; set; }
        public string p_asc_name { get; set; }
        public string p_asc_member_category { get; set; }
        public string p_asc_position { get; set; }
        public string p_location_region { get; set; }
        public string p_location_sub_region { get; set; }
        public string p_employee_name { get; set; }
        public string p_customerpurge { get; set; }
        public string p_exceptentryfee { get; set; }
        public string p_secondinstallation { get; set; }
        public string p_amendment_flag { get; set; }
        public string p_service_level { get; set; }
        public string p_first_install_date { get; set; }
        public string p_first_time_slot { get; set; }
        public string p_line_temp_id { get; set; }
        public string p_non_res_flag { get; set; }
        public string p_fmc_special_flag { get; set; }
        public string p_criteria_mobile { get; set; }
        public string p_remark_for_subcontract { get; set; }
        public string p_online_flag { get; set; }
        public string p_transaction_staff { get; set; }
        public string P_SPECIAL_SKILL { get; set; }
        public string P_TDM_CONTRACT_ID { get; set; }
        public string P_TDM_DURATION { get; set; }
        public string P_TDM_CONTRACT_Flag { get; set; }
        public string P_TDM_PENALTY_GROUP_ID { get; set; }
        public string P_TDM_PENALTY_ID { get; set; }
        public string P_TDM_RULE_ID { get; set; }

        public string p_latitude { get; set; }
        public string p_longtitude { get; set; }

        // ArrayParameter
        public PackageObjectModel p_REC_REG_PACKAGE { get; set; }
        public PictureObjectModel p_REC_UPLOAD_FILE { get; set; }
        public SplitterObjectModel p_REC_SPLITTER_LIST { get; set; }
        public CPEListObjectModel p_REC_CPE_LIST { get; set; }
        public CustInsightObjectModel p_REC_CUST_INSIGHT { get; set; }
        public FBBRegistDcontractRecordObjectModel p_REC_DCONTRACT { get; set; }
    }
    #endregion
}
