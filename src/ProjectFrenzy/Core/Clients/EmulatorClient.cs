using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ProjectFrenzy.Core.Clients.Cryptography;
using ProjectFrenzy.Core.Model;
using ProjectFrenzy.Core.Model.AndroidPayload;
using ProjectFrenzy.Core.Model.AndroidResponse;
using ProjectFrenzy.Core.Services;

namespace ProjectFrenzy.Core.Clients
{
  public class EmulatorClient : IEmulatorClient
  {
    private readonly ILicenseKeyProvider _licenseKeyProvider;
    private readonly IFrenzyApiClient _frenzyApiClient;
    private readonly IEmulatorService _emulatorService;
    private static readonly AES AndroidEncryption = new AES("6jxaz2jwsnf0a7kw2k7dqf7k62apknua");
    private static readonly SemaphoreSlim RootSync = new SemaphoreSlim(1, 1);

    private static readonly IDictionary<string, SemaphoreSlim> EmulatorQueueLimiter =
      new Dictionary<string, SemaphoreSlim>();

    public EmulatorClient(ILicenseKeyProvider licenseKeyProvider, IFrenzyApiClient frenzyApiClient,
      IEmulatorService emulatorService)
    {
      _licenseKeyProvider = licenseKeyProvider;
      _frenzyApiClient = frenzyApiClient;
      _emulatorService = emulatorService;
    }

    public AndroidResponse GetToken(AndroidPayload payload, TokenRequestLifetimeCallbacks callbacks,
      Emulator preferredEmulator = null, CancellationToken ct = default)
    {
      Emulator emulator = preferredEmulator ?? _emulatorService.GetAvailableEmulator();
      if (emulator == null)
      {
        return null;
      }

      HttpClient client = new HttpClient {Timeout = TimeSpan.FromHours(1)};
      var body = AndroidEncryption.Encrypt(JsonConvert.SerializeObject(payload));
      // SemaphoreSlim limiter = await GetEmulatorLimitSemaphore(ct, callbacks, emulator);
      // Monitor.Enter(emulator);
      // try
      // {
      int attempt = 2;
      HttpResponseMessage androidResp;
      lock (emulator)
      {
        callbacks.EmulatorSelected?.Invoke();
        while (true)
        {
          try
          {
            androidResp = client.GetAsync($"{emulator.GetUrl()}/payload/{body}", ct).GetAwaiter().GetResult();
            androidResp.EnsureSuccessStatusCode();

            var content = androidResp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            
            var decryptedContent = _frenzyApiClient
              .DecodeAsync(_licenseKeyProvider.CurrentLicenseKey, content, ct)
              .GetAwaiter()
              .GetResult();

            return JsonConvert.DeserializeObject<AndroidResponse>(decryptedContent);
          }
          catch (Exception exc)
          {
            if (exc is TaskCanceledException || exc is OperationCanceledException ||
                exc is HttpRequestException || exc is JsonException)
            {
              if (ct.IsCancellationRequested)
              {
                throw;
              }

              RetryAsync(callbacks, () => attempt++, ct).GetAwaiter().GetResult();
            }
            else
            {
              throw;
            }
          }
        }
      }

      // Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
      // Console.WriteLine(Task.CurrentId);
      // limiter.Release();


      // }
      // catch (OperationCanceledException)
      // {
      // throw;
      // }

      // finally
      // {
      //   limiter?.Release();
      // }
    }

    private static async Task RetryAsync(TokenRequestLifetimeCallbacks callbacks, Func<int> attemptProvider,
      CancellationToken ct)
    {
      callbacks.RetryAuthorizationFailed?.Invoke(attemptProvider());
      await Task.Delay(300, ct);
    }

    private static async Task<SemaphoreSlim> GetEmulatorLimitSemaphore(CancellationToken ct,
      TokenRequestLifetimeCallbacks callbacks, Emulator emulator)
    {
      SemaphoreSlim limiter;
      await RootSync.WaitAsync(ct);
      if (!EmulatorQueueLimiter.TryGetValue(emulator.Ip, out limiter))
      {
        limiter = new SemaphoreSlim(1, 1);
        EmulatorQueueLimiter[emulator.Ip] = limiter;
      }

      RootSync.Release();

      await limiter.WaitAsync(ct);
      callbacks.EmulatorSelected?.Invoke();

      return limiter;
    }
  }
}