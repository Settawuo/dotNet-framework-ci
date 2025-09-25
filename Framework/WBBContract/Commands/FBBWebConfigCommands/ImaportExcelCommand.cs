using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class ImaportExcelCommand
    {
        public List<AWCexportlist> Imex;
        public string filename { get; set; }
        public string user { get; set; }
        //public List<Import_Excel_AWC> Imex
        //{
        //    get { return _Imex; }
        //    set { _Imex = value; }
        //}

        // for return
        public int Return_Code { get; set; }
        public string Return_Desc { get; set; }

        ////public string Cust_Non_Mobile { get; set; }
        ////public string Ca_Id { get; set; }
        ////public string Sa_Id { get; set; }
        ////public string Ba_Id { get; set; }
        ////public string Ia_Id { get; set; }
        ////public string Cust_Name { get; set; }
        ////public string Cust_Surname { get; set; }
        ////public string Cust_Id_Card_Type { get; set; }
        ////public string Cust_Id_Card_Num { get; set; }
        ////public string Cust_Category { get; set; }
        ////public string Cust_Sub_Category { get; set; }
        ////public string Cust_Gender { get; set; }
        ////public string Cust_Birthday { get; set; }
        ////public string Cust_Nationality { get; set; }
        ////public string Cust_Title { get; set; }
        ////public string Online_Number { get; set; }
        ////public string Condo_Type { get; set; }
        ////public string Condo_Direction { get; set; }
        ////public string Condo_Limit { get; set; }
        ////public string Condo_Area { get; set; }
        ////public string Home_Type { get; set; }
        ////public string Home_Area { get; set; }
        ////public string Document_Type { get; set; }
        ////public string Cvr_Id { get; set; }
        ////public string Port_Id { get; set; }
        ////public string Order_No { get; set; }
        ////public string Remark { get; set; }

        //public class Import_Excel_AWC
        //{
        //    public string Address_In_Seq { get; set; }
        //    public string Contact_First_Name { get; set; }
        //    public string Contact_Last_Name { get; set; }
        //    public string Contact_Home_Phone { get; set; }
        //    public string Contact_Mobile_Phone1 { get; set; }
        //    public string Contact_Mobile_Phone2 { get; set; }
        //    public string Contact_Email { get; set; }
        //}
    }
}
