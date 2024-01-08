using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using ProjectFrenzy.Core.Model;
using ProjectFrenzy.Core.ViewModels;

namespace ProjectFrenzy.Core.Services
{
    public interface IProfilesService
    {
        Task InitializeAsync(CancellationToken ct = default);
        IObservableCache<Profile, string> Profiles { get; }
        Task AddOrUpdateAsync(Profile profile, CancellationToken ct = default);
        Task RemoveAsync(Profile profile, CancellationToken ct = default);
    }
}