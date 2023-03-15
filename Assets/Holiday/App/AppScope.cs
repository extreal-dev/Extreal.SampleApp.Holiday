#if UNITY_IOS
using Cysharp.Threading.Tasks;
#endif
using System.Diagnostics.CodeAnalysis;
using Extreal.Core.Common.Retry;
using Extreal.Core.Logging;
using Extreal.Core.StageNavigation;
using Extreal.Integration.AssetWorkflow.Addressables;
using Extreal.SampleApp.Holiday.App.AssetWorkflow;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.Common.Config;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;
using VContainer.Unity;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

namespace Extreal.SampleApp.Holiday.App
{
    public class AppScope : LifetimeScope
    {
        [SerializeField] private LoggingConfig loggingConfig;
        [SerializeField] private StageConfig stageConfig;

        private void InitializeApp()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
            const int timeout = 5;
            Addressables.ResourceManager.WebRequestOverride = unityWebRequest => unityWebRequest.timeout = timeout;

            ClearAssetBundleCacheOnDev();

            var logLevel = InitializeLogging();
            InitializeMicrophone();

            var logger = LoggingManager.GetLogger(nameof(AppScope));
            if (logger.IsDebug())
            {
                logger.LogDebug($"targetFrameRate: {Application.targetFrameRate}, unityWebRequest.timeout: {timeout}, logLevel: {logLevel}");
            }
        }

        private LogLevel InitializeLogging()
        {
#if HOLIDAY_PROD
            const LogLevel logLevel = LogLevel.Info;
            LoggingManager.Initialize(logLevel: logLevel);
#else
            const LogLevel logLevel = LogLevel.Debug;
            var checker = new LogLevelLogOutputChecker(loggingConfig.CategoryFilters);
            var writer = new UnityDebugLogWriter(loggingConfig.LogFormats);
            LoggingManager.Initialize(logLevel, checker, writer);
#endif
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

        [SuppressMessage("Design", "IDE0022")]
        private static void ClearAssetBundleCacheOnDev()
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

            builder.Register<AssetProvider>(Lifetime.Singleton)
                .WithParameter<IRetryStrategy>(new CountingRetryStrategy());
            builder.Register<AssetHelper>(Lifetime.Singleton);

            builder.RegisterEntryPoint<AppPresenter>();
        }
    }
}
