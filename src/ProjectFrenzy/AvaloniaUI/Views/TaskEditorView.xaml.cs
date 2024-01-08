using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ProjectFrenzy.AvaloniaUI.Infra.Services;
using ProjectFrenzy.Core.ViewModels;
using ReactiveUI;
using Splat;

namespace ProjectFrenzy.AvaloniaUI.Views
{
    public class TaskEditorView
        : ReactiveUserControl<TaskEditorViewModel>
    {
        public TaskEditorView()
        {
            this.InitializeComponent();
            this.WhenActivated(d =>
            {
                // var popup = this.FindControl<Popup>("Popup");
                // var cmb = this.FindControl<ComboBox>("ComboBox");
                // popup.PlacementTarget = cmb;
                // popup.Open();
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
