using System;
using System.Reflection;

namespace ProjectFrenzy
{
  public class AppConstants
  {
    public static Version CurrentAppVersion { get; } = Assembly.GetCallingAssembly().GetName().Version;
    public const string ProductName = "Project Frenzy";
    public static string ProductFullName { get; }  = ProductName + " v" + CurrentAppVersion;
  }
}