using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using ProjectFrenzy.Core.Model;

namespace ProjectFrenzy.Core.Services
{
  public class SettingsProfilesService : IProfilesService
  {
    private readonly ISettingsService _settingsService;
    private const string SettingsFileName = "Profiles.json";
    private readonly ISourceCache<Profile, string> _profiles = new SourceCache<Profile, string>(_ => _.ProfileName);

    public SettingsProfilesService(ISettingsService settingsService)
    {
      _settingsService = settingsService;

      Profiles = _profiles.AsObservableCache();
    }

    public IObservableCache<Profile, string> Profiles { get; }

    public async Task InitializeAsync(CancellationToken ct = default)
    {
        var profiles =
          await _settingsService.ReadSettingsOrDefaultAsync(SettingsFileName, () => new List<Profile>(), ct);

        _profiles.AddOrUpdate(profiles);
    }

    public async Task AddOrUpdateAsync(Profile profile, CancellationToken ct = default)
    {
      _profiles.AddOrUpdate(profile);
      await _settingsService.WriteSettingsAsync(SettingsFileName, _profiles.Items, ct);
    }

    public async Task RemoveAsync(Profile profile, CancellationToken ct = default)
    {
      _profiles.Remove(profile);
      await _settingsService.WriteSettingsAsync(SettingsFileName, _profiles.Items, ct);
    }
  }
}