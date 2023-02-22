using System.Linq;
using Cysharp.Threading.Tasks;
using Extreal.Core.Logging;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.Common;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.App.Data;
using UniRx;

namespace Extreal.SampleApp.Holiday.Screens.AvatarSelectionScreen
{
    public class AvatarSelectionScreenPresenter : StagePresenterBase
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(AvatarSelectionScreenPresenter));

        private readonly AvatarSelectionScreenView avatarSelectionScreenView;
        private readonly AppState appState;
        private readonly DataRepository dataRepository;

        public AvatarSelectionScreenPresenter
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            AvatarSelectionScreenView avatarSelectionScreenView,
            AppState appState,
            DataRepository dataRepository
        ) : base(stageNavigator)
        {
            this.avatarSelectionScreenView = avatarSelectionScreenView;
            this.appState = appState;
            this.dataRepository = dataRepository;
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
                    var avatar = dataRepository.AvatarService.FindAvatarByName(avatarName);
                    appState.SetAvatar(avatar);
                })
                .AddTo(sceneDisposables);

            avatarSelectionScreenView.OnGoButtonClicked
                .Subscribe(_ => stageNavigator.ReplaceAsync(StageName.VirtualStage).Forget())
                .AddTo(sceneDisposables);
        }

        protected override void OnStageEntered(StageName stageName, CompositeDisposable stageDisposables)
        {
            var avatars = dataRepository.AvatarService.Avatars;
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
