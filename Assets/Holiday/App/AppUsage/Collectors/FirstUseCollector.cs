using System;
using System.Diagnostics.CodeAnalysis;
using Extreal.Core.Common.Hook;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.App.AppUsage.Collectors
{
    public class FirstUseCollector : IAppUsageCollector
    {
        public IDisposable Collect(AppUsageManager appUsageManager) =>
            appUsageManager.OnFirstUsed
                .Hook(clientId =>
                    appUsageManager.Collect(
                        new FirstUse
                        {
                            UsageId = nameof(FirstUse),
                            OS = SystemInfo.operatingSystem,
                            DeviceModel = SystemInfo.deviceModel,
                            DeviceType = SystemInfo.deviceType.ToString(),
                            DeviceId = SystemInfo.deviceUniqueIdentifier,
                            ProcessorType = SystemInfo.processorType
                        },
                        clientId));
    }

    [SuppressMessage("Usage", "IDE1006")]
    public class FirstUse : AppUsageBase
    {
        public string OS;
        public string DeviceModel;
        public string DeviceType;
        public string DeviceId;
        public string ProcessorType;
    }
}
