namespace Extreal.SampleApp.Holiday.App
{
    using Core.Logging;
    using UnityEngine;

    public static class AppInitializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;

            const LogLevel logLevel = LogLevel.Debug;
            LoggingManager.Initialize(logLevel: logLevel);

            var logger = LoggingManager.GetLogger(nameof(AppInitializer));
            if (logger.IsDebug())
            {
                logger.LogDebug($"targetFrameRage: {Application.targetFrameRate}, logLevel: {logLevel}");
            }
        }
    }
}
