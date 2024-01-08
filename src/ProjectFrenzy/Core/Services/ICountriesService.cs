using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectFrenzy.Core.Model;

namespace ProjectFrenzy.Core.Services
{
    public interface ICountriesService
    {
        Task InitializeAsync(CancellationToken ct = default);
        IReadOnlyList<Country> Countries { get; }
    }
}
