using System;
using System.IO;
using System.Web;

namespace FBBConfig.Extensions
{
    public class Commone
    {
        public byte[] ExportExcelTemplate(string filename)
        {
            byte[] bytes = null;
            try
            {
                string ServerPath = HttpContext.Current.Server.MapPath("~/App_Data/ExcelTemplate/");
                string FullPath = Path.Combine(ServerPath, filename);
                //FileStream stream = new FileStream(FullPath, FileMode.Open);
                //bytes = new byte[stream.Length];
                //stream.Read(bytes, 0, bytes.Length);
                //stream.Close();
                bytes = File.ReadAllBytes(FullPath);
                return bytes;
            }
            catch (Exception ex)
            {
                return bytes;
            }


        }

    }
}