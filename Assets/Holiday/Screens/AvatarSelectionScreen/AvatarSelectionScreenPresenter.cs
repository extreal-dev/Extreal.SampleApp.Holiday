using System.Linq;
using Cysharp.Threading.Tasks;
using Extreal.Core.Logging;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.Avatars;
using Extreal.SampleApp.Holiday.App.Common;
using Extreal.SampleApp.Holiday.App.Config;
using UniRx;

namespace Extreal.SampleApp.Holiday.Screens.AvatarSelectionScreen
{
    public class AvatarSelectionScreenPresenter : StagePresenterBase
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(AvatarSelectionScreenPresenter));

        private readonly AvatarSelectionScreenView avatarSelectionScreenView;
        private readonly AppState appState;
        private readonly AssetProvider assetProvider;

        public AvatarSelectionScreenPresenter
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            AvatarSelectionScreenView avatarSelectionScreenView,
            AppState appState,
            AssetProvider assetProvider
        ) : base(stageNavigator)
        {
            this.avatarSelectionScreenView = avatarSelectionScreenView;
            this.appState = appState;
            this.assetProvider = assetProvider;
        }

        protected override async void Initialize(
            StageNavigator<StageName, SceneName> stageNavigator, CompositeDisposable sceneDisposables)
        {
            avatarSelectionScreenView.OnNameChanged
                .Subscribe(appState.SetPlayerName)
                .AddTo(sceneDisposables);

            var avatarService = (await assetProvider.LoadAssetAsync<AvatarRepository>(nameof(AvatarRepository))).ToAvatarService();

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

        protected override async void OnStageEntered(StageName stageName, CompositeDisposable stageDisposables)
        {
            var avatarService = (await assetProvider.LoadAssetAsync<AvatarRepository>(nameof(AvatarRepository))).ToAvatarService();
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
