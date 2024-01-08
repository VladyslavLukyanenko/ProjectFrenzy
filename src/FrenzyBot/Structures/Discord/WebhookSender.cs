using FrenzyBot.Structures.User;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FrenzyBot.Structures.Discord
{
    public static class WebhookSender
    {
        private static HttpClient Client = new HttpClient();
        private static Queue<DiscordWebhookBody> queue = new Queue<DiscordWebhookBody>();

        public async static Task<bool> TestWebhook()
        {
            if (Program.FrenzySettings.DiscordWebhookUrl == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No Webhook URL loaded");
                return false;
            }

            var WebhookObj = new DiscordWebhookBody
            {
                Content = "",
                Username = "Project Frenzy",
                AvatarUrl = "https://cdn.discordapp.com/attachments/682673305995575370/685699712086441993/ProjectFrenzy.png",
                Embeds = new List<Embed>()
            };

            WebhookObj.Embeds.Add(new Embed
            {
                Author = new Author
                {
                    Name = "",
                    Url = "",
                    IconUrl = "https://cdn.discordapp.com/attachments/682673305995575370/685699712086441993/ProjectFrenzy.png"
                },
                Color = 0xE71A48,
                Title = "**Test Webhook** :partying_face:",
                Url = "",
                Image = new Image { Url = "https://cdn.discordapp.com/attachments/682673305995575370/685699712086441993/ProjectFrenzy.png" },
                Thumbnail = new Image { Url = "" },
                Footer = new Footer
                {
                    Text = $"Project Frenzy v{Versions.program_version}",
                    IconUrl = "https://cdn.discordapp.com/attachments/682673305995575370/685699712086441993/ProjectFrenzy.png"
                },
                Fields = new List<Field>()
            });

            try
            {
                var WebhookContent = new StringContent(JsonConvert.SerializeObject(WebhookObj), Encoding.UTF8, "application/json");
                var WebhookResp = await Client.PostAsync(Program.FrenzySettings.DiscordWebhookUrl, WebhookContent);

                if ((int)WebhookResp.StatusCode == 204)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }


        public static void SendFailedCheckoutWebhook(string ProductName, string Mode, string Email, int CustomDelay, string Option, string ShopName, string CheckoutTime, string Error, string ThumbnailUrl= "https://cdn.discordapp.com/attachments/682673305995575370/685699712086441993/ProjectFrenzy.png")
        {
            if (Program.FrenzySettings.DiscordWebhookUrl == null)
            {
                return;
            }

            var WebhookObj = new DiscordWebhookBody
            {
                Content = "",
                Username = "Project Frenzy",
                AvatarUrl = "https://cdn.discordapp.com/attachments/682673305995575370/685699712086441993/ProjectFrenzy.png",
                Embeds = new List<Embed>()
            };

            WebhookObj.Embeds.Add(new Embed
            {
                Author = new Author
                {
                    Name = "",
                    Url = "",
                    IconUrl = "https://cdn.discordapp.com/attachments/682673305995575370/685699712086441993/ProjectFrenzy.png"
                },
                Color = 0xCC0000,
                Title = "**Checkout Declined** :frowning:",
                Url = "",
                Image = new Image { Url = "" },
                Thumbnail = new Image { Url = ThumbnailUrl },
                Footer = new Footer
                {
                    Text = $"Project Frenzy v{Versions.program_version}",
                    IconUrl = "https://cdn.discordapp.com/attachments/682673305995575370/685699712086441993/ProjectFrenzy.png"
                },
                Fields = new List<Field>()
            });
            WebhookObj.Embeds[0].Fields.Add(new Field
            {
                Name = "Email",
                Value = $"||{Email}||",
                Inline = false
            });

            WebhookObj.Embeds[0].Fields.Add(new Field
            {
                Name = "Product",
                Value = @ProductName,
                Inline = false
            });

            WebhookObj.Embeds[0].Fields.Add(new Field
            {
                Name = "Mode",
                Value = Mode,
                Inline = false
            });
            WebhookObj.Embeds[0].Fields.Add(new Field
            {
                Name = "Shop",
                Value = @ShopName,
                Inline = false
            });

            WebhookObj.Embeds[0].Fields.Add(new Field
            {
                Name = "Option",
                Value = Option,
                Inline = false
            });

            WebhookObj.Embeds[0].Fields.Add(new Field
            {
                Name = "Checkout Time",
                Value = @CheckoutTime,
                Inline = false
            });

            WebhookObj.Embeds[0].Fields.Add(new Field
            {
                Name = "Delay",
                Value = $"||{CustomDelay}||",
                Inline = false
            });

            WebhookObj.Embeds[0].Fields.Add(new Field
            {
                Name = "Error",
                Value = @Error,
                Inline = false
            });

            queue.Enqueue(WebhookObj);
        }

        public static void SendSuccessfulCheckoutWebhook(string ProductName, string Mode, string Email, int CustomDelay, string Option, string ShopName, string CheckoutTime, string Price, string ThumbnailUrl = "https://cdn.discordapp.com/attachments/682673305995575370/685699712086441993/ProjectFrenzy.png")
        {
            if (Program.FrenzySettings.DiscordWebhookUrl == null)
            {
                return;
            }

            var WebhookObj = new DiscordWebhookBody
            {
                Content = "",
                Username = "Project Frenzy",
                AvatarUrl = "https://cdn.discordapp.com/attachments/682673305995575370/685699712086441993/ProjectFrenzy.png",
                Embeds = new List<Embed>()
            };

            WebhookObj.Embeds.Add(new Embed
            {
                Author = new Author
                {
                    Name = "",
                    Url = "",
                    IconUrl = "https://cdn.discordapp.com/attachments/682673305995575370/685699712086441993/ProjectFrenzy.png"
                },
                Color = 0x32CD32,
                Title = "**Successful Checkout** :partying_face:",
                Url = "",
                Image = new Image { Url = "" },
                Thumbnail = new Image { Url = ThumbnailUrl },
                Footer = new Footer
                {
                    Text = $"Project Frenzy v{Versions.program_version}",
                    IconUrl = "https://cdn.discordapp.com/attachments/682673305995575370/685699712086441993/ProjectFrenzy.png"
                },
                Fields = new List<Field>()
            });

            WebhookObj.Embeds[0].Fields.Add(new Field
            {
                Name = "Email",
                Value = $"||{Email}||",
                Inline = false
            });

            WebhookObj.Embeds[0].Fields.Add(new Field
            {
                Name = "Product",
                Value = @ProductName,
                Inline = false
            });

            WebhookObj.Embeds[0].Fields.Add(new Field
            {
                Name = "Mode",
                Value = Mode,
                Inline = false
            });

            WebhookObj.Embeds[0].Fields.Add(new Field
            {
                Name = "Shop",
                Value = @ShopName,
                Inline = false
            });
            WebhookObj.Embeds[0].Fields.Add(new Field
            {
                Name = "Price",
                Value = @Price,
                Inline = false
            });
            WebhookObj.Embeds[0].Fields.Add(new Field
            {
                Name = "Option",
                Value = Option,
                Inline = false
            });

            WebhookObj.Embeds[0].Fields.Add(new Field
            {
                Name = "Checkout Time",
                Value = @CheckoutTime,
                Inline = false
            });

            WebhookObj.Embeds[0].Fields.Add(new Field
            {
                Name = "Delay",
                Value = $"||{CustomDelay}||",
                Inline = false
            });
            queue.Enqueue(WebhookObj);
        }
        public static async Task Logic()
        {
            while (true)
            {
                if(queue.Count > 0)
                {
                    DiscordWebhookBody item = queue.Dequeue();
                    try
                    {
                        var WebhookContent = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");
                        var WebhookResp = await Client.PostAsync(Program.FrenzySettings.DiscordWebhookUrl, WebhookContent);

                        if ((int)WebhookResp.StatusCode != 204)
                            queue.Enqueue(item);
                    }
                    catch
                    {
                        if(item != null)
                            queue.Enqueue(item);
                    }
                }
                await Task.Delay(500);
            }
        }

    }
}
