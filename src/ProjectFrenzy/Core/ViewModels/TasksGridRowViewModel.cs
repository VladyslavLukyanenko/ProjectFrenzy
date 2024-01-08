using System;
using System.Reactive;
using System.Reactive.Linq;
using ProjectFrenzy.Core.Model;
using ProjectFrenzy.Core.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProjectFrenzy.Core.ViewModels
{
  public class TasksGridRowViewModel : ViewModelBase
  {
    // private static readonly List<CheckoutStatus> SuccessStatuses = new List<CheckoutStatus>
    // {
    //   CheckoutStatus.Running,
    //   CheckoutStatus.Created,
    //   CheckoutStatus.CheckoutSuccessful
    // };
    //
    // private static readonly List<CheckoutStatus> RunningStatuses = new List<CheckoutStatus>
    // {
    //   CheckoutStatus.Running,
    //   CheckoutStatus.ProcessingResults,
    //   CheckoutStatus.Submitting,
    //   CheckoutStatus.PendingAuthorization,
    //   CheckoutStatus.SelectingProductVariant,
    //   CheckoutStatus.AwaitingForUpcoming,
    //   CheckoutStatus.DelayUntilCheckoutTime
    // };

    public TasksGridRowViewModel(FrenzyCheckoutTask task, ITasksService tasksService,
      IFrenzyCheckoutTaskExecutor taskExecutor)
    {
      Task = task;
      ProductPicture = task.Product.DefaultPicture;
      ProductTitle = task.Product.Title;
      UseProxies = task.UseProxies;
      ProfileName = task.SelectedProfile.ProfileName;
      CheckoutDelay = task.CheckoutDelay;
      SelectedSizes = string.Join(", " ,task.SelectedSizes);
      AssignedEmail = task.AssignedEmail.Value;
      StoreName = task.Flashsale.Shop.Name;

      ThrownExceptions.Subscribe(exc => Console.WriteLine(exc));
      var statusObs = this.WhenAnyValue(_ => _.Status)
        .Where(s => s != null);

      statusObs
        .Subscribe(_ =>
        {
          IsCheckoutDurationVisible = _.IsSuccessful || _.IsFailed;
          IsDescVisible = !string.IsNullOrEmpty(_.Description);
        });

      statusObs
        .Select(_ => _.IsRunning)
        .ToPropertyEx(this, _ => _.IsRunning);

      var canStart = statusObs.Select(status => !status.IsRunning);
      var canStop = statusObs.Select(status => status.IsRunning);

      canStart.ToPropertyEx(this, _ => _.CanBeRunned);
      canStop.ToPropertyEx(this, _ => _.CanBeStopped);
      canStart.ToPropertyEx(this, _ => _.CanBeRemoved);

      StartCommand = ReactiveCommand.Create(() => { taskExecutor.ExecuteAsync(task); }, canStart);

      StopCommand = ReactiveCommand.Create(() => { taskExecutor.Cancel(task); }, canStop);
      RemoveCommand = ReactiveCommand.Create(() => { tasksService.Remove(task); }, canStart);


      Task.WhenAnyValue(_ => _.Status)
        .ObserveOn(RxApp.MainThreadScheduler)
        .Subscribe(s => Status = s);

      Task.WhenAnyValue(_ => _.CheckoutDuration)
        .ObserveOn(RxApp.MainThreadScheduler)
        .Subscribe(s => CheckoutDuration = s);

      //
      // Task.WhenAnyValue(_ => _.Status)
      //   .ObserveOn(RxApp.MainThreadScheduler)
      //   .ToPropertyEx(this, _ => _.Status);
    }

    public FrenzyCheckoutTask Task { get; }
    public bool IsRunning { [ObservableAsProperty] get; }
    
    [Reactive] public CheckoutStatus Status { get; private set; }
    [Reactive] public TimeSpan CheckoutDuration { get; private set; }
    [Reactive] public string AssignedEmail { get; private set; }
    [Reactive] public bool IsDescVisible { get; set; }
    [Reactive] public bool IsCheckoutDurationVisible { get; set; }
    [Reactive] public string StoreName { get; private set; }
    
    [Reactive] public string ProfileName { get; set; }
    [Reactive] public int CheckoutDelay { get; set; }
    [Reactive] public string ProductPicture { get; set; }
    [Reactive] public string ProductTitle { get; set; }
    [Reactive] public string SelectedSizes { get; set; }
    [Reactive] public bool UseProxies { get; set; }

    public bool CanBeRunned { [ObservableAsProperty] get; }
    public bool CanBeStopped { [ObservableAsProperty] get; }
    public bool CanBeRemoved { [ObservableAsProperty] get; }

    public ReactiveCommand<Unit, Unit> StartCommand { get; set; }
    public ReactiveCommand<Unit, Unit> StopCommand { get; set; }
    public ReactiveCommand<Unit, Unit> RemoveCommand { get; set; }
  }
}