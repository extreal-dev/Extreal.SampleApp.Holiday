namespace Extreal.SampleApp.Holiday.Holiday.Stages.VirtualSpace
{
    using Core.Logging;
    using Models;
    using VContainer;
    using VContainer.Unity;

    public class VirtualSpaceProvider : IStartable
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(VirtualSpaceProvider));

        [Inject] private Player player;

        public void Start()
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"player name: {player.Name} avatar: {player.Avatar}");
            }
        }
    }
}
