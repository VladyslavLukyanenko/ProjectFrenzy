using System.Threading;
using System.Threading.Tasks;
using ProjectFrenzy.Core.Model;

namespace ProjectFrenzy.Core.Services
{
  public interface IEmulatorsAvailabilityWatcher
  {
    void Spawn();
    Task<bool> PingAsync(Emulator emulator, CancellationToken ct = default);
    Task UpdateAvailabilityAsync(Emulator emulator, CancellationToken ct = default);
  }
}