using System.Linq;
using Cysharp.Threading.Tasks;
using Extreal.Core.Logging;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.Avatars;
using UniRx;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Screens.AvatarSelectionScreen
{
    public class AvatarSelectionScreenPresenter : IStartable
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(AvatarSelectionScreenPresenter));

        private readonly StageNavigator<StageName, SceneName> stageNavigator;
        private readonly AvatarSelectionScreenView avatarSelectionScreenView;
        private readonly AvatarService avatarService;
        private readonly AppState appState;

        public AvatarSelectionScreenPresenter
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            AvatarSelectionScreenView avatarSelectionScreenView,
            AvatarService avatarService,
            AppState appState
        )
        {
            this.stageNavigator = stageNavigator;
            this.avatarSelectionScreenView = avatarSelectionScreenView;
            this.avatarService = avatarService;
            this.appState = appState;
        }

        public void Start()
        {
            var avatars = avatarService.Avatars;
            if (appState.Avatar.Value == null)
            {
                appState.SetAvatar(avatars.First());
            }

            if (Logger.IsDebug())
            {
                Logger.LogDebug($"player: name: {appState.PlayerName.Value} avatar: {appState.Avatar.Value.Name}");
            }

            var avatarNames = avatars.Select(avatar => avatar.Name).ToList();
            avatarSelectionScreenView.Initialize(avatarNames);

            avatarSelectionScreenView.SetInitialValues(appState.PlayerName.Value, appState.Avatar.Value.Name);

            avatarSelectionScreenView.OnNameChanged.Subscribe(appState.SetPlayerName);

            avatarSelectionScreenView.OnAvatarChanged
                .Subscribe(avatarName =>
                {
                    var avatar = avatarService.FindAvatarByName(avatarName);
                    appState.SetAvatar(avatar);
                });

            avatarSelectionScreenView.OnGoButtonClicked
                .Subscribe(_ => stageNavigator.ReplaceAsync(StageName.VirtualStage).Forget());
        }
    }
}
