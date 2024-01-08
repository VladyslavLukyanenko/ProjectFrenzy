using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

using DynamicData;

using ProjectFrenzy.Core.Clients;
using ProjectFrenzy.Core.Model;
using ProjectFrenzy.Core.Model.FlashSale;
using ProjectFrenzy.Core.Services;
using ProjectFrenzy.Core.ToastNotifications;
using ProjectFrenzy.Core.Validators;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProjectFrenzy.Core.ViewModels
{
  public class TaskEditorViewModel
    : ViewModelBase
  {
    private readonly IFlashsaleService _flashsaleService;
    private readonly ITasksService _tasksService;
    private readonly IToastNotificationManager _toasts;
    private readonly IEmailService _emailService;
    private readonly IFlashsalesApiClient _flashsalesApiClient;
    private readonly IEmulatorService _emulatorService;
    private static readonly string[] DefaultSizes =
    {
      "default",
      "S",
      "L",
      "XL",
      "M",
      "4.5",
      "5",
      "5.5",
      "6",
      "6.5",
      "7",
      "7.5",
      "8",
      "8.5",
      "9",
      "9.5",
      "10",
      "10.5",
      "11",
      "11.5",
      "12",
      "12.5",
      "13",
    };

    private readonly ReadOnlyObservableCollection<Profile> _profiles;
    private readonly ReadOnlyObservableCollection<Emulator> _emulators;
    private readonly ReadOnlyObservableCollection<Email> _emails;
    private readonly ReadOnlyObservableCollection<FlashsaleItemPickerViewModel> _drops;

    public TaskEditorViewModel(IProfilesService profilesService, IFlashsaleService flashsaleService,
      ITasksService tasksService, FrenzyCheckoutTaskValidator taskValidator, IToastNotificationManager toasts,
      IMessageBus messageBus, IEmailService emailService, IFlashsalesApiClient flashsalesApiClient,
      IEmulatorService emulatorService)
    {
      _flashsaleService = flashsaleService;
      _tasksService = tasksService;
      _toasts = toasts;
      _emailService = emailService;
      _flashsalesApiClient = flashsalesApiClient;
      _emulatorService = emulatorService;
      profilesService.Profiles.Connect()
        .Bind(out _profiles)
        .DisposeMany()
        .Subscribe();

      emulatorService.Emulators.Connect()
        .Bind(out _emulators)
        .Subscribe();

      emailService.Emails.Connect()
        .Bind(out _emails)
        .Subscribe();

      flashsaleService.Flashsales.Connect()
        .Filter(_ => _.IsAvailable())
        .Transform(f => new FlashsaleItemPickerViewModel(f))
        .Bind(out _drops)
        .DisposeMany()
        .Subscribe();

      CloseCommand = ReactiveCommand.Create(() => { messageBus.SendMessage(HideModalComponentMessage.Instance); });

      CheckoutModes = Enum.GetValues(typeof(CheckoutMode))
        .OfType<CheckoutMode>()
        .ToArray();

      var whenDropChanged = this.WhenAnyValue(_ => _.SelectedDrop);
      whenDropChanged
        .Select(d => d != null)
        .ToPropertyEx(this, _ => _.IsDropSelected);

      whenDropChanged.Select(d =>
        {
          if (d == null)
          {
            return Array.Empty<ProductItemPickerViewModel>();
          }

          return d.Flashsale.ProductDetails.Select(p => new ProductItemPickerViewModel(p, d.Flashsale.Shop.Currency))
            .ToArray();
        })
        .ToPropertyEx(this, _ => _.Products);

      this.WhenAnyValue(_ => _.Mode)
        .Select(m => m != CheckoutMode.Random)
        .ToPropertyEx(this, _ => _.IsSizeSelectVisible);

      this.WhenAnyValue(_ => _.IsSizeSelectVisible)
        .CombineLatest(this.WhenAnyValue(_ => _.Sizes),
          (m, s) => m && s.Any())
        .ToPropertyEx(this, _ => _.CanSizeBeSelected);

      ClearDropSelectionCommand = ReactiveCommand.Create(() => { SelectedDrop = null; });

      var canSave = new BehaviorSubject<bool>(false);
      SaveCommand = ReactiveCommand.CreateFromTask(async () => { await SaveAsync(); }, canSave);
      RemoveSelectedSizeCommand = ReactiveCommand.Create<string>(size =>
      {
        SelectedSizes.Remove(size);
        Sizes = DefaultSizes.Except(SelectedSizes).ToArray();
      });

      this.WhenAnyValue(_ => _.IsValid)
        .CombineLatest(SaveCommand.IsExecuting, (isValid, isExecuting) => isValid && !isExecuting)
        .ObserveOn(RxApp.MainThreadScheduler)
        .Subscribe(can => canSave.OnNext(can));

      Changed
        .Throttle(TimeSpan.FromMilliseconds(200))
        .Select(_ => taskValidator.Validate(CreateTask(null)).IsValid)
        .ToPropertyEx(this, _ => _.IsValid);

      this.WhenAnyValue(_ => _.SelectedSize)
        .Subscribe(s =>
        {
          if (s == null)
          {
            return;
          }

          SelectedSizes.Add(s);
          Sizes = Sizes.Except(SelectedSizes).ToArray();
          SelectedSize = null;
        });

      AddFlashsaleManuallyCommand = ReactiveCommand.CreateFromTask(AddFlashsaleManuallyAsync);
      ToggleFlashsaleManualInputCommand = ReactiveCommand.Create(() =>
      {
        ShowManualFlashsaleInput = !ShowManualFlashsaleInput;
      });
    }

    private async Task AddFlashsaleManuallyAsync(CancellationToken ct)
    {
      if (_drops.Any(d => d.Flashsale.Password == ManualFlashsalePassword))
      {
        _toasts.Show(ToastContent.Error("This drop already exist.", "Not added"));
        return;
      }

      Flashsale manualFlashsale = await _flashsalesApiClient.GetByPasswordAsync(ManualFlashsalePassword, ct);

      _flashsaleService.AddOrUpdate(manualFlashsale);
      ShowManualFlashsaleInput = false;
      ManualFlashsalePassword = null;

      _toasts.Show(ToastContent.Success("Drop successfully added to list.", "Manually added drop"));
    }

    [Reactive] public string ManualFlashsalePassword { get; set; }
    [Reactive] public bool ShowManualFlashsaleInput { get; set; }

    private async Task SaveAsync()
    {
      if (!_emailService.HasUnusedEmails)
      {
        _toasts.Show(ToastContent.Error("No emails available to use with tasks."));

        return;
      }

      for (int count = 0; count < Quantity; count++)
      {
        Email email;
        if (PreferredEmail != null)
        {
          email = PreferredEmail.IsCatchAll ? _emailService.MaterializeEmail(PreferredEmail) : PreferredEmail;
        }
        else
        {
          email = _emailService.GetUnusedEmail();
        }

        if (email == null)
        {
          _toasts.Show(ToastContent.Warning($"Not enough emails to create {Quantity} tasks. Created {count} only."));
          Quantity = 0;
          break;
        }

        _tasksService.Add(CreateTask(email));
      }

      Reset();
      await CloseCommand.Execute().FirstOrDefaultAsync();
      _toasts.Show(ToastContent.Success("Task created"));
    }

    private FrenzyCheckoutTask CreateTask(Email email)
    {
      return new FrenzyCheckoutTask
      {
        AssignedEmail = email,
        PreferredEmulator = PreferredEmulator,
        SelectedProfile = SelectedProfile,
        Mode = Mode,
        SelectedSizes = SelectedSizes,
        CheckoutDelay = CheckoutDelay,
        UseProxies = UseProxies,
        Flashsale = SelectedDrop?.Flashsale,
        Product = SelectedProduct?.Item as ProductDetail
      };
    }

    public ObservableCollection<string> SelectedSizes { get; } = new ObservableCollection<string>();

    public bool IsValid { [ObservableAsProperty] get; }
    public ReadOnlyObservableCollection<Profile> Profiles => _profiles;
    public ReadOnlyObservableCollection<Emulator> Emulators => _emulators;
    public ReadOnlyObservableCollection<Email> Emails => _emails;
    public IList<ProductItemPickerViewModel> Products { [ObservableAsProperty] get; }
    [Reactive] public IList<string> Sizes { get; private set; } = DefaultSizes;
    public IReadOnlyList<CheckoutMode> CheckoutModes { get; private set; }
    public ReadOnlyObservableCollection<FlashsaleItemPickerViewModel> Drops => _drops;

    [Reactive] public FlashsaleItemPickerViewModel SelectedDrop { get; set; }
    [Reactive] public ProductItemPickerViewModel SelectedProduct { get; set; }

    public bool IsDropSelected { [ObservableAsProperty] get; }
    public bool IsSizeSelectVisible { [ObservableAsProperty] get; }
    public bool CanSizeBeSelected { [ObservableAsProperty] get; }

    [Reactive] public bool UseProxies { get; set; }
    [Reactive] public Profile SelectedProfile { get; set; }
    [Reactive] public int CheckoutDelay { get; set; }
    [Reactive] public int Quantity { get; set; }
    [Reactive] public CheckoutMode Mode { get; set; }
    [Reactive] public string SelectedSize { get; set; }
    [Reactive] public Emulator PreferredEmulator { get; set; }
    [Reactive] public Email PreferredEmail { get; set; }

    public ReactiveCommand<Unit, Unit> CloseCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> SaveCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> ClearDropSelectionCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> AddFlashsaleManuallyCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> ToggleFlashsaleManualInputCommand { get; private set; }


    public ReactiveCommand<string, Unit> RemoveSelectedSizeCommand { get; private set; }

    public void Reset()
    {
      SelectedSizes.Clear();
      PreferredEmulator = null;
      Sizes = DefaultSizes;
      SelectedDrop = null;
      SelectedProduct = null;
      Mode = CheckoutMode.Preference;
      SelectedSize = Sizes.FirstOrDefault();
      UseProxies = false;
      CheckoutDelay = 200;
      Quantity = 1;
      SelectedProfile = _profiles.FirstOrDefault();
      PreferredEmail = null;
    }
  }
}