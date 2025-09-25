using System.Linq;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers
{
    public class CostInstallationCommandHandler : ICommandHandler<CostInstallationCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBSS_INSTALLATION_COST> _costInstall;

        public CostInstallationCommandHandler(
            ILogger logger,
            IEntityRepository<FBSS_INSTALLATION_COST> costInstall,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _costInstall = costInstall;
            _uow = uow;
        }

        public void Handle(CostInstallationCommand command)
        {
            var _cost = new FBSS_INSTALLATION_COST();

            _cost.SERVICE = command.SERVICE.ToSafeString();
            _cost.VENDOR = command.VENDOR.ToSafeString();
            _cost.ORDER_TYPE = command.ORDER_TYPE.ToSafeString();
            _cost.INTERNET = command.RATE;
            _cost.PLAYBOX = command.PLAYBOX;
            _cost.VOIP = command.VOIP;
            _cost.EFFECTIVE_DATE = command.EFFECTIVE_DATE;
            _cost.EXPIRE_DATE = command.EXPIRE_DATE;
            _cost.REMARK = command.REMARK.ToSafeString();
            _cost.INS_OPTION = command.PHASE;

            if (command.ACTION == WBBContract.Commands.ActionType.Insert)
            {
                var cos = _costInstall.Get(a => a.SERVICE == command.SERVICE && a.VENDOR == command.VENDOR && a.ORDER_TYPE == command.ORDER_TYPE && a.INS_OPTION == command.PHASE);
                var anyItem = true;
                if (cos.Any())
                {
                    foreach (var co in cos)
                    {
                        if (((command.EFFECTIVE_DATE >= co.EFFECTIVE_DATE) && (co.EXPIRE_DATE == null)) || ((command.EFFECTIVE_DATE <= co.EFFECTIVE_DATE) && command.EXPIRE_DATE == null))
                        {
                            anyItem = false;
                        }
                        else if (
                            ((command.EFFECTIVE_DATE >= co.EFFECTIVE_DATE) && (command.EFFECTIVE_DATE <= co.EXPIRE_DATE) || (command.EXPIRE_DATE >= co.EFFECTIVE_DATE) && (command.EXPIRE_DATE <= co.EXPIRE_DATE))
                            ||
                            ((co.EFFECTIVE_DATE >= command.EFFECTIVE_DATE) && (co.EFFECTIVE_DATE <= command.EXPIRE_DATE) || (co.EXPIRE_DATE >= command.EFFECTIVE_DATE) && (co.EXPIRE_DATE <= command.EXPIRE_DATE))
                            )
                        {
                            anyItem = false;
                        }
                    }


                }

                if (anyItem) _costInstall.Create(_cost);

            }
            else if (command.ACTION == WBBContract.Commands.ActionType.Update)
            {
                _costInstall.Update(_cost);

            }
            else if (command.ACTION == WBBContract.Commands.ActionType.Delete)
            {
                var cos = _costInstall.Get(a => a.SERVICE == command.SERVICE &&
                    a.VENDOR == command.VENDOR && a.ORDER_TYPE == command.ORDER_TYPE &&
                    a.EFFECTIVE_DATE == command.EFFECTIVE_DATE);

                if (cos.Any())
                {
                    foreach (var co in cos)
                    {
                        _costInstall.Delete(co);
                    }
                }
                //_costInstall.Delete(_cost);
            }
            _uow.Persist();
            command.RETURN_CODE = 0;
            command.RETURN_DESC = "Already Existing.";

        }
    }
}
