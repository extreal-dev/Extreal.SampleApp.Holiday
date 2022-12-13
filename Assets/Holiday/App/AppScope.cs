using Extreal.Core.Logging;
using Extreal.Core.StageNavigation;
using Extreal.Integration.Chat.Vivox;
using Extreal.Integration.Multiplay.NGO;
using Unity.Netcode;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using LogLevel = Extreal.Core.Logging.LogLevel;

namespace Extreal.SampleApp.Holiday.App
{
    public class AppScope : LifetimeScope
    {
        [SerializeField] private StageConfig stageConfig;
        [SerializeField] private BuiltinAvatarRepository builtinAvatarRepository;
        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private VivoxAppConfig vivoxAppConfig;

        private static void InitializeApp()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;

            const LogLevel logLevel = LogLevel.Debug;
            LoggingManager.Initialize(logLevel: logLevel);

            var logger = LoggingManager.GetLogger(nameof(AppScope));
            if (logger.IsDebug())
            {
                logger.LogDebug($"targetFrameRage: {Application.targetFrameRate}, logLevel: {logLevel}");
            }
        }

        protected override void Awake()
        {
            InitializeApp();
            base.Awake();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(stageConfig).AsImplementedInterfaces();
            builder.Register<StageNavigator<StageName, SceneName>>(Lifetime.Singleton);

            builder.RegisterComponent(builtinAvatarRepository).AsImplementedInterfaces();
            builder.Register<AvatarService>(Lifetime.Singleton);

            builder.RegisterComponent(networkManager);
            builder.Register<NgoClient>(Lifetime.Singleton);

            builder.RegisterComponent(vivoxAppConfig).AsImplementedInterfaces();
            builder.Register<VivoxClient>(Lifetime.Singleton);

            builder.Register<AppState>(Lifetime.Singleton);

            builder.RegisterEntryPoint<AppPresenter>();
        }
    }
}
