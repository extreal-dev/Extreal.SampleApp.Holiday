using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.MultiplayServer
{
    public class MultiplayServerPresenter : IInitializable, IStartable
    {
        private readonly MultiplayServer multiplayServer;

        public MultiplayServerPresenter(MultiplayServer multiplayServer)
            => this.multiplayServer = multiplayServer;

        public void Initialize()
            => multiplayServer.Initialize();

        public void Start()
            => multiplayServer.StartAsync().Forget();
    }
}
