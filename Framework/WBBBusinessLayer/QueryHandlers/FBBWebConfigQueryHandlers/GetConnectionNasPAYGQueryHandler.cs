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
    class GetConnectionNasPAYGQueryHandler : IQueryHandler<GetConnectionNasPAYGQuery, ConnectionNasPAYGListResult>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;

        public GetConnectionNasPAYGQueryHandler(ILogger logger, IEntityRepository<FBB_CFG_LOV> lov)
        {
            _logger = logger;
            _lov = lov;
        }

        public ConnectionNasPAYGListResult Handle(GetConnectionNasPAYGQuery query)
        {

            ConnectionNasPAYGListResult result = new ConnectionNasPAYGListResult();

            try
            {
                _logger.Info("GetConnectionNasPAYGQueryHandler : Start..");

                var ConstantNas = (from z in _lov.Get()
                                   where z.LOV_TYPE == "FBB_CONSTANT_NAS"
                                   select z).ToList();

                string TempUsername = "";
                string TempPass = "";//Fixed Code scan : string TempPassword = "";
                string SapUsername = "";
                string SapPass = "";//Fixed Code scan : string SapPassword = "";
                string TargetUsername = "";
                string TargetPass = "";//Fixed Code scan : string TargetPassword = "";
                string SapNewUsername = "";
                string SapNewPass = "";

                TempUsername = (from z in ConstantNas
                                where z.LOV_NAME == "NasConnectionTemp" && z.ACTIVEFLAG == "Y" && z.DISPLAY_VAL == "UserName"
                                select z.LOV_VAL1).FirstOrDefault().ToSafeString();

                TempPass = (from z in ConstantNas
                            where z.LOV_NAME == "NasConnectionTemp" && z.ACTIVEFLAG == "Y" && z.DISPLAY_VAL == "Password"
                            select z.LOV_VAL1).FirstOrDefault().ToSafeString();

                SapUsername = (from z in ConstantNas
                               where z.LOV_NAME == "NasConnectionSap" && z.ACTIVEFLAG == "Y" && z.DISPLAY_VAL == "UserName"
                               select z.LOV_VAL1).FirstOrDefault().ToSafeString();

                SapPass = (from z in ConstantNas
                           where z.LOV_NAME == "NasConnectionSap" && z.ACTIVEFLAG == "Y" && z.DISPLAY_VAL == "Password"
                           select z.LOV_VAL1).FirstOrDefault().ToSafeString();

                TargetUsername = (from z in ConstantNas
                                  where z.LOV_NAME == "NasConnectionTarget" && z.ACTIVEFLAG == "Y" && z.DISPLAY_VAL == "UserName"
                                  select z.LOV_VAL1).FirstOrDefault().ToSafeString();

                TargetPass = (from z in ConstantNas
                              where z.LOV_NAME == "NasConnectionTarget" && z.ACTIVEFLAG == "Y" && z.DISPLAY_VAL == "Password"
                              select z.LOV_VAL1).FirstOrDefault().ToSafeString();

                SapNewUsername = (from z in ConstantNas
                               where z.LOV_NAME == "NasConnectionSapNew" && z.ACTIVEFLAG == "Y" && z.DISPLAY_VAL == "UserName"
                               select z.LOV_VAL1).FirstOrDefault().ToSafeString();

                SapNewPass = (from z in ConstantNas
                           where z.LOV_NAME == "NasConnectionSapNew" && z.ACTIVEFLAG == "Y" && z.DISPLAY_VAL == "Password"
                           select z.LOV_VAL1).FirstOrDefault().ToSafeString();


                ConnectionNasPAYG NasTemp = new ConnectionNasPAYG();
                NasTemp.Username = TempUsername;
                NasTemp.Password = TempPass;

                ConnectionNasPAYG NasSap = new ConnectionNasPAYG();
                NasSap.Username = SapUsername;
                NasSap.Password = SapPass;

                ConnectionNasPAYG NasSapNew = new ConnectionNasPAYG();
                NasSapNew.Username = SapNewUsername;
                NasSapNew.Password = SapNewPass;

                ConnectionNasPAYG NasTarget = new ConnectionNasPAYG();
                NasTarget.Username = TargetUsername;
                NasTarget.Password = TargetPass;

                result.Return_Code = 0;
                result.Return_Desc = "Success";
                result.NasTemp = NasTemp;
                result.NasSap = NasSap;
                result.NasTarget = NasTarget;
                result.NasSapNew = NasSapNew;

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
