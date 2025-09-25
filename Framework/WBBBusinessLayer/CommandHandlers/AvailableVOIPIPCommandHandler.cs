using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBBContract;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.Extensions;
using WBBContract.Commands;

namespace WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers
{
    public class AvailableVOIPIPCommandHandler : ICommandHandler<AvailableVOIPIPCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_VOIP_IPMASTER_INFO> _FBB_VOIP_IPMASTER_INFO;
        private readonly IWBBUnitOfWork _uow;

        public AvailableVOIPIPCommandHandler(ILogger logger, IEntityRepository<string> objService, IEntityRepository<FBB_VOIP_IPMASTER_INFO> FBB_VOIP_IPMASTER_INFO)
        {
            _logger = logger;
            _objService = objService;
            _FBB_VOIP_IPMASTER_INFO = FBB_VOIP_IPMASTER_INFO;
        }
        public void Handle(AvailableVOIPIPCommand command)
        {



            var data = from v in _FBB_VOIP_IPMASTER_INFO.Get()
                       where v.IP_ADDRESS == command.IP_ADDRESS.ToSafeString()
                       select v;
            var date = System.DateTime.Now;
            if (data.Any())
            {
                var FBB_VOIP_IPMASTER_INFO_Update = new FBB_VOIP_IPMASTER_INFO
                {
                    STATUS = "1",
                    CREATED_DATE = date,
                    REMARK = "",
                    UPDATED_BY = command.REFF_USER,
                    UPDATED_DATE = date



                };

                _FBB_VOIP_IPMASTER_INFO.Update(FBB_VOIP_IPMASTER_INFO_Update);
                _uow.Persist();
                command.Return_Code = 0;
                command.Return_Desc = "Success";
                
            }
            else
            {

                command.Return_Code = -1;
                command.Return_Desc = "error";

            }
        }
    }
}
