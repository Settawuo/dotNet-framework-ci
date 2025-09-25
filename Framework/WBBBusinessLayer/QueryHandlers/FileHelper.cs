using AIRNETEntity.Extensions;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;
using WBBContract.Queries.SftpQueries;

namespace WBBBusinessLayer.QueryHandlers
{
    /// <summary>
    /// 
    /// </summary>
    public class FileHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lpszUsername"></param>
        /// <param name="lpszDomain"></param>
        /// <param name="lpszPassword"></param>
        /// <param name="dwLogonType"></param>
        /// <param name="dwLogonProvider"></param>
        /// <param name="phToken"></param>
        /// <returns></returns>
        [DllImport("advapi32.DLL", SetLastError = true)]
        public static extern int LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType,
            int dwLogonProvider, ref IntPtr phToken);

        /// <summary>
        /// copy file with remote computer
        /// </summary>
        /// <param name="username">username for authenticate</param>
        /// <param name="pwd">password for authenticate</param>
        /// <param name="domin">domain name of user for authenticate</param>
        /// <param name="sourceFullPathName">source path and file name with file extension</param>
        /// <param name="targetFullPathName">target path and file name with file extension</param>
        ///
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static bool CopyFile(string username, string pwd, string domin, string sourceFullPathName,
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
                if (LogonUser(username, domin, pwd, 9, 0, ref adminToken) != 0)
                {
                    wic = new WindowsIdentity(adminToken).Impersonate();
                    File.Copy(sourceFullPathName, targetFullPathName, true);
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
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
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
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
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
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
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
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pwd"></param>
        /// <param name="domin"></param>
        /// <param name="excelPackage"></param>
        /// <returns></returns>
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static bool CopyFileDataTableEpPlus(string username, string pwd, string domin, ExcelPackage excelPackage)
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
                    excelPackage.Save();
                    //var newFile = new FileInfo(fullFileName);
                    //using (var package = new ExcelPackage(newFile))
                    //{
                    //    var worksheet =
                    //        package.Workbook.Worksheets.Add(workSheetName);
                    //    worksheet.Cells["A1"].LoadFromDataTable(dt, true, TableStyles.None);
                    //    worksheet.Cells["A:Z"].AutoFitColumns(15);
                    //    package.Save();
                    //}

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

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static bool WriteTextfileListString(string username, string pwd, string domin, string header, List<String> strDataList, string pathFileName, string fileName, Encoding encoding)
        {
            // ReSharper disable once UnusedVariable
            var widCurrent = WindowsIdentity.GetCurrent();
            WindowsImpersonationContext wic = null;
            var isSucess = false;
            FileStream fs = null;
            try
            {

                var adminToken = new IntPtr();
                //if (LogonUser("nas_staffpacks", "corp-ais900", "/O9|}yPS.1[t", 9, 0, ref admin_token) != 0)
                if (LogonUser(username, domin, pwd, 9, 0, ref adminToken) != 0)
                {
                    wic = new WindowsIdentity(adminToken).Impersonate();
                    string filepath = @"";
                    filepath += pathFileName + fileName + ".txt";
                    if (File.Exists(filepath))
                        File.Delete(filepath);
                    //==========================================
                    fs = new FileStream(filepath,
                        FileMode.Create,
                        FileAccess.ReadWrite, FileShare.None);
                    //==========================================

                    using (TextWriter objFile = new StreamWriter(fs, encoding))//ANSI/OEM Thai (ISO 8859-11); Thai (Windows)
                    {
                        if (!string.IsNullOrEmpty(header))
                            objFile.WriteLine(header);

                        foreach (string val in strDataList)
                        {
                            objFile.WriteLine(val);
                        }
                    }

                    isSucess = true;

                }

            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (wic != null)
                {
                    wic.Undo();
                }
                fs.Dispose();
            }

            return isSucess;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pwd"></param>
        /// <param name="domin"></param>
        /// <param name="fullFilename"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static bool ZipFile(string username, string pwd, string domin, string fullFilename, string filename)
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
                    var extension = Path.GetExtension(fullFilename);
                    var zipFileName = fullFilename.Replace(extension.ToSafeString(), ".zip");
                    using (var zipToCreate = new FileStream(zipFileName, FileMode.Create))
                    {
                        using (var archive = new ZipArchive(zipToCreate, ZipArchiveMode.Update))
                        {
                            archive.CreateEntryFromFile(fullFilename, filename);
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
        /// <param name="fullFilename"></param>
        /// <param name="filenames"></param>
        /// <returns></returns>
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static bool ZipMultipleFile(string username, string pwd, string domin, string zipFilename, string fullFilename, List<FileInfo> filenames)
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
                    var extension = Path.GetExtension(zipFilename);
                    var zipFileName = zipFilename.Replace(extension.ToSafeString(), ".zip");
                    using (var zipToCreate = new FileStream(zipFileName, FileMode.Create))
                    {
                        using (var archive = new ZipArchive(zipToCreate, ZipArchiveMode.Update))
                        {
                            foreach (var item in filenames)
                            {
                                archive.CreateEntryFromFile(fullFilename + item.Name, item.Name, CompressionLevel.Optimal);
                            }
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

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static bool AttachmentFileEmail(string username, string pwd, string domin, MailMessage mailMessage, string fullFilename)
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
                    if (mailMessage != null)
                        mailMessage.Attachments.Add(new Attachment(fullFilename));

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

        #region new method nas

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pwd"></param>
        /// <param name="domin"></param>
        /// <param name="sourceFullPathName"></param>
        /// <returns></returns>
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static List<ListfilesModels> GetAllFileNas(string username, string pwd, string domin, string sourceFullPathName, out string msg)
        {
            WindowsImpersonationContext wic = null;
            var listFiles = new List<ListfilesModels>();
            msg = "Success";
            try
            {

                var adminToken = new IntPtr();
                if (LogonUser(username, domin, pwd, 9, 0, ref adminToken) != 0)
                {
                    wic = new WindowsIdentity(adminToken).Impersonate();

                    if (Directory.Exists(sourceFullPathName))
                    {
                        var filelist = Directory.GetFiles(sourceFullPathName);
                        var di = new DirectoryInfo(sourceFullPathName);
                        listFiles.AddRange(di.GetFiles().Select(file => new ListfilesModels
                        {
                            Name = Path.GetFileName(file.FullName),
                            FullPath = file.FullName,
                            DateModified = file.LastWriteTime.ToString("dd-MM-yyyy HH:mm:ss"),
                            FileSize = file.Length,
                        }));
                    }
                    else
                    {
                        msg = "Folder not found.";
                    }
                }
            }
            // ReSharper disable once RedundantCatchClause
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (wic != null)
                {
                    wic.Undo();
                }
            }

            return listFiles;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="host"></param>
        /// <param name="remotePath"></param>
        /// <returns></returns>
        public static DownloadFileModel DownloadFile(string userName, string password, string host, string remotePath, out string msg)
        {
            var resp = new DownloadFileModel();
            var widCurrent = WindowsIdentity.GetCurrent();
            WindowsImpersonationContext wic = null;
            msg = "Success";
            try
            {
                var adminToken = new IntPtr();

                if (LogonUser(userName, host, password, 9, 0, ref adminToken) != 0)
                {

                    wic = new WindowsIdentity(adminToken).Impersonate();
                    if (File.Exists(remotePath))
                    {
                        resp.Download = File.ReadAllBytes(remotePath);
                    }
                    else
                    {
                        msg = "File not found.";
                    }
                }
            }
            // ReSharper disable once RedundantCatchClause
            catch (Exception ex)
            {
                msg = ex.Message.ToString();
                resp.msg = ex.Message.ToString();
            }
            finally
            {
                if (wic != null)
                {
                    wic.Undo();
                }
            }

            return resp;
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
        public static DeleteFileModel NasRemoveFile(string username, string pwd, string domin, string sourcePath, out string msg)
        {
            var resp = new DeleteFileModel();
            var widCurrent = WindowsIdentity.GetCurrent();
            WindowsImpersonationContext wic = null;
            msg = "Success";
            try
            {
                var adminToken = new IntPtr();

                if (LogonUser(username, domin, pwd, 9, 0, ref adminToken) != 0)
                {
                    wic = new WindowsIdentity(adminToken).Impersonate();
                    if (File.Exists(sourcePath))
                    {
                        File.Delete(sourcePath);
                        resp.Delete = true;
                    }
                    else
                    {
                        msg = "Delete File not found.";
                        resp.Delete = false;
                    }
                }
                else
                {
                    resp.Delete = false;
                }
            }
            // ReSharper disable once RedundantCatchClause
            catch (Exception ex)
            {
                msg = ex.Message.ToString();
            }
            return resp;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="host"></param>
        /// <param name="remotePath"></param>
        /// <param name="dataFile"></param>
        /// <returns></returns>
        public static UploadFileModel UploadFile(string userName, string password, string host, string remotePath, byte[] dataFile, out string msg)
        {

            var resp = new UploadFileModel();
            var widCurrent = WindowsIdentity.GetCurrent();
            WindowsImpersonationContext wic = null;
            msg = "Success";
            try
            {
                var adminToken = new IntPtr();

                if (LogonUser(userName, host, password, 9, 0, ref adminToken) != 0)
                {
                    wic = new WindowsIdentity(adminToken).Impersonate();

                    try
                    {
                        // Write the byte array to the other FileStream.
                        using (FileStream fsNew = new FileStream(remotePath,
                            FileMode.Create, FileAccess.Write))
                        {
                            fsNew.Write(dataFile, 0, dataFile.Length);
                        }
                        resp.Upload = true;
                    }
                    catch (FileNotFoundException ioEx)
                    {

                        msg = ioEx.Message;
                        resp.Upload = false;
                        Console.WriteLine(ioEx.Message);
                    }
                    // byte[] bytes = new byte[dataFile.Length];
                    /* StreamWriter file = new StreamWriter(pathNew, true);

                     using (FileStream fsNew = new FileStream(pathNew,
                         FileMode.Create, FileAccess.Write))
                     {
                         fsNew.Write(dataFile, 0, dataFile.Length);
                     }*/

                    // file.WriteLine(dataFile);
                    // file.Close();
                    // resp.Upload = true;
                }
                else
                {
                    resp.Upload = false;
                }
            }
            catch
            {
                msg = "Upload failed";
                resp.Upload = false;
            }
            return resp;

        }
        #endregion
    }
}