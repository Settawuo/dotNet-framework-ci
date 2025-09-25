using One2NetBusinessLayer;
using One2NetContract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Data;
using System.IO;
using System.Drawing;
using OfficeOpenXml;    
using OfficeOpenXml.Drawing;    
using OfficeOpenXml.Style;
using WBBEntity.Extensions;

namespace FBBDormitoryBatchForService
{
    using One2NetBusinessLayer.QueryHandlers.Common;
    using One2NetContract.Queries.InWebServices;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.ComponentModel;
    using System.Reflection;
    using WBBEntity.PanelModels.One2NetModels.InWebServices;
    using OfficeOpenXml.Table;
    using One2NetContract.Commands.InWebServices;
    using WBBEntity.PanelModels;
    using One2NetContract.Queries.Common;
    using System.Text.RegularExpressions;
    using System.Net.Mail;
    using WBBEntity.Models;
    using WBBData.Repository;
    using FBBDormitoryBatchForService.CompositionRoot;

    public class FBBDormitoryBatchForServiceJob 
    {
        private Stopwatch _timer;
        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private ICommandHandler<SendMailNotificationCommand> _sendMail;
        private readonly ICommandHandler<SaveResponseBatchCommand> _saveResponseBatchCommand;
     
        private string errorMsg = string.Empty;
        private string _attachFile;
        public string[] _attachFiles;
        public string[] _subjectFiles;

        private void StartWatching()
        {
            _timer = Stopwatch.StartNew();
        }

        private void StopWatching(string mode)
        {
            _timer.Stop();
            _logger.Info(string.Format("{0} take : {1}", mode, _timer.Elapsed));
        }

        public FBBDormitoryBatchForServiceJob(
            ILogger logger,
            IQueryProcessor queryProcessor,
            ICommandHandler<SendMailNotificationCommand> sendMail,
            ICommandHandler<SaveResponseBatchCommand> saveResponseBatchCommand
           )
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
            _sendMail = sendMail;
            _saveResponseBatchCommand = saveResponseBatchCommand;

        }

        public void Execute()
        {
            QueryDataToSendMail();
        }

        public void QueryDataToSendMail()
        {
            try
            {
                string strUser = "";
                string strFileName = "";
                string strPathNas = "";
                string FromPass = "";// Fixed Code scan : string FromPassword = "";
                string Port = "";
                string Domaim = "";
                string IPMailServer = "";

                var vUser = GetLovList("FBBDORM_CONTANT","FBBDORM_USER_FOR_BATCH");
                if( vUser!=null)
                {
                    if (vUser.Count > 0)
                    {
                        strUser = vUser[0].LovValue1;
                    }
                }

                var vFileName = GetLovList("FBBDORM_CONTANT", "FBBDORM_HEAD_NAME_EXCEL");
                if (vFileName != null)
                {
                    if (vFileName.Count > 0)
                    {
                        strFileName = vFileName[0].LovValue1;
                    }
                }

                //path
                var vPath = GetLovList("FBBDORM_CONTANT", "FBBDORM_PATH_BATCH");
                if (vPath != null)
                {
                    if (vPath.Count > 0)
                    {
                        strPathNas = vPath[0].LovValue1;
                    }
                }

                // Mail server
                var vMailServer = GetLovList("FBBDORM_CONSTANT", "").Where(p => p.Name.Contains("VAR") && p.LovValue5 == "FBBDORM010").ToList();
                if (vMailServer != null)
                {
                    if (vMailServer.Count > 0)
                    {
                        foreach (var key in vMailServer)
                        {
                            if (key.Name.Trim() == "VAR_FROM_PASSWORD")
                            {
                                FromPassword = key.LovValue1;
                            }
                            else if (key.Name.Trim() == "VAR_HOST")
                            {
                                IPMailServer = key.LovValue1;
                            }
                            else if (key.Name.Trim() == "VAR_PORT")
                            {
                                Port = key.LovValue1;
                            }
                            else if (key.Name.Trim() == "VAR_DOMAIN")
                            {
                                Domaim = key.LovValue1;
                            }

                        }
                       
                    }
                }

                var query = new GetBatchNoRouterQuery()
                {
                    P_SYSDATE = DateTime.Now.ToString("yyyyMMdd"),
                    P_USER = strUser
                };

                var result = _queryProcessor.Execute(query);

                if (result != null)
                {
                    if (result.P_RETURN_CODE == "0")
                    {
                        // string tempPath = System.IO.Path.GetTempPath();
                        DataTable dtDetail = new DataTable();
                        System.Data.DataColumn newColumn = new System.Data.DataColumn("No", typeof(System.Int32));

                        DataTable dtSubj = ToDataTable(result.P_RES_DATA1);
                        dtDetail = ToDataTable(result.P_RES_DATA2);
                        DataTable dtCopy = new DataTable();

                        dtDetail.Columns.Add(newColumn);

                        String[] columnNames = new string[] { "No", "P_FIRST_NAME", "P_LAST_NAME", "P_CONTACT_PHONE", "P_ID_CARD_TYPE", "P_ID_CARD_NO", "P_DORMITORY_NAME", "P_BUILDING_NO", "P_FLOOR_NO", "P_ROOM_NO", "P_FIBRE_ID", "P_REGION" };
                        List<string> listColNames = columnNames.ToList();

                        //Remove invalid column names.
                        foreach (string colName in columnNames)
                        {
                            if (!dtDetail.Columns.Contains(colName))
                            {
                                listColNames.Remove(colName);
                            }
                        }

                        foreach (string colName in listColNames)
                        {
                            dtDetail.Columns[colName].SetOrdinal(listColNames.IndexOf(colName));
                        }

                        dtCopy = dtDetail.Clone();

                         var grouped = from table in dtDetail.AsEnumerable()
                                       group table by new { placeCol = table["P_REGION"] } into groupby
                                       select new
                                       {
                                           Value = groupby.Key,
                                           ColumnValues = groupby

                                       };

                         Int32 iRow = 0;
                         foreach (var key in grouped)
                         {
                            // string ipServer = "";

                             string strFrom = "";
                             string strTo = "";
                             string strCC = "";
                             string strBCC= "";
                             string strSubject = "";
                             string strBody = "";
                              string strRegionSuj = "";

                             string strRegion = key.Value.placeCol.ToString();
                             DataRow[] rowSubj = dtSubj.Select("P_REGION ='" + strRegion + "'");
                             foreach (DataRow rowS in rowSubj)
                             {
                               //  ipServer = rowS["P_IP_EMAIL_SERVER"].ToString();
                                 strFrom = rowS["P_SEND_FROM"].ToString();
                                 strTo = rowS["P_SEND_TO"].ToString();
                                 strCC = rowS["P_SEND_CC"].ToString();
                                 strBCC = rowS["P_SEND_BCC"].ToString();
                                 strRegionSuj = rowS["P_REGION"].ToString();
                                 strSubject = rowS["P_SUBJECT_NAME"].ToString();
                                 strBody = rowS["P_BODY_DETAIL"].ToString();

                                 break;
                             }

                           
                             dtCopy.Clear();
                             DataRow[] rowData = dtDetail.Select("P_REGION ='" + strRegion + "'");

                             Int32 iRowNum = 0;
                             foreach (DataRow row in rowData)
                             {
                                 iRowNum += 1;
                                 row["No"] = iRowNum;
                                 dtCopy.ImportRow(row);
                             }

                             dtCopy.AcceptChanges();

                             var regex = new Regex(Regex.Escape("{1}"));
                             var newRegion = regex.Replace(strFileName, strRegion, 1);

                             var regex2 = new Regex(Regex.Escape("{2}"));
                             var newfileName = regex2.Replace(newRegion, DateTime.Now.ToString("ddMMyyyy"), 1);

                             string finalFileNameWithPath = string.Empty;

                             finalFileNameWithPath = string.Format("{0}\\{1}.xlsx", strPathNas, newfileName);

                             GenerateExcel(finalFileNameWithPath, dtCopy);

                             string[] attach = new string[iRow+1];
                             attach[iRow] = finalFileNameWithPath;

                             string[] sendResult = Sendmail("DormBatch", "BATCH", strFrom, strTo, strCC,
                                 strBCC, strSubject, strBody, attach, IPMailServer, FromPassword, Port, Domaim);

                             if (sendResult != null)
                             {
                                 SetResponseBatch(strUser, sendResult[0], sendResult[1]);
                             }
  
                            // iRow += 1;
                         }

                        

                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Info(" Error QueryDataToSendMail :" + ex.GetErrorMessage());
                      
            }
        }


        public void SetResponseBatch(string user,string respone_code,string response_msg)
        {
            try
            {
                var savecommand = new SaveResponseBatchCommand
                {
                    P_USER = user,
                    P_REPONSE_CODE = respone_code,
                    P_REPONSE_MESSAGE = response_msg
                };

                _saveResponseBatchCommand.Handle(savecommand);

            }catch(Exception ex)
            {
                _logger.Info("Error SetResponseBatch :" + ex.GetErrorMessage());
            }
        }

        public List<LovValueModel> GetLovList(string type, string name = "")
        {
            try
            {
                var query = new GetLovQuery
                {
                    LovType = type,
                    LovName = name
                };

                var lov = _queryProcessor.Execute(query);
                return lov;
            }
            catch (Exception ex)
            {
                _logger.Info("Error GetLovList : " +ex.GetErrorMessage());
                return new List<LovValueModel>();
            }
        }

        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }

            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }

            //put a breakpoint here and check datatable
            return dataTable;
        }


        public void GenerateExcel(string p_strPath, DataTable p_dsSrc)
        {

            var newFile = new FileInfo(p_strPath);

            using (ExcelPackage objExcelPackage = new ExcelPackage())
            {
                //Create the worksheet    
                ExcelWorksheet objWorksheet = objExcelPackage.Workbook.Worksheets.Add("Sheet1");//p_dsSrc.TableName
                //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1    
                objWorksheet.Cells["A1"].LoadFromDataTable(p_dsSrc,true);
                objWorksheet.Cells.Style.Font.SetFromFont(new Font("Calibri", 10));    
                objWorksheet.Cells.AutoFitColumns();

                objWorksheet.Cells[1, 1].Value = "";
                objWorksheet.Cells[1, 2].Value = "First name";
                objWorksheet.Cells[1, 3].Value = "Last name";
                objWorksheet.Cells[1, 4].Value = "Contact Phone";
                objWorksheet.Cells[1, 5].Value = "ID card type";
                objWorksheet.Cells[1, 6].Value = "ID card no";
                objWorksheet.Cells[1, 7].Value = "Dormitory name";
                objWorksheet.Cells[1, 8].Value = "Building no";
                objWorksheet.Cells[1, 9].Value = "Floor no";
                objWorksheet.Cells[1, 10].Value = "Room no";
                objWorksheet.Cells[1, 11].Value = "Fibre id";
                objWorksheet.Cells[1, 12].Value = "Region";
                

                //Format the header    
                using (ExcelRange objRange = objWorksheet.Cells["A1:L1"])
                {
                    objRange.Style.Font.Bold = true;
                    objRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    objRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    objRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    objRange.Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                    objRange.Style.ShrinkToFit = false;
                }

                for (int i = 0; i <= 11; i++)
                {
                    var cell = objWorksheet.Cells[1, i+1];

                    //Setting top,left,right,bottom border of header cells
                    var border = cell.Style.Border;
                    border.Top.Style = border.Left.Style = border.Bottom.Style = border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                }

                //Write it back to the client    
                if (File.Exists(newFile.FullName))
                    File.Delete(newFile.FullName);

                //Create excel file on physical disk    
                FileStream objFileStrm = File.Create(newFile.FullName);
                objFileStrm.Close();

                //Write content to excel file    
                File.WriteAllBytes(newFile.FullName, objExcelPackage.GetAsByteArray());
            }
        } 
    
 
        public string PrepareAttachFile(string nasPath)
        {
            _logger.Info("Prepare attachment file(s).");
            StartWatching();
            try
            {
                if (!string.IsNullOrEmpty(nasPath))
                {
                    var tempFiles = nasPath.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    var tmpAttachFile = "";

                    foreach (var file in tempFiles)
                    {
                        tmpAttachFile += file + ',';
                    }

                    nasPath = tmpAttachFile.TrimEnd(',');
                }

                StopWatching("Sending an Email");
            }
            catch (Exception ex)
            {
                _logger.Info("Sending an Email" + string.Format(" is error on execute : {0}.",
                    ex.GetErrorMessage()));
                _logger.Info(ex.RenderExceptionMessage());
                StopWatching("Prepare attachment file(s)");
                //throw ex;
            }
            return nasPath;
        }

        public string[] Sendmail(string processname, string createuser, string sendfrom, string sendto, string sendcc, string sendbcc
           , string subject, string body, string[] attach, string ip_mail_server, string frompass, string port,string domain)
        {
            _logger.Info("Sending an Email.");

            string[] result = new string[2];

            StartWatching();
            try
            {
                var command = new SendMailNotificationCommand
                {

                    ProcessName = processname,
                    CreateUser = createuser,
                    SendTo = sendto,
                    SendCC = sendcc,
                    SendBCC = sendbcc,
                    SendFrom = sendfrom,
                    Subject = subject,
                    Body = body,
                    AttachFiles = attach,
                    FromPassword = frompass,
                    Port = port,
                    Domaim = domain,
                    IPMailServer = ip_mail_server
                };


                _sendMail.Handle(command);

                _logger.Info(string.Format("Sending an Email : {0}.", command.ReturnMessage));
                StopWatching("Sending an Email");

                if (command.ReturnMessage == "Success.")
                {
                    result[0] = "0";
                    result[1] = "";
                }
                else
                {
                    result[0] = "-1";
                    result[1] = command.ReturnMessage;
                }
               
            }
            catch (Exception ex)
            {
                _logger.Info("Sending an Email" + string.Format(" is error on execute : {0}.",
                   ex.GetErrorMessage()));
                _logger.Info(ex.GetErrorMessage());

                StopWatching("Sending an Email");
                //throw ex;
               
            }

            return result;
        }


    }
}
