using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using ProjectFrenzy.Core.Model;
using ProjectFrenzy.Core.Services;
using ReactiveUI;

namespace ProjectFrenzy.Core.ViewModels
{
  public class TasksGridViewModel
    : ViewModelBase, IRoutableViewModel
  {
    private readonly ReadOnlyObservableCollection<TasksGridRowViewModel> _tasks;

    public TasksGridViewModel(ITasksService tasksService, IFrenzyCheckoutTaskExecutor taskExecutor,
      TaskEditorViewModel taskEditor, IScreen hostScreen, IMessageBus messageBus)
    {
      HostScreen = hostScreen;
      var taskRows = tasksService.Tasks.Connect()
        .Transform(t => new TasksGridRowViewModel(t, tasksService, taskExecutor));
      taskRows
        .Bind(out _tasks)
        .DisposeMany()
        .Subscribe();

      CreateTaskCommand = ReactiveCommand.Create<bool>(show =>
      {
        taskEditor.Reset();
        messageBus.SendMessage(new ShowModalComponentMessage(taskEditor));
      });

      var hasNotStartedTasks = taskRows
        .AutoRefresh(_ => _.Status)
        .Select(c => _tasks.Any(_ => !_.IsRunning));

      var hasRunningTasks = taskRows
        .AutoRefresh(_ => _.Status)
        .Select(c => _tasks.Any(_ => _.IsRunning));

      StartTasksCommand = ReactiveCommand.Create(() =>
        {
          taskExecutor.ExecuteAsync(_tasks.Select(_ => _.Task));
        }, hasNotStartedTasks);
      StopTasksCommand = ReactiveCommand.Create(taskExecutor.CancelAllTasks, hasRunningTasks);
    }

    public ReadOnlyObservableCollection<TasksGridRowViewModel> Tasks => _tasks;

    public ReactiveCommand<Unit, Unit> StartTasksCommand { get; set; }
    public ReactiveCommand<Unit, Unit> StopTasksCommand { get; set; }


    public ReactiveCommand<bool, Unit> CreateTaskCommand { get; private set; }
    public string UrlPathSegment => nameof(TasksGridViewModel);
    public IScreen HostScreen { get; }
  }
}