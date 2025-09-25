using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WBBContract.Queries.SftpQueries;

namespace WBBBusinessLayer
{
    public class SftpService
    {
        public string Host { get; set; }
        public int Port { get; set; }

        private List<AuthenticationMethod> SetAuthenticationMethod(string UserName, string KeyFile)
        {
            var privateKey = new PrivateKeyFile(KeyFile);
            List<AuthenticationMethod> _methods = new List<AuthenticationMethod>();
            var keyFiles = new[] { privateKey };
            _methods.Add(new PrivateKeyAuthenticationMethod(UserName, keyFiles));

            return _methods;
        }
        public List<ListfilesModels> Listfiles(string UserName, string KeyFile, string RemovePath, string action, out string msg)
        {
            var listFiles = new List<ListfilesModels>();
            msg = "Success";
            try
            {
                if (action.Equals("DataPower"))
                {
                    List<AuthenticationMethod> _methods = new List<AuthenticationMethod>();
                    _methods = SetAuthenticationMethod(UserName, KeyFile);
                    var con = new ConnectionInfo(Host, port: Port != 0 ? Port : 22, username: UserName, authenticationMethods: _methods.ToArray());
                    using (var client = new SftpClient(con))
                    {
                        client.Connect();
                        if (!Directory.Exists(RemovePath))
                        {
                            listFiles = client.ListDirectory(RemovePath).Select(x => new ListfilesModels
                            {
                                Name = Path.GetFileName(x.FullName),
                                FullPath = x.FullName,
                                DateModified = x.Attributes.LastWriteTime.ToString("dd-MM-yyyy HH:mm:ss"),
                                FileSize = x.Attributes.Size,
                            }).ToList();
                        }
                        else
                        {
                            msg = "Folder not found.";
                        }


                        return listFiles;
                    }
                }
                else
                {
                    using (var client = new SftpClient(Host, UserName, KeyFile))
                    {
                        client.Connect();
                        // / home / leardnk8 / SAP
                        if (!Directory.Exists(RemovePath))
                        {
                            listFiles = client.ListDirectory(RemovePath).Select(x => new ListfilesModels
                            {
                                Name = Path.GetFileName(x.FullName),
                                FullPath = x.FullName,
                                DateModified = x.Attributes.LastWriteTime.ToString("dd-MM-yyyy HH:mm:ss"),
                                FileSize = x.Attributes.Size,
                            }).ToList();
                        }
                        else
                        {
                            msg = "Folder not found.";
                        }
                        return listFiles;
                    }
                }

            }
            catch (Exception exception)
            {
                listFiles.Add(new ListfilesModels { msg = exception.Message.ToString() });
                return listFiles;
            }
        }

        public DownloadFileModel Download(string username, string keyfile, string path, string fileName, string action, out string msg)
        {
            var resp = new DownloadFileModel();
            msg = "Success";
            try
            {
                if (action.Equals("DataPower"))
                {
                    List<AuthenticationMethod> _methods = new List<AuthenticationMethod>();
                    _methods = SetAuthenticationMethod(username, keyfile);
                    var con = new ConnectionInfo(Host, port: Port != 0 ? Port : 22, username: username, authenticationMethods: _methods.ToArray());
                    using (var client = new SftpClient(con))
                    {
                        client.Connect();
                        var file = string.Concat(path, "/", fileName);
                        if (!System.IO.File.Exists(file))
                        {
                            resp.Download = client.ReadAllBytes(file);
                        }
                        else
                        {
                            msg = "File not found.";
                            resp.msg = "File not found.";
                        }

                        return resp;
                    }
                }
                else
                {
                    using (var client = new SftpClient(Host, username, keyfile))
                    {
                        client.Connect();
                        var file = string.Concat(path, "/", fileName);
                        if (!System.IO.File.Exists(file))
                        {
                            resp.Download = client.ReadAllBytes(file);
                        }
                        else
                        {
                            msg = "File not found.";
                            resp.msg = "File not found.";
                        }
                        return resp;
                    }
                }
            }
            catch (Exception exception)
            {
                resp.msg = exception.Message.ToString();
                return resp;
            }
        }

        public DeleteFileModel Delete(string username, string keyfile, string path, string fileName, string action, out string msg)
        {
            DeleteFileModel delfile = new DeleteFileModel();
            msg = "Success";
            try
            {
                if (action.Equals("DataPower"))
                {
                    List<AuthenticationMethod> _methods = new List<AuthenticationMethod>();
                    _methods = SetAuthenticationMethod(username, keyfile);
                    var con = new ConnectionInfo(Host, port: Port != 0 ? Port : 22, username: username, authenticationMethods: _methods.ToArray());
                    using (var client = new SftpClient(con))
                    {
                        try
                        {
                            client.Connect();
                            if (!System.IO.File.Exists(fileName))
                            {
                                client.DeleteFile(fileName);
                                delfile.Delete = true;
                            }
                            else
                            {
                                delfile.Delete = false;
                                msg = "File not found.";
                                delfile.Message = "File not found.";
                            }

                            return delfile;
                        }
                        catch (Exception ex)
                        {
                            delfile.Delete = false;
                            delfile.Message = ex.Message.ToString();
                            return delfile;
                        }
                        finally
                        {
                            client.Disconnect();
                        }
                    }
                }
                else
                {
                    using (var client = new SftpClient(Host, username, keyfile))
                    {
                        try
                        {
                            client.Connect();
                            if (!System.IO.File.Exists(fileName))
                            {
                                client.DeleteFile(fileName);
                                delfile.Delete = true;
                            }
                            else
                            {
                                delfile.Delete = false;
                                msg = "File not found.";
                                delfile.Message = "File not found.";
                            }

                            return delfile;
                        }
                        catch (Exception ex)
                        {
                            delfile.Delete = false;
                            delfile.Message = ex.Message.ToString();
                            return delfile;
                        }
                        finally
                        {
                            client.Disconnect();
                        }
                    }
                }

            }
            catch (Exception e)
            {
                delfile.Delete = false;
                delfile.Message = e.Message.ToString();
                return delfile;
            }
        }

        public UploadFileModel Upload(string username, string keyfile, string path, string fileName, byte[] dataFile, string action, out string msg)
        {
            UploadFileModel resp = new UploadFileModel();
            msg = "Success";
            try
            {
                if (action.Equals("DataPower"))
                {
                    List<AuthenticationMethod> _methods = new List<AuthenticationMethod>();
                    _methods = SetAuthenticationMethod(username, keyfile);
                    var con = new ConnectionInfo(Host, port: Port != 0 ? Port : 22, username: username, authenticationMethods: _methods.ToArray());
                    using (var client = new SftpClient(con))
                    {
                        try
                        {
                            var remotePath = string.Concat(path, "/", fileName);

                            using (Stream stream = new MemoryStream(dataFile))
                            {
                                client.Connect();
                                client.UploadFile(stream, remotePath, true);
                                resp.Upload = true;
                            }
                        }
                        catch
                        {
                            resp.Upload = false;
                            msg = "Upload failed.";
                            resp.Message = "Upload failed.";
                        }
                        finally
                        {
                            client.Disconnect();
                        }
                    }
                }
                else
                {
                    using (var client = new SftpClient(Host, username, keyfile))
                    {
                        try
                        {
                            var remotePath = string.Concat(path, "/", fileName);

                            using (Stream stream = new MemoryStream(dataFile))
                            {
                                client.Connect();
                                client.UploadFile(stream, remotePath, true);
                                resp.Upload = true;
                            }
                        }
                        catch
                        {
                            client.Disconnect();
                            resp.Upload = false;
                            msg = "Upload failed.";
                            resp.Message = "Upload failed.";
                        }
                        finally
                        {
                            client.Disconnect();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                resp.Message = ex.Message.ToString();
            }
            return resp;
        }
    }
}
