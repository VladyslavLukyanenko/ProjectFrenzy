using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ProjectFrenzy.AvaloniaUI.Views;
using ProjectFrenzy.Core.Services;
using ProjectFrenzy.Core.ViewModels;
using ReactiveUI;
using Splat;

namespace ProjectFrenzy.AvaloniaUI
{
  public class App : Application
  {
    public override void Initialize()
    {
      AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
      WriteLine("Framework initialized");
      if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime)
      {
        Locator.CurrentMutable.RegisterViewsForViewModels(typeof(MainWindowView).Assembly);
        var vm = Locator.Current.GetService<MainWindowViewModel>();
        Locator.CurrentMutable.RegisterConstant<IScreen>(vm);

        Locator.CurrentMutable.Register(() => new MainWindowView
        {
          DataContext = vm,
          ViewModel = vm
        });

        Locator.CurrentMutable.Register(() =>
        {
          var loginVm = Locator.Current.GetService<LoginViewModel>();
          return new LoginView
          {
            DataContext = loginVm,
            ViewModel = loginVm
          };
        });

        WriteLine("Spawning lifetime services");
        Resolve<ISecurityManager>().Spawn();
        Resolve<IWebHookManager>().Spawn();
        Resolve<IApplicationEventsManager>().Spawn();
        Resolve<IEmulatorsAvailabilityWatcher>().Spawn();
        Resolve<IUpdatesManager>().Spawn();
        Resolve<IRPCManager>().UpdateState("Cooking Frenzy!");
        WriteLine("Lifetime services are spawned");
      }

      base.OnFrameworkInitializationCompleted();
    }


    private static void WriteLine(string message, params object[] args)
    {
#if DEBUG
      Console.WriteLine(message, args);
#endif
    }
    
    private static T Resolve<T>()
    {
      return Locator.Current.GetService<T>();
    }
  }
}