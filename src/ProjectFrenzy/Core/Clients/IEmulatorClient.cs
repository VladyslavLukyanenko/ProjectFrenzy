using ProjectFrenzy.Core.Model;
using ProjectFrenzy.Core.Model.AndroidPayload;
using ProjectFrenzy.Core.Model.AndroidResponse;

using System;
using System.Threading;

namespace ProjectFrenzy.Core.Clients
{
  public interface IEmulatorClient
  {
    AndroidResponse GetToken(AndroidPayload payload, TokenRequestLifetimeCallbacks callbacks,
      Emulator preferredEmulator = null, CancellationToken ct = default);
  }

  public class TokenRequestLifetimeCallbacks
  {
    public Action EmulatorSelected { get; set; }
    public Action<int> RetryAuthorizationFailed { get; set; }
  }
}