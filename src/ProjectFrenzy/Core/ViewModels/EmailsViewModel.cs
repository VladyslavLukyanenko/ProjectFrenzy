using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using ProjectFrenzy.Core.Model;
using ProjectFrenzy.Core.Services;
using ProjectFrenzy.Core.ToastNotifications;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProjectFrenzy.Core.ViewModels
{
  public class EmailsViewModel : ViewModelBase, IRoutableViewModel
  {
    private readonly IEmailService _emailService;
    private readonly IToastNotificationManager _toasts;
    private readonly ReadOnlyObservableCollection<Email> _emails;
    public EmailsViewModel(IEmailService emailService, IScreen hostScreen, IToastNotificationManager toasts)
    {
      _emailService = emailService;
      _toasts = toasts;

      HostScreen = hostScreen;
      emailService.Emails.Connect()
        .Bind(out _emails)
        .DisposeMany()
        .Subscribe();

      emailService.Emails.Connect()
        .CountChanged()
        .Select(_ => _emails.Count)
        .ToPropertyEx(this, _ => _.Count);

      var canBeCreated = this.WhenAnyValue(_ => _.RawEmails).Select(r => !string.IsNullOrEmpty(r));
      CreateEmailsCommand = ReactiveCommand.CreateFromTask(CreateEmailsAsync, canBeCreated);
      RemoveEmailCommand = ReactiveCommand.CreateFromTask<Email>(RemoveEmailAsync);
    }

    private async Task RemoveEmailAsync(Email p, CancellationToken ct)
    {
      await _emailService.RemoveAsync(p, ct);
      _toasts.Show(ToastContent.Success("Email removed."));
    }

    private async Task CreateEmailsAsync(CancellationToken ct)
    {
      if (string.IsNullOrEmpty(RawEmails))
      {
        return;
      }

      var validEmails = new List<Email>();
      var invalidEmails = new List<string>();
      foreach (var raw in RawEmails.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries))
      {
        if (Email.TryParse(raw, out var email))
        {
          validEmails.Add(email);
        }
        else
        {
          invalidEmails.Add(raw);
        }
      }

      await _emailService.CreateAsync(validEmails, ct);
      if (invalidEmails.Any())
      {
        _toasts.Show(ToastContent.Error(
          "Some of emails are malformed. Please check them and try again", "Can't save all emails"));
        RawEmails = string.Join(Environment.NewLine, invalidEmails);
      }
      else
      {
        _toasts.Show(ToastContent.Success("Emails are saved."));
        RawEmails = null;
      }
    }

    public ReadOnlyObservableCollection<Email> Emails => _emails;

    [Reactive] public string RawEmails { get; set; }

    public ReactiveCommand<Unit, Unit> CreateEmailsCommand { get; set; }
    public ReactiveCommand<Email, Unit> RemoveEmailCommand { get; set; }

    public int Count { [ObservableAsProperty] get; }

    public string UrlPathSegment => nameof(EmailsViewModel);
    public IScreen HostScreen { get; }
  }
}