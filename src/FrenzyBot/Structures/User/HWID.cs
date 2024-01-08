using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace FrenzyBot.Structures.User
{
    class HWID
    {
        public static string generateHWID()
        {
            if (OperatingSystem.IsWindows())
            {
                string hwid = CreateMD5(WindowsHWID.Value());
                return hwid;

            } else if (OperatingSystem.IsMacOS())
            {
                string hwid = CreateMD5(Bash("system_profiler SPHardwareDataType | awk '/UUID/ { print $3; }'"));
                return hwid;
            }
            return null;
        }
        public static string CreateMD5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
        private static string Bash(string cmd)
        {
            var escapedArgs = cmd.Replace("\"", "\\\"");

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return result.Trim();
        }
    }
}
