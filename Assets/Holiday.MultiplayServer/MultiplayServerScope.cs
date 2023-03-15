using Extreal.Core.Logging;
using Extreal.Integration.Multiplay.NGO;
using Extreal.SampleApp.Holiday.Common.Config;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using LogLevel = Extreal.Core.Logging.LogLevel;

namespace Extreal.SampleApp.Holiday.MultiplayServer
{
    public class MultiplayServerScope : LifetimeScope
    {
        [SerializeField] private LoggingConfig loggingConfig;
        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private GameObject playerPrefab;

        private void InitializeApp()
        {
#if HOLIDAY_PROD
            LoggingManager.Initialize();
#else
            var checker = new LogLevelLogOutputChecker(loggingConfig.CategoryFilters);
            var writer = new UnityDebugLogWriter(loggingConfig.LogFormats);
            LoggingManager.Initialize(LogLevel.Debug, checker, writer);
#endif
            InitializeNetworkManager();
        }

        private void InitializeNetworkManager()
        {
            if (networkManager.NetworkConfig.NetworkTransport is UnityTransport unityTransport)
            {
                unityTransport.ConnectionData.Port = MultiplayServerArgumentHandler.Port;
            }
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
