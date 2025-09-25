using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Notify;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.UI;
using WBBBusinessLayer;
using WBBContract.Commands;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.Account;
using WBBWeb.CompositionRoot;
using WBBWeb.Extension;
using WBBWeb.Models;
using WBBWeb.Solid.CompositionRoot;
using ZXing;
using ZXing.Common;

namespace WBBWeb.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class WBBController : Controller
    {

        public ILogger Logger { get; set; }

        private List<LovValueModel> _LovData = new List<LovValueModel>();
        private static List<LovValueModel> NULL_LovData = new List<LovValueModel>();
        //private CacheStrigsStack _CacheStrigsStack = new CacheStrigsStack();
        int countReload = 0;
        public List<LovValueModel> LovData
        {
            get
            {

                var masterController = Bootstrapper.GetInstance<MasterDataController>();

                if (null == HttpRuntime.Cache[WebConstants.SessionKeys.AllLov])
                {
                    LoadLOV();
                }
                if (null == HttpRuntime.Cache[WebConstants.SessionKeys.LoadLov])
                {
                    var loadLOVFlag = masterController.GetLoadConfigLov("FBB_CFG_LOV").FLAG.ToSafeString();
                    if (loadLOVFlag == "Y")
                    {
                        loadLOVFlag = "N";
                        masterController.SetLoadConfigLov("FBB_CFG_LOV", "1", GetIPAddress());
                        UpdateLOV();
                    }
                    else if (masterController.GetLoadConfigLov("FBB_CFG_LOV").FLAG2.ToSafeString() == "Y")
                    {
                        masterController.SetLoadConfigLov("FBB_CFG_LOV", "2", GetIPAddress());
                        UpdateLOV();
                    }
                    HttpRuntime.Cache.Insert(
                        WebConstants.SessionKeys.LoadLov,
                        loadLOVFlag,
                        null,
                        DateTime.UtcNow.AddMinutes(30),
                        Cache.NoSlidingExpiration);
                }

                _LovData = HttpRuntime.Cache.Get(WebConstants.SessionKeys.AllLov) as List<LovValueModel>;
                if ((_LovData == null || _LovData.Count == 0 || NULL_LovData == _LovData) && countReload == 0)
                {
                    countReload++;
                    LoadLOV();
                    _LovData = HttpRuntime.Cache.Get(WebConstants.SessionKeys.AllLov) as List<LovValueModel>;
                }

                return _LovData;

            }
            set
            {
                _LovData = value;
            }
        }

        public void LoadLOV()
        {
            var _logger = Bootstrapper.GetInstance<DebugLogger>();
            _logger.Info("Start LoadLOV");
            LineNotify.SendMessage(WebConstants.NotifyKey.LineNotifyFBB, this.GetType().Name, "LoadLOV");
            var masterController = Bootstrapper.GetInstance<MasterDataController>();
            try
            {
                HttpRuntime.Cache.Insert(
                                        WebConstants.SessionKeys.AllLov,
                                        masterController.GetLovList("", ""),
                                        null,
                                        Cache.NoAbsoluteExpiration,
                                        Cache.NoSlidingExpiration);

                //_LovData = masterController.GetLovList("", "").ToList();

            }
            catch (Exception ex)
            {
                _logger.Info("Get value db is null.");
                //_LovData = masterController.GetLovList("", "").ToList();
                LineNotify.SendMessage(WebConstants.NotifyKey.LineNotifyFBB, this.GetType().Name, ex);
            }

        }

        [HttpPost]
        public JsonResult UpdateLOV()
        {
            var _logger = Bootstrapper.GetInstance<DebugLogger>();
            _logger.Info("Call UpdateLOV");
            LineNotify.SendMessage(WebConstants.NotifyKey.LineNotifyFBB, this.GetType().Name, "UpdateLOV");
            var masterController = Bootstrapper.GetInstance<MasterDataController>();
            try
            {
                HttpRuntime.Cache.Insert(
                                        WebConstants.SessionKeys.AllLov,
                                        masterController.GetLovList("", ""),
                                        null,
                                        Cache.NoAbsoluteExpiration,
                                        Cache.NoSlidingExpiration);

                _logger.Info("New LOV Success");
                LineNotify.SendMessage(WebConstants.NotifyKey.LineNotifyFBB, this.GetType().Name, "New LOV Success");
                //_LovData = masterController.GetLovList("", "").ToList();

            }
            catch (Exception ex)
            {
                //_LovData = masterController.GetLovList("", "").ToList();
                LineNotify.SendMessage(WebConstants.NotifyKey.LineNotifyFBB, this.GetType().Name, ex);
            }

            return Json("Success", JsonRequestBehavior.AllowGet);

        }

        public List<ZipCodeModel> ZipCodeData(int currentCulture)
        {
            var masterController = Bootstrapper.GetInstance<MasterDataController>();
            if (null == HttpRuntime.Cache[WebConstants.SessionKeys.ZipCodeData])
            {
                HttpRuntime.Cache.Insert(
                                    WebConstants.SessionKeys.ZipCodeData,
                                    masterController.GetZipCodeList(currentCulture),
                                    null,
                                    DateTime.UtcNow.AddMinutes(30),
                                    Cache.NoSlidingExpiration);
            }

            var zipcodeData = ((List<ZipCodeModel>)HttpRuntime.Cache[WebConstants.SessionKeys.ZipCodeData]);
            if (currentCulture.IsThaiCulture())
                return zipcodeData.Where(t => t.IsThai == true).ToList();
            else
                return zipcodeData.Where(t => t.IsThai == false).ToList();
        }

        public class StringNum : IComparable<StringNum>
        {

            private List<string> _strings;
            private List<int> _numbers;

            public StringNum(string value)
            {
                _strings = new List<string>();
                _numbers = new List<int>();
                int pos = 0;
                bool number = false;
                while (pos < value.Length)
                {
                    int len = 0;
                    while (pos + len < value.Length && Char.IsDigit(value[pos + len]) == number)
                    {
                        len++;
                    }
                    if (number)
                    {
                        _numbers.Add(int.Parse(value.Substring(pos, len)));
                    }
                    else
                    {
                        _strings.Add(value.Substring(pos, len));
                    }
                    pos += len;
                    number = !number;
                }
            }

            public int CompareTo(StringNum other)
            {
                int index = 0;
                while (index < _strings.Count && index < other._strings.Count)
                {
                    int result = _strings[index].CompareTo(other._strings[index]);
                    if (result != 0) return result;
                    if (index < _numbers.Count && index < other._numbers.Count)
                    {
                        result = _numbers[index].CompareTo(other._numbers[index]);
                        if (result != 0) return result;
                    }
                    else
                    {
                        return index == _numbers.Count && index == other._numbers.Count ? 0 : index == _numbers.Count ? -1 : 1;
                    }
                    index++;
                }
                return index == _strings.Count && index == other._strings.Count ? 0 : index == _strings.Count ? -1 : 1;
            }

        }

        // action
        [OutputCache(Location = OutputCacheLocation.None, NoStore = false)]
        public ActionResult ChangeCurrentCulture(int culture)
        {
            SiteSession.CurrentUICulture = culture;
            Session[WebConstants.SessionKeys.CurrentUICulture] = culture;
            if (Request.UrlReferrer != null)
            {
                //return Redirect(Request.UrlReferrer.ToString());

                //Partner
                if (Request.UrlReferrer.ToString().Contains("Partner"))
                {
                    var Show = Session["Partner_Show"];
                    var Ref = Session["Partner_Ref"];
                    if (Show == null && Ref == null)
                    {
                        return Redirect(Request.UrlReferrer.ToString());
                    }
                    else
                    {
                        return Redirect(Request.UrlReferrer.ToString() + "?Show=" + Show + "&Ref=" + Ref);
                    }
                }
                else
                {
                    return Redirect(Request.UrlReferrer.ToString());
                }
            }
            else
            {
                return null;
            }
        }

        [OutputCache(Location = OutputCacheLocation.None, NoStore = false)]
        public int GetCurrentCulture()
        {
            return Convert.ToInt32(Session[WebConstants.SessionKeys.CurrentUICulture].ToString());
        }

        public UserModel CurrentUser
        {
            get { return (UserModel)Session[WebConstants.FBBConfigSessionKeys.User]; }
            set { Session[WebConstants.FBBConfigSessionKeys.User] = value; }
        }

        public LoginPaymentReportProblemModel CurrentSessionPaymentProblemReport
        {
            get
            {
                var objSession =
                    (LoginPaymentReportProblemModel)Session[WebConstants.FBBConfigSessionKeys.UserPaymentPromblemReport];
                return objSession ?? new LoginPaymentReportProblemModel();
            }
            set { Session[WebConstants.FBBConfigSessionKeys.UserPaymentPromblemReport] = value; }
        }

        public PaymentLogCommand CurrentSessionPayment
        {
            get
            {
                var objSession =
                    (PaymentLogCommand)Session[WebConstants.FBBConfigSessionKeys.PaymentLog];
                return objSession ?? new PaymentLogCommand();
            }
            set { Session[WebConstants.FBBConfigSessionKeys.PaymentLog] = value; }
        }

        public static string Encrypt(string textToEncrypt, string key = "")
        {
            try
            {
                if (key == "")
                {
                    var _LovData = HttpRuntime.Cache.Get(WebConstants.SessionKeys.AllLov) as List<LovValueModel>;
                    key = _LovData.Where(t => t.Name == "MY_AIS_KEY").Select(t => t.LovValue1).FirstOrDefault().ToSafeString();
                }

                RijndaelManaged rijndaelCipher = new RijndaelManaged();
                rijndaelCipher.Mode = CipherMode.ECB;
                rijndaelCipher.Padding = PaddingMode.PKCS7;
                rijndaelCipher.KeySize = 0x80;
                rijndaelCipher.BlockSize = 0x80;

                String pass = null;
                pass = padString(key);
                byte[] pwdBytes = Encoding.UTF8.GetBytes(pass);

                rijndaelCipher.Key = pwdBytes;
                rijndaelCipher.IV = pwdBytes;

                ICryptoTransform transform = rijndaelCipher.CreateEncryptor();
                byte[] plainText = Encoding.UTF8.GetBytes(textToEncrypt);
                byte[] encrypt = transform.TransformFinalBlock(plainText, 0, plainText.Length);
                return HexEncodingToString(encrypt);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static string Decrypt(string textToDecrypt, string key = "")
        {
            try
            {

                if (key == "")
                {
                    var _LovData = HttpRuntime.Cache.Get(WebConstants.SessionKeys.AllLov) as List<LovValueModel>;
                    key = _LovData.Where(t => t.Name == "MY_AIS_KEY").Select(t => t.LovValue1).FirstOrDefault().ToSafeString();
                }


                RijndaelManaged rijndaelCipher = new RijndaelManaged();
                rijndaelCipher.Mode = CipherMode.ECB;
                rijndaelCipher.Padding = PaddingMode.PKCS7;
                rijndaelCipher.KeySize = 0x80;
                rijndaelCipher.BlockSize = 0x80;

                byte[] encryptedData = HexToBytes(textToDecrypt);
                //byte[] encryptedData = Convert.FromBase64String(textToDecrypt);
                String pass = null;
                pass = padString(key);
                byte[] pwdBytes = Encoding.UTF8.GetBytes(pass);


                rijndaelCipher.Key = pwdBytes;
                rijndaelCipher.IV = pwdBytes;


                byte[] plainText = rijndaelCipher.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);
                return Encoding.UTF8.GetString(plainText);

            }
            catch (Exception ex)
            {
                return "";
                throw new Exception(ex.Message);
            }
        }

        public static string DecryptStringAES(string textToDecrypt, string keyAES = "")
        {
            var keybytes = Encoding.UTF8.GetBytes(keyAES);
            var iv = Encoding.UTF8.GetBytes(keyAES);

            var encrypted = Convert.FromBase64String(textToDecrypt);
            var decriptedFromJavascript = DecryptStringFromBytes(encrypted, keybytes, iv);
            return string.Format(decriptedFromJavascript);
        }

        private static String padString(String str)
        {
            int slen = (str.Length % 16);
            int i = (16 - slen);
            if ((i > 0) && (i < 16))
            {
                StringBuilder buf = new StringBuilder(str.Length + i);
                buf.Insert(0, str);
                for (i = (16 - slen); i > 0; i--)
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

        private static byte[] HexToBytes(string str)
        {
            if (str.Length == 0 || str.Length % 2 != 0)
                return new byte[0];

            byte[] buffer = new byte[str.Length / 2];
            char c;
            for (int bx = 0, sx = 0; bx < buffer.Length; ++bx, ++sx)
            {
                // Convert first half of byte
                c = str[sx];
                buffer[bx] = (byte)((c > '9' ? (c > 'Z' ? (c - 'a' + 10) : (c - 'A' + 10)) : (c - '0')) << 4);

                // Convert second half of byte
                c = str[++sx];
                buffer[bx] |= (byte)(c > '9' ? (c > 'Z' ? (c - 'a' + 10) : (c - 'A' + 10)) : (c - '0'));
            }

            return buffer;
        }

        private static string HexEncodingToString(byte[] bytes)
        {
            string hexString = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                hexString += bytes[i].ToString("X2");
            }
            return hexString;
        }

        private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Check arguments.  
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException("cipherText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }

            // Declare the string used to hold  
            // the decrypted text.  
            string plaintext = null;

            // Create an RijndaelManaged object  
            // with the specified key and IV.  
            using (var rijAlg = new RijndaelManaged())
            {
                //Settings  
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = key;
                rijAlg.IV = iv;

                // Create a decrytor to perform the stream transform.  
                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                try
                {
                    // Create the streams used for decryption.  
                    using (var msDecrypt = new MemoryStream(cipherText))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {

                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                // Read the decrypted bytes from the decrypting stream  
                                // and place them in a string.  
                                plaintext = srDecrypt.ReadToEnd();

                            }

                        }
                    }
                }
                catch
                {
                    plaintext = "keyError";
                }
            }

            return plaintext;
        }

        private static byte[] EncryptStringToBytes(string plainText, byte[] key, byte[] iv)
        {
            // Check arguments.  
            if (plainText == null || plainText.Length <= 0)
            {
                throw new ArgumentNullException("plainText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            byte[] encrypted;
            // Create a RijndaelManaged object  
            // with the specified key and IV.  
            using (var rijAlg = new RijndaelManaged())
            {
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = key;
                rijAlg.IV = iv;

                // Create a decrytor to perform the stream transform.  
                var encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption.  
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.  
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            // Return the encrypted bytes from the memory stream.  
            return encrypted;
        }

        public string GetIPAddress()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName()); // `Dns.Resolve()` method is deprecated.
            IPAddress ipAddress = ipHostInfo.AddressList[0];

            return ipAddress.ToString();
        }

        public DateTime ConvertTimeSlotToTime(DateTime CerrenDateTime, string TimeSlot)
        {
            string[] TimeSlots = TimeSlot.Split('-');
            if (TimeSlots.Count() > 0)
            {
                CerrenDateTime = DateTime.ParseExact(TimeSlots[0], "HH:mm",
                                                CultureInfo.InvariantCulture);
            }
            return CerrenDateTime;
        }

        public string GenerateQRCode(string url)
        {
            var qrWriter = new BarcodeWriter();
            qrWriter.Format = BarcodeFormat.QR_CODE;
            qrWriter.Options = new EncodingOptions() { Height = 150, Width = 150, Margin = 0 };
            try
            {
                using (var q = qrWriter.Write(url))
                {
                    using (var ms = new MemoryStream())
                    {
                        q.Save(ms, ImageFormat.Png);
                        var img = new TagBuilder("img");
                        img.Attributes.Add("src", String.Format("data:image/png;base64,{0}", Convert.ToBase64String(ms.ToArray())));
                        img.Attributes.Add("alt", url);
                        string result = MvcHtmlString.Create(img.ToString(TagRenderMode.SelfClosing)).ToString();
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                return "";
            }

        }

        public string JwtDecode(string token, string key)
        {
            IJsonSerializer serializer = new JsonNetSerializer();
            var provider = new UtcDateTimeProvider();
            IJwtValidator validator = new JwtValidator(serializer, provider);
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm(); // symmetric
            IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, algorithm);
            var json = decoder.Decode(token, key, verify: true);
            return json;
        }

        public string JwtEncode(Dictionary<string, object> payload, string key)
        {
            //var payload = new Dictionary<string, object>
            //{
            //    { "linetmpid", linetmpid },
            //    { "ch", ch }
            //};
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
            var token = encoder.Encode(payload, key);
            return token;
        }

        public static string DecryptHmacsha256(string encryptedText, string password)
        {
            if (encryptedText == null)
            {
                return null;
            }

            if (password == null)
            {
                password = String.Empty;
            }

            // Get the bytes of the string
            var bytesToBeDecrypted = Convert.FromBase64String(encryptedText);
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesDecrypted = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            var saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);

                    AES.KeySize = 256;
                    AES.BlockSize = 128;
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);
                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        //cs.Close();
                    }

                    bytesDecrypted = ms.ToArray();
                }
            }

            return Encoding.UTF8.GetString(bytesDecrypted);
        }


        public static string hmacsha256(string message, string secret)
        {
            secret = secret ?? "";
            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return Hex(hashmessage);
            }
        }

        private static string Hex(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        [HttpPost]
        public JsonResult getRedirectURL(string location)
        {
            //// var RequestUrl = Request.Url.OriginalString.Replace(Request.Path,$"/{Controllers}/{Actions}");
            var RequestUrl = location;
            return Json(RequestUrl, JsonRequestBehavior.AllowGet);
        }

    }
    public class CacheStrigsStack
    {
        private readonly RedisEndpoint _redisEndpoint;
        public CacheStrigsStack()
        {
            var host = ConfigurationManager.AppSettings["redisHost"].ToString();
            var port = Convert.ToInt32(ConfigurationManager.AppSettings["redisPort"]);
            _redisEndpoint = new RedisEndpoint(host, port);
        }

        public bool IsKeyExists(string key)
        {
            using (var redisClient = new RedisClient(_redisEndpoint))
            {
                if (redisClient.ContainsKey(key))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public void SetStrings(string key, string value)
        {
            using (var redisClient = new RedisClient(_redisEndpoint))
            {
                redisClient.SetValue(key, value);
            }
        }

        public string GetStrings(string key, string value)
        {
            using (var redisClient = new RedisClient(_redisEndpoint))
            {
                return redisClient.GetValue(key);
            }
        }

        public bool StoreList<T>(string key, T value, TimeSpan timeout)
        {
            try
            {
                using (var redisClient = new RedisClient(_redisEndpoint))
                {
                    redisClient.As<T>().SetValue(key, value, timeout);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public T GetList<T>(string key)
        {
            T result;

            using (var client = new RedisClient(_redisEndpoint))
            {
                var wrapper = client.As<T>();

                result = wrapper.GetValue(key);
            }

            return result;
        }

        public long Increment(string key)
        {
            using (var client = new RedisClient(_redisEndpoint))
            {
                return client.Increment(key, 1);
            }
        }

        public long Decrement(string key)
        {
            using (var client = new RedisClient(_redisEndpoint))
            {
                return client.Decrement(key, 1);
            }
        }
    }
}
