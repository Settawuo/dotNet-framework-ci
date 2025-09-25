using AIRNETEntity.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WBBWeb.Extension
{
    public static class DataConvertHelpers
    {
        public static string ToStr(object value)
        {
            try
            {
                return !string.IsNullOrEmpty(value.ToString()) ? Convert.ToString(value).Trim() : string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string ToDisplayFormat_DDMMYYYY(object value)
        {
            try
            {
                if (value == null) return string.Empty;

                var dt = Convert.ToDateTime(value);

                return dt.ToString("dd-MM-yyyy");
            }
            catch
            {
                return string.Empty;
            }
        }

        public static DateTime? ToDate_FormatYYYY_MM_DD(object value)
        {
            try
            {
                if (value == null) return null;

                var strValue = value.ToString().Replace("-", "").Replace("/", "");
                var year = strValue.Substring(0, 4);
                var month = strValue.Substring(4, 2);
                var day = strValue.Substring(6, 2);

                var dt = new DateTime(Convert.ToInt16(year), Convert.ToInt16(month), Convert.ToInt16(day));

                return dt;
            }
            catch
            {
                return null;
            }
        }

        public static string To_FormatDD_MM_YYYY(object value)
        {
            try
            {
                if (value == null) return null;

                var strValue = value.ToString().Replace("-", "").Replace("/", "");
                var year = strValue.Substring(0, 4);
                var month = strValue.Substring(4, 2);
                var day = strValue.Substring(6, 2);

                var strReturn = string.Format("{0}/{1}/{2}", day, month, year);

                return strReturn;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string ToStrCurrencyFormat(object value)
        {
            try
            {
                if (Convert.ToString(value).IsNumeric())
                {
                    var num = Convert.ToDouble(value);
                    return num.ToString("#,##0.00");
                }
                else
                {
                    return string.Empty;
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        public static bool IsNumeric(this string s)
        {
            float output;
            return float.TryParse(s, out output);
        }

        public static List<string> ToDecodeList(object value)
        {
            try
            {
                string[] splitStr = { "<br/>", "<br />" };
                var valueStr = value != null ? HttpUtility.HtmlDecode(value.ToString()) : string.Empty;
                return !string.IsNullOrEmpty(valueStr) ? valueStr.Split(splitStr, StringSplitOptions.None).ToList() : new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }

        public static int ToInt(object value)
        {
            try
            {
                return int.Parse(value.ToString());
            }
            catch (Exception)
            {
                return 0;
            }

        }

        public static DateTime? ToDate(object value)
        {
            try
            {
                return value.ToString().ToDate();
            }
            catch (Exception)
            {
                return null;
            }
        }


        public static string ToStrCurrencyFormatPdf(object value)
        {
            try
            {
                if (Convert.ToString(value).IsNumeric())
                {
                    var num = Convert.ToDouble(value);
                    return num.ToString("#,###.##");
                }
                else
                {
                    return string.Empty;
                }
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
