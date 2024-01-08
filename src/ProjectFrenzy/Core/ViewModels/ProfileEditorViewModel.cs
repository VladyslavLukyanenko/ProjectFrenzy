using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ProjectFrenzy.Core.Model;
using ProjectFrenzy.Core.Services;
using ProjectFrenzy.Core.ToastNotifications;
using ProjectFrenzy.Core.Validators;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProjectFrenzy.Core.ViewModels
{
  public class ProfileEditorViewModel
    : ViewModelBase
  {
    public ProfileEditorViewModel(ICountriesService countriesService, IProfilesService profilesService,
      CheckoutAddressValidator addressValidator, ProfileValidator profileValidator,
      IToastNotificationManager toasts, IMessageBus messageBus)
    {
      SelectBillingCommand = ReactiveCommand.Create(() => { ToggleTab(false); });
      SelectShippingCommand = ReactiveCommand.Create(() => { ToggleTab(true); });
      
      var profile = this.WhenAnyValue(_ => _.Profile);
      IDisposable disposable = null;
      profile
        .Where(p => p != null)
        .Subscribe(p =>
        {
          disposable?.Dispose();

          SelectShippingCommand.Execute().Subscribe();
          ShippingAddress = new EditAddressViewModel(countriesService, p.ShippingAddress, addressValidator);
          BillingAddress = new EditAddressViewModel(countriesService, p.BillingAddress, addressValidator);
          var changes = p.Changed
            .Throttle(TimeSpan.FromMilliseconds(200))
            .Select(_ => profileValidator.Validate(Profile).IsValid)
            .ToPropertyEx(this, _ => _.IsValid);

          var isShippingAndBillingChanges = p.WhenAnyValue(_ => _.IsShippingSameAsBilling)
            .Where(i => i)
            .Subscribe(_ => SelectShippingCommand.Execute().Subscribe());
            
          disposable = new CompositeDisposable(changes, isShippingAndBillingChanges);
        });

      profile.Select(p => p != null)
        .ToPropertyEx(this, _ => _.IsProfileSelected);

      CloseCommand = ReactiveCommand.Create(() => messageBus.SendMessage(HideModalComponentMessage.Instance));
      
      var canExecuteSave = new BehaviorSubject<bool>(false);
      SaveCommand = ReactiveCommand.CreateFromTask(async ct =>
      {
        await profilesService.AddOrUpdateAsync(Profile, ct);
        await CloseCommand.Execute().FirstOrDefaultAsync();
        toasts.Show(ToastContent.Success("Changes made to profile was saved"));
      }, canExecuteSave);

      this.WhenAnyValue(_ => _.IsValid)
        .ObserveOn(RxApp.MainThreadScheduler)
        .CombineLatest(SaveCommand.IsExecuting, (isValid, isExecuting) => (isValid, isExecuting))
        .Subscribe(p => canExecuteSave.OnNext(p.isValid && !p.isExecuting));
    }

    public bool IsValid { [ObservableAsProperty] get; }
    [Reactive] public Profile Profile { get; set; }
    [Reactive] public EditAddressViewModel ShippingAddress { get; private set; }
    [Reactive] public EditAddressViewModel BillingAddress { get; private set; }

    [Reactive] public bool IsShippingSelected { get; private set; }
    [Reactive] public bool IsBillingSelected { get; private set; }
    public bool IsProfileSelected { [ObservableAsProperty] get; }


    public ReactiveCommand<Unit, Unit> CloseCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> SaveCommand { get; private set; }

    public ReactiveCommand<Unit, Unit> SelectShippingCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> SelectBillingCommand { get; private set; }

    private void ToggleTab(bool isShippingSelected)
    {
      IsShippingSelected = isShippingSelected || Profile.IsShippingSameAsBilling;
      IsBillingSelected = !IsShippingSelected;
    }
  }
}