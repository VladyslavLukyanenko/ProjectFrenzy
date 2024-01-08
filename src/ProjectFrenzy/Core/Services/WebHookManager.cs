using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ProjectFrenzy.Core.Model;
using ProjectFrenzy.Core.Model.Discord;

namespace ProjectFrenzy.Core.Services
{
  public class WebHookManager : IWebHookManager
  {
    private DiscordSettings _discordSettings;
    private readonly ISoftwareInfoProvider _softwareInfoProvider;

    private static readonly HttpClient Client = new HttpClient();
    private static readonly Queue<ScheduledWebHook> Queue = new Queue<ScheduledWebHook>();

    public WebHookManager(IDiscordSettingsService discordSettingsService, ISoftwareInfoProvider softwareInfoProvider)
    {
      _softwareInfoProvider = softwareInfoProvider;
      discordSettingsService.Settings.Subscribe(s => _discordSettings = s);
    }

    public void EnqueueWebhook(CheckoutResult checkoutResult, FrenzyCheckoutTask task)
    {
      var webhookObj = new DiscordWebhookBody
      {
        Embeds = new List<Embed>
        {
          Embed.CreateFrom(checkoutResult, task, task.Product.DefaultPicture,
            _softwareInfoProvider.CurrentSoftwareVersion)
        }
      };

      if (checkoutResult.Status.IsSuccessful)
      {
        TryEnqueueSuccess(webhookObj);
      }
      else
      {
        TryEnqueueFailure(webhookObj);
      }
    }

    public async Task<bool> TestWebhook()
    {
      if (string.IsNullOrEmpty(_discordSettings.SuccessWebHook))
      {
        return false;
      }

      var webhookObj = new DiscordWebhookBody
      {
        Content = "",
        Username = "Project Frenzy",
        AvatarUrl =
          "https://cdn.discordapp.com/attachments/682673305995575370/685699712086441993/ProjectFrenzy.png",
        Embeds = new List<Embed>()
      };

      webhookObj.Embeds.Add(new Embed
      {
        Author = new Author
        {
          Name = "",
          Url = "",
          IconUrl =
            "https://cdn.discordapp.com/attachments/682673305995575370/685699712086441993/ProjectFrenzy.png"
        },
        Color = 0xE71A48,
        Title = "**Test Webhook** :partying_face:",
        Url = "",
        Image = new Image
        {
          Url =
            "https://cdn.discordapp.com/attachments/682673305995575370/685699712086441993/ProjectFrenzy.png"
        },
        Thumbnail = new Image {Url = ""},
        Footer = new Footer
        {
          Text = $"Project Frenzy v{_softwareInfoProvider.CurrentSoftwareVersion}",
        },
        Fields = new List<Field>()
      });

      try
      {
        var webhookContent = new StringContent(JsonConvert.SerializeObject(webhookObj), Encoding.UTF8,
          "application/json");
        var webhookResp = await Client.PostAsync(_discordSettings.SuccessWebHook, webhookContent);

        return (int) webhookResp.StatusCode == 204;
      }
      catch
      {
        return false;
      }
    }


    public void TryEnqueueFailure(DiscordWebhookBody webhookBody)
    {
      if (string.IsNullOrEmpty(_discordSettings.FailureWebHook))
      {
        return;
      }

      Queue.Enqueue(new ScheduledWebHook(_discordSettings.FailureWebHook, webhookBody));
    }

    private void TryEnqueueSuccess(DiscordWebhookBody webhookBody)
    {
      if (string.IsNullOrEmpty(_discordSettings.SuccessWebHook))
      {
        return;
      }

      Queue.Enqueue(new ScheduledWebHook(_discordSettings.SuccessWebHook, webhookBody));
    }

    public void Spawn()
    {
      Task.Factory.StartNew(async () =>
      {
        while (true)
        {
          if (Queue.Count > 0)
          {
            var item = Queue.Dequeue();
            try
            {
              var webhookContent = new StringContent(JsonConvert.SerializeObject(item.Content), Encoding.UTF8,
                "application/json");
              var webhookResp = await Client.PostAsync(item.WebHookApiUrl, webhookContent);

              if (webhookResp.StatusCode == HttpStatusCode.NotFound)
              {
                continue;
              }
              
              if ((int) webhookResp.StatusCode != 204)
              {
                Queue.Enqueue(item);
              }
            }
            catch
            {
              if (item != null)
              {
                Queue.Enqueue(item);
              }
            }
          }

          await Task.Delay(500);
        }
        // ReSharper disable once FunctionNeverReturns
      }, TaskCreationOptions.LongRunning);
    }

    private class ScheduledWebHook
    {
      public ScheduledWebHook(string webHookApiUrl, DiscordWebhookBody content)
      {
        WebHookApiUrl = webHookApiUrl;
        Content = content;
      }

      public string WebHookApiUrl { get; }
      public DiscordWebhookBody Content { get; }
    }
  }
}