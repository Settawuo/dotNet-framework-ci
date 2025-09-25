namespace WBBEntity.PanelModels.ExWebServiceModels
{
    public class PortValueModel
    {
        public PortValueModel()
        {
            this.PortId = -1;
            this.PortType = "";
            this.NetWorkTechnology = "";
            this.DataOnlyFlag = "";

            this.DSLAMNumber = -1;
            this.CardNumber = -1;
            this.PortNumber = -1;
        }

        public decimal PortId { get; set; }

        public string PortDescription
        {
            get
            {
                return string.Format("{0}/{1}/{2}", this.DSLAMNumber, this.CardNumber, this.PortNumber);
            }
        }

        public string PortType { get; set; }

        public string NetWorkTechnology { get; set; }

        public string DataOnlyFlag { get; set; }

        public decimal DSLAMNumber { get; set; }

        public decimal CardNumber { get; set; }

        public decimal PortNumber { get; set; }

        public string NODEId { get; set; }

        public decimal MaxPort { get; set; }
    }
}
