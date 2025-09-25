namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    public class GetEmployeeProfileByPINModel
    {
        public GetEmployeeProfileByPINModel()
        {

            this.NewDataSet = new NewDataSet();

        }
        public NewDataSet NewDataSet { get; set; }
    }
    public class NewDataSet
    {
        public Permission Permission { get; set; }
        public Table Table { get; set; }
    }
    public class Permission
    {
        public string MsgDetail { get; set; }
        public string MsgStatus { get; set; }
    }
    public class Table
    {
        public string PIN { get; set; }
        public string USERID { get; set; }
        public string THFIRSTNAME { get; set; }
        public string THLASTNAME { get; set; }
        public string ENFIRSTNAME { get; set; }
        public string ENLASTNAME { get; set; }
        public string EMAIL { get; set; }
        public string EMPLOYEETYPE { get; set; }
        public string EMPLOYEEGROUP { get; set; }
        public string POSITIONCODE { get; set; }
        public string POSITIONDESC { get; set; }
        public string ORGID { get; set; }
        public string ORGNAME { get; set; }
        public string ORGDESC { get; set; }
        public string COMPANYCODE { get; set; }
        public string COMPANYNAME { get; set; }
        public string PGGROUP { get; set; }
        public string TELNO { get; set; }
        public string MOBILENO { get; set; }
        public string DPDESC { get; set; }
        public string BUNAME { get; set; }
        public string MANAGER { get; set; }
        public string IDCARD { get; set; }
        public string BIRTHDATE { get; set; }
        public string MANAGERPIN { get; set; }
    }
}
