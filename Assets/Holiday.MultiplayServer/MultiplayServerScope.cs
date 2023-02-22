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
        [SerializeField] private GameObject playerPrefab;

        private static void InitializeApp()
        {
#if HOLIDAY_PROD
            const LogLevel logLevel = LogLevel.Info;
#else
            const LogLevel logLevel = LogLevel.Debug;
#endif
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
            builder.RegisterComponent(playerPrefab);
            builder.Register<NgoServer>(Lifetime.Singleton);
            builder.Register<MultiplayServer>(Lifetime.Singleton);

            builder.RegisterEntryPoint<MultiplayServerPresenter>();
        }
    }
}
