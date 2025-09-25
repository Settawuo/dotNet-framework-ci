using System;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetSaveOrderNewHandler : IQueryHandler<GetSaveOrderNewQuery, SaveOrderNewModel>
    {
        private readonly ILogger _logger;

        public GetSaveOrderNewHandler(ILogger logger)
        {
            _logger = logger;
        }

        //public List<PackageModel> Handle(GetListPackageByServiceQuery query)
        //{
        //    var model = new List<PackageModel>();
        //    using (var service = new SBNServices.SBNWebServiceService())
        //    {
        //        //var data = service.listPackageByService(query.PRODUCT_SUBTYPE);

        //        //if (data.RETURN_CODE.GetValueOrDefault() != 0)
        //        //{
        //        //    _logger.Info(data.RETURN_MESSAGE);
        //        //}
        //        //else
        //        //{
        //        //    var packages = data.PackageByServiceArray.Select(s => new PackageModel
        //        //    {
        //        //        PACKAGE_CODE = s.PACKAGE_CODE,
        //        //        PACKAGE_NAME = s.PACKAGE_NAME,
        //        //        PACKAGE_GROUP = s.PACKAGE_GROUP,
        //        //        RECURRING_CHARGE = s.RECURRING_CHARGE,
        //        //        TECHNOLOGY = s.TECHNOLOGY,
        //        //        DOWNLOAD_SPEED = s.DOWNLOAD_SPEED,
        //        //        UPLOAD_SPEED = s.UPLOAD_SPEED,
        //        //        INITIATION_CHARGE = s.INITIATION_CHARGE,
        //        //        DISCOUNT_INITIATION_CHARGE = s.DISCOUNT_INITIATION_CHARGE,
        //        //        PACKAGE_CODE_ONTOP = s.PACKAGE_CODE_ONTOP,
        //        //        PACKAGE_NAME_ONTOP = s.PACKAGE_NAME_ONTOP,
        //        //        SFF_PROMOTION_BILL_THA = s.SFF_PROMOTION_BILL_THA,
        //        //        SFF_PROMOTION_BILL_ENG = s.SFF_PROMOTION_BILL_ENG,
        //        //        SFF_PROMOTION_BILL_THA_ONTOP = s.SFF_PROMOTION_BILL_THA_ONTOP,
        //        //        SFF_PROMOTION_BILL_ENG_ONTOP = s.SFF_PROMOTION_BILL_ENG_ONTOP,
        //        //        INITIATION_CHARGE_ONTOP = s.INITIATION_CHARGE_ONTOP,
        //        //        DISCOUNT_INITIATION_ONTOP = s.DISCOUNT_INITIATION_ONTOP,
        //        //        PACKAGE_CLASS_ONTOP = s.PACKAGE_CLASS_ONTOP,
        //        //        PACKAGE_CLASS_MAIN = s.PACKAGE_CLASS_MAIN,
        //        //    });

        //        //    model = packages.ToList();
        //        //}
        //    }
        //    return model;
        //}

        public SaveOrderNewModel Handle(GetSaveOrderNewQuery query)
        {
            throw new NotImplementedException();
        }
    }
}
