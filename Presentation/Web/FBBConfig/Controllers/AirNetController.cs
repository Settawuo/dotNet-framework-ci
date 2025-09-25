using FBBConfig.Extensions;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.FBBWebConfigQueries.AirWirelessCoverage;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBConfig.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
    public class AirNetController : FBBConfigController
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly IEntityRepository<FBB_AP_INFO> _apifo;
        private readonly ICommandHandler<DeleteAWCconfigCommand> _DeleteAWCconfigCommand;
        private readonly ICommandHandler<DeleteAWCInfoCommand> _DeleteAWCInfoCommand;
        private readonly ICommandHandler<CreateAWCconfigCommand> _CreateAWCconfigCommand;
        private readonly ICommandHandler<CreateAWCInfoCommand> _CreateAWCInfoCommand;
        private readonly ICommandHandler<UpdateAWCconfigCommand> _UpdateAWCconfigCommand;
        private readonly ICommandHandler<UpdateAWCInfoCommand> _UpdateAWCInfoCommand;
        private readonly ICommandHandler<DeletetempAPCommand> _DeletetempAPCommand;
        //
        // GET: /AirNet/
        public AirNetController(ILogger logger,
             IQueryProcessor queryProcessor, ICommandHandler<DeleteAWCconfigCommand> DeleteAWCconfigCommand,
           ICommandHandler<DeleteAWCInfoCommand> DeleteAWCInfoCommand, ICommandHandler<CreateAWCconfigCommand> CreateAWCconfigCommand
           , ICommandHandler<CreateAWCInfoCommand> CreateAWCInfoCommand, ICommandHandler<UpdateAWCconfigCommand> UpdateAWCconfigCommand
           , ICommandHandler<UpdateAWCInfoCommand> UpdateAWCInfoCommand, ICommandHandler<DeletetempAPCommand> DeletetempAPCommand)
        {
            base._Logger = logger;
            _queryProcessor = queryProcessor;
        }

        [AuthorizeUserAttribute]
        public ActionResult Index(string modelpage1 = "", string siteid = "", string fag = "")
        {
            //var query = new GetUserDataQuery
            //{
            //    UserName = "thitimaw"
            //};

            //base.CurrentUser = _queryProcessor.Execute(query);
            //ViewBag.User = base.CurrentUser;

            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;

            var addedit = Session["addeditap"] as List<AWCconfig>;
            if (addedit != null)
                Session.Remove("addeditap");
            var addnew = Session["addnewap"] as List<AWCconfig>;
            if (addnew != null)
                Session.Remove("addnewap");

            if (fag == "edit")
            {
                var a = getedit(modelpage1, siteid);
                return View(a);
            }
            else if (fag == "new")
            {
                var a = getnew(modelpage1);
                return View(a);
            }
            else
            {
                return View();
            }

        }

        public AWCinformation getnew(string modelpage1 = "")
        {

            var oldmodelpage1 = new JavaScriptSerializer().Deserialize<AWCModel>(modelpage1);
            var result = new AWCinformation();
            result.oldmodelpage1 = oldmodelpage1;
            return result;

        }
        public AWCinformation getedit(string modelpage1 = "", string siteid = "")
        {

            return GetDataInformation(siteid, modelpage1);

        }
        private AWCinformation GetDataInformation(string siteid = "", string modelpage1 = "")
        {
            try
            {
                var oldmodelpage1 = new JavaScriptSerializer().Deserialize<AWCModel>(modelpage1);
                var query = new GetAWCEditQuery
                {
                    site_id = siteid,
                    oldmodelpage1 = oldmodelpage1
                };
                return _queryProcessor.Execute(query);


            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return new AWCinformation();
            }

        }

    }
}
