using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBPAYG_GenFile_DisneyPB
{
    internal class FBBPAYGGenFileDisneyPBBatch
    {
        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<GendatafileExpireSIMCommand> _genfile;

        public FBBPAYGGenFileDisneyPBBatch(
          ILogger logger,
          IQueryProcessor queryProcessor,
          ICommandHandler<GendatafileExpireSIMCommand> genfile)
        {
            this._logger = logger;
            this._queryProcessor = queryProcessor;
            this._genfile = genfile;
        }
        protected string _Key = ConfigurationManager.AppSettings["KEY"].ToSafeString();
        //public void GenFiles()
        //{
        //    try
        //    {
        //        GenfileExpireSIMCommand command = new GenfileExpireSIMCommand()
        //        {
        //            in_report_name = "FBBPAYG_REPORT_DISNEY_PB"
        //        };
        //        this._genfile.Handle(command);


        //        if (command.ret_code != "0")
        //        {
        //            this._logger.Info((object)("GenFiles Result : " + command.ret_msg));
        //            Console.WriteLine("GenFiles Result : " + command.ret_msg);
        //        }
        //        else
        //        {
        //            this._logger.Info((object)("GenFiles Result : " + command.ret_msg));
        //            Console.WriteLine("GenFiles Result : " + command.ret_msg);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        this._logger.Error((object)("GenFiles Result : " + ex.Message.ToString()));
        //    }
        //    finally
        //    {
        //        this._logger.Info((object)"============ FBBPAYG_GenFile_DisneyPB End ============");
        //    }
        //}

        public void GendataFiles()
        {
            try
            {
                var command = new GendatafileExpireSIMCommand()
                {
                    in_report_name = "FBBPAYG_REPORT_DISNEY_PB"
                };
                _genfile.Handle(command);

                string pathNewDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Temp/");
                if (!Directory.Exists(pathNewDirectory))
                {
                    Directory.CreateDirectory(pathNewDirectory);
                }

                foreach (var data in command.return_report_data_list_cur)
                {
                    string filePath = Path.Combine(pathNewDirectory, data.FILE_NAME);

                    try
                    {
                        File.WriteAllText(filePath, data.FILE_DATA);
                        this._logger.Info((object)($"File created: {filePath}"));
                    }
                    catch (Exception ex)
                    {
                        this._logger.Info((object)($"Error creating file {filePath}: {ex.Message}"));
                    }
                }
                CredentialHelper crd = new CredentialHelper(_logger);

                //Query CREDENTIAL_NAS_TEMP
                var nas_temp_datafile = Get_FBSS_CONFIG_TBL_LOV("FBBPAYG_REPORT_DISNEY_PB", "NAS_CONNECTION")
                        .Where(record => record.ACTIVEFLAG == "Y")
                        .FirstOrDefault();

                var username = EncryptionUtility.Decrypt(nas_temp_datafile.VAL1, _Key);
                var password = EncryptionUtility.Decrypt(nas_temp_datafile.VAL2, _Key); 
                var domain = nas_temp_datafile.VAL3;
                var path = nas_temp_datafile.VAL4;

                crd.UploadFileToServer(username, password, domain, path);

            }
            catch (Exception ex)
            {
                this._logger.Error((object)("GenFiles Result : " + ex.Message.ToString()));
            }
            finally
            {
                this._logger.Info((object)"============ FBBPAYG_GenFile_DisneyPB End ============");
            }
        }

        public bool CheckProcessBatch()
        {
            bool flag = false;
            try
            {
                this._logger.Info((object)"============ FBBPAYG_GenFile_DisneyPB Start ============");
                FbssConfigTBL fbssConfigTbl = this.Get_FBSS_CONFIG_TBL_LOV("FBBPAYG_REPORT_DISNEY_PB", "PROCESS").FirstOrDefault<FbssConfigTBL>();
                this._logger.Info((object)("PROGRAM_PROCESS: " + fbssConfigTbl.ACTIVEFLAG));
                if (fbssConfigTbl.ACTIVEFLAG == "Y")
                    flag = true;
                return flag;
            }
            catch (Exception ex)
            {
                this._logger.Error((object)("Exception CheckProcessBatch : " + ex.Message));
                return flag;
            }
        }

        //public List<FbssConfigTBL> Get_FBSS_CONFIG_TBL_LOV(string _CON_TYPE, string _CON_NAME) => this._queryProcessor.Execute<List<FbssConfigTBL>>((IQuery<List<FbssConfigTBL>>)new GetFbssConfigTBLQuery()
        //{
        //    CON_TYPE = _CON_TYPE,
        //    CON_NAME = _CON_NAME
        //});

        public List<FbssConfigTBL> Get_FBSS_CONFIG_TBL_LOV(string _CON_TYPE, string _CON_NAME)
        {
            var query = new GetFbssConfigTBLQuery()
            {
                CON_TYPE = _CON_TYPE,
                CON_NAME = _CON_NAME
            };
            var _FbssConfig = _queryProcessor.Execute(query);

            return _FbssConfig;
        }


    }
}