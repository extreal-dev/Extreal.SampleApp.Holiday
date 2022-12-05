using System.Linq;
using Cysharp.Threading.Tasks;
using Extreal.Core.Logging;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.MultiplayClient.App;
using Extreal.SampleApp.Holiday.MultiplayClient.Models;
using UniRx;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.MultiplayClient.Screens.AvatarSelectionScreen
{
    public class AvatarSelectionScreenPresenter : IStartable
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(AvatarSelectionScreenPresenter));

        private readonly StageNavigator<StageName, SceneName> stageNavigator;

        private readonly AvatarSelectionScreenView avatarSelectionScreenView;

        private readonly AppState player;

        public AvatarSelectionScreenPresenter(StageNavigator<StageName, SceneName> stageNavigator,
            AvatarSelectionScreenView avatarSelectionScreenView, AppState player)
        {
            this.stageNavigator = stageNavigator;
            this.avatarSelectionScreenView = avatarSelectionScreenView;
            this.player = player;
        }

        public void Start()
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"player: name: {player.PlayerName.Value} avatar: {player.Avatar.Value.Name}");
            }

            var avatars = player.Avatars.Select(avatar => avatar.Name).ToList();
            avatarSelectionScreenView.Initialize(avatars);

            avatarSelectionScreenView.SetInitialValues(player.PlayerName.Value, player.Avatar.Value.Name);

            avatarSelectionScreenView.OnNameChanged.Subscribe(player.SetPlayerName);

            avatarSelectionScreenView.OnAvatarChanged.Subscribe(player.SetAvatar);

            avatarSelectionScreenView.OnGoButtonClicked
                .Subscribe(_ => stageNavigator.ReplaceAsync(StageName.VirtualStage).Forget());
        }
    }
}
