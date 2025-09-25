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



    public class Assign_VOIP_IPCommandHandler : ICommandHandler<AssignVOIPIPCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_VOIP_IPMASTER_INFO> _FBB_VOIP_IPMASTER_INFO;
        private readonly IWBBUnitOfWork _uow;

        public Assign_VOIP_IPCommandHandler(ILogger logger, IEntityRepository<string> objService, IEntityRepository<FBB_VOIP_IPMASTER_INFO> FBB_VOIP_IPMASTER_INFO)
        {
            _logger = logger;
            _objService = objService;
            _FBB_VOIP_IPMASTER_INFO = FBB_VOIP_IPMASTER_INFO;
        }
        public void Handle(AssignVOIPIPCommand command)
        {
            
            
            var dataIp = (from vm1 in _FBB_VOIP_IPMASTER_INFO.Get()
                          where vm1.STATUS == "1" && vm1.ACTIVE_FLAG == "Y"

                          select vm1);
            if (dataIp.Any())
            {

                var data = (from vm in _FBB_VOIP_IPMASTER_INFO.Get()
                            where vm.IP_ADDRESS == dataIp.FirstOrDefault().IP_ADDRESS && vm.STATUS == "1" && vm.ACTIVE_FLAG == "Y"
                            select vm);
                var date = System.DateTime.Now;
                if (data.Any())
                {
                    var FBB_VOIP_IPMASTER_INFO_Update = new FBB_VOIP_IPMASTER_INFO
                    {
                        STATUS = "2",
                        CREATED_DATE = date,
                        REMARK = command.REFF_KEY,
                        UPDATED_BY = command.REFF_USER,
                        UPDATED_DATE = date                      
                    };

                    
                    _FBB_VOIP_IPMASTER_INFO.Update(FBB_VOIP_IPMASTER_INFO_Update);
                    _uow.Persist();

                    command.RETURN_CODE = 0;
                    command.RETURN_DESC = "Success";
                    command.RESULT.IP = FBB_VOIP_IPMASTER_INFO_Update.IP_ADDRESS;
                    command.RESULT.PORT = "";
                    }
                    //command.IP = FBB_VOIP_IPMASTER_INFO_Update.IP_ADDRESS;
                    //command.PORT = "";
                }
                else
                {

                    command.RETURN_CODE = -1;
                    command.RETURN_DESC = "error";

                }

            }
            

        }
    

}
