using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Runtime.Serialization;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.Commons.Masters;
using WBBContract.Queries.ExWebServices;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
using WBBContract.Queries.PatchEquipment;
using WBBContract.Queries.WebServices;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.ExWebServiceModels;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBPAYGPatch_Equipment
{
    public class FBBPAYGPatchEquipment : IFBBPAYGPatchEquipment
    {
        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<UpdateFbssPatchDataConfigTblCommand> _TblCommand;
        private readonly ICommandHandler<SendMailBatchPatchDataCommand> _sendMail;
        private readonly ICommandHandler<FBBPaygPatchDataUpdateCommand> _updateCommand;
        private readonly IQueryHandler<GetTokenFbbQuery, GetTokenFbbModel> _getToken;
        private readonly MemoryCache cache = MemoryCache.Default;
        public FBBPAYGPatchEquipment(
            ILogger logger,
            IQueryProcessor queryProcessor,
            ICommandHandler<UpdateFbssPatchDataConfigTblCommand> TblCommand,
            ICommandHandler<SendMailBatchPatchDataCommand> sendMail,
            ICommandHandler<FBBPaygPatchDataUpdateCommand> updateCommand,
            IQueryHandler<GetTokenFbbQuery, GetTokenFbbModel> getToken
            )

        {
            _logger = logger;
            _queryProcessor = queryProcessor;
            _TblCommand = TblCommand;
            _sendMail = sendMail;
            _updateCommand = updateCommand;
            _getToken = getToken;
        }

        /// <summary>
        /// PKG : FBBADM.PKG_FBB_PAYG_PATCH_SN.p_search_patch_sn
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<RetCheckSerialStatus> CheckSerialStatus(CheckSerialStatusQuery model)
        {
            try
            {
                _logger.Info($"Start Check SerialStatus By SERIAL NUMBER : {string.Join(",", model.checkSerials.Select(s => s.SERIAL_NUMBER).ToList())} ");
                return _queryProcessor.Execute(model);
            }
            catch (Exception ex)
            {
                _logger.Info("CheckSerialStatus" + string.Format(" is error on execute : {0}.",
                       ex.GetErrorMessage()));
                return new List<RetCheckSerialStatus>();
            }

        }
        public List<RetCheckSerialStatus> CheckSerialStatusHVR(CheckSerialStatusHVRQuery model)
        {
            try
            {
                _logger.Info($"Start Check SerialStatus By SERIAL NUMBER (HVR) : {string.Join(",", model.checkSerials.Select(s => s.SERIAL_NUMBER).ToList())} ");
                return _queryProcessor.Execute(model);
            }
            catch (Exception ex)
            {
                _logger.Info("CheckSerialStatus (HVR) " + string.Format(" is error on execute : {0}.",
                       ex.GetErrorMessage()));
                return new List<RetCheckSerialStatus>();
            }

        }

        /// <summary>
        /// Query DATABASE Form Packages WBB.PKG_FBB_PAYG_PATCH_SN.p_search_patch_sn
        /// </summary>
        /// <param name="patchEquipment"></param>
        /// <returns>RetPatchEquipment</returns>
        public List<RetPatchEquipment> getdataPatchEquipment(PatchEquipmentQuery patchEquipment)
        {
            try
            {
                _logger.Info($"Start Get data Patch Equipment DateFrom : {patchEquipment.CreateDateFrom} , DateTo : {patchEquipment.CreateDateTo}");

                var result = _queryProcessor.Execute(patchEquipment);

                _logger.Info($"Get data Patch Equipment Total : {result.Count()} Record.");
                return result;
            }
            catch (Exception ex)
            {
                _logger.Info("Get data Patch Equipment" + string.Format(" is error on execute : {0}.",
                       ex.GetErrorMessage()));
                return new List<RetPatchEquipment>();
            }

        }

        /// <summary>
        /// call webservice new regist
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public NewRegistForSubmitFOAResponse CallWSNewRegistSAP(NewRegistForSubmitFOAQuery model)
        {
            try
            {
                _logger.Info($"Start Call WS NewRegistSAP SN : {model.ProductList.FirstOrDefault().SerialNumber} ");
                var result = _queryProcessor.Execute(model);
                _logger.Info($"Call WS NewRegistSAP SN : {model.ProductList.FirstOrDefault().SerialNumber} SUCCESS.");
                return result;
            }
            catch (Exception ex)
            {
                _logger.Info($"Call WS NewRegistSAP SN : {model.ProductList.FirstOrDefault().SerialNumber} " + string.Format(" is error on execute : {0}.",
                       ex.GetErrorMessage()));
                return new NewRegistForSubmitFOAResponse();
            }

        }


        public NewRegistForSubmitFOAS4HANAResponse CallWSNewRegistSAPS4HANA(NewRegistForSubmitFOA4HANAQuery model)
        {
            try
            {
                _logger.Info($"Start Call WS NewRegistSAP SN : {model.ProductList.FirstOrDefault().SerialNumber} ");
                var result = _queryProcessor.Execute(model);
                _logger.Info($"Call WS NewRegistSAP SN : {model.ProductList.FirstOrDefault().SerialNumber} SUCCESS.");
                return result;
            }
            catch (Exception ex)
            {
                _logger.Info($"Call WS NewRegistSAP SN : {model.ProductList.FirstOrDefault().SerialNumber} " + string.Format(" is error on execute : {0}.",
                       ex.GetErrorMessage()));
                return new NewRegistForSubmitFOAS4HANAResponse();
            }

        }





        /// <summary>
        /// get data Table FBB_CFG_LOV by LOV_VAL5
        /// </summary>
        /// <param name="LOV_VAL5"></param>
        /// <returns></returns>
        public List<LovModel> Get_FBB_CFG_LOV(string LOV_VAL5)
        {
            try
            {
                _logger.Info($"Get CFG LOV By LOV_VAL5 : {LOV_VAL5}");
                var query = new SelectLovVal5Query()
                {
                    LOV_VAL5 = LOV_VAL5
                };
                var _FbbCfgLov = _queryProcessor.Execute(query);

                return _FbbCfgLov;
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                return new List<LovModel>();
            }
        }

        public List<LovValueModel> Get_FBB_CFG_LOV_TOKEN(string LOV_TYPE, string LOV_NAME)
        {
            try
            {
                var query = new GetLovQuery()
                {
                    LovType = LOV_TYPE,
                    LovName = LOV_NAME
                };
                var _FbbCfgLov = _queryProcessor.Execute(query);

                return _FbbCfgLov;
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                return new List<LovValueModel>();
            }
        }

        /// <summary>
        /// get data Table FBSS_CONFIG_TBL_LOV
        /// </summary>
        /// <param name="_CON_TYPE"></param>
        /// <param name="_CON_NAME"></param>
        /// <returns></returns>
        public IEnumerable<FbssConfigTBL> Get_FBSS_CONFIG_TBL_LOV(string _CON_TYPE, string _CON_NAME)
        {
            try
            {
                _logger.Info($"Start Get CONFIG TBL by CONTYPE : {_CON_TYPE} and CON_NAME : {_CON_NAME}");
                var query = new GetFbssConfigTBLQuery()
                {
                    CON_TYPE = _CON_TYPE,
                    CON_NAME = _CON_NAME
                };
                var _FbssConfig = _queryProcessor.Execute(query);

                return _FbssConfig;
            }
            catch (Exception ex)
            {
                _logger.Info("Get_FBSS_CONFIG_TBL_LOV" + string.Format(" is error on execute : {0}.",
                       ex.GetErrorMessage()));
                return new List<FbssConfigTBL>();
            }

        }
        /// <summary>
        /// call webservice for Update CPE 
        /// Ws FBSSOrderServices
        /// </summary>
        /// <param name="setPatchupdateCPE"></param>
        /// <returns></returns>
        /// 
        public List<LovModel> GET_FBSS_FIXED_ASSET_CONFIG(string product_name)
        {
            var query = new GetFixedAssetConfigQuery()
            {
                Program = product_name
            };
            var _FbssConfig = _queryProcessor.Execute(query);

            return _FbssConfig;
        }
        public UpdateCPEResponse CallWSUpdateCPE(UpdateCPE setPatchupdateCPE)
        {
            try
            {
                _logger.Info($"Start Call WS UpdateCPE : {setPatchupdateCPE.CPE_LIST.SERIAL_NO}");
                var resp = _queryProcessor.Execute(setPatchupdateCPE);
                _logger.Info($"Call WS UpdateCPE : {setPatchupdateCPE.CPE_LIST.SERIAL_NO} {(resp == null ? "UNSUCCESSFUL - Response is null." : "SUCCESS")}");
                return resp;
            }
            catch (Exception ex)
            {
                _logger.Info($"Call WS UpdateCPE SN : {setPatchupdateCPE.CPE_LIST.SERIAL_NO} " + string.Format(" is error on execute : {0}.",
                       ex.GetErrorMessage()));
                return new UpdateCPEResponse();
            }

        }

        public string[] SendMail(SendMailBatchPatchDataList sendmail)
        {
            _logger.Info($"Start Sending an Email.");

            string[] result = new string[2];
            if (sendmail != null)
            {
                try
                {
                    if (sendmail.sendMailBatchPatchLists != null)
                    {
                        foreach (var item in sendmail.sendMailBatchPatchLists)
                        {
                            var data = new SendMailBatchPatchDataCommand()
                            {
                                ProcessName = item.ProcessName,
                                SendTo = item.SendTo.ToSafeString(),
                                SendFrom = item.SendFrom,
                                Subject = item.Subject,
                                Body = item.Body,
                                FromPassword = "",
                                Port = "",
                                Domaim = "",
                                IPMailServer = "",
                                FileName = item.FileName,
                                SendCC = "",
                                SendBCC = "",
                            };
                            _sendMail.Handle(data);

                            _logger.Info(string.Format("Sending an Email : {0}.", sendmail.ReturnMessage));

                            if (sendmail.ReturnMessage == "Success.")
                            {
                                result[0] = "0";
                                result[1] = "";
                            }
                            else
                            {
                                result[0] = "-1";
                                result[1] = sendmail.ReturnMessage;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //SendSms();
                    //_outErrorResult = "Sending an Email" + string.Format(" is error on execute : {0}.",
                    //					  ex.GetErrorMessage());
                    _logger.Info("Sending an Email" + string.Format(" is error on execute : {0}.",
                       ex.GetErrorMessage()));
                    result[0] = "-2";
                    result[1] = ex.GetErrorMessage();
                    //_logger.Info(ex.GetErrorMessage());
                }
                finally
                {

                }
            }


            return result;
        }

        public string GetDataPatchSNsendmail(string FileName)
        {
            try
            {
                _logger.Info($"Start Get MailTo by FileName : {FileName}");
                var model = new GetDataPatchSNsendmailQuery() { FileName = FileName };
                var result = _queryProcessor.Execute(model);
                _logger.Info($"Start Get MailTo : {result}");

                return result;
            }
            catch (Exception ex)
            {
                _logger.Info($"ERROR Get MailTo by FileName : {FileName} " + string.Format(" is error on execute : {0}.",
                       ex.ToString()));
                return "";
            }

        }
        public void UpdatePatchDataSN(FBBPaygPatchDataUpdateCommand commandupdate)
        {
            try
            {
                _logger.Info($"Start Update PatchStatus SN : {commandupdate.serialno} is {commandupdate.patchstatus}.");
                _updateCommand.Handle(commandupdate);
            }
            catch (Exception ex)
            {
                _logger.Info($"UpdatePatchData SN : {commandupdate.serialno}" + string.Format(" is error on execute : {0}.",
                       ex.GetErrorMessage()));
            }
        }
        public void TblUpdateData(UpdateFbssPatchDataConfigTblCommand queryUpdateDate)
        {
            try
            {
                _logger.Info($"Start Tbl Update Data : {queryUpdateDate.display_val}");
                _TblCommand.Handle(queryUpdateDate);
            }
            catch (Exception ex)
            {
                _logger.Info("Tbl Update Data" + string.Format(" is error on execute : {0}.",
                       ex.GetErrorMessage()));
            }
        }

        public List<ProductListSharePlex> getProductList(GetProductListSharePlexQuery q_productlist)
        {
            try
            {
                _logger.Info($"Start Get ProductList SN : {q_productlist.SERIAL_NO}");
                var result = _queryProcessor.Execute(q_productlist);
                if (result == null)
                    _logger.Info($"Get ProductList SN : {q_productlist.SERIAL_NO} not data.");
                return result;
            }
            catch (Exception ex)
            {
                _logger.Info($"GetProductList  SN : {q_productlist.SERIAL_NO}" + string.Format(" is error on execute : {0}.",
                       ex.GetErrorMessage()));
                return new List<ProductListSharePlex>();
            }
        }

        public List<ProductListHVR> getProductListHVR(GetProductListHVRQuery q_productlist)
        {
            try
            {
                _logger.Info($"Start Get ProductList SN (HVR) : {q_productlist.SERIAL_NO}");
                var result = _queryProcessor.Execute(q_productlist);
                if (result == null)
                    _logger.Info($"Get ProductList SN (HVR) : {q_productlist.SERIAL_NO} not data.");
                return result;
            }
            catch (Exception ex)
            {
                _logger.Info($"GetProductList  SN (HVR) : {q_productlist.SERIAL_NO}" + string.Format(" is error on execute : {0}.",
                       ex.GetErrorMessage()));
                return new List<ProductListHVR>();
            }
        }
    }

}