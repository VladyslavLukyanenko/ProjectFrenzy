using System.Runtime.Serialization;
using ProjectFrenzy.Core.ViewModels;
using ReactiveUI.Fody.Helpers;

namespace ProjectFrenzy.Core.Model
{
    [DataContract]
    public class DiscordSettings : ViewModelBase
    {
        [Reactive, DataMember] public string SuccessWebHook { get; set; }
        [Reactive, DataMember] public string FailureWebHook { get; set; }
        
        public static readonly DiscordSettings Empty = new DiscordSettings();
    }
}