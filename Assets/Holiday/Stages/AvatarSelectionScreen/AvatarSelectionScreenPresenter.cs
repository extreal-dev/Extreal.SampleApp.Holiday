using System.Linq;
using Cysharp.Threading.Tasks;
using Extreal.Core.Logging;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.Models;
using UniRx;
using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Stages.AvatarSelectionScreen
{
    public class AvatarSelectionScreenPresenter : IStartable
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(AvatarSelectionScreenPresenter));

        [Inject] private IStageNavigator<StageName> stageNavigator;

        [Inject] private AvatarSelectionScreenView avatarSelectionScreenView;

        [Inject] private Player player;

        public void Start()
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"player: name: {player.Name.Value} avatar: {player.Avatar.Value.Name}");
            }

            var avatars = player.Avatars.Select(avatar => avatar.Name).ToList();
            avatarSelectionScreenView.Initialize(avatars);

            avatarSelectionScreenView.SetInitialValues(player.Name.Value, player.Avatar.Value.Name);

            avatarSelectionScreenView.OnNameChanged.Subscribe(player.SetName);

            avatarSelectionScreenView.OnAvatarChanged.Subscribe(player.SetAvatar);

            avatarSelectionScreenView.OnGoButtonClicked.Subscribe(_ =>
            {
                stageNavigator.ReplaceAsync(StageName.VirtualSpace).Forget();
            });
        }
    }
}
