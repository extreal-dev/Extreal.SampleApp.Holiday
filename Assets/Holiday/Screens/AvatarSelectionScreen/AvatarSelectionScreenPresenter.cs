using System.Linq;
using Cysharp.Threading.Tasks;
using Extreal.Core.Logging;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;
using UniRx;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Screens.AvatarSelectionScreen
{
    public class AvatarSelectionScreenPresenter : IStartable
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(AvatarSelectionScreenPresenter));

        private readonly StageNavigator<StageName, SceneName> stageNavigator;

        private readonly AvatarSelectionScreenView avatarSelectionScreenView;

        private readonly AppState appState;

        public AvatarSelectionScreenPresenter(StageNavigator<StageName, SceneName> stageNavigator,
            AvatarSelectionScreenView avatarSelectionScreenView, AppState appState)
        {
            this.stageNavigator = stageNavigator;
            this.avatarSelectionScreenView = avatarSelectionScreenView;
            this.appState = appState;
        }

        public void Start()
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"player: name: {appState.PlayerName.Value} avatar: {appState.Avatar.Value.Name}");
            }

            var avatars = appState.Avatars.Select(avatar => avatar.Name).ToList();
            avatarSelectionScreenView.Initialize(avatars);

            avatarSelectionScreenView.SetInitialValues(appState.PlayerName.Value, appState.Avatar.Value.Name);

            avatarSelectionScreenView.OnNameChanged.Subscribe(appState.SetPlayerName);

            avatarSelectionScreenView.OnAvatarChanged.Subscribe(appState.SetAvatar);

            avatarSelectionScreenView.OnGoButtonClicked
                .Subscribe(_ => stageNavigator.ReplaceAsync(StageName.VirtualStage).Forget());
        }
    }
}
