
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;
using System.Windows.Input;

namespace ProjectFrenzy.Core.ViewModels
{
    public class NavigationButtonItemViewModel
        : ViewModelBase
    {
        [Reactive] public bool IsActivated { get; set; }
        public string ActiveIconSrc { get; set; }
        public string NormalIconSrc { get; set; }
        public ReactiveCommand<Unit, Unit> Command { get; set; }
    }
}
