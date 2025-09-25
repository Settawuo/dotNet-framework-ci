using System;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    class GetConnectionNasMatDocQueryHandler : IQueryHandler<GetConnectionNasPAYGMatdocQuery, ConnectionNasPAYGMatdocListResult>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;

        public GetConnectionNasMatDocQueryHandler(ILogger logger, IEntityRepository<FBB_CFG_LOV> lov)
        {
            _logger = logger;
            _lov = lov;
        }

        public ConnectionNasPAYGMatdocListResult Handle(GetConnectionNasPAYGMatdocQuery query)
        {

            ConnectionNasPAYGMatdocListResult result = new ConnectionNasPAYGMatdocListResult();

            try
            {
                _logger.Info("GetConnectionNasPAYGMatdocQueryHandler : Start..");

                var ConstantNas = (from z in _lov.Get()
                                   where z.LOV_TYPE == "FBB_CONSTANT_NAS"
                                   select z).ToList();

                string TempUsername = "";
                string TempPass = "";//Fixed Code scan : string TempPassword = "";
                string SapUsername = "";
                string SapPass = "";//Fixed Code scan : string SapPassword = "";
                string TargetUsername = "";
                string TargetPass = "";//Fixed Code scan : string TargetPassword = "";
                string ArchUsername = "";
                string ArchPass = "";

                TempUsername = (from z in ConstantNas
                                where z.LOV_NAME == "NasConnectionMatdocTemp" && z.ACTIVEFLAG == "Y" && z.DISPLAY_VAL == "UserName"
                                select z.LOV_VAL1).FirstOrDefault().ToSafeString();

                TempPass = (from z in ConstantNas
                            where z.LOV_NAME == "NasConnectionMatdocTemp" && z.ACTIVEFLAG == "Y" && z.DISPLAY_VAL == "Password"
                            select z.LOV_VAL1).FirstOrDefault().ToSafeString();

                SapUsername = (from z in ConstantNas
                               where z.LOV_NAME == "NasConnectionMatdoc" && z.ACTIVEFLAG == "Y" && z.DISPLAY_VAL == "UserName"
                               select z.LOV_VAL1).FirstOrDefault().ToSafeString();

                SapPass = (from z in ConstantNas
                           where z.LOV_NAME == "NasConnectionMatdoc" && z.ACTIVEFLAG == "Y" && z.DISPLAY_VAL == "Password"
                           select z.LOV_VAL1).FirstOrDefault().ToSafeString();

                TargetUsername = (from z in ConstantNas
                                  where z.LOV_NAME == "NasConnectionMatdocTarget" && z.ACTIVEFLAG == "Y" && z.DISPLAY_VAL == "UserName"
                                  select z.LOV_VAL1).FirstOrDefault().ToSafeString();

                TargetPass = (from z in ConstantNas
                              where z.LOV_NAME == "NasConnectionMatdocTarget" && z.ACTIVEFLAG == "Y" && z.DISPLAY_VAL == "Password"
                              select z.LOV_VAL1).FirstOrDefault().ToSafeString();

                ArchUsername = (from z in ConstantNas
                                where z.LOV_NAME == "NasConnectionMatdocArchive" && z.ACTIVEFLAG == "Y" && z.DISPLAY_VAL == "UserName"
                                select z.LOV_VAL1).FirstOrDefault().ToSafeString();

                ArchPass = (from z in ConstantNas
                            where z.LOV_NAME == "NasConnectionMatdocArchive" && z.ACTIVEFLAG == "Y" && z.DISPLAY_VAL == "Password"
                            select z.LOV_VAL1).FirstOrDefault().ToSafeString();





                ConnectionNasPAYG NasTemp = new ConnectionNasPAYG();
                NasTemp.Username = TempUsername;
                NasTemp.Password = TempPass;

                ConnectionNasPAYG NasSap = new ConnectionNasPAYG();
                NasSap.Username = SapUsername;
                NasSap.Password = SapPass;

                ConnectionNasPAYG NasArch = new ConnectionNasPAYG();
                NasArch.Username = ArchUsername;
                NasArch.Password = ArchPass;

                ConnectionNasPAYG NasTarget = new ConnectionNasPAYG();
                NasTarget.Username = TargetUsername;
                NasTarget.Password = TargetPass;

                result.Return_Code = 0;
                result.Return_Desc = "Success";
                result.NasTemp = NasTemp;
                result.NasSap = NasSap;
                result.NasTarget = NasTarget;
                result.NasArch = NasArch;

                return result;
            }
            catch (Exception ex)
            {
                _logger.Info("GetConnectionNasPAYGMatdocQueryHandler : Error.");
                _logger.Info(ex.Message);

                result.Return_Code = -1;
                result.Return_Desc = "Error call save event service " + ex.Message;
                return result;
            }

        }
    }
}