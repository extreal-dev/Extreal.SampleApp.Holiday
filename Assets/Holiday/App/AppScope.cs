#if UNITY_IOS
using Cysharp.Threading.Tasks;
#endif
using System.Diagnostics.CodeAnalysis;
using Extreal.Core.Logging;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App.AppUsage;
using Extreal.SampleApp.Holiday.App.AppUsage.Collectors;
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
        [SerializeField] private AppConfig appConfig;
        [SerializeField] private LoggingConfig loggingConfig;
        [SerializeField] private StageConfig stageConfig;
        [SerializeField] private AppUsageConfig appUsageConfig;

        private void InitializeApp()
        {
            QualitySettings.vSyncCount = appConfig.VerticalSyncs;
            Application.targetFrameRate = appConfig.TargetFrameRate;
            var timeout = appConfig.DownloadTimeoutSeconds;
            Addressables.ResourceManager.WebRequestOverride = unityWebRequest => unityWebRequest.timeout = timeout;

            ClearCacheOnDev();

            var logLevel = InitializeLogging();
            InitializeMicrophone();

            var logger = LoggingManager.GetLogger(nameof(AppScope));
            if (logger.IsDebug())
            {
                logger.LogDebug(
                    $"targetFrameRate: {Application.targetFrameRate}, unityWebRequest.timeout: {timeout}, logLevel: {logLevel}");
            }
        }

        private LogLevel InitializeLogging()
        {
#if HOLIDAY_PROD
            const LogLevel logLevel = LogLevel.Info;
            LoggingManager.Initialize(logLevel: logLevel, writer: new AppUsageLogWriter(appUsageConfig, appStateProvider));
#else
            const LogLevel logLevel = LogLevel.Debug;
            var checker = new LogLevelLogOutputChecker(loggingConfig.CategoryFilters);
            var defaultWriter = new UnityDebugLogWriter(loggingConfig.LogFormats);
            var writer = new AppUsageLogWriter(appUsageConfig, appStateProvider, defaultWriter);
            LoggingManager.Initialize(logLevel, checker, writer);
#endif
            return logLevel;
        }

        private readonly AppStateProvider appStateProvider = new AppStateProvider();

        // The provider is added to pass AppState to LogWriter. AppState gets the logger to output logs.
        // Therefore, if AppState is created before log output is initialized,
        // only AppState acquires the logger before initialization, resulting in inconsistency.
        // In order to resolve this issue, the provider is introduced and AppState is passed to LogWriter
        // while delaying the timing of AppState creation.
        public class AppStateProvider
        {
            public AppState AppState { get; private set; }
            internal AppStateProvider() { }
            internal void Init() => AppState = new AppState();
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

        [SuppressMessage("Design", "IDE0022"), SuppressMessage("Design", "CC0091")]
        private void ClearCacheOnDev()
        {
#if !HOLIDAY_PROD
            Caching.ClearCache();
            PlayerPrefs.DeleteKey(appUsageConfig.ClientIdKey);
#endif
        }

        protected override void Awake()
        {
            InitializeApp();
            base.Awake();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(appConfig);

            builder.RegisterComponent(stageConfig).AsImplementedInterfaces();
            builder.Register<StageNavigator<StageName, SceneName>>(Lifetime.Singleton);

            appStateProvider.Init();
            builder.RegisterComponent(appStateProvider.AppState);

            builder.Register<AssetHelper>(Lifetime.Singleton);

            builder.RegisterComponent(appUsageConfig);
            builder.Register<AppUsageEmitter>(Lifetime.Singleton);
            builder.Register<FirstUseCollector>(Lifetime.Singleton).As<IAppUsageCollector>();
            builder.Register<StageUsageCollector>(Lifetime.Singleton).As<IAppUsageCollector>();
            builder.Register<ResourceUsageCollector>(Lifetime.Singleton).As<IAppUsageCollector>();
            builder.Register<ErrorStatusCollector>(Lifetime.Singleton).As<IAppUsageCollector>();
            builder.Register<AppUsageManager>(Lifetime.Singleton);

            builder.RegisterEntryPoint<AppPresenter>();
        }
    }
}
