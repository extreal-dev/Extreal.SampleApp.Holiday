using System;
using System.Diagnostics.CodeAnalysis;
using Extreal.Core.Common.Hook;
using Extreal.SampleApp.Holiday.App.Config;
using UniRx;
using UnityEngine.Profiling;

namespace Extreal.SampleApp.Holiday.App.AppUsage.Collectors
{
    public class ResourceUsageCollector : IAppUsageCollector
    {
        private readonly AppUsageConfig appUsageConfig;

        public ResourceUsageCollector(AppUsageConfig appUsageConfig) => this.appUsageConfig = appUsageConfig;

        public IDisposable Collect(Action<AppUsageBase> collect) =>
            Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(appUsageConfig.ResourceUsageCollectPeriodSeconds))
                .Hook(_ => collect?.Invoke(
                    new ResourceUsage
                    {
                        UsageId = nameof(ResourceUsage),
                        TotalReservedMemoryMb = AppUtils.ToMb(Profiler.GetTotalReservedMemoryLong()),
                        TotalAllocatedMemoryMb = AppUtils.ToMb(Profiler.GetTotalAllocatedMemoryLong()),
                        MonoHeapSizeMb = AppUtils.ToMb(Profiler.GetMonoHeapSizeLong()),
                        MonoUsedSizeMb = AppUtils.ToMb(Profiler.GetMonoUsedSizeLong())
                    }));

        [SuppressMessage("Usage", "IDE1006")]
        public class ResourceUsage : AppUsageBase
        {
            public long TotalReservedMemoryMb;
            public long TotalAllocatedMemoryMb;
            public long MonoHeapSizeMb;
            public long MonoUsedSizeMb;
        }
    }
}
