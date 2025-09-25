using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Xml.Serialization;

namespace WBBEntity.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToDateDisplayText(this DateTime? dt)
        {
            return ToDisplayText(dt, Constants.DisplayFormats.DateFormat);
        }

        /// <summary>
        /// Convet dateime to string format "dd/MM/yyyy HH:mm"
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToDateTimeDisplayText(this DateTime? dt)
        {
            return ToDisplayText(dt, Constants.DisplayFormats.DateTimeFormatNoSecond);
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

        public static DateTime? ToFBSSDate(this string dt)
        {
            DateTime date = new DateTime();
            if (DateTime.TryParseExact(dt, Constants.FBSSDateFormats.DateFormat,
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

        public static DateTime? ToDateNotTime(this string dt)
        {
            DateTime date;
            if (DateTime.TryParseExact(dt, Constants.DisplayFormats.DateTimeFormat,
                Constants.DisplayFormats.DefaultCultureInfo,
                DateTimeStyles.None, out date))
            {
                return date.Date;
            }
            return null;
        }

        public static string ToDisplayThaiDate(this string dt)
        {
            dt = dt.Substring(0, 10);
            var year = dt.Substring(6, 4);
            var switch_on = dt.Substring(3, 2);
            dt = dt.Replace(year, ((int.Parse(year)) + 543).ToString());
            switch (switch_on)
            {
                case "01":
                    dt = dt.Replace($"/01/", " มกราคม ");
                    break;
                case "02":
                    dt = dt.Replace($"/02/", " กุมภาพันธ์ ");
                    break;
                case "03":
                    dt = dt.Replace($"/03/", " มีนาคม ");
                    break;
                case "04":
                    dt = dt.Replace($"/04/", " เมษายน ");
                    break;
                case "05":
                    dt = dt.Replace($"/05/", " พฤษภาคม ");
                    break;
                case "06":
                    dt = dt.Replace($"/06/", " มิถุนายน ");
                    break;
                case "07":
                    dt = dt.Replace($"/07/", " กรกฎาคม ");
                    break;
                case "08":
                    dt = dt.Replace($"/08/", " สิงหาคม ");
                    break;
                case "09":
                    dt = dt.Replace($"/09/", " กันยายน ");
                    break;
                case "10":
                    dt = dt.Replace($"/10/", " ตุลาคม ");
                    break;
                case "11":
                    dt = dt.Replace($"/11/", " พฤศจิกายน ");
                    break;
                case "12":
                    dt = dt.Replace($"/12/", " ธันวาคม ");
                    break;
            }


            return dt;
        }


        public static bool IsDateStringCorrectedFormat(this string dateString, string format)
        {
            var dateTime = new DateTime();
            return DateTime.TryParseExact(dateString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime);
        }
    }

    public static class WBBExtensions
    {
        public static int ToCultureCode(this string langCode)
        {
            if (string.IsNullOrEmpty(langCode))
            {
                return 1;
            }

            if (langCode == "THA" || langCode.Contains("T"))
            {
                return 1;
            }


            return 2;
        }

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

        public static object GetPropValue(object src, string propName)
        {
            var thing = src.GetType().GetProperty(propName);

            if (null == thing)
                return null;

            return thing.GetValue(src, null);
        }

        public static string DumpToXml<T>(this T value)
        {
            if (value == null)
            {
                return "";
            }
            try
            {
                var settings = new System.Xml.XmlWriterSettings();
                settings.Encoding = new UnicodeEncoding(false, false); // no BOM in a .NET string
                settings.Indent = false;
                settings.OmitXmlDeclaration = false;

                var xmlserializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                var stringWriter = new StringWriter();

                var writer = System.Xml.XmlWriter.Create(stringWriter, settings);

                xmlserializer.Serialize(writer, value);

                var serializeXml = stringWriter.ToString();

                writer.Close();
                return serializeXml;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static object DeserializedToObj<T>(this string value)
        {
            if (value == null)
            {
                return null;
            }
            try
            {
                var serializer = new XmlSerializer(typeof(T));

                using (TextReader reader = new StringReader(value))
                {
                    var result = (T)serializer.Deserialize(reader);
                    return result;
                }
            }
            catch (Exception)
            {
                return null;
            }
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

        public static string ToUnicodeString(this string s)
        {
            var sb = new StringBuilder();
            foreach (char c in s)
            {
                sb.Append("\\u");
                sb.Append(String.Format("{0:x4}", (int)c));
            }
            return sb.ToString();
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

        public static bool IsOnTopPack(this string packtype)
        {
            if (packtype == "Ontop")
                return true;

            return false;
        }

        public static bool IsMainPack(this string packtype)
        {
            if (packtype == "Main")
                return true;

            return false;
        }

        public static bool IsSWiFi(this string tech)
        {
            if (tech == "SWiFi")
                return true;

            return false;
        }

        public static bool IsFTTH(this string tech)
        {
            if (tech == "FTTH")
                return true;

            return false;
        }

        public static bool IsFTTx(this string tech)
        {
            if (tech == "FTTx")
                return true;

            return false;
        }

        public static bool IsVDSL(this string tech)
        {
            if (tech == "VDSL")
                return true;

            return false;
        }

        public static bool IsNonInternetPackage(this string productSubtype)
        {
            var nonInternetProduct = new List<string> { "PBOX" };
            if (nonInternetProduct.Contains(productSubtype))
                return true;

            return false;
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

        public static string ObjectToString(this object obj)
        {
            var result = "";

            foreach (var item in obj.GetType().GetProperties())
            {
                if (result == "")
                    result = item.GetValue(obj).ToSafeString();
                else
                    result = result + "," + item.GetValue(obj).ToSafeString();
            }

            return result;
        }

        public static string CompareObjectToString(object oldObject, object newObject)
        {
            var result = "";

            PropertyInfo[] props = Type.GetType(oldObject.GetType().FullName).GetProperties();

            foreach (PropertyInfo prop in props)
            {
                object[] attrs = prop.GetCustomAttributes(true);
                foreach (object attr in attrs)
                {
                    DisplayNameAttribute displayAttr = attr as DisplayNameAttribute;
                    if (displayAttr != null)
                    {
                        var oldVal = oldObject.GetType().GetProperty(prop.Name).GetValue(oldObject, null);
                        var newVal = newObject.GetType().GetProperty(prop.Name).GetValue(newObject, null);

                        if (oldVal.ToSafeString() != newVal.ToSafeString())
                        {
                            if (result == "")
                                result = displayAttr.DisplayName + ": " + oldVal + " to " + newVal;
                            else
                                result += ", " + displayAttr.DisplayName + ": " + oldVal + " to " + newVal;
                        }
                    }
                }
            }

            //for (int i = 0; i < oldObj.Count(); i++)
            //{
            //    var oldVal = oldObj[i].GetValue(oldObject);
            //    var newVal = newObj[i].GetValue(newObject);

            //    if (oldVal.ToSafeString() != newVal.ToSafeString())
            //    {
            //        var objName = oldObj[i].Name.ToUpper();

            //        if (objName != "CREATED_BY" && objName != "UPDATED_BY" && objName != "CREATED_DATE" && objName != "UPDATED_DATE")
            //        {
            //            if (result == "")
            //                result = oldObj[i].Name + ": " + oldVal + " to " + newVal;
            //            else
            //                result = result + ", " + oldObj[i].Name + ": " + oldVal + " to " + newVal;
            //        }
            //    }
            //} 

            return result;
        }

        public static T ParseEnum<T>(this string value)
        {
            if (string.IsNullOrEmpty(value))
                value = "";

            return (T)Enum.Parse(typeof(T), value, true);
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

        public static IEnumerable<T> getListNonDuplicated<T>(this IEnumerable<T> extList, Func<T, object> groupProps) where T : class
        {
            return extList
                .GroupBy(groupProps)
                .SelectMany(z => z.Take(1));
        }

        public static IEnumerable<T> getMoreThanOnceRepeated<T>(this IEnumerable<T> extList, Func<T, object> groupProps) where T : class
        { //Return only the second and next reptition
            return extList
                .GroupBy(groupProps)
                .SelectMany(z => z.Skip(1)); //Skip the first occur and return all the others that repeats
        }

        public static List<T> ConvertDataTable<T>(this DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }

        public static TResult GetItem<TResult>(DataRow dr)
        {

            PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(TResult));
            string fieldName = string.Empty;

            TResult obj = Activator.CreateInstance<TResult>();
            for (int i = 0; i < dr.Table.Columns.Count; i++)
            {
                fieldName = dr.Table.Columns[i].ColumnName;
                var raw = dr[fieldName];
                if (raw != DBNull.Value)
                {
                    PropertyDescriptor pd = pdc.Find(fieldName, true);
                    if (pd != null)
                    {

                        var value = dr[fieldName];
                        Type tagettype = Nullable.GetUnderlyingType(pd.PropertyType) ?? pd.PropertyType;
                        pd.SetValue(obj, Convert.ChangeType(value, tagettype));
                    }
                }
            }
            return obj;
        }
    }

    public static class iWorkflowsException
    {
        public static string GetErrorMessage(this Exception ex)
        {
            return (ex.InnerException == null ?
                ex.Message : ex.InnerException.Message);
        }

        /// <summary>
        /// Renders an exception with messages and stack traces
        /// </summary>
        /// <param name="ex">The exception to render</param>    
        /// <param name="noTrace">Whether trace information should be omitted</param>
        /// <returns>A string containing messages and stack traces</returns>
        public static string RenderExceptionMessage(this Exception ex, bool noTrace = false)
        {
            var s = new StringBuilder("\n");
            int i = 0;
            do
            {
                s.AppendFormat("{0:#\\. inner E;;E}xception ({1}):\n   {2}\n",
                    i++,
                    ex.GetType().Name,
                    ex.Message.Replace("\n", "\n   "));
                if (ex is UpdateException)
                {
                    foreach (var stateEntry in ((UpdateException)ex).StateEntries)
                    {
                        var entity = stateEntry.Entity ?? new object();
                        s.AppendFormat("     {0} {1}: {2}\n", stateEntry.State, entity.GetType().Name, entity);
                        var values =
                            stateEntry.State == EntityState.Deleted
                                ? stateEntry.OriginalValues
                                : stateEntry.CurrentValues;
                        for (int j = 0; j < values.FieldCount; j++)
                        {
                            var currentValue = values[j];
                            var originalValue =
                                stateEntry.State == EntityState.Added
                                    ? currentValue
                                    : stateEntry.OriginalValues[j];
                            s.AppendFormat(originalValue.Equals(currentValue) ? "      {0}: <{1}>\n" : "      {0}: <{1}>→<{2}\n",
                                           values.GetName(j), originalValue, currentValue);
                        }
                    }
                }
                s.AppendFormat(noTrace ? "\n" : "Trace:\n{0}\n", ex.StackTrace);
                ex = ex.InnerException;
            }
            while (ex != null);
            return s.ToString();
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

        public static List<T> DataTableToList<T>(this DataTable table) where T : class, new()
        {
            try
            {
                List<T> list = new List<T>();

                foreach (var row in table.AsEnumerable())
                {
                    T obj = new T();

                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        try
                        {
                            PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                            propertyInfo.SetValue(obj, Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                        }
                        catch
                        {
                            continue;
                        }
                    }

                    list.Add(obj);
                }

                return list;
            }
            catch
            {
                return null;
            }
        }

        public static List<T> Separate<T>(this List<object> obj, ref string ret_code, ref string ret_msg)
        {
            List<T> data = new List<T>();
            for(int i =0; i< obj.Count; i++)
            {
                var result = (DataTable)obj[i];
                if(result.TableName.Contains("code") || 
                    result.TableName.Contains("msg") ||
                    result.TableName.Contains("multi_ret_cur"))
                {
                    ret_code = result.Rows[0][0].ToString();
                    ret_msg = result.Rows[0][0].ToString();
                }else
                {
                    data = result.ConvertDataTable<T>();
                }
            }
            return data;

        }

        public static List<T> Separate<T>(this object obj)
        {
            var result = (DataTable)obj;
            return result.ConvertDataTable<T>();
        }
    }

    public static class EncryptionUtility
    {
        private static RijndaelManaged GetRijndaelManaged(String secretKey)
        {
            var keyBytes = new byte[32];
            var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
            Array.Copy(secretKeyBytes, keyBytes, Math.Min(keyBytes.Length, secretKeyBytes.Length));
            return new RijndaelManaged
            {
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7,
                KeySize = 256,
                BlockSize = 256,
                Key = keyBytes,
                IV = keyBytes
            };
        }

        private static byte[] Encrypt(byte[] plainBytes, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateEncryptor()
                .TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        }

        private static byte[] Decrypt(byte[] encryptedData, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateDecryptor()
                .TransformFinalBlock(encryptedData, 0, encryptedData.Length);
        }

        public static String Encrypt(String plainText, String key)
        {
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(Encrypt(plainBytes, GetRijndaelManaged(key)));
        }

        public static String Decrypt(String encryptedText, String key)
        {
            var encryptedBytes = Convert.FromBase64String(encryptedText);
            return Encoding.UTF8.GetString(Decrypt(encryptedBytes, GetRijndaelManaged(key)));
        }

        public static string DecryptBase64(string encryptedText, string key)
        {
            string results = "";
            if (!string.IsNullOrEmpty(encryptedText))
            {
                try
                {
                    byte[] src = Convert.FromBase64String(encryptedText);
                    RijndaelManaged aes = new RijndaelManaged();
                    byte[] keyy = Encoding.ASCII.GetBytes(key);
                    aes.KeySize = 256;
                    aes.BlockSize = 128;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.Mode = CipherMode.ECB;
                    using (ICryptoTransform decrypt = aes.CreateDecryptor(keyy, null))
                    {
                        byte[] dest = decrypt.TransformFinalBlock(src, 0, src.Length);
                        decrypt.Dispose();
                        return Encoding.UTF8.GetString(dest);
                    }

                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            return results;


        }

        public static string EncryptBase64(string textToEncrypt, string key)
        {
            string results = "";
            if (!string.IsNullOrEmpty(textToEncrypt))
            {
                try
                {
                    //if (string.IsNullOrEmpty(key))
                    //   key = GetCryptoKey();


                    var aes = GetCryptographer();
                    string pass = null;
                    pass = padString(key);
                    byte[] keyBytes = ASCIIEncoding.ASCII.GetBytes(pass.PadLeft(32));
                    //byte[] ivBytes = ASCIIEncoding.ASCII.GetBytes(pass.PadLeft(16));

                    aes.Key = keyBytes;

                    //aes.IV = Encoding.ASCII.GetBytes(key);
                    //aes.IV = ivBytes;
                    ;

                    ICryptoTransform transform = aes.CreateEncryptor();
                    byte[] plainText = Encoding.UTF8.GetBytes(textToEncrypt);
                    byte[] encrypt = transform.TransformFinalBlock(plainText, 0, plainText.Length);

                    results = Convert.ToBase64String(encrypt);


                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            return results;
        }

        private static string padString(string str)
        {
            int slen = str.Length % 32;
            int i = 32 - slen;
            if (i > 0 && i < 32)
            {
                StringBuilder buf = new StringBuilder(str.Length + i);
                buf.Insert(0, str);
                for (i = 32 - slen; i > 0; i--)
                {
                    buf.Append(" ");
                }
                return buf.ToString();
            }
            else
            {
                return str;
            }
        }
        private static SymmetricAlgorithm GetCryptographer()
        {
            var aes = Aes.Create("AesManaged");
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            aes.KeySize = 256;
            aes.BlockSize = 256;

            return aes;
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static String Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}