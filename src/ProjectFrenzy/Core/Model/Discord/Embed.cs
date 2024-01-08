using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Newtonsoft.Json;

namespace ProjectFrenzy.Core.Model.Discord
{
  [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
  public partial class Embed
  {
    private const string SuccessTitle = "**Successful Checkout** :partying_face:";
    private const int SuccessColor = 0x32CD32;

    private const string FailureTitle = "**Checkout Declined** :frowning:";
    private const int FailureColor = 0xCC0000;

    [JsonProperty("author")] public Author Author { get; set; } = new Author();

    [JsonProperty("color")] public long Color { get; set; }

    [JsonProperty("title")] public string Title { get; set; }

    [JsonProperty("url")] public string Url { get; set; } = "";

    [JsonProperty("image")] public Image Image { get; set; } = new Image();

    [JsonProperty("thumbnail")] public Image Thumbnail { get; set; } = new Image();

    [JsonProperty("footer")] public Footer Footer { get; set; } = new Footer();

    [JsonProperty("fields")] public List<Field> Fields { get; set; } = new List<Field>();

    public void Add(CheckoutResult checkoutResult, FrenzyCheckoutTask task)
    {
      Fields.Add(new Field
      {
        Name = "Email",
        Value = $"||{task.AssignedEmail.Value}||",
      });

      Fields.Add(new Field
      {
        Name = "Product",
        Value = task.Product.Title,
      });

      Fields.Add(new Field
      {
        Name = "Mode",
        Value = task.Mode.ToString(),
      });

      Fields.Add(new Field
      {
        Name = "Shop",
        Value = task.Flashsale.Shop.Name,
      });
      var isSuccessful = checkoutResult.Status.IsSuccessful;
      Fields.Add(new Field
      {
        Name = isSuccessful ? "Price" : "Error",
        Value = isSuccessful
          ? checkoutResult.TotalPrice.ToString(CultureInfo.InvariantCulture)
          : checkoutResult.StatusMessage,
      });
      Fields.Add(new Field
      {
        Name = "Option",
        Value = string.Join(",", task.SelectedSizes),
      });

      Fields.Add(new Field
      {
        Name = "Checkout Time",
        Value = $"{task.CheckoutDuration.Seconds}.{task.CheckoutDuration.Milliseconds:D3} seconds",
      });

      Fields.Add(new Field
      {
        Name = "Delay",
        Value = $"||{task.CheckoutDelay}||",
      });
    }

    public static Embed CreateFrom(CheckoutResult checkoutResult, FrenzyCheckoutTask task, string thumbnailUrl,
      string softwareVersion)
    {
      int color;
      string title;
      if (checkoutResult.Status.IsSuccessful)
      {
        color = SuccessColor;
        title = SuccessTitle;
      }
      else
      {
        color = FailureColor;
        title = FailureTitle;
      }

      var embed = new Embed
      {
        Color = color,
        Title = title,
        Footer =
        {
          Text = $"Project Frenzy v{softwareVersion}"
        },
        Thumbnail = new Image {Url = thumbnailUrl}
      };

      embed.Add(checkoutResult, task);

      return embed;
    }
  }
}