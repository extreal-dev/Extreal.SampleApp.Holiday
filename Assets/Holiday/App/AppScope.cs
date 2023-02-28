#if UNITY_IOS
using Cysharp.Threading.Tasks;
#endif
using Extreal.Core.Logging;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App.Common;
using Extreal.SampleApp.Holiday.App.Config;
using UnityEngine;
using VContainer;
using VContainer.Unity;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

namespace Extreal.SampleApp.Holiday.App
{
    public class AppScope : LifetimeScope
    {
        [SerializeField] private StageConfig stageConfig;

        private static void InitializeApp()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;

            var logLevel = InitializeLogging();
            InitializeMicrophone();
            InitializeAssets();

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

        private static void InitializeAssets()
        {
#if !HOLIDAY_PROD
            Caching.ClearCache();
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

            builder.Register<AppState>(Lifetime.Singleton);

            builder.Register<AssetProvider>(Lifetime.Singleton);
            builder.Register<AssetDownloadHelper>(Lifetime.Singleton);

            builder.RegisterEntryPoint<AppPresenter>();
        }
    }
}
