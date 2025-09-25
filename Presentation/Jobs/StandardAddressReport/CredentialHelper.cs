using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;


namespace FBBPayGTransAirnet
{
    using OfficeOpenXml;
    using OfficeOpenXml.Style;
    using OfficeOpenXml.Table;
    using System.Data;
    using System.IO;
    using System.Text;
    using WBBBusinessLayer;

    public class CredentialHelper
    {
        public ILogger _logger;
        public CredentialHelper(ILogger logger)
        {
            _logger = logger;
        }

        const int LOGON32_PROVIDER_DEFAULT = 0;
        // This parameter causes LogonUser to create a primary token.
        const int LOGON32_LOGON_INTERACTIVE = 9;

        [DllImport("advapi32.DLL", SetLastError = true)]
        public static extern int LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType,
            int dwLogonProvider, ref IntPtr phToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public extern static bool CloseHandle(IntPtr handle);


        /// <summary>
        /// copy file with remote computer
        /// </summary>
        /// <param name="username">username for authenticate</param>
        /// <param name="pwd">password for authenticate</param>
        /// <param name="domin">domain name of user for authenticate</param>
        /// <param name="sourceFullPathName">source path and file name with file extension</param>
        /// <param name="targetFullPathName">target path and file name with file extension</param>
        ///
        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        public bool CopyFile(string username, string pwd, string domin, string sourceFullPathName,
            string targetFullPathName)
        {
            // ReSharper disable once UnusedVariable
            var widCurrent = WindowsIdentity.GetCurrent();
            WindowsImpersonationContext wic = null;
            var isCopySucess = false;

            try
            {
                var adminToken = new IntPtr();

                //if (LogonUser("nas_staffpacks", "corp-ais900", "/O9|}yPS.1[t", 9, 0, ref admin_token) != 0)
                if (LogonUser(username, domin, pwd, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref adminToken) != 0)
                {
                    wic = new WindowsIdentity(adminToken).Impersonate();
                    //File.Copy(sourceFullPathName, targetFullPathName, true);
                    if (Directory.Exists(sourceFullPathName))
                    {
                        string[] files = Directory.GetFiles(sourceFullPathName);
                        var sdas = "asdsad";
                    }
                    isCopySucess = true;
                }
                else
                {
                    int ret = Marshal.GetLastWin32Error();
                    _logger.Info(string.Format("LogonUser failed with error code : {0}", ret));
                    //Console.WriteLine("LogonUser failed with error code : {0}", ret);
                    //throw new System.ComponentModel.Win32Exception(ret);
                }
            }
            // ReSharper disable once RedundantCatchClause
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (wic != null)
                {
                    wic.Undo();
                }
            }
            return isCopySucess;
        }

        /// <summary>
        /// copy file
        /// </summary>
        /// <param name="sourceFullPathName">source path and file name with file extension</param>
        /// <param name="targetFullPathName">target path and file name with file extension</param>
        public static bool CopyFile(string sourceFullPathName, string targetFullPathName)
        {
            bool isCopySucess;

            try
            {
                File.Copy(sourceFullPathName, targetFullPathName, true);
                isCopySucess = true;
            }
            catch (Exception)
            {
                isCopySucess = false;
            }

            return isCopySucess;
        }

        /// <summary>
        /// copy file
        /// </summary>
        /// <param name="fileName">filename</param>
        /// <param name="sourcePath">source path</param>
        /// <param name="targetPath">target path</param>
        public static bool CopyFile(string fileName, string sourcePath, string targetPath)
        {
            bool isCopySucess;

            try
            {
                File.Copy(sourcePath + fileName, targetPath + fileName, true);
                isCopySucess = true;
            }
            catch (Exception)
            {
                isCopySucess = false;
            }

            return isCopySucess;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <returns></returns>
        public static bool RemoveFile(string sourcePath)
        {
            bool isRemoveSucess;

            try
            {
                File.Delete(sourcePath);
                isRemoveSucess = true;
            }
            catch (Exception)
            {
                isRemoveSucess = false;
            }

            return isRemoveSucess;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pwd"></param>
        /// <param name="domin"></param>
        /// <param name="sourcePath"></param>
        /// <returns></returns>
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static bool RemoveFile(string username, string pwd, string domin, string sourcePath)
        {
            // ReSharper disable once UnusedVariable
            var widCurrent = WindowsIdentity.GetCurrent();
            // ReSharper disable once NotAccessedVariable
            WindowsImpersonationContext wic = null;
            bool isRemoveSucess;

            try
            {
                var adminToken = new IntPtr();

                if (LogonUser(username, domin, pwd, 9, 0, ref adminToken) != 0)
                {
                    // ReSharper disable once RedundantAssignment
                    wic = new WindowsIdentity(adminToken).Impersonate();

                    //File.Copy(@"D:\test.wsdl", @"\\10.239.109.211\staff_upload\test.wsdl", true);
                    File.Delete(sourcePath);
                    isRemoveSucess = true;
                }
                else
                {
                    isRemoveSucess = false;
                }
            }
            // ReSharper disable once RedundantCatchClause
            catch (Exception)
            {
                throw;
            }
            return isRemoveSucess;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pwd"></param>
        /// <param name="domin"></param>
        /// <param name="sourcePath"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static bool RemoveFile(string username, string pwd, string domin, string sourcePath, string fileName)
        {
            // ReSharper disable once UnusedVariable
            var widCurrent = WindowsIdentity.GetCurrent();
            // ReSharper disable once NotAccessedVariable
            WindowsImpersonationContext wic = null;
            var isRemoveSucess = false;

            try
            {
                var adminToken = new IntPtr();
                if (LogonUser(username, domin, pwd, 9, 0, ref adminToken) != 0)
                {
                    // ReSharper disable once RedundantAssignment
                    wic = new WindowsIdentity(adminToken).Impersonate();
                    var path = Path.Combine(sourcePath, fileName);
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                    isRemoveSucess = true;
                }
            }
            // ReSharper disable once RedundantCatchClause
            catch (Exception)
            {
                throw;
            }

            return isRemoveSucess;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pwd"></param>
        /// <param name="domin"></param>
        /// <param name="sourceFullPathName"></param>
        /// <returns></returns>
        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        public static byte[] ReadFile(string username, string pwd, string domin, string sourceFullPathName)
        {
            // ReSharper disable once UnusedVariable
            var widCurrent = WindowsIdentity.GetCurrent();
            WindowsImpersonationContext wic = null;
            byte[] byteData = null;

            try
            {
                var adminToken = new IntPtr();

                //if (LogonUser("nas_staffpacks", "corp-ais900", "/O9|}yPS.1[t", 9, 0, ref admin_token) != 0)
                if (LogonUser(username, domin, pwd, 9, 0, ref adminToken) != 0)
                {

                    wic = new WindowsIdentity(adminToken).Impersonate();
                    if (File.Exists(sourceFullPathName))
                    {
                        byteData = File.ReadAllBytes(sourceFullPathName);
                    }
                }
            }
            // ReSharper disable once RedundantCatchClause
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (wic != null)
                {
                    wic.Undo();
                }
            }

            return byteData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pwd"></param>
        /// <param name="domin"></param>
        /// <param name="targetPathName"></param>
        /// <param name="fileName"></param>
        /// <param name="inputStream"></param>
        /// <returns></returns>
        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        public static bool CopyFileStream(string username, string pwd, string domin, string targetPathName, string fileName, Stream inputStream)
        {
            // ReSharper disable once UnusedVariable
            var widCurrent = WindowsIdentity.GetCurrent();
            WindowsImpersonationContext wic = null;
            var isCopySucess = false;
            try
            {

                var adminToken = new IntPtr();
                //if (LogonUser("nas_staffpacks", "corp-ais900", "/O9|}yPS.1[t", 9, 0, ref admin_token) != 0)
                if (LogonUser(username, domin, pwd, 9, 0, ref adminToken) != 0)
                {
                    wic = new WindowsIdentity(adminToken).Impersonate();
                    if (!Directory.Exists(targetPathName))
                    {
                        Directory.CreateDirectory(targetPathName);
                    }
                    var stream = inputStream;
                    var path = Path.Combine(targetPathName, fileName);
                    using (var fileStream = File.Create(path))
                    {
                        if (stream != null)
                        {
                            stream.CopyTo(fileStream);
                        }
                    }

                    isCopySucess = true;
                }
            }
            // ReSharper disable once RedundantCatchClause
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (wic != null)
                {
                    wic.Undo();
                }
            }

            return isCopySucess;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pwd"></param>
        /// <param name="domin"></param>
        /// <param name="sourceFullPathName"></param>
        /// <returns></returns>
        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        public static string[] GetAllFile(string username, string pwd, string domin, string sourceFullPathName)
        {
            WindowsImpersonationContext wic = null;
            string[] filelist = null;

            try
            {
                var adminToken = new IntPtr();
                if (LogonUser(username, domin, pwd, 9, 0, ref adminToken) != 0)
                {
                    wic = new WindowsIdentity(adminToken).Impersonate();

                    if (Directory.Exists(sourceFullPathName))
                    {
                        filelist = Directory.GetFiles(sourceFullPathName);
                    }
                }
            }
            // ReSharper disable once RedundantCatchClause
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (wic != null)
                {
                    wic.Undo();
                }
            }

            return filelist;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pwd"></param>
        /// <param name="domin"></param>
        /// <param name="sourceFullPathName"></param>
        /// <returns></returns>
        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        public static bool TestConectionNas(string username, string pwd, string domin, string sourceFullPathName)
        {
            // ReSharper disable once UnusedVariable
            var widCurrent = WindowsIdentity.GetCurrent();
            WindowsImpersonationContext wic = null;
            var isSuccess = false;

            try
            {
                var adminToken = new IntPtr();
                if (LogonUser(username, domin, pwd, 9, 0, ref adminToken) != 0)
                {
                    wic = new WindowsIdentity(adminToken).Impersonate();
                    if (File.Exists(sourceFullPathName))
                    {
                    }
                    isSuccess = true;
                }
            }
            catch (Exception)
            {
                isSuccess = false;
            }
            finally
            {
                if (wic != null)
                {
                    wic.Undo();
                }
            }

            return isSuccess;
        }



        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        public bool WriteFile(string username, string pwd, string domin, string pathName, string fileName, DataTable data)
        {
            // ReSharper disable once UnusedVariable
            var widCurrent = WindowsIdentity.GetCurrent();
            WindowsImpersonationContext wic = null;
            var isSuccess = false;

            try
            {
                var adminToken = new IntPtr();
                if (LogonUser(username, domin, pwd, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref adminToken) != 0)
                {
                    wic = new WindowsIdentity(adminToken).Impersonate();
                    isSuccess = GenerateDATForStandAddress(data, pathName, fileName);
                    isSuccess = GenerateDATSyncForStandAddress(data, pathName, fileName);
                }
            }
            catch (Exception)
            {
                //_logger.Info("Write file => Exception : " + e.Message);
                isSuccess = false;
            }

            return isSuccess;
        }

        public bool WriteFile(string pathName, string fileName, DataTable data)
        {
            bool isSuccess;

            try
            {
                isSuccess = GenerateDATForStandAddress(data, pathName, fileName);
                isSuccess = GenerateDATSyncForStandAddress(data, pathName, fileName);

            }
            catch (Exception)
            {
                //_logger.Info("Write file => Exception : " + e.Message);
                isSuccess = false;
            }

            return isSuccess;
        }

        private bool GenerateDATForStandAddress(DataTable data, string directoryPath, string fileName)
        {
            string finalFileNameWithPath = PrepareFile(directoryPath, string.Format("{0}.{1}", fileName, "dat"));
            if (finalFileNameWithPath.Equals("N"))
            {
                return false;
            }

            StreamWriter file = new StreamWriter(finalFileNameWithPath, true);
            file.WriteLine("01|{0}", fileName);

            for (int i = 0; i < data.Rows.Count; i++)
            {
                DataRow temprow = (DataRow)data.Rows[i];
                object[] array = temprow.ItemArray;
                StringBuilder sb = new StringBuilder();
                sb.Append(string.Format("02|{0}", (i + 1)));
                for (var j = 1; j < array.Length; j++)
                {
                    sb.Append(string.Format("|{0}", array[j].ToString()));
                }
                file.WriteLine(sb.ToString());
            }
            //foreach (DataRow row in data.Rows){}

            file.WriteLine("09|{0}", data.Rows.Count);
            file.Flush();
            file.Close();

            return WriteFileProcess(finalFileNameWithPath);
        }

        private bool GenerateDATSyncForStandAddress(DataTable data, string directoryPath, string fileName)
        {
            string finalFileNameWithPath = PrepareFile(directoryPath, string.Format("{0}.{1}", fileName, "sync"));
            if (finalFileNameWithPath.Equals("N"))
            {
                return false;
            }

            StreamWriter file = new StreamWriter(finalFileNameWithPath, true);
            file.WriteLine("{0}|{1}", fileName, data.Rows.Count);
            file.Close();

            return WriteFileProcess(finalFileNameWithPath);
        }

        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        public bool WriteFileExcel(string username, string pwd, string domin, string pathName, string fileName, DataTable data, string excelSheetName, string lovVal5, int columnCount)
        {
            // ReSharper disable once UnusedVariable
            var widCurrent = WindowsIdentity.GetCurrent();
            WindowsImpersonationContext wic = null;
            var isSuccess = false;

            try
            {
                var adminToken = new IntPtr();
                if (LogonUser(username, domin, pwd, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref adminToken) != 0)
                {
                    wic = new WindowsIdentity(adminToken).Impersonate();
                    isSuccess = GenerateExcelForStandAddress(data, excelSheetName, pathName, fileName, lovVal5, columnCount);
                }
            }
            catch (Exception e)
            {
                _logger.Info("Write file excel => Exception : " + e.Message);
                isSuccess = false;
            }

            return isSuccess;
        }

        public bool WriteFileExcel(string pathName, string fileName, DataTable data, string excelSheetName, string lovVal5, int columnCount)
        {
            bool isSuccess;

            try
            {
                isSuccess = GenerateExcelForStandAddress(data, excelSheetName, pathName, fileName, lovVal5, columnCount);
            }
            catch (Exception)
            {
                isSuccess = false;
            }

            return isSuccess;
        }

        private bool GenerateExcelForStandAddress(DataTable dataToExcel, string excelSheetName, string directoryPath, string fileName, string lovVal5, int columnCount)
        {
            string finalFileNameWithPath = PrepareFile(directoryPath, string.Format("{0}.{1}", fileName, "xlsx"));
            if (finalFileNameWithPath.Equals("N"))
            {
                return false;
            }

            var newFile = new FileInfo(finalFileNameWithPath);
            ExcelRange range1 = null;

            //Step 1 : Create object of ExcelPackage class and pass file path to constructor.
            using (var package = new ExcelPackage(newFile))
            {
                //Step 2 : Add a new worksheet to ExcelPackage object and give a suitable name
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(excelSheetName);

                //worksheet 
                if (string.Compare(lovVal5, "FULLCONNECTION", true) == 0)
                {
                    #region FULLCONNECTION
                    //int x = (table1.Rows.Count / 10000);

                    range1 = worksheet.SelectedRange[1, 1, 1, columnCount];
                    range1.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range1.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#c9c9c9"));
                    range1.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheet.View.FreezePanes(2, 1);

                    worksheet.Cells["A1"].LoadFromDataTable(dataToExcel, true, TableStyles.None);
                    #endregion
                }
                //Step 4 : (Optional) Set the file properties like title, author and subject
                package.Workbook.Properties.Title = @"FBB Config";
                package.Workbook.Properties.Author = "FBB";
                package.Workbook.Properties.Subject = @"" + excelSheetName;

                //Step 5 : Save all changes to ExcelPackage object which will create Excel 2007 file.
                package.Save();

                return true;
            }
        }

        private string PrepareFile(string directoryPath, string fileName)
        {
            string finalFileNameWithPath = Path.Combine(directoryPath, fileName);
            // Check Directory
            if (!Directory.Exists(directoryPath))
            {
                _logger.Info("Directory path not found -> " + directoryPath);
                return "N";
                //Directory.CreateDirectory(directoryPath)
            };
            // Check File Duplicate
            if (File.Exists(finalFileNameWithPath)) File.Delete(finalFileNameWithPath);

            return finalFileNameWithPath;
        }

        private bool WriteFileProcess(string finalFileNameWithPath)
        {
            try
            {
                using (var fileIO = File.Open(finalFileNameWithPath, FileMode.Open))
                {
                    return true;
                }
                // Todo: Work with open file here
            }
            catch (FileNotFoundException e)
            {
                _logger.Info("Failed to open file -> " + finalFileNameWithPath);
                return false;
            }

        }

    }
}
