using FBBConfig.Extensions;
using System.Collections.Generic;
using System.Web.Mvc;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBConfig.Controllers
{
    public class AirNetSearchController : FBBConfigController
    {
        //
        // GET: /AirNetSearch/
        //
        // GET: /AirNetWirelessCoverage/
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<DeleteAWCconfigCommand> _DeleteAWCconfigCommand;
        private readonly ICommandHandler<DeleteAWCInfoCommand> _DeleteAWCInfoCommand;
        private readonly ICommandHandler<CreateAWCconfigCommand> _CreateAWCconfigCommand;
        private readonly ICommandHandler<CreateAWCInfoCommand> _CreateAWCInfoCommand;
        private readonly ICommandHandler<UpdateAWCconfigCommand> _UpdateAWCconfigCommand;
        private readonly ICommandHandler<UpdateAWCInfoCommand> _UpdateAWCInfoCommand;
        private readonly ICommandHandler<DeletetempAPCommand> _DeletetempAPCommand;



        public AirNetSearchController(ILogger logger,
             IQueryProcessor queryProcessor, ICommandHandler<DeleteAWCconfigCommand> DeleteAWCconfigCommand,
           ICommandHandler<DeleteAWCInfoCommand> DeleteAWCInfoCommand, ICommandHandler<CreateAWCconfigCommand> CreateAWCconfigCommand
           , ICommandHandler<CreateAWCInfoCommand> CreateAWCInfoCommand, ICommandHandler<UpdateAWCconfigCommand> UpdateAWCconfigCommand
           , ICommandHandler<UpdateAWCInfoCommand> UpdateAWCInfoCommand, ICommandHandler<DeletetempAPCommand> DeletetempAPCommand)
        {
            base._Logger = logger;
            _queryProcessor = queryProcessor;
            _DeleteAWCInfoCommand = DeleteAWCInfoCommand;
            _DeleteAWCconfigCommand = DeleteAWCconfigCommand;
            _CreateAWCconfigCommand = CreateAWCconfigCommand;
            _CreateAWCInfoCommand = CreateAWCInfoCommand;
            _UpdateAWCconfigCommand = UpdateAWCconfigCommand;
            _UpdateAWCInfoCommand = UpdateAWCInfoCommand;
            _DeletetempAPCommand = DeletetempAPCommand;
        }

        [AuthorizeUserAttribute]
        public ActionResult Index(string apname = "", string province = "", string aumphur = "", string tumbon = "", string region = "")
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;

            var awcmodel = new AWCinformation();
            awcmodel.oldmodelpage1 = new AWCModel();
            awcmodel.oldmodelpage1.APname = apname;
            awcmodel.oldmodelpage1.aumphur = aumphur;
            awcmodel.oldmodelpage1.province = province;
            awcmodel.oldmodelpage1.tumbon = tumbon;
            awcmodel.oldmodelpage1.region = region;

            var addedit = Session["addeditap"] as List<AWCconfig>;
            if (addedit != null)
                Session.Remove("addeditap");
            var addnew = Session["addnewap"] as List<AWCconfig>;
            if (addnew != null)
                Session.Remove("addnewap");

            return View(awcmodel);
        }

    }
}
