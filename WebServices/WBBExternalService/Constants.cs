using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

namespace WBBExternalService
{
    public static class Constants
    {
        public enum AppSettingKeys
        {
            ProjectCodeDb,
            TestFlag,
            OnDevEnvironment,

            ProjectCodeNAS,
            UploadFileByVirtualDir,
            UploadFileTempPath,
            UploadFilePath,
            UploadFileByImpersonate,
            UploadPDFVastotals,

            FontFolder,
            ImageFolder,
        }

        public static class Result
        {
            public const string Success = "Success";
            public const string Failed = "Failed";
        }

        public static class ErrorCode
        {
            public const string External = "01";
            public const string Internal = "02";
        }

        public static class ErrorReason
        {
            public const string Standard = "The service was encountered with some error, please contact fbb admin.";
        }

        public static class QuickWinModelBuilderErrorMessage
        {
            public static string MainPackIsNull(bool isLocalLanguage)
            {
                return isLocalLanguage ? "ไม่พบแพ็คเกจหลักของคุณ กรุณาเลือกแพ็คเกจที่ต้องการสมัคร"
                    : "Your main package not found. Please choose package before. ";
            }
        }

        public static class CheckCoverageErrorMessage
        {
            public static string OutOfCoverage(bool isLocalLanguage)
            {
                return isLocalLanguage ? "ที่อยู่ที่คุณแจ้งมาอยู่ระหว่างพิจารณาขยายพื้นที่ให้บริการ หากสามารถให้บริการได้แล้วเจ้าหน้าที่จะทำการติดต่อกลับทันที คุณสามารถติดตามข่าวสารเพิ่มเติมได้ผ่านหน้าเว็บไซต์ AIS FibreNet"
                    : "Your address is in the area that is on the expanded process.When your location is in coverage, We will contact you back immediately.Thank you.";
            }
        }

        public static class CheckSFFInternetProfileErrorMessage
        {
            public static string ProfileNotFound(bool isLocalLanguage)
            {
                return isLocalLanguage ? "ไม่พบข้อมูลการสมัครของคุณ โปรดตรวจสอบคำค้นหาของคุณอีกครั้ง"
                    : "Data not found, Please try again.";
            }
        }

        public static DataTable ToDataTable<T>(this IList<T> data)
        {
            PropertyDescriptorCollection props =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }
    }
}