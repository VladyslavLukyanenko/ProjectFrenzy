using System.Threading;
using System.Threading.Tasks;

namespace ProjectFrenzy.Core.Services
{
  public class LicenseKeyStore : ILicenseKeyStore
  {
    private const string SettingsFileName = "Key.projectfrenzy";
    private readonly ISettingsService _settingsService;

    public LicenseKeyStore(ISettingsService settingsService)
    {
      _settingsService = settingsService;
    }

    public async Task<string> GetStoredKeyAsync(CancellationToken ct = default)
    {
      return await _settingsService.ReadSettingsOrDefaultAsync<string>(SettingsFileName, ct: ct);
    }

    public async Task StoreKeyAsync(string key, CancellationToken ct = default)
    {
      await _settingsService.WriteSettingsAsync(SettingsFileName, key, ct);
    }
  }
}