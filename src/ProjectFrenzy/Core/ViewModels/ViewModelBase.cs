using ProjectFrenzy.AvaloniaUI;
using ReactiveUI;

namespace ProjectFrenzy.Core.ViewModels
{
    public class ViewModelBase
        : ReactiveObject
    {
        public string ApplicationFullName => AppConstants.ProductFullName;
    }
}