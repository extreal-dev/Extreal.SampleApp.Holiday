using Extreal.Core.Logging;
using Extreal.Core.StageNavigation;
using Extreal.Integration.Chat.Vivox;
using Extreal.Integration.Multiplay.NGO;
using Extreal.SampleApp.Holiday.Models.ScriptableObject;
using Unity.Netcode;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.App
{
    public class AppScope : LifetimeScope
    {
        [SerializeField] private StageConfig stageConfig;
        [SerializeField] private BuiltinAvatarRepository builtinAvatarRepository;
        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private VivoxAppConfig appConfig;

        private static void InitializeApp()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;

            const Core.Logging.LogLevel logLevel = Core.Logging.LogLevel.Debug;
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
            builder.RegisterComponent(builtinAvatarRepository).AsImplementedInterfaces();
            builder.RegisterComponent(networkManager);
            builder.RegisterComponent(appConfig);

            builder.Register<StageNavigator<StageName, SceneName>>(Lifetime.Singleton);
            builder.RegisterEntryPoint<AppState>(Lifetime.Singleton).AsSelf();
            builder.Register<NgoClient>(Lifetime.Singleton);
            builder.Register<VivoxClient>(Lifetime.Singleton);

            builder.RegisterEntryPoint<AppPresenter>();
        }
    }
}
