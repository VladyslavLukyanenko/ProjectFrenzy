using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ProjectFrenzy.Core.Model.FlashSale;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProjectFrenzy.Core.ViewModels
{
  public class FlashsaleItemPickerViewModel : ViewModelBase, IDisposable
  {
    private readonly CompositeDisposable _disposable = new CompositeDisposable();
    public FlashsaleItemPickerViewModel(Flashsale flashsale)
    {
      Flashsale = flashsale;
      Name = flashsale.Title;
      Picture = flashsale.ImageUrls.FirstOrDefault()?.ToString();

      FormattedPriceRange = flashsale.PriceRange.ToString(flashsale.Shop.Currency);
      Pickup = flashsale.Pickup;
      ShopName = flashsale.Shop.Name;
      Dropzone = flashsale.GetDropType();

      StartTime = flashsale.GetDelay();
      var d = Observable.Interval(TimeSpan.FromMilliseconds(120), RxApp.TaskpoolScheduler)
        .TakeUntil(_ => StartTime < TimeSpan.Zero)
        .ObserveOn(RxApp.MainThreadScheduler)
        .Subscribe(_ => StartTime = flashsale.GetDelay());
      _disposable.Add(d);

      this.WhenAnyValue(_ => _.StartTime)
        .ObserveOn(RxApp.MainThreadScheduler)
        .Select(t => t <= TimeSpan.Zero)
        .ToPropertyEx(this, _ => _.IsStartedAlready)
        .DisposeWith(_disposable);
    }

    public string Picture { get; private set; }
    public string Name { get; private set; }

    [Reactive] public TimeSpan StartTime { get; private set; }
    public string ShopName { get; private set; }
    public string Dropzone { get; private set; }
    public bool Pickup { get; private set; }
    public string FormattedPriceRange { get; private set; }

    public bool IsStartedAlready { [ObservableAsProperty] get; }

    public Flashsale Flashsale { get; private set; }

    public void Dispose()
    {
      _disposable.Dispose();
    }
  }
}