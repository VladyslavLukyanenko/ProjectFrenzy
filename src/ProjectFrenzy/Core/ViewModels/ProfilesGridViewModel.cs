using System;
using System.Collections.ObjectModel;
using System.Reactive;
using DynamicData;
using ProjectFrenzy.Core.Model;
using ProjectFrenzy.Core.Services;
using ReactiveUI;

namespace ProjectFrenzy.Core.ViewModels
{
    public class ProfilesGridViewModel
        : ViewModelBase, IRoutableViewModel
    {
        private readonly ReadOnlyObservableCollection<Profile> _profiles;

        public ProfilesGridViewModel(IProfilesService profilesService, ProfileEditorViewModel profileEditor,
            IScreen hostScreen, IMessageBus messageBus)
        {
            HostScreen = hostScreen;
            profilesService.Profiles.Connect()
                .Bind(out _profiles)
                .DisposeMany()
                .Subscribe();


            EditProfileCommand = ReactiveCommand.Create<Profile>(p =>
            {
                profileEditor.Profile = p;
                messageBus.SendMessage(new ShowModalComponentMessage(profileEditor));
            });

            RemoveProfileCommand = ReactiveCommand.CreateFromTask<Profile>(async (p, ct) =>
            {
                await profilesService.RemoveAsync(p, ct);
            });

            CreateProfileCommand = ReactiveCommand.Create(() =>
            {
                profileEditor.Profile = new Profile();
                messageBus.SendMessage(new ShowModalComponentMessage(profileEditor));
            });
        }

        public ReadOnlyObservableCollection<Profile> Profiles => _profiles;

        public ReactiveCommand<Profile, Unit> EditProfileCommand { get; private set; }
        public ReactiveCommand<Profile, Unit> RemoveProfileCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> CreateProfileCommand { get; private set; }
        public string UrlPathSegment => nameof(ProfilesGridViewModel);
        public IScreen HostScreen { get; }
    }
}