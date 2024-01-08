using System;
using System.Threading;
using System.Threading.Tasks;
using ProjectFrenzy.Core.Model;

namespace ProjectFrenzy.Core.Services
{
    public interface IDiscordSettingsService
    {
        IObservable<DiscordSettings> Settings { get; }
        Task UpdateAsync(DiscordSettings settings, CancellationToken ct = default);
        Task RefreshAsync(CancellationToken ct = default);
    }
}