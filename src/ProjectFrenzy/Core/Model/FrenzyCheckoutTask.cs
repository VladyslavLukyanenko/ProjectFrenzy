using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using ProjectFrenzy.Core.Model.FlashSale;
using ProjectFrenzy.Core.ViewModels;
using ReactiveUI.Fody.Helpers;

namespace ProjectFrenzy.Core.Model
{
  [DataContract]
  public class FrenzyCheckoutTask : ViewModelBase
  {
    // private CheckoutStatus _status = CheckoutStatus.Created;
    [DataMember] public long Id { get; private set; } = Stopwatch.GetTimestamp();

    [Reactive, DataMember] public TimeSpan CheckoutDuration { get; set; }

    [Reactive, DataMember] public CheckoutStatus Status { get; set; } = CheckoutStatus.Created;

    [Reactive, DataMember] public int CheckoutDelay { get; set; }
    [Reactive, DataMember] public Flashsale Flashsale { get; set; }
    [Reactive, DataMember] public IList<string> SelectedSizes { get; set; }
    [Reactive, DataMember] public CheckoutMode Mode { get; set; } = CheckoutMode.Preference;
    [Reactive, DataMember] public bool UseProxies { get; set; }
    [Reactive, DataMember] public Profile SelectedProfile { get; set; }
    [Reactive, DataMember] public ProductDetail Product { get; set; }
    [Reactive, DataMember] public Emulator PreferredEmulator { get; set; }

    [Reactive, DataMember] public Email AssignedEmail { get; set; }
  }
}