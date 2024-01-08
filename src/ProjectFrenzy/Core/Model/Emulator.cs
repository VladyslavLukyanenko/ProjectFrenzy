using System.Runtime.Serialization;
using ProjectFrenzy.Core.Validators;
using ProjectFrenzy.Core.ViewModels;
using ReactiveUI.Fody.Helpers;

namespace ProjectFrenzy.Core.Model
{
  [DataContract]
  public class Emulator: ViewModelBase
  {
    [Reactive, DataMember] public string Ip { get; set; }
    [Reactive] public bool IsAvailable { get; set; }

    public static Emulator FromRawEndpointAddress(string endpoint)
    {
      if (EmulatorValidator.IpAddressRegex.IsMatch(endpoint))
      {
        endpoint += ":3000";
      }

      return new Emulator {Ip = endpoint, IsAvailable = true};
    }

    public string GetUrl()
    {
      if (!Ip.StartsWith("http"))
      {
        return "http://" + Ip;
      }

      return Ip;
    }
  }
}