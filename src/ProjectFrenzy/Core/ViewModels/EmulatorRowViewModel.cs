using System.Reactive.Linq;
using ProjectFrenzy.Core.Model;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProjectFrenzy.Core.ViewModels
{
  public class EmulatorRowViewModel : ViewModelBase
  {
    public EmulatorRowViewModel(Emulator emulator)
    {
      Emulator = emulator;
      emulator.WhenAnyValue(_ => _.Ip).ObserveOn(RxApp.MainThreadScheduler)
        .ToPropertyEx(this, _ => _.Ip);

      emulator.WhenAnyValue(_ => _.IsAvailable)
        .ObserveOn(RxApp.MainThreadScheduler)
        .ToPropertyEx(this, _ => _.IsAvailable);
    }
    
    
    public bool IsAvailable { [ObservableAsProperty] get; }
    public string Ip { [ObservableAsProperty] get; }
    public Emulator Emulator { get; }
  }
}