using System;
using System.Reactive.Linq;
using ProjectFrenzy.Core.Model;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ProjectFrenzy.Core.Clients;

namespace ProjectFrenzy.Core.ViewModels
{
    public class CheckedOutProductViewModel
        : ViewModelBase
    {
        public CheckedOutProductViewModel(CheckoutDetailsData product)
        {
            var p = this.WhenAnyValue(_ => _.Product);

            p.Select(_ => _?.Image).ToPropertyEx(this, _ => _.Picture);
            p.Select(_ => _?.Name).ToPropertyEx(this, _ => _.Name);
            p.Where(_ => _ != null).Select(_ => DateTimeOffset.FromUnixTimeSeconds(_.Date).ToString("dd-MM-yyyy"))
                .ToPropertyEx(this, _ => _.CheckoutDate);
            p.Select(it => it == null ? 0M : it.Price).ToPropertyEx(this, _ => _.CheckoutPrice);

            Product = product;
        }

        [Reactive] public CheckoutDetailsData Product { get; set; }

        public string Name { [ObservableAsProperty] get; }
        public string CheckoutDate { [ObservableAsProperty] get; }
        public decimal CheckoutPrice { [ObservableAsProperty] get; }
        public string Picture { [ObservableAsProperty] get; }
    }
}