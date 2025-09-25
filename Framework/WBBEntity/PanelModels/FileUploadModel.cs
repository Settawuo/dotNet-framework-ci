using System;

namespace WBBEntity.PanelModels
{
    public class FileUploadModel
    {
        public enum ReadFrom
        {
            DB,
            SESSION,
            TEMP
        }

        public ReadFrom ReadDataFrom { get; set; }
        public decimal ID { get; set; }
        public int NO { get; set; }

        public string FileName { get; set; }
        public string RealFileName { get; set; }
        public string FilePath { get; set; }
        public string Type { get; set; }
        public string AttachedBy { get; set; }
        public string AttachedByText { get; set; }
        public DateTime AttachedDate { get; set; }
    }
}
