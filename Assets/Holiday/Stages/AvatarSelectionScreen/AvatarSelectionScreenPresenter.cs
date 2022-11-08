namespace Extreal.SampleApp.Holiday.Stages.AvatarSelectionScreen
{
    using System.Linq;
    using App;
    using Cysharp.Threading.Tasks;
    using Models;
    using UniRx;
    using VContainer;
    using VContainer.Unity;

    public class AvatarSelectionScreenPresenter : IStartable
    {
        [Inject] private StageNavigator stageNavigator;

        [Inject] private AvatarSelectionScreenView avatarSelectionScreenView;

        [Inject] private Player player;

        [Inject] private IAvatarRepository avatarRepository;

        public void Start()
        {
            var avatars = avatarRepository.Avatars.Select(avatar => avatar.AvatarName.ToString()).ToList();
            avatarSelectionScreenView.Initialize(avatars);

            avatarSelectionScreenView.SetInitialValues(player.Name.Value, player.Avatar.Value.ToString());

            avatarSelectionScreenView.OnNameChanged.Subscribe(player.SetName);

            avatarSelectionScreenView.OnAvatarChanged.Subscribe(avatarName =>
            {
                var avatar = avatarRepository.Avatars.Find(avatar => avatar.AvatarName.ToString() == avatarName);
                player.SetAvatar(avatar.AvatarName);
            });

            avatarSelectionScreenView.OnGoButtonClicked.Subscribe(_ =>
            {
                stageNavigator.ReplaceAsync(StageName.VirtualSpace).Forget();
            });
        }
    }
}
