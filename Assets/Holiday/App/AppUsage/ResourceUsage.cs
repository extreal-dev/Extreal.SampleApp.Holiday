using System.Diagnostics.CodeAnalysis;

namespace Extreal.SampleApp.Holiday.App.AppUsage
{
    [SuppressMessage("Usage", "IDE1006")]
    public class ResourceUsage : AppUsageBase
    {
        public long TotalReservedMemoryMb;
        public long TotalAllocatedMemoryMb;
        public long MonoHeapSizeMb;
        public long MonoUsedSizeMb;
    }
}
