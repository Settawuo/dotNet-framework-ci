
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WBBBusinessLayer;
using WBBContract.Queries.SftpQueries;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBPAYG_LoadFile_3BBReport
{
    public class SftpService
    {

        public ILogger _logger;
        public SftpService(ILogger logger)
        {
            this._logger = logger;
        }
        public string Host { get; set; }
        public int Port { get; set; }
  
        /// DownloadFile solution
        public List<string> DownloadFile(string display_val, string destination_path, SftpClient client, DateTime startDate_, DateTime endDate_, out string msg)
        {
            List<string> resp = new List<string>();
            msg = "Success";
            string returnStatusfile = "";
            string downloadSuccess = "";
            string downloadFailed = "";
            try
            {
                _logger.Info((object)(display_val + " Start Date : " + startDate_.ToString("dd-MM-yyyy")));
                _logger.Info((object)(display_val + " End Date : " + endDate_.ToString("dd-MM-yyyy")));

                var files = client.ListDirectory(destination_path);
                    foreach (var file in files)
                    {
                        string pathNewDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Download/" + display_val);
                        if (!Directory.Exists(pathNewDirectory))
                        {
                            DirectoryInfo di = Directory.CreateDirectory(pathNewDirectory);
                        }

                        if (!file.IsDirectory && !file.IsSymbolicLink)//symbolic link ไม่ใช่ไฟล์ //IsDirectory เป็นแค่ Path ต้องมีชื่อไฟล์ ถ้ามีแค่ path ข้าม
                        {
                            DateTime lastModified = file.LastWriteTime;
                        DateTime fileDate = lastModified.Date;
                        DateTime checkStartDate = startDate_.Date;
                        DateTime checkEndDate = endDate_.Date;

                      
                        if (fileDate >= checkStartDate && fileDate <= checkEndDate)
                            {
                                this._logger.Info((object)("Process Download Start"));
                                string filePath = Path.Combine(pathNewDirectory, file.Name);
                                string sanitizedFileName = filePath.Replace("\\", "/");
                                try
                                {
                                    using (Stream fileStream = File.OpenWrite(sanitizedFileName))
                                    {
                                        client.DownloadFile(file.FullName, fileStream);
                                        downloadSuccess = display_val + " : " + "File Download" + " : " + file.Name +" : SUCCESS";
                                        this._logger.Info(downloadSuccess);
                                        resp.Add(downloadSuccess);    
                                }
                            }
                                catch (Exception ex)
                                {
                                    this._logger.Info((object)($"Download failed for file: {file.Name}. Error: {ex.Message}"));
                                    downloadFailed = display_val + " : " + file.Name + ": FAILED";
                                    resp.Add(downloadFailed);
                                    this._logger.Info((object)(downloadFailed));
                                }
                            }
                        }
                    }
                
                return resp;
            }
            catch (Exception exception)
            {
                this._logger.Info((object)("Download Process failed " + exception.ToString()));
                msg = "Error occurred while downloading files.";
                resp.Add(exception.ToString());
                return resp;
            }
        }



    }
}
