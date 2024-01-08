using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using ProjectFrenzy.Core.Model;

namespace ProjectFrenzy.Core.Services
{
    public interface IEmulatorService
    {
        IObservableCache<Emulator, string> Emulators { get; }
        Task RemoveAsync(Emulator toRemove, CancellationToken ct = default);
        Task AddOrUpdateAsync(Emulator toAdd, CancellationToken ct = default);
        Emulator GetAvailableEmulator();
    }
}