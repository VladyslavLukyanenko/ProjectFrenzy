using System;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using OxyPlot;
using OxyPlot.Avalonia;
using ProjectFrenzy.Core.ViewModels;
using ReactiveUI;

namespace ProjectFrenzy.AvaloniaUI.Views
{
    public class DashboardView : ReactiveUserControl<DashboardViewModel>
    {
        public DashboardView()
        {
            this.InitializeComponent();
            PlotController = new PlotController();
            PlotController.UnbindMouseDown(OxyMouseButton.Left);
            PlotController.BindMouseEnter(PlotCommands.HoverSnapTrack);

            this.WhenActivated(d =>
            {
                var chart = this.FindControl<Plot>("Chart");
                ViewModel.WhenAnyValue(v => v.ChartData)
                    .Where(c => c != null)
                    .Subscribe(c =>
                    {
                        chart.InvalidatePlot();
                    });
            });
        }

        public PlotController PlotController { get; private set; }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
