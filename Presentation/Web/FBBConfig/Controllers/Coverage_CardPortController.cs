using FBBConfig.Extensions;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.FBBWebConfigQueries.CoverageManagePage;

namespace FBBConfig.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
    public class Coverage_CardPortController : FBBConfigController
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<SaveCradPortDataCommand> _COMMAND_SaveCradPortDataCommand;
        private readonly ICommandHandler<CreateCardPortDataCommand> _COMMAND_CreateCardPortDataCommand;



        public Coverage_CardPortController(ILogger logger,
             IQueryProcessor queryProcessor,
            ICommandHandler<SaveCradPortDataCommand> COMMAND_SaveCradPortDataCommand,
            ICommandHandler<CreateCardPortDataCommand> COMMAND_CreateCardPortDataCommand
          )
        {
            base._Logger = logger;
            _queryProcessor = queryProcessor;
            _COMMAND_SaveCradPortDataCommand = COMMAND_SaveCradPortDataCommand;
            _COMMAND_CreateCardPortDataCommand = COMMAND_CreateCardPortDataCommand;
        }
        [AuthorizeUserAttribute]
        public ActionResult index(decimal cvrId, decimal dslamId, decimal ContactID)
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;


            ViewBag.DSLAMID = dslamId;
            ViewBag.cvrId = cvrId;
            ViewBag.ContactID = ContactID;

            var queryLotStartIndex = new GetLotStratIndex
            {
                CVRID = cvrId
            };


            var resul22t = _queryProcessor.Execute(queryLotStartIndex);

            if (Session["LotStartIndex"] != null)
            {
                Session.Remove("LotStartIndex");
                Session["LotStartIndex"] = resul22t;
                var sttt = Session["LotStartIndex"];
            }
            else
            {
                Session["LotStartIndex"] = resul22t;
                var sttt = Session["LotStartIndex"];
                ////return View();

            }


            var etCheckCorverageTieCardPortQuer = new GetCheckCorverageTieCardPortQuery
            {
                CVRID = cvrId

            };


            var TieCardPortQuer = _queryProcessor.Execute(etCheckCorverageTieCardPortQuer);


            if (TieCardPortQuer)
            {
                ViewBag.FalgTieCardportQuery = "1";
            }
            else
            {
                ViewBag.FalgTieCardportQuery = "2";
            }







            ///ViewBag.StartIndex = _queryProcessor.Execute();
            return View();





        }




        public JsonResult ReadCradPortData([DataSourceRequest] DataSourceRequest request, decimal DSLAMID)
        {
            Session.Remove("Number_MaxCard_In_Fo");
            ////Session.Remove("NodeID");
            var query = new GetCardInfoPortPanelDataQuery
            {
                DSLAMID = DSLAMID,
                ResultReaderdataPor = "ReadCradPortData"

            };
            var result = _queryProcessor.Execute(query);
            var result22 = result.OrderByDescending(s => s.Number);


            if (result22.Any())
            {

                if (result22 != null)
                {
                    var Number2222 = result22.FirstOrDefault().Number + 1;
                    Session["Number_MaxCard_In_Fo"] = Number2222;
                }

            }
            /// ViewBag.NODEID = result.Count() > 0 ? result[0].Nodeid : "";

            if (result22.Count() == 0)
            {
                if (Session["LotStartIndex"] != null)
                {
                    Session["Number_MaxCard_In_Fo"] = Session["LotStartIndex"];

                }
            }

            return Json(result.ToDataSourceResult(request));
        }



        public JsonResult Max_number()
        {
            decimal Number_MaxCard_In_Fo = 0;
            Number_MaxCard_In_Fo = Session["Number_MaxCard_In_Fo"] != null ? (decimal)Session["Number_MaxCard_In_Fo"] : 0;

            return Json(Number_MaxCard_In_Fo, JsonRequestBehavior.AllowGet);
        }
        public JsonResult setdataTransition()
        {
            string resultDesc = "";
            string returnCode = "";
            string CARDID = "";
            string ResultCommand = "";
            var listStrt = new List<string>();

            if (Session["returnDesc"] != null && Session["Return_Code"] != null)
            {
                resultDesc = Session["returnDesc"].ToString();
                returnCode = Session["Return_Code"].ToString();

                listStrt.Add(resultDesc);
                listStrt.Add(returnCode);
            }

            if (Session["ResultCommand"] != null && Session["ResultCommand"].ToString() == "CARIDGRID" && Session["CARDID"] != null)
            {

                CARDID = Session["CARDID"].ToString();
                ResultCommand = Session["ResultCommand"].ToString();

                listStrt.Add(CARDID);
                listStrt.Add(ResultCommand);

            }
            return Json(listStrt, JsonRequestBehavior.AllowGet);
        }


        public JsonResult ReadCradPort([DataSourceRequest] DataSourceRequest request, decimal CARDID)
        {

            var query = new GetCardInfoPortDataPanelBQuery
            {
                CardID = CARDID

            }; var result = _queryProcessor.Execute(query);

            return Json(result != null ? result.ToDataSourceResult(request) : "".ToDataSourceResult(request));
        }

        public JsonResult ReadCradPortHistory([DataSourceRequest] DataSourceRequest request, decimal Portid)
        {
            var query = new GetCardInfoPortDataPaneHistorylBQuery
            {
                PorID = Portid


            };

            var result = _queryProcessor.Execute(query);

            return Json(result.ToDataSourceResult(request));
        }

        public JsonResult CreatePortDataGrid(decimal DSLAMID, string ComandBox, decimal CARDID, decimal CardModelID)
        {
            Session.Remove("returnDesc");
            Session.Remove("Return_Code");
            Session.Remove("CARDID");
            Session.Remove("ResultCommand");
            var commad = new CreateCardPortDataCommand()
            {

                CradNumber = 0,
                CardModel = "",
                CardType = "",
                RESERVE_TECHNOLOGY = "",
                CardModelID = CardModelID,
                PORTNUMBER = 0,
                CARDID = CARDID,
                DSalamModelID = DSLAMID,
                ResultCommand = "",
                Return_Desc = "",
                Create_BY = base.CurrentUser.UserName.ToString(),
                Update_BY = base.CurrentUser.UserName.ToString(),
                ComandBoxdata = ComandBox

            };
            _COMMAND_CreateCardPortDataCommand.Handle(commad);

            Session["returnDesc"] = commad.Return_Desc;
            Session["Return_Code"] = commad.Return_Code;
            Session["CARDID"] = commad.CARDID;
            Session["ResultCommand"] = commad.ResultCommand;
            return Json(commad, JsonRequestBehavior.AllowGet);
        }





        public JsonResult Add_CardCradPort(string ResultCommand, string RESERVE_TECHNOLOGY, decimal CardModelID,
        decimal DSalamModelID, decimal CradNumber, decimal CARDID, string NodeID, string CardType, string Building)
        {
            Session.Remove("returnDesc");
            Session.Remove("Return_Code");

            var CREATED_DATE = DateTime.Now.Date;

            var SaveCardPort = new SaveCradPortDataCommand
            {
                CARDID = CARDID,
                CardType = CardType,
                CardModelID = CardModelID,
                CradNumber = CradNumber,
                CRATE_DATE = CREATED_DATE,
                DSalamModelID = DSalamModelID,
                RESERVE_TECHNOLOGY = RESERVE_TECHNOLOGY,
                ResultCommand = ResultCommand,
                UPDATE_DATE = CREATED_DATE,
                Node_ID = NodeID,
                UPDATEBY = base.CurrentUser.UserName.ToString(),
                Building = Building



            };



            try
            {

                if (SaveCardPort.ResultCommand == "ADD")
                {
                    _COMMAND_SaveCradPortDataCommand.Handle(SaveCardPort);


                }
                else if (SaveCardPort.ResultCommand == "UPDATE")
                {

                    _COMMAND_SaveCradPortDataCommand.Handle(SaveCardPort);

                }
                else if (SaveCardPort.ResultCommand == "DELETE")
                {

                    _COMMAND_SaveCradPortDataCommand.Handle(SaveCardPort);

                }






            }

            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            Session["Number_MaxCard_In_Fo"] = NodeID;
            Session["returnDesc"] = SaveCardPort.Return_Desc;
            Session["Return_Code"] = SaveCardPort.Return_Code;
            return Json(SaveCardPort.ResultCommand, JsonRequestBehavior.AllowGet);

        }


    }
}
