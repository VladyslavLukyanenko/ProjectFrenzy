using System;
using System.Runtime.Serialization;
using ProjectFrenzy.Core.ViewModels;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProjectFrenzy.Core.Model
{
    [DataContract]
    public class Profile : ViewModelBase
    {
        public Profile()
        {
            ShippingAddress.Changed.Subscribe(_ => this.RaisePropertyChanged(nameof(ShippingAddress)));
            BillingAddress.Changed.Subscribe(_ => this.RaisePropertyChanged(nameof(BillingAddress)));
        }
        
        [Reactive, DataMember] public string ProfileName { get; set; }

        [DataMember] public CheckoutAddress ShippingAddress { get; set; } = new CheckoutAddress();
        [DataMember] public CheckoutAddress BillingAddress { get; set; } = new CheckoutAddress();
        [Reactive, DataMember] public bool IsShippingSameAsBilling { get; set; } = true;
    }
}
