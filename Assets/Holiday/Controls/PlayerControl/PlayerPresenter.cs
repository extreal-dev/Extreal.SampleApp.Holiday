namespace Extreal.SampleApp.Holiday.Holiday.Controls.PlayerControl
{
    using Core.Logging;
    using Cysharp.Threading.Tasks;
    using Models;
    using VContainer;
    using VContainer.Unity;

    public class PlayerPresenter : IStartable
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(PlayerPresenter));

        [Inject] private Player player;

        public void Start() => player.CreateAsync().Forget();
    }
}
