using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using ProjectFrenzy.Core.Model;

namespace ProjectFrenzy.Core.Services
{
  public class SettingsEmailService : IEmailService
  {
    private readonly ISettingsService _settingsService;
    private const string SettingsFileName = "Emails.json";
    private readonly ISourceCache<Email, string> _cache = new SourceCache<Email, string>(_ => _.Value);
    private readonly LinkedList<Email> _availableEmails = new LinkedList<Email>();
    private readonly List<Email> _busyEmails = new List<Email>();
    private static readonly SemaphoreSlim EmailsSemaphoreSlim = new SemaphoreSlim(1, 1);

    public SettingsEmailService(ISettingsService settingsService)
    {
      _settingsService = settingsService;
      Emails = _cache.AsObservableCache();
    }

    public async Task InitializeAsync(CancellationToken ct = default)
    {
      var emails = await _settingsService.ReadSettingsOrDefaultAsync(SettingsFileName, () => new List<Email>(), ct);
      _cache.AddOrUpdate(emails);

      foreach (var email in emails.OrderBy(_ => _.IsCatchAll))
      {
        _availableEmails.AddLast(email);
      }
    }

    public IObservableCache<Email, string> Emails { get; }

    public async Task CreateAsync(IEnumerable<Email> emails, CancellationToken ct = default)
    {
      foreach (var email in emails)
      {
        var result = _cache.Lookup(email.Value);
        if (result.HasValue)
        {
          continue;
        }

        AddAvailableEmail(email);
        _cache.AddOrUpdate(email);
      }

      await _settingsService.WriteSettingsAsync(SettingsFileName, _cache.Items, ct);
    }

    public async Task RemoveAsync(Email toRemove, CancellationToken ct = default)
    {
      if (_availableEmails.Remove(toRemove))
      {
        _availableEmails.Remove(toRemove);
      }

      _busyEmails.Remove(toRemove);

      _cache.Remove(toRemove);
      await _settingsService.WriteSettingsAsync(SettingsFileName, _cache.Items, ct);
    }

    public Email GetUnusedEmail()
    {
      if (!HasUnusedEmails)
      {
        return null;
      }

      Email email;
      EmailsSemaphoreSlim.Wait();
      var currNode = _availableEmails.First;
      var curr = currNode.Value;
      _availableEmails.Remove(currNode);
      curr.IsAllocated = true;
      if (curr.IsCatchAll)
      {
        email = MaterializeEmail(curr);
        _availableEmails.AddLast(currNode);
      }
      else
      {
        email = curr;
        _busyEmails.Add(curr);
      }


      EmailsSemaphoreSlim.Release();

      return email;
    }

    public void ReleaseEmail(Email email)
    {
      EmailsSemaphoreSlim.Wait();
      email.IsAllocated = false;
      // can be removed so we don't have to add it back after it released
      if (!(email is CatchAllEmail) && _busyEmails.Contains(email))
      {
        _busyEmails.Remove(email);
        AddAvailableEmail(email);
      }

      EmailsSemaphoreSlim.Release();
    }

    private void AddAvailableEmail(Email email)
    {
      email.IsAllocated = false;
      var firstNotCatch = _availableEmails.FirstOrDefault(_ => !_.IsCatchAll);
      if (firstNotCatch != null)
      {
        var firstNonCatchAllNode = _availableEmails.Find(firstNotCatch);
        _availableEmails.AddAfter(firstNonCatchAllNode, email);
      }
      else
      {
        _availableEmails.AddFirst(email);
      }
    }

    public bool HasUnusedEmails => _availableEmails.Any();

    private class CatchAllEmail : Email
    {
      public CatchAllEmail(Email source)
        : base(Guid.NewGuid().ToString("N") + source.Value, true)
      {
        if (!source.IsCatchAll)
        {
          throw new InvalidOperationException($"Only IsCatchAll=True can be materialized. '{source.Value}' isn't.");
        }

        Source = source;
      }

      public Email Source { get; private set; }
    }

    public Email MaterializeEmail(Email email) => new CatchAllEmail(email);
  }
}