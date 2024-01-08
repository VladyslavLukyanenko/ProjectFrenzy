using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FrenzyBot.Cryptography
{
    public class AES
    {
        private string Password;

        public AES(string Password)
        {
            this.Password = Password;
        }

        public string Encrypt(string str)
        {
            AesCryptoServiceProvider aesCryptoProvider = new AesCryptoServiceProvider();

            byte[] byteBuff;

            try
            {
                aesCryptoProvider.Key = Encoding.UTF8.GetBytes(Password);

                aesCryptoProvider.GenerateIV();
                aesCryptoProvider.IV = aesCryptoProvider.IV;
                byteBuff = Encoding.UTF8.GetBytes(str);

                byte[] encoded = aesCryptoProvider.CreateEncryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length);

                string ivHexString = ToHexString(aesCryptoProvider.IV);
                string encodedHexString = ToHexString(encoded);


                return ivHexString + ':' + encodedHexString;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public string Decrypt(string encodedStr)
        {
            AesCryptoServiceProvider aesCryptoProvider = new AesCryptoServiceProvider();

            byte[] byteBuff;

            try
            {
                aesCryptoProvider.Key = Encoding.UTF8.GetBytes(Password);


                string[] textParts = encodedStr.Split(':');
                byte[] iv = FromHexString(textParts[0]);
                aesCryptoProvider.IV = iv;
                byteBuff = FromHexString(textParts[1]);

                string plaintext = Encoding.UTF8.GetString(aesCryptoProvider.CreateDecryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));

                return plaintext;

            }
            catch
            {
                return null;
            }
        }

        public string ToHexString(byte[] str)
        {
            var sb = new StringBuilder();

            var bytes = str;
            foreach (var t in bytes)
            {
                sb.Append(t.ToString("X2"));
            }

            return sb.ToString();
        }

        public byte[] FromHexString(string hexString)
        {
            var bytes = new byte[hexString.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            return bytes;
        }
    }
}