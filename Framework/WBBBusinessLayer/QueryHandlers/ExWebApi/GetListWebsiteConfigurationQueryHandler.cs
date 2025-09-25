using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.ExWebApi;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.ExWebApiModel;

namespace WBBBusinessLayer.QueryHandlers.ExWebApi
{
    public class GetListWebsiteConfigurationQueryHandler : IQueryHandler<GetListWebsiteConfigurationQuery, GetListWebsiteConfigurationQueryModel>
    {
        private readonly IWBBUnitOfWork _unitOfWork;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _repositoryLog;
        private readonly IEntityRepository<object> _storeRepository;

        public GetListWebsiteConfigurationQueryHandler(IWBBUnitOfWork unitOfWork, IEntityRepository<FBB_INTERFACE_LOG> repositoryLog, IEntityRepository<object> storeRepository)
        {
            _unitOfWork = unitOfWork;
            _repositoryLog = repositoryLog;
            _storeRepository = storeRepository;
        }

        public GetListWebsiteConfigurationQueryModel Handle(GetListWebsiteConfigurationQuery query)
        {
            if (string.IsNullOrEmpty(query.TransactionId))
            {
                return new GetListWebsiteConfigurationQueryModel()
                {
                    RESULT_CODE = ((int)ResultMessageEnum.SystemNotExit).ToString(),
                    RESULT_DESC = ResultMessageEnum.SystemNotExit.ToString()
                };
            }

            var result = new GetListWebsiteConfigurationQueryModel();
            result.TRANSACTION_ID = query.TransactionId;

            string jsonQuery = JsonConvert.SerializeObject(query);

            InterfaceLogCommand interfaceLog = null;

            try
            {
                interfaceLog = InterfaceLogServiceHelper.StartInterfaceLog(
                    _unitOfWork,
                    _repositoryLog,
                    jsonQuery,
                    query.TransactionId,
                    "GetListWebsiteConfigurationQueryHandler",
                    "GetListWebsiteConfigurationQueryHandler",
                    query.TransactionId,
                    "FBB",
                    "ExternalApi"
                    );

                var pColumnName = new OracleParameter()
                {
                    ParameterName = "P_COLUMN_NAME",
                    Value = query.ColumnName,
                    Direction = ParameterDirection.Input
                };

                var pPackageCode = new OracleParameter()
                {
                    ParameterName = "P_PACKAGE_CODE",
                    Value = query.LovName,
                    Direction = ParameterDirection.Input
                };

                var pLovType = new OracleParameter()
                {
                    ParameterName = "P_LOV_TYPE",
                    Value = query.LovType,
                    Direction = ParameterDirection.Input
                };

                var outCurRef = new OracleParameter()
                {
                    ParameterName = "OUT_CUR_REF",
                    Direction = ParameterDirection.Output,
                    OracleDbType = OracleDbType.RefCursor
                };

                var storeResult = _storeRepository.ExecuteStoredProcMultipleCursor("wbb.sp_get_lov_termsconditions", new object[]
                {
                    pColumnName,
                    pPackageCode,
                    pLovType,

                    outCurRef
                });

                List<LovExternalWebApiList> lovExWebApiList = ConvertToList(storeResult[0]);
                List<LovList> lovList = new List<LovList>();
                if (lovExWebApiList.Count > 0)
                {
                    lovExWebApiList.ForEach(e =>
                    {
                        LovList lovObj = LovList.CreateLovList(e);
                        lovList.Add(lovObj);
                    });
                }
                if (lovList.Count > 0)
                {
                    string trFirst_th = "";
                    string trFirst_en = "";

                    string trLast_th = "";
                    string trLast_en = "";

                    int switchCheckCase = (int)(lovExWebApiList.Select(e => e.FORMAT_TYPE).First());
                    switch (switchCheckCase)
                    {
                        case 1:
                            {
                                lovList.ForEach(d =>
                                {
                                    if (d.ORDER_BY != 0)
                                    {
                                        d.LOV_VAL1 = $"<tr>{d.LOV_VAL1}</tr>";
                                        d.LOV_VAL2 = $"<tr>{d.LOV_VAL2}</tr>";
                                    }
                                });

                                trFirst_th = $"<table>{lovList[1].LOV_VAL1}";
                                trFirst_en = $"<table>{lovList[1].LOV_VAL2}";

                                trLast_th = $"{lovList.Last().LOV_VAL1}</table>";
                                trLast_en = $"{lovList.Last().LOV_VAL2}</table>";

                                lovList[1].LOV_VAL1 = trFirst_th;
                                lovList[1].LOV_VAL2 = trFirst_en;

                                lovList.Last().LOV_VAL1 = trLast_th;
                                lovList.Last().LOV_VAL2 = trLast_en;
                            }
                            break;
                        case 2:
                            {
                                var iconPath = lovList.Where(d => d.DISPLAY_VAL == "ICON_PATH").Select(d => d.IMAGE_BLOB).FirstOrDefault();
                                var iconDesTh = lovList.Where(d => d.DISPLAY_VAL == "ICON_DES").Select(d => d.LOV_VAL1).FirstOrDefault();
                                var iconDesEn = lovList.Where(d => d.DISPLAY_VAL == "ICON_DES").Select(d => d.LOV_VAL2).FirstOrDefault();
                                var detailPlaboxPath = lovList.Where(d => d.DISPLAY_VAL == "DETAIL_PLABOX_PATH").Select(d => d.IMAGE_BLOB).FirstOrDefault();
                                var detailHeaderTh = lovList.Where(d => d.DISPLAY_VAL == "DETAIL_HEADER").Select(d => d.LOV_VAL1).FirstOrDefault();
                                var detailHeaderEn = lovList.Where(d => d.DISPLAY_VAL == "DETAIL_HEADER").Select(d => d.LOV_VAL2).FirstOrDefault();
                                var detailContentPath = lovList.Where(d => d.DISPLAY_VAL == "DETAIL_CONTENT_PATH").Select(d => d.IMAGE_BLOB).FirstOrDefault();
                                var detailContentDescriptionTh = lovList.Where(d => d.DISPLAY_VAL == "DETAIL_CONTENT_DESCRIPTION").Select(d => d.LOV_VAL1).FirstOrDefault();
                                var detailContentDescriptionEn = lovList.Where(d => d.DISPLAY_VAL == "DETAIL_CONTENT_DESCRIPTION").Select(d => d.LOV_VAL2).FirstOrDefault();

                                string imageHead = detailPlaboxPath == null ? string.Empty : Convert.ToBase64String(detailPlaboxPath);
                                string imageDetail = detailContentPath == null ? string.Empty : Convert.ToBase64String(detailContentPath);
                                detailContentDescriptionTh = detailContentDescriptionTh.Replace("margin-left: 40px;", "");
                                detailContentDescriptionEn = detailContentDescriptionEn.Replace("margin-left: 40px;", "");

                                var lovs = new List<LovList>();
                                lovs.Add(new LovList()
                                {
                                    LOV_VAL1 = !string.IsNullOrEmpty(imageHead) ? $"<img style='width: auto; max-width: 100%; height: auto;' src='data:image/png;base64,{imageHead}' >" : "",
                                    LOV_VAL2 = !string.IsNullOrEmpty(imageHead) ? $"<img style='width: auto; max-width: 100%; height: auto;' src='data:image/png;base64,{imageHead}' >" : "",
                                    DISPLAY_VAL = "DETAIL_PLABOX_PATH",
                                    IMAGE_BLOB = null,
                                    ORDER_BY = 0
                                });
                                lovs.Add(new LovList()
                                {
                                    LOV_VAL1 = $"<h3 style='font-weight:bold'>{detailHeaderTh}</h3>",
                                    LOV_VAL2 = $"<h3 style='font-weight:bold'>{detailHeaderEn}</h3>",
                                    DISPLAY_VAL = "DETAIL_HEADER",
                                    IMAGE_BLOB = null,
                                    ORDER_BY = 0
                                });
                                lovs.Add(new LovList()
                                {
                                    LOV_VAL1 = !string.IsNullOrEmpty(imageDetail) ? $"<img style='width: auto; max-width: 100%; height: auto;' src='data:image/png;base64,{imageDetail}' >" : "",
                                    LOV_VAL2 = !string.IsNullOrEmpty(imageDetail) ? $"<img style='width: auto; max-width: 100%; height: auto;' src='data:image/png;base64,{imageDetail}' >" : "",
                                    DISPLAY_VAL = "DETAIL_CONTENT_PATH",
                                    IMAGE_BLOB = null,
                                    ORDER_BY = 0
                                });
                                lovs.Add(new LovList()
                                {
                                    LOV_VAL1 = $"<span>{detailContentDescriptionTh}</span>",
                                    LOV_VAL2 = $"<span>{detailContentDescriptionEn}</span>",
                                    DISPLAY_VAL = "DETAIL_CONTENT_DESCRIPTION",
                                    IMAGE_BLOB = null,
                                    ORDER_BY = 0
                                });

                                lovList = lovs;
                            }
                            break;
                        default:
                            lovList = new List<LovList>();
                            break;
                    }

                    InterfaceLogServiceHelper.EndInterfaceLog(
                        _unitOfWork,
                        _repositoryLog,
                        ResultMessageEnum.SUCCESS.ToString(),
                        interfaceLog,
                        "Success",
                        "",
                        "ExternalApi");

                    if (lovList.Count > 0)
                    {
                        result.RESULT_CODE = Convert.ToString((int)ResultMessageEnum.SUCCESS);
                        result.RESULT_DESC = ResultMessageEnum.SUCCESS.ToString();
                        result.LOV_LIST = lovList;
                    }
                    else
                    {
                        result.RESULT_CODE = Convert.ToString((int)ResultMessageEnum.DataNotFound);
                        result.RESULT_DESC = ResultMessageEnum.DataNotFound.ToString();
                        result.LOV_LIST = lovList;
                    }
                }
                else
                {
                    result.RESULT_CODE = Convert.ToString((int)ResultMessageEnum.DataNotFound);
                    result.RESULT_DESC = ResultMessageEnum.DataNotFound.ToString();

                    InterfaceLogServiceHelper.EndInterfaceLog(
                     _unitOfWork,
                     _repositoryLog,
                     ResultMessageEnum.DataNotFound.ToString(),
                     interfaceLog,
                     "Success",
                     "",
                     "ExternalApi");
                }

                return result;
            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(
                    _unitOfWork,
                    _repositoryLog,
                    ex,
                    interfaceLog,
                    "Failed",
                    ex.GetErrorMessage(),
                    "ExternalApi"
                    );

                result.RESULT_CODE = Convert.ToString((int)ResultMessageEnum.SystemNotExit);
                result.RESULT_DESC = ResultMessageEnum.SystemNotExit.ToString();

                return result;
            }
        }

        private List<LovExternalWebApiList> ConvertToList(object obj)
        {
            string json = JsonConvert.SerializeObject(obj);
            return JsonConvert.DeserializeObject<List<LovExternalWebApiList>>(json);
        }
    }
}
