using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using ProjectFrenzy.Core.ViewModels;
using ReactiveUI.Fody.Helpers;

namespace ProjectFrenzy.Core.Model
{
  [DataContract]
  public class Email : ViewModelBase
  {
    public static readonly Regex CatchAllRegex =
      new Regex(@"^@[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)$",
        RegexOptions.Compiled);

    private static readonly Regex EmailRegex = new Regex(
      @"(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])",
      RegexOptions.Compiled);

    public Email(string value, bool isCatchAll)
    {
      Value = value;
      IsCatchAll = isCatchAll;
    }

    [DataMember] public string Value { get; private set; }
    [DataMember] public bool IsCatchAll { get; private set; }
    [Reactive] public bool IsAllocated { get; set; }

    public static bool TryParse(string raw, out Email email)
    {
      var isCatchAll = CatchAllRegex.IsMatch(raw);
      if (isCatchAll || EmailRegex.IsMatch(raw))
      {
        email = new Email(raw, isCatchAll);
      }
      else
      {
        email = null;
      }

      return email != null;
    }
  }
}