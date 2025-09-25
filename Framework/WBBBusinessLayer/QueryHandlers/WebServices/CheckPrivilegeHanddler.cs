using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class CheckPrivilegeHanddler : IQueryHandler<CheckPrivilegeQuery, CheckPrivilegeQueryModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;
        private readonly IWBBUnitOfWork _uow;
        private readonly IQueryHandler<GenerateTokenQuery, GenerateTokenModel> _GenerateTokenQuery;

        public CheckPrivilegeHanddler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_CFG_LOV> lov,
            IWBBUnitOfWork uow,
            IQueryHandler<GenerateTokenQuery, GenerateTokenModel> generateTokenQuery)
        {
            _logger = logger;
            _intfLog = intfLog;
            _lov = lov;
            _uow = uow;
            _GenerateTokenQuery = generateTokenQuery;
        }

        public CheckPrivilegeQueryModel Handle(CheckPrivilegeQuery query)
        {

            InterfaceLogCommand log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.MobileNo, "CheckPrivilege", "CheckPrivilegeQuery", "", "FBB|" + query.FullURL, "");
            CheckPrivilegeQueryModel checkPrivilegeQueryModel = new CheckPrivilegeQueryModel();

            try
            {
                CheckPrivilegeResponse result = new CheckPrivilegeResponse();
                string URL_PRIVILEGE = "";
                string FROMPASS = "";// Fixed Code scan : string PASSWORD = "";
                string USER_NAME = "";
                string SHORT_CODE_LOV_NAME = "checkPrivilege";
                string SHORT_CODE = "";
                string IP_ADDRESS = "";
                string FLAG_VARSION = "";

                if (query.ShortCodeLovName != null && query.ShortCodeLovName != "")
                    SHORT_CODE_LOV_NAME = query.ShortCodeLovName;

                URL_PRIVILEGE = (from z in _lov.Get()
                                 where z.LOV_NAME == "URL_PRIVILEGE" && z.ACTIVEFLAG == "Y"
                                 select z.LOV_VAL1).FirstOrDefault().ToSafeString();

                USER_NAME = (from z in _lov.Get()
                             where z.LOV_NAME == "checkPrivilege" && z.DISPLAY_VAL == "username" && z.ACTIVEFLAG == "Y"
                             select z.LOV_VAL1).FirstOrDefault().ToSafeString();

                IP_ADDRESS = (from z in _lov.Get()
                              where z.LOV_NAME == "checkPrivilege" && z.DISPLAY_VAL == "ipAddress" && z.ACTIVEFLAG == "Y"
                              select z.LOV_VAL1).FirstOrDefault().ToSafeString();

                FROMPASS = (from z in _lov.Get()
                            where z.LOV_NAME == "checkPrivilege" && z.DISPLAY_VAL == "password" && z.ACTIVEFLAG == "Y"
                            select z.LOV_VAL1).FirstOrDefault().ToSafeString();

                SHORT_CODE = (from z in _lov.Get()
                              where z.LOV_NAME == SHORT_CODE_LOV_NAME && z.DISPLAY_VAL == "shortcode" && z.ACTIVEFLAG == "Y"
                              select z.LOV_VAL1).FirstOrDefault().ToSafeString();

                FLAG_VARSION = (from z in _lov.Get()
                                where z.LOV_NAME == "CheckPrivilegeVersion" && z.LOV_TYPE == "FBB_CONSTANT" && z.ACTIVEFLAG == "Y"
                                select z.LOV_VAL1).FirstOrDefault().ToSafeString();

                string TransactionID = DateTime.Now.ToString("yyyyMMddHHmmss") + query.MobileNo;

                if (URL_PRIVILEGE != "" && USER_NAME != "" && IP_ADDRESS != "" && FROMPASS != "" && SHORT_CODE != "")
                {
                    CheckPrivilegeContent contents = new CheckPrivilegeContent
                    {
                        transactionID = TransactionID,
                        username = USER_NAME,
                        ipAddress = IP_ADDRESS,
                        password = FROMPASS,
                        msisdn = query.MobileNo,
                        shortcode = SHORT_CODE
                    };
                    CheckPrivilegeContent contentsForlog = new CheckPrivilegeContent
                    {
                        transactionID = TransactionID,
                        username = "",
                        ipAddress = IP_ADDRESS,
                        password = "",
                        msisdn = query.MobileNo,
                        shortcode = SHORT_CODE
                    };
                    GenerateTokenParam tokenParam = new GenerateTokenParam
                    {
                        user = USER_NAME,
                        pass = FROMPASS
                    };

                    GenerateTokenQuery generateToken = new GenerateTokenQuery()
                    {
                        contents = JsonConvert.SerializeObject(contents),
                        url_privitege = URL_PRIVILEGE,
                        genTokenParam = tokenParam,
                        accessTokenName = "checkPrivilege",
                        flag = FLAG_VARSION

                    };
                    InterfaceLogCommand log2 = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, JsonConvert.SerializeObject(contentsForlog), query.MobileNo, "CheckPrivilegePost", "CheckPrivilegeQuery", "", "FBB|" + query.FullURL, "");
                    var resultData = _GenerateTokenQuery.Handle(generateToken); ;

                    if (resultData != null && resultData.IsSuccessStatusCode)
                    {
                        string resultContent = resultData.resultData.ToString();
                        result = JsonConvert.DeserializeObject<CheckPrivilegeResponse>(resultContent);
                        if (result != null)
                        {
                            checkPrivilegeQueryModel = new CheckPrivilegeQueryModel
                            {
                                TransactionID = result.transactionID.ToSafeString(),
                                HttpStatus = result.httpStatus.ToSafeString(),
                                Status = result.status.ToSafeString(),
                                Description = result.description.ToSafeString(),
                                Msg = result.msg.ToSafeString()
                            };
                            if (result.status == "20000" && result.httpStatus == "200")
                            {
                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log2, "Success", "", "");
                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log, "Success", "", "");
                            }
                            else if (result.httpStatus == "200")
                            {
                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log2, "Success", "", "");
                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log, "Success", "", "");
                            }
                            else
                            {
                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log2, "Failed", "", "");
                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log, "Failed", "", "");
                            }
                        }
                        else
                        {
                            checkPrivilegeQueryModel.TransactionID = "";
                            checkPrivilegeQueryModel.HttpStatus = "500";
                            checkPrivilegeQueryModel.Status = "Error";
                            checkPrivilegeQueryModel.Description = "Sevice No result";
                            checkPrivilegeQueryModel.Msg = "";
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log2, "Failed", "Sevice No result", "");
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log, "Failed", "", "");
                        }

                    }
                    else if (resultData != null)
                    {
                        string resultContent = resultData.resultData.ToString();
                        result = JsonConvert.DeserializeObject<CheckPrivilegeResponse>(resultContent);
                        if (result != null)
                        {
                            checkPrivilegeQueryModel = new CheckPrivilegeQueryModel
                            {
                                TransactionID = result.transactionID.ToSafeString(),
                                HttpStatus = result.httpStatus.ToSafeString(),
                                Status = result.status.ToSafeString(),
                                Description = result.description.ToSafeString(),
                                Msg = result.msg.ToSafeString()
                            };
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, JsonConvert.SerializeObject(checkPrivilegeQueryModel), log2, "Failed", "Call Service not Success", "");
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log, "Failed", "", "");
                        }
                        else
                        {
                            checkPrivilegeQueryModel.TransactionID = "";
                            checkPrivilegeQueryModel.HttpStatus = "500";
                            checkPrivilegeQueryModel.Status = "Error";
                            checkPrivilegeQueryModel.Description = "Call Service not Success";
                            checkPrivilegeQueryModel.Msg = "resultContent is null";
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, JsonConvert.SerializeObject(checkPrivilegeQueryModel), log2, "Failed", "Call Service not Success", "");
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log, "Failed", "", "");
                        }
                    }
                    else
                    {
                        checkPrivilegeQueryModel.TransactionID = "";
                        checkPrivilegeQueryModel.HttpStatus = "500";
                        checkPrivilegeQueryModel.Status = "Error";
                        checkPrivilegeQueryModel.Description = "Call Service not Success";
                        checkPrivilegeQueryModel.Msg = "resultData is null";
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, JsonConvert.SerializeObject(checkPrivilegeQueryModel), log2, "Failed", "Call Service not Success", "");
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, JsonConvert.SerializeObject(checkPrivilegeQueryModel), log, "Failed", "", "");
                    }

                }
                else
                {
                    checkPrivilegeQueryModel.TransactionID = "";
                    checkPrivilegeQueryModel.HttpStatus = "500";
                    checkPrivilegeQueryModel.Status = "Error";
                    checkPrivilegeQueryModel.Description = "Config Lov is null";
                    checkPrivilegeQueryModel.Msg = "LOVData is null";
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, JsonConvert.SerializeObject(checkPrivilegeQueryModel), log, "Failed", "", "");
                }
            }
            catch (Exception ex)
            {
                checkPrivilegeQueryModel.TransactionID = "";
                checkPrivilegeQueryModel.HttpStatus = "500";
                checkPrivilegeQueryModel.Status = "Error";
                checkPrivilegeQueryModel.Description = ex.Message;
                checkPrivilegeQueryModel.Msg = "";
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, JsonConvert.SerializeObject(checkPrivilegeQueryModel), log, "Failed", ex.GetErrorMessage(), "");
            }
            return checkPrivilegeQueryModel;
        }
    }

    public class CheckPrivilegePointHanddler : IQueryHandler<CheckPrivilegePointQuery, CheckPrivilegePointQueryModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;
        private readonly IWBBUnitOfWork _uow;
        private readonly IQueryHandler<GenerateTokenQuery, GenerateTokenModel> _GenerateTokenQuery;

        public CheckPrivilegePointHanddler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_CFG_LOV> lov,
            IWBBUnitOfWork uow,
            IQueryHandler<GenerateTokenQuery, GenerateTokenModel> generateTokenQuery)
        {
            _logger = logger;
            _intfLog = intfLog;
            _lov = lov;
            _uow = uow;
            _GenerateTokenQuery = generateTokenQuery;
        }

        public CheckPrivilegePointQueryModel Handle(CheckPrivilegePointQuery query)
        {

            InterfaceLogCommand log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.MobileNo, "CheckPrivilege", "CheckPrivilegeQuery", "", "FBB|" + query.FullURL, "");
            CheckPrivilegePointQueryModel checkPrivilegePointQueryModel = new CheckPrivilegePointQueryModel();

            try
            {
                CheckPrivilegePointResponse result = new CheckPrivilegePointResponse();
                string URL_PRIVILEGE = "";
                string FROMPASS = "";// Fixed Code scan : string PASSWORD = "";
                string USER_NAME = "";
                string IP_ADDRESS = "";
                string FLAG_VARSION = "";

                URL_PRIVILEGE = (from z in _lov.Get()
                                 where z.LOV_NAME == "getPointSet" && z.DISPLAY_VAL == "url" && z.LOV_VAL5 == "FBBOR041" && z.LOV_TYPE == "FBB_CONSTANT" && z.ACTIVEFLAG == "Y"
                                 select z.LOV_VAL1).FirstOrDefault().ToSafeString();

                USER_NAME = (from z in _lov.Get()
                             where z.LOV_NAME == "getPointSet" && z.DISPLAY_VAL == "username" && z.LOV_VAL5 == "FBBOR041" && z.LOV_TYPE == "FBB_CONSTANT" && z.ACTIVEFLAG == "Y"
                             select z.LOV_VAL1).FirstOrDefault().ToSafeString();

                IP_ADDRESS = (from z in _lov.Get()
                              where z.LOV_NAME == "getPointSet" && z.DISPLAY_VAL == "ipAddress" && z.LOV_VAL5 == "FBBOR041" && z.LOV_TYPE == "FBB_CONSTANT" && z.ACTIVEFLAG == "Y"
                              select z.LOV_VAL1).FirstOrDefault().ToSafeString();

                FROMPASS = (from z in _lov.Get()
                            where z.LOV_NAME == "getPointSet" && z.DISPLAY_VAL == "password" && z.LOV_VAL5 == "FBBOR041" && z.LOV_TYPE == "FBB_CONSTANT" && z.ACTIVEFLAG == "Y"
                            select z.LOV_VAL1).FirstOrDefault().ToSafeString();

                FLAG_VARSION = (from z in _lov.Get()
                                where z.LOV_NAME == "CheckPrivilegeVersion" && z.LOV_TYPE == "FBB_CONSTANT" && z.ACTIVEFLAG == "Y"
                                select z.LOV_VAL1).FirstOrDefault().ToSafeString();

                if (URL_PRIVILEGE != "" && USER_NAME != "" && IP_ADDRESS != "" && FROMPASS != "")
                {
                    CheckPrivilegeContent contents = new CheckPrivilegeContent
                    {
                        transactionID = query.PaymentOrderID,
                        username = USER_NAME,
                        ipAddress = IP_ADDRESS,
                        password = FROMPASS,
                        msisdn = query.MobileNo + "|" + query.InternetNo
                    };
                    GenerateTokenParam tokenParam = new GenerateTokenParam
                    {
                        user = USER_NAME,
                        pass = FROMPASS
                    };

                    GenerateTokenQuery generateToken = new GenerateTokenQuery()
                    {
                        contents = JsonConvert.SerializeObject(contents),
                        url_privitege = URL_PRIVILEGE,
                        genTokenParam = tokenParam,
                        accessTokenName = "getPointSet",
                        flag = FLAG_VARSION

                    };
                    InterfaceLogCommand log2 = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, JsonConvert.SerializeObject(contents), query.MobileNo, "CheckPrivilegePoint", "CheckPrivilegePointQuery", "", "FBB|" + query.FullURL, "");
                    var resultData = _GenerateTokenQuery.Handle(generateToken); ;

                    if (resultData != null && resultData.IsSuccessStatusCode)
                    {
                        string resultContent = resultData.resultData.ToString();
                        result = JsonConvert.DeserializeObject<CheckPrivilegePointResponse>(resultContent);
                        if (result != null)
                        {
                            List<PrivilegePoint> PrivilegePointList = new List<PrivilegePoint>();
                            if (result.pointArr != null && result.pointArr.Count() > 0)
                            {
                                foreach (var item in result.pointArr)
                                {
                                    PrivilegePoint privilegePoint = new PrivilegePoint()
                                    {
                                        msisdn = item.msisdn,
                                        points = item.points,
                                        pointsBonus = item.pointsBonus
                                    };
                                    PrivilegePointList.Add(privilegePoint);
                                }
                            }
                            checkPrivilegePointQueryModel = new CheckPrivilegePointQueryModel
                            {
                                TransactionID = result.transactionID.ToSafeString(),
                                HttpStatus = result.httpStatus.ToSafeString(),
                                Status = result.status.ToSafeString(),
                                Description = result.description.ToSafeString(),
                                PrivilegePointList = PrivilegePointList
                            };
                            if (result.status == "20000" && result.httpStatus == "200")
                            {
                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log2, "Success", "", "");
                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log, "Success", "", "");
                            }
                            else if (result.httpStatus == "200")
                            {
                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log2, "Success", "", "");
                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log, "Success", "", "");
                            }
                            else
                            {
                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log2, "Failed", "", "");
                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log, "Failed", "", "");
                            }
                        }
                        else
                        {
                            checkPrivilegePointQueryModel.TransactionID = "";
                            checkPrivilegePointQueryModel.HttpStatus = "500";
                            checkPrivilegePointQueryModel.Status = "Error";
                            checkPrivilegePointQueryModel.Description = "Sevice No result";
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log2, "Failed", "Sevice No result", "");
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log, "Failed", "", "");
                        }

                    }
                    else if (resultData != null)
                    {
                        string resultContent = resultData.resultData.ToString();
                        result = JsonConvert.DeserializeObject<CheckPrivilegePointResponse>(resultContent);
                        if (result != null)
                        {
                            List<PrivilegePoint> PrivilegePointList = new List<PrivilegePoint>();
                            if (result.pointArr != null && result.pointArr.Count() > 0)
                            {
                                foreach (var item in result.pointArr)
                                {
                                    PrivilegePoint privilegePoint = new PrivilegePoint()
                                    {
                                        msisdn = item.msisdn,
                                        points = item.points,
                                        pointsBonus = item.pointsBonus
                                    };
                                    PrivilegePointList.Add(privilegePoint);
                                }
                            }
                            checkPrivilegePointQueryModel = new CheckPrivilegePointQueryModel
                            {
                                TransactionID = result.transactionID.ToSafeString(),
                                HttpStatus = result.httpStatus.ToSafeString(),
                                Status = result.status.ToSafeString(),
                                Description = result.description.ToSafeString(),
                                PrivilegePointList = PrivilegePointList
                            };
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, JsonConvert.SerializeObject(checkPrivilegePointQueryModel), log2, "Failed", "Call Service not Success", "");
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log, "Failed", "", "");
                        }
                        else
                        {
                            checkPrivilegePointQueryModel.TransactionID = "";
                            checkPrivilegePointQueryModel.HttpStatus = "500";
                            checkPrivilegePointQueryModel.Status = "Error";
                            checkPrivilegePointQueryModel.Description = "Call Service not Success";
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, JsonConvert.SerializeObject(checkPrivilegePointQueryModel), log2, "Failed", "Call Service not Success", "");
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log, "Failed", "", "");
                        }
                    }
                    else
                    {
                        checkPrivilegePointQueryModel.TransactionID = "";
                        checkPrivilegePointQueryModel.HttpStatus = "500";
                        checkPrivilegePointQueryModel.Status = "Error";
                        checkPrivilegePointQueryModel.Description = "Call Service not Success";
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, JsonConvert.SerializeObject(checkPrivilegePointQueryModel), log2, "Failed", "Call Service not Success", "");
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, JsonConvert.SerializeObject(checkPrivilegePointQueryModel), log, "Failed", "", "");
                    }


                }
                else
                {
                    checkPrivilegePointQueryModel.TransactionID = "";
                    checkPrivilegePointQueryModel.HttpStatus = "500";
                    checkPrivilegePointQueryModel.Status = "Error";
                    checkPrivilegePointQueryModel.Description = "Config Lov is null";
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, JsonConvert.SerializeObject(checkPrivilegePointQueryModel), log, "Failed", "", "");
                }
            }
            catch (Exception ex)
            {
                checkPrivilegePointQueryModel.TransactionID = "";
                checkPrivilegePointQueryModel.HttpStatus = "500";
                checkPrivilegePointQueryModel.Status = "Error";
                checkPrivilegePointQueryModel.Description = ex.Message;
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, JsonConvert.SerializeObject(checkPrivilegePointQueryModel), log, "Failed", ex.GetErrorMessage(), "");
            }
            return checkPrivilegePointQueryModel;
        }
    }

    public class PrivilegeRedeemPointHanddler : IQueryHandler<PrivilegeRedeemPointQuery, PrivilegeRedeemPointQueryModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;
        private readonly IWBBUnitOfWork _uow;
        private readonly IQueryHandler<GenerateTokenQuery, GenerateTokenModel> _GenerateTokenQuery;

        public PrivilegeRedeemPointHanddler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_CFG_LOV> lov,
            IWBBUnitOfWork uow,
            IQueryHandler<GenerateTokenQuery, GenerateTokenModel> generateTokenQuery)
        {
            _logger = logger;
            _intfLog = intfLog;
            _lov = lov;
            _uow = uow;
            _GenerateTokenQuery = generateTokenQuery;
        }

        public PrivilegeRedeemPointQueryModel Handle(PrivilegeRedeemPointQuery query)
        {

            InterfaceLogCommand log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.MobileNo, "PrivilegeRedeemPoint", "PrivilegeRedeemPointQuery", "", "FBB|" + query.FullURL, "");
            PrivilegeRedeemPointQueryModel privilegeRedeemPointQueryModel = new PrivilegeRedeemPointQueryModel();

            try
            {
                PrivilegeRedeemPointResponse result = new PrivilegeRedeemPointResponse();
                string URL_PRIVILEGE = "";
                string FROMPASS = "";// Fixed Code scan : string PASSWORD = "";
                string USER_NAME = "";
                string IP_ADDRESS = "";
                string SHORTCODE = "";
                string FLAG_VARSION = "";

                URL_PRIVILEGE = (from z in _lov.Get()
                                 where z.LOV_NAME == "requestPrivilegeBarcode" && z.DISPLAY_VAL == "url" && z.LOV_VAL5 == "FBBOR041" && z.LOV_TYPE == "FBB_CONSTANT" && z.ACTIVEFLAG == "Y"
                                 select z.LOV_VAL1).FirstOrDefault().ToSafeString();

                USER_NAME = (from z in _lov.Get()
                             where z.LOV_NAME == "requestPrivilegeBarcode" && z.DISPLAY_VAL == "username" && z.LOV_VAL5 == "FBBOR041" && z.LOV_TYPE == "FBB_CONSTANT" && z.ACTIVEFLAG == "Y"
                             select z.LOV_VAL1).FirstOrDefault().ToSafeString();

                IP_ADDRESS = (from z in _lov.Get()
                              where z.LOV_NAME == "requestPrivilegeBarcode" && z.DISPLAY_VAL == "ipAddress" && z.LOV_VAL5 == "FBBOR041" && z.LOV_TYPE == "FBB_CONSTANT" && z.ACTIVEFLAG == "Y"
                              select z.LOV_VAL1).FirstOrDefault().ToSafeString();

                FROMPASS = (from z in _lov.Get()
                            where z.LOV_NAME == "requestPrivilegeBarcode" && z.DISPLAY_VAL == "password" && z.LOV_VAL5 == "FBBOR041" && z.LOV_TYPE == "FBB_CONSTANT" && z.ACTIVEFLAG == "Y"
                            select z.LOV_VAL1).FirstOrDefault().ToSafeString();

                FLAG_VARSION = (from z in _lov.Get()
                                where z.LOV_NAME == "CheckPrivilegeVersion" && z.LOV_TYPE == "FBB_CONSTANT" && z.ACTIVEFLAG == "Y"
                                select z.LOV_VAL1).FirstOrDefault().ToSafeString();

                if (query.SFFPromotioncode.ToSafeString() == "")
                {
                    SHORTCODE = (from z in _lov.Get()
                                 where z.LOV_NAME == "requestPrivilegeBarcode" && z.DISPLAY_VAL == "shortcode" && z.LOV_VAL5 == "FBBOR041" && z.LOV_TYPE == "FBB_CONSTANT" && z.ACTIVEFLAG == "Y"
                                 select z.LOV_VAL1).FirstOrDefault().ToSafeString();
                }
                else
                {
                    string SffPromotionConfig = (from z in _lov.Get()
                                                 where z.LOV_NAME == "AIS_POINT_PROMOTION_CODE" && z.LOV_VAL1 == query.SFFPromotioncode && z.ACTIVEFLAG == "Y"
                                                 select z.LOV_VAL2).FirstOrDefault().ToSafeString();
                    if (SffPromotionConfig != "")
                    {
                        SHORTCODE = (from z in _lov.Get()
                                     where z.LOV_NAME == "AIS_POINT_PROMOTION_CODE" && z.LOV_VAL2 == SffPromotionConfig && z.ACTIVEFLAG == "Y"
                                     select z.LOV_VAL3).FirstOrDefault().ToSafeString();
                    }
                }

                if (URL_PRIVILEGE != "" && USER_NAME != "" && IP_ADDRESS != "" && FROMPASS != "" && SHORTCODE != "")
                {
                    CheckPrivilegeContent contents = new CheckPrivilegeContent
                    {
                        transactionID = query.PaymentOrderID,
                        username = USER_NAME,
                        ipAddress = IP_ADDRESS,
                        password = FROMPASS,
                        msisdn = query.MobileNo,
                        shortcode = SHORTCODE
                    };

                    CheckPrivilegeContent contentsForlog = new CheckPrivilegeContent
                    {
                        transactionID = query.PaymentOrderID,
                        username = "",
                        ipAddress = IP_ADDRESS,
                        password = "",
                        msisdn = query.MobileNo,
                        shortcode = SHORTCODE
                    };
                    GenerateTokenParam tokenParam = new GenerateTokenParam
                    {
                        user = USER_NAME,
                        pass = FROMPASS
                    };

                    GenerateTokenQuery generateToken = new GenerateTokenQuery()
                    {
                        contents = JsonConvert.SerializeObject(contents),
                        url_privitege = URL_PRIVILEGE,
                        genTokenParam = tokenParam,
                        accessTokenName = "requestPrivilegeBarcode",
                        flag = FLAG_VARSION

                    };

                    InterfaceLogCommand log2 = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, JsonConvert.SerializeObject(contentsForlog), query.MobileNo, "PrivilegeRedeemPoint", "PrivilegeRedeemPointQuery (Service PrivApiRestful)", "", "FBB|" + query.FullURL, "");

                    var resultData = _GenerateTokenQuery.Handle(generateToken); ;

                    if (resultData != null && resultData.IsSuccessStatusCode)
                    {
                        string resultContent = resultData.resultData.ToString();
                        result = JsonConvert.DeserializeObject<PrivilegeRedeemPointResponse>(resultContent);
                        if (result != null)
                        {
                            privilegeRedeemPointQueryModel = new PrivilegeRedeemPointQueryModel
                            {
                                TransactionID = result.transactionID.ToSafeString(),
                                HttpStatus = result.httpStatus.ToSafeString(),
                                Status = result.status.ToSafeString(),
                                Description = result.description.ToSafeString(),
                                Msg = result.msg.ToSafeString(),
                                MsgBarcode = result.msgBarcode.ToSafeString()
                            };
                            if (result.status == "20000" && result.httpStatus == "200")
                            {
                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log2, "Success", "", "");
                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log, "Success", "", "");
                            }
                            else if (result.httpStatus == "200")
                            {
                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log2, "Success", "", "");
                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log, "Success", "", "");
                            }
                            else
                            {
                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log2, "Failed", "", "");
                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log, "Failed", "", "");
                            }
                        }
                        else
                        {
                            privilegeRedeemPointQueryModel.TransactionID = "";
                            privilegeRedeemPointQueryModel.HttpStatus = "500";
                            privilegeRedeemPointQueryModel.Status = "Error";
                            privilegeRedeemPointQueryModel.Description = "Sevice No result";
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log2, "Failed", "Sevice No result", "");
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log, "Failed", "", "");
                        }

                    }
                    else if (resultData != null)
                    {
                        string resultContent = resultData.resultData.ToString();
                        result = JsonConvert.DeserializeObject<PrivilegeRedeemPointResponse>(resultContent);
                        if (result != null)
                        {
                            privilegeRedeemPointQueryModel = new PrivilegeRedeemPointQueryModel
                            {
                                TransactionID = result.transactionID.ToSafeString(),
                                HttpStatus = result.httpStatus.ToSafeString(),
                                Status = result.status.ToSafeString(),
                                Description = result.description.ToSafeString(),
                                Msg = result.msg.ToSafeString(),
                                MsgBarcode = result.msgBarcode.ToSafeString()
                            };
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, JsonConvert.SerializeObject(privilegeRedeemPointQueryModel), log2, "Failed", "Call Service not Success", "");
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log, "Failed", "", "");
                        }
                        else
                        {
                            privilegeRedeemPointQueryModel.TransactionID = "";
                            privilegeRedeemPointQueryModel.HttpStatus = "500";
                            privilegeRedeemPointQueryModel.Status = "Error";
                            privilegeRedeemPointQueryModel.Description = "Call Service not Success";
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, JsonConvert.SerializeObject(privilegeRedeemPointQueryModel), log2, "Failed", "Call Service not Success", "");
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log, "Failed", "", "");
                        }
                    }
                    else
                    {
                        privilegeRedeemPointQueryModel.TransactionID = "";
                        privilegeRedeemPointQueryModel.HttpStatus = "500";
                        privilegeRedeemPointQueryModel.Status = "Error";
                        privilegeRedeemPointQueryModel.Description = "Call Service not Success";
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, JsonConvert.SerializeObject(privilegeRedeemPointQueryModel), log2, "Failed", "Call Service not Success", "");
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, JsonConvert.SerializeObject(privilegeRedeemPointQueryModel), log, "Failed", "", "");
                    }


                }
                else
                {
                    privilegeRedeemPointQueryModel.TransactionID = "";
                    privilegeRedeemPointQueryModel.HttpStatus = "500";
                    privilegeRedeemPointQueryModel.Status = "Error";
                    privilegeRedeemPointQueryModel.Description = "Config Lov is null";
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, JsonConvert.SerializeObject(privilegeRedeemPointQueryModel), log, "Failed", "", "");
                }
            }
            catch (Exception ex)
            {
                privilegeRedeemPointQueryModel.TransactionID = "";
                privilegeRedeemPointQueryModel.HttpStatus = "500";
                privilegeRedeemPointQueryModel.Status = "Error";
                privilegeRedeemPointQueryModel.Description = ex.Message;
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, JsonConvert.SerializeObject(privilegeRedeemPointQueryModel), log, "Failed", ex.GetErrorMessage(), "");
            }
            return privilegeRedeemPointQueryModel;
        }
    }

    public class CheckPrivilegeContent
    {
        public string transactionID { get; set; }
        public string username { get; set; }
        public string ipAddress { get; set; }
        public string password { get; set; }
        public string msisdn { get; set; }
        public string shortcode { get; set; }
    }

    public class CheckPrivilegeResponse
    {
        public string transactionID { get; set; }
        public string httpStatus { get; set; }
        public string status { get; set; }
        public string description { get; set; }
        public string msg { get; set; }
    }

    public class CheckPrivilegePointResponse
    {
        public string transactionID { get; set; }
        public string httpStatus { get; set; }
        public string status { get; set; }
        public string description { get; set; }
        public decimal totalPointSet { get; set; }
        public PointArr[] pointArr { get; set; }
    }

    public class PrivilegeRedeemPointResponse
    {
        public string transactionID { get; set; }
        public string httpStatus { get; set; }
        public string status { get; set; }
        public string description { get; set; }
        public string msg { get; set; }
        public string msgBarcode { get; set; }
    }

    public class PointArr
    {
        public string msisdn { get; set; }
        public string detail { get; set; }
        public decimal points { get; set; }
        public decimal pointsBonus { get; set; }
        public string pointRegisterDate { get; set; }
        public string pointExpiredDate { get; set; }
        public string pointBonusExpiredDate { get; set; }
    }
}
