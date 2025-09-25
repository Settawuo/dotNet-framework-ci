using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WBBContract.QueryModels.WebServices
{
    public class GetUserProfileQueryModel
    {
        public string status { get; set; }
        public string code { get; set; }
        public string message { get; set; }
        public List<QueryUserProfileResponseList> aisEmployeeHierarchy { get; set; }
    }
    public class GetAccessTokenUserProfileQueryModel
    {
        public string status { get; set; }
        public string code { get; set; }
        public string message { get; set; }
        public string access_token { get; set; }
    }


    public class Userprofile
    {
        public string Username { get; set; }
    }



    public class GetProfileQueryRequest
    {
        public List<Userprofile> userprofile { get; set; }
    }

    public class QueryUserProfileResponseList
    {
        public string subProfile { get; set; }
        public string userName { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string mobileNo { get; set; }
        public string period { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string Status { get; set; }
        public string pinCode { get; set; } 
        public string ascCode { get; set; }
       
        public string locationCode { get; set; }
        public string chnSaleCode { get; set; }
        public List<Listposition> Listposition { get; set; }
        public string locationNameTH { get; set; }
        public string locationNameEN { get; set; }
    }

    public class Listposition
    {
        public string employeeDivisionType { get; set; }
        public string positionName { get; set; }
        public string positionType { get; set; }
    }

}
