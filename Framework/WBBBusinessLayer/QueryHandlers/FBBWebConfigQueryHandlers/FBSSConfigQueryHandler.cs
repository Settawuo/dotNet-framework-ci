using System;
using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class FBSSConfigQueryHandler : IQueryHandler<FBSSConfigQuery, FBSSConfig>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBSS_CONFIG_TBL> _configTBL;

        public FBSSConfigQueryHandler(ILogger logger, IEntityRepository<FBSS_CONFIG_TBL> configTBL)
        {
            _logger = logger;
            _configTBL = configTBL;
        }

        public FBSSConfig Handle(FBSSConfigQuery query)
        {
            var config = (from c in _configTBL.Get() select c);

            if (query.CON_TYPE != null)
            {
                if (query.CON_TYPE != "")
                {
                    config = (from c in config where (c.CON_TYPE.ToUpper() == query.CON_TYPE.ToUpper() && c.ACTIVEFLAG == "Y") select c);
                }
            }

            var result = new FBSSConfig();
            result.VAL_LIST = new List<FbssConfigTBL>();


            var display_program_process = (from c in config where (c.CON_NAME == "PROGRAM_PROCESS") select c.DISPLAY_VAL).ToList().First();

            if (display_program_process == "Y")
            {
                result.PROGRAM_PROCESS = true;
                var sysDate = System.DateTime.Now;

                var config_start_div = (from c in config
                                        where (c.CON_NAME == "DATE_START" || c.CON_NAME == "DATE_DIV")
                                        select new FbssConfigTBL
                                        {
                                            CON_ID = c.CON_ID,
                                            CON_TYPE = c.CON_TYPE,
                                            CON_NAME = c.CON_NAME,
                                            DISPLAY_VAL = c.DISPLAY_VAL,
                                            VAL1 = c.VAL1,
                                            VAL2 = c.VAL2,
                                            VAL3 = c.VAL3,
                                            VAL4 = c.VAL4,
                                            VAL5 = c.VAL5,
                                            ACTIVEFLAG = c.ACTIVEFLAG,
                                            ORDER_BY = (c.ORDER_BY == null) ? 0 : c.ORDER_BY,
                                            DEFAULT_VALUE = c.DEFAULT_VALUE,
                                            CREATED_BY = c.CREATED_BY,
                                            CREATED_DATE = c.CREATED_DATE,
                                            UPDATED_BY = c.UPDATED_BY,
                                            UPDATED_DATE = c.UPDATED_DATE
                                        }).ToList();

                config_start_div = config_start_div.Where(a => a.CON_NAME == "DATE_START").Select(c =>
                {
                    if (c.CON_NAME == "DATE_START" && c.DISPLAY_VAL == "Y")
                    {
                        c.VAL1 = c.VAL1;
                    }
                    else
                    {
                        if (DateTime.TryParse(c.VAL1, out DateTime parsedDate) && (c.CON_NAME == "DATE_DIV" && c.DISPLAY_VAL == "Y"))
                        {
                            c.VAL1 = (sysDate - parsedDate.AddDays(1)).ToString("ddMMyyyy HHmmss");
                        }
                        else
                        {
                            c.VAL1 = sysDate.AddDays(-(1 + 1)).ToString("ddMMyyyy HHmmss");
                        }
                    }

                    return c;
                }).OrderBy(p => p.CON_TYPE)
                  .ThenBy(p => p.ORDER_BY)
                  .ThenBy(p => p.UPDATED_DATE)
                  .ToList();

                var config_date_to = (from c in config
                                      where (c.CON_NAME == "DATE_TO")
                                      select new FbssConfigTBL
                                      {
                                          CON_ID = c.CON_ID,
                                          CON_TYPE = c.CON_TYPE,
                                          CON_NAME = c.CON_NAME,
                                          DISPLAY_VAL = c.DISPLAY_VAL,
                                          VAL1 = c.VAL1, // Use the original VAL1 here
                                          VAL2 = c.VAL2,
                                          VAL3 = c.VAL3,
                                          VAL4 = c.VAL4,
                                          VAL5 = c.VAL5,
                                          ACTIVEFLAG = c.ACTIVEFLAG,
                                          ORDER_BY = (c.ORDER_BY == null) ? 0 : c.ORDER_BY,
                                          DEFAULT_VALUE = c.DEFAULT_VALUE,
                                          CREATED_BY = c.CREATED_BY,
                                          CREATED_DATE = c.CREATED_DATE,
                                          UPDATED_BY = c.UPDATED_BY,
                                          UPDATED_DATE = c.UPDATED_DATE
                                      })
                          .OrderBy(p => p.CON_TYPE)
                          .ThenBy(p => p.ORDER_BY)
                          .ThenBy(p => p.UPDATED_DATE)
                          .ToList();

                foreach (var item in config_date_to)
                {
                    item.VAL1 = (item.CON_NAME == "DATE_TO" && item.DISPLAY_VAL == "Y") ? item.VAL1 : sysDate.ToString("ddMMyyyy HHmmss");
                }

                result.VAL_LIST.AddRange(config_start_div);
                result.VAL_LIST.AddRange(config_date_to);
            }
            else
            {
                result.PROGRAM_PROCESS = false;
            }


            return result;
        }

    }
}
