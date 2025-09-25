using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WBBWeb.Controllers
{
    public class YController : Controller
    {
        //
        // GET: /Y/

        public ActionResult Index()
        {
            FBBProfileCustomer11.FBBProfileCustomer service = new FBBProfileCustomer11.FBBProfileCustomer();

            FBBProfileCustomer11.ProfileCustomerCommand profileCustomerCommand = new FBBProfileCustomer11.ProfileCustomerCommand();
            #region profileCustomerCommand data
            profileCustomerCommand.Cust_Non_Mobile = "Cust_Non_Mobile";
            profileCustomerCommand.Ca_Id = "Ca_Id";
            profileCustomerCommand.sa_Id = "sa_Id";
            profileCustomerCommand.ba_Id = "ba_Id";
            profileCustomerCommand.Ia_Id = "Ia_Id";
            profileCustomerCommand.Cust_Name = "Cust_Name";
            profileCustomerCommand.Cust_Surname = "Cust_Surname";
            profileCustomerCommand.Cust_Id_Card_Type = "Cust_Id_Card_Type";
            profileCustomerCommand.Cust_Id_Card_Num = "Cust_Id_Card_Num";
            profileCustomerCommand.Cust_Category = "Cust_Category";
            profileCustomerCommand.Cust_Sub_Category = "Cust_Sub_Category";
            profileCustomerCommand.Cust_Gender = "Cust_Gender";
            profileCustomerCommand.Cust_Birthday = "Cust_Birthday";
            profileCustomerCommand.Cust_Nationality = "Cust_Nationality";
            profileCustomerCommand.Cust_Title = "Cust_Title";
            profileCustomerCommand.Online_Number = "Online_Number";
            profileCustomerCommand.Condo_Type = "Condo_Type";
            profileCustomerCommand.Condo_Direction = "Condo_Direction";
            profileCustomerCommand.Condo_Limit = "Condo_Limit";
            profileCustomerCommand.Condo_Area = "Condo_Area";
            profileCustomerCommand.Home_Type = "Home_Type";
            profileCustomerCommand.Home_Area = "Home_Area";
            profileCustomerCommand.Document_Type = "Document_Type";
            profileCustomerCommand.Cvr_Id = "Cvr_Id";
            profileCustomerCommand.Port_Id = "Port_Id";
            profileCustomerCommand.Order_No = "Order_No";
            profileCustomerCommand.Remark = "Remark";
            #endregion

            FBBProfileCustomer11.REC_CUST_CONTACT[] rec_Cust_Contact = new FBBProfileCustomer11.REC_CUST_CONTACT[1];
            #region rec_Cust_Contact data
            rec_Cust_Contact[0] = new FBBProfileCustomer11.REC_CUST_CONTACT();
            rec_Cust_Contact[0].Contact_First_Name = "Contact_First_Name";
            rec_Cust_Contact[0].Contact_Last_Name = "Contact_Last_Name";
            rec_Cust_Contact[0].Contact_Home_Phone = "Contact_Home_Phone";
            rec_Cust_Contact[0].Contact_Mobile_Phone1 = "Contact_Mobile_Phone1";
            rec_Cust_Contact[0].Contact_Mobile_Phone2 = "Contact_Mobile_Phone2";
            rec_Cust_Contact[0].Contact_Email = "Contact_Email"; 
            #endregion

            FBBProfileCustomer11.REC_CUST_ADDRESS[] rec_Cust_Address = new FBBProfileCustomer11.REC_CUST_ADDRESS[1];
            #region rec_Cust_Address data
            rec_Cust_Address[0] = new FBBProfileCustomer11.REC_CUST_ADDRESS();
            rec_Cust_Address[0].Address_Type = "Address_Type";
            rec_Cust_Address[0].House_No = "House_No";
            rec_Cust_Address[0].Soi = "Soi";
            rec_Cust_Address[0].Moo = "Moo";
            rec_Cust_Address[0].Mooban = "Mooban";
            rec_Cust_Address[0].Building_Name = "Building_Name";
            rec_Cust_Address[0].Floor = "Floor";
            rec_Cust_Address[0].Room = "Room";
            rec_Cust_Address[0].Street_Name = "Street_Name";
            rec_Cust_Address[0].Install_Zipcode_Id = "Install_Zipcode_Id";
            rec_Cust_Address[0].Address_Vat = "Address_Vat";
            #endregion

            FBBProfileCustomer11.REC_CUST_ASSET[] rec_Cust_Asset = new FBBProfileCustomer11.REC_CUST_ASSET[1];
            #region rec_Cust_Asset data
            rec_Cust_Asset[0] = new FBBProfileCustomer11.REC_CUST_ASSET();
            rec_Cust_Asset[0].Asset_Code = "Asset_Code";
            rec_Cust_Asset[0].Package_Code = "Package_Code";
            rec_Cust_Asset[0].Equipment_Type = "Equipment_Type";
            rec_Cust_Asset[0].Asset_Status = "Asset_Status";
            rec_Cust_Asset[0].Asset_Name = "Asset_Name";
            rec_Cust_Asset[0].Asset_Charge = "Asset_Charge";
            rec_Cust_Asset[0].Asset_Discount = "Asset_Discount";
            rec_Cust_Asset[0].Asset_Start_Dt = "Asset_Start_Dt";
            rec_Cust_Asset[0].Asset_End_Dt = "Asset_End_Dt";
            rec_Cust_Asset[0].Serial_Number = "Serial_Number";
            rec_Cust_Asset[0].Asset_Model = "Asset_Model";  
            #endregion

            FBBProfileCustomer11.REC_CUST_PACKAGE[] rec_Cust_Package = new FBBProfileCustomer11.REC_CUST_PACKAGE[1];
            #region rec_Cust_Package data
            rec_Cust_Package[0] = new FBBProfileCustomer11.REC_CUST_PACKAGE();
            rec_Cust_Package[0].Package_Code = "Package_Code";
            rec_Cust_Package[0].Package_Class = "Package_Class";
            rec_Cust_Package[0].Package_Type = "Package_Type";
            rec_Cust_Package[0].Package_Group = "Package_Group";
            rec_Cust_Package[0].Package_Subtype = "Package_Subtype";
            rec_Cust_Package[0].Package_Owner = "Package_Owner";
            rec_Cust_Package[0].Technology = "Technology";
            rec_Cust_Package[0].package_Status = "package_Status";
            rec_Cust_Package[0].package_Name = "package_Name";
            rec_Cust_Package[0].Recurring_Charge = "Recurring_Charge";
            rec_Cust_Package[0].Recurring_Discount = "Recurring_Discount";
            rec_Cust_Package[0].Recurring_Disc_Exp = "Recurring_Disc_Exp";
            rec_Cust_Package[0].Recurring_Start_Dt = "Recurring_Start_Dt";
            rec_Cust_Package[0].Recurring_End_Dt = "Recurring_End_Dt";
            rec_Cust_Package[0].Initiation_Charge = "Initiation_Charge";
            rec_Cust_Package[0].Initiation_Discount = "Initiation_Discount";
            rec_Cust_Package[0].Package_Bill_Tha = "Package_Bill_Tha";
            rec_Cust_Package[0].Download_Speed = "Download_Speed";
            rec_Cust_Package[0].Upload_Speed = "Upload_Speed"; 
            #endregion

            profileCustomerCommand.Rec_Cust_Address = rec_Cust_Address;
            profileCustomerCommand.Rec_Cust_Contact = rec_Cust_Contact;
            profileCustomerCommand.Rec_Cust_Package = rec_Cust_Package;
            profileCustomerCommand.Rec_Cust_Asset = rec_Cust_Asset;

            var result = service.SaveProfileCustomer(profileCustomerCommand);

            return View();
        }

    }
}
