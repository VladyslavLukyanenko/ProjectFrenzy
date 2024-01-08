using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData;
using ProjectFrenzy.Core.Events;
using ProjectFrenzy.Core.Model;
using ProjectFrenzy.Core.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProjectFrenzy.Core.ViewModels
{
  public class DashboardViewModel
    : ViewModelBase, IRoutableViewModel
  {
    private readonly ReadOnlyObservableCollection<CheckedOutProductViewModel> _products;

    public DashboardViewModel(IIdentityService identityService, ICheckoutService checkoutService, IScreen hostScreen,
      IStatsService statsService, IMessageBus messageBus)
    {
      HostScreen = hostScreen;
      identityService.CurrentUser.Subscribe(u => CurrentUser = u);
      statsService.Stats
        .ObserveOn(RxApp.MainThreadScheduler)
        .Subscribe(s => Stats = s);

      checkoutService.LastCheckouts.Connect()
        .ObserveOn(RxApp.MainThreadScheduler)
        .Transform(p => new CheckedOutProductViewModel(p))
        .Bind(out _products)
        .DisposeMany()
        .Subscribe();

      messageBus.Listen<FrenzyCheckoutTaskCompleted>()
        .Throttle(TimeSpan.FromSeconds(5))
        .Subscribe(async m => { await Task.WhenAll(checkoutService.RefreshAsync(), statsService.RefreshAsync()); });

      this.WhenAnyValue(_ => _.Stats, _ => _.SuccessfulCheckoutsDisplayPeriod, (s, _) => s)
        .Where(s => s != null)
        .Select(s => s.SuccessfulCheckouts)
        .Select(c =>
        {
          return SuccessfulCheckoutsDisplayPeriod switch
          {
            StatsPeriodRange.Daily => c.Daily,
            StatsPeriodRange.Monthly => c.Monthly,
            StatsPeriodRange.Yearly => c.Yearly,
            _ => throw new ArgumentOutOfRangeException()
          };
        })
        .ToPropertyEx(this, _ => _.SuccessfulCheckoutsCount);

      ChangeSuccessfulCheckoutsDisplayPeriodCommand = ReactiveCommand.Create<StatsPeriodRange>(d =>
      {
        SuccessfulCheckoutsDisplayPeriod = d;
      });

      this.WhenAnyValue(_ => _.Stats, _ => _.FailedCheckoutsDisplayPeriod, (s, _) => s)
        .Where(s => s != null)
        .Select(s => s.FailedCheckouts)
        .Select(c =>
        {
          return FailedCheckoutsDisplayPeriod switch
          {
            StatsPeriodRange.Daily => c.Daily,
            StatsPeriodRange.Monthly => c.Monthly,
            StatsPeriodRange.Yearly => c.Yearly,
            _ => throw new ArgumentOutOfRangeException()
          };
        })
        .ToPropertyEx(this, _ => _.FailedCheckoutsCount);

      ChangeFailedCheckoutsDisplayPeriodCommand = ReactiveCommand.Create<StatsPeriodRange>(d =>
      {
        FailedCheckoutsDisplayPeriod = d;
      });

      this.WhenAnyValue(_ => _.Stats, _ => _.ChartDisplayPeriod, (s, _) => s)
        .Where(s => s != null)
        .Select(s => s.Chart)
        .Select(c =>
        {
          ChartData data;
          switch (ChartDisplayPeriod)
          {
            case StatsPeriodRange.Daily:
              data = c.Daily;
              MinDate = DateTime.Now.Date;
              MaxDate = DateTime.Now.Date.AddDays(1).AddMilliseconds(-1);
              break;
            case StatsPeriodRange.Monthly:
              data = c.Monthly;
              MinDate = DateTime.Now.Date
                .AddDays((DateTime.Now.Day - 1) * -1);

              MaxDate = MinDate.AddDays(DateTime.DaysInMonth(MinDate.Year, MinDate.Month) - 1)
                .AddDays(1)
                .AddMilliseconds(-1);
              break;
            case StatsPeriodRange.Yearly:
              data = c.Yearly;
              MinDate = new DateTime(DateTime.Now.Year, 1, 1);
              MaxDate = DateTime.Now.Date.AddDays(1).AddMilliseconds(-1);
              break;
            default:
              throw new ArgumentOutOfRangeException();
          }

          data.Entries = data.Entries.OrderBy(_ => _.SpendingDate).ToArray();

          return data;
        })
        .ToPropertyEx(this, _ => _.ChartData);

      ChangeChartDisplayPeriodCommand = ReactiveCommand.Create<StatsPeriodRange>(d => { ChartDisplayPeriod = d; });
    }


    [Reactive] public StatsPeriodRange SuccessfulCheckoutsDisplayPeriod { get; set; }
    public ReactiveCommand<StatsPeriodRange, Unit> ChangeSuccessfulCheckoutsDisplayPeriodCommand { get; }
    public int SuccessfulCheckoutsCount { [ObservableAsProperty] get; }

    [Reactive] public StatsPeriodRange FailedCheckoutsDisplayPeriod { get; set; }
    public ReactiveCommand<StatsPeriodRange, Unit> ChangeFailedCheckoutsDisplayPeriodCommand { get; }
    public int FailedCheckoutsCount { [ObservableAsProperty] get; }


    [Reactive] public StatsPeriodRange ChartDisplayPeriod { get; set; }
    public ReactiveCommand<StatsPeriodRange, Unit> ChangeChartDisplayPeriodCommand { get; }
    public ChartData ChartData { [ObservableAsProperty] get; }


    [Reactive] public FrenzyStatisticsResponseModel Stats { get; private set; }
    [Reactive] public DateTime MinDate { get; private set; }
    [Reactive] public DateTime MaxDate { get; private set; }


    public ReadOnlyObservableCollection<CheckedOutProductViewModel> Products => _products;
    [Reactive] public User CurrentUser { get; private set; }


    public IScreen HostScreen { get; }
    public string UrlPathSegment => nameof(DashboardViewModel);
  }

  public enum StatsPeriodRange
  {
    Daily,
    Monthly,
    Yearly
  }

  //
  //
  // public class StatisticEntry
  // {
  //     public decimal Revenue { get; set; }
  //     public DateTime DayDate { get; set; }
  //     public string FormattedDate => DayDate.ToString("MMMM dd");
  //
  //     public override string ToString()
  //     {
  //         return String.Format("{0:C2} {1:MMMM dd}", Revenue, DayDate);
  //     }
  // }
}