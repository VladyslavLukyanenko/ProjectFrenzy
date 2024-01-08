using System;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.InteropServices;
using Autofac;
using Avalonia;
using Avalonia.Logging.Serilog;
using Avalonia.ReactiveUI;
using FluentValidation;
using ProjectFrenzy.AvaloniaUI;
using ProjectFrenzy.AvaloniaUI.Infra.Services;
using ProjectFrenzy.AvaloniaUI.Infra.Services.ToastNotifications;
using ProjectFrenzy.Core;
using ProjectFrenzy.Core.Clients;
using ProjectFrenzy.Core.EventHandlers;
using ProjectFrenzy.Core.Services;
using ProjectFrenzy.Core.ToastNotifications;
using ProjectFrenzy.Core.Validators;
using ProjectFrenzy.Core.ViewModels;
using ReactiveUI;
using Sentry;
using Splat;
using Splat.Autofac;

namespace ProjectFrenzy
{
  class Program
  {
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    public static void Main(string[] args)
    {
      WriteLine("Starting up...");
      using (SentrySdk.Init(config =>
      {
        config.Dsn = new Dsn("https://229069d6653f46ab8b6faea5d6e1e22a@o361255.ingest.sentry.io/3811406");
        config.Release = $"projectfrenzy@1.0.3|1.0.0";
      }))
      {
#if !DEBUG
        var location = typeof(Program).Assembly.Location;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && !location.StartsWith("/Applications"))
        {
          var cmd =
            "osascript -e 'set theAlertText to \"Move to Applications Folder\"' -e 'set theAlertMessage to \"Please move the ProjectFrenzy app into the Applications folder and try again.\"' -e 'display alert theAlertText message theAlertMessage as critical buttons {\"Ok\"} default button \"Ok\"'";
          Bash(cmd);

          Environment.Exit(0);
        }
#endif

        WriteLine("Sentry configured.");
        ConfigureIoC();
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
      }
    }

    private static void Bash(string cmd)
    {
      var escapedArgs = cmd.Replace("\"", "\\\"");

      var process = new Process()
      {
        StartInfo = new ProcessStartInfo
        {
          FileName = "/bin/bash",
          Arguments = $"-c \"{escapedArgs}\"",
          RedirectStandardOutput = true,
          UseShellExecute = false,
          CreateNoWindow = true,
        }
      };
      process.Start();
      process.WaitForExit();
      string result = process.StandardOutput.ReadToEnd();
      // return result.Trim();
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
      => AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .LogToDebug()
        .UseReactiveUI();

    private static void ConfigureIoC()
    {
      WriteLine("Configuring IoC");
      var container = new ContainerBuilder();
      //
      // var configurationBuilder = new ConfigurationBuilder()
      //   .SetBasePath(Directory.GetCurrentDirectory())
      //   .AddJsonFile("appsettings.json", false);
      //
      //   string env = Environment.GetEnvironmentVariable("ENV");
// #if DEBUG
//        if (string.IsNullOrEmpty(env))
//        {
//          env = "Development";
//        }
// #endif
      //
      //  if (!string.IsNullOrEmpty(env))
      //  {
      //    configurationBuilder.AddJsonFile($"appsettings.{env}.json", true);
      //  }
      //
      // var config = configurationBuilder.Build();

      var cfg = new ApplicationConfig
      {
        Security =
        {
          ReauthenticateInternalMillis = 60_000
        },
        ProjectIndustriesApi =
        {
#if !DEBUG
          ApiHostName = "https://api.projectindustries.gg"
#else
          ApiHostName = "http://localhost:3000"
#endif
        }
      };
      // config.Bind(cfg);

      container.RegisterInstance(cfg);
      container.RegisterInstance(cfg.Security);
      container.RegisterInstance(cfg.ProjectIndustriesApi);
      WriteLine("Configured cfg");

      container.RegisterInstance(new HttpClient());

      container.RegisterInstance(new RoutingState())
        .SingleInstance()
        .AsSelf();

      container.RegisterAssemblyTypes(typeof(ViewModelBase).Assembly)
        .AssignableTo<ViewModelBase>()
        .AsSelf()
        .InstancePerLifetimeScope();
      container.RegisterAssemblyTypes(typeof(ViewModelBase).Assembly)
        .AssignableTo<IValidator>()
        .AsSelf()
        .InstancePerLifetimeScope();

      container.RegisterAssemblyTypes(typeof(ViewModelBase).Assembly)
        .AssignableTo<IApplicationEventHandler>()
        .As<IApplicationEventHandler>()
        .InstancePerLifetimeScope();

      container.RegisterType<InMemoryTasksService>()
        .As<ITasksService>()
        .InstancePerLifetimeScope();

      container.RegisterType<ApplicationEventsManager>()
        .As<IApplicationEventsManager>()
        .InstancePerLifetimeScope();

      container.RegisterType<SettingsProfilesService>()
        .As<IProfilesService>()
        .InstancePerLifetimeScope();

      container.RegisterType<SettingsProxiesService>()
        .As<IProxiesService>()
        .InstancePerLifetimeScope();

      container.RegisterType<CountriesService>()
        .As<ICountriesService>()
        .InstancePerLifetimeScope();

      container.RegisterType<ProductsApiClient>()
        .As<IProductsApiClient>()
        .InstancePerLifetimeScope();

      container.RegisterType<FlashsalesApiClient>()
        .As<IFlashsalesApiClient>()
        .InstancePerLifetimeScope();

      container.RegisterType<FlashsaleService>()
        .As<IFlashsaleService>()
        .InstancePerLifetimeScope();

      container.RegisterType<IdentityService>()
        .As<IIdentityService>()
        .InstancePerLifetimeScope();

      container.RegisterType<SettingsEmulatorService>()
        .As<IEmulatorService>()
        .InstancePerLifetimeScope();

      container.RegisterType<LicenseKeyApiClient>()
        .As<ILicenseKeyApiClient>()
        .InstancePerLifetimeScope();

      container.RegisterType<LicenseKeyProvider>()
        .As<ILicenseKeyProvider>()
        .InstancePerLifetimeScope();

      container.RegisterType<DeviceInfoProvider>()
        .As<IDeviceInfoProvider>()
        .InstancePerLifetimeScope();

      container.RegisterType<SecurityManager>()
        .As<ISecurityManager>()
        .InstancePerLifetimeScope();

      container.RegisterType<StatsService>()
        .As<IStatsService>()
        .InstancePerLifetimeScope();

      container.RegisterType<FrenzyApiClient>()
        .As<IFrenzyApiClient>()
        .InstancePerLifetimeScope();

      container.RegisterType<CheckoutApiClient>()
        .As<ICheckoutApiClient>()
        .InstancePerLifetimeScope();

      container.RegisterType<CheckoutService>()
        .As<ICheckoutService>()
        .InstancePerLifetimeScope();

      container.RegisterType<UserDataLocatedSettingsService>()
        .As<ISettingsService>()
        .InstancePerLifetimeScope();

      container.RegisterType<SettingsDiscordSettingsService>()
        .As<IDiscordSettingsService>()
        .InstancePerLifetimeScope();

      container.RegisterType<EmulatorClient>()
        .As<IEmulatorClient>()
        .InstancePerLifetimeScope();

      container.RegisterType<FrenzyCheckoutApiClient>()
        .As<IFrenzyCheckoutApiClient>()
        .InstancePerLifetimeScope();

      container.RegisterType<WebHookManager>()
        .As<IWebHookManager>()
        .InstancePerLifetimeScope();

      container.RegisterType<EmulatorClient>()
        .As<IEmulatorClient>()
        .InstancePerLifetimeScope();

      container.RegisterType<StatsApiClient>()
        .As<IStatsApiClient>()
        .InstancePerLifetimeScope();

      container.RegisterType<TelemetryApiClient>()
        .As<ITelemetryApiClient>()
        .InstancePerLifetimeScope();

      container.RegisterType<HttpClientUpdateApiClient>()
        .As<IUpdateApiClient>()
        .InstancePerLifetimeScope();

      container.RegisterType<UpdatesManager>()
        .As<IUpdatesManager>()
        .InstancePerLifetimeScope();

      container.RegisterType<SoftwareInfoProvider>()
        .As<ISoftwareInfoProvider>()
        .InstancePerLifetimeScope();

      container.RegisterType<RPCManager>()
        .As<IRPCManager>()
        .InstancePerLifetimeScope();

      container.RegisterType<PreloadService>()
        .As<IPreloadService>()
        .InstancePerLifetimeScope();

      container.RegisterType<LicenseKeyStore>()
        .As<ILicenseKeyStore>()
        .InstancePerLifetimeScope();

      container.RegisterType<CountriesApiClient>()
        .As<ICountriesApiClient>()
        .InstancePerLifetimeScope();

      container.RegisterType<AvaloniaToastNotificationManager>()
        .As<IToastNotificationManager>()
        .InstancePerLifetimeScope();

      container.RegisterType<ActiveWindowNotificationsHostProvider>()
        .As<INotificationsHostProvider>()
        .InstancePerLifetimeScope();

      container.RegisterType<ThreadPoolFrenzyCheckoutTaskExecutor>()
        .As<IFrenzyCheckoutTaskExecutor>()
        .InstancePerLifetimeScope();

      container.RegisterType<EmulatorValidator>()
        .AsSelf()
        .InstancePerLifetimeScope();

      container.RegisterType<EmulatorsAvailabilityWatcher>()
        .As<IEmulatorsAvailabilityWatcher>()
        .InstancePerLifetimeScope();

      container.RegisterType<SettingsEmailService>()
        .As<IEmailService>()
        .InstancePerLifetimeScope();

      container.RegisterInstance(MessageBus.Current)
        .As<IMessageBus>()
        .SingleInstance();

      WriteLine("Registered services");

      container.UseAutofacDependencyResolver();
      Locator.CurrentMutable.RegisterViewsForViewModels(typeof(DashboardViewModel).Assembly);

      WriteLine("Configured to use Splat and views for viewmodels");
      //Locator.CurrentMutable.Register<IProfileDialogsService>(() => new ProfileDialogsService(() => ((IClassicDesktopStyleApplicationLifetime)App.Current.ApplicationLifetime).MainWindow));
    }

    private static void WriteLine(string message)
    {
#if DEBUG
      Console.WriteLine(message);
#endif
    }
  }
}