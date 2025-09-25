using System.Linq;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetOwnerProductByNoHandler : IQueryHandler<GetOwnerProductByNoQuery, DropdownModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CUST_PROFILE> _custProfile;
        private readonly IEntityRepository<FBB_CUST_PACKAGE> _custPackage;
        private readonly IEntityRepository<FBB_CUST_CONTACT> _custContact;

        public GetOwnerProductByNoHandler(ILogger logger, IEntityRepository<FBB_CUST_PROFILE> custProfile,
            IEntityRepository<FBB_CUST_PACKAGE> custPackage, IEntityRepository<FBB_CUST_CONTACT> custContact)
        {
            _logger = logger;
            _custProfile = custProfile;
            _custPackage = custPackage;
            _custContact = custContact;
        }

        public DropdownModel Handle(GetOwnerProductByNoQuery query)
        {
            var a = (from cp in _custProfile.Get()
                     join pkg in _custPackage.Get() on cp.CUST_NON_MOBILE equals pkg.CUST_NON_MOBILE
                     where pkg.PACKAGE_CLASS == "Main" && cp.CUST_NON_MOBILE == query.No
                     select new DropdownModel
                     {
                         Value = pkg.PACKAGE_OWNER,
                         Value2 = pkg.PACKAGE_CODE,
                         Value3 = cp.ADDRESS_ID,
                         Value5 = pkg.PACKAGE_SUBTYPE
                     }).ToList();

            if (a != null && a.Count > 0)
            {
                var contactMobileNo = "";
                var b = (from c in _custContact.Get()
                        join p in _custProfile.Get() on c.BA_ID equals p.BA_ID
                        where c.CONTACT_SEQ == 1 && p.CUST_NON_MOBILE == query.No && p.BA_ID == query.BA_ID
                        select c.CONTACT_MOBILE_PHONE1).ToList();

                if (b != null && b.Count > 0)
                    contactMobileNo = b.FirstOrDefault();

                var result = a.FirstOrDefault();
                result.Value4 = contactMobileNo;
                return result;
            }
            else
                return new DropdownModel { };

        }


    }
}
