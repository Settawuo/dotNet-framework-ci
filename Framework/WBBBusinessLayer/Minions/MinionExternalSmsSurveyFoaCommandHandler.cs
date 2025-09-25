using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using WBBBusinessLayer.QueryHandlers;
using WBBContract;
using WBBContract.Minions;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Minions;
using WBBEntity.Models;

namespace WBBBusinessLayer.Minions
{
    public class MinionExternalSmsSurveyFoaCommandHandler : ICommandHandler<MinionExternalSmsSurveyFoaCommand>
    {
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<MinionFbbInterfaceLogPayg> _objInterfaceService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG_PAYG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public MinionExternalSmsSurveyFoaCommandHandler(IEntityRepository<string> objService,
            IEntityRepository<MinionFbbInterfaceLogPayg> objInterfaceService,
            IEntityRepository<FBB_INTERFACE_LOG_PAYG> intfLog,
            IWBBUnitOfWork uow)
        {
            _objService = objService;
            _objInterfaceService = objInterfaceService;
            _intfLog = intfLog;
            _uow = uow;
        }

        public void Handle(MinionExternalSmsSurveyFoaCommand command)
        {
            var results = new List<MinionExternalSmsSurveyFoaCommand.MinionExternalSmsSurveyFoaResult>();
            try
            {
                var externalSmsSurveyFoaList = command.ExternalSmsSurveyFoaList;
                foreach (var row in externalSmsSurveyFoaList)
                {
                    try
                    {
                        var stringQuery = string.Format(
                        "SELECT * FROM TABLE(WBB.PKG_FBB_FOA_SMSSURVEY.GetInterfaceLogPayG(p_interface_id => '{0}',p_ord_no => '{1}', p_non_mobile => '{2}'))",
                        row.InterfaceId.ToSafeString(),
                        row.OrderNo.ToSafeString(),
                        row.NonMobile.ToSafeString());
                        var executeResult = _objInterfaceService.SqlQuery(stringQuery).ToList();

                        if (executeResult.Any())
                        {
                            var rowData = executeResult.FirstOrDefault();

                            #region Call smsSurveyFoa

                            try
                            {
                                if (rowData == null) continue;

                                var decodexml = HttpUtility.HtmlDecode(rowData.IN_XML_PARAM.ToSafeString());
                                var foaRequert = MinionDeserialize<SmsSurveyFOAWebService.FOAParam>(decodexml);

                                var smsSurveyFoa = new SmsSurveyFOAWebService.smsSurveyFOA
                                {
                                    UseDefaultCredentials = true,
                                    Url = rowData.URL_LINE.ToSafeString()
                                };

                                var interfaceNode = String.Format("{0}|{1}", "FOA", smsSurveyFoa.Url.ToSafeString());
                                var foaLog = InterfaceLogPayGServiceHelper.StartInterfaceLogPayG(_uow, _intfLog,
                                    foaRequert,
                                    foaRequert.ordNo, "insertFOAInfo", "SmsSurveyFOA", foaRequert.nonMobile, interfaceNode,
                                    "MINION");

                                try
                                {
                                    var smsSurveyFoaResult = smsSurveyFoa.insertFOAInfo(foaRequert);

                                    InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog,
                                        smsSurveyFoaResult,
                                        foaLog,
                                        smsSurveyFoaResult.IndexOf("Success", StringComparison.Ordinal) > 0
                                            ? "Success"
                                            : "Failed",
                                        "", "MINION");

                                    results.Add(new MinionExternalSmsSurveyFoaCommand.MinionExternalSmsSurveyFoaResult
                                    {
                                        InterfaceId = row.InterfaceId.ToSafeString(),
                                        OrderNo = row.OrderNo.ToSafeString(),
                                        NonMobile = row.NonMobile.ToSafeString(),
                                        ReturnCode = "0",
                                        ReturnDesc = smsSurveyFoaResult
                                    });
                                }
                                catch (Exception ex)
                                {
                                    results.Add(new MinionExternalSmsSurveyFoaCommand.MinionExternalSmsSurveyFoaResult
                                    {
                                        InterfaceId = row.InterfaceId.ToSafeString(),
                                        OrderNo = row.OrderNo.ToSafeString(),
                                        NonMobile = row.NonMobile.ToSafeString(),
                                        ReturnCode = "-1",
                                        ReturnDesc = String.Format("{0}:{1}", ex.Source, ex.StackTrace)
                                    });

                                    InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog,
                                        ex.GetBaseException(), foaLog,
                                        "Error", ex.Message, "MINION");
                                }
                            }
                            catch (Exception ex)
                            {
                                results.Add(new MinionExternalSmsSurveyFoaCommand.MinionExternalSmsSurveyFoaResult
                                {
                                    InterfaceId = row.InterfaceId.ToSafeString(),
                                    OrderNo = row.OrderNo.ToSafeString(),
                                    NonMobile = row.NonMobile.ToSafeString(),
                                    ReturnCode = "-1",
                                    ReturnDesc = String.Format("{0}:{1}", ex.Source, ex.StackTrace)
                                });
                            }

                            #endregion Call smsSurveyFoa
                        }
                        else
                        {
                            results.Add(new MinionExternalSmsSurveyFoaCommand.MinionExternalSmsSurveyFoaResult
                            {
                                InterfaceId = row.InterfaceId.ToSafeString(),
                                OrderNo = row.OrderNo.ToSafeString(),
                                NonMobile = row.NonMobile.ToSafeString(),
                                ReturnCode = "-1",
                                ReturnDesc = "No Data Found."
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        results.Add(new MinionExternalSmsSurveyFoaCommand.MinionExternalSmsSurveyFoaResult
                        {
                            InterfaceId = row.InterfaceId.ToSafeString(),
                            OrderNo = row.OrderNo.ToSafeString(),
                            NonMobile = row.NonMobile.ToSafeString(),
                            ReturnCode = "-1",
                            ReturnDesc = String.Format("{0}:{1}", ex.Source, ex.StackTrace)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                results.Add(new MinionExternalSmsSurveyFoaCommand.MinionExternalSmsSurveyFoaResult
                {
                    ReturnCode = "-1",
                    ReturnDesc = "Error call MinionExternalSmsSurveyFoaCommand " + ex.GetErrorMessage()
                });
            }
            command.Results = results;
        }

        public T MinionDeserialize<T>(string input) where T : class
        {
            var serializer = new XmlSerializer(typeof(T));
            using (TextReader reader = new StringReader(input))
            {
                return (T)serializer.Deserialize(reader);
            }
        }
    }
}

