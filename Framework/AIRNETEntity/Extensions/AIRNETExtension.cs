using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace AIRNETEntity.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToDateDisplayText(this DateTime? dt)
        {
            return ToDisplayText(dt, Constants.DisplayFormats.DateFormat);
        }

        public static string ToDateDisplayText(this DateTime dt)
        {
            return ToDisplayText(dt, Constants.DisplayFormats.DateFormat);
        }

        public static string ToDisplayText(this DateTime? dt)
        {
            return ToDisplayText(dt, Constants.DisplayFormats.DateTimeFormat);
        }

        public static string ToDisplayText(this DateTime? dt, string format)
        {
            if (!dt.HasValue)
                return "-";

            return dt.Value.ToString(format, CultureInfo.GetCultureInfo("en-US"));
        }

        public static string ToDisplayText(this DateTime dt)
        {
            return ToDisplayText(dt, Constants.DisplayFormats.DateTimeFormat);
        }

        public static string ToDisplayText(this DateTime dt, string format)
        {
            return dt.ToString(format, CultureInfo.GetCultureInfo("en-US"));
        }

        public static string ToDisplayText(this TimeSpan? ts)
        {
            if (!ts.HasValue)
                return "";

            string displayText = ts.Value.ToDisplayText();

            return displayText;
        }

        public static string ToDisplayText(this TimeSpan ts)
        {
            string displayText = "";

            if (ts.Days > 0)
                displayText += string.Format(" {0} วัน", ts.Days.ToString());

            if (ts.Hours > 0)
                displayText += string.Format(" {0} ชั่วโมง", ts.Hours.ToString());

            if (ts.Minutes > 0)
                displayText += string.Format(" {0} นาที", ts.Minutes.ToString());

            return displayText;
        }

        public static DateTime? ToDate(this string dt)
        {
            DateTime date = new DateTime();
            if (DateTime.TryParseExact(dt, Constants.DisplayFormats.DateFormat,
                Constants.DisplayFormats.DefaultCultureInfo,
                System.Globalization.DateTimeStyles.None, out date))

                return date;
            else
                return null;
        }

        public static DateTime? ToDateTime(this string dt)
        {
            DateTime date = new DateTime();
            if (DateTime.TryParseExact(dt, Constants.DisplayFormats.DateTimeFormat,
                Constants.DisplayFormats.DefaultCultureInfo,
                System.Globalization.DateTimeStyles.None, out date))

                return date;
            else
                return null;
        }
    }

    public static class AIRNETExtension
    {
        public static bool IsThaiCulture(this int currentCulture)
        {
            if (currentCulture == 1)
                return true;

            return false;
        }

        public static bool IsEngCulture(this int currentCulture)
        {
            if (currentCulture == 2)
                return true;

            return false;
        }

        private const string fixKey1 = "714D4FC1C5764E078A64683B0A73A085";

        private const string saltValue = "tu89geji340t89u2";

        public static T GetValue<T>(this object o, string fieldName, T returnIfNull)
        {
            var value = typeof(object).GetProperty(fieldName).GetValue(o);
            if (value == null)
                return returnIfNull;
            else
                return (T)value;
        }

        public static T GetValue<T>(this object o, string fieldName, T returnValueIfDBNull, Func<object, T> converter)
        {
            var value = typeof(object).GetProperty(fieldName).GetValue(o);
            if (value == null)
                return returnValueIfDBNull;
            else
                return converter(value);
        }

        public static OracleParameter SetStringValue(this OracleParameter param, string value)
        {
            if (string.IsNullOrEmpty(value)) { param.Value = DBNull.Value; } else { param.Value = value; }
            return param;
        }

        public static string SimpleDecrypt(this string src)
        {
            byte[] salt = Encoding.ASCII.GetBytes(saltValue);
            byte[] cipherTextBytes = Convert.FromBase64String(src);
            PasswordDeriveBytes password = new PasswordDeriveBytes(fixKey1, null);
            byte[] keyBytes = password.GetBytes(32);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, salt);
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
        }

        public static string SimpleEncrypt(this string src)
        {
            byte[] saltBytes = Encoding.UTF8.GetBytes(saltValue);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(src);
            PasswordDeriveBytes password = new PasswordDeriveBytes(fixKey1, null);
            byte[] keyBytes = password.GetBytes(32);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, saltBytes);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] cipherTextBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            return Convert.ToBase64String(cipherTextBytes);
        }

        public static T ToEnum<T>(this object o)
        {
            T enumVal = (T)Enum.Parse(typeof(T), o.ToString());
            return enumVal;
        }

        public static double ToSafeDouble(this double? dou)
        {
            return dou.HasValue ? dou.Value : dou.GetValueOrDefault(0d);
        }

        public static double ToSafeDouble(this string text)
        {
            var value = 0d;
            return double.TryParse(text, out value) ? value : value;
        }

        public static decimal ToSafeDecimal(this decimal? dec)
        {
            return dec.HasValue ? dec.Value : dec.GetValueOrDefault(0m);
        }

        public static decimal ToSafeDecimal(this string text)
        {
            var value = 0m;
            return decimal.TryParse(text, out value) ? value : value;
        }

        public static decimal ToSafeInteger(this int? inte)
        {
            return inte.HasValue ? inte.Value : inte.GetValueOrDefault(0);
        }

        public static int ToSafeInteger(this string text)
        {
            var value = 0;
            return int.TryParse(text, out value) ? value : value;
        }

        public static decimal? ToSafeNullableDecimal(this string text)
        {
            var value = 0m;
            return decimal.TryParse(text, out value) ? value : (decimal?)null;
        }

        public static int? ToSafeNullableInteger(this string text)
        {
            var value = 0;
            return int.TryParse(text, out value) ? value : (int?)null;
        }

        public static string ToSafeString(this object value)
        {
            if (null == value)
            {
                return string.Empty;
            }
            else
            {
                return value.ToString();
            }
        }

        public static bool ToCompareText(this object value, string text)
        {
            return String.Compare(value.ToSafeTrim(), text.ToSafeTrim(), StringComparison.OrdinalIgnoreCase) == 0;
        }

        public static string ToSafeTrim(this object value)
        {
            if (null == value)
            {
                return string.Empty;
            }
            else
            {
                var str = value.ToString().Trim();
                if (str.ToLower() == "null")
                {
                    return string.Empty;
                }
                return str;
            }
        }


        public static bool ToSafeBoolean(this object boolVal)
        {
            bool? x = (bool?)boolVal;
            if (x != null)
                return x.Value;
            else
                return false;
        }

        public static bool ToYesNoFlgBoolean(this string flag)
        {
            if (string.IsNullOrEmpty(flag))
                return false;

            return flag.ToLower().Equals("y") ? true : false;
        }

        public static string ToYesNoFlgString(this bool logic)
        {
            return logic ? "Y" : "N";
        }

        public static void Each<T>(this IEnumerable<T> ie, Action<T, int> action)
        {
            var i = 0;
            foreach (var e in ie) action(e, i++);
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static bool IsActive(this string flag)
        {
            if (string.IsNullOrEmpty(flag))
                return false;

            return flag.ToLower().Equals("y") ? true : false;
        }

        public static bool HaveValue(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            return true;
        }

        public static bool Whatever<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null || !source.Any())
                return false;

            return true;
        }

        #region GUID

        [StructLayout(LayoutKind.Explicit)]
        struct DecimalGuidConverter
        {
            [FieldOffset(0)]
            public decimal Decimal;
            [FieldOffset(0)]
            public Guid Guid;
        }

        private static DecimalGuidConverter _Converter;
        public static Guid DecimalToGuid(decimal dec)
        {
            _Converter.Decimal = dec;
            return _Converter.Guid;
        }

        public static decimal GuidToDecimal(Guid guid)
        {
            _Converter.Guid = guid;
            return _Converter.Decimal;
        }

        public static decimal GenerateId()
        {
            return GuidToDecimal(Guid.NewGuid());
        }

        #endregion

        // todo : รอก่อนเดี๋ยวมาเขียนให้เสร็จ
        //public static void GetAndSetField<T>(List<T> list, string fieldNumber, string fieldValue)
        //    where T : class
        //{
        //    var tempField = list.FirstOrDefault(x => x.FieldNumber.Equals(fieldNumber));
        //    if (tempField != null)
        //        tempField.FieldValue = fieldValue;
        //}
    }

    public static class iWorkflowsException
    {
        public static string GetErrorMessage(this Exception ex)
        {
            return (ex.InnerException == null ?
                ex.Message : ex.InnerException.Message);
        }
    }

    public static class iWorkflowsOracleExtension
    {
        public static void CloseConnection(this OracleConnection conn)
        {
            if (conn.State != ConnectionState.Closed)
                conn.Close();
        }
    }

    public static class DBUtilityExtensions
    {
        /// <summary>
        /// Set string value to parameter, auto set value to DBNull if assigning string is null or empty
        /// </summary>
        /// <param name="param">reference to OracleParameter</param>
        /// <param name="value">assigning string value</param>
        /// <returns></returns>
        public static OracleParameter SetDbStringValue(this OracleParameter param, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                param.Value = DBNull.Value;
            }
            else
            {
                param.Value = value;
            }

            return param;
        }

        public static T GetDbValue<T>(this OracleDataReader reader, string fieldName, T returnValueIfDBNull)
        {
            if (reader[fieldName] == DBNull.Value)
                return returnValueIfDBNull;
            else
                return (T)reader[fieldName];
        }

        public static T GetDbValue<T>(this OracleDataReader reader, string fieldName, T returnValueIfDBNull, Func<object, T> converter)
        {
            if (reader[fieldName] == DBNull.Value)
                return returnValueIfDBNull;
            else
                return converter(reader[fieldName]);
        }
    }

    public static class ObjectCopier
    {
        /// <summary>
        /// http://stackoverflow.com/questions/78536/deep-cloning-objects-in-c-sharp
        /// Perform a deep Copy of the object.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T Clone<T>(T source)
        {
            if (!typeof(T).IsSerializable)
                throw new ArgumentException("The type must be serializable.", "source");

            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }
    }

    public static class LoggMastery
    {
        public static void LogError(MethodBase method, Exception ex, params object[] values)
        {
            ParameterInfo[] parms = method.GetParameters();
            object[] namevalues = new object[2 * parms.Length];

            string msg = "Error in " + method.Name + "(";
            for (int i = 0, j = 0; i < parms.Length; i++, j += 2)
            {
                msg += "{" + j + "}={" + (j + 1) + "}, ";
                namevalues[j] = parms[i].Name;
                if (i < values.Length) namevalues[j + 1] = values[i];
            }
            msg += "exception=" + ex.Message + ")";
            Console.WriteLine(string.Format(msg, namevalues));
        }

        public static string ParameterValues<T>(PropertyInfo[] props, T paras)
        {

            var values = string.Join("",
                        from property in props
                        where property.CanRead
                        select string.Format("<{0}>{1}</{0}>", property.Name,
                            property.GetValue(paras, null)));

            return values;
        }
    }
}
