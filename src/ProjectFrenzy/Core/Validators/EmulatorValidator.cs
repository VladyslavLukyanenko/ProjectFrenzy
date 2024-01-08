using System.Text.RegularExpressions;

namespace ProjectFrenzy.Core.Validators
{
  public class EmulatorValidator
  {
    private static readonly Regex NgrokUrlRegex = new Regex(@"https?:\/\/(www\.)?[a-zA-Z0-9]{1,256}\.ngrok.io",
      RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static readonly Regex IpAddressRegex =
      new Regex(@"^((25[0-5]|(2[0-4]|1[0-9]|[1-9]|)[0-9])(\.(?!$)|$)){4}$", RegexOptions.Compiled);

    public bool IsEndpointValid(string endpoint)
    {
      return !string.IsNullOrEmpty(endpoint) && (NgrokUrlRegex.IsMatch(endpoint) ||
                                                 IpAddressRegex.IsMatch(endpoint));
    }
  }
}