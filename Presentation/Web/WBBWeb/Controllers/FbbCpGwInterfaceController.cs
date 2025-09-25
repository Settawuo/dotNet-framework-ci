using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBContract.Queries.WebServices.FBSS;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.WebServiceModels;
using WBBWeb.Models;
using WBBWeb.Solid.CompositionRoot;

namespace WBBWeb.Controllers
{
    public class FbbCpGwInterfaceController : WBBController
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<InterfaceLog3BBCommand> _intfLog3bbCommand;

        public FbbCpGwInterfaceController(IQueryProcessor queryProcessor,
            ICommandHandler<InterfaceLog3BBCommand> intfLog3bbCommand)
        {
            _queryProcessor = queryProcessor;
            _intfLog3bbCommand = intfLog3bbCommand;
        }

        //R22.09 3BB : Add FbbCpGwInterface/Checkcoverage.
        [HttpPost]
        public JsonResult Checkcoverage(APICheckCoverageRequest request)
        {
            InterfaceLog3BBCommand log3bb = null;
            log3bb = StartInterfaceLog3BB(request, request.transactionId.ToSafeString(), "FbbCpGwInterface", "Checkcoverage", "", "3BB", "");
            APICheckCoverageResult result = new APICheckCoverageResult();

            try
            {
                string tmpUser = "3BB_testAPI";
                string tmpPass = "X@3BB@test_API_FIBRE";

                var lovConfig = base.LovData.Where(l => (l.Type == "FBB_CONFIG" && l.Name == "APICheckCoverageUser"));
                if (lovConfig != null)
                {
                    tmpUser = lovConfig.FirstOrDefault().LovValue1.ToSafeString();
                    tmpPass = lovConfig.FirstOrDefault().LovValue2.ToSafeString();
                }

                bool NotCheckPercentQueryPort = true;
                var lovConfigNotCheckPercentQueryPort = base.LovData.Where(l => (l.Type == "FBB_CONFIG" && l.Name == "NotCheckPercentQueryPort"));
                if (lovConfigNotCheckPercentQueryPort != null && lovConfigNotCheckPercentQueryPort.Count() > 0)
                    NotCheckPercentQueryPort = false;


                    var authHeader = Request.Headers["Authorization"].ToString();
                if (authHeader != null && authHeader.StartsWith("basic", StringComparison.OrdinalIgnoreCase))
                {
                    var token = authHeader.Substring("Basic ".Length).Trim();

                    var credentialstring = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                    var credentials = credentialstring.Split(':');
                    if (credentials[0] == tmpUser && credentials[1] == tmpPass)
                    {
                        if (request != null)
                        {
                            if (string.IsNullOrEmpty(request.buildingAddressID))
                            {
                                #region old flow
                                //Step 1: Call MapService
                                var queryCheckCoverageMap = new CheckCoverageMapServiceQuery
                                {
                                    latitude = string.IsNullOrEmpty(request.latitude) ? "" : request.latitude.ToString(),
                                    longitude = string.IsNullOrEmpty(request.longitude) ? "" : request.longitude.ToString(),
                                    transactionId = string.IsNullOrEmpty(request.transactionId) ? "" : request.transactionId.ToString(),
                                    FullUrl = "3BB"
                                };

                                CheckCoverageMapServiceDataModel dataCheckCoverageMap = _queryProcessor.Execute(queryCheckCoverageMap);

                                if (dataCheckCoverageMap != null && dataCheckCoverageMap.returnCode == "0")
                                {
                                    if (dataCheckCoverageMap.status == "ON_SERVICE" && dataCheckCoverageMap.subStatus == "AWN_EXCLUSIVE")
                                    {
                                        //AWN_EXCLUSIVE
                                        result.returnCode = "00000";
                                        result.returnMessage = "Success";
                                        result.inServiceDate = "";
                                        result.coverage = "OUT_OF_SERVICE";
                                        result.splitterList = new List<APICheckCoverageSplitterResult>();
                                    }
                                    else if (dataCheckCoverageMap.status == "ON_SERVICE" && dataCheckCoverageMap.splitterList.Count > 0)
                                    {
                                        List<SPLITTER_INFO> splitterCheckCoverageList = dataCheckCoverageMap.splitterList.ConvertAll(x => new SPLITTER_INFO
                                        {
                                            Splitter_Name = x.Name.ToSafeString(),
                                            Distance = 0,
                                            Distance_Type = string.Empty,
                                            Resource_Type = string.Empty
                                        });

                                        //Step 2: Call FBSS ResQuery
                                        var queryZTEResQuery = new ZTEResQueryQuery
                                        {
                                            PRODUCT = "FTTH",
                                            LISTOFSPLITTER = splitterCheckCoverageList.ToArray(),
                                            TRANSACTION_ID = request.transactionId.ToSafeString(),
                                            PHONE_FLAGE = string.Empty,
                                            LISTOFDSLAM = null,
                                            ADDRESS_ID = string.Empty,
                                            FullUrl = "3BB"
                                        };

                                        var dataZTEResQuery = _queryProcessor.Execute(queryZTEResQuery);

                                        if (dataZTEResQuery != null && dataZTEResQuery.RESULT_SPLITTERLIST.Length > 0)
                                        {
                                            List<ResultSplitList> splitterZTEList = dataZTEResQuery.RESULT_SPLITTERLIST
                                                .Select(item => new ResultSplitList()
                                                {
                                                    SPLITTER_NO = item.SPLITTER_NO,
                                                    RESULT_CODE = item.RESULT_CODE, //"1",
                                                    RESULT_DESCRIPTION = item.RESULT_DESCRIPTION
                                                }).ToList();

                                            //Step 3: filter SplitterZTEList RESULT_CODE 1
                                            List<APICheckCoverageSplitterResult> filterSplitterZTEList = new List<APICheckCoverageSplitterResult>();
                                            foreach (var i in splitterZTEList)
                                            {
                                                if (i.RESULT_CODE == "1" || i.RESULT_CODE == "4")
                                                {
                                                    var tmpSplitterlist = dataCheckCoverageMap.splitterList.Where(w => w.Name == i.SPLITTER_NO)
                                                        .Select(item => new APICheckCoverageSplitterResult()
                                                        {
                                                            distance = item.Distance.ToSafeString(),
                                                            splitterAlias = string.Empty,
                                                            splitterCode = item.Name.ToSafeString(),
                                                            splitterLatitude = item.Lat.ToSafeString(),
                                                            splitterLongitude = item.Lon.ToSafeString(),
                                                            splitterPort = string.Empty
                                                        }).FirstOrDefault();
                                                    filterSplitterZTEList.Add(tmpSplitterlist);
                                                }
                                            }

                                            result.splitterList = new List<APICheckCoverageSplitterResult>();

                                            //Step 4: Call FBSS QueryPort
                                            bool chkCallQueryPort = true;
                                            int maxSplitterCode = 2;
                                            bool reChkPort = true;
                                            foreach (var j in filterSplitterZTEList)
                                            {
                                                if (result.splitterList.Count < maxSplitterCode && reChkPort)
                                                {
                                                    var queryPort = new QueryPortQuery
                                                    {
                                                        RESOURCE_NO = j.splitterCode.ToSafeString(),
                                                        RESOURCE_TYPE = "SPLITTER",
                                                        SERVICE_STATE = "ALL",
                                                        TRANSACTION_ID = request.transactionId.ToSafeString(),
                                                        FullUrl = "3BB"
                                                    };
                                                    var dataqueryPort = _queryProcessor.Execute(queryPort);

                                                    if (dataqueryPort.return_code == "0" && dataqueryPort.Data != null)
                                                    {
                                                        if (dataqueryPort.Data.RESULT_CODE == "0" && dataqueryPort.Data.QueryPortNoList.Count > 0)
                                                        {
                                                            int inactivePercent = 0;
                                                            int portCount = dataqueryPort.Data.QueryPortNoList.Count();
                                                            int inactiveCount = dataqueryPort.Data.QueryPortNoList.Where(t => t.SERVICE_STATE == "Inactive").Count();
                                                            if (inactiveCount > 0 && portCount > 0)
                                                            {
                                                                if (NotCheckPercentQueryPort)
                                                                {
                                                                    inactivePercent = inactiveCount * 100 / portCount;
                                                                    if (inactivePercent >= 70)
                                                                        reChkPort = false;
                                                                }

                                                                var tmpSplitterlist2 = dataqueryPort.Data.QueryPortNoList.Where(t => t.SERVICE_STATE == "Inactive")
                                                                .Select(item => new APICheckCoverageSplitterResult()
                                                                {
                                                                    distance = j.distance.ToSafeString(),
                                                                    splitterAlias = dataqueryPort.Data.RESOURCE_ALIAS.ToSafeString(),
                                                                    splitterCode = j.splitterCode.ToSafeString(),
                                                                    splitterLatitude = j.splitterLatitude.ToSafeString(), //dataqueryPort.Data.RESOURCE_LATITUDE.ToSafeString(),
                                                                    splitterLongitude = j.splitterLongitude.ToSafeString(),//dataqueryPort.Data.RESOURCE_LONGITUDE.ToSafeString(),
                                                                    splitterPort = item.PORT_NO
                                                                }).FirstOrDefault();
                                                                result.splitterList.Add(tmpSplitterlist2);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        //case => dataqueryPort.return_code == "-1" ตก catch call QuertPort ไม่ได้
                                                        chkCallQueryPort = false;
                                                    }
                                                }
                                            }

                                            if (result.splitterList.Count > 0)
                                            {
                                                //ON_SERVICE=อยู่ในพื้นที่ให้บริการ
                                                result.returnCode = "00000";
                                                result.returnMessage = "Success";
                                                result.inServiceDate = "";
                                                result.coverage = dataCheckCoverageMap.status.ToSafeString();
                                            }
                                            else if (chkCallQueryPort == false)
                                            {
                                                //Can't connect database
                                                result.returnCode = "50301";
                                                result.returnMessage = "Can't connect database";
                                                result.inServiceDate = "";
                                                result.coverage = "";
                                                result.splitterList = new List<APICheckCoverageSplitterResult>();
                                            }
                                            else
                                            {
                                                //ไม่มี Splitter Port
                                                result.returnCode = "00000";
                                                result.returnMessage = "Success";
                                                result.inServiceDate = "";
                                                result.coverage = "OUT_OF_SERVICE";
                                                result.splitterList = new List<APICheckCoverageSplitterResult>();
                                            }
                                        }
                                        else
                                        {
                                            //Data not found
                                            result.returnCode = "50204";
                                            result.returnMessage = "Data not found";
                                            result.inServiceDate = "";
                                            result.coverage = "";
                                            result.splitterList = new List<APICheckCoverageSplitterResult>();
                                        }
                                    }
                                    else if (dataCheckCoverageMap.status == "PLAN")
                                    {
                                        //PLAN=มีแผนจะเปิดให้บริการ
                                        result.returnCode = "00000";
                                        result.returnMessage = "Success";
                                        result.inServiceDate = dataCheckCoverageMap.inserviceDate.ToSafeString();
                                        result.coverage = dataCheckCoverageMap.status.ToSafeString();
                                        result.splitterList = new List<APICheckCoverageSplitterResult>();
                                    }
                                    else if (dataCheckCoverageMap.status == "ON_SERVICE" && dataCheckCoverageMap.splitterList.Count == 0)
                                    {
                                        //OUT_OF_SERVICE
                                        result.returnCode = "00000";
                                        result.returnMessage = "Success";
                                        result.inServiceDate = "";
                                        result.coverage = "OUT_OF_SERVICE";
                                        result.splitterList = new List<APICheckCoverageSplitterResult>();
                                    }
                                    else if (string.IsNullOrEmpty(dataCheckCoverageMap.status))
                                    {
                                        //UNDEFINED = กรณี Error
                                        result.returnCode = "50403";
                                        result.returnMessage = "Response Error";
                                        result.inServiceDate = "";
                                        result.coverage = "";
                                        result.splitterList = new List<APICheckCoverageSplitterResult>();
                                    }
                                    else
                                    {
                                        result.returnCode = "00000";
                                        result.returnMessage = "Success";
                                        result.inServiceDate = "";
                                        result.coverage = dataCheckCoverageMap.status.ToSafeString();
                                        result.splitterList = new List<APICheckCoverageSplitterResult>();
                                    }
                                    //else if (dataCheckCoverageMap.status == "OUT_OF_SERVICE"
                                    //    || dataCheckCoverageMap.status == "ON_DEMAND"
                                    //    || dataCheckCoverageMap.status == "RESTRICTION_VILLAGE_PERMISSION"
                                    //    || dataCheckCoverageMap.status == "RESTRICTION_CABLE_UNDERGROUND"
                                    //    || dataCheckCoverageMap.status == "ON_SERVICE_SPECIAL"
                                    //    || dataCheckCoverageMap.status == "RESTRICTION VILLAGE PERMISSION"
                                    //    || dataCheckCoverageMap.status == "UNDERGROUND"
                                    //    || dataCheckCoverageMap.status == "OUT_OF_COVERAGE"
                                    //    )
                                    //{
                                    //    //OUT_OF_SERVICE=ไม่อยู่ในพื้นที่ให้บริการ
                                    //    //ON_DEMAND=พื้นที่เบาบาง แต่ให้บริการได้
                                    //    //RESTRICTION_VILLAGE_PERMISSION=ติด permission ของหมู่บ้าน
                                    //    //RESTRICTION_CABLE_UNDERGROUND=ติด Cable Underground ของหมู่บ้าน

                                    //    result.returnCode = "00000";
                                    //    result.returnMessage = "Success";
                                    //    result.inServiceDate = "";
                                    //    result.coverage = dataCheckCoverageMap.status.ToSafeString();
                                    //    result.splitterList = new List<APICheckCoverageSplitterResult>();
                                    //}
                                    //else
                                    //{
                                    //    //UNDEFINED=กรณี Error
                                    //    result.returnCode = "50403";
                                    //    result.returnMessage = "Response Error";
                                    //    result.inServiceDate = "";
                                    //    result.coverage = "";
                                    //    result.splitterList = new List<APICheckCoverageSplitterResult>();
                                    //}
                                }
                                else if (dataCheckCoverageMap != null && dataCheckCoverageMap.returnCode == "1")
                                {
                                    //1 = lat หรือ long เป็นค่าว่าง
                                    string strRequireField = "";
                                    if (string.IsNullOrEmpty(request.latitude)) { strRequireField += "Require field - ( latitude )"; }
                                    else if (string.IsNullOrEmpty(request.longitude)) { strRequireField += "Require field - ( longitude )"; }

                                    result.returnCode = "50101";
                                    result.returnMessage = strRequireField;
                                    result.inServiceDate = "";
                                    result.coverage = "";
                                    result.splitterList = new List<APICheckCoverageSplitterResult>();
                                }
                                else if (dataCheckCoverageMap != null && dataCheckCoverageMap.returnCode == "-1")
                                {
                                    //-1 = Query Error
                                    result.returnCode = "50301";
                                    result.returnMessage = "Can't connect database";
                                    result.inServiceDate = "";
                                    result.coverage = "";
                                    result.splitterList = new List<APICheckCoverageSplitterResult>();
                                }
                                else if (dataCheckCoverageMap.returnCode == "-1")
                                {
                                    //Data not found
                                    result.returnCode = "50204";
                                    result.returnMessage = "Data not found";
                                    result.inServiceDate = "";
                                    result.coverage = "";
                                    result.splitterList = new List<APICheckCoverageSplitterResult>();
                                }
                                else
                                {
                                    //-2 = System Error
                                    result.returnCode = "59999";
                                    result.returnMessage = "System error";
                                    result.inServiceDate = "";
                                    result.coverage = dataCheckCoverageMap.status.ToSafeString();
                                    result.splitterList = new List<APICheckCoverageSplitterResult>();
                                }

                                EndInterface3BB(result, log3bb, request.transactionId.ToSafeString(), "Success", "", "");
                                return Json(result, JsonRequestBehavior.AllowGet);
                            }
                            #endregion
                            else
                            {
                                #region FTTR flow request.buildingAddressID
                                var queryCheckCoverageMap = new CheckCoverageListbvByAddressIDQuery
                                {
                                    latitude = string.IsNullOrEmpty(request.latitude) ? null : request.latitude.ToString(),
                                    longitude = string.IsNullOrEmpty(request.longitude) ? null : request.longitude.ToString(),
                                    buildingAddressID = string.IsNullOrEmpty(request.buildingAddressID) ? null : request.buildingAddressID.ToString(),
                                    transactionId = string.IsNullOrEmpty(request.transactionId) ? null : request.transactionId.ToString()

                                };
                                List<CheckCoverageListbvByAddressIDDataModel> dataCheckCoverageMap = _queryProcessor.Execute(queryCheckCoverageMap);

                                // Get IP Address
                                string aisAirNumber = request.transactionId.ToString();

                                string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                                if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                                string transactionId = aisAirNumber + ipAddress;

                                if (dataCheckCoverageMap.Count() > 0)
                                {
                                    // Check FttrFlag           
                                    if (CheckAisExclusive(dataCheckCoverageMap[0].FttrFlag))
                                    {
                                        // เข้า AIS Exclusive
                                        result.returnCode = "00000";
                                        result.returnMessage = "Success";
                                        result.inServiceDate = "";
                                        result.coverage = "OUT_OF_SERVICE";
                                        result.splitterList = new List<APICheckCoverageSplitterResult>();
                                    }
                                    else
                                    {

                                        //CallCheckCoverage
                                        var query = new GetFBSSFeasibilityCheck
                                        {
                                            AddressType = dataCheckCoverageMap[0].AddressType,
                                            PostalCode = dataCheckCoverageMap[0].PostalCode,
                                            Language = dataCheckCoverageMap[0].Language,
                                            SubDistricName = dataCheckCoverageMap[0].SubDistricName,
                                            BuildingName = dataCheckCoverageMap[0].BuildingName,
                                            BuildingNo = dataCheckCoverageMap[0].BuildingNo,
                                            PhoneFlag = dataCheckCoverageMap[0].PhoneFlag,
                                            Latitude = dataCheckCoverageMap[0].Latitude,
                                            Longitude = dataCheckCoverageMap[0].Longitude,
                                            UnitNo = dataCheckCoverageMap[0].UnitNo,
                                            FloorNo = dataCheckCoverageMap[0].FloorNo,
                                            TransactionId = dataCheckCoverageMap[0].TransactionId,
                                            FullUrl = dataCheckCoverageMap[0].FullUrl
                                        };

                                        FBSSCoverageResult coverageResult = _queryProcessor.Execute(query);

                                        List<SPLITTER_INFO> splitterCheckCoverageList = coverageResult.AccessModeList[0].ResourceList.ConvertAll(x => new SPLITTER_INFO
                                        {
                                            Splitter_Name = x.ResourceName.ToSafeString(),
                                            Distance = 0,
                                            Distance_Type = string.Empty,
                                            Resource_Type = string.Empty
                                        });

                                        //Step 2: Call FBSS ResQuery
                                        var queryZTEResQuery = new ZTEResQueryQuery
                                        {
                                            PRODUCT = "FTTH",
                                            LISTOFSPLITTER = splitterCheckCoverageList.ToArray(),
                                            TRANSACTION_ID = request.transactionId.ToSafeString(),
                                            PHONE_FLAGE = string.Empty,
                                            LISTOFDSLAM = null,
                                            ADDRESS_ID = string.Empty,
                                            FullUrl = "3BB"
                                        };

                                        var dataZTEResQuery = _queryProcessor.Execute(queryZTEResQuery);

                                        if (dataZTEResQuery != null && dataZTEResQuery.RESULT_SPLITTERLIST.Length > 0 && coverageResult.Coverage == "YES")
                                        {
                                            List<ResultSplitList> splitterZTEList = dataZTEResQuery.RESULT_SPLITTERLIST
                                                .Select(item => new ResultSplitList()
                                                {
                                                    SPLITTER_NO = item.SPLITTER_NO,
                                                    RESULT_CODE = item.RESULT_CODE, //"1",
                                                    RESULT_DESCRIPTION = item.RESULT_DESCRIPTION
                                                }).ToList();

                                            //Step 3: filter SplitterZTEList RESULT_CODE 1
                                            List<APICheckCoverageSplitterResult> filterSplitterZTEList = new List<APICheckCoverageSplitterResult>();
                                            foreach (var i in splitterZTEList)
                                            {
                                                if (i.RESULT_CODE == "1")
                                                {
                                                    var tmpSplitterlist = splitterCheckCoverageList.Where(w => w.Splitter_Name == i.SPLITTER_NO)
                                                        .Select(item => new APICheckCoverageSplitterResult()
                                                        {
                                                            distance = item.Distance.ToSafeString(),
                                                            splitterAlias = string.Empty,
                                                            splitterCode = item.Splitter_Name.ToSafeString(),
                                                            splitterLatitude = dataCheckCoverageMap[0].Latitude,
                                                            splitterLongitude = dataCheckCoverageMap[0].Longitude,
                                                            splitterPort = string.Empty
                                                        }).FirstOrDefault();
                                                    filterSplitterZTEList.Add(tmpSplitterlist);
                                                }
                                            }

                                            result.splitterList = new List<APICheckCoverageSplitterResult>();

                                            //Step 4: Call FBSS QueryPort
                                            bool chkCallQueryPort = true;
                                            if (filterSplitterZTEList != null)
                                            {
                                                int maxSplitterCode = 2;
                                                bool reChkPort = true;
                                                foreach (var j in filterSplitterZTEList)
                                                {
                                                    if (result.splitterList.Count < maxSplitterCode && reChkPort)
                                                    {
                                                        var queryPort = new QueryPortQuery
                                                        {
                                                            RESOURCE_NO = j.splitterCode.ToSafeString(),
                                                            RESOURCE_TYPE = "SPLITTER",
                                                            SERVICE_STATE = "ALL",
                                                            TRANSACTION_ID = request.transactionId.ToSafeString(),
                                                            FullUrl = "3BB"
                                                        };
                                                        var dataqueryPort = _queryProcessor.Execute(queryPort);

                                                        if (dataqueryPort.return_code == "0" && dataqueryPort.Data != null)
                                                        {
                                                            if (dataqueryPort.Data.RESULT_CODE == "0" && dataqueryPort.Data.QueryPortNoList.Count > 0)
                                                            {
                                                                int inactivePercent = 0;
                                                                int portCount = dataqueryPort.Data.QueryPortNoList.Count();
                                                                int inactiveCount = dataqueryPort.Data.QueryPortNoList.Where(t => t.SERVICE_STATE == "Inactive").Count();
                                                                if (inactiveCount > 0 && portCount > 0)
                                                                {
                                                                    if (NotCheckPercentQueryPort)
                                                                    {
                                                                        inactivePercent = inactiveCount * 100 / portCount;
                                                                        if (inactivePercent >= 70)
                                                                            reChkPort = false;
                                                                    }

                                                                    var tmpSplitterlist2 = dataqueryPort.Data.QueryPortNoList
                                                                    .Where(t => t.SERVICE_STATE == "Inactive")
                                                                    .Select(item => new APICheckCoverageSplitterResult()
                                                                    {
                                                                        distance = j.distance.ToSafeString(),
                                                                        splitterAlias = dataqueryPort.Data.RESOURCE_ALIAS.ToSafeString(),
                                                                        splitterCode = j.splitterCode.ToSafeString(),
                                                                        splitterLatitude = j.splitterLatitude.ToSafeString(), //dataqueryPort.Data.RESOURCE_LATITUDE.ToSafeString(),
                                                                        splitterLongitude = j.splitterLongitude.ToSafeString(),//dataqueryPort.Data.RESOURCE_LONGITUDE.ToSafeString(),
                                                                        splitterPort = item.PORT_NO
                                                                    }).FirstOrDefault();
                                                                    result.splitterList.Add(tmpSplitterlist2);
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            //case => dataqueryPort.return_code == "-1" ตก catch call QuertPort ไม่ได้
                                                            chkCallQueryPort = false;
                                                        }
                                                    }
                                                }
                                            }

                                            if (result.splitterList.Count > 0)
                                            {
                                                //ON_SERVICE=อยู่ในพื้นที่ให้บริการ
                                                result.returnCode = "00000";
                                                result.returnMessage = "Success";
                                                result.inServiceDate = "";
                                                result.coverage = "ON_SERVICE";
                                            }
                                            else if (chkCallQueryPort == false)
                                            {
                                                //Can't connect database
                                                result.returnCode = "50301";
                                                result.returnMessage = "Can't connect database";
                                                result.inServiceDate = "";
                                                result.coverage = "";
                                                result.splitterList = new List<APICheckCoverageSplitterResult>();
                                            }
                                            else
                                            {
                                                //ไม่มี Splitter Port
                                                result.returnCode = "00000";
                                                result.returnMessage = "Success";
                                                result.inServiceDate = "";
                                                result.coverage = "OUT_OF_SERVICE";
                                                result.splitterList = new List<APICheckCoverageSplitterResult>();
                                            }
                                        }
                                        else
                                        {
                                            //Data not found
                                            result.returnCode = "50204";
                                            result.returnMessage = "Data not found";
                                            result.inServiceDate = "";
                                            result.coverage = "";
                                            result.splitterList = new List<APICheckCoverageSplitterResult>();
                                        }
                                    }
                                }
                                else
                                {
                                    //Data not found
                                    result.returnCode = "50204";
                                    result.returnMessage = "Data not found";
                                    result.inServiceDate = "";
                                    result.coverage = "";
                                    result.splitterList = new List<APICheckCoverageSplitterResult>();
                                }

                                return Json(result, JsonRequestBehavior.AllowGet);
                            }
                            #endregion
                        }
                        else
                        {
                            //Format incorreact
                            result.returnCode = "50102";
                            result.returnMessage = "Format incorreact";
                            result.inServiceDate = "";
                            result.coverage = "";
                            result.splitterList = new List<APICheckCoverageSplitterResult>();

                            EndInterface3BB(result, log3bb, request.transactionId.ToSafeString(), "Success", "", "");
                            return Json(result, JsonRequestBehavior.AllowGet);
                        }

                    }
                    else
                    {
                        //Username Password failed
                        HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        HttpContext.Response.End();

                        EndInterface3BB(result, log3bb, request.transactionId.ToSafeString(), "Failed", "Username Password failed", "");
                        return Json("Unauthorized", JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    //Authorization failed
                    HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    HttpContext.Response.End();

                    EndInterface3BB(result, log3bb, request.transactionId.ToSafeString(), "Failed", "Authorization failed", "");
                    return Json("Unauthorized", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                //Authorization failed
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                HttpContext.Response.End();

                EndInterface3BB(result, log3bb, request.transactionId.ToSafeString(), "Failed", ex.Message, "");
                return Json("Unauthorized", JsonRequestBehavior.AllowGet);
            }

        }

        private InterfaceLog3BBCommand StartInterfaceLog3BB<T>(T query, string transactionId, string methodName, string serviceName, string idCardNo, string interfaceNode, string createdBy)
        {
            var dbIntfCmd = new InterfaceLog3BBCommand
            {
                ActionType = ActionType.Insert,
                IN_TRANSACTION_ID = transactionId,
                METHOD_NAME = methodName,
                SERVICE_NAME = serviceName,
                IN_ID_CARD_NO = idCardNo,
                IN_XML_PARAM = query.DumpToXml(),
                INTERFACE_NODE = interfaceNode,
                CREATED_BY = createdBy,
            };

            _intfLog3bbCommand.Handle(dbIntfCmd);

            return dbIntfCmd;
        }

        private void EndInterface3BB<T>(T output, InterfaceLog3BBCommand dbIntfCmd, string transactionId, string result, string reason, string updateBy)
        {
            dbIntfCmd.ActionType = ActionType.Update;
            dbIntfCmd.REQUEST_STATUS = (result == "Success") ? "Success" : "Error";
            dbIntfCmd.OUT_RESULT = result;
            dbIntfCmd.OUT_ERROR_RESULT = reason.Length > 100 ? reason.Substring(0, 100) : reason;
            dbIntfCmd.OUT_XML_PARAM = output.DumpToXml();
            dbIntfCmd.UPDATED_BY = updateBy;

            _intfLog3bbCommand.Handle(dbIntfCmd);
        }

        private bool CheckAisExclusive(string fttrFlag)
        {

            bool result = false;
            List<LovValueModel> msgLov = new List<LovValueModel>();
            var masterController = Bootstrapper.GetInstance<MasterDataController>();
            msgLov = masterController.GetLovList("EXCUSIVE_CONFIG", "").ToList();
           
            if(msgLov != null && msgLov.Count() > 0)
            {
         
                if (!String.IsNullOrEmpty(fttrFlag) && fttrFlag.Length > 4)
                {
                    var subStringLast = msgLov.FirstOrDefault(t => t.Name == "EXCUSIVE_SUBSTRING_AIS").LovValue2;
                    int subStringLastInt = 0;
                    Int32.TryParse(subStringLast, out subStringLastInt);
                    var checkExclusive = msgLov.Where(t => t.Name == "EXCUSIVE_SUBSTRING_LAST_AIS").Select(t => t.LovValue1).ToList();
                    int lastIndex = fttrFlag.Length - subStringLastInt;
                    var fttrFlagLast = fttrFlag.Substring(lastIndex, subStringLastInt); // 1 ตัวหลัง
                    if (checkExclusive.IndexOf(fttrFlagLast) >= 0)
                    {
                        var subStringFirst = msgLov.FirstOrDefault(t => t.Name == "EXCUSIVE_SUBSTRING_AIS").LovValue1;
                        int subStringFirstInt = 0;
                        Int32.TryParse(subStringFirst, out subStringFirstInt);
                        var checkPartner = msgLov.Where(t => t.Name == "EXCUSIVE_SUBSTRING_FRIST_AIS").Select(t => t.LovValue1).ToList();
                        var fttrFlagFirst = fttrFlag.Substring(0, subStringFirstInt); // 3 ตัวแรก

                        if (checkPartner.IndexOf(fttrFlagFirst) >= 0)
                            result = true;
                    }
                }
            }         
            return result;
        }

    }
}