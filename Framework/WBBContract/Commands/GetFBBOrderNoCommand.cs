namespace WBBContract.Commands
{
    public class GetFBBOrderNoCommand
    {
        public GetFBBOrderNoCommand()
        {
            this.Return_Code = "-1";
            this.Return_Desc = "";
        }
        public string addressAmphur { get; set; }

        public string addressBuilding { get; set; }

        public string addressFloor { get; set; }

        public string addressMoo { get; set; }

        public string addressMooban { get; set; }

        public string addressNo { get; set; }

        public string addressPostCode { get; set; }

        public string addressProvince { get; set; }

        public string addressRoad { get; set; }

        public string addressSoi { get; set; }

        public string addressTumbol { get; set; }

        public string orderNo { get; set; }

        public string orderNoStatus { get; set; }

        public string caseID { get; set; }
        public string FullUrl { get; set; }


        public string Return_Code { get; set; }
        public string Return_Desc { get; set; }
    }
}
