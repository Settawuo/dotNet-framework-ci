using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WBBBusinessLayer.SBNNewWebService;
using WBBBusinessLayer.SBNWebService;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.Extension
{
    public static class FbbCpGwCustRegisterHelper
    {
        public static AirRegistPackageRecord[] CreateAirnetPackageRecord(
            List<PackageModel> packageList)
        {
            AirRegistPackageRecord[] airregists = packageList.Where(o => o != null).Select(o => new AirRegistPackageRecord()
            {
                FaxFlag = "",
                TempIa = o.MAPPING_PRODUCT.ToSafeString(),
                HomeIp = "",
                HomePort = "",
                IddFlag = "",
                PackageCode = o.PACKAGE_CODE.ToSafeString(),
                PackageType = o.PACKAGE_TYPE.ToSafeString(),
                ProductSubtype = o.PRODUCT_SUBTYPE.ToSafeString(),
            }).ToArray();

            return airregists;
        }

        public static airRegistPackageRecord[] CreateNewAirnetPackageRecord(
            List<PackageModel> packageList)
        {
            airRegistPackageRecord[] airregists = packageList.Where(o => o != null).Select(o => new airRegistPackageRecord()
            {
                faxFlag = "",
                tempIa = o.MAPPING_PRODUCT.ToSafeString(),
                homeIp = "",
                homePort = "",
                iddFlag = "",
                packageCode = o.PACKAGE_CODE.ToSafeString(),
                packageType = o.PACKAGE_TYPE.ToSafeString(),
                productSubtype = o.PRODUCT_SUBTYPE.ToSafeString(),
            }).ToArray();

            return airregists;
        }

        //public static airRegistPackageRecord[] CreateNewAirnetPackageRecord(
        //   List<PackageModel> packageList)
        //{
        //    airRegistPackageRecord[] airregists = packageList.Where(o => o != null).Select(o => new airRegistPackageRecord()
        //    {
        //        faxFlag = "",
        //        tempIa = o.MAPPING_PRODUCT.ToSafeString(),
        //        homeIp = "",
        //        homePort = "",
        //        iddFlag = "",
        //        packageCode = o.PACKAGE_CODE.ToSafeString(),
        //        packageType = o.PACKAGE_TYPE.ToSafeString(),
        //        productSubtype = o.PRODUCT_SUBTYPE.ToSafeString(),
        //    }).ToArray();

        //    return airregists;
        //}

        public static string ToDateString(bool isThai, string rawTextDate)
        {
            var dateString = string.Empty;
            DateTime parsedDate;
            var date = DateTime.TryParseExact(rawTextDate.ToSafeString(),
                                                "dd/MM/yyyy",
                                                CultureInfo.InvariantCulture,
                                                DateTimeStyles.None, out parsedDate);

            if (isThai)
            {
                if (parsedDate > DateTime.MinValue)
                    dateString = parsedDate.AddYears(-543).ToDateDisplayText();
            }
            else
            {
                if (parsedDate > DateTime.MinValue)
                    dateString = parsedDate.ToDateDisplayText();
            }

            return dateString;
        }

        public static string FindZipCodeRowID(
            IEntityRepository<FBB_ZIPCODE> zipCode, string province, string district, string subDistrict)
        {
            var zipcodeRowId = (from z in zipCode.Get()
                                where z.PROVINCE == province
                                && z.AMPHUR == district
                                && z.TUMBON == subDistrict
                                select z.ZIPCODE_ROWID).FirstOrDefault().ToSafeString();

            return zipcodeRowId;
        }

        public static List<string> FindLatLng(
            IEntityRepository<FBB_COVERAGEAREA_RESULT> coverageAreaRes,
            string transactionId)
        {
            var latlngRow = (from t in coverageAreaRes.Get()
                             where t.TRANSACTION_ID == transactionId
                             select new { t.LATITUDE, t.LONGITUDE }).FirstOrDefault();

            var latlngList = new List<string>();

            if (null != latlngRow)
            {
                latlngList.Add(latlngRow.LATITUDE);
                latlngList.Add(latlngRow.LONGITUDE);
            }

            return latlngList;

        }

        public static string HaveWifiAccessPoint(
            IEntityRepository<FBB_CFG_LOV> lov,
            List<PackageModel> packageList)
        {
            foreach (var pack in packageList)
            {
                if (null == pack)
                    continue;

                if (pack.PRODUCT_SUBTYPE == "FTTx" || pack.PRODUCT_SUBTYPE == "WireBB")
                {
                    return "Y";
                }
                else
                {
                    var wifiApPackCode = (from t in lov.Get()
                                          where (t.LOV_NAME == "PRO_INSTALL_ROUTER") || (t.LOV_NAME == "HOME_INSTALL_ROUTER")
                                          select t.LOV_VAL1).FirstOrDefault();

                    if (pack.PACKAGE_CODE == wifiApPackCode)
                    {
                        return "Y";
                    }
                }
            }

            return "N";
        }

        public static string FindAddressTypeWire(
            IEntityRepository<FBB_COVERAGEAREA_RESULT> coverageAreaRes,
            string transactionId)
        {
            var coverageType = (from t in coverageAreaRes.Get()
                                where t.TRANSACTION_ID == transactionId
                                select t.COVERAGETYPE).FirstOrDefault();

            if (coverageType == "CONDOMINIUM")
            {
                return "อาคาร";
            }

            return "หมู่บ้าน";
        }

        public static List<string> FindCvrId(
            IEntityRepository<FBB_COVERAGEAREA_RESULT> coverageAreaRes,
            string transactionId)
        {
            var coverageAreaRow = (from t in coverageAreaRes.Get()
                                   where t.TRANSACTION_ID == transactionId
                                   select new { t.CVRID, t.RESULTID }).FirstOrDefault();

            var coverageAreaList = new List<string>();

            if (null != coverageAreaRow)
            {
                coverageAreaList.Add(coverageAreaRow.CVRID.ToSafeString());
                coverageAreaList.Add(coverageAreaRow.RESULTID.ToSafeString());
            }

            return coverageAreaList;
        }

        public static bool IsNonMobile(string mobileNum)
        {
            if (string.IsNullOrEmpty(mobileNum))
            {
                return false;
            }

            var firstChar = mobileNum.ToArray()[0];
            if (firstChar == '0')
            {
                return false;
            }

            return true;
        }

        public static List<string> FindSffComrades(
            IEntityRepository<FBB_CFG_LOV> lov,
            IEntityRepository<FBB_SFF_CHKPROFILE_LOG> sffChkProfLogs,
            string mobileNo, string idCardNo, string transactionId)
        {
            var sffComradeList = new List<string>();
            sffComradeList.Add("");
            sffComradeList.Add("");
            sffComradeList.Add("");
            sffComradeList.Add("");
            sffComradeList.Add("");

            if (!string.IsNullOrEmpty(mobileNo)
                && mobileNo.Length > 2)
            {
                var m1 = mobileNo.ToArray()[0];
                var m2 = mobileNo.ToArray()[1];

                var sffChkProfLog = (from t in sffChkProfLogs.Get()
                                     where t.INMOBILENO == mobileNo
                                         && t.INIDCARDNO == idCardNo
                                         && t.TRANSACTION_ID == transactionId
                                     select t).FirstOrDefault();

                if (null != sffChkProfLog)
                {
                    var logProdNames = sffChkProfLog.OUTPRODUCTNAME.Split('/');
                    var sffProdName = "";
                    if (logProdNames.Any())
                    {
                        // sff product name จะอยู่ที่ตัวแรก
                        sffProdName = logProdNames.FirstOrDefault();
                    }
                    else
                    {
                        sffProdName = sffChkProfLog.OUTPRODUCTNAME;
                    }

                    sffComradeList[3] = sffProdName;
                    sffComradeList[4] = sffChkProfLog.OUTSERVICEYEAR;

                    if ((from i_l in lov.Get()
                         where i_l.LOV_TYPE == "AWN_PRODUCT"
                             && i_l.LOV_NAME == sffProdName
                         select i_l).Any())
                    {
                        sffComradeList[0] = sffChkProfLog.OUTACCOUNTNUMBER;
                        sffComradeList[1] = sffChkProfLog.OUTSERVICEACCOUNTNUMBER;
                        sffComradeList[2] = sffChkProfLog.OUTBILLINGACCOUNTNUMBER;
                    }
                    else if (m1 == '8' && m2 == '9')
                    {
                        sffComradeList[0] = sffChkProfLog.OUTACCOUNTNUMBER;
                        sffComradeList[1] = "";
                        sffComradeList[2] = "";
                    }
                    else
                    {
                        sffComradeList[0] = "";
                        sffComradeList[1] = "";
                        sffComradeList[2] = "";
                    }
                }
            }

            return sffComradeList;
        }

        public static string FindVendor(
            IEntityRepository<FBB_COVERAGEAREA_RESULT> coverageAreaRes,
            List<PackageModel> packageList, string transactionId, string partnerName = "")
        {
            foreach (var pack in packageList)
            {
                if (pack.PRODUCT_SUBTYPE == "FTTx"
                    && pack.PACKAGE_TYPE == "Main")
                {
                    var ownerProduct = (from t in coverageAreaRes.Get()
                                        where t.TRANSACTION_ID == transactionId
                                        select t.OWNER_PRODUCT).FirstOrDefault();

                    var splitOwnerProduct = ownerProduct.Split('|');

                    foreach (var op in splitOwnerProduct)
                    {
                        if (op == "NSN" || op == "SIMAT" || op == "SYMPHONY")
                        {
                            return op;
                        }
                    }
                }
            }

            return "";
        }

        public static string FindVendorByPartner(IEntityRepository<FBB_CFG_LOV> lov, string partnerName)
        {
            var partners = (from t in lov.Get()
                            where t.LOV_NAME == "MAPPING_OWNER_PRODUCT"
                                && t.LOV_VAL1 == "FTTH"
                            select t.LOV_VAL3)
                            .ToList();

            return partners.Select(t => partnerName.Contains(t) ? t : "").FirstOrDefault();
        }

        public static string FindInstallNote(
            IEntityRepository<FBB_COVERAGEAREA> coverageArea,
            IEntityRepository<FBB_COVERAGEAREA_BUILDING> coverageBuilding,
            List<PackageModel> packageList, bool isThai, string cvrid, string tower)
        {
            foreach (var pack in packageList)
            {
                if (pack.OWNER_PRODUCT == "WireBB")
                {
                    var cvridm = cvrid.ToSafeDecimal();
                    if (isThai)
                    {
                        return (from a in coverageArea.Get()
                                join b in coverageBuilding.Get() on a.CONTACT_ID equals b.CONTACT_ID
                                where a.ACTIVEFLAG == "Y"
                                && b.ACTIVE_FLAG == "Y"
                                && a.CVRID == cvridm
                                && b.BUILDING_TH == tower
                                select b.INSTALL_NOTE).FirstOrDefault();
                    }
                    else
                    {
                        return (from a in coverageArea.Get()
                                join b in coverageBuilding.Get() on a.CONTACT_ID equals b.CONTACT_ID
                                where a.ACTIVEFLAG == "Y"
                                && b.ACTIVE_FLAG == "Y"
                                && a.CVRID == cvridm
                                && b.BUILDING_EN == tower
                                select b.INSTALL_NOTE).FirstOrDefault();
                    }
                }
            }

            return "";
        }

        public static string ToOrderNo(this string orderNo)
        {
            try
            {
                return orderNo.Substring(7);
            }
            catch (Exception)
            {
                return "";
            }
        }
    }

}
