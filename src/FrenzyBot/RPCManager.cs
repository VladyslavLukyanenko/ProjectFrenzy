using DiscordRPC;
using FrenzyBot.Structures.User;

namespace FrenzyBot
{
    public static class RPCManager
    {
        public static DiscordRpcClient RPCClient;

        public static void UpdateState(string state)
        {

            RPCClient = new DiscordRpcClient("686697186821799945");

            RPCClient.Initialize();

            RPCClient.SetPresence(new RichPresence()
            {
                Details = $"v{Versions.program_version}",
                State = state,
                Timestamps = Timestamps.Now,
                Assets = new Assets()
                {
                    LargeImageKey = "rpcprojectfrenzylogo",
                    LargeImageText = "Project Frenzy",
                }
            });
        }
    }
}
