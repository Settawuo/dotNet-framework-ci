using iTextSharp.text.pdf;
using System;
using System.IO;

namespace WBBWeb.Extension
{
    public class PdfSecurity
    {
        public static byte[] SetPasswordPdf(byte[] dataBytes, string keyPassword)
        {
            //SET PDF PASSWORD
            var pdfPwd = GetPassword(keyPassword ?? string.Empty);
            var pdfbyte = PdfSettingPassword(dataBytes, pdfPwd);

            return pdfbyte;
        }

        public static void WriteFile(string path, byte[] dataBytes)
        {
            if (dataBytes == null) return;
            var f = new FileStream(path, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            f.Write(dataBytes, 0, dataBytes.Length);
            f.Close();
        }

        private static byte[] PdfSettingPassword(byte[] inputBytes, string pdfPassword)
        {
            byte[] resultBytes;

            if (!string.IsNullOrEmpty(pdfPassword))
            {
                using (var input = new MemoryStream(inputBytes))
                {
                    using (var output = new MemoryStream())
                    {
                        var reader = new PdfReader(input);
                        PdfEncryptor.Encrypt(reader, output, true, pdfPassword, pdfPassword,
                            PdfWriter.ALLOW_SCREENREADERS);

                        resultBytes = output.ToArray();
                    }
                }
            }
            else
            {
                resultBytes = inputBytes;
            }

            return resultBytes;

        }

        private static string GetPassword(string contactMobile)
        {
            const int resultLength = 4; //Fixed รหัส Passsword 4 ตัวท้าย
            var result = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(contactMobile))
                {
                    if (contactMobile.Length >= resultLength)
                    {
                        result = contactMobile.Substring(contactMobile.Length - resultLength, resultLength);
                    }
                }
            }
            catch (Exception)
            {
                result = string.Empty;
            }

            return result;
        }
    }
}
