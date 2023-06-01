using System;
using Extreal.SampleApp.Holiday.App.Config;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.App.AppUsage
{
    public static class AppUsageUtils
    {
        public static (string Value, bool IsGenerated) GetClientId(AppUsageConfig appUsageConfig)
        {
            var hasKey = PlayerPrefs.HasKey(appUsageConfig.ClientIdKey);
            if (!hasKey)
            {
                var clientId = Guid.NewGuid().ToString();
                PlayerPrefs.SetString(appUsageConfig.ClientIdKey, clientId);
            }
            return (PlayerPrefs.GetString(appUsageConfig.ClientIdKey), !hasKey);
        }

        public static string ToJson(AppUsageBase appUsageBase, AppUsageConfig appUsageConfig, AppState appState)
            => ToJson(appUsageBase, GetClientId(appUsageConfig).Value, appState);

        public static string ToJson(AppUsageBase appUsageBase, string clientId, AppState appState)
        {
            appUsageBase.ClientId = clientId;
            appUsageBase.StageName = appState.StageState?.StageName.ToString();
            return JsonUtility.ToJson(appUsageBase);
        }
    }
}
