using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace ProjectFrenzy.Core.ViewModels
{
    public class MainWindowViewModel
        : ViewModelBase, IScreen
    {
        public MainWindowViewModel(RoutingState routingState, IMessageBus messageBus)
        {
            Router = routingState;
            NavigationButtons = new List<NavigationButtonItemViewModel>
            {
                new NavigationButtonItemViewModel
                {
                    ActiveIconSrc = "/Assets/Icons/dashboard_active.png",
                    NormalIconSrc = "/Assets/Icons/dashboard_normal.png",
                    Command = ReactiveCommand.Create(SetActiveScreen<DashboardViewModel>)
                },
                new NavigationButtonItemViewModel
                {
                    ActiveIconSrc = "/Assets/Icons/rocket_active.png",
                    NormalIconSrc = "/Assets/Icons/rocket_normal.png",
                    Command = ReactiveCommand.Create(SetActiveScreen<TasksGridViewModel>),
                },
                new NavigationButtonItemViewModel
                {
                    ActiveIconSrc = "/Assets/Icons/credit-card_active.png",
                    NormalIconSrc = "/Assets/Icons/credit-card_normal.png",
                    Command = ReactiveCommand.Create(SetActiveScreen<ProfilesGridViewModel>)
                },
                new NavigationButtonItemViewModel
                {
                    ActiveIconSrc = "/Assets/Icons/www_active.png",
                    NormalIconSrc = "/Assets/Icons/www_normal.png",
                    Command = ReactiveCommand.Create(SetActiveScreen<ProxiesViewModel>)
                },
                new NavigationButtonItemViewModel
                {
                    ActiveIconSrc = "/Assets/Icons/emails_active.png",
                    NormalIconSrc = "/Assets/Icons/emails_normal.png",
                    Command = ReactiveCommand.Create(SetActiveScreen<EmailsViewModel>)
                },
                new NavigationButtonItemViewModel
                {
                    ActiveIconSrc = "/Assets/Icons/settings_active.png",
                    NormalIconSrc = "/Assets/Icons/settings_normal.png",
                    Command = ReactiveCommand.Create(SetActiveScreen<SettingsViewModel>)
                },
            };

            messageBus.Listen<ShowModalComponentMessage>()
                .Select(_ => _.ViewModel)
                .Subscribe(vm => ModalComponent = vm);

            messageBus.Listen<HideModalComponentMessage>()
                .Subscribe(_ => ModalComponent = null);

            foreach (var item in NavigationButtons)
            {
                item.Command.Subscribe(_ =>
                {
                    foreach (var i in NavigationButtons)
                    {
                        i.IsActivated = i == item;
                    }
                });
            }

            this.WhenAnyValue(_ => _.ModalComponent)
                .Select(c => c != null)
                .ToPropertyEx(this, _ => _.IsModalComponentVisible);
        }

        public IList<NavigationButtonItemViewModel> NavigationButtons { get; }

        private void SetActiveScreen<TViewModel>()
            where TViewModel : class, IRoutableViewModel
        {
            Router.NavigateAndReset.Execute(Locator.Current.GetService<TViewModel>()).Subscribe();
        }

        public RoutingState Router { get; }

        [Reactive] public ViewModelBase ModalComponent { get; set; }
        
        public bool IsModalComponentVisible { [ObservableAsProperty] get; }
    }

    public class ShowModalComponentMessage
    {
        public ShowModalComponentMessage(ViewModelBase viewModel)
        {
            ViewModel = viewModel;
        }
        
        public ViewModelBase ViewModel { get; }
    }

    public class HideModalComponentMessage
    {
        private HideModalComponentMessage() {}
        public static readonly HideModalComponentMessage Instance = new HideModalComponentMessage();
    }
}