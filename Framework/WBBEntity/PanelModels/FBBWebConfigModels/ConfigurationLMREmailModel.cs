using System;

namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    public class ConfigurationLMREmailModel
    {
        public string Text { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public decimal Id { get; set; }
        public decimal ParId { get; set; }
        public string LovValue1 { get; set; }
        public string LovValue2 { get; set; }
        public string LovValue3 { get; set; }
        public string LovValue4 { get; set; }
        public string LovValue5 { get; set; }
        public byte[] Image_blob { get; set; }

        public decimal? OrderBy { get; set; }
        public string DefaultValue { get; set; }
        public string Update_by { get; set; }
        public DateTime? Update_date { get; set; }
        public string Update_date_text { get; set; }
    }
}
