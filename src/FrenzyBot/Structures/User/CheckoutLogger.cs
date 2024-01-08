using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace FrenzyBot.Structures.User
{
    public static class CheckoutLogger
    {
        private static string API_ENDPOINT = "https://api.projectfrenzy.com";
        public static void PostCheckout(FlashSale.Flashsale Item, string ProductTitle, string CheckoutTime, string ModeName, string Option,int CustomDelay, string ShopName, bool Success, string Message)
        {
            return;
            CheckoutLogStructure Checkout = new CheckoutLogStructure
            {
                Item=Item,
                CheckoutSpeed=CheckoutTime,
                Mode=ModeName,
                Option=Option,
                Success=Success,
                ShopName=ShopName,
                ProductTitle=ProductTitle,
                CustomDelay=CustomDelay,
                Message=Message
            };

            var Param = Encrypt(JsonConvert.SerializeObject(Checkout));
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Auth", Program.FrenzySettings.LicenseKey);
            httpClient.GetAsync($"{API_ENDPOINT}/user/checkout?checkoutData={WebUtility.UrlEncode(Param)}");
        }

        private static string Encrypt(string PlainText)
        {
            RijndaelManaged Algorithm = new RijndaelManaged();
            const int CHUNK_SIZE = 128;

            Algorithm.Mode = CipherMode.CBC;
            Algorithm.Padding = PaddingMode.PKCS7;
            Algorithm.KeySize = 256;
            Algorithm.BlockSize = CHUNK_SIZE;
            Algorithm.Key = Encoding.UTF8.GetBytes("B90FB8FF82666B4199ADCD63E0294B06");
            Algorithm.IV = Encoding.UTF8.GetBytes("413B428A85CF7E2B");

            var CipherBytes = Encoding.UTF8.GetBytes(PlainText);
            ICryptoTransform transform = Algorithm.CreateEncryptor();
            byte[] EncryptedValue = transform.TransformFinalBlock(CipherBytes, 0, CipherBytes.Length);
            return Convert.ToBase64String(EncryptedValue);
        }

    }
}
