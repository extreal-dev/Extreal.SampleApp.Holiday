using Extreal.Core.Logging;
using Extreal.Integration.Multiplay.NGO;
using Unity.Netcode;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using LogLevel = Extreal.Core.Logging.LogLevel;

namespace Extreal.SampleApp.Holiday.MultiplayServer
{
    public class MultiplayServerScope : LifetimeScope
    {
        [SerializeField] private NetworkManager networkManager;

        private static void InitializeApp()
        {
            const LogLevel logLevel = LogLevel.Debug;
            LoggingManager.Initialize(logLevel: logLevel);
        }

        protected override void Awake()
        {
            InitializeApp();
            base.Awake();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(networkManager);
            builder.Register<NgoServer>(Lifetime.Singleton);

            builder.RegisterEntryPoint<MultiplayServer>().AsSelf();
            builder.RegisterEntryPoint<MultiplayServerPresenter>();
        }
    }
}
