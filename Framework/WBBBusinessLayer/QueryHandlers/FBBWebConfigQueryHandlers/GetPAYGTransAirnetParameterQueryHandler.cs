using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    class GetPAYGTransAirnetParameterQueryHandler : IQueryHandler<GetPAYGTransAirnetParameterQuery, PAYGTransAirnetParameterListResult>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;

        public GetPAYGTransAirnetParameterQueryHandler(ILogger logger,
            IEntityRepository<FBB_CFG_LOV> lov)
        {
            _logger = logger;
            _lov = lov;
        }

        public PAYGTransAirnetParameterListResult Handle(GetPAYGTransAirnetParameterQuery query)
        {

            PAYGTransAirnetParameterListResult result = new PAYGTransAirnetParameterListResult();

            try
            {
                _logger.Info("GetPAYGTransAirnetParameterQueryHandler : Start.");

                var ConstantNas = (from z in _lov.Get()
                                   where z.LOV_TYPE == "FBB_CONSTANT_NAS"
                                   select z);
                UserConnectionList Temp = new UserConnectionList();
                UserConnectionList SAP = new UserConnectionList();
                UserConnectionList Target = new UserConnectionList();

                Temp.Username = (from z in ConstantNas
                            where z.LOV_NAME == "NasConnectionTemp" && z.ACTIVEFLAG == "Y" && z.DISPLAY_VAL == "UserName"
                                 select z.LOV_VAL1).FirstOrDefault().ToString();

                Temp.Password = (from z in ConstantNas
                           where z.LOV_NAME == "NasConnectionTemp" && z.ACTIVEFLAG == "Y" && z.DISPLAY_VAL == "Password"
                                 select z.LOV_VAL1).FirstOrDefault().ToString();

                SAP.Username = (from z in ConstantNas
                           where z.LOV_NAME == "NasConnectionSap" && z.ACTIVEFLAG == "Y" && z.DISPLAY_VAL == "UserName"
                                select z.LOV_VAL1).FirstOrDefault().ToString();

                SAP.Password = (from z in ConstantNas
                          where z.LOV_NAME == "NasConnectionSap" && z.ACTIVEFLAG == "Y" && z.DISPLAY_VAL == "Password"
                          select z.LOV_VAL1).FirstOrDefault().ToString();
                Target.Username = (from z in ConstantNas
                              where z.LOV_NAME == "NasConnectionTarget" && z.ACTIVEFLAG == "Y" && z.DISPLAY_VAL == "UserName"
                                   select z.LOV_VAL1).FirstOrDefault().ToString();

                Target.Password = (from z in ConstantNas
                             where z.LOV_NAME == "NasConnectionTarget" && z.ACTIVEFLAG == "Y" && z.DISPLAY_VAL == "Password"
                             select z.LOV_VAL1).FirstOrDefault().ToString();

                result.Return_Code = 0;
                result.Return_Desc = "Success";
                result.DataTemp = Temp;
                result.DataSAP = SAP;
                result.DataTarget = Target;
                return result;

            }
            catch (Exception ex)
            {
                _logger.Info("GetPAYGTransAirnetParameterQueryHandler : Error.");
                _logger.Info(ex.Message);

                result.Return_Code = -1;
                result.Return_Desc = "Error call save event service " + ex.Message;
                return result;
            }

        }
    }
}
