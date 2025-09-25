using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WBBBusinessLayer.Extension;
using WBBContract;
using WBBContract.Queries.ExWebServices;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.ExWebServices
{
    public class GetAWCQueryHandler : IQueryHandler<GetAirnetWirelessCoverageQuery, SBNCheckCoverageResponse>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_APCOVERAGE> _apCovService;
        private readonly IEntityRepository<FBB_COVERAGEAREA_RELATION> _covRelService;
        private readonly IEntityRepository<FBB_CFG_LOV> _lovService;
        protected Stopwatch timer;

        public GetAWCQueryHandler(ILogger logger,
            IEntityRepository<FBB_APCOVERAGE> apCovService,
            IEntityRepository<FBB_COVERAGEAREA_RELATION> covRelService,
            IEntityRepository<FBB_CFG_LOV> lovService)
        {
            _logger = logger;
            _apCovService = apCovService;
            _covRelService = covRelService;
            _lovService = lovService;
        }

        private void StartWatch()
        {
            timer = Stopwatch.StartNew();
        }

        private void StopWatch(string actionName)
        {
            timer.Stop();
            _logger.Info(string.Format("Handle '" + actionName + "' take total elapsed time: {0} seconds.", timer.Elapsed.TotalSeconds));
        }

        public SBNCheckCoverageResponse Handle(GetAirnetWirelessCoverageQuery query)
        {
            StartWatch();
            var response = GetAirnetWirelessCoverageHelper.GetAirnetWirelessCoverage(_logger, _apCovService, _covRelService, _lovService, query);
            StopWatch("Calucalate Coverage Area By Latitue and Longitude.");
            return response;
        }

        //private double distance(double lat1, double lon1, double lat2, double lon2, string unit)
        //{
        //    double theta = lon1 - lon2;
        //    double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
        //    dist = Math.Acos(dist);
        //    dist = rad2deg(dist);
        //    dist = dist * 60 * 1.1515;
        //    if (unit.ToUpper() == "K")
        //    {
        //        dist = dist * 1.609344;
        //    }
        //    else if (unit.ToUpper() == "N")
        //    {
        //        dist = dist * 0.8684;
        //    }
        //    return (dist);
        //}

        //private double rad2deg(double rad)
        //{
        //    return (rad / Math.PI * 180.0);
        //}

        //private double deg2rad(double deg)
        //{
        //    return (Math.PI * deg / 180.0);
        //}
    }

    public static class GetAirnetWirelessCoverageHelper
    {
        public static SBNCheckCoverageResponse GetAirnetWirelessCoverage(ILogger logger,
            IEntityRepository<FBB_APCOVERAGE> apCovService,
            IEntityRepository<FBB_COVERAGEAREA_RELATION> covRelService,
            IEntityRepository<FBB_CFG_LOV> lovService,
            GetAirnetWirelessCoverageQuery query)
        {
            var response = new SBNCheckCoverageResponse();

            IEnumerable<FBB_APCOVERAGE> abCoverageList = null;

            abCoverageList = (from r in apCovService.Get()
                              where r.ACTIVE_FLAG == "Y" && r.COVERAGE_STATUS == "ON_SITE"
                              select r);

            if (query.SSO == "")
                abCoverageList = abCoverageList.Where(a => a.ONTARGET_DATE_EX <= DateTime.Now.Date);
            else
                abCoverageList = abCoverageList.Where(a => a.ONTARGET_DATE_IN <= DateTime.Now.Date);

            var available = false;

            try
            {

                var floorLimit = lovService.Get(l => l.LOV_TYPE.Equals("WIRELESS_FLOOR_LIMIT")
                    && l.LOV_NAME.Equals(query.COVERAGETYPE)).Select(l => l.LOV_VAL1).FirstOrDefault();

                if (null == floorLimit)
                {
                    response.RETURN_CODE = -1;
                    response.RETURN_DESC = "WIRELESS_FLOOR_LIMIT is null.";
                    return response;
                }

                if (query.FLOOR < floorLimit.ToSafeDecimal())
                {
                    response.SBNCheckCoverageData.AVALIABLE = "N";
                    response.RETURN_CODE = 0;
                    response.RETURN_DESC = "";
                    return response;
                }

                if (query.CVRID > 0)
                {
                    var data = covRelService.Get(cr => cr.CVRID == query.CVRID)
                        .Select(cr => new
                        {
                            LAT = cr.LATITUDE,
                            LNG = cr.LONGITUDE,
                        });

                    foreach (var latlng in data)
                    {
                        foreach (var ap in abCoverageList)
                        {
                            if (CheckAirNetCoverage(latlng.LAT.ToSafeDouble(),
                                latlng.LNG.ToSafeDouble(),
                                ap.LAT.ToSafeDouble(),
                                ap.LNG.ToSafeDouble(), UnitsOfLength.Kilometer))
                            {
                                available = true;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    foreach (var ap in abCoverageList)
                    {
                        if (CheckAirNetCoverage(query.LAT.ToSafeDouble(),
                            query.LNG.ToSafeDouble(),
                            ap.LAT.ToSafeDouble(),
                            ap.LNG.ToSafeDouble(), UnitsOfLength.Kilometer))
                        {
                            available = true;
                            break;
                        }
                    }
                }


                response.SBNCheckCoverageData.AVALIABLE = available.ToYesNoFlgString();
                response.RETURN_CODE = 0;
                response.RETURN_DESC = "";

            }
            catch (Exception ex)
            {
                response.RETURN_CODE = -1;
                response.RETURN_DESC = ex.GetErrorMessage();
            }

            return response;
        }

        private static bool CheckAirNetCoverage(double lat1, double lon1, double lat2, double lon2, UnitsOfLength unitsOfLength)
        {
            var res_suc = false;
            var coordinate1 = new Coordinate
            {
                Latitude = lat1,
                Longitude = lon1
            };

            var coordinate2 = new Coordinate
            {
                Latitude = lat2,
                Longitude = lon2,
            };

            var check_dis = AWAPDirectionHelper.Distance(coordinate1, coordinate2, unitsOfLength);
            if (check_dis < 3)
            {
                res_suc = true;
                return res_suc;
            }
            else
            {
                res_suc = false;
            }
            return res_suc;
        }
    }
}
