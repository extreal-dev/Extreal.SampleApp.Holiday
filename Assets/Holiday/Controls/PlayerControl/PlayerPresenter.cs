namespace Extreal.SampleApp.Holiday.Holiday.Controls.PlayerControl
{
    using Cysharp.Threading.Tasks;
    using Models;
    using VContainer;
    using VContainer.Unity;

    public class PlayerPresenter : IStartable
    {
        [Inject] private Player player;

        public void Start()
        {
            player.SpawnAsync().Forget();
        }
    }
}
