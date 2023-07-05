using System.Linq;
using Extreal.Core.Logging;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.AssetWorkflow;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.App.Stages;
using UniRx;

namespace Extreal.SampleApp.Holiday.Screens.AvatarSelectionScreen
{
    public class AvatarSelectionScreenPresenter : StagePresenterBase
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(AvatarSelectionScreenPresenter));

        private readonly AssetHelper assetHelper;
        private readonly AvatarSelectionScreenView avatarSelectionScreenView;

        public AvatarSelectionScreenPresenter
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            AssetHelper assetHelper,
            AvatarSelectionScreenView avatarSelectionScreenView
        ) : base(stageNavigator, appState)
        {
            this.assetHelper = assetHelper;
            this.avatarSelectionScreenView = avatarSelectionScreenView;
        }

        protected override void Initialize(
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            CompositeDisposable sceneDisposables)
        {
            avatarSelectionScreenView.OnNameChanged
                .Subscribe(appState.SetPlayerName)
                .AddTo(sceneDisposables);

            avatarSelectionScreenView.OnAvatarChanged
                .Subscribe(avatarName =>
                {
                    var avatar = assetHelper.AvatarConfig.Avatars.First(avatar => avatar.Name == avatarName);
                    appState.SetAvatar(avatar);
                })
                .AddTo(sceneDisposables);

            avatarSelectionScreenView.OnGoButtonClicked
                .Subscribe(_ => stageNavigator.ReplaceAsync(StageName.GroupSelectionStage))
                .AddTo(sceneDisposables);
        }

        protected override void OnStageEntered(
            StageName stageName,
            AppState appState,
            CompositeDisposable stageDisposables)
        {
            var avatars = assetHelper.AvatarConfig.Avatars;
            if (appState.Avatar == null)
            {
                appState.SetAvatar(avatars.First());
            }

            var avatarNames = avatars.Select(avatar => avatar.Name).ToList();
            avatarSelectionScreenView.Initialize(avatarNames);

            avatarSelectionScreenView.SetInitialValues(appState.PlayerName, appState.Avatar.Name);

            if (Logger.IsDebug())
            {
                Logger.LogDebug($"player: name: {appState.PlayerName} avatar: {appState.Avatar.Name}");
            }
        }
    }
}
