using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Validators;
using Newtonsoft.Json.Linq;
using ProjectFrenzy.Core.Model;

namespace ProjectFrenzy.Core.Validators
{
  public class DiscordSettingsValidator : AbstractValidator<DiscordSettings>
  {
    const string NotAvailableMessage = "Provided webhook url is not available or invalid. Please check it again.";
    const string InvalidUrlMessage = "Invalid discord webhook url provided.";
    public DiscordSettingsValidator()
    {
      RuleFor(_ => _.SuccessWebHook).Must(BeValidUrl).WithMessage(InvalidUrlMessage)
        .MustAsync(BeValidHook).WithMessage(NotAvailableMessage);
      RuleFor(_ => _.FailureWebHook).Must(BeValidUrl).WithMessage(InvalidUrlMessage)
        .MustAsync(BeValidHook).WithMessage(NotAvailableMessage);
    }

    private bool BeValidUrl(DiscordSettings settings, string url, PropertyValidatorContext ctx)
    {
      return !string.IsNullOrEmpty(url) && url.Contains("discord", StringComparison.CurrentCultureIgnoreCase);
    }

    private async Task<bool> BeValidHook(DiscordSettings settings, string url, PropertyValidatorContext ctx,
      CancellationToken ct)
    {
      if (string.IsNullOrEmpty(url))
      {
        return false;
      }
      
      var client = new HttpClient();
      var resp = await client.GetAsync(url, ct);
      if (!resp.IsSuccessStatusCode)
      {
        return false;
      }

      var json = await resp.Content.ReadAsStringAsync();
      var obj = JObject.Parse(json);

      return obj.ContainsKey("name");
    }
  }
}