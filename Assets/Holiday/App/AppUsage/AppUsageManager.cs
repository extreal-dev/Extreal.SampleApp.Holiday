using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Extreal.Core.Common.System;
using Extreal.Core.Logging;
using Extreal.SampleApp.Holiday.App.Config;
using UniRx;

namespace Extreal.SampleApp.Holiday.App.AppUsage
{
    public class AppUsageManager : DisposableBase
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(AppUsage));

        [SuppressMessage("Usage", "CC0033")]
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        private readonly AppState appState;
        private readonly AppUsageConfig appUsageConfig;
        private readonly AppUsageEmitter appUsageEmitter;
        private readonly IEnumerable<IAppUsageCollector> appUsageCollectors;

        public AppUsageManager(
            AppState appState,
            AppUsageConfig appUsageConfig,
            AppUsageEmitter appUsageEmitter,
            IEnumerable<IAppUsageCollector> appUsageCollectors)
        {
            this.appState = appState;
            this.appUsageConfig = appUsageConfig;
            this.appUsageEmitter = appUsageEmitter;
            this.appUsageCollectors = appUsageCollectors;
        }

        protected override void ReleaseManagedResources() => disposables.Dispose();

        public void CollectAppUsage()
        {
            if (!appUsageConfig.Enable)
            {
                return;
            }

            appUsageEmitter.Handle();

            foreach (var appUsageCollector in appUsageCollectors)
            {
                disposables.Add(appUsageCollector.Collect(Collect));
            }
        }

        private void Collect(AppUsageBase appUsageBase)
            => Logger.LogInfo(AppUsageUtils.ToJson(appUsageBase, GetClientId(), appState));

        private string GetClientId()
        {
            var clientId = AppUsageUtils.GetClientId(appUsageConfig);
            if (clientId.IsGenerated)
            {
                appUsageEmitter.PlayForFirstTime();
            }

            return clientId.Value;
        }
    }
}
