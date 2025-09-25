using System;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class GetOtherInfoHandle : IQueryHandler<GetOtherInfo, string>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CUST_PROFILE> _FBB_CUST_PROFILE;
        private readonly IEntityRepository<FBB_CUST_CONTACT> _FBB_CUST_CONTACT;

        public GetOtherInfoHandle(ILogger logger
            , IEntityRepository<FBB_CUST_PROFILE> FBB_CUST_PROFILE
            , IEntityRepository<FBB_CUST_CONTACT> FBB_CUST_CONTACT
            )
        {
            _logger = logger;
            _FBB_CUST_PROFILE = FBB_CUST_PROFILE;
            _FBB_CUST_CONTACT = FBB_CUST_CONTACT;
        }

        public string Handle(GetOtherInfo query)
        {
            _logger.Info("Call evESeService SFF");

            if (query.selectvalue == "GENDER")
            {
                var result = (from r in _FBB_CUST_PROFILE.Get()
                              where r.CUST_NON_MOBILE == query.mobileno && r.CA_ID == query.caid && r.BA_ID == query.baid
                              select r.CUST_GENDER).FirstOrDefault();

                if (result == null)
                {
                    result = "";
                }
                return result;
            }
            else if (query.selectvalue == "CONTACT")
            {
                var result3 = "";
                var result2 = (from p in _FBB_CUST_PROFILE.Get()
                               join c in _FBB_CUST_CONTACT.Get() on p.BA_ID equals c.BA_ID
                               where c.BA_ID == query.baid
                               && c.CONTACT_SEQ == 1 && p.CUST_NON_MOBILE == query.mobileno
                               select new
                               {
                                   c.CONTACT_HOME_PHONE,
                                   //c.CONTACT_MOBILE_PHONE1,
                                   c.CONTACT_MOBILE_PHONE2

                               }).FirstOrDefault();

                if (result2 != null)
                {
                    //021358555 Ext.016
                    string strHomePhone = "";
                    string strExt = "";

                    if (!String.IsNullOrEmpty(result2.CONTACT_HOME_PHONE))
                    {
                        var arrHomePhone = result2.CONTACT_HOME_PHONE.Split(new char[0]);
                        if (arrHomePhone.Length == 2)
                        {
                            strHomePhone = arrHomePhone[0];
                            strExt = arrHomePhone[1].Replace("Ext.", "");
                        }
                        else
                        {
                            strHomePhone = result2.CONTACT_HOME_PHONE;
                        }
                    }
                    //result3 = strHomePhone + "," + strExt + "," + result2.CONTACT_MOBILE_PHONE1 + "," + result2.CONTACT_MOBILE_PHONE2;
                    result3 = strHomePhone + "," + strExt + "," + query.servicemobileno + "," + result2.CONTACT_MOBILE_PHONE2;

                    #region old condition of john

                    //if (!String.IsNullOrEmpty(result2.CONTACT_HOME_PHONE) && !String.IsNullOrEmpty(result2.CONTACT_MOBILE_PHONE1) && !String.IsNullOrEmpty(result2.CONTACT_MOBILE_PHONE2))
                    //{
                    //    result3 = result2.CONTACT_HOME_PHONE + ", " + result2.CONTACT_MOBILE_PHONE1 + ", " + result2.CONTACT_MOBILE_PHONE2;
                    //}
                    //else if (!String.IsNullOrEmpty(result2.CONTACT_HOME_PHONE) && !String.IsNullOrEmpty(result2.CONTACT_MOBILE_PHONE1) && String.IsNullOrEmpty(result2.CONTACT_MOBILE_PHONE2))
                    //{
                    //    result3 = result2.CONTACT_HOME_PHONE + ", " + result2.CONTACT_MOBILE_PHONE1;
                    //}
                    //else if (!String.IsNullOrEmpty(result2.CONTACT_HOME_PHONE) && String.IsNullOrEmpty(result2.CONTACT_MOBILE_PHONE1) && String.IsNullOrEmpty(result2.CONTACT_MOBILE_PHONE2))
                    //{
                    //    result3 = result2.CONTACT_HOME_PHONE;
                    //}
                    //else if (String.IsNullOrEmpty(result2.CONTACT_HOME_PHONE) && !String.IsNullOrEmpty(result2.CONTACT_MOBILE_PHONE1) && String.IsNullOrEmpty(result2.CONTACT_MOBILE_PHONE2))
                    //{
                    //    result3 = result2.CONTACT_MOBILE_PHONE1;
                    //}
                    //else if (String.IsNullOrEmpty(result2.CONTACT_HOME_PHONE) && !String.IsNullOrEmpty(result2.CONTACT_MOBILE_PHONE1) && !String.IsNullOrEmpty(result2.CONTACT_MOBILE_PHONE2))
                    //{
                    //    result3 = result2.CONTACT_MOBILE_PHONE1 + ", " + result2.CONTACT_MOBILE_PHONE2;
                    //}
                    //else if (String.IsNullOrEmpty(result2.CONTACT_HOME_PHONE) && String.IsNullOrEmpty(result2.CONTACT_MOBILE_PHONE1) && !String.IsNullOrEmpty(result2.CONTACT_MOBILE_PHONE2))
                    //{
                    //    result3 = result2.CONTACT_MOBILE_PHONE2;
                    //}
                    //else
                    //{
                    //    result3 = "";
                    //}
                    #endregion
                }
                else
                {
                    result3 = "" + "," + "" + "," + query.servicemobileno + "," + "";
                }

                return result3;
            }
            return "";
        }
    }
}

