using System.Linq;
using Cysharp.Threading.Tasks;
using Extreal.Core.Logging;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;
using UniRx;

namespace Extreal.SampleApp.Holiday.Screens.AvatarSelectionScreen
{
    public class AvatarSelectionScreenPresenter : StagePresenterBase
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(AvatarSelectionScreenPresenter));

        private readonly AvatarService avatarService;
        private readonly AvatarSelectionScreenView avatarSelectionScreenView;
        private readonly AppState appState;

        public AvatarSelectionScreenPresenter
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            AvatarService avatarService,
            AvatarSelectionScreenView avatarSelectionScreenView,
            AppState appState
        ) : base(stageNavigator)
        {
            this.avatarSelectionScreenView = avatarSelectionScreenView;
            this.avatarService = avatarService;
            this.appState = appState;
        }

        protected override void Initialize(
            StageNavigator<StageName, SceneName> stageNavigator, CompositeDisposable sceneDisposables)
        {
            avatarSelectionScreenView.OnNameChanged
                .Subscribe(appState.SetPlayerName)
                .AddTo(sceneDisposables);

            avatarSelectionScreenView.OnAvatarChanged
                .Subscribe(avatarName =>
                {
                    var avatar = avatarService.FindAvatarByName(avatarName);
                    appState.SetAvatar(avatar);
                })
                .AddTo(sceneDisposables);

            avatarSelectionScreenView.OnGoButtonClicked
                .Subscribe(_ => stageNavigator.ReplaceAsync(StageName.VirtualStage).Forget())
                .AddTo(sceneDisposables);
        }

        protected override void OnStageEntered(StageName stageName, CompositeDisposable stageDisposables)
        {
            var avatars = avatarService.Avatars;
            if (appState.Avatar.Value == null)
            {
                appState.SetAvatar(avatars.First());
            }

            var avatarNames = avatars.Select(avatar => avatar.Name).ToList();
            avatarSelectionScreenView.Initialize(avatarNames);

            avatarSelectionScreenView.SetInitialValues(appState.PlayerName.Value, appState.Avatar.Value.Name);

            if (Logger.IsDebug())
            {
                Logger.LogDebug($"player: name: {appState.PlayerName.Value} avatar: {appState.Avatar.Value.Name}");
            }
        }

        protected override void OnStageExiting(StageName stageName)
        {
        }
    }
}
