using System;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;
using WBBEntity.PanelModels.WebServiceModels;


namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    class GetConnectionTransAirnetNasPAYGQueryHandler : IQueryHandler<GetConnectionTransAirnetNasPAYGQuery, ConnectionNasPAYGTransAirnetListResult>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;

        public GetConnectionTransAirnetNasPAYGQueryHandler(ILogger logger, IEntityRepository<FBB_CFG_LOV> lov)
        {
            _logger = logger;
            _lov = lov;
        }

        public ConnectionNasPAYGTransAirnetListResult Handle(GetConnectionTransAirnetNasPAYGQuery query)
        {

            ConnectionNasPAYGTransAirnetListResult result = new ConnectionNasPAYGTransAirnetListResult();

            try
            {
                _logger.Info("GetConnectionNasPAYGQueryHandler : Start..");

                var ConstantNas = (from z in _lov.Get()
                                   where z.LOV_TYPE == "FBB_CONSTANT_ENHANCE_NAS"
                                   select z).ToList();

                string TempUsername = "";
                string TempPass = "";//Fixed Code scan : string TempPassword = "";
                string SapUsername = "";
                string SapPass = "";//Fixed Code scan : string SapPassword = "";
                string TargetUsername = "";
                string TargetPass = "";//Fixed Code scan : string TargetPassword = "";

                var tempNasList = (from z in ConstantNas
                                where z.LOV_NAME == "TRANSAIRNET" && z.ACTIVEFLAG == "Y" && z.DISPLAY_VAL == "TEMP_PATH"
                                   select z).ToList();

                var sapInNasList = (from z in ConstantNas
                                   where z.LOV_NAME == "TRANSAIRNET" && z.ACTIVEFLAG == "Y" && z.DISPLAY_VAL == "SAP_IN"
                                   select z).ToList();

                var targetNasList = (from z in ConstantNas
                                    where z.LOV_NAME == "TRANSAIRNET" && z.ACTIVEFLAG == "Y" && z.DISPLAY_VAL == "TARGET_PATH"
                                    select z).ToList();

                var archiveNasList = (from z in ConstantNas
                                     where z.LOV_NAME == "TRANSAIRNET" && z.ACTIVEFLAG == "Y" && z.DISPLAY_VAL == "ARCHIVE_PATH"
                                     select z).ToList();

                var sapInNas3BBList = (from z in ConstantNas
                                       where z.LOV_NAME == "TRANSAIRNET" && z.ACTIVEFLAG == "Y" && z.DISPLAY_VAL == "SAP_IN_3BB"
                                       select z).ToList();


                ConnectionNasPAYGTransAirnet NasTemp = new ConnectionNasPAYGTransAirnet();
                foreach (var item in tempNasList)
                {
                    NasTemp.Username = item.LOV_VAL2;
                    NasTemp.Password = item.LOV_VAL3;
                    NasTemp.Domain = item.LOV_VAL4;
                    NasTemp.Path = item.LOV_VAL1;
                }

                ConnectionNasPAYGTransAirnet NasSap = new ConnectionNasPAYGTransAirnet();
                foreach (var item in sapInNasList)
                {
                    NasSap.Username = item.LOV_VAL2;
                    NasSap.Password = item.LOV_VAL3;
                    NasSap.Domain = item.LOV_VAL4;
                    NasSap.Path = item.LOV_VAL1;
                }

                ConnectionNasPAYGTransAirnet NasSap3BB = new ConnectionNasPAYGTransAirnet();
                foreach (var item in sapInNas3BBList)
                {
                    NasSap3BB.Username = item.LOV_VAL2;
                    NasSap3BB.Password = item.LOV_VAL3;
                    NasSap3BB.Domain = item.LOV_VAL4;
                    NasSap3BB.Path = item.LOV_VAL1;
                }

                ConnectionNasPAYGTransAirnet NasTarget = new ConnectionNasPAYGTransAirnet();
                foreach (var item in targetNasList)
                {
                    NasTarget.Username = item.LOV_VAL2;
                    NasTarget.Password = item.LOV_VAL3;
                    NasTarget.Domain = item.LOV_VAL4;
                    NasTarget.Path = item.LOV_VAL1;
                }

                ConnectionNasPAYGTransAirnet NasArchive = new ConnectionNasPAYGTransAirnet();
                foreach (var item in archiveNasList)
                {
                    NasArchive.Username = item.LOV_VAL2;
                    NasArchive.Password = item.LOV_VAL3;
                    NasArchive.Domain = item.LOV_VAL4;
                    NasArchive.Path = item.LOV_VAL1;
                }

                result.Return_Code = 0;
                result.Return_Desc = "Success";
                result.NasTemp = NasTemp;
                result.NasSap3BB = NasSap3BB;
                result.NasSap = NasSap;
                result.NasTarget = NasTarget;
                result.NasArchive = NasArchive;

                return result;
            }
            catch (Exception ex)
            {
                _logger.Info("GetConnectionNasPAYGQueryHandler : Error.");
                _logger.Info(ex.Message);

                result.Return_Code = -1;
                result.Return_Desc = "Error call save event service " + ex.Message;
                return result;
            }

        }
    }
}
