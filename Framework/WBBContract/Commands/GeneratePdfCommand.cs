using WBBEntity.PanelModels;

namespace WBBContract.Commands
{
    public class GeneratePdfCommand
    {
        public string DirectoryPath { get; set; }
        public string DirectoryTempPath { get; set; }
        public string FontFolderPath { get; set; }
        public string ImageFolderPath { get; set; }

        public string FileName { get; set; }
        public int CurrentUICulture { get; set; }


        public QuickWinPanelModel Model { get; set; }

        //public AddressPanelModel SendDocAddress { get; set; }
        //public AddressPanelModel InstallAddress { get; set; }
        //public AddressPanelModel VatAddress { get; set; }

        // as a return value
        public string PdfPath
        {
            get
            {
                return this.DirectoryTempPath + "\\" + this.FileName + ".pdf";
            }
        }
    }
}
