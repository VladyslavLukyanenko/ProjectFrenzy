using FrenzyBot.Structures.CheckoutPayload;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net;
using FrenzyBot.Structures.Discord;
using FrenzyBot.Structures.FlashSale;

namespace FrenzyBot.Structures.User
{
    public class FrenzyTask
    {
        private long Variant;
        private AndroidResponse.AndroidResponse AndroidResponse;
        private AndroidPayload AndroidPayload;
        private CheckoutPayload.CheckoutPayload Payload;
        public int TaskId;
        private Flashsale Item;
        public int Mode;
        private string ModeName;
        public string Option;
        private Random random = new Random();
        public string ProductTitle;
        private bool Started = false;
        private string ImageUrl = "https://cdn.discordapp.com/attachments/682673305995575370/685699712086441993/ProjectFrenzy.png";
        private string ShopName;
        private string CheckoutTime = "";
        private int CustomDelay;
        private bool UseProxy;
        private int ProductIndex;
        private string Email;
        public string[] SelectedOptions;

        public FrenzyTask(int TaskId, AndroidPayload AndroidPayload, Flashsale Item, int Mode, int ProductIndex, string ProductName, int customDelay, bool UseProxy, string email, string[] Options)
        {
            this.TaskId = TaskId;
            this.AndroidPayload = AndroidPayload;
            this.Item = Item;
            this.Mode = Mode;
            this.ProductIndex = ProductIndex;
            this.UseProxy = UseProxy;
            ProductTitle = ProductName;
            ShopName = Item.Shop.Name;
            CustomDelay = customDelay;
            Email = email;
            SelectedOptions = Options;
        }

        private async Task<long> GetVariant()
        {
            ModeName = Program.modes[Mode - 1].Split(":")[0].ToLower();

            var Products = await Endpoints.GetProducts(Item.Password);
            while(Products == null)
            {
                await Task.Delay(100);
                Products = await Endpoints.GetProducts(Item.Password);
            }

            if (Products[ProductIndex].Images.Count > 0)
                ImageUrl = Products[ProductIndex].Images[0].Src;
            ProductTitle = Products[ProductIndex].Title;
            int VariantCount = Products[ProductIndex].Variants.Count;

            if (ModeName == "random")
            {
                for (int retries = 0; retries < 50; retries++)
                {
                    int Index = random.Next(VariantCount - 1);
                    if (Products[ProductIndex].Variants[Index].InventoryQuantity > 0)
                    {
                        Option = Products[ProductIndex].Variants[Index].Title;
                        return Products[ProductIndex].Variants[Index].ShopifyVariantId;
                    }
                }
            }
            else if(ModeName == "onlysize")
            {
                foreach (var SelectedOption in SelectedOptions)
                {
                    foreach (var _Variant in Products[ProductIndex].Variants)
                    {
                        if (_Variant.Title.ToLower().Contains(SelectedOption.ToLower().Trim()))
                        {
                            if (_Variant.InventoryQuantity <= 0)
                                break;
                            Option = _Variant.Title;
                            return _Variant.ShopifyVariantId;
                        }
                    }
                }
            }
            else if(ModeName == "preference")
            {
                foreach (var SelectedOption in SelectedOptions)
                {
                    foreach (var _Variant in Products[ProductIndex].Variants)
                    {
                        if (_Variant.Title.ToLower().Contains(SelectedOption.ToLower().Trim()))
                        {
                            if (_Variant.InventoryQuantity <= 0)
                                break;
                            Option = _Variant.Title;
                            return _Variant.ShopifyVariantId;
                        }
                    }
                }
                for (int retries = 0; retries < 50; retries++)
                {
                    int Index = random.Next(VariantCount - 1);
                    if (Products[ProductIndex].Variants[Index].InventoryQuantity > 0)
                    {
                        Option = Products[ProductIndex].Variants[Index].Title;
                        return Products[ProductIndex].Variants[Index].ShopifyVariantId;
                    }
                }
            }

            return 0L;
        }

        public async Task<AndroidResponse.AndroidResponse> GetToken()
        {
            HttpClient Client = new HttpClient();
            string LanIP = NetworkScanner.Emulator;

            Client.Timeout = TimeSpan.FromSeconds(60*20);
            try
            {
                await Task.Delay(300);
                Logger.Log($"Task {TaskId} - Click continue on the emulator to get the token! (you have 30 seconds)", ConsoleColor.Yellow);
                var Body = Program.AndroidEncryption.Encrypt(JsonConvert.SerializeObject(AndroidPayload));
                var AndroidResp = await Client.GetAsync($"{LanIP}/payload/{Body}");
                var Content = await AndroidResp.Content.ReadAsStringAsync();
                var DecryptedContent = await Decrypt(Content);
                return JsonConvert.DeserializeObject<AndroidResponse.AndroidResponse>(DecryptedContent);
            }
            catch
            {
                Logger.Log($"Task {TaskId} - Failed Getting Token. Retrying.", ConsoleColor.Red);
                return null;
            }
        }

        private async Task<string> Decrypt(string Content)
        {
            HttpClient Client = new HttpClient();
            Client.DefaultRequestHeaders.Add("Auth", Program.FrenzySettings.LicenseKey);
            var Param = WebUtility.UrlEncode(Content);
            var Response = await Client.GetAsync($"https://api.projectindustries.gg/frenzy/api/{Param}");
            var ResponseContent = await Response.Content.ReadAsStringAsync();
            return ResponseContent;
        }

        private void ConstructCheckoutPayload()
        {
            Dropzone spoofer = null;
            if (Item.Dropzone.Count > 0 )
                spoofer = new Spoofer(Item.Dropzone).Spoof();
            Payload = new CheckoutPayload.CheckoutPayload
            {
                AccelerometerData = new Data { X = 1, Y = 1, Z = 1 },
                Email = Email,
                FlashsalePassword = Item.Password,
                ShopId = Item.Shop.Id,
                GyroData = new Data { X = 1, Y = 1, Z = 1 },
                LastDigits = AndroidResponse.Card,
                LineItems = new List<LineItem>(new LineItem[] { new LineItem { Quantity = 1, VariantId = Variant } }),
                Location = new Location
                {
                    Altitude = "0.0",
                    Course = "0.0",
                    Lat = Item.Dropzone.Count == 0 ? 0 : spoofer.Lat,
                    Lng = Item.Dropzone.Count == 0 ? 0 : spoofer.Lng,
                    Speed = "0.0"
                },
                Payment = new Payment
                {
                    Source = new Source
                    {
                        PaymentToken = new PaymentToken
                        {
                            PaymentData = AndroidResponse.Token,
                            Type = "google_pay"
                        }
                    }
                },
                BillingAddress = AndroidResponse.Address.Billing,
                ShippingAddress = AndroidResponse.Address.Shipping
            };
            if(string.IsNullOrWhiteSpace(Payload.BillingAddress.City))
            {
                Payload.BillingAddress.City = "ABCD";
            }
            if (string.IsNullOrWhiteSpace(Payload.ShippingAddress.City))
            {
                Payload.ShippingAddress.City = "ABCD";
            }
            if (string.IsNullOrWhiteSpace(Payload.BillingAddress.ProvinceCode))
            {
                Payload.BillingAddress.ProvinceCode = "AA";
            }
            if (string.IsNullOrWhiteSpace(Payload.ShippingAddress.ProvinceCode))
            {
                Payload.ShippingAddress.ProvinceCode = "AA";
            }
        }

        private HttpClient InitializeClient()
        {
            HttpClientHandler Handler = new HttpClientHandler();

            if (UseProxy)
            {
                Handler.Proxy = ProxyManager.GetUnusedProxy();
                if (Handler.Proxy != null)
                    Handler.UseProxy = true;
                else
                {
                    Handler.UseProxy = false;
                    Logger.Log($"Task {TaskId} - No unused proxies available. No proxy will be used.", ConsoleColor.Yellow);
                }
            }

            HttpClient Client = new HttpClient(Handler);
            Client.DefaultRequestHeaders.Clear();
            Client.DefaultRequestHeaders.Add("User-Agent", "okhttp/3.8.0");
            Client.DefaultRequestHeaders.Add("Accept", "application/json");
            Client.DefaultRequestHeaders.Add("Accept-Language", "en-US");
            return Client;
        }
        public async Task Start()
        {
            if (Started)
                return;

            Started = true;
            //get token from harvester
            GetToken:
            AndroidResponse = await GetToken();
            if (AndroidResponse == null)
                goto GetToken;
            Logger.Log($"Task {TaskId} - Received Token!", ConsoleColor.Green);
            //create checkout payload
            ConstructCheckoutPayload();
            //init http client used for checkout requests
            HttpClient Client = InitializeClient();

            var RootPayload = new CheckoutPayloadRoot
            {
                Checkout = Payload
            };


            if (Item.Upcoming)
            {
                var DelayTime = (Item.StartedAt - TimeSpan.FromMilliseconds(3000)) - DateTime.Now;
                if (DelayTime > TimeSpan.FromMilliseconds(0))
                {
                    Logger.Log($"Task {TaskId} will wait {DelayTime.Days} days {DelayTime.Hours} hours {DelayTime.Minutes} minutes {DelayTime.Seconds} seconds", ConsoleColor.Yellow);
                    await Task.Delay(DelayTime);
                }
            }
            Variant = await GetVariant();
            if (Variant == 0L)
            {
                Logger.Log($"Task {TaskId} - Can't find size", ConsoleColor.Red);
                return;
            }
            Logger.Log($"Task {TaskId} - Found Size", ConsoleColor.Yellow);
            var watch = new Stopwatch();
            watch.Start();

            RootPayload.Checkout.LineItems[0].VariantId = Variant;

            //create checkout body from root payload
            var CheckoutPostBody = new StringContent(JsonConvert.SerializeObject(RootPayload), Encoding.UTF8, "application/json");

            if (DateTime.Now < Item.StartedAt)
            {
                var DelayTime = Item.StartedAt - DateTime.Now;
                await Task.Delay(DelayTime);
            }
            await Task.Delay(TimeSpan.FromMilliseconds(CustomDelay));
            //start stopwatch for logging checkout time, submit checkout, and stop watch
            Logger.Log($"Task {TaskId} - Submitting Checkout", ConsoleColor.DarkYellow);

            SubmitCheckout:
            HttpResponseMessage CheckoutResponse;
            try
            {
                CheckoutResponse = await Client.PostAsync("https://frenzy.shopifyapps.com/api/checkouts", CheckoutPostBody);
            }
            catch
            {
                InitializeClient();
                goto SubmitCheckout;
            }
            if(!CheckoutResponse.IsSuccessStatusCode)
            {
                goto SubmitCheckout;
            }

            watch.Stop();
            CheckoutTime = $"{watch.Elapsed.Seconds}.{watch.Elapsed.Milliseconds.ToString("D3")} seconds";


            Logger.Log($"Task {TaskId} - Submitted Checkout in {CheckoutTime}.", ConsoleColor.Yellow);

            try
            {
                var CheckoutResponseContent = await CheckoutResponse.Content.ReadAsStringAsync();
                var JObjectResponse = JObject.Parse(CheckoutResponseContent);
                var CheckoutUUID = JObjectResponse["checkout"]["uuid"].ToString();

                while (true)
                {
                    try
                    {
                        var PollResponse = await Client.GetAsync($"https://frenzy.shopifyapps.com/api/checkouts/{CheckoutUUID}");
                        var PollContent = JObject.Parse(await PollResponse.Content.ReadAsStringAsync())["checkout"];
                        if (PollContent["status"].ToString() == "error")
                        {
                            CheckoutLogger.PostCheckout(Item, ProductTitle, CheckoutTime, ModeName, Option, CustomDelay, ShopName, false, PollContent["normalized_errors"].ToString() ?? "Unknown error");
                            WebhookSender.SendFailedCheckoutWebhook(ProductTitle, ModeName, Email, CustomDelay, Option, ShopName, CheckoutTime, PollContent["normalized_errors"].ToString() ?? "Unknown error", ImageUrl);
                            Logger.Log($" Task {TaskId} - Payment Failed: {(PollContent["normalized_errors"] ?? "Unknown error")}", ConsoleColor.Red);
                            return;
                        }
                        else if (PollContent["status"].ToString() == "out_of_stock")
                        {
                            CheckoutLogger.PostCheckout(Item, ProductTitle, CheckoutTime, ModeName, Option, CustomDelay, ShopName, false, "Out of Stock");
                            WebhookSender.SendFailedCheckoutWebhook(ProductTitle, ModeName, Email, CustomDelay, Option, ShopName, CheckoutTime, "Out of Stock", ImageUrl);
                            Logger.Log($" Task {TaskId} - Checkout Failed - Out of Stock", ConsoleColor.Red);
                            return;
                        }
                        else if (PollContent["status"].ToString() == "success")
                        {
                            CheckoutLogger.PostCheckout(Item, ProductTitle, CheckoutTime, ModeName, Option, CustomDelay, ShopName, true, "Success");
                            WebhookSender.SendSuccessfulCheckoutWebhook(ProductTitle, ModeName, Email, CustomDelay, Option, ShopName, CheckoutTime, PollContent["total_price"].ToString(), ImageUrl);
                            Logger.Log($"Task {TaskId} - Payment Completed: Checkout Success", ConsoleColor.Green);
                            return;
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }
    }
}
