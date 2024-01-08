using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using ProjectFrenzy.Core.Model;

namespace ProjectFrenzy.Core.Services
{
  public interface IEmailService
  {
    Task InitializeAsync(CancellationToken ct = default);
    IObservableCache<Email, string> Emails { get; }
    Task CreateAsync(IEnumerable<Email> emails, CancellationToken ct = default);
    Task RemoveAsync(Email toRemove, CancellationToken ct = default);

    Email GetUnusedEmail();
    Email MaterializeEmail(Email email);
    void ReleaseEmail(Email email);
    bool HasUnusedEmails { get; }
  }
}