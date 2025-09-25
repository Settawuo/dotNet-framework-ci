using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract.Queries.SftpQueries;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers
{
    public static class GetConfigSftpServiceHelper
    {
        public static FileParameter GetParameter(IEntityRepository<object> entity, SftpFileParameter sftpFile)
        {
            #region NAS_PORTAL_ACCESS_TYPE  

            var return_code = new OracleParameter()
            {
                OracleDbType = OracleDbType.Int16,
                ParameterName = "return_code",
                Direction = ParameterDirection.Output
            };
            var return_message = new OracleParameter()
            {
                OracleDbType = OracleDbType.Varchar2,
                ParameterName = "return_message",
                Size = 2000,
                Direction = ParameterDirection.Output
            };
            var cur_list_lov = new OracleParameter()
            {
                OracleDbType = OracleDbType.RefCursor,
                ParameterName = "cur_list_lov",
                Direction = ParameterDirection.Output
            };
            var p_lov_type = new OracleParameter()
            {
                OracleDbType = OracleDbType.Varchar2,
                ParameterName = "p_lov_type",
                Direction = ParameterDirection.Input,
                Value = sftpFile.Lovtype
            };
            var p_lov_name = new OracleParameter()
            {
                OracleDbType = OracleDbType.Varchar2,
                ParameterName = "p_lov_name",
                Direction = ParameterDirection.Input,
                Value = ""
            };


            var outp = new List<object>();
            var paramOut = outp.ToArray();

            var executeResult = entity.ExecuteStoredProcMultipleCursor("wbb.pkg_fbbpayg_get_config.get_lov",
            new object[]{
                   p_lov_type ,
                   p_lov_name ,
                   return_code,
                   return_message,
                   cur_list_lov

               });

            var data = (DataTable)executeResult[2];
            var lov = data.DataTableToList<FBB_CFG_LOV>();
            #endregion


            var acc = lov.Where(s => s.LOV_NAME == sftpFile.LovnameAcc).FirstOrDefault();
            var path = lov.Where(s => s.LOV_NAME == sftpFile.LovnamePath).ToList();


            #region Config_DataPower
            //var p_lov_typePower = new OracleParameter
            //{
            //    ParameterName = "p_lov_typePower",
            //    OracleDbType = OracleDbType.Varchar2,          
            //    Direction = ParameterDirection.Input,
            //    Value = sftpFile.LovtypePower
            //};
            //var p_lov_namePower = new OracleParameter
            //{
            //    ParameterName = "p_lov_namePower",
            //    OracleDbType = OracleDbType.Varchar2,
            //    Direction = ParameterDirection.Input,
            //    Value = acc == null ? "DataPower" : acc.LOV_VAL1
            //};
            //var return_codePower = new OracleParameter()
            //{
            //    OracleDbType = OracleDbType.Int16,
            //    ParameterName = "return_codePower",
            //    Direction = ParameterDirection.Output
            //};
            //var return_messagePower = new OracleParameter()
            //{
            //    OracleDbType = OracleDbType.Varchar2,
            //    ParameterName = "return_messagePower",
            //    Size = 2000,
            //    Direction = ParameterDirection.Output
            //};
            //var cur_list_lovPower = new OracleParameter()
            //{
            //    OracleDbType = OracleDbType.RefCursor,
            //    ParameterName = "cur_list_lovPower",
            //    Direction = ParameterDirection.Output
            //};
            //var getLovPower = entity.ExecuteStoredProcMultipleCursor("wbb.pkg_fbbpayg_get_config.get_lov",
            //    new object[]
            //    {
            //            p_lov_typePower,
            //            p_lov_namePower,
            //            return_codePower,
            //            return_messagePower,
            //            cur_list_lovPower
            //    });

            //var dataPower = (DataTable)getLovPower[2];
            //var lovPower = dataPower.DataTableToList<FBB_CFG_LOV>().First();
            #endregion


            FBB_CFG_LOV user = null;
            if (sftpFile.Nas == null)
                user = path.First();
            else
                user = path.Where(s => s.DISPLAY_VAL.Equals(sftpFile.Nas)).First();

            string username = "";
            string KeyFiles = "";
            string remotePath = "";

            if (sftpFile.Lovtype == "FBB_NAS_PORTAL" || sftpFile.Lovtype == "Config_LoadFile")
            {
                username = EncryptionUtility.Decrypt(user.LOV_VAL2, sftpFile.Key);
                KeyFiles = EncryptionUtility.Decrypt(user.LOV_VAL3.ToString(), sftpFile.Key.ToString());//PATH LOV_VAL1
                remotePath = user.LOV_VAL1;
            }
            else if (sftpFile.Lovtype == "Config_OM10")
            {
                username = EncryptionUtility.DecryptBase64(user.LOV_VAL1.ToString(), sftpFile.Key.ToString());
                KeyFiles = EncryptionUtility.DecryptBase64(user.LOV_VAL2.ToString(), sftpFile.Key.ToString());//PATH LOV_VAL4
                remotePath = user.LOV_VAL4;
            }

            var param = new FileParameter
            {
                Host = user.LOV_VAL4,
                Port = user.LOV_VAL5.ToSafeInteger(),
                KeyFile = KeyFiles ?? "",
                UserName = username,
                ConfigType = acc.LOV_VAL1,
                remotePath = remotePath
            };
            return param;
        }

    }

    public static class GetNasServiceHelper
    {
        public static NasFileParameter GetParameter(IEntityRepository<object> entity, SftpFileParameter sftpFile)
        {
            #region NAS_PORTAL_ACCESS_TYPE  

            var return_code = new OracleParameter()
            {
                OracleDbType = OracleDbType.Int16,
                ParameterName = "return_code",
                Direction = ParameterDirection.Output
            };
            var return_message = new OracleParameter()
            {
                OracleDbType = OracleDbType.Varchar2,
                ParameterName = "return_message",
                Size = 2000,
                Direction = ParameterDirection.Output
            };
            var cur_list_lov = new OracleParameter()
            {
                OracleDbType = OracleDbType.RefCursor,
                ParameterName = "cur_list_lov",
                Direction = ParameterDirection.Output
            };
            var p_lov_type = new OracleParameter()
            {
                OracleDbType = OracleDbType.Varchar2,
                ParameterName = "p_lov_type",
                Direction = ParameterDirection.Input,
                Value = sftpFile.Lovtype
            };
            var p_lov_name = new OracleParameter()
            {
                OracleDbType = OracleDbType.Varchar2,
                ParameterName = "p_lov_name",
                Direction = ParameterDirection.Input,
                Value = ""
            };


            var outp = new List<object>();
            var paramOut = outp.ToArray();

            var executeResult = entity.ExecuteStoredProcMultipleCursor("wbb.pkg_fbbpayg_get_config.get_lov",
            new object[]{
                   p_lov_type ,
                   p_lov_name ,
                   return_code,
                   return_message,
                   cur_list_lov

               });

            var data = (DataTable)executeResult[2];
            var lov = data.DataTableToList<FBB_CFG_LOV>();
            #endregion


            var acc = lov.Where(s => s.LOV_NAME == sftpFile.LovnameAcc).FirstOrDefault();
            var path = lov.Where(s => s.LOV_NAME == sftpFile.LovnamePath).ToList();

            FBB_CFG_LOV user = null;
            if (sftpFile.Nas == null)
                user = path.First();
            else
                user = path.Where(s => s.DISPLAY_VAL.Equals(sftpFile.Nas)).First();

            string username = "";
            string FromPass = "";// Fixed Code scan : string Password = "";
            string remotePath = "";

            if (sftpFile.Lovtype == "FBB_NAS_PORTAL" || sftpFile.Lovtype == "Config_LoadFile")
            {
                username = EncryptionUtility.Decrypt(user.LOV_VAL2, sftpFile.Key);
                FromPass = EncryptionUtility.Decrypt(user.LOV_VAL3.ToString(), sftpFile.Key.ToString());//PATH LOV_VAL1
                remotePath = user.LOV_VAL1;
            }
            else if (sftpFile.Lovtype == "Config_OM10")
            {
                username = EncryptionUtility.DecryptBase64(user.LOV_VAL1.ToString(), sftpFile.Key.ToString());
                FromPass = EncryptionUtility.DecryptBase64(user.LOV_VAL2.ToString(), sftpFile.Key.ToString());//PATH LOV_VAL4
                remotePath = user.LOV_VAL4;
            }

            var param = new NasFileParameter
            {
                Host = user.LOV_VAL4,
                Port = user.LOV_VAL5.ToSafeInteger(),
                Password = FromPass ?? "",
                UserName = username,
                ConfigType = acc.LOV_VAL1,
                remotePath = remotePath
            };
            return param;
        }

    }
}
