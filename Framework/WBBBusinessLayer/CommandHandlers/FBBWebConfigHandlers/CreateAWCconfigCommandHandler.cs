using System.Linq;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers
{
    public class CreateAWCconfigCommandHandler : ICommandHandler<CreateAWCconfigCommand>
    {

        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_AP_INFO> _FBB_AP_INFO;

        private readonly IWBBUnitOfWork _uow;

        public CreateAWCconfigCommandHandler(ILogger logger,
            IWBBUnitOfWork uow, IEntityRepository<FBB_AP_INFO> FBB_AP_INFO)

        {
            _logger = logger;
            _uow = uow;
            _FBB_AP_INFO = FBB_AP_INFO;

        }

        public void Handle(CreateAWCconfigCommand command)
        {
            var model = command.AWCconfigModel;
            var check = from r in _FBB_AP_INFO.Get()
                        where r.AP_NAME == model.AP_Name && r.ACTIVE_FLAG == "Y"
                        select r;

            if (check.Any())
            {
                command.FlagDup = true;
            }
            //else
            //{                 
            //        //insert 
            //         var fbbDSLAM = new FBB_AP_INFO
            //        {
            //            CREATED_BY = model.user,
            //            CREATED_DATE = DateTime.Now,
            //            UPDATED_BY = model.user,
            //            UPDATED_DATE = DateTime.Now,
            //            SECTOR = model.Sector,
            //            AP_NAME = model.AP_Name.Trim(),
            //            ACTIVE_FLAG = "Y",
            //            SITE_ID = model.Site_id,


            //        };

            //         _FBB_AP_INFO.Create(fbbDSLAM);
            //        _uow.Persist();                 
            //}
        }
    }
}
