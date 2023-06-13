using System;
using System.Diagnostics.CodeAnalysis;
using Extreal.Core.Common.Hook;
using UniRx;
using UnityEngine.Profiling;

namespace Extreal.SampleApp.Holiday.App.AppUsage.Collectors
{
    public class ResourceUsageCollector : IAppUsageCollector
    {
        public IDisposable Collect(AppUsageManager appUsageManager)
        {
            var period = appUsageManager.AppUsageConfig.ResourceUsageCollectPeriodSeconds;
            return Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(period))
                .Hook(_ => appUsageManager.Collect(new ResourceUsage
                {
                    UsageId = nameof(ResourceUsage),
                    TotalReservedMemoryMb = AppUtils.ToMb(Profiler.GetTotalReservedMemoryLong()),
                    TotalAllocatedMemoryMb = AppUtils.ToMb(Profiler.GetTotalAllocatedMemoryLong()),
                    MonoHeapSizeMb = AppUtils.ToMb(Profiler.GetMonoHeapSizeLong()),
                    MonoUsedSizeMb = AppUtils.ToMb(Profiler.GetMonoUsedSizeLong())
                }));
        }

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
