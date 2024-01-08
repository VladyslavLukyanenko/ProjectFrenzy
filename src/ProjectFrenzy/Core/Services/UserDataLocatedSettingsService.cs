using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ProjectFrenzy.Core.Services
{
  public class UserDataLocatedSettingsService : ISettingsService
  {
    private const string StorageFolderName = "ProjectFrenzy";

    private static readonly string StorageLocation =
      Path.Combine(GetLocalAppDataFolder(), StorageFolderName);

    private static readonly SemaphoreSlim ReadSemaphore = new SemaphoreSlim(1, 1);
    private static readonly SemaphoreSlim WriteSemaphore = new SemaphoreSlim(1, 1);

    private static string GetLocalAppDataFolder()
    {
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
      {
        return Environment.GetEnvironmentVariable("LOCALAPPDATA");
      }

      if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
      {
        return Environment.GetEnvironmentVariable("XDG_DATA_HOME") ??
               Path.Combine(Environment.GetEnvironmentVariable("HOME"), ".local", "share");
      }

      if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
      {
        return Path.Combine(Environment.GetEnvironmentVariable("HOME"), "Library", "Application Support");
      }

      throw new NotImplementedException("Unknown OS Platform");
    }

    private static void WriteLine(string message, params object[] args)
    {
#if DEBUG
      Console.WriteLine(message, args);
#endif
    }

    public async Task<T> ReadSettingsOrDefaultAsync<T>(string name, Func<T> defaultFactory = null,
      CancellationToken ct = default)
    {
      try
      {
        await ReadSemaphore.WaitAsync(ct);
        var fullPath = GetSettingsFullPathOrDefault(name);
        WriteLine($"Reading settings from path '{fullPath}' by key '{name}'");
        if (!File.Exists(fullPath))
        {
          WriteLine("File not found. Returning default value");
          return defaultFactory != null ? defaultFactory.Invoke() : default;
        }

        WriteLine("File found. Reading...");
        var content = await File.ReadAllTextAsync(fullPath, ct);

        WriteLine("Deserializing");
        return JsonConvert.DeserializeObject<T>(content);
      }
      finally
      {
        WriteLine("Read finished");
        ReadSemaphore.Release();
      }
    }

    public async Task WriteSettingsAsync<T>(string name, T settings, CancellationToken ct = default)
    {
      try
      {
        await WriteSemaphore.WaitAsync(ct);
        var fullPath = GetSettingsFullPathOrDefault(name);
        var json = JsonConvert.SerializeObject(settings);
        await File.WriteAllTextAsync(fullPath, json, ct);
      }
      finally
      {
        WriteSemaphore.Release();
      }
    }

    private static string GetSettingsFullPathOrDefault(string name)
    {
      if (!Directory.Exists(StorageLocation))
      {
        WriteLine("Store location doesn't exists. Creating '{0}'", StorageLocation);
        Directory.CreateDirectory(StorageLocation);
        WriteLine("Store location created");
      }

      return Path.Combine(StorageLocation, name);
    }
  }
}