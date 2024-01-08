using System.Reflection;

[assembly: Obfuscation(
  Feature = "4. encrypt symbol names with password YGtu9NQfZWoATcAlVrCtnHrHesIVZXSDpXwCDLxor2guuPwL6KVi1gBD785LShqs",
  Exclude = false)]
[assembly: Obfuscation(
  Feature =
    "5. apply to type ProjectFrenzy.Core.*: all",
  Exclude = false, ApplyToMembers = true)]
[assembly:
  Obfuscation(Feature = "2. apply to type ProjectFrenzy.AvaloniaUI.*: renaming", Exclude = true, ApplyToMembers = true)]
[assembly:
  Obfuscation(Feature = "2. apply to type ProjectFrenzy.Core.Services.ApplicationEventsManager: renaming", Exclude = true, ApplyToMembers = true)]
[assembly:
  Obfuscation(Feature = "2. apply to type ProjectFrenzy.Core.Services.IApplicationEventsManager: renaming", Exclude = true, ApplyToMembers = true)]
[assembly: Obfuscation(Feature = "2. apply to type * when enum: renaming", Exclude = true, ApplyToMembers = true)]
[assembly:
  Obfuscation(Feature = "2. apply to type ProjectFrenzy.Core.Model.*: renaming", Exclude = true, ApplyToMembers = true)]
[assembly:
  Obfuscation(Feature = "2. apply to type ProjectFrenzy.Core.ViewModels.*: renaming", Exclude = true,
    ApplyToMembers = true)]

[assembly: Obfuscation(Feature = "1. apply to type CompiledAvaloniaXaml.*: apply to member *: renaming", Exclude = true,
  ApplyToMembers = true)]