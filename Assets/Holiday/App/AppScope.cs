#if UNITY_IOS
using Cysharp.Threading.Tasks;
#endif
using Extreal.Core.Logging;
using Extreal.Core.StageNavigation;
using Extreal.Integration.Chat.Vivox;
using Extreal.Integration.Multiplay.NGO;
using Extreal.SampleApp.Holiday.App.Avatars;
using Extreal.SampleApp.Holiday.App.Config;
using Unity.Netcode;
using UnityEngine;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif
using VContainer;
using VContainer.Unity;
using LogLevel = Extreal.Core.Logging.LogLevel;

namespace Extreal.SampleApp.Holiday.App
{
    public class AppScope : LifetimeScope
    {
        [SerializeField] private StageConfig stageConfig;
        [SerializeField] private BuiltinAvatarRepository builtinAvatarRepository;
        [SerializeField] private MultiplayConfig multiplayConfig;
        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private ChatConfig chatConfig;

        private static void InitializeApp()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;

            var logLevel = InitializeLogging();
            InitializeMicrophone();

            var logger = LoggingManager.GetLogger(nameof(AppScope));
            if (logger.IsDebug())
            {
                logger.LogDebug($"targetFrameRage: {Application.targetFrameRate}, logLevel: {logLevel}");
            }
        }

        private static LogLevel InitializeLogging()
        {
#if HOLIDAY_PROD
            const LogLevel logLevel = LogLevel.Info;
#else
            const LogLevel logLevel = LogLevel.Debug;
#endif
            LoggingManager.Initialize(logLevel: logLevel);
            return logLevel;
        }

        private static void InitializeMicrophone()
        {
#if UNITY_IOS
            if (!Application.HasUserAuthorization(UserAuthorization.Microphone))
            {
                Application.RequestUserAuthorization(UserAuthorization.Microphone).ToUniTask().Forget();
            }
#endif

#if UNITY_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                Permission.RequestUserPermission(Permission.Microphone);
            }
#endif
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

            builder.RegisterComponent(multiplayConfig.ToNgoConfig());
            builder.RegisterComponent(networkManager);
            builder.Register<NgoClient>(Lifetime.Singleton);

            builder.RegisterComponent(chatConfig.ToVivoxAppConfig());
            builder.Register<VivoxClient>(Lifetime.Singleton);

            builder.Register<AppState>(Lifetime.Singleton);

            builder.RegisterEntryPoint<AppPresenter>();
        }
    }
}
