using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ProjectFrenzy.Core.ViewModels;

namespace ProjectFrenzy.AvaloniaUI.Views
{
    public class TasksGridView : ReactiveUserControl<TasksGridViewModel>
    {
        public TasksGridView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
