namespace WBBContract.Commands
{
    public class GetLeaveMsgReferenceNoCommand
    {
        public GetLeaveMsgReferenceNoCommand()
        {
            this.Return_Code = "-1";
            this.Return_Desc = "";
        }
        public string contactMobileNo { get; set; }

        public string customerLastName { get; set; }

        public string customerName { get; set; }

        public string referenceNo { get; set; }

        public string referenceNoStatus { get; set; }

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

        public string inService { get; set; }

        public string caseID { get; set; }
        public string assetNumber { get; set; }

        public string productType { get; set; }

        public string FullUrl { get; set; }

        public decimal RESULTID { get; set; }

        public string Return_Code { get; set; }
        public string Return_Desc { get; set; }
    }
}
