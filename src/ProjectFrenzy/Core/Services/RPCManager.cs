using DiscordRPC;

namespace ProjectFrenzy.Core.Services
{
  public class RPCManager : IRPCManager
  {
    private readonly ISoftwareInfoProvider _softInfo;
    private DiscordRpcClient _rpcClient;

    public RPCManager(ISoftwareInfoProvider softInfo)
    {
      _softInfo = softInfo;
    }

    public void UpdateState(string state)
    {
      if (_rpcClient?.IsInitialized ?? false)
      {
        return;
      }
      
      _rpcClient = new DiscordRpcClient("686697186821799945");
      _rpcClient.Initialize();
      _rpcClient.SetPresence(new RichPresence
      {
        Details = $"v{_softInfo.CurrentSoftwareVersion}",
        State = state,
        Timestamps = Timestamps.Now,
        Assets = new Assets
        {
          LargeImageKey = "rpcprojectfrenzylogo",
          LargeImageText = "Project Frenzy",
        }
      });
    }
  }
}