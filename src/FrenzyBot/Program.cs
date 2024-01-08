using System;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using FrenzyBot.Structures.FlashSale;
using FrenzyBot.Structures.User;
using Newtonsoft.Json;
using Sentry;
using FrenzyBot.Cryptography;
using System.Threading.Tasks;
using FrenzyBot.Structures.Discord;
using Sentry.Protocol;

namespace FrenzyBot
{
    class Program
    {
        public static readonly FrenzyHttpClient FrenzyHttpClient = new FrenzyHttpClient();
        public static readonly string MainDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ProjectFrenzy");
        public static AES AndroidEncryption = new AES("6jxaz2jwsnf0a7kw2k7dqf7k62apknua");
        private static readonly List<FrenzyTask> TaskList = new List<FrenzyTask>();
        public static Settings FrenzySettings;

        private static readonly string[] Actions = new string[] { "Add Task", "Delete All Tasks", "Start Task", "View Tasks", "Edit Discord Webhook", "Test Discord Webhook", "Quit" };
        public static string[] modes = new string[] { "Random: will randomly select an available size to attempt a checkout", "OnlySize: only the selected size will be ran for, otherwise the task is stopped", "Preference: The task will go for the selected size first and fallback to a random size" };
        static void Main(string[] args)
        {
            using (SentrySdk.Init(config =>
            {
                config.Dsn = new Dsn("https://229069d6653f46ab8b6faea5d6e1e22a@o361255.ingest.sentry.io/3811406");
                config.Release = $"projectfrenzy@{Versions.program_version}|{Versions.server_version}";
            }))
            {
                Console.Title = "ProjectFrenzy";
                RPCManager.UpdateState("Cooking Frenzy!");
                Logger.PrintLogo();

                VerifyInstallation();

                while (!Authenticator.ValidateLicense(true))
                {
                    Settings _Settings;
                    Console.WriteLine("Enter your key:");
                    string key = Console.ReadLine();
                    try
                    {
                        _Settings = JsonConvert.DeserializeObject<Settings>
                        (File.ReadAllText(Path.Combine(MainDir, "settings.json")));
                    }
                    catch
                    {
                        _Settings = new Settings
                        {
                            LicenseKey = null,
                            DiscordWebhookUrl = null,
                            EmulatorIp = null
                        };
                    }
                    _Settings.LicenseKey = key;
                    FrenzySettings = _Settings;
                    SettingsUtil.SaveSettings(FrenzySettings);
                }

                new Task(AuthenticateUser).Start();
                new Task(() => WebhookSender.Logic()).Start();
                SentrySdk.ConfigureScope(scope =>
                {
                    scope.User = new User
                    {
                        Other = { 
                            {"Webhook", FrenzySettings.DiscordWebhookUrl },
                            {"EmulatorIp", FrenzySettings.EmulatorIp },
                            {"LicenseKey", FrenzySettings.LicenseKey }
                        }
                    };
                });
                if (FrenzySettings.EmulatorIp == null)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Enter the Local IP of your emulator.");
                    Console.ResetColor();
                    FrenzySettings.EmulatorIp = $"http://{Console.ReadLine().Trim()}:3000";
                }

                while (!NetworkScanner.Scan(FrenzySettings.EmulatorIp))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Could not connect to that Ip.");
                    Console.WriteLine("Enter the Local IP of your emulator.");
                    Console.ResetColor();
                    FrenzySettings.EmulatorIp = $"http://{Console.ReadLine().Trim()}:3000";
                }

                SettingsUtil.SaveSettings(FrenzySettings);

                Console.ResetColor();
                MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }
        static async Task MainAsync(string[] args)
        {
            int Action = 0;
            while (Action != Actions.Length)
            {
                Console.Title = $"ProjectFrenzy | {TaskList.Count} Tasks Loaded";
                Action = GetUserAction();

                if (Action == 1)
                    AddTask();

                else if (Action == 2)
                    TaskList.Clear();

                else if (Action == 3)
                    StartTasks();

                else if (Action == 4)
                    DisplayTasks();

                else if (Action == 5)
                    AddDiscordWebhook();

                else if (Action == 6)
                {
                    if (WebhookSender.TestWebhook().Result)
                    {
                        Logger.Log("Success sending webhook", ConsoleColor.Green);
                    }
                    else
                    {
                        Logger.Log("Failed sending webhook", ConsoleColor.Red);
                    }
                    await Task.Delay(3000);
                }

                else if (Action == Actions.Length)
                    Environment.Exit(0);

                Console.Clear();
                Logger.PrintLogo();
            }
        }

        private static void AuthenticateUser()
        {
            while (true)
            {
                Thread.Sleep(TimeSpan.FromSeconds(60));
                if (!Authenticator.ValidateLicense())
                    Environment.Exit(-1);
            }
        }

        /*
         * Ensures the folder %appdata%/FrenzyBot exists along with the files settings.json within it 
        */
        private static void VerifyInstallation()
        {
            //Will fail if not enough perms (MACOS)
                if (!Directory.Exists(MainDir))
                    Directory.CreateDirectory(MainDir);

            if (!File.Exists(Path.Combine(MainDir, "settings.json")))
            {
                Settings settings = new Settings { };
                SettingsUtil.SaveSettings(settings);
            }

        }

        private static void AddProfile()
        {
            Profile profile;

            var ProfileName = GetInput("Enter a profile name:");
            var Email = GetInput("Enter the email:");
            var Phone = GetInput("Enter the phone number:");
            var ShippingFirstName = GetInput("Enter the first name (shipping):");
            var ShippingLastName = GetInput("Enter the last name (shipping):");
            var ShippingAddressLine1 = GetInput("Enter the address line 1 (shipping):");
            var ShippingAddressLine2 = GetInput("Enter the address line 2 (shipping) (optional):");
            var ShippingCity = GetInput("Enter the city (shipping)");
            var ShippingState = GetInput("Enter the state (shipping)");
            var ShippingZip = GetInput("Enter the zip/postal (shipping)");
        AskDifferentBilling:
            var DifferentBilling = GetInput("Different billing address? (yes/no)");

            if (DifferentBilling.ToLower().Trim() == "y" || DifferentBilling.ToLower().Trim() == "yes")
            {
                var BillingFirstName = GetInput("Enter the first name (billing):");
                var BillingLastName = GetInput("Enter the last name (billing):");
                var BillingAddressLine1 = GetInput("Enter the address line 1 (billing):");
                var BillingAddressLine2 = GetInput("Enter the address line 2 (billing) (optional):");
                var BillingCity = GetInput("Enter the city (billing)");
                var BillingState = GetInput("Enter the state (billing)");
                var BillingZip = GetInput("Enter the zip/postal (billing)");

                profile = new Profile
                {
                    ProfileName = ProfileName,
                    Email = Email,
                    Phone = Phone,
                    ShippingFirstName = ShippingFirstName,
                    ShippingLastName = ShippingLastName,
                    ShippingAddress1 = ShippingAddressLine1,
                    ShippingAddress2 = ShippingAddressLine2,
                    ShippingCity = ShippingCity,
                    ShippingState = ShippingState,
                    ShippingZip = ShippingZip,
                    BillingFirstName = BillingFirstName,
                    BillingLastName = BillingLastName,
                    BillingAddress1 = BillingAddressLine1,
                    BillingAddress2 = BillingAddressLine2,
                    BillingCity = BillingCity,
                    BillingState = BillingState,
                    BillingZip = BillingZip
                };
            }
            else if (DifferentBilling.ToLower().Trim() == "n" || DifferentBilling.ToLower().Trim() == "no")
            {
                profile = new Profile
                {
                    ProfileName = ProfileName,
                    Email = Email,
                    Phone = Phone,
                    ShippingFirstName = ShippingFirstName,
                    ShippingLastName = ShippingLastName,
                    ShippingAddress1 = ShippingAddressLine1,
                    ShippingAddress2 = ShippingAddressLine2,
                    ShippingCity = ShippingCity,
                    ShippingState = ShippingState,
                    ShippingZip = ShippingZip,
                    BillingFirstName = "",
                    BillingLastName = "",
                    BillingAddress1 = "",
                    BillingAddress2 = "",
                    BillingCity = "",
                    BillingState = "",
                    BillingZip = ""
                };
            }
            else
                goto AskDifferentBilling;

            //Profiles.Add(profile);

            //File.WriteAllText(Path.Combine(MainDir, "profiles.json"), JsonConvert.SerializeObject(Profiles));
        }

        private static string GetInput(string Prompt, ConsoleColor C = ConsoleColor.Green)
        {
            Console.ForegroundColor = C;
            Console.WriteLine(Prompt);
            Console.ResetColor();
            return Console.ReadLine();
        }

        //display menu and return the respective int for the action
        private static int GetUserAction()
        {
            Console.WriteLine("MAIN MENU");
            Console.ForegroundColor = ConsoleColor.Green;
            for (int i = 0; i < Actions.Length; i++)
                Console.WriteLine($"{i+1}. {Actions[i]}");
            Console.ResetColor();

            bool IsNum = false;
            int Action = -1;
            do
            {
                Console.WriteLine("Choose an action (enter the number):");
                var Input = Console.ReadLine();
                IsNum = int.TryParse(Input, out Action);
            }
            while (!IsNum || Action < 1 || Action > Actions.Length);

            return Action;
        }

        private static void DisplayTasks()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[Task ID] - Product - Mode - Option");
            foreach (FrenzyTask ft in TaskList)
            {
                Console.WriteLine($"[{ft.TaskId}] - {ft.ProductTitle} - {modes[ft.Mode - 1].Split(":")[0].ToLower()} - {ft.SelectedOptions.Length} Option(s) Selected");
            }
            Console.ResetColor();
            Console.WriteLine("Press enter to continue...");
            Console.ReadKey();
        }

        private static void StartTasks()
        {
            foreach (FrenzyTask task in TaskList)
                new Task(() => task.Start()).Start();
        }

        //function to add task to the task list
        private static void AddTask()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Fetching Live Releases...");
            Console.ResetColor();

            var Payload = new AndroidPayload();

            //Display menu, and obtain and validated requested item id
            int retries = 0;
            List<Flashsale> FlashSales = new List<Flashsale>();
            Menu:
            try
            {
                FlashSales = Menu.DisplayProductMenu().Result;
            }
            catch
            {
                retries++;
                if (retries < 3)
                    goto Menu;
                else
                {
                    Logger.Log("Failed to get releases. Press any key to continue.", ConsoleColor.Red);
                    Console.ReadKey();
                    return;
                }
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Enter the ID of the drop you would like to run for (If private sale enter password):");
            Console.ResetColor();
            var ItemIdChoiceString = Console.ReadLine();

            //find product with entered id
            Flashsale Item = null;
            foreach (var Sale in FlashSales)
            {
                if (Sale.Id.ToString() == ItemIdChoiceString)
                {
                    Item = Sale;
                    break;
                }
            }
            if(Item == null && ItemIdChoiceString.Length == 12) 
                Item = Endpoints.GetFlashsale(ItemIdChoiceString).Result;
            

            if (Item == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid ID.");
                Console.ResetColor();
                Thread.Sleep(500);
                Console.Clear();
                goto Menu;
            }
            if(Item.SoldOut)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Selected Drop doesn't have products or is sold out");
                Console.ResetColor();
                Thread.Sleep(1000);
                Console.Clear();
                goto Menu;
            }

            Payload.GatewayMerchantId = Item.Shop.ShopZones.GooglePayMerchantId.ToString();
            Payload.TotalPrice = Item.PriceRange.Max.ToString();
            Payload.Currency = Item.Shop.Currency;
            string ProductName;
            string Options = "random";
            int ProductIndex;

            if (Item.ProductsCount > 1)
            {
                var TitleList = new List<string>();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Enter the ID of the product:");
                foreach (var Product in Item.ProductDetails)
                {
                    TitleList.Add(Product.Title);
                    Console.WriteLine($"[{TitleList.Count}] - [{Product.Title}]");
                }

                int ProductIndex_;
                while (!int.TryParse(Console.ReadLine(), out ProductIndex_) || ProductIndex_ < 1 || ProductIndex_ > TitleList.Count)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid Product ID");
                    Console.ResetColor();
                }
                ProductName = Item.ProductDetails[ProductIndex_ - 1].Title;
                ProductIndex = ProductIndex_ - 1;
            }
            else
            {
                if (Item.ProductsCount == 1)
                {
                    ProductName = Item.ProductDetails[0].Title;
                    ProductIndex = 0;
                }
                else 
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Warning! Looks like this Drop has no items, input below the productIndex you want to run for (0 being 1)");
                    Console.ResetColor();
                    int ProductIndex_;
                    while (!int.TryParse(Console.ReadLine(), out ProductIndex_))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid input");
                        Console.ResetColor();
                    }
                    ProductName = $"{Item.Title} [{ProductIndex_}]";
                    ProductIndex = ProductIndex_;
                }
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Enter the ID of the mode:");
            for (int i = 0; i < modes.Length; i++)
                Console.WriteLine($"[{i + 1}] {modes[i]}");
            Console.ResetColor();
            int ModeIndex;
            while (!int.TryParse(Console.ReadLine(), out ModeIndex) || ModeIndex < 1 || ModeIndex > modes.Length)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid Mode ID");
                Console.ResetColor();
            }
            if(ModeIndex == 2 || ModeIndex == 3)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Enter the size/option for the item.");
                Console.WriteLine("Shoes: 10 , 5.5, etc...");
                Console.WriteLine("Clothing: M, XXL, etc...");
                Console.WriteLine("One Size: default");
                Console.WriteLine("For multiple size choices, enter them separated by commas with no spaces. eg: 5.5,6,8,12.5");
                Console.ResetColor();
                Options = Console.ReadLine();
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Enter a delay in milliseconds:");
            int CheckoutDelay;
            while (true)
            {
                Console.ResetColor();
                if(int.TryParse(Console.ReadLine(), out CheckoutDelay))
                {
                    if(CheckoutDelay >= 0)
                    {
                        break;
                    }
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid delay.");
            }

            /* ProxySelection:
             Console.ForegroundColor = ConsoleColor.Green;
             /Console.WriteLine("Use Proxy? (yes/no)");
             Console.ResetColor();
             var UseProxyInput = Console.ReadLine();
             bool UseProxy;

             UseProxyInput = UseProxyInput.ToLower().Trim();
             if (UseProxyInput == "y" || UseProxyInput == "yes")
             {
                 while (ProxyManager.UnusedProxies.Count == 0)
                     ProxyManager.ReloadProxies();
                 UseProxy = true;
             }
             else if (UseProxyInput == "n" || UseProxyInput == "no")
                 UseProxy = false;
             else
                 goto ProxySelection;
                 */
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Enter the unique email for this task:");
            Console.ResetColor();
            var email = Console.ReadLine();

            Console.ResetColor();
            TaskList.Add(new FrenzyTask(TaskList.Count + 1, Payload, Item, ModeIndex, ProductIndex, ProductName, CheckoutDelay, false, email, Options.Split(',')));
        }

        
        private static void AddDiscordWebhook()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Enter the Discord Webhook URL:");
            Console.ResetColor();
            var DiscordWebhook = Console.ReadLine();

            while(!DiscordWebhook.StartsWith("https://discordapp.com/api/webhooks/"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid Discord Webhook URL. Try again.");
                Console.ResetColor();
                DiscordWebhook = Console.ReadLine();
            }

            FrenzySettings.DiscordWebhookUrl = DiscordWebhook;

            SettingsUtil.SaveSettings(FrenzySettings);
        }

    }
}
