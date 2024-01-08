using System;
using System.Reactive;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

using ProjectFrenzy.Core.ViewModels;
using ReactiveUI;

namespace ProjectFrenzy.AvaloniaUI.Views
{
  public class UpdateInfoView : ReactiveUserControl<UpdateInfoViewModel>
  {
    public UpdateInfoView()
    {
      this.InitializeComponent();
      this.WhenActivated(d =>
      {
        ViewModel.CheckForUpdatesCommand.Execute(Unit.Default).Subscribe();
        ViewModel.LaunchUpdaterCommand.Subscribe(_ =>
        {
          ((IClassicDesktopStyleApplicationLifetime) Application.Current.ApplicationLifetime).Shutdown();
        });
      });
    }

    private void InitializeComponent()
    {
      AvaloniaXamlLoader.Load(this);
    }
  }
}
