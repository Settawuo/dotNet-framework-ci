using System;
using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class CheckCoverageListbvByAddressIDServiceQueryHandler : IQueryHandler<CheckCoverageListbvByAddressIDQuery, List<CheckCoverageListbvByAddressIDDataModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_FBSS_LISTBV> _FBB_FBSS_LISTBV;
        private readonly IEntityRepository<FBB_ZIPCODE> _fbb_zipcode;

        public CheckCoverageListbvByAddressIDServiceQueryHandler(ILogger logger,
            IEntityRepository<FBB_FBSS_LISTBV> FBB_FBSS_LISTBV,
            IEntityRepository<FBB_ZIPCODE> fbb_zipcode)
        {
            _logger = logger;
            _FBB_FBSS_LISTBV = FBB_FBSS_LISTBV;
            _fbb_zipcode = fbb_zipcode;
        }

        public List<CheckCoverageListbvByAddressIDDataModel> Handle(CheckCoverageListbvByAddressIDQuery query)
        {
            //R22.09 3BB : Add FbbCpGwInterface/Checkcoverage
            InterfaceLog3BBCommand log3bb = null;
            //CheckCoverageListbvByAddressIDDataModel response = new CheckCoverageListbvByAddressIDDataModel();
            var bvList = new List<CheckCoverageListbvByAddressIDDataModel>();

            try
            {
                var allAddressId = (
                          from l in _FBB_FBSS_LISTBV.Get()
                          where
                            l.ADDRESS_ID == query.buildingAddressID &&
                            //l.LANGUAGE == "E" &&
                            l.ACTIVE_FLAG == "Y"
                          select new CheckCoverageListbvByAddressIDDataModel
                          {
                              AddressId = l.ADDRESS_ID,
                              AddressType = l.ADDRESS_TYPE,
                              PostalCode = l.POSTAL_CODE,
                              Language = l.LANGUAGE,
                              SubDistricName = l.SUB_DISTRICT,
                              BuildingName = l.BUILDING_NAME,
                              BuildingNo = l.BUILDING_NO,
                              PhoneFlag = "N",
                              Latitude = l.LATITUDE,
                              Longitude = l.LONGTITUDE,
                              UnitNo = "",
                              FloorNo = "",
                              TransactionId = query.transactionId,
                              FttrFlag = l.FTTR_FLAG,
                              FullUrl = "3BB"

                          });

                bvList = allAddressId.ToList();

                if (bvList.Count() > 1)
                {
                    if (string.IsNullOrEmpty(query.buildingAddressID))
                    {
                        bvList = allAddressId.ToList();
                    }
                    else
                    {
                        bvList = allAddressId.Where(b => b.Language == "E").ToList();
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(query.buildingAddressID))
                    {
                        bvList = allAddressId.ToList();
                    }
                    else
                    {
                        bvList = allAddressId.Where(b => b.AddressId == query.buildingAddressID).ToList();
                    }
                }


            }
            catch (Exception ex)
            {
                //response.returnCode = "-1";
                //response.returnMessage = ex.Message;

                //if (query.FullUrl == "3BB")
                //InterfaceLogServiceHelper.EndInterfaceLog3BB(_uow, _intfLog3bb, response, log3bb, "Failed", ex.Message, "");
            }

            return bvList;
        }

        private string convertDate(string strDate)
        {
            string responseDate = "";
            if (strDate.IndexOf("/") >= 0)
            {
                var strDateSplit = strDate.Split('/');
                responseDate = strDateSplit[2] + '-' + strDateSplit[1] + '-' + strDateSplit[0];
            }
            return responseDate;
        }
    }
}
